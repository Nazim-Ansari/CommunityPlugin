using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;

namespace CommunityPlugin.Objects
{
    public static class PluginEntry
    {
        public static void Start()
        {

            InterfaceHelper i = new InterfaceHelper();
            List<Plugin> activePlugins = new List<Plugin>();
            foreach (Type type in i.GetAll(typeof(Plugin)))
            {
                try
                {
                    Plugin p = Activator.CreateInstance(type) as Plugin;
                    p.Run();
                    activePlugins.Add(p);
                }
                catch(Exception ex)
                {
                    Logger.HandleError(ex, nameof(PluginEntry));
                }
            }

            Global.ActivePlugins = activePlugins;
        }
    }
}
