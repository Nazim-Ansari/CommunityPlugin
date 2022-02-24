using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Standard_Plugins
{
    public class ServiceNotification : Plugin, INativeFormLoaded
    {
        private Form OpenForm;
        private TabControl TabControl;
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals("OrderDialog"))
            {
                OpenForm = e.OpenForm;

                Control control = GetControl("addBtn2");
                if (control != null)
                {
                    Button run = control as Button;
                    run.Click += Run_Click;


                    control = GetControl("toolsFormsTabControl");
                    TabControl = control as TabControl;
                }
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            Control controls = GetControl(TabControl.SelectedIndex.Equals(0) ? "myLst" : "allLst");
            if (controls != null)
                return;

            ListBox box = (ListBox)controls;
            string SelectedService = box.Text;
            EncompassHelper.CurrentLoan.Fields["CX.SERVICE.NOTIFY"].Value = SelectedService;
        }

        private Control GetControl(string ControlID)
        {
            return OpenForm.Controls.Find(ControlID, true).FirstOrDefault();
        }
    }
}