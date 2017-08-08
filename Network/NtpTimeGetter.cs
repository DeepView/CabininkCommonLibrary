using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using Cabinink.Windows.Energy;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Cabinink.Network
{
   /// <summary>
   /// 系统时间结构，将文件时间转换为系统时间格式，另外系统时间是以协调世界时（UTC）为基础的。
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public class SystemTime
   {
      /// <summary>
      /// 年份。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Year;
      /// <summary>
      /// 月份。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Month;
      /// <summary>
      /// 星期，一周的第几天。
      /// </summary>
      [CLSCompliant(false)]
      public ushort DayOfWeek;
      /// <summary>
      /// 指定月份的某一天。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Day;
      /// <summary>
      /// 当天的某一个时。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Hour;
      /// <summary>
      /// 指定时的某一分钟。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Minute;
      /// <summary>
      /// 指定分钟的某一秒。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Second;
      /// <summary>
      /// 指定秒数的某一毫秒。
      /// </summary>
      [CLSCompliant(false)]
      public ushort Miliseconds;
   }
   /// <summary>
   /// NTP时间获取类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public unsafe class NtpTimeGetter
   {
      private string _ntpServer;//指定的NTP服务器。
      private ManualResetEvent _doneEvent;//通知线程等待用的事件。
      private DateTime _dateTime;//存储已经更新的时间。
      private const int TOKEN_QUERY = 8;//API常量，令牌查询。
      private const int TOKEN_ADJUST_PRIVILEGES = 32;//API常量，特权提升。
      private const int ERROR_CALL_NOT_IMPLEMENTED = 0x78;//API常量，说明系统不支持该功能。
      private const int SE_PRIVILEGE_ENABLED = 2;//提升进程的权限。
      private const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";//更改系统时间所需要用到的权限。
      /// <summary>
      /// 设置当前本地时间及日期。
      /// </summary>
      /// <param name="systemTime">一个SYSTEMTIME结构的指针，包含了新的本地日期和时间。</param>
      /// <returns>如果函数调用成功，则返回值为非零值,如果函数失败，返回值是零,为了得到扩展的错误信息，请调用GetLastError函数 。</returns>
      [return: MarshalAs(UnmanagedType.Bool)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
      private static extern bool SetLocalTime(ref SystemTime systemTime);
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
      /// 关闭一个内核对象。
      /// </summary>
      /// <param name="handle">需要被关闭的对象。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      /// <remarks>关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。在CreateThread成功之后会返回一个hThread的handle，且内核对象的计数加1，CloseHandle之后，引用计数减1，当变为0时，系统删除内核对象。若在线程执行完之后，没有调用CloseHandle，在进程执行期间，将会造成内核对象的泄露，相当于句柄泄露，但不同于内存泄露，这势必会对系统的效率带来一定程度上的负面影响。但当进程结束退出后，系统会自动清理这些资源。</remarks>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      private static extern bool CloseHandle(IntPtr handle);
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern void ZeroMemory(void* destination, long length);
      /// <summary>
      /// 返回调用线程最近的错误代码值，错误代码以单线程为基础来维护的，多线程不重写各自的错误代码值。
      /// </summary>
      /// <returns>返回值为调用的线程的错误代码值(unsigned long)，函数通过调 SetLastError 函数来设置此值，每个函数资料的返回值部分都注释了函数设置错误代码的情况。</returns>
      [DllImport("kernel32.dll", EntryPoint = "GetLastError", CharSet = CharSet.Ansi)]
      private static extern long GetLastError();
      /// <summary>
      /// 设置调用线程的最后一个错误代码。
      /// </summary>
      /// <param name="errorCode">线程的最后一个错误代码。</param>
      [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
      private static extern void SetLastError(uint errorCode);
      /// <summary>
      /// 构造函数，创建一个指定NTP服务器和线程等待通知事件的NTP时间获取实例。
      /// </summary>
      /// <param name="ntpServer">指定的NTP服务器。</param>
      /// <param name="doneEvent">用于通知线程等待用的事件。</param>
      public NtpTimeGetter(string ntpServer, ManualResetEvent doneEvent)
      {
         _dateTime = DateTime.Now;
         _ntpServer = ntpServer;
         _doneEvent = doneEvent;
      }
      /// <summary>
      /// 获取或设置当前实例的NTP服务器。
      /// </summary>
      public string NtpServer { get => _ntpServer; set => _ntpServer = value; }
      /// <summary>
      /// 获取或设置当前实例需要更新的时间。
      /// </summary>
      public DateTime Time { get => _dateTime; set => _dateTime = value; }
      /// <summary>
      /// 从当前实例指定的NTP服务器获取时间，但不会应用到本地计算机。
      /// </summary>
      /// <exception cref="NetworkNotAvailableException">当网络不可用时，则会抛出这个异常。</exception>
      public void UpdateTimeFromNetwork()
      {
         try
         {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;
            IPAddress[] addresses = Dns.GetHostEntry(NtpServer).AddressList;
            IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(ipEndPoint);
            socket.ReceiveTimeout = 3000;
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
            const byte serverReplyTime = 40;
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);
            ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
            Time = networkDateTime.ToLocalTime();
         }
         catch
         {
            throw new NetworkNotAvailableException();
         }
      }
      /// <summary>
      /// 多线程多服务器更新时间，但不会将时间应用到本地计算机。
      /// </summary>
      /// <param name="ntpServers">指定多个NTP服务器，用“|”符号分割，比如说："ntp.api.bz|time.windows.com|210.72.145.44|time.nist.gov"</param>
      public void UpdateTimeWithMultithreading(string ntpServers)
      {
         string[] ntpServerArray = ntpServers.Split('|');
         Thread[] ths = new Thread[ntpServerArray.Length];
         NtpTimeGetter[] ntpTime = new NtpTimeGetter[ntpServerArray.Length];
         ManualResetEvent[] mre = new ManualResetEvent[ntpServerArray.Length];
         for (int i = 0; i < ntpServerArray.Length; i++)
         {
            mre[i] = new ManualResetEvent(false);
            NtpTimeGetter ntp = new NtpTimeGetter(ntpServerArray[i], mre[i]);
            ntpTime[i] = ntp;
            ThreadPool.QueueUserWorkItem(ntp.ThreadPoolCallback, i);
         }
         WaitHandle.WaitAny(mre, 15000);
         DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0, 0);
         for (int i = 0; i < ntpServerArray.Length; i++)
         {
            if (ntpTime[i].Time > dt) Time = ntpTime[i].Time;
         }
      }
      /// <summary>
      /// 将Time属性所包含的时间更新本地计算机时间。
      /// </summary>
      /// <returns>如果操作成功则会返回true，否则会返回false。</returns>
      public bool UpdateLocalTime()
      {
         SystemTime systemTime = new SystemTime()
         {
            Year = (ushort)Time.Year,
            Month = (ushort)Time.Month,
            DayOfWeek = (ushort)Time.DayOfWeek,
            Day = (ushort)Time.Day,
            Hour = (ushort)Time.Hour,
            Minute = (ushort)Time.Minute,
            Second = (ushort)Time.Second,
            Miliseconds = (ushort)Time.Millisecond
         };
         EnableSetTimePriviledge();
         return SetLocalTime(ref systemTime);
      }
      /// <summary>
      /// 交换字节顺序。
      /// </summary>
      /// <param name="swapedNum">用于执行该操作的必要参数。</param>
      /// <returns>返回一个交换字节顺序之后的结果。</returns>
      private static uint SwapEndianness(ulong swapedNum)
      {
         return (uint)(((swapedNum & 0x000000ff) << 24) + ((swapedNum & 0x0000ff00) << 8) + ((swapedNum & 0x00ff0000) >> 8) + ((swapedNum & 0xff000000) >> 24));
      }
      /// <summary>
      /// 线程池回调。
      /// </summary>
      /// <param name="threadContext">指定的线程上下文。</param>
      private void ThreadPoolCallback(Object threadContext)
      {
         int threadIndex = (int)threadContext;
         UpdateTimeFromNetwork();
         if (Time.Year >= 2014) _doneEvent.Set();
      }
      /// <summary>
      /// 提升设置系统时间的权限。
      /// </summary>
      /// <returns>如果权限提升成功，则会返回true，否则返回false。</returns>
      private bool EnableSetTimePriviledge()
      {
         IntPtr token = IntPtr.Zero;
         STokenPrivileges privileges = new STokenPrivileges();
         STokenPrivileges priv_t = new STokenPrivileges();
#pragma warning disable CS0219
         bool takenPriviledge = false;
#pragma warning restore CS0219
         bool openToken = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);
         if (!openToken)
         {
            if (GetLastError() == ERROR_CALL_NOT_IMPLEMENTED)
            {
               SetLastError(0x0000);
               if (!(token.ToInt32() != 0 ? true : false)) CloseHandle(token);
               return true;
            }
            if (!(token.ToInt32() != 0 ? true : false)) CloseHandle(token);
            return false;
         }
         ZeroMemory(&privileges, Marshal.SizeOf(privileges));
         if (!(LookupPrivilegeValue(null, SE_SYSTEMTIME_NAME, ref privileges.Privileges.ParticularLuid) != 0 ? true : false))
         {
            if (!(token.ToInt32() != 0 ? true : false)) CloseHandle(token);
            return false;
         }
         privileges.PrivilegeCount = 1;
         privileges.Privileges.Attributes |= SE_PRIVILEGE_ENABLED;
         takenPriviledge = true;
         int zero = 0;
         bool success = AdjustTokenPrivileges(token, 0, ref privileges, 0, ref priv_t, ref zero) != 0 ? true : false;
         if (!(token.ToInt32() != 0 ? true : false)) CloseHandle(token);
         return success;
      }
   }
}
