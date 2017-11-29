namespace Cabinink.Windows.Applications
{
   /// <summary>
   /// 应用程序级别的基础IO接口。
   /// </summary>
   public interface IApplicationBasicIO
   {
      /// <summary>
      /// 读取操作。
      /// </summary>
      /// <param name="exceptionInformation">如果操作结束之后存在异常，则该字符串将包含一个异常信息。</param>
      /// <returns>如果没有任何异常抛出，以及操作成功，则将会返回true，否则返回false。</returns>
      bool Read(ref string exceptionInformation);
      /// <summary>
      /// 存储操作。
      /// </summary>
      /// <param name="exceptionInformation">如果操作结束之后存在异常，则该字符串将包含一个异常信息。</param>
      /// <returns>如果没有任何异常抛出，以及操作成功，则将会返回true，否则返回false。</returns>
      bool Write(ref string exceptionInformation);
   }
}