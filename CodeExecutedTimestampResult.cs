using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cabinink
{
   /// <summary>
   /// 用于描述代码执行所需时间的时间戳返回结果的类，基于IDisposable接口实现。
   /// </summary>
   /// <example>
   /// 该类的建议使用方法如下代码所示：
   /// <code>
   /// CodeExecutedTimestampResult result = new CodeExecutedTimestampResult("NeedExecutedCodes");
   /// //This is your code...
   /// result.StopMeasure();
   /// Console.WriteLine("ExecutedTime = {0}", result.ExecuteTime);
   /// Console.WriteLine("Timestamp = {0}", result.ExecutedTimestamp);
   /// </code>
   /// </example>
   [Serializable]
   [ComVisible(true)]
   public class CodeExecutedTimestampResult : IDisposable
   {
      private string _codeName;//所执行的代码的描述文本或者注释文本。
      private DateTime _executeTime;//记录开始执行这些代码时的日期和时间。
      private Stopwatch _stopwatch;//用于准确测量代码执行时长的实例。
      private long _elapsedMilliseconds;//用于记录代码执行时长的长整形数据，单位为毫秒。
      /// <summary>
      /// 构造函数，创建一个默认的CodeExecutedTimestampResult实例。
      /// </summary>
      public CodeExecutedTimestampResult()
      {
         _stopwatch = new Stopwatch();
         _executeTime = DateTime.Now;
         _stopwatch.Start();
         _codeName = "CODE_EXECUTED_TIME_" + _executeTime.Ticks;
         _elapsedMilliseconds = 0;
      }
      /// <summary>
      /// 构造函数，创建一个指定所执行代码描述文本或者注释文本的CodeExecutedTimestampResult实例。
      /// </summary>
      /// <param name="codeName">所执行的代码的描述文本或者注释文本。</param>
      public CodeExecutedTimestampResult(string codeName)
      {
         _stopwatch = new Stopwatch();
         _executeTime = DateTime.Now;
         _stopwatch.Start();
         _codeName = codeName;
         _elapsedMilliseconds = 0;
      }
      /// <summary>
      /// 获取当前实例在执行代码时所花费的时长。
      /// </summary>
      public long ExecutedTimestamp => _elapsedMilliseconds;
      /// <summary>
      /// 获取当前实例在代码开始执行的时候的本地时间。
      /// </summary>
      public DateTime ExecuteTime => _executeTime;
      /// <summary>
      /// 结束测量某一段代码的执行时间间隔或者运行时长。
      /// </summary>
      public void StopMeasure()
      {
         _stopwatch.Stop();
         _elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
      }
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose()
      {
         StopMeasure();
         Console.WriteLine("{0}'s running time is {1} milliseconds.", _codeName, ExecutedTimestamp);
      }
   }
}
