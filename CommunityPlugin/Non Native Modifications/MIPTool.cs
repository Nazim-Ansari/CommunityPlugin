using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class MIPTool : Plugin, INativeFormLoaded
    {
        private const string Key = "CX.GETMI";
        private bool Pressed = false;
        private bool CanRun => EncompassApplication.Session.Loans.FieldDescriptors.Cast<FieldDescriptor>().Any(x => x.FieldID.Equals(Key));

        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals("MIPDialog"))
            {
                Button mip = e.OpenForm.Controls.Find("btnGetMI", true)[0] as Button;
                mip.Click += Mip_Click;
                Button ok = e.OpenForm.Controls.Find("okBtn", true)[0] as Button;
                ok.Click += Ok_Click;
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (Pressed && CanRun)
            {
                Loan.Fields[Key].Value = Loan.Fields[Key].ToDecimal() + 1;
                Pressed = false;
            }
        }

        private void Mip_Click(object sender, EventArgs e)
        {
            Pressed = true;
        }
    }
}