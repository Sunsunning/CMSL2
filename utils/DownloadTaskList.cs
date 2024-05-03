using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSL.utils
{
    public class DownloadTaskList
    {
        private List<DownloadItem> items;
        public DownloadTaskList()
        {
            items = new List<DownloadItem>();
        }
        public void AddItem(DownloadItem item)
        {
            foreach (var downloadItem in items)
            {
                if (downloadItem != null && item != null) { 
                    if (downloadItem.id == item.id)
                    {
                        throw new Exception("重复ID这是不允许的标签");
                    }
                    items.Add(downloadItem);
                }
            }
        }
        

        public class DownloadItem
        {
            public int id;
            public string name;
            public string url;
            public string description;
        }
    }
}
