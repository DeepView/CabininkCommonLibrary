namespace Cabinink.IOSystem.RevisionControl
{
   /// <summary>
   /// 版本控制活动更改执行接口。
   /// </summary>
   public interface IExecuter
   {
      /// <summary>
      /// 执行版本控制器所对应的活动更改。
      /// </summary>
      /// <param name="exceptionInformation">如果在这个操作出现异常的状态下，则这个参数将会记录所遇到的异常的异常信息。</param>
      /// <param name="stackTrackRecord">如果在这个操作出现异常的状态下，则这个参数将会记录所遇到的异常的内存堆栈跟踪信息。</param>
      /// <returns>如果这个操作没有发生任何异常或错误，则这个操作视作成功，返回true，否则将会返回false。</returns>
      bool Execute(ref string exceptionInformation,ref string stackTrackRecord);
   }
}
