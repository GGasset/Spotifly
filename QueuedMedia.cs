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
                var folderQueueArray = folderQueue.ToArray();

                List<string> queueList = new List<string>(queueArray), folderQueueList = new List<string>(folderQueueArray);
                queueList.RemoveAt(e.ItemIndex);
                folderQueueList.RemoveAt(e.ItemIndex);

                queuedMedia = new Queue<string>(queueList);
                folderQueue = new Queue<string>(folderQueueList);

                UpdateQueuedMediaListView();
                //DeleteItemFromQueueButton_Click(this, null);
            }
            else if (PassItemToFirstInQueue)
            {
                string[] queueArray = queuedMedia.ToArray();
                (queueArray[0], queueArray[e.ItemIndex]) = (queueArray[e.ItemIndex], queueArray[0]);
                queuedMedia = new Queue<string>(queueArray);
                PassButtonToFirstInQueue_Click(this, null);
                UpdateQueuedMediaListView();
            }
            else
            {
                string itemName = e.Item.Text;
                string[] folderQueueArray = folderQueue.ToArray();
                PlayFileInUnshuffled(itemName, folderQueueArray[e.ItemIndex], CheckMediaIndexWithSongQueueCheckBox.Checked);

                List<string> folderQueueList = new List<string>(folderQueueArray);
                folderQueueList.RemoveAt(e.ItemIndex);

                string[] queueArray = queuedMedia.ToArray();
                List<string> queueList = new List<string>(queueArray);
                queueList.RemoveAt(e.ItemIndex);

                folderQueue = new Queue<string>(folderQueueList);
                queuedMedia = new Queue<string>(queueList);
                
                UpdateQueuedMediaListView();
            }
        }

        private void ShuffleQueueButton_Click(object sender, EventArgs e)
        {
            string[] queueArray = queuedMedia.ToArray();
            string[] folderQueueArray = folderQueue.ToArray();

            Random r = new Random(DateTime.Now.Millisecond + DateTime.Now.Day);
            for (int i = 0; i < queueArray.Length; i++)
            {
                int randomI = r.Next(queueArray.Length);
                (queueArray[i], queueArray[randomI]) = (queueArray[randomI], queueArray[i]);
                (folderQueueArray[i], folderQueueArray[randomI]) = (folderQueueArray[randomI], folderQueueArray[i]);
            }

            queuedMedia = new Queue<string>(queueArray);
            folderQueue = new Queue<string>(folderQueueArray);
            UpdateQueuedMediaListView();
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
