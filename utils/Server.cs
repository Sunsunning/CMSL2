using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSL.utils
{
    class Server
    {
        public enum Side
        {
            Spigot,
            Paper
        }

        public string _name { get; private set; }
        public string _version { get; private set; }
        public Side _side { get; private set; }

        public Server(string name, string version, Side side)
        {
            _name = name;
            _version = version;
            _side = side;
        }

    }
}
