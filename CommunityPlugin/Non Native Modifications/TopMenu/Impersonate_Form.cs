﻿using CommunityPlugin.Objects.Helpers;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.MainUI;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class Impersonate_Form : Form
    {
        public Impersonate_Form()
        {
            InitializeComponent();
        }

        private void BtnImpersonate_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text;
            if (!string.IsNullOrEmpty(user))
            {
                EncompassHelper.SessionObjects.Session.ImpersonateUser(user);
                EncompassHelper.SessionObjects.InvalidateUserInfo();
                RefreshPipeline();
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            EncompassHelper.SessionObjects.Session.RestoreIdentity();
            RefreshPipeline();
        }

        private void TxtUser_TextChanged(object sender, EventArgs e)
        {


        }

        private void RefreshPipeline()
        {
            MainScreen mainScreen = FormWrapper.Find("mainScreen") as MainScreen;
            mainScreen.PipelineScreenBrowser = null;
            IPipeline pipeline = mainScreen.GetService<EllieMae.EMLite.Common.UI.IPipeline>();
            mainScreen.ShowTab("Home");
            mainScreen.ShowTab("Pipeline");
        }
    }
}
