using EllieMae.Encompass.Automation;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Factories;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;

namespace CommunityPlugin.Objects.Models
{
    public class MailTrigger : ITask
    {
        public string Name { get; set; }

        public bool Active { get; set; }
        public bool HasRan { get; set; }

        public MailTriggerType TriggerType { get; set; }

        #region TriggerProperties

        public List<string> TriggerFields { get; set; }

        public string ReportFilter { get; set; }

        public FrequencyType Frequency { get; set; }

        public int[] Days { get; set; }

        public DateTime Time { get; set; }
        public DateTime Date { get; set; }

        #endregion TriggerProperties

        #region EmailProperties

        public string Subject { get; set; }

        public string Body { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        #endregion EmailProperties

        public MailMessage FillMessage (string guid) 
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(EncompassApplication.CurrentUser.Email, EncompassApplication.CurrentUser.FullName);
            mail.IsBodyHtml = true;
            mail.Subject = EncompassHelper.InsertEncompassValue(Subject, guid);
            mail.Body = EncompassHelper.InsertEncompassValue(Body, guid);
            foreach (string email in To.Split(','))
                mail.To.Add(new MailAddress(EncompassHelper.InsertEncompassValue(email, guid)));
            foreach(string email in CC.Split(','))
                mail.CC.Add(new MailAddress(EncompassHelper.InsertEncompassValue(email, guid)));
            foreach (string email in BCC.Split(','))
                mail.Bcc.Add(new MailAddress(EncompassHelper.InsertEncompassValue(email, guid)));

            return mail;
        }

        public MailMessage AttachMessage(DataTable dt = null)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(EncompassApplication.CurrentUser.Email, EncompassApplication.CurrentUser.FullName);
            mail.IsBodyHtml = true;
            mail.Subject = Subject;
            mail.Body = Body;
            foreach (string email in To.Split(','))
                mail.To.Add(email);
            foreach (string email in CC.Split(','))
                mail.CC.Add(email);
            foreach (string email in BCC.Split(','))
                mail.Bcc.Add(email);
            
            
            //Attachment a = new Attachment(null, "ReportData.xml");
            //mail.Attachments.Add(a);
            return mail;
        }
        public MailTrigger Clone(MailTrigger Original)
        {
            MailTrigger newTrigger = new MailTrigger();
            newTrigger.Name = $"Copy of {Original.Name}";
            newTrigger.Active = Original.Active;
            newTrigger.BCC = Original.BCC;
            newTrigger.Body = Original.Body;
            newTrigger.CC = Original.CC;
            newTrigger.Days = Original.Days;
            newTrigger.Frequency = Original.Frequency;
            newTrigger.ReportFilter = Original.ReportFilter;
            newTrigger.Subject = Original.Subject;
            newTrigger.Time = Original.Time;
            newTrigger.To = Original.To;
            newTrigger.TriggerFields = Original.TriggerFields;
            newTrigger.TriggerType = Original.TriggerType;

            return newTrigger;
        }

        public Action Run()
        {
            return () => EmailFactory.Run(this);
        }
    }
}