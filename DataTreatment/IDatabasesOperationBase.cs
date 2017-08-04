using System.Data;
namespace Cabinink.DataTreatment
{
   /// <summary>
   /// 数据库操作的基础共性成员接口
   /// </summary>
   public interface IDatabasesOperationBase
   {
      /// <summary>
      /// 获取当前数据库的连接状态。
      /// </summary>
      ConnectionState ConnectionStatus { get; }
      /// <summary>
      /// 获取或设置当前数据库的连接字符串。
      /// </summary>
      string ConnectionString { get; set; }
      /// <summary>
      /// 初始化当前的数据库连接。
      /// </summary>
      /// <returns>如果初始化成功则会返回true，若被该方法捕获了一些无法处理的异常则会视为初始化失败，并返回false。</returns>
      bool InitializeConnection();
      /// <summary>
      /// 连接当前实例指定的数据库。
      /// </summary>
      void Connect();
      /// <summary>
      /// 断开当前数据库的连接。
      /// </summary>
      void Disconnect();
      /// <summary>
      /// 执行指定的SQL语句。
      /// </summary>
      /// <param name="sqlSentence">需要被执行的SQL语句。</param>
      /// <returns>如果该方法未产生任何异常，则会返回数据表里面受影响的数据行。</returns>
      int ExecuteSql(string sqlSentence);
   }
}
