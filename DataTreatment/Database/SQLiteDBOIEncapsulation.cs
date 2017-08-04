using System;
using System.IO;
using System.Data;
using Cabinink.IOSystem;
using System.Data.SQLite;
using System.Data.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.DataTreatment.Database
{
   /// <summary>
   /// SQLite数据库操作接口封装应用类，用于实现最常用的SQLite数据库操作。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class SQLiteDBOIEncapsulation : IDatabasesOperationBase
   {
      private string _dbConnectionString;//数据库连接字符串。
      private SQLiteCommand _sqlCommand;//SQLite数据库查询执行类。
      private SQLiteConnection _sqlConnection;//SQLite数据库连接类。
      /// <summary>
      /// 构造函数，通过一个指定的数据库文件地址来初始化当前实例。
      /// </summary>
      /// <param name="dbFileUrl">指定的数据库文件地址，如果这个文件地址无效则会抛出FileNotFoundException异常。</param>
      /// <exception cref="FileNotFoundException">如果找不到dbFileUrl参数指定的数据库文件，则会抛出这个异常。</exception>
      public SQLiteDBOIEncapsulation(Uri dbFileUrl)
      {
         if (FileOperator.FileExists(dbFileUrl.LocalPath) == false) throw new FileNotFoundException("找不到数据库文件！");
         _dbConnectionString = "Data Source=" + dbFileUrl.LocalPath;
      }
      /// <summary>
      /// 构造函数，通过一个指定的数据库连接字符串地址来初始化当前实例。
      /// </summary>
      /// <param name="dbConnectionString">指定的数据库连接字符串。</param>
      public SQLiteDBOIEncapsulation(string dbConnectionString)
      {
         _dbConnectionString = dbConnectionString;
      }
      /// <summary>
      /// 获取或设置当前数据库的连接字符串。
      /// </summary>
      /// <exception cref="FileNotFoundException">如果找不到value参数指定的数据库文件，则会抛出这个异常。</exception>
      public string ConnectionString
      {
         get => _dbConnectionString;
         set
         {
            string furl = value.Substring("Data Source=".Length);
            if (FileOperator.FileExists(furl) == false)
            {
               throw new FileNotFoundException("找不到数据库文件！");
            }
            _dbConnectionString = value;
         }
      }
      /// <summary>
      /// 获取或设置当前数据库的源文件地址（set代码对于其他程序集不可见）。
      /// </summary>
      public string SourceUri
      {
         get
         {
            string furl = _dbConnectionString.Substring("Data Source=".Length);
            return furl;
         }
         internal set => _dbConnectionString = "Data Source=" + value;
      }
      /// <summary>
      /// 获取或设置当前实例的SQLiteConnection实例。
      /// </summary>
      public SQLiteConnection DbConnector
      {
         get => _sqlConnection; set => _sqlConnection = value;
      }
      /// <summary>
      /// 获取当前数据库的连接状态。
      /// </summary>
      public ConnectionState ConnectionStatus => DbConnector.State;
      /// <summary>
      /// 初始化当前的数据库连接。
      /// </summary>
      /// <returns>如果初始化成功则会返回true，若被该方法捕获了一些无法处理的异常则会视为初始化失败，并返回false。</returns>
      public bool InitializeConnection()
      {
         bool isconned = true;
         try
         {
            DbConnector = new SQLiteConnection(ConnectionString);
            _sqlCommand = new SQLiteCommand(DbConnector);
         }
         catch (Exception ex)
         {
            if (ex != null) isconned = false;
         }
         return isconned;
      }
      /// <summary>
      /// 连接当前实例指定的数据库。
      /// </summary>
      public void Connect() => DbConnector.Open();
      /// <summary>
      /// 执行指定的SQL语句。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <returns>如果该方法未产生任何异常，则会返回数据表里面受影响的数据行。</returns>
      /// <exception cref="EmptySqlCommandLineException">当出现空的SQL语句时，则会抛出这个异常。</exception>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      /// <exception cref="SqlGrammarErrorException">当SQL语法出现错误时，则会抛出这个异常。</exception>
      public int ExecuteSql(string sqlSentence)
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
            if (ex != null) throw new SqlGrammarErrorException();
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
      public int ExecuteSql(string sqlSentence, IList<SQLiteParameter> parameters)
      {
         int affectedRows = 0;
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         DbTransaction transaction = DbConnector.BeginTransaction();
         SQLiteCommand command = new SQLiteCommand(DbConnector)
         {
            CommandText = sqlSentence
         };
         if (!(parameters == null || parameters.Count == 0))
         {
            foreach (SQLiteParameter parameter in parameters) command.Parameters.Add(parameter);
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
      public DataTable ExecuteSqlToDataTable(string sqlSentence, IList<SQLiteParameter> parameters)
      {
         SQLiteCommand command = new SQLiteCommand(sqlSentence, DbConnector);
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         if (!(parameters == null || parameters.Count == 0))
         {
            foreach (SQLiteParameter parameter in parameters) command.Parameters.Add(parameter);
         }
         SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
         DataTable data = new DataTable();
         adapter.Fill(data);
         return data;
      }
      /// <summary>
      /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的SQLiteDataReader实例。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public SQLiteDataReader ExecuteSqlToReader(string sqlSentence)
      {
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         SQLiteCommand command = new SQLiteCommand(sqlSentence, DbConnector);
         return command.ExecuteReader();
      }
      /// <summary>
      /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
      /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的SQLiteDataReader实例。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public SQLiteDataReader ExecuteSqlToReader(string sqlSentence, IList<SQLiteParameter> parameters)
      {
         SQLiteCommand command = new SQLiteCommand(sqlSentence, DbConnector);
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         if (!(parameters == null || parameters.Count == 0))
         {
            foreach (SQLiteParameter parameter in parameters) command.Parameters.Add(parameter);
         }
         Connect();
         return command.ExecuteReader(CommandBehavior.CloseConnection);
      }
      /// <summary>
      /// 检查当前数据库中指定的数据表格是否存在。
      /// </summary>
      /// <param name="tableName">需要被检查是否存在的表格。</param>
      /// <returns>如果这个表格存在，则返回true，否则将返回false。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public bool DataTableExists(string tableName)
      {
         bool exists = true;
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         _sqlCommand = DbConnector.CreateCommand();
         _sqlCommand.CommandText = "select count(*) from sqlite_master where type='table' and name='" + tableName + "';";
         if (Convert.ToInt32(_sqlCommand.ExecuteScalar()) == 0) exists = false;
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
         List<string> dtnames = new List<string>();
         if (ConnectionStatus == ConnectionState.Closed) Connect();
         SQLiteDataReader reader = ExecuteSqlToReader(@"SELECT name FROM sqlite_master WHERE type='table'");
         while (reader.Read()) dtnames.Add(reader.GetString(0));
         return dtnames;
      }
      /// <summary>     
      /// 查询数据库中的所有数据类型信息。
      /// </summary>     
      /// <returns>操作成功之后会返回所查询数据库中的所有数据类型的相关信息。</returns>     
      /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
      public DataTable GetSchema()
      {
         if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
         DataTable data = DbConnector.GetSchema("TABLES");
         return data;
      }
      /// <summary>
      /// 为当前数据库设置访问密码。
      /// </summary>
      /// <param name="password">即将生效的数据库访问密码。</param>
      public void SetDbPassword(string password)
      {
         DbConnector.SetPassword(password);
      }
      /// <summary>
      /// 更新当前数据库的访问密码。
      /// </summary>
      /// <param name="password">用于重新生效的数据库访问密码。</param>
      public void UpgradePassword(string password)
      {
         DbConnector.ChangePassword(password);
      }
      /// <summary>
      /// 断开当前数据库的连接。
      /// </summary>
      public void Disconnect() => DbConnector.Close();
      /// <summary>
      /// 根据指定的路径创建一个SQLite数据库。
      /// </summary>
      /// <param name="dbFileUrl">需要被创建的数据库的路径。</param>
      public static void CreateDatabase(string dbFileUrl)
      {
         using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbFileUrl))
         {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
               command.CommandText = "create table demo(id integer not null primary key autoincrement unique)";
               command.ExecuteNonQuery();
               command.CommandText = "drop table demo";
               command.ExecuteNonQuery();
            }
         }
      }
      /// <summary>
      /// 获取当前SQLiteDBOIEncapsulation实例的完整类名的字符串表达形式。
      /// </summary>
      /// <returns>该操作会返回一个当前实例的完整类名的字符串表达形式。</returns>
      public override string ToString()
      {
         return "Cabinink.DataTreatment.Database.SQLiteDBOIEncapsulation";
      }
   }
   /// <summary>
   /// 当SQL语句为NULL的时候抛出的异常。
   /// </summary>
   [Serializable]
   public class EmptySqlCommandLineException : Exception
   {
      public EmptySqlCommandLineException() : base("不允许空的SQL语句！") { }
      public EmptySqlCommandLineException(string message) : base(message) { }
      public EmptySqlCommandLineException(string message, Exception inner) : base(message, inner) { }
      protected EmptySqlCommandLineException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当SQL语句出现语法错误或者其他异常情况的时候而抛出的异常。
   /// </summary>
   [Serializable]
   public class SqlGrammarErrorException : Exception
   {
      public SqlGrammarErrorException() : base("SQL语法错误或者出现了其他异常！") { }
      public SqlGrammarErrorException(string message) : base(message) { }
      public SqlGrammarErrorException(string message, Exception inner) : base(message, inner) { }
      protected SqlGrammarErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 数据库连接不存在或者已断开连接时而抛出的异常。
   /// </summary>
   [Serializable]
   public class ConnectionNotExistsException : Exception
   {
      public ConnectionNotExistsException() : base("数据库未连接或者连接已断开！") { }
      public ConnectionNotExistsException(string message) : base(message) { }
      public ConnectionNotExistsException(string message, Exception inner) : base(message, inner) { }
      protected ConnectionNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
