using Elli.Server.Remoting;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class PipelineViewTransfer_Form : Form
    {
        private FileSystemEntry[] CurrentUserEntries;
        public PipelineViewTransfer_Form()
        {
            InitializeComponent();
            string[] users = EncompassApplication.Session.Users.GetAllUsers().Cast<EllieMae.Encompass.BusinessObjects.Users.User>().OrderBy(x=>x.ID).Select(x => x.ID).ToArray();

            cmbFrom.Items.AddRange(users);
            cmbTo.Items.AddRange(users);
            cmbType.Items.AddRange(Enum.GetNames(typeof(TemplateSettingsType)));
            cmbType.Text = TemplateSettingsType.PipelineView.ToString();

            cmbFrom.SelectedIndexChanged += CmbFrom_SelectedIndexChanged;
            cmbTo.SelectedIndexChanged += CmbTo_SelectedIndexChanged;
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTo();
            RefreshFrom();
        }

        private void CmbTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTo();
        }

        private void RefreshTo()
        {
            lbToViews.Items.Clear();
            string id = cmbTo.Text;
            if (string.IsNullOrEmpty(id))
                return;

            TemplateSettingsType type = (TemplateSettingsType)Enum.Parse(typeof(TemplateSettingsType), cmbType.Text);
            FileSystemEntry[] viewFileEntries = Session.ConfigurationManager.GetAllTemplateSettingsFileEntries(type, id);
            lbToViews.Items.AddRange(viewFileEntries.Select(x => x.Name).ToArray());
        }

        private void CmbFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFrom();
        }

        private void RefreshFrom()
        {
            clbViews.Items.Clear();
            string id = cmbFrom.Text;
            if (string.IsNullOrEmpty(id))
                return;

            TemplateSettingsType type = (TemplateSettingsType)Enum.Parse(typeof(TemplateSettingsType), cmbType.Text);
            CurrentUserEntries = Session.ConfigurationManager.GetAllTemplateSettingsFileEntries(type, id);
            clbViews.Items.AddRange(CurrentUserEntries.Select(x => x.Name).ToArray());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            foreach (string item in clbViews.CheckedItems.Cast<string>())
            {
                FileSystemEntry entry = CurrentUserEntries.FirstOrDefault(x => x.Name.Equals(item));
                if (entry == null)
                    return;

                string userID = cmbTo.Text;
                if (string.IsNullOrEmpty(userID))
                    return;

                TemplateSettingsType type = (TemplateSettingsType)Enum.Parse(typeof(TemplateSettingsType), cmbType.Text);
                var view = Session.ConfigurationManager.GetTemplateSettings(type, entry);
                Session.ConfigurationManager.SaveTemplateSettings(type, FileSystemEntry.PrivateRoot(userID).Combine(entry.Name), view);
            }
            RefreshTo();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
        }
    }
}
