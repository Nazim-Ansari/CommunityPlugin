using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.Common;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Linq;

namespace CommunityPlugin.Standard_Plugins
{
    public class GKPlugin: Plugin, ILogin, ILoanOpened, IFieldChange, INativeFormLoaded
    {
        private GKConfig config;
        public override void Login(object sender, EventArgs e)
        {
            config = CustomDataObject.Get<GKConfig>();
        }

        public override void LoanOpened(object sender, EventArgs e) { }

        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {

        }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            //if(config.Info.Any(x=>x.TriggerField.Equals(e.FieldID)))
            //{
            //    //do stuff
            //}
        }
    }
}
