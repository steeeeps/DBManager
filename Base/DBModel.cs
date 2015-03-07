using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DBManager
{
    class DBModel
    {
        public string Type { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        private string _dir;
        public string Dir
        {
            get
            {
                return _dir;

            }
            set
            {
                if (value == string.Empty)
                {
                    _dir = "";
                }
                else
                {
                    _dir = value.EndsWith("\\") ? value : value + "\\";
                }
            }
        }

        public string Instance { get; set; }
        public DBModel(string type, string ip, string port, string user, string pwd, string dir,string instance)
        {
            this.Type = type;
            this.IP = ip;
            this.Port = port;
            this.User = user;
            this.Pwd = pwd;
            this.Dir = dir;
            this.Instance = instance;
        }
    }

    public enum DbType : int
    {
        oracle = 1,
        postgresql = 2,
        sqlserver = 3
    }
}
