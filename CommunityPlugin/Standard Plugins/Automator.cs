using EllieMae.Encompass.BusinessObjects.Loans;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;


namespace CommunityPlugin.Plugins
{
    public class Automator : Plugin, ILogin, ILoanOpened, IFieldChange
    {
        private BlockingCollectionQueue Producer;
        Timer t;

        public override void Login(object sender, EventArgs e)
        {
            if (!EncompassHelper.User.ID.Equals("Automator", StringComparison.InvariantCultureIgnoreCase))
                return;

            Global.CDOs = new Dictionary<string, object>();
            AutoMailerCDO amCDO = CustomDataObject.Get<AutoMailerCDO>();
            Global.CDOs.Add(nameof(AutoMailerCDO), amCDO);
            Producer = new BlockingCollectionQueue();
            RunTimer();
        }


        private void RunTimer()
        {
            t = new Timer();
            t.Interval = 60000;
            t.Enabled = true;
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<ITask> triggers = new List<ITask>();
            foreach (Type type in ((IEnumerable<Type>)this.GetType().Assembly.GetTypes()).Where((type => type.GetInterfaces().Contains(typeof(IFactory)))).ToList<Type>())
            {
                IFactory pluginClassItem = Activator.CreateInstance(type) as IFactory;
                triggers.AddRange(pluginClassItem.GetTriggers());
            }
            if (Producer != null)
            {
                foreach (ITask trigger in triggers)
                {
                    Producer.EnqueueTask(trigger);
                }
            }
        }
    }
}
