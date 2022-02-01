using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class GKConfig
    {
        public List<GKInfo> Info { get; set; }
        public GKConfig()
        {
            Info = new List<GKInfo>();
        }
    }

    public class GKInfo
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public GKInfo()
        {
            Name = string.Empty;
            Street = string.Empty;
        }
    }
}
