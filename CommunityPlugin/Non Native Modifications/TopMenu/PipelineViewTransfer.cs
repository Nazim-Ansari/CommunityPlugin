using System;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public class PipelineViewTransfer : MenuItemBase
    {
        protected override void menuItem_Click(object sender, EventArgs e)
        {
            PipelineViewTransfer_Form f = new PipelineViewTransfer_Form();
            f.ShowDialog();
        }
    }
}
