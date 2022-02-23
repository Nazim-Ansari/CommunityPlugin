
using EllieMae.EMLite.ClientServer.Query;
using EllieMae.EMLite.ClientServer.Reporting;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Reporting;
using EllieMae.Encompass.Automation;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;

namespace CommunityPlugin.Objects.Factories
{
    public class EmailFactory : IFactory
    {
        private int Seconds = 60;

        /// <summary>
        /// Every minute, check to see if there are any email triggers to process. 
        /// Always run any Field Triggers then delete them from the config.
        /// </summary>
        /// <returns></returns>
        public List<ITask> GetTriggers()
        {
            AutoMailerCDO cdo = (AutoMailerCDO)Global.CDOs[nameof(AutoMailerCDO)];
            List<MailTrigger> Triggers = cdo.Triggers; 

            List<ITask> result = new List<ITask>();

            DateTime Now = DateTime.Now;
            foreach (MailTrigger trigger in Triggers.Where(x => x.Active))
            {
                bool field = trigger.TriggerType == Enums.MailTriggerType.Field;
                bool run = field;
                if (!run)
                {
                    bool onTime = trigger.Time.Hour.Equals(Now.Hour) && trigger.Time.Minute.Equals(Now.Minute) && Math.Abs(trigger.Time.Second - Now.Second) < Seconds;
                    if (!onTime)
                        continue;

                    switch (trigger.Frequency)
                    {
                        case Enums.FrequencyType.Daily:
                        case Enums.FrequencyType.Weekly:
                        case Enums.FrequencyType.BiWeekly:
                            run = DaysOfWeek(trigger.Days).Contains(Now.DayOfWeek.ToString());
                            break;
                        case Enums.FrequencyType.Monthly:
                            run = Now.Day.Equals(trigger.Date.Day);
                            break;
                        case Enums.FrequencyType.Yearly:
                            run = Now.Day.Equals(trigger.Date.Day) && Now.Month.Equals(trigger.Date.Month);
                            break;
                    }
                }

                if (run)
                    result.Add(trigger);
            }

            return result;
        }

        private List<string> DaysOfWeek(int[] Days)
        {
            List<string> results = new List<string>();

            if (Days.Contains(0))
                results.Add("Monday");
            if (Days.Contains(1))
                results.Add("Tuesday");
            if (Days.Contains(2))
                results.Add("Wednesday");
            if (Days.Contains(3))
                results.Add("Thursday");
            if (Days.Contains(4))
                results.Add("Friday");
            if (Days.Contains(5))
                results.Add("Saturday");
            if (Days.Contains(6))
                results.Add("Sunday");

            return results;
        }

        public static void Run(MailTrigger Trigger)
        {
            GetGuidsFromReport(Trigger);

            AutoMailerCDO cdo = (AutoMailerCDO)Global.CDOs[nameof(AutoMailerCDO)];
            if (Trigger.TriggerType == Enums.MailTriggerType.Field)
                cdo.Triggers.Remove(Trigger);
        }

        private static void GetGuidsFromReport(MailTrigger Trigger)
        {
            string folder = "\\AutoMailer\\";
            FileSystemEntry fileSystemEntry = new FileSystemEntry(folder, FileSystemEntry.Types.Folder, (string)null);
            Sessions.Session defaultInstance = Session.DefaultInstance;
            FSExplorer rptExplorer = new FSExplorer(defaultInstance);
            ReportMainControl r = new ReportMainControl(defaultInstance, false);
            ReportIFSExplorer ifsExplorer = new ReportIFSExplorer(r, defaultInstance);


            FileSystemEntry report = ifsExplorer.GetFileSystemEntries(fileSystemEntry).Where(x => x.Name.Equals(Trigger.ReportFilter)).FirstOrDefault();
            ReportSettings reportSettings = Session.DefaultInstance.ReportManager.GetReportSettings(report);

            LoanReportParameters reportParams1 = new LoanReportParameters();
            reportParams1.Fields.AddRange((IEnumerable<ColumnInfo>)reportSettings.Columns);
            reportParams1.FieldFilters.AddRange((IEnumerable<FieldFilter>)reportSettings.Filters);
            reportParams1.UseDBField = reportSettings.UseFieldInDB;
            reportParams1.UseDBFilter = reportSettings.UseFilterFieldInDB;
            reportParams1.UseExternalOrganization = reportSettings.ForTPO;
            reportParams1.CustomFilter = CreateLoanCustomFilter(reportSettings);
            ReportResults results = Session.DefaultInstance.ReportManager.QueryLoansForReport(reportParams1, null);


            List<string[]> reportResults = ReportResults.Download(results);
            List<string> guids = results.GetAllResults().Select(x => x.FirstOrDefault()).ToList();

            bool fieldsAreGuids = Guid.TryParse(guids.FirstOrDefault(), out Guid _);

            if (fieldsAreGuids)
            {
                SendEmails(Trigger, guids, reportResults);
            }
        }

        private static void SendEmails(MailTrigger Trigger, List<string> guids, List<string[]> reportResults)
        {
            if (Trigger.TriggerType.Equals(Enums.MailTriggerType.ScheduleFill))
            {
                guids.ForEach(x => EncompassHelper.SendEmail(Trigger.FillMessage(x)));
            }
            else if (Trigger.TriggerType.Equals(Enums.MailTriggerType.ScheduleAttach))
            {
                //DataTable dt = FileParser.DataTableFromReport(reportResults);
                //Trigger.AttachMessage(dt)


            }

            //Email Owner of Report
            //MailMessage mailMessage = new MailMessage();
            //mailMessage.From = new MailAddress(EncompassApplication.CurrentUser.Email, EncompassApplication.CurrentUser.FullName);
            //mailMessage.Subject = $"Report for {Trigger.Name}";
            //mailMessage.Body = $"Loans that were included in the emailed report {string.Join(Environment.NewLine, guids)}";
            //mailMessage.To.Add(new MailAddress(EncompassApplication.CurrentUser.Email));
            //EncompassHelper.SendEmail(mailMessage);
        }

       

        private static QueryCriterion CreateLoanCustomFilter(ReportSettings ReportSettings)
        {
            QueryCriterion queryCriterion = ReportSettings.ToQueryCriterion();
            switch (ReportSettings.LoanFilterType)
            {
                case ReportLoanFilterType.Role:
                    QueryCriterion criterion1 = (QueryCriterion)new BinaryOperation(BinaryOperator.And, (QueryCriterion)new OrdinalValueCriterion("LoanAssociateUser.RoleID", (object)ReportSettings.LoanFilterRoleId), (QueryCriterion)new StringValueCriterion("LoanAssociateUser.UserID", ReportSettings.LoanFilterUserInRole));
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion1) : criterion1;
                    break;
                case ReportLoanFilterType.Organization:
                    QueryCriterion criterion2 = (QueryCriterion)new OrdinalValueCriterion("AssociateUser.org_id", (object)ReportSettings.LoanFilterOrganizationId);
                    if (ReportSettings.LoanFilterIncludeChildren)
                        criterion2 = criterion2.Or((QueryCriterion)new XRefValueCriterion("Associateuser.org_id", "org_descendents.descendent", (QueryCriterion)new OrdinalValueCriterion("org_descendents.oid", (object)ReportSettings.LoanFilterOrganizationId)));
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion2) : criterion2;
                    break;
                case ReportLoanFilterType.UserGroup:
                    QueryCriterion criterion3 = (QueryCriterion)new OrdinalValueCriterion("AssociateGroup.GroupID", (object)ReportSettings.LoanFilterUserGroupId);
                    queryCriterion = queryCriterion != null ? queryCriterion.And(criterion3) : criterion3;
                    break;
            }
            if (ReportSettings.DynamicQueryCriterion != null)
                queryCriterion = queryCriterion != null ? queryCriterion.And(ReportSettings.DynamicQueryCriterion) : ReportSettings.DynamicQueryCriterion;
            return queryCriterion;
        }
    }
}
