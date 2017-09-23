using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Cabinink.IOSystem;
using Cabinink.TypeExtend;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cabinink.DataTreatment.DocumentData;
namespace Cabinink.DataTreatment.Database
{
   /// <summary>
   /// Micrsoft Access 97-2003数据库操作接口封装应用类，用于实现最常用的Micrsoft Access 97-2003数据库操作。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class MSAccessDBOIEncapsulation : DBOIEncapsulation
   {
      private OleDbCommand _sqlCommand;//Micrsoft Access 97-2003数据库查询执行类。
      private OleDbConnection _sqlConnection;//Micrsoft Access 97-2003数据库连接类。
      /// <summary>
      /// 构造函数，通过一个指定的数据库连接字符串地址来初始化当前实例。
      /// </summary>
      /// <param name="dbConnectionString">指定的数据库连接字符串。</param>
      public MSAccessDBOIEncapsulation(string dbConnectionString) : base(dbConnectionString)
      {
         _dbConnectionString = dbConnectionString;
      }
      /// <summary>
      /// 构造函数，通过一个指定的数据库文件地址来初始化当前实例。
      /// </summary>
      /// <param name="dbFileUrl">指定的数据库文件地址，如果这个文件地址无效则会抛出FileNotFoundException异常。</param>
      /// <exception cref="FileNotFoundException">如果找不到dbFileUrl参数指定的数据库文件，则会抛出这个异常。</exception>
      /// <exception cref="FileTypeNotLegitimateException">如果文件类型不符合匹配条件，则将会抛出这个异常。</exception>
      public MSAccessDBOIEncapsulation(Uri dbFileUrl) : base()
      {
         if (FileOperator.FileExists(dbFileUrl.LocalPath) == false) throw new FileNotFoundException("找不到数据库文件！");
         if (FileOperator.GetFileExtension(dbFileUrl.LocalPath) != ".mdb") throw new FileTypeNotLegitimateException();
         _dbConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + @dbFileUrl.LocalPath;
      }
      /// <summary>
      /// 构造函数，通过一个指定的数据库文件地址和数据库访问密码来初始化当前实例。
      /// </summary>
      /// <param name="dbFileUrl">指定的数据库文件地址，如果这个文件地址无效则会抛出FileNotFoundException异常。</param>
      /// <param name="dbPassword">需要访问的数据库的访问密码，如果这个密码不正确，可能会引发一些无法在当前构造函数捕获的异常。</param>
      /// <exception cref="FileNotFoundException">如果找不到dbFileUrl参数指定的数据库文件，则会抛出这个异常。</exception>
      /// <exception cref="FileTypeNotLegitimateException">如果文件类型不符合匹配条件，则将会抛出这个异常。</exception>
      public MSAccessDBOIEncapsulation(Uri dbFileUrl, ExString dbPassword)
      {
         if (FileOperator.FileExists(dbFileUrl.LocalPath) == false) throw new FileNotFoundException("找不到数据库文件！");
         if (FileOperator.GetFileExtension(dbFileUrl.LocalPath) != ".mdb") throw new FileTypeNotLegitimateException();
         _dbConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + @dbFileUrl.LocalPath + @";Jet OLEDB:Database Password=" + dbPassword;
      }
      /// <summary>
      /// 获取或设置当前实例的OleDbConnection实例。
      /// </summary>
      public OleDbConnection DbConnector
      {
         get => _sqlConnection; set => _sqlConnection = value;
      }
      /// <summary>
      /// 获取当前数据库的连接状态。
      /// </summary>
      public override ConnectionState ConnectionStatus => DbConnector.State;
      /// <summary>
      /// 获取当前数据库的源文件地址。
      /// </summary>
      public string SourceUri => DbConnector.DataSource;
      /// <summary>
      /// 获取当前数据库的名称或者启动连接后要使用的数据库名称。
      /// </summary>
      public string DatabaseName => DbConnector.Database;
      /// <summary>
      /// 获取在指定的OLEDB提供程序的名称“提供程序=”连接字符串的子句。
      /// </summary>
      public string ProviderString => DbConnector.Provider;
      /// <summary>
      /// 获取或设置当前数据库的连接字符串。
      /// </summary>
      public override string ConnectionString
      {
         get => _dbConnectionString; set => _dbConnectionString = value;
      }
      /// <summary>
      /// 连接当前实例指定的数据库。
      /// </summary>
      public override void Connect() => DbConnector.Open();
      /// <summary>
      /// 断开当前数据库的连接。
      /// </summary>
      public override void Disconnect() => DbConnector.Close();
      /// <summary>
      /// 执行指定的SQL语句。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <returns>如果该方法未产生任何异常，则会返回数据表里面受影响的数据行。</returns>
      /// <exception cref="EmptySqlCommandLineException">当出现空的SQL语句时，则会抛出这个异常。</exception>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      /// <exception cref="SqlGrammarErrorException">当SQL语法出现错误时，则会抛出这个异常。</exception>
      public override int ExecuteSql(string sqlSentence)
      {
         if (sqlSentence.Length == 0 || sqlSentence == null) throw new EmptySqlCommandLineException();
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         int impact = -1;
         _sqlCommand.CommandText = sqlSentence;
         try
         {
            impact = _sqlCommand.ExecuteNonQuery();
         }
         catch (Exception ex)
         {
            if (ex != null)
            {
               throw new SqlGrammarErrorException();
            }
         }
         return impact;
      }
      /// <summary>     
      /// 执行指定的SQL语句。
      /// </summary>     
      /// <param name="sqlSentence">要执行的增删改的SQL语句。</param>     
      /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>     
      /// <returns>对SQLite数据库执行增删改操作，返回受影响的行数。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public int ExecuteSql(string sqlSentence, IList<OleDbParameter> parameters)
      {
         int affectedRows = 0;
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         OleDbTransaction transaction = DbConnector.BeginTransaction();
         OleDbCommand command = new OleDbCommand(sqlSentence, DbConnector);
         if (!(parameters == null || parameters.Count == 0))
         {
            foreach (OleDbParameter parameter in parameters) command.Parameters.Add(parameter);
         }
         affectedRows = command.ExecuteNonQuery();
         transaction.Commit();
         return affectedRows;
      }
      /// <summary>
      /// 执行指定的SQL语句，返回一个包含查询结果的DataTable。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
      /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public DataTable ExecuteSqlToDataTable(string sqlSentence, IList<OleDbParameter> parameters)
      {
         OleDbCommand command = new OleDbCommand(sqlSentence, DbConnector);
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         if (!(parameters == null || parameters.Count == 0))
         {
            foreach (OleDbParameter parameter in parameters) command.Parameters.Add(parameter);
         }
         OleDbDataAdapter adapter = new OleDbDataAdapter(command);
         DataTable data = new DataTable();
         adapter.Fill(data);
         return data;
      }
      /// <summary>
      /// 执行一个查询语句，返回一个关联的OleDbDataReader实例。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的OleDbDataReader实例。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public OleDbDataReader ExecuteSqlToReader(string sqlSentence)
      {
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         _sqlCommand = new OleDbCommand(sqlSentence, DbConnector);
         return _sqlCommand.ExecuteReader();
      }
      /// <summary>
      /// 执行一个查询语句，返回一个关联的OleDbDataReader实例。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
      /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的OleDbDataReader实例。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public OleDbDataReader ExecuteSqlToReader(string sqlSentence, IList<OleDbParameter> parameters)
      {
         _sqlCommand = new OleDbCommand(sqlSentence, DbConnector);
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         if (!(parameters == null || parameters.Count == 0))
         {
            foreach (OleDbParameter parameter in parameters) _sqlCommand.Parameters.Add(parameter);
         }
         Connect();
         return _sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
      }
      /// <summary>
      /// 检查当前数据库中指定的数据表格是否存在。
      /// </summary>
      /// <param name="tableName">需要被检查是否存在的表格。</param>
      /// <returns>如果这个表格存在，则返回true，否则将返回false。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public bool DataTableExists(string tableName)
      {
         bool exists = false;
         List<string> tables = GetAllDataTableName();
         Parallel.For(0, tables.Count, (index, interrupt) =>
         {
            if (tables[index] == tableName)
            {
               exists = true;
               interrupt.Stop();
            }
         });
         return exists;
      }
      /// <summary>
      /// 根据指定条件判断指定数据表中的某一条记录是否存在。
      /// </summary>
      /// <param name="tableName">需要被判定记录存在性的数据表。</param>
      /// <param name="condition">用于判定的条件，这个条件会直接作为SQL语句的条件。</param>
      /// <returns>如果这条记录存在，则返回true，否则将返回false。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public bool RecordExists(string tableName, string condition)
      {
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         string sql = "select * from " + tableName + " where " + condition;
         return ExecuteSqlToReader(sql).HasRows;
      }
      /// <summary>
      /// 获取当前数据库中所有数据表的表名称。
      /// </summary>
      /// <returns>如果操作无异常，则会以列表实例的形式返回当前数据库中所有数据表的表名称。</returns>
      public List<string> GetAllDataTableName()
      {
         List<string> list = new List<string>();
         if (ConnectionStatus == ConnectionState.Closed) Connect();
         DataTable dt = DbConnector.GetSchema("Tables");
         foreach (DataRow row in dt.Rows) if (row[3].ToString() == "TABLE") list.Add(row[2].ToString());
         return list;
      }
      /// <summary>     
      /// 查询数据库中的所有数据类型信息。
      /// </summary>     
      /// <returns>操作成功之后会返回所查询数据库中的所有数据类型的相关信息。</returns>     
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public DataTable GetSchema()
      {
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         DataTable data = DbConnector.GetSchema("Tables");
         return data;
      }
      /// <summary>
      /// 初始化当前的数据库连接。
      /// </summary>
      /// <returns>如果初始化成功则会返回true，若被该方法捕获了一些无法处理的异常则会视为初始化失败，并返回false。</returns>
      public override bool InitializeConnection()
      {
         bool isconned = true;
         try
         {
            DbConnector = new OleDbConnection(ConnectionString);
            _sqlCommand = new OleDbCommand(string.Empty, DbConnector);
         }
         catch (Exception ex)
         {
            if (ex != null) isconned = false;
         }
         return isconned;
      }
      /// <summary>
      /// 更新当前数据库连接对象的连接状态属性。
      /// </summary>
      public void RefreshStatue() => DbConnector.ResetState();
   }
}
