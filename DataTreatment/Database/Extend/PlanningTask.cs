using System;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.DataTreatment.Database.Extend
{
   /// <summary>
   /// 数据库计划任务类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class PlanningTask : IDbOperatorSwitch, IDisposable
   {
      private DBOIEncapsulation _dbOperatorBaser;//数据库操作基类，这个类可以显式转换为其继承的任意子类。
      private string _plannigTaskName;//计划任务的名称。
      private DateTime _executeTime;//计划任务要执行的时间。
      private string _description;//计划任务的详细注释。
      private string _code;//计划任务需要执行的SQL语句。
      private int _maximalDelayed = 1000;//允许当前计划任务执行时间与实际时间的最大时延，默认值为1000，单位毫秒（Millisecond）。
      private bool _disposedValue = false; //要检测冗余调用。
      /// <summary>
      /// 构造函数，创建一个具有有效数据库操作类，计划任务名称，执行时间和注释的计划任务实例。
      /// </summary>
      /// <param name="dbOperator">有效的数据库操作实例。</param>
      /// <param name="planningTaskName">计划任务的名称，不过这个名称不允许为空。</param>
      /// <param name="executeTime">计划任务的执行时间。</param>
      /// <param name="description">计划任务的详细注释，相当于给其他用户或者管理员解释这个计划任务的具体细节或者其他相关注意事项。</param>
      public PlanningTask(DBOIEncapsulation dbOperator, string planningTaskName, DateTime executeTime, string description)
      {
         if (string.IsNullOrEmpty(planningTaskName))
         {
            throw new ArgumentException("计划任务名称不能为空！", "planningTaskName");
         }
         else
         {
            _dbOperatorBaser = dbOperator;
            _plannigTaskName = planningTaskName;
            _executeTime = executeTime;
            _description = description;
         }
      }
      /// <summary>
      /// 获取或设置当前计划任务的数据库操作实例。
      /// </summary>
      public DBOIEncapsulation DbOperator { get => _dbOperatorBaser; set => _dbOperatorBaser = value; }
      /// <summary>
      /// 获取或设置当前计划任务的名称。
      /// </summary>
      public string Name
      {
         get => _plannigTaskName;
         set
         {
            if (string.IsNullOrEmpty(value))
            {
               throw new ArgumentException("计划任务名称不能为空！", "planningTaskName");
            }
            else _plannigTaskName = value;
         }
      }
      /// <summary>
      /// 获取或设置当前计划任务的执行时间。
      /// </summary>
      public DateTime ExecuteTime { get => _executeTime; set => _executeTime = value; }
      /// <summary>
      /// 获取或设置当前计划任务的详细注释。
      /// </summary>
      public string Description { get => _description; set => _description = value; }
      /// <summary>
      /// 获取或设置当前计划任务需要执行的SQL操作代码。
      /// </summary>
      public string Code
      {
         get => _code;
         set
         {
            if (string.IsNullOrEmpty(value)) throw new SqlGrammarErrorException();
            else _code = value;
         }
      }
      /// <summary>
      /// 获取或设置允许当前计划任务执行时间与实际时间的最大时延，默认值为1000，计时单位为毫秒（Millisecond）。
      /// </summary>
      public int MaximalExecuteDelayed { get => _maximalDelayed; set => _maximalDelayed = value; }
      /// <summary>
      /// 启用数据库操作实例，如果不执行这个操作来进行触发器的一些核心操作，将会出现一些异常。
      /// </summary>
      /// <returns>这个操作会返回当前被操作数据库的连接状态。</returns>
      public ConnectionState EnableOperator()
      {
         DbOperator.InitializeConnection();
         DbOperator.Connect();
         return DbOperator.ConnectionStatus;
      }
      /// <summary>
      /// 禁用数据库操作实例。
      /// </summary>
      /// <returns>这个操作会返回当前被操作数据库的连接状态。</returns>
      public ConnectionState DisableOperator()
      {
         DbOperator.Disconnect();
         return DbOperator.ConnectionStatus;
      }
      /// <summary>
      /// 检测当前计划任务的执行时间是否已经到达。
      /// </summary>
      /// <param name="maxExecuteDelayed">计划任务执行的时间与当前时间的实际时延。</param>
      /// <returns>该操作会返回一个Boolean值，如果当前时间大于或者等于计划任务的执行时间，则会返回true，否则会返回false。</returns>
      public bool IsTimeOut(ref int maxExecuteDelayed)
      {
         maxExecuteDelayed = (int)(ExecuteTime.Ticks - DateTime.Now.Ticks);
         return DateTime.Now.Ticks >= ExecuteTime.Ticks ? true : false;
      }
      /// <summary>
      /// 执行当前的计划任务。
      /// </summary>
      /// <returns>该操作会返回计划任务所执行的数据库操作之后，数据表受影响的行数。</returns>
      /// <exception cref="DelayedIsOverflowException">当实际时延超出指定范围，则会抛出这个异常。</exception>
      public int Execute()
      {
         int affectedRows = -1;
         int maxExecuteDelayed = 0;
         if (!IsTimeOut(ref maxExecuteDelayed)) throw new DelayedIsOverflowException();
         else
         {
            if (maxExecuteDelayed > MaximalExecuteDelayed) throw new DelayedIsOverflowException();
            if (!(DbOperator.ConnectionStatus == ConnectionState.Open)) throw new ConnectionNotExistsException();
            else DbOperator.ExecuteSql(Code);
         }
         return affectedRows;
      }
      /// <summary>
      /// 执行当前的计划任务，并把执行之后的结果作为一个DbDataReader返回给用户。
      /// </summary>
      /// <returns>该操作会返回一个DbDataReader，可用于获取执行操作之后的详细数据。</returns>
      /// <exception cref="DelayedIsOverflowException">当实际时延超出指定范围，则会抛出这个异常。</exception>
      /// <exception cref="ConnectionNotExistsException">当数据库连接出现异常时，则会抛出这个异常。</exception>
      public DbDataReader ExecuteToReader()
      {
         DbDataReader reader = null;
         Type sqliteDbOptorType = new SQLiteDBOIEncapsulation(string.Empty).GetType();
         Type sqlserverDbOptorType = new SQLServerDBOIEncapsulation(string.Empty).GetType();
         Type msaccessDbOptorType = new MSAccessDBOIEncapsulation(string.Empty).GetType();
         int maxExecuteDelayed = 0;
         if (!IsTimeOut(ref maxExecuteDelayed)) throw new DelayedIsOverflowException();
         else
         {
            if (maxExecuteDelayed > MaximalExecuteDelayed) throw new DelayedIsOverflowException();
            if (!(DbOperator.ConnectionStatus == ConnectionState.Open)) throw new ConnectionNotExistsException();
            else
            {
               if (DbOperator.GetType() == sqliteDbOptorType)
               {
                  reader = ((SQLiteDBOIEncapsulation)DbOperator).ExecuteSqlToReader(Code);
               }
               if (DbOperator.GetType() == sqlserverDbOptorType)
               {
                  reader = ((SQLServerDBOIEncapsulation)DbOperator).ExecuteSqlToReader(Code);
               }
               if (DbOperator.GetType() == msaccessDbOptorType)
               {
                  reader = ((SQLServerDBOIEncapsulation)DbOperator).ExecuteSqlToReader(Code);
               }
            }
         }
         sqliteDbOptorType = null;
         sqlserverDbOptorType = null;
         msaccessDbOptorType = null;
         GC.KeepAlive(reader);
         GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false);
         return reader;
      }
      #region IDisposable Support
      /// <summary>
      /// 释放该对象引用的所有内存资源。
      /// </summary>
      /// <param name="disposing">用于指示是否释放托管资源。</param>
      protected virtual void Dispose(bool disposing)
      {
         int maxGeneration = GC.MaxGeneration;
         if (!_disposedValue)
         {
            if (disposing)
            {
               _plannigTaskName = null;
               _dbOperatorBaser = null;
               _description = null;
               _code = null;
               GC.Collect(maxGeneration, GCCollectionMode.Forced, true);
            }
            _disposedValue = true;
         }
      }
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose() => Dispose(true);
      #endregion
   }
   /// <summary>
   /// 当实际时延超出指定时延时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class DelayedIsOverflowException : Exception
   {
      public DelayedIsOverflowException() : base("实际时延超出指定的范围！") { }
      public DelayedIsOverflowException(string message) : base(message) { }
      public DelayedIsOverflowException(string message, Exception inner) : base(message, inner) { }
      protected DelayedIsOverflowException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
