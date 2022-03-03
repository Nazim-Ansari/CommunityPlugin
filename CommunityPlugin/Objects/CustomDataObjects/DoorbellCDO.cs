namespace CommunityPlugin.Objects.CustomDataObjects
{
    public class DoorbellCDO
    { 
        public string UserInFileMessage { get; set; }
        public string UserOutMessage { get; set; }

        public string ConfirmationMessage { get; set; }

        public DoorbellCDO()
        {
            UserInFileMessage = string.Empty;
            UserOutMessage = string.Empty;
            ConfirmationMessage = string.Empty;
        }
    }
}
