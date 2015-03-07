using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DBManager.Base
{
    interface IDBOperate
    {
        DbConnection connection { get; set; }
        void OpenConnection(DBModel model);
        void ExecuteSqlCmd( string cmdStr);
        void ExecuteCmdFile(string filepath);
        void CloseConnection();
    }
}
