using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class Form1 : Form
    {
        public void UpdateQueuedMediaListView()
        {
            ImageList icons = new ImageList();
            icons.Images.Add(Properties.Resources.video_icon);
            var items = new ListViewItem[priorityQueue.Count];
            var queue = priorityQueue.ToArray();
            for (int i = 0; i < priorityQueue.Count; i++)
                items[i] = new ListViewItem(queue[i], 0);
            QueuedMediaListView.Items.Clear();
            QueuedMediaListView.Items.AddRange(items);
        }
    }
}
