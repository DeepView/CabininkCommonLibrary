using System.Data;
namespace Cabinink.DataTreatment.Database.Extend
{
   /// <summary>
   /// 数据库扩展类的数据库接口封装的开关控制接口。
   /// </summary>
   public interface IDbOperatorSwitch
   {
      /// <summary>
      /// 启用数据库操作实例，如果不执行这个操作来进行触发器的一些核心操作，将会出现一些异常。
      /// </summary>
      /// <returns>这个操作会返回当前被操作数据库的连接状态。</returns>
      ConnectionState DisableOperator();
      /// <summary>
      /// 禁用数据库操作实例。
      /// </summary>
      /// <returns>这个操作会返回当前被操作数据库的连接状态。</returns>
      ConnectionState EnableOperator();
   }
}