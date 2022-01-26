using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;

namespace CommunityPlugin.Standard_Plugins
{
    public class TestPlugin: Plugin, INativeFormLoaded
    {
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals("FormSelectioinDialog") && e.OpenForm.Text.Equals("Select Documents"))
            {
                System.Windows.Forms.Button b = e.OpenForm.Controls.Find("btnSend", true)[0] as System.Windows.Forms.Button;
                b.Enabled = EncompassApplication.CurrentLoan.Fields["CX.HYBRID.OPTOUT"].FormattedValue.Equals("X");
            }
        }
    }
}
