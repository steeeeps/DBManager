using DBManager.Base;
//using DBManager.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DBManager.Base
{
    class SqlServerDBOperate:IDBOperate
    {
        DbConnection _connection = null;



        public System.Data.Common.DbConnection connection
        {
            get
            {
                return this._connection;
            }
            set
            {
                this._connection = value;
            }
        }

        public void OpenConnection(DBModel model)
        {
            //sqlserver 解决远程连接不上的问题http://blog.csdn.net/cupid0051/article/details/6750736
            string connStr = "Data Source=" + model.IP + "," + model.Port + ";Integrated Security=False;Initial Catalog=" + (model.Instance == string.Empty ? "" : model.Instance) + ";User ID=" + model.User + ";Password=" + model.Pwd + ";";
           
            this._connection = new SqlConnection(connStr);
            if (this._connection.State == ConnectionState.Closed)
                this._connection.Open();
        }

        public void ExecuteSqlCmd(string cmdStr)
        {
            SqlCommand cmd = (this._connection as SqlConnection).CreateCommand();
            try
            {
                cmd.CommandText = cmdStr;
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("该错误发生于下列sql语句: " + cmdStr);
                //throw (e);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public void ExecuteCmdFile(string path)
        {
            string filepath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, path);
            using (StreamReader reader = new StreamReader(filepath, Encoding.Default))
            {
                string sqlCmd = "";
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("--") || line == string.Empty || line.StartsWith("/**") || line.StartsWith("print"))
                    {
                        continue;
                    }
                    else if (line == "GO")
                    {
                        //sqlCmd += line;
                        this.ExecuteSqlCmd( sqlCmd);
                        sqlCmd = "";
                    }
                    else
                    {
                        sqlCmd += line + "\r\n";
                    }
                }
            }
        }

        public void CloseConnection()
        {
            if (this._connection.State == ConnectionState.Open)
                this._connection.Close();
        }
    }
}
