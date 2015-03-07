
using DBManager.Base;
//using DBManager.Utils;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
//using System.Data.OracleClient;
using System.IO;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DBManager
{
    /// <summary>
    /// oracle操作类
    /// </summary>
    class OraceDBOperate:IDBOperate
    {

        DbConnection _connection = null;


        public DbConnection connection
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
            string connStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + model.IP + ")(PORT=" + model.Port + "))(CONNECT_DATA=(SERVICE_NAME=" + (model.Instance == string.Empty ? "orcl" : model.Instance) + ")));User Id=" + model.User + ";Password=" + model.Pwd + ";";
            if (model.User == "sys")
            {
                connStr += "DBA PRIVILEGE=sysdba";
            }


            this._connection = new OracleConnection(connStr);

            if (this._connection.State == ConnectionState.Closed)
            {
                this._connection.Open();
            }
        }

        public void ExecuteSqlCmd(string cmdStr)
        {
            //OracleTransaction trans = connection.BeginTransaction();
            OracleCommand cmd = ((OracleConnection)this._connection).CreateCommand();
            try
            {
                cmd.CommandText = cmdStr;
                // cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                //trans.Commit();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            using (StreamReader reader = new StreamReader(filepath, Encoding.Default))
            {
                string sqlCmd = "";
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("--") || line.StartsWith("prompt") || line.StartsWith("spool") || line.StartsWith("set feedback") || line.StartsWith("set define"))
                    {
                        continue;
                    }
                    else if (line.EndsWith(";") && !slashStart)
                    {
                        sqlCmd += line.Substring(0, line.Length - 1);
                        this.ExecuteSqlCmd(sqlCmd);

                        sqlCmd = "";
                    }
                    else if (line == "/")
                    {
                        // 第一次/
                        if (!slashStart)
                        {

                            slashStart = true;
                        }
                        if (sqlCmd != string.Empty)
                        {
                            this.ExecuteSqlCmd(sqlCmd);
                            sqlCmd = "";
                        }

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
