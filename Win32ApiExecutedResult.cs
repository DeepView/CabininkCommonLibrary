using System;
using System.Runtime.InteropServices;
namespace Cabinink
{
   /// <summary>
   /// Win32Api执行结果描述类，用于存储非托管代码在执行之后的结果，这个结果一般是一个错误代码。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Win32ApiExecutedResult
   {
      private long _win32ApiErrorCode;//Win32Api错误代码。
      private long _previousWin32ApiErrorCode;//上一个Win32Api错误代码。
      private string _formatedErrorInfoString;//Win32Api错误代码所代表的详细信息。
      private const long WIN32API_ERROR_CODE_SUCCESS = 0x00000000;//API常量，表示Win32Api中的错误代码ERROR_SUCCESS（0x0000）。
      /// <summary>
      /// 构造函数，创建一个错误代码为0的Win32ApiExecutedResult实例。
      /// </summary>
      public Win32ApiExecutedResult()
      {
         _win32ApiErrorCode = WIN32API_ERROR_CODE_SUCCESS;
         _previousWin32ApiErrorCode = Win32ApiHelper.GetLastWin32ApiError();
         _formatedErrorInfoString = Win32ApiHelper.FormatErrorCode(_win32ApiErrorCode);
      }
      /// <summary>
      /// 构造函数，创建一个由用户指定错误代码的Win32ApiExecutedResult实例。
      /// </summary>
      /// <param name="win32ApiErrorCode">指定的错误代码。</param>
      public Win32ApiExecutedResult(long win32ApiErrorCode)
      {
         _win32ApiErrorCode = win32ApiErrorCode;
         _previousWin32ApiErrorCode = Win32ApiHelper.GetLastWin32ApiError();
         _formatedErrorInfoString = Win32ApiHelper.FormatErrorCode(win32ApiErrorCode);
      }
      /// <summary>
      /// 获取或设置当前实例的错误代码。
      /// </summary>
      public long ErrorCode
      {
         get => _win32ApiErrorCode;
         set
         {
            _previousWin32ApiErrorCode = ErrorCode;
            _win32ApiErrorCode = value;
         }
      }
      /// <summary>
      /// 获取当前实例存储的上一个错误代码。
      /// </summary>
      [Obsolete("Will delete.")]
      public long PreviousErrorCode => _previousWin32ApiErrorCode;
      /// <summary>
      /// 获取当前实例所存储的错误代码对应的详细信息。
      /// </summary>
      public string ErrorInformation => Win32ApiHelper.FormatErrorCode(ErrorCode);
      /// <summary>
      /// 获取当前实例所存储的错误代码对应的执行结果，如果这个执行结果为true，则表示非托管代码执行成功且没有任何错误，否则表示非托管代码执行失败。
      /// </summary>
      public bool IsSuccessful => ErrorCode == 0 ? true : false;
      /// <summary>
      /// 获取一个表示Win32Api代码执行成功的Win32ApiExecutedResult实例。
      /// </summary>
      public static Win32ApiExecutedResult SuccessFlag => new Win32ApiExecutedResult(WIN32API_ERROR_CODE_SUCCESS);
      /// <summary>
      /// 手动更新Win32Api错误代码，与此同时，该方法还会手动更新上一个Win32Api错误代码。
      /// </summary>
      public void UpdateErrorCode() => ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
      /// <summary>
      /// 隐式转换操作符重载（To Int64）。
      /// </summary>
      /// <param name="v">隐式转换操作符的源类型。</param>
      public static implicit operator long(Win32ApiExecutedResult v) => v.ErrorCode;
      /// <summary>
      /// 隐式转换操作符重载（To Win32ApiExecutedResult）。
      /// </summary>
      /// <param name="v">隐式转换操作符的源类型。</param>
      public static implicit operator Win32ApiExecutedResult(long v) => new Win32ApiExecutedResult(v);
   }
}
