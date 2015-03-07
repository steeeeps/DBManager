# DBManager
use c# to execute sql files to database： oracle、sqlserver、postgresql  

how to use:  

		    DBModel model = new DBModel("Oracle", "127.0.0.1", "1521", "sys", "admin", "", "orcl");
            string filepath = "test.sql";
            IDBOperate dboperate = null;
            if (model.Type == "Oracle")
            {
                dboperate = new OraceDBOperate();
            }
            else if (model.Type == "Postgresql")
            {
                dboperate = new PostgreSqlDBOperate();
            }
            else if (model.Type == "Sqlserver")
            {
                dboperate = new SqlServerDBOperate();
            }
            try
            {
                dboperate.OpenConnection(model);
                dboperate.ExecuteCmdFile(filepath);
                dboperate.CloseConnection();
            }
            catch (Exception ex)
            {
                throw (ex);
            }


----------

get more in ：[http://steeeeps.net/2015/03/07/csharp-exec-sql-files/](http://steeeeps.net/2015/03/07/csharp-exec-sql-files/)

   

