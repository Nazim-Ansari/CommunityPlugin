using CommunityPlugin.Non_Native_Modifications;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Objects
{
    public abstract class Plugin : IPlugin
    {
        public EllieMae.EMLite.RemotingServices.Sessions.Session RemoteSession => EllieMae.EMLite.RemotingServices.Session.DefaultInstance;
        public Loan Loan => EncompassApplication.CurrentLoan;
        public virtual void Configure()
        {

        }
        public virtual void Run()
        {
            if (!PluginAccess.CheckAccess(GetType().Name))
                return;


            LoansScreen loan = (LoansScreen)EncompassApplication.Screens[EncompassScreen.Loans];
            if (typeof(IFormLoaded).IsAssignableFrom(GetType()) && loan != null)
            {
                loan.FormLoaded -= Base_FormLoaded;
                loan.FormLoaded += Base_FormLoaded;
            }

            if (typeof(ILoanClosing).IsAssignableFrom(GetType()))
            {
                EncompassApplication.LoanClosing -= Base_LoanClosing;
                EncompassApplication.LoanClosing += Base_LoanClosing;
            }

            if (typeof(ILoanOpened).IsAssignableFrom(GetType()))
            {
                EncompassApplication.LoanOpened -= Base_LoanOpened;
                EncompassApplication.LoanOpened += Base_LoanOpened;
            }

            if (typeof(ILogin).IsAssignableFrom(GetType()) || typeof(ITabChanged).IsAssignableFrom(GetType()) || typeof(ILoanTabChanged).IsAssignableFrom(GetType()) || typeof(IPipelineTabChanged).IsAssignableFrom(GetType()))
            {
                EncompassApplication.Login -= Base_Login;
                EncompassApplication.Login += Base_Login;
            }

            if (typeof(INativeFormLoaded).IsAssignableFrom(GetType()))
            {
                FormWrapper.FormOpened -= Base_NativeFormLoaded;
                FormWrapper.FormOpened += Base_NativeFormLoaded;
            }

            if (typeof(IDataExchangeReceived).IsAssignableFrom(GetType()))
            {
                EncompassApplication.Session.DataExchange.DataReceived -= Base_DataExchangeReceived;
                EncompassApplication.Session.DataExchange.DataReceived += Base_DataExchangeReceived;
            }
        }


        public virtual void LoanOpened(object sender, EventArgs e)
        {
        }

        private void Base_LoanOpened(object sender, EventArgs e)
        {
            Loan loan = EncompassApplication.CurrentLoan;
            if (loan == null)
                return;

            if (typeof(IBeforeCommit).IsAssignableFrom(GetType()))
            {
                loan.BeforeCommit -= Base_BeforeCommit;
                loan.BeforeCommit += Base_BeforeCommit;
            }

            if (typeof(IFieldChange).IsAssignableFrom(GetType()))
            {
                loan.FieldChange -= Base_FieldChange;
                loan.FieldChange += Base_FieldChange;
            }

            if (typeof(ICommitted).IsAssignableFrom(GetType()))
            {
                loan.Committed -= Base_Committed;
                loan.Committed += Base_Committed;
            }

            if (typeof(IBeforeCommit).IsAssignableFrom(GetType()))
            {
                loan.BeforeCommit -= Base_BeforeCommit;
                loan.BeforeCommit += Base_BeforeCommit;
            }

            if (typeof(ILogEntryAdded).IsAssignableFrom(GetType()))
            {
                loan.LogEntryAdded -= Base_LogEntryAdded;
                loan.LogEntryAdded += Base_LogEntryAdded;
            }

            if (typeof(ILogEntryChanged).IsAssignableFrom(GetType()))
            {
                loan.LogEntryChange -= Base_LogEntryChanged;
                loan.LogEntryChange += Base_LogEntryChanged;
            }

            if (typeof(ILogEntryRemoved).IsAssignableFrom(GetType()))
            {
                loan.LogEntryRemoved -= Base_LogEntryRemoved;
                loan.LogEntryRemoved += Base_LogEntryRemoved;
            }

            if (typeof(IBeforeMilestoneCompleted).IsAssignableFrom(GetType()))
            {
                loan.BeforeMilestoneCompleted -= Base_BeforeMilestoneCompleted;
                loan.BeforeMilestoneCompleted += Base_BeforeMilestoneCompleted;
            }

            if (typeof(IMilestoneCompleted).IsAssignableFrom(GetType()))
            {
                loan.MilestoneCompleted -= Base_MilestoneCompleted;
                loan.MilestoneCompleted += Base_MilestoneCompleted;
            }

            //if (typeof(IBorrowerPairChanged).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.BorrowerPairChanged += Base_BorrowerPairChanged;

            //if (typeof(IBeforeFieldChanged).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.BeforeFieldChanged += Base_BeforeFieldChanged;

            //if (typeof(IBeforeFieldChanged).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.BeforeTriggerRuleApplied += Base_BeforeTriggerRuleApplied;

            //if (typeof(IDisclosure2015Created).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.Disclosure2015Created += Base_Disclosure2015Created;

            //if (typeof(ILockRequestFieldChanged).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.LockRequestFieldChanged += Base_LockRequestFieldChanged;

            //if (typeof(IRateLockConfirmed).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.RateLockConfirmed += Base_RateLockConfirmed;

            //if (typeof(IRateLockDenied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.RateLockDenied += Base_RateLockDenied;

            //if (typeof(IRateLockRequested).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.RateLockRequested += Base_RateLockRequested;

            //if (typeof(IAfterDDMApplied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.BeforeMilestoneCompleted += Base_AfterDDMApplied;

            //if (typeof(IExecuteEmailTriggers).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.ExecuteEmailTriggers += Base_ExecuteEmailTriggers;

            //if (typeof(IAfterDDMApplied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.FormVersionChanged += Base_AfterDDMApplied;

            //if (typeof(IAfterDDMApplied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.LockRequestFieldChanged += Base_AfterDDMApplied;

            //if (typeof(IAfterDDMApplied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.RateLockDenied += Base_AfterDDMApplied;

            //if (typeof(IAfterDDMApplied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.LoanData.RateLockRequested += Base_AfterDDMApplied;

            //if (typeof(IAfterDDMApplied).IsAssignableFrom(GetType()))
            //    EncompassHelper.LoanDataManager.FieldRulesChanged += Base_AfterDDMApplied;


            LoanOpened(sender, e);
        }


        public virtual void Login(object sender, EventArgs e) { }

        private void Base_Login(object sender, EventArgs e)
        {
            try
            {
                FormWrapper.TabControl.SelectedIndexChanged += Base_TabChanged;
                Login(sender, e);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(Base_Login));
            }
        }


        public virtual void DataExchangeReceived(object sender, DataExchangeEventArgs e) { }

        private void Base_DataExchangeReceived(object sender, DataExchangeEventArgs e)
        {
            try
            {
                DataExchangeReceived(sender, e);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(Base_DataExchangeReceived));
            }
        }


        public virtual void BeforeCommit(object sender, CancelableEventArgs e) { }

        private void Base_BeforeCommit(object sender, CancelableEventArgs e)
        {
            BeforeCommit(sender, e);
        }

        public virtual void BeforeMilestoneCompleted(object sender, EllieMae.Encompass.BusinessObjects.Loans.CancelableMilestoneEventArgs e)
        {

        }
        private void Base_BeforeMilestoneCompleted(object sender, EllieMae.Encompass.BusinessObjects.Loans.CancelableMilestoneEventArgs e)
        {
            BeforeMilestoneCompleted(sender, e);
        }

        public virtual void Committed(object sender, EventArgs e) { }

        private void Base_Committed(object sender, EventArgs e)
        {
            Committed(sender, e);
        }


        public virtual void LoanClosing(object sender, EventArgs e) { }

        private void Base_LoanClosing(object sender, EventArgs e)
        {
            LoanClosing(sender, e);
        }

        public virtual void LogEntryAdded(object sender, LogEntryEventArgs e) { }

        private void Base_LogEntryAdded(object sender, LogEntryEventArgs e)
        {
            LogEntryAdded(sender, e);
        }

        public virtual void LogEntryChanged(object sender, LogEntryEventArgs e) { }

        private void Base_LogEntryChanged(object sender, LogEntryEventArgs e)
        {
            LogEntryChanged(sender, e);
        }

        public virtual void LogEntryRemoved(object sender, LogEntryEventArgs e) { }

        private void Base_LogEntryRemoved(object sender, LogEntryEventArgs e)
        {
            LogEntryRemoved(sender, e);
        }

        public virtual void MilestoneCompleted(object sender, EllieMae.Encompass.BusinessObjects.Loans.MilestoneEventArgs e) { }

        private void Base_MilestoneCompleted(object sender, EllieMae.Encompass.BusinessObjects.Loans.MilestoneEventArgs e)
        {
            MilestoneCompleted(sender, e);
        }

        public virtual void FieldChanged(object sender, FieldChangeEventArgs e) { }


        private void Base_FieldChange(object sender, FieldChangeEventArgs e)
        {
            try
            {
                this.FieldChanged(sender, e);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(Base_FieldChange), (object)null);
            }
        }

        public virtual void FormLoaded(object sender, FormChangeEventArgs e) { }

        private void Base_FormLoaded(object sender, FormChangeEventArgs e)
        {
            FormLoaded(sender, e);
        }


        public virtual void NativeFormLoaded(object sender, FormOpenedArgs e) { }

        private void Base_NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            try
            {
                NativeFormLoaded(sender, e);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(Base_NativeFormLoaded));
            }
        }


        public virtual void TabChanged(object sender, EventArgs e) { }


        public virtual void LoanTabChanged(object sender, EventArgs e)
        {
            
        }

        public virtual void PipelineTabChanged(object sender, EventArgs e)
        {

        }

        private void Base_TabChanged(object sender, EventArgs e)
        {
            try
            {
                TabControl tabs = sender as TabControl;
                TabPage page = tabs.TabPages[tabs.SelectedIndex];
                if (page != null && page.Name.Equals("loanTabPage"))
                    LoanTabChanged(sender, e);
                else if (page != null && page.Name.Equals("pipelineTabPage"))
                    PipelineTabChanged(sender, e);


                TabChanged(sender, e);
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(Base_TabChanged));
            }
        }
    }
}
