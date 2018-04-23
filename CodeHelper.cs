using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cabinink
{
   /// <summary>
   /// 代码辅助工作类，适用于在开发编码时的一些辅助，比如说检测代码是否会抛出异常，参数交换等等。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class CodeHelper
   {
      /// <summary>
      /// 执行参数executedCode包含的代码并检查是否会抛出异常。
      /// </summary>
      /// <param name="executedCode">需要执行的代码。</param>
      /// <param name="throwedException">用于保存已经抛出的异常，但这里只能捕获到最先抛出的异常。</param>
      /// <returns>如果该操作不会抛出任何异常，则将会返回false，但是在抛出任何异常的情况下，该方法都会抛出true。</returns>
      public static bool IsThrowedException(Action executedCode, ref Exception throwedException)
      {
         int returnValue = 0;
         Func<int> executed = new Func<int>(delegate
         {
            executedCode.Invoke();
            return returnValue;
         });
         return IsThrowedException(executed, ref returnValue, ref throwedException);
      }
      /// <summary>
      /// 执行参数executedCode包含存在返回值的代码并检查是否会抛出异常。
      /// </summary>
      /// <typeparam name="T">用于表示参数executedCode所封装委托的返回值类型。</typeparam>
      /// <param name="executedCode">需要执行的代码。</param>
      /// <param name="returnValue">参数executedCode封装的委托的返回值。</param>
      /// <param name="throwedException">用于保存已经抛出的异常，但这里只能捕获到最先抛出的异常。</param>
      /// <returns>如果该操作不会抛出任何异常，则将会返回false，但是在抛出任何异常的情况下，该方法都会抛出true。</returns>
      public static bool IsThrowedException<T>(Func<T> executedCode, ref T returnValue, ref Exception throwedException)
      {
         bool result = false;
         try
         {
            returnValue = executedCode.Invoke();
         }
         catch (Exception exception)
         {
            if (exception != null)
            {
               throwedException = exception.InnerException;
               result = true;
            }
         }
         return result;
      }
      /// <summary>
      /// 执行参数executedUnmanagedCode包含的非托管代码并检查是否写入了新的Win32错误代码。
      /// </summary>
      /// <param name="executedUnmanagedCode">需要执行的非托管代码。</param>
      /// <param name="win32ErrorCode">用于已写入的新的Win32错误代码，如果该方法没有写入新的错误代码，则该参数将会保存操作系统中上一个Win32错误代码。</param>
      /// <returns>如果该操作不会写入新的Win32错误代码，则将会返回false，但是在写入任何Win32错误代码的情况下，该方法都会抛出true。</returns>
      public static bool IsWritedWin32ErrorCode(Action executedUnmanagedCode, ref long win32ErrorCode)
      {
         int returnValue = 0;
         Func<int> executed = new Func<int>(delegate
         {
            executedUnmanagedCode.Invoke();
            return returnValue;
         });
         return IsWritedWin32ErrorCode(executed, ref returnValue, ref win32ErrorCode);
      }
      /// <summary>
      /// 执行参数executedUnmanagedCode包含存在返回值的非托管代码并检查是否写入了新的Win32错误代码。
      /// </summary>
      /// <typeparam name="T">用于表示参数executedUnmanagedCode所封装委托的返回值类型。</typeparam>
      /// <param name="executedUnmanagedCode">需要执行的非托管代码。</param>
      /// <param name="returnValue">参数executedUnmanagedCode封装的委托的返回值。</param>
      /// <param name="win32ErrorCode">用于已写入的新的Win32错误代码，如果该方法没有写入新的错误代码，则该参数将会保存操作系统中上一个Win32错误代码。</param>
      /// <returns>如果该操作不会写入新的Win32错误代码，则将会返回false，但是在写入任何Win32错误代码的情况下，该方法都会抛出true。</returns>
      public static bool IsWritedWin32ErrorCode<T>(Func<T> executedUnmanagedCode, ref T returnValue, ref long win32ErrorCode)
      {
         bool result = false;
         long lastWin32ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
         returnValue = executedUnmanagedCode.Invoke();
         win32ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
         if (lastWin32ErrorCode != win32ErrorCode) result = true;
         else win32ErrorCode = lastWin32ErrorCode;
         return result;
      }
      /// <summary>
      /// 执行参数executedUnmanagedCode包含存在返回值的非托管代码并检查是否写入了新的Win32错误代码，这个方法会返回一个表示执行结果的Win32ApiExecutedResult实例。
      /// </summary>
      /// <typeparam name="T">用于表示参数executedUnmanagedCode所封装委托的返回值类型。</typeparam>
      /// <param name="executedUnmanagedCode">需要执行的非托管代码。</param>
      /// <param name="returnValue">参数executedUnmanagedCode封装的委托的返回值。</param>
      /// <returns>这个操作无论是否写入了新的Win32错误代码，都会返回一个Win32ApiExecutedResult实例。</returns>
      public static Win32ApiExecutedResult IsWritedWin32ErrorCode<T>(Func<T> executedUnmanagedCode, ref T returnValue)
      {
         Win32ApiExecutedResult result = new Win32ApiExecutedResult();
         returnValue = executedUnmanagedCode.Invoke();
         result.UpdateErrorCode();
         return result;
      }
      /// <summary>
      /// 交换两个相同类型参数的值。
      /// </summary>
      /// <typeparam name="T">参数类型，在这里可以被替换为任何受CLR支持的数据类型。</typeparam>
      /// <param name="arg01">第一个参数。</param>
      /// <param name="arg02">第二个参数。</param>
      public static void Swap<T>(ref T arg01, ref T arg02)
      {
         T temp = arg01;
         arg01 = arg02;
         arg02 = temp;
      }
      /// <summary>
      /// 交换两个相同类型参数的值，但是在交换参数值之前，可以选择是否去比较这两个参数的内容是否相同。
      /// </summary>
      /// <typeparam name="T">参数类型，在这里可以被替换为任何受CLR支持的数据类型。</typeparam>
      /// <param name="arg01">第一个参数。</param>
      /// <param name="arg02">第二个参数。</param>
      /// <param name="isAppliedCompareOperation">是否应用参数内容比较的操作。</param>
      /// <remarks>该重载版本相比上一个版本多了一个参数内容比较，如果应用了这个操作，并且当比较结果为true时，那么当前方法将不会进行参数内容交换的操作。</remarks>
      public static void Swap<T>(ref T arg01, ref T arg02, bool isAppliedCompareOperation)
      {
         bool isSwaped = false;
         if (isAppliedCompareOperation) isSwaped = !arg01.Equals(arg02);
         if (isSwaped) Swap(ref arg01, ref arg02);
      }
      /// <summary>
      /// 执行Windows命令行，并获取执行的结果。
      /// </summary>
      /// <param name="commandLineString">需要执行的命令行。</param>
      /// <returns>该操作会返回一个System.String实例，这个实例包含了命令行执行之后的所有文本结果。</returns>
      public static string ExecuteCommandLine(string commandLineString) => ExecuteCommandLine(commandLineString, 0);
      /// <summary>
      /// 指定一个超时时间来执行Windows命令行，并获取执行的结果。
      /// </summary>
      /// <param name="commandLineString">需要执行的命令行。</param>
      /// <param name="timeOut">命令行的执行等待时间，如果为0则表示无限等待，单位：毫秒。</param>
      /// <returns>该操作会返回一个System.String实例，这个实例包含了命令行执行之后的所有文本结果。</returns>
      public static string ExecuteCommandLine(string commandLineString, int timeOut)
      {
         string outputString = string.Empty;
         if (commandLineString != null && !commandLineString.Equals(""))
         {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
               FileName = "cmd.exe",
               Arguments = "/C " + commandLineString,
               UseShellExecute = false,
               RedirectStandardInput = false,
               RedirectStandardOutput = true,
               CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            try
            {
               if (process.Start())
               {
                  if (timeOut == 0) process.WaitForExit();
                  else process.WaitForExit(timeOut);
                  outputString = process.StandardOutput.ReadToEnd();
               }
            }
            catch (Exception throwedException) { if (throwedException != null) throw throwedException.InnerException; }
            finally { if (process != null) process.Close(); }
         }
         return outputString;
      }
   }
}
