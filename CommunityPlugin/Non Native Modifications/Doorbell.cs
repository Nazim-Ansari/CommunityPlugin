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
            {
                DoorBellMsg msg = new DoorBellMsg();
                msg.Type = Objects.Enums.DoorbellMsgType.Out;
                msg.Message = DingBackMessage;
                PostMsg(msg, DingBackID);
            }

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

            MessageBox.Show(FillMessage(CDO.ConfirmationMessage));
        }

        private void RingDoorbell()
        {
            DoorBellMsg msg = new DoorBellMsg();
            msg.Type = Objects.Enums.DoorbellMsgType.Exit;
            msg.Message = FillMessage(CDO.UserInFileMessage, Tag.LockInfo);
            PostMsg(msg, Tag.LockInfo.LockedBy);
        }


        private void PostMsg(DoorBellMsg Msg, string ID)
        {
            DataExchange data = EncompassApplication.Session.DataExchange;
            data.PostDataToUser(ID, JsonConvert.SerializeObject(Msg));
        }
        private string FillMessage(string Input, string Guid = null, bool User = true)
        {
            return EncompassHelper.InsertEncompassValue(Input, Guid ?? Tag.GUID).Replace("{user}", EncompassHelper.User.ID).Replace("{name}", EncompassHelper.User.FullName);
        }
        private string FillMessage(string Input, LockInfo Lock)
        {
            return EncompassHelper.InsertEncompassValue(Input, Tag.GUID).Replace("{user}", Lock.LockedBy).Replace("{name}", $"{Lock.LockedByFirstName} {Lock.LockedByLastName}");
        }
        public override void DataExchangeReceived(object sender, DataExchangeEventArgs e)
        {
            DoorBellMsg msg = JsonConvert.DeserializeObject<DoorBellMsg>(e.Data.ToString());
            if (msg == null)
                return;

            System.Media.SoundPlayer music = new System.Media.SoundPlayer();
            music.Stream = msg.Type.Equals(Objects.Enums.DoorbellMsgType.Exit) ? Resources.Exit :
                           msg.Type.Equals(Objects.Enums.DoorbellMsgType.Out) ? Resources.Out : 
                           Resources.Out;
            music.Play(); 
            EncompassHelper.ShowOnTop("DoorBell Notification", msg.Message);
            if (msg.Type.Equals(Objects.Enums.DoorbellMsgType.Exit))
            {
                bool inLoan = EncompassApplication.CurrentLoan == null;
                DingBackMessage = FillMessage(CDO.UserOutMessage, Loan.Guid);
                DingBackID = e.Source.UserID;
                if (inLoan)
                {
                    DoorBellMsg cMsg = new DoorBellMsg();
                    msg.Type = Objects.Enums.DoorbellMsgType.Out;
                    msg.Message = FillMessage(CDO.UserOutMessage);
                    PostMsg(cMsg, e.Source.UserID);
                }
                else
                {
                    //We can use this if we want the requesting user to know when the infile user see's their message

                    //EllieMae.Encompass.BusinessObjects.Users.User c = EncompassApplication.CurrentUser;
                    //DoorBellMsg cMsg = new DoorBellMsg();
                    //msg.Type = Objects.Enums.DoorbellMsgType.Confirm;
                    //msg.Message = FillMessage(CDO.ConfirmationMessage, Loan.Guid);
                    //PostMsg(cMsg, e.Source.UserID);
                    SendOutOfFileMessage = true;
                }
            }
        }
    }
}
