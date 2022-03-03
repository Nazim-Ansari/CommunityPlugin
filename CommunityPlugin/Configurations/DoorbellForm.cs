using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Configurations
{
    public partial class DoorbellForm : Form
    {
        private DoorbellCDO CDO;
        public DoorbellForm()
        {
            InitializeComponent();
            CDO = CustomDataObject.Get<DoorbellCDO>();
            txtRequester.Text = CDO.UserOutMessage;
            txtInLoan.Text = CDO.UserInFileMessage;
            txtConfirmation.Text = CDO.ConfirmationMessage;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CDO.UserOutMessage = txtRequester.Text;
            CDO.UserInFileMessage = txtInLoan.Text;
            CDO.ConfirmationMessage = txtConfirmation.Text;
            CustomDataObject.Save<DoorbellCDO>(CDO);
            this.Close();
        }
    }
}
