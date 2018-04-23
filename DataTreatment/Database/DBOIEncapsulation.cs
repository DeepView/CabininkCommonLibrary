using System;
using System.Data;
using System.Runtime.CompilerServices;
namespace Cabinink.DataTreatment.Database
{
   /// <summary>
   /// 数据库操作傻瓜式接口封装基类。
   /// </summary>
   public abstract class DBOIEncapsulation : IDatabasesOperationBase,IDisposable
   {
      [CLSCompliant(false)]
      protected string _dbConnectionString;//数据库连接字符串。
      /// <summary>
      /// 构造函数，创建一个空的连接字符串的实例。
      /// </summary>
      public DBOIEncapsulation() => _dbConnectionString = string.Empty;
      /// <summary>
      /// 构造函数，创建一个指定数据库连接字符串的实例。
      /// </summary>
      /// <param name="dbConnectionString">指定的有效数据库连接字符串。</param>
      public DBOIEncapsulation(string dbConnectionString) => _dbConnectionString = dbConnectionString;
      /// <summary>
      /// 获取当前数据库的连接状态。
      /// </summary>
      public abstract ConnectionState ConnectionStatus { get; }
      /// <summary>
      /// 获取或设置当前数据库的连接字符串。
      /// </summary>
      public abstract string ConnectionString { get; set; }
      /// <summary>
      /// 连接当前实例指定的数据库。
      /// </summary>
      [MethodImpl(MethodImplOptions.Synchronized)]
      public abstract void Connect();
      /// <summary>
      /// 断开当前数据库的连接。
      /// </summary>
      [MethodImpl(MethodImplOptions.Synchronized)]
      public abstract void Disconnect();
      /// <summary>
      /// 执行指定的SQL语句。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <returns>如果该方法未产生任何异常，则会返回数据表里面受影响的数据行。</returns>
      public abstract int ExecuteSql(string sqlSentence);
      /// <summary>
      /// 初始化当前的数据库连接。
      /// </summary>
      [MethodImpl(MethodImplOptions.Synchronized)]
      public abstract bool InitializeConnection();
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public virtual void Dispose() => Disconnect();
   }
}
