using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSL.utils
{

    class SelectedServer
    {
        public Server SelServer { get; private set; } = null;
        public bool IsSelectedServer()
        {
            if (SelServer == null)
            {
                return false;
            }
            if (SelServer != null)
            {
                return true;
            }
            return false;
        }

        public void SelectionServer(Server server)
        {
            string path;
            if (server._side.Equals(Server.Side.Spigot))
            {
                path = @"D:\CMSL\Spigot";
            }else
            {
                path = @"D:\CMSL\Paper";
            }

            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                FileInfo[] fileInfos = directory.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (fileInfo.Name == server._name)
                    {
                        SelServer = server;
                        break;
                    }
                }
            }else
            {
                throw new DirectoryNotFoundException("未找到指定文件夹");
            }
        }
    }
}
