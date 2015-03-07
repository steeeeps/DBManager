
using DBManager.Base;
//using DBManager.Utils;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Threading.Tasks;

namespace DBManager
{
    /// <summary>
    /// PostgreSql 操作类
    /// </summary>
    class PostgreSqlDBOperate:IDBOperate
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
            string connStr = "Server=" + model.IP + ";Port=" + model.Port + ";User Id=" + model.User + ";Password=" + model.Pwd + ";Database=" + (model.Instance == "" ? "postgres" : model.Instance) + ";";
            this._connection = new NpgsqlConnection(connStr);
            if (this._connection.State == ConnectionState.Closed)
            {
                this._connection.Open();
            }
        }

        public void ExecuteSqlCmd(string cmdStr)
        {
            //NpgsqlTransaction trans = connection.BeginTransaction();
            NpgsqlCommand cmd = (this._connection as NpgsqlConnection).CreateCommand();
            try
            {
                cmd.CommandText = cmdStr;
                //cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                //trans.Commit();
            }
            catch (Exception e)
            {
                   Console.WriteLine(e.Message );
                   Console.WriteLine("该错误发生于下列sql语句: " + cmdStr);
                //throw (e);
            }
            finally
            {
                // trans.Dispose();
                cmd.Dispose();
            }
        }

        public void ExecuteCmdFile(string path)
        {
           
            string filepath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, path);
            
            bool slashStart = false;
            using (StreamReader reader = new StreamReader(filepath, Encoding.UTF8))
            {
              
                string sqlCmd = "";
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("--") || line == string.Empty)
                    {
                        continue;
                    }
                    else if (line.EndsWith(";") && !slashStart)
                    {
                        sqlCmd += line.Substring(0, line.Length - 1);
                        this.ExecuteSqlCmd(sqlCmd);
                        sqlCmd = "";
                    }
                    else if (line.StartsWith("CREATE FUNCTION") && !slashStart)
                    {
                        slashStart = true;
                        sqlCmd += line + "\r\n";
                    }
                    else if (line == "$$;" && slashStart)
                    {
                        sqlCmd += line + "\r\n";
                        this.ExecuteSqlCmd(sqlCmd);
                        sqlCmd = "";
                        slashStart = false;
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
            {
                this._connection.Close();
            }
        }
    }
}
