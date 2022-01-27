using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.ePass.Services;
using EllieMae.EMLite.UI;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class DisableServices : Plugin, ILoanTabChanged, ILoanOpened, IFieldChange, INativeFormLoaded
    {
        private string ServicesToDisable;
        private bool ShouldRun;
        private const string Field = "CX.DISABLE.SERVICES";
        private Form OpenForm;
        HashSet<string> FormNames = new HashSet<string>() {"OrderDialog", "AppraserDialog", "LenderDialog" };
        private bool isLender => OpenForm.Name.Equals("LenderDialog");
        private bool isAppraiser => OpenForm.Name.Equals("AppraserDialog");
        private TabControl TabControl  => (TabControl)OpenForm.Controls.Find((isLender ? "lenderTab" : "orderTab"), true)[0];

        public override void LoanTabChanged(object sender, EventArgs e)
        {
            bool shouldRun = EncompassApplication.Session.Loans.FieldDescriptors.CustomFields.Cast<FieldDescriptor>().Any(x => x.FieldID.Equals("CX.DISABLE.SERVICES"));
            if (!shouldRun)
                return;

            Timer t = new Timer();
            t.Interval = 1000;
            t.Tick += T_Tick;
            t.Enabled = true;
        }
        public override void LoanOpened(object sender, EventArgs e) { }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            if (e.FieldID.Equals(Field))
                Remove();
        }
        private void T_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;

            Remove();
        }

        private void Remove()
        {
            ServicesToDisable = EncompassApplication.CurrentLoan.Fields["CX.DISABLE.SERVICES"].FormattedValue.ToLower().Replace(" ", "");

            bool all = ServicesToDisable.Equals("all", StringComparison.OrdinalIgnoreCase);
            Control[] controlArray = FormWrapper.EncompassForm.Controls.Find("toolsFormsTabControl", true);
            if (((IEnumerable<Control>)controlArray).Count<Control>() < 1)
                return;
            TabControl tabcontrol = controlArray[0] as TabControl;
            TabPage servicePage = tabcontrol.TabPages.Cast<TabPage>().Where(x => x.Name.Equals("servicesPage")).FirstOrDefault();
            GradientMenuStrip menuStrip = (GradientMenuStrip)FormWrapper.EncompassForm.Controls.Find("mainMenu", true)?[0];

            if (all)
            {
                if (tabcontrol.TabPages.Contains(servicePage))
                    tabcontrol.TabPages.Remove(servicePage);
            }
            else
            {
                Panel panel = tabcontrol.Controls.Find("pnlCategories", true).FirstOrDefault() as Panel;
                if (panel == null)
                    return;


                EpassCategoryControl[] controls = panel.Controls.OfType<EpassCategoryControl>().ToArray();
                foreach (EpassCategoryControl control in controls)
                {
                    string name = control.Title.ToLower().Replace(" ", "");
                    control.Visible = string.IsNullOrEmpty(ServicesToDisable) || !ServicesToDisable.Contains(name);
                }
            }

            ToolStripMenuItem serviceItem = (ToolStripMenuItem)menuStrip.Items["menuItemServices"];
            serviceItem.Visible = string.IsNullOrEmpty(ServicesToDisable);
        }

        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (FormNames.Contains(e.OpenForm.Name))
            {
                OpenForm = e.OpenForm;
                Timer t = new Timer();
                t.Interval = 300;
                t.Tick += T_Tick1;
                t.Enabled = true;
            }
        }

        private void T_Tick1(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;
            ServicesToDisable = EncompassHelper.Val("CX.DISABLE.SERVICES");
            RefreshGrid();
            RefreshGrid();
            TabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
            RefreshGrid(true);
        }

        private void RefreshGrid(bool Supress = false)
        {
            string controlID = isAppraiser ? "lvwMyAppraisers" : TabControl.SelectedIndex.Equals(0) ? "myLst" : "allLst";
            Control[] controls = OpenForm.Controls.Find(controlID, true);
            if (controls.Count().Equals(0))
                return;

            ListBox box = (ListBox)controls[0];
            List<string> services = new List<string>();
            if (box.Name.Equals("allLst"))
            {
                for (int i = 0; i < box.Items.Count; i++)
                {
                    if (ServicesToDisable.Contains(box.Items[i].ToString()))
                        services.Add(box.Items[i].ToString());
                }
                if (services.Any() && !Supress)
                    MessageBox.Show($"The Following services will be limited:{Environment.NewLine}{string.Join(Environment.NewLine, services)}");
            }

            for (int i = 0; i < box.Items.Count; i++)
            {
                if (ServicesToDisable.Contains(box.Items[i].ToString()))
                    box.Items.Remove(box.Items[i]);
            }
        }
    }
}