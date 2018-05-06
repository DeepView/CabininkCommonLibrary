using System;
using System.Text;
using System.Drawing;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink
{
   /// <summary>
   /// Win32Api基础性帮助类，实现一些有助于访问操作系统API的一些功能。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class Win32ApiHelper
   {
      private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;//API常量，指定了函数需要为请求消息查找系统消息表资源。
      private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;//API常量，在消息定义中的插入序列将会被忽略，这个标示符在获取一个格式化好的消息十分有用，如果这个标示符设置好了，那么Arguments参数将被忽略。
      /// <summary>
      /// FormatMessage格式化消息字符串。
      /// </summary>
      /// <param name="flags">指定格式化处理和如何翻译 lpSource 参数. dwFlags的低字节指定函数如何处理输出缓冲区的换行. 低字节也可以指定格式化后的输出缓冲区的最大宽度.</param>
      /// <param name="messageDefineSource">指定消息定义的位置. 此参数的类型依据 dwFlags 参数的设定.</param>
      /// <param name="messageId">指定消息的消息标志ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING ，则该参数被忽略.</param>
      /// <param name="languageId">指定消息的语言ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING，则该参数被忽略.</param>
      /// <param name="buffer">用来放置格式化消息(以null结束)的缓冲区.如果 dwFlags 参数包括 FORMAT_MESSAGE_ALLOCATE_BUFFER, 本函数将使用LocalAlloc函数定位一个缓冲区，然后将缓冲区指针放到 lpBuffer 指向的地址.</param>
      /// <param name="charSize">如果没有设置 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志, 此参数指定了输出缓冲区可以容纳的TCHARS最大个数. 如果设置了 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志，则此参数指定了输出缓冲区可以容纳的TCHARs 的最小个数. 对于ANSI文本, 容量为bytes的个数; 对于Unicode 文本, 容量为字符的个数.</param>
      /// <param name="arguments">数组指针,用于在格式化消息中插入信息. 格式字符串中的 A %1 指示参数数组中的第一值; a %2 表示第二个值; 以此类推.</param>
      /// <returns>如果执行成功, 返回值为存储在输出缓冲区的TCHARs个数, 包括了null结束符。如果执行失败, 返回值为0。 如果要获取更多的错误信息, 请调用 Marshal.GetLastWin32Error或者调用Win32Api函数GetLastError。</returns>
      [DllImport("kernel32.dll", EntryPoint = "FormatMessage", CharSet = CharSet.Ansi)]
      private extern static int FormatMessage(int flags, IntPtr messageDefineSource, int messageId, int languageId, [Out]StringBuilder buffer, int charSize, int arguments);
      /// <summary>
      /// 返回调用线程最近的错误代码值，错误代码以单线程为基础来维护的，多线程不重写各自的错误代码值。
      /// </summary>
      /// <returns>返回值为调用的线程的错误代码值(unsigned long)，函数通过调 SetLastError 函数来设置此值，每个函数资料的返回值部分都注释了函数设置错误代码的情况。</returns>
      [DllImport("kernel32.dll", EntryPoint = "GetLastError", CharSet = CharSet.Ansi)]
      private static extern long GetLastError();
      /// <summary>
      /// 将指定的可执行模块映射到调用进程的地址空间。
      /// </summary>
      /// <param name="libraryName">可执行模块(dll or exe)的名字：以null结束的字符串指针. 该名称是可执行模块的文件名，与模块本身存储的,用关键字LIBRARY在模块定义文件(.def)中指定的名称无关。</param>
      /// <returns>如果执行成功, 返回模块的句柄。如果执行失败, 返回 NULL. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error.</returns>
      [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi)]
      private extern static IntPtr LoadLibrary(string libraryName);
      /// <summary>
      /// 将装载的dll引用计数器减一，当引用计数器的值为0后，模块将从调用进程的地址空间退出，模块的句柄将不可再用。
      /// </summary>
      /// <param name="libraryModuleHandle">dll模块的句柄. LoadLibrary 或者 GetModuleHandle 函数返回该句柄</param>
      /// <returns>如果执行成功, 返回值为非0<br></br><br>如果失败，返回值为0. 如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error.</br></returns>
      [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi)]
      private extern static int FreeLibrary(IntPtr libraryModuleHandle);
      /// <summary>
      /// 获取外部函数的入口地址。
      /// </summary>
      /// <param name="dllHandle">Dll的句柄，包含了函数或者变量，LoadLibrary 或 GetModuleHandle 函数返回该句柄。</param>
      /// <param name="memeberName">以null结束的字符串指针，包含函数或者变量名，或者函数的顺序值，如果该参数是一个顺序值，它必须是低序字be in the low-order word,高序字(the high-order)必须为0。</param>
      /// <returns>如果执行成功, 返回值为外部函数或变量的地址。如果执行失败，返回值为NULL。如果要获取更多错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
      private extern static IntPtr GetProcAddress(IntPtr dllHandle, string memeberName);
      /// <summary>
      /// 返回包含了指定点的窗口的句柄。忽略屏蔽、隐藏以及透明窗口。
      /// </summary>
      /// <param name="mouseLocation">指定的鼠标坐标。</param>
      /// <returns>鼠标坐标处的窗口句柄，如果没有，返回空句柄。</returns>
      [DllImport("kernel32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Ansi)]
      private extern static IntPtr WindowFromPoint(Point mouseLocation);
      /// <summary>
      /// 获取鼠标所处位置的坐标。
      /// </summary>
      /// <param name="mouseLocation">随同指针在屏幕像素坐标中的位置载入的一个结构。</param>
      /// <returns>该操作将会获取一个Windows API操作状态代码，非零表示成功，零表示失败。</returns>
      [DllImport("kernel32.dll", EntryPoint = "GetCursorPos", CharSet = CharSet.Ansi)]
      private extern static bool GetCursorPos(out Point mouseLocation);
      /// <summary>
      /// 获取目前的应用程序所执行的Windows API的最近一个错误代码。
      /// </summary>
      /// <returns>该操作将会返回一个64位的整型数据，这个数据表示了最近一次Windows应用程序所执行的API的错误代码。</returns>
      [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
      public static long GetLastWin32ApiError() => GetLastError();
      /// <summary>
      /// 将错误号转换为错误消息。
      /// </summary>
      /// <param name="errorCode">需要转换的Win32Api错误代码。</param>
      /// <returns>代表指定错误号的字符串。</returns>
      /// <exception cref="DataTypeConvertFailedException">当强制数据类型转换失败时，则会抛出这个异常。</exception> 
      [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
      public static string FormatErrorCode(long errorCode)
      {
         if (errorCode > long.MaxValue) throw new DataTypeConvertFailedException();
         int flags = FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;
         StringBuilder buffer = new StringBuilder(255);
         FormatMessage(flags, IntPtr.Zero, (int)errorCode, 0, buffer, buffer.Capacity, 0);
         return buffer.ToString();
      }
      /// <summary>
      /// 将指定的可执行模块映射到调用进程的地址空间。
      /// </summary>
      /// <param name="dllFileUrl">可执行模块(dll or exe)的名字：以null结束的字符串指针. 该名称是可执行模块的文件名，与模块本身存储的,用关键字LIBRARY在模块定义文件(.def)中指定的名称无关。</param>
      /// <returns>如果执行成功, 返回模块的句柄。如果执行失败, 返回 NULL. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error。</returns>
      public static IntPtr LoadDynamicLinkLibrary(string dllFileUrl) => LoadLibrary(dllFileUrl);
      /// <summary>
      /// 将装载的dll引用计数器减一，当引用计数器的值为0后，模块将从调用进程的地址空间退出，模块的句柄将不可再用。
      /// </summary>
      /// <param name="dllHandle">dll模块的句柄. LoadLibrary 或者 GetModuleHandle 函数返回该句柄</param>
      /// <returns>如果执行成功, 返回值为非0<br></br><br>如果失败，返回值为0. 如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error.</br></returns>
      public static int ReleaseDynamicLinkLibrary(IntPtr dllHandle) => FreeLibrary(dllHandle);
      /// <summary>
      /// 获取外部函数的入口地址。
      /// </summary>
      /// <param name="dllHandle">Dll的句柄，包含了函数或者变量，LoadLibrary 或 GetModuleHandle 函数返回该句柄。</param>
      /// <param name="memeberName">以null结束的字符串指针，包含函数或者变量名，或者函数的顺序值，如果该参数是一个顺序值，它必须是低序字be in the low-order word,高序字(the high-order)必须为0。</param>
      /// <returns>如果执行成功, 返回值为外部函数或变量的地址。如果执行失败，返回值为NULL。如果要获取更多错误信息，请调用Marshal.GetLastWin32Error。</returns>
      public static IntPtr GetOutsideFuncEntryAddress(IntPtr dllHandle, string memeberName) => GetProcAddress(dllHandle, memeberName);
      /// <summary>
      /// 检测本地系统上是否存在一个指定的方法入口。
      /// </summary>
      /// <param name="libraryFileName">包含指定方法的库文件。</param>
      /// <param name="methodInterfaceName">指定方法的入口。</param>
      /// <returns>如果存在指定方法，返回True，否则返回False。</returns>
      /// <remarks>该方法会启用一个独立的句柄去装载DLL并进行入口函数的判断，操作结束后会释放这个句柄，因此在访问这个函数是需要额外的注意！</remarks>
      public static bool CheckEntryPoint(string libraryFileName, string methodInterfaceName)
      {
         IntPtr libPtr = LoadLibrary(libraryFileName);
         if (!libPtr.Equals(IntPtr.Zero))
         {
            if (!GetProcAddress(libPtr, methodInterfaceName).Equals(IntPtr.Zero))
            {
               FreeLibrary(libPtr);
               return true;
            }
            FreeLibrary(libPtr);
         }
         return false;
      }
      /// <summary>
      /// 获取当前鼠标所在位置所对应窗口的句柄。
      /// </summary>
      /// <param name="mouseLocation">一个任意的Point结构体实例，该实例可以为任意值，但是建议不要赋值为null。</param>
      /// <returns>该操作将会返回当前鼠标所在位置所对应的窗体的Windows句柄。</returns>
      public static IntPtr GetWindowHandle(Point mouseLocation)
      {
         GetCursorPos(out mouseLocation);
         return WindowFromPoint(mouseLocation);
      }
      /// <summary>
      /// 获取当前鼠标所在位置所对应窗口的句柄的十六进制字符串。
      /// </summary>
      /// <param name="mouseLocation">一个任意的Point结构体实例，该实例可以为任意值，但是建议不要赋值为null。</param>
      /// <returns>该操作将会返回当前鼠标所在位置所对应的窗体的Windows句柄的十六进制字符串。</returns>
      public static string GetWindowHandleHexString(Point mouseLocation)
      {
         return GetWindowHandle(mouseLocation).ToString("X");
      }
   }
   /// <summary>
   /// 当强制类型转换失败时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class DataTypeConvertFailedException : Exception
   {
      public DataTypeConvertFailedException() : base("强制类型转换失败！") { }
      public DataTypeConvertFailedException(string message) : base(message) { }
      public DataTypeConvertFailedException(string message, Exception inner) : base(message, inner) { }
      protected DataTypeConvertFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
