using CommunityPlugin.Configurations;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using CommunityPlugin.Properties;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.Client;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class Doorbell : Plugin, ILogin, ILoanClosing, IPipelineTabChanged, IDataExchangeReceived
    {
        private bool Hide;
        private ToolStripItem DoorBellItem;
        private string DingBackID;
        private string DingBackMessage;
        private GridView Pipeline;
        private bool SendOutOfFileMessage;
        private PipelineInfo Tag;
        private DoorbellCDO CDO;

        public override void Configure()
        {
            DoorbellForm f = new DoorbellForm();
            f.ShowDialog();
        }
        public override void Login(object sender, EventArgs e)
        {
            DoorBellItem = new ToolStripMenuItem("DoorBell");
            CDO = CustomDataObject.Get<DoorbellCDO>();
            if (string.IsNullOrEmpty(CDO.UserInFileMessage))
                CDO.UserInFileMessage = $"Please exit the Loan, {EncompassHelper.User.ID} needs access";
            if (string.IsNullOrEmpty(CDO.UserOutMessage))
                CDO.UserOutMessage = "User is out of the Loan [364] [4002],[4000]";
        }

        public override void LoanClosing(object sender, EventArgs e)
        {
            if (SendOutOfFileMessage)
                DingBack(string.Empty);

            SendOutOfFileMessage = false;
            DingBackID = string.Empty;
            DingBackMessage = string.Empty;
        }

        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            Pipeline = FormWrapper.GetPipeline();
            Pipeline.ContextMenuStrip.Opened += ContextMenuStrip_Opened;
        }

        private void ContextMenuStrip_Opened(object sender, EventArgs e)
        {
            if (Pipeline == null)
                Pipeline = FormWrapper.GetPipeline();
            if (Pipeline.SelectedItems.Count < 1)
                return;

            Tag = Pipeline.SelectedItems[0].Tag as PipelineInfo;
            if (!string.IsNullOrEmpty(Tag.LockInfo.LockedBy))
            {
                if (!Pipeline.ContextMenuStrip.Items.Contains(DoorBellItem))
                {
                    Pipeline.ContextMenuStrip.Items.Insert(0, DoorBellItem);

                    DoorBellItem.Click -= DoorBellItem_Click;
                    DoorBellItem.Click += DoorBellItem_Click;
                }
            }
            else
            {
                if (Pipeline.ContextMenuStrip.Items.Contains(DoorBellItem))
                {
                    Pipeline.ContextMenuStrip.Items.Remove(DoorBellItem);
                }
            }
        }

        private void DoorBellItem_Click(object sender, EventArgs e)
        {
            ToolStripItem Item = sender as ToolStripItem;
            if (Item.Text.Equals("DoorBell"))
                RingDoorbell();
        }

        private void RingDoorbell()
        {
            DataExchange data = EncompassApplication.Session.DataExchange;
            string lockInfo = $"exit: {EncompassHelper.InsertEncompassValue(CDO.UserInFileMessage, Tag.GUID).Replace("{user}", EncompassHelper.User.ID)}";  
            data.PostDataToUser(Tag.LockInfo.LockedBy, lockInfo);
        }

        private void DingBack(string LoanNumber)
        {
            DataExchange data = EncompassApplication.Session.DataExchange;
            string locked = $"out: {DingBackMessage}";  
            data.PostDataToUser(DingBackID, locked);
        }

        public override void DataExchangeReceived(object sender, DataExchangeEventArgs e)
        {
            bool exit = e.Data.ToString().Contains("exit: ");
            bool isDoorbell = exit || e.Data.ToString().Contains("out: ");
            if (!Hide && isDoorbell)
            {
                Hide = false;
                System.Media.SoundPlayer music = new System.Media.SoundPlayer(exit ? Resources.Exit : Resources.Out);
                music.Play();
                EncompassHelper.ShowOnTop("DoorBell Notification", e.Data.ToString());

                if (exit)
                {
                    bool inLoan = EncompassApplication.CurrentLoan == null;
                    DingBackMessage = EncompassHelper.InsertEncompassValue(CDO.UserOutMessage, Loan.Guid);
                    DingBackID = e.Source.UserID;
                    if (inLoan)
                        DingBack(DingBackMessage);
                    else
                        SendOutOfFileMessage = true;
                }
            }
        }
    }
}
