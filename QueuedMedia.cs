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
        private bool deleteItemFromQueue = false;
        private bool PassItemToFirstInQueue = false;

        private void QueuedMediaListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
                return;

            if (deleteItemFromQueue)
            {
                var queueArray = queuedMedia.ToArray();

                queuedMedia.Clear();
                for (int i = 0; i < queueArray.Length; i++)
                {
                    if (e.ItemIndex != i)
                        queuedMedia.Enqueue(queueArray[i]);
                }
                UpdateQueuedMediaListView();
                //DeleteItemFromQueueButton_Click(this, null);
                return;
            }
            else if (PassItemToFirstInQueue)
            {
                string[] queueToArray = queuedMedia.ToArray();
                (queueToArray[0], queueToArray[e.ItemIndex]) = (queueToArray[e.ItemIndex], queueToArray[0]);
                PassButtonToFirstInQueue_Click(this, null);
            }
            else
            {

            }
        }

        private void ShuffleQueueButton_Click(object sender, EventArgs e)
        {

        }

        private void PassButtonToFirstInQueue_Click(object sender, EventArgs e)
        {
            PassItemToFirstInQueue = !PassItemToFirstInQueue;
            if (PassItemToFirstInQueue)
                PassItemToFirstInQueueButton.Text = "Pass Item to first of queue t";
            else
                PassItemToFirstInQueueButton.Text = "Pass Item to first of queue";

            if (deleteItemFromQueue)
                DeleteItemFromQueueButton_Click(this, null);
        }

        private void DeleteItemFromQueueButton_Click(object sender, EventArgs e)
        {
            deleteItemFromQueue = !deleteItemFromQueue;
            if (deleteItemFromQueue)
                DeleteItemFromQueueButton.Text = "Delete item from queue t";
            else
                DeleteItemFromQueueButton.Text = "Delete item from queue";

            if (PassItemToFirstInQueue)
                PassButtonToFirstInQueue_Click(this, null);
        }

        public void UpdateQueuedMediaListView()
        {
            ImageList icons = new ImageList();
            icons.Images.Add(Properties.Resources.video_icon);
            var items = new ListViewItem[queuedMedia.Count];
            var queue = queuedMedia.ToArray();
            for (int i = 0; i < queuedMedia.Count; i++)
                items[i] = new ListViewItem(queue[i], 0);
            QueuedMediaListView.Items.Clear();
            QueuedMediaListView.Items.AddRange(items);
        }
    }
}
