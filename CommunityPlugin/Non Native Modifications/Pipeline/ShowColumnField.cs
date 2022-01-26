using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.UI;
using System;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.Pipeline
{
    public class ShowColumnField : Plugin, INativeFormLoaded
    {
        private GridView Grid;
        private TextBox Search;
        private string LastFind;
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if (e.OpenForm.Name.Equals("TableLayoutColumnSelector"))
            {
                Grid = e.OpenForm.Controls.Find("gvColumns", true)[0] as GridView;
                if (Grid != null)
                {
                    //Add new Column to Customize DataGrid
                    GVColumn c = new GVColumn("Field ID");
                    c.Width = 200;
                    Grid.Columns.Add(c);
                    e.OpenForm.Width += 100;
                    Search = e.OpenForm.Controls.Find("txtFind", true)[0] as TextBox;

                    //Create a button to overlay the standard Find button to perform our own search
                    Button find = e.OpenForm.Controls.Find("btnFind", true)[0] as Button;
                    Button imp = new Button();
                    find.Parent.Controls.Add(imp);
                    find.Visible = false;
                    imp.Location = find.Location;
                    imp.Text = find.Text;
                    imp.Size = find.Size;
                    imp.Click += Imp_Click;

                    Timer t = new Timer();
                    t.Interval = 1000;
                    t.Enabled = true;
                    t.Tick += T_Tick;
                }
            }
        }

        /// <summary>
        /// First Search for exact fieldid matches, then do a regular search by description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Imp_Click(object sender, EventArgs e)
        {
            string text = Search.Text;

            foreach(GVItem i in Grid.Items)
            {
                string id = i.SubItems[1].Text;
                if(text.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                {
                    if(LastFind != id)
                    {
                        LastFind = id;
                        i.Selected = true;
                        Grid.EnsureVisible(i.Index);
                        Grid.Focus();
                        return;
                    }
                    else
                    {
                        LastFind = null;
                    }
                }
            }

            int num1 = 0;
            int num2 = -1;
            if (Grid.SelectedItems.Count > 0)
                num2 = Grid.SelectedItems[0].Index;
            if (text == LastFind && num2 >= 0)
                num1 = num2 + 1;

            for(int i = num1; i < Grid.Items.Count;i++)
            {
                if(Grid.Items[i].Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    LastFind = text;
                    Grid.Items[i].Selected = true;
                    Grid.EnsureVisible(i);
                    Grid.Focus();
                    return;
                }
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            t.Enabled = false;
            if (Grid != null)
            {
                foreach (GVItem item in Grid.Items)
                {
                    item.SubItems[1].Text = ((EllieMae.EMLite.ClientServer.TableLayout.Column)item.Value).Tag.Replace("Fields.",string.Empty);
                }
            }
        }
    }
}
