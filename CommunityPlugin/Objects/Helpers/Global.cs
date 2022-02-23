using System.Collections.Generic;

namespace CommunityPlugin.Objects.Helpers
{
    public static class Global
    {
        public static List<Plugin> ActivePlugins { get; set; }

        public static Dictionary<string, object> CDOs { get; set; }
    }
}
