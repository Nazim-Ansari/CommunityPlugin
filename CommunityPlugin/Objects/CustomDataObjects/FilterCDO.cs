using EllieMae.EMLite.ClientServer.Reporting;
using System.Collections.Generic;

namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class FilterCDO
    {
        public List<FieldFilter> Filters { get; set; }
        public FilterCDO()
        {
            Filters = new List<FieldFilter>();
        }
    }
}
