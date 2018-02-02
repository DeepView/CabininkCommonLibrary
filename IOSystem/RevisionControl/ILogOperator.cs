namespace Cabinink.IOSystem.RevisionControl
{
   /// <summary>
   /// 日志记录操作接口。
   /// </summary>
   public interface ILogOperator
   {
      /// <summary>
      /// 清除所有的本地日志。
      /// </summary>
      void ClearLog();
      /// <summary>
      /// 更新本地日志。
      /// </summary>
      void UpdateLog();
   }
}