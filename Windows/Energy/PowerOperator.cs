using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Energy
{
   /// <summary>
   /// TokenPrivileges 结构包含了一个访问令牌的一组权限信息：即该访问令牌具备的权限。
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct STokenPrivileges
   {
      /// <summary>
      /// 指定了权限数组的容量。
      /// </summary>
      public int PrivilegeCount;
      /// <summary>
      /// 指定一组的LuidAttributes 结构，每个结构包含了LUID和权限的属性。
      /// </summary>
      public SLuidAttributes Privileges;
   }
   /// <summary>
   /// LuidAttributes 结构呈现了本地唯一标志和它的属性。
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct SLuidAttributes
   {
      /// <summary>
      /// 特定的LUID。
      /// </summary>
      public SLocallyUniqueIdentifier ParticularLuid;
      /// <summary>
      /// 指定了LUID的属性，其值可以是一个32位大小的bit 标志，具体含义根据LUID的定义和使用来看。
      /// </summary>
      public int Attributes;
   }
   /// <summary>
   /// 本地唯一标志是一个64位的数值，它被保证在产生它的系统上唯一！LUID的在机器被重启前都是唯一的。
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct SLocallyUniqueIdentifier
   {
      /// <summary>
      /// 本地唯一标志的低32位。
      /// </summary>
      public int LowPart;
      /// <summary>
      /// 本地唯一标志的高32位。
      /// </summary>
      public int HighPart;
   }
   /// <summary>
   /// Windows电源相关操作类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class PowerOperator
   {
      private const int EWX_FORCE = 4;//API常量，强迫终止没有响应的进程。
      private const int EWX_LOGOFF = 0;//API常量，终止进程，然后注销。
      private const int EWX_REBOOT = 2;//API常量，重新引导系统。
      private const int EWX_SHUTDOWN = 1;//API常量，关闭系统。
      private const int TOKEN_QUERY = 8;//API常量，令牌查询。
      private const int TOKEN_ADJUST_PRIVILEGES = 32;//API常量，特权提升。
      private const int SE_PRIVILEGE_ENABLED = 2;//API常量，启用新特权。
      private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;//API常量，指定了函数需要为请求消息查找系统消息表资源。
      private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";//API常量，执行ExitWindowsEx需要用到的权限字符串。
      /// <summary>
      /// 退出、重启或注销系统。
      /// </summary>
      /// <param name="flags">指定关闭的类型。</param>
      /// <param name="reserved">系统保留，这参数被忽略。</param>
      /// <returns>如果函数成功，返回值为非零。如果函数失败，返回值是零。</returns>
      [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
      private static extern int ExitWindowsEx(int flags, int reserved);
      /// <summary>
      /// 打开与进程相关联的访问令牌。
      /// </summary>
      /// <param name="processHandle">要修改访问权限的进程句柄。</param>
      /// <param name="opeartionType">指定要进行的操作类型。</param>
      /// <param name="token">返回的访问令牌指针。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
      private static extern bool OpenProcessToken(IntPtr processHandle, int opeartionType, ref IntPtr token);
      /// <summary>
      /// 获取当前应用程序实例的进程。
      /// </summary>
      /// <returns>操作成功后将返回这个进程的句柄。</returns>
      [DllImport("kernel32.dll", ExactSpelling = true)]
      private static extern IntPtr GetCurrentProcess();
      /// <summary>
      /// 获取本地唯一标志LUID。
      /// </summary>
      /// <param name="systemName">以null结束的字符串指针，标志了在其上查找权限名的系统名称。如果设置为null, 函数将试图查找指定系统的权限名。</param>
      /// <param name="jurisdictionName">以null结束的字符串指针，指定了在Winnt.h头文件中定义的权限名。例如, 该参数可以是一个常量 SE_SECURITY_NAME, 或者对应的字符串 "SeSecurityPrivilege"。</param>
      /// <param name="luidPointer">接收本地唯一标志LUID的变量指针，通过它可以知道由lpSystemName 参数指定的系统上的权限。</param>
      /// <returns>如果执行成功，返回非0，如果执行失败，返回0，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", CharSet = CharSet.Ansi)]
      private static extern int LookupPrivilegeValue(string systemName, string jurisdictionName, ref SLocallyUniqueIdentifier luidPointer);
      /// <summary>
      /// 启用或禁用指定访问令牌的权限。
      /// </summary>
      /// <param name="token">需要改变权限的访问令牌句柄。 句柄必须含有对令牌的 TOKEN_ADJUST_PRIVILEGES 访问权限。 如果 PreviousState 参数非null, 句柄还需要有 TOKEN_QUERY 访问权限。</param>
      /// <param name="isUnenableJuri">执行函数是否禁用访问令牌的所有权限。 如果参数值为 TRUE, 函数将禁用所有权限并忽略 NewState 参数。 如果其值为 FALSE, 函数将根据NewState参数指向的信息改变权限。</param>
      /// <param name="tokenPrivileges">一个 TokenPrivileges 结构的指针，指定了一组权限以及它们的属性。 如果 DisableAllPrivileges 参数为 FALSE, AdjustTokenPrivileges 函数将启用或禁用访问令牌的这些权限。 如果你为一个权限设置了 SE_PRIVILEGE_ENABLED 属性, 本函数将启用该权限; 否则, 它将禁用该权限。 如果 DisableAllPrivileges 参数为 TRUE, 本函数忽略此参数。</param>
      /// <param name="bufferLength">为PreviousState参数指向的缓冲区用字节设置大小。 如果PreviousState 参数为 NULL，此参数可以为0</param>
      /// <param name="bufferPointer">一个缓冲区指针，被函数用来填充 TOKENT_PRIVILEGES结构，它包含了被函数改变的所有权限的先前状态。 此参数可以为 NULL。</param>
      /// <param name="bufferSize">一个变量指针，指示了由PreviousState参数指向的缓冲区的大小。如果 PreviousState 参数为 NULL，此参数可以为NULL 。</param>
      /// <returns>如果执行成功，返回非0。 如果要检测函数是否调整了指定的权限, 请调用 Marshal.GetLastWin32Error。</returns>
      [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", CharSet = CharSet.Ansi)]
      private static extern int AdjustTokenPrivileges(IntPtr token, int isUnenableJuri, ref STokenPrivileges tokenPrivileges, int bufferLength, ref STokenPrivileges bufferPointer, ref int bufferSize);
      /// <summary>
      /// FormatMessage格式化消息字符串。
      /// </summary>
      /// <param name="flags">指定格式化处理和如何翻译 lpSource 参数. dwFlags的低字节指定函数如何处理输出缓冲区的换行. 低字节也可以指定格式化后的输出缓冲区的最大宽度.</param>
      /// <param name="source">指定消息定义的位置. 此参数的类型依据 dwFlags 参数的设定.</param>
      /// <param name="messageId">指定消息的消息标志ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING ，则该参数被忽略.</param>
      /// <param name="languageId">指定消息的语言ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING，则该参数被忽略.</param>
      /// <param name="buffer">用来放置格式化消息(以null结束)的缓冲区.如果 dwFlags 参数包括 FORMAT_MESSAGE_ALLOCATE_BUFFER, 本函数将使用LocalAlloc函数定位一个缓冲区，然后将缓冲区指针放到 lpBuffer 指向的地址.</param>
      /// <param name="size">如果没有设置 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志, 此参数指定了输出缓冲区可以容纳的TCHARs最大个数. 如果设置了 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志，则此参数指定了输出缓冲区可以容纳的TCHARs 的最小个数. 对于ANSI文本, 容量为bytes的个数; 对于Unicode 文本, 容量为字符的个数.</param>
      /// <param name="arguments">数组指针,用于在格式化消息中插入信息. 格式字符串中的 A %1 指示参数数组中的第一值; a %2 表示第二个值; 以此类推.</param>
      /// <returns>如果执行成功, 返回值为存储在输出缓冲区的TCHARs个数, 包括了null结束符。如果执行失败, 返回值为0。 如果要获取更多的错误信息, 请调用 Marshal.GetLastWin32Error或者调用Win32Api函数GetLastError。</returns>
      [DllImport("user32.dll", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi)]
      private extern static int FormatMessage(int flags, IntPtr source, int messageId, int languageId, StringBuilder buffer, int size, int arguments);
      /// <summary>
      /// 将指定的可执行模块映射到调用进程的地址空间。
      /// </summary>
      /// <param name="libraryName">可执行模块(dll or exe)的名字：以null结束的字符串指针. 该名称是可执行模块的文件名，与模块本身存储的,用关键字LIBRARY在模块定义文件(.def)中指定的名称无关,</param>
      /// <returns>如果执行成功, 返回模块的句柄<br></br><br>如果执行失败, 返回 NULL. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error.</br></returns>
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
      /// <param name="dllHandle">Dll的句柄，包含了函数或者变量，LoadLibrary 或 GetModuleHandle 函数返回该句柄 </param>
      /// <param name="memeberName">以null结束的字符串指针，包含函数或者变量名，或者函数的顺序值，如果该参数是一个顺序值，它必须是低序字be in the low-order word,高序字(the high-order)必须为0</param>
      /// <returns>如果执行成功, 返回值为外部函数或变量的地址。如果执行失败，返回值为NULL。如果要获取更多错误信息，请调用Marshal.GetLastWin32Error.</returns>
      [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
      private extern static IntPtr GetProcAddress(IntPtr dllHandle, string memeberName);
      /// <summary>
      /// 返回调用线程最近的错误代码值，错误代码以单线程为基础来维护的，多线程不重写各自的错误代码值。
      /// </summary>
      /// <returns>返回值为调用的线程的错误代码值(unsigned long)，函数通过调 SetLastError 函数来设置此值，每个函数资料的返回值部分都注释了函数设置错误代码的情况。</returns>
      [DllImport("kernel32.dll", EntryPoint = "GetLastError", CharSet = CharSet.Ansi)]
      private static extern long GetLastError();
      /// <summary>
      /// 注销当前用户，但是不退出Windows。
      /// </summary>
      /// <returns>如果操作成功，则返回ERROR_SUCCESS（API定义：#define ERROR_SUCCESS 0L），否则将会显示其他的错误代码。</returns>
      public static long LogOff()
      {
         GetSystemAuthority(SE_SHUTDOWN_NAME);
         ExitWindowsEx(EWX_LOGOFF, 0);
         return GetLastError();
      }
      /// <summary>
      /// 重新启动Windows。
      /// </summary>
      /// <returns>如果操作成功，则返回ERROR_SUCCESS（API定义：#define ERROR_SUCCESS 0L），否则将会显示其他的错误代码。</returns>
      public static long ResetBoot()
      {
         GetSystemAuthority(SE_SHUTDOWN_NAME);
         ExitWindowsEx(EWX_REBOOT, 0);
         return GetLastError();
      }
      /// <summary>
      /// 强制中断当前用户的所有进程，即强制注销。
      /// </summary>
      /// <returns>如果操作成功，则返回ERROR_SUCCESS（API定义：#define ERROR_SUCCESS 0L），否则将会显示其他的错误代码。</returns>
      public static long InterruptAllUserProcess()
      {
         GetSystemAuthority(SE_SHUTDOWN_NAME);
         ExitWindowsEx(EWX_FORCE, 0);
         return GetLastError();
      }
      /// <summary>
      /// 关闭Windows。
      /// </summary>
      /// <returns>如果操作成功，则返回ERROR_SUCCESS（WIN32_API定义：#define ERROR_SUCCESS 0L），否则将会显示其他的错误代码。</returns>
      public static long Shutdown()
      {
         GetSystemAuthority(SE_SHUTDOWN_NAME);
         ExitWindowsEx(EWX_SHUTDOWN, 0);
         return GetLastError();
      }
      /// <summary>
      /// 使Windows强制进入休眠状态。
      /// </summary>
      public static void Hibernate()
      {
         Application.SetSuspendState(PowerState.Hibernate, false, true);
      }
      /// <summary>
      /// 使Windows进入休眠状态，并告知其他进程使其进入休眠状态。
      /// </summary>
      /// <param name="force">决定是否强制休眠。</param>
      public static void Hibernate(bool force)
      {
         Application.SetSuspendState(PowerState.Hibernate, force, true);
      }
      /// <summary>
      /// 强制挂起Windows，即睡眠模式。
      /// </summary>
      public static void Suspend()
      {
         Application.SetSuspendState(PowerState.Suspend, false, false);
      }
      /// <summary>
      /// 挂起Windows，并通知其他进程是否决定该操作。
      /// </summary>
      /// <param name="force">决定是否强制挂起。</param>
      public static void Suspend(bool force)
      {
         Application.SetSuspendState(PowerState.Suspend, force, false);
      }
      /// <summary>
      /// 获取更高级别的操作系统权限。
      /// </summary>
      /// <param name="privilege">需要获得的权限。</param>
      /// <exception cref="Win32ApiErrorInformationException">当出现未知的或者无法处理的Win32Api错误代码时，则需要抛出这个异常。</exception>
      public static void GetSystemAuthority(string privilege)
      {
         if (!Win32ApiHelper.CheckEntryPoint("advapi32.dll", "AdjustTokenPrivileges")) return;
         IntPtr tokenHandle = IntPtr.Zero;
         SLocallyUniqueIdentifier luid = new SLocallyUniqueIdentifier();
         STokenPrivileges newPrivileges = new STokenPrivileges();
         STokenPrivileges tokenPrivileges;
         if (OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle) == false)
         {
            throw new Win32ApiErrorInformationException();
         }
         if (LookupPrivilegeValue(null, privilege, ref luid) == 0)
         {
            throw new Win32ApiErrorInformationException();
         }
         tokenPrivileges.PrivilegeCount = 1;
         tokenPrivileges.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
         tokenPrivileges.Privileges.ParticularLuid = luid;
         int size = 4;
         if (AdjustTokenPrivileges(tokenHandle, 0, ref tokenPrivileges, 4 + (12 * tokenPrivileges.PrivilegeCount), ref newPrivileges, ref size) == 0)
         {
            throw new Win32ApiErrorInformationException();
         }
      }
   }
   /// <summary>
   /// Win32Api错误信息异常。
   /// </summary>
   [Serializable]
   public class Win32ApiErrorInformationException : Exception
   {
      public Win32ApiErrorInformationException() : base(Win32ApiHelper.FormatErrorCode(Marshal.GetLastWin32Error())) { }
      public Win32ApiErrorInformationException(string message) : base(message) { }
      public Win32ApiErrorInformationException(string message, Exception inner) : base(message, inner) { }
      protected Win32ApiErrorInformationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
