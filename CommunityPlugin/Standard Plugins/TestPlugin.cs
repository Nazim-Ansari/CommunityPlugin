using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.DataEngine.Log;
using EllieMae.EMLite.LoanServices;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System.Collections.Generic;

namespace CommunityPlugin.Standard_Plugins
{
    public class TestPlugin: Plugin, INativeFormLoaded, IFieldChange, ILoanOpened
    {
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals("FormSelectioinDialog") && e.OpenForm.Text.Equals("Select Documents"))
            {
                System.Windows.Forms.Button b = e.OpenForm.Controls.Find("btnSend", true)[0] as System.Windows.Forms.Button;
                b.Enabled = EncompassApplication.CurrentLoan.Fields["CX.HYBRID.OPTOUT"].FormattedValue.Equals("X");
            }
        }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            if(e.FieldID.Equals("cx.doc.request"))
            {
                //ClientSession.dll
                DocumentLog[] docs = Session.LoanDataMgr.LoanData.GetLogList().GetAllDocuments();
                Session.Application.GetService<IEFolder>().Request(Session.LoanDataMgr, docs);
                
            }
        }

        private void DoNothing()
        {
        }
    }
}
