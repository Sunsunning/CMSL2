using CMSL.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSL.utils
{
    public class DownloadTaskList
    {
        private List<DownloadTaskListItem> items;
        public static event EventHandler<DownloadTaskListItem> OnDownloadTaskList;
        public static event EventHandler<string> OnProcessDownload;
        public DownloadTaskList()
        {
            items = new List<DownloadTaskListItem>();
        }
        public int getCount()
        {
            return items.Count;
        }

        public void AddItem(DownloadTaskListItem item)
        {
            item.id = items.Count + 1;
            items.Add(item);
            OnDownloadTaskList(this, item);
        }
        
        public void RemoveItem(string name)
        {
            foreach (var downloadItem in items)
            {
                if (downloadItem == null || downloadItem.name == null) continue;
                if (downloadItem.name == name)
                {
                    items.Remove(downloadItem);
                    OnProcessDownload(this, name);
                }
            }   
        }

        public class DownloadTaskListItem
        {
            public int id;
            public string name;
            public string url;
            public string description;
        }
    }
}
