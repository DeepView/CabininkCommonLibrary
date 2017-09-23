using System;
using System.Data;
using System.Data.SQLite;
using Cabinink.TypeExtend;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.DataTreatment.Database.Extend
{
   /// <summary>
   /// SQLite触发器类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class SQLiteTrigger : IDbOperatorSwitch, IDisposable
   {
      private SQLiteDBOIEncapsulation _sqliteDbOperator;//用于操作指定数据库的SQLite数据库操作实例。
      private string _triggerName;//当前触发器的名称。
      private ESQLiteTriggerAction _triggerAction;//触发器需要执行的操作，这个操作允许为删除、更新与插入。
      private ESQLiteWhenExecuteTrigger _whenExecute;//当前触发器在执行操作之前还是之后执行。
      private ExString _preCondition;//预置条件。
      private ExString _code;//触发器事件，就是触发器需要执行的具体操作。
      /// <summary>
      /// 构造函数，创建一个指定的数据库操作实例、触发器名称、执行动作、触发先后时间和预置条件的触发器实例。
      /// </summary>
      /// <param name="sqliteDbOperator">指定的数据库操作实例，这个实例定义了需要操作的SQLite数据库。</param>
      /// <param name="triggerName">需要定义的触发器的名称。</param>
      /// <param name="action">指定触发器执行的动作，SQLite 的触发器（Trigger）可以指定在特定的数据库表发生DELETE、INSERT或UPDATE时触发，或在一个或多个指定表的列发生更新时触发。</param>
      /// <param name="whenExecute">当前触发器在执行操作之前还是之后执行。</param>
      /// <param name="preCondition">触发器被触发的先决条件。</param>
      public SQLiteTrigger(SQLiteDBOIEncapsulation sqliteDbOperator, string triggerName, ESQLiteTriggerAction action, ESQLiteWhenExecuteTrigger whenExecute, string preCondition)
      {
         _sqliteDbOperator = sqliteDbOperator;
         _triggerName = triggerName;
         _triggerAction = action;
         _whenExecute = whenExecute;
         _preCondition = preCondition;
         _code = string.Empty;
      }
      /// <summary>
      /// 获取或设置当前触发器的数据库操作实例。
      /// </summary>
      public SQLiteDBOIEncapsulation SQLiteOperator { get => _sqliteDbOperator; set => _sqliteDbOperator = value; }
      /// <summary>
      /// 获取或设置当前触发器的名称。
      /// </summary>
      public string Name { get => _triggerName; set => _triggerName = value; }
      /// <summary>
      /// 获取或设置当前触发器在满足条件之后需要执行的操作。
      /// </summary>
      public ESQLiteTriggerAction Action { get => _triggerAction; set => _triggerAction = value; }
      /// <summary>
      /// 获取或设置当前触发器在执行操作之前还是之后执行。
      /// </summary>
      public ESQLiteWhenExecuteTrigger WhenExecute { get => _whenExecute; set => _whenExecute = value; }
      /// <summary>
      /// 获取或设置当前触发器执行操作需要满足的先决条件。
      /// </summary>
      public ExString PresetCondition { get => _preCondition; set => _preCondition = value; }
      /// <summary>
      /// 获取或设置当前触发器在满足条件之后需要执行的操作。
      /// </summary>
      public ExString ExecutedCode { get => _code; set => _code = value; }
      /// <summary>
      /// 启用数据库操作实例，如果不执行这个操作来进行触发器的一些核心操作，将会出现一些异常。
      /// </summary>
      /// <returns>这个操作会返回当前被操作数据库的连接状态。</returns>
      public ConnectionState EnableOperator()
      {
         SQLiteOperator.InitializeConnection();
         SQLiteOperator.Connect();
         return SQLiteOperator.ConnectionStatus;
      }
      /// <summary>
      /// 禁用数据库操作实例。
      /// </summary>
      /// <returns>这个操作会返回当前被操作数据库的连接状态。</returns>
      public ConnectionState DisableOperator()
      {
         SQLiteOperator.Disconnect();
         return SQLiteOperator.ConnectionStatus;
      }
      /// <summary>
      /// 获取当前实例所连接的数据库中的所有的触发器名称。
      /// </summary>
      /// <returns>这个操作会返回一个触发器名称列表。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库连接出现丢失或者其他状态时，则会抛出这个异常。</exception>
      public List<string> GetTriggers()
      {
         if (SQLiteOperator.ConnectionStatus != ConnectionState.Open) throw new ConnectionNotExistsException();
         List<string> triggers = new List<string>();
         SQLiteDataReader reader = SQLiteOperator.ExecuteSqlToReader("select name from sqlite_master where type = 'trigger';");
         while (reader.Read()) triggers.Add(reader.GetString(reader.GetOrdinal("name")));
         return triggers;
      }
      /// <summary>
      /// 获取当前实例所连接的数据库中关联指定表格的所有的触发器名称。
      /// </summary>
      /// <param name="tableName">数据表名称，指定这个值可以限制需要获取的触发器名称。</param>
      /// <returns>这个操作会返回一个满足条件的触发器名称列表。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库连接出现丢失或者其他状态时，则会抛出这个异常。</exception>
      public List<string> GetTriggers(string tableName)
      {
         if (SQLiteOperator.ConnectionStatus != ConnectionState.Open) throw new ConnectionNotExistsException();
         List<string> triggers = new List<string>();
         SQLiteDataReader reader = SQLiteOperator.ExecuteSqlToReader("select name from sqlite_master where type = 'trigger' and tbl_name = '" + tableName + "';");
         while (reader.Read()) triggers.Add(reader.GetString(reader.GetOrdinal("name")));
         return triggers;
      }
      /// <summary>
      /// 创建一个触发器。
      /// </summary>
      /// <param name="tableName">指定需要触发操作的数据表。</param>
      /// <returns>该操作会返回整个数据库数据受影响的行数。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库连接出现丢失或者其他状态时，则会抛出这个异常。</exception>
      public int Create(string tableName)
      {
         if (SQLiteOperator.ConnectionStatus != ConnectionState.Open) throw new ConnectionNotExistsException();
         else return SQLiteOperator.ExecuteSql(BuildSqlSentence(tableName));
      }
      /// <summary>
      /// 根据当前实例所包含触发器名称来移除一个触发器。
      /// </summary>
      /// <returns>该操作会返回整个数据库数据受影响的行数。</returns>
      /// <exception cref="ConnectionNotExistsException">当数据库连接出现丢失或者其他状态时，则会抛出这个异常。</exception>
      public int Remove()
      {
         if (SQLiteOperator.ConnectionStatus != ConnectionState.Open) throw new ConnectionNotExistsException();
         else return SQLiteOperator.ExecuteSql("drop trigger " + Name);
      }
      /// <summary>
      /// 组建一个触发器的SQL语句。
      /// </summary>
      /// <param name="tableName">指定需要触发操作的数据表。</param>
      /// <returns>这个操作会返回一个创建触发器的SQL语句。</returns>
      /// <exception cref="SqlGrammarErrorException">当触发器需要执行的操作为空时，则会抛出这个异常。</exception>
      private string BuildSqlSentence(string tableName)
      {
         string sqlSentence = string.Empty;
         string executeEvent = sqlSentence;
         string whenExecute = WhenExecute == ESQLiteWhenExecuteTrigger.Before ? @"before" : @"after";
         switch (Action)
         {
            case ESQLiteTriggerAction.WhenDeleted:
               executeEvent = @"delete";
               break;
            case ESQLiteTriggerAction.WhenUpdated:
               executeEvent = @"update";
               break;
            case ESQLiteTriggerAction.WhenInserted:
               executeEvent = @"insert";
               break;
            default:
               break;
         }
         if (string.IsNullOrEmpty(ExecutedCode)) throw new SqlGrammarErrorException();
         else
         {
            if (string.IsNullOrEmpty(PresetCondition))
            {
               sqlSentence = "create trigger " + Name + " \n" + whenExecute + " " + executeEvent + " on " +
                  tableName + " \nfor each row \nbegin \n" + ExecutedCode + " \nend;";
            }
            else
            {
               sqlSentence = "create trigger " + Name + " \n" + whenExecute + " " + executeEvent + " on " +
                  tableName + " when " + PresetCondition + " \nfor each row \nbegin \n" + ExecutedCode + " \nend;";
            }
         }
         return sqlSentence;
      }
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose()
      {
         SQLiteOperator.Disconnect();
         SQLiteOperator = null;
         Name = null;
         PresetCondition.Dispose();
         ExecutedCode.Dispose();
      }
   }
   /// <summary>
   /// 触发器的触发动作集合枚举，SQLite的触发器（Trigger）可以指定在特定的数据库表发生 DELETE、INSERT 或 UPDATE 时触发，或在一个或多个指定表的列发生更新时触发。
   /// </summary>
   public enum ESQLiteTriggerAction : int
   {
      /// <summary>
      /// 当删除记录的时候触发动作。
      /// </summary>
      WhenDeleted = 0x0000,
      /// <summary>
      /// 当更新数据表的时候触发动作。
      /// </summary>
      WhenUpdated = 0x0001,
      /// <summary>
      /// 当在数据表中插入新的数据的时候触发动作。
      /// </summary>
      WhenInserted = 0x0002
   }
   /// <summary>
   /// 触发器执行的时间枚举，决定何时执行触发器动作，决定是在关联行的插入、修改或删除之前或者之后执行触发器动作。
   /// </summary>
   public enum ESQLiteWhenExecuteTrigger : int
   {
      /// <summary>
      /// 指示在执行ESQLiteTriggerAction指定动作之前执行操作。
      /// </summary>
      Before = 0x0000,
      /// <summary>
      /// 指示在执行ESQLiteTriggerAction指定动作之后执行操作。
      /// </summary>
      After = 0xffff
   }
}
