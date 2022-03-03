using CommunityPlugin.Objects.Enums;

namespace CommunityPlugin.Objects.Models
{
    public class DoorBellMsg
    {
        public string Message { get; set; }

        public DoorbellMsgType  Type {get;set;}

        public DoorBellMsg()
        {
            Message = string.Empty;
        }
    }
}
