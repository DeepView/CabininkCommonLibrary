using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Cabinink.Network
{
   /// <summary>
   /// 系统时间结构，将文件时间转换为系统时间格式，另外系统时间是以协调世界时（UTC）为基础的。
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public struct SLocalSystemTime
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
      /// <summary>
      /// 设置当前本地时间及日期。
      /// </summary>
      /// <param name="systemTime">一个SYSTEMTIME结构的指针，包含了新的本地日期和时间。</param>
      /// <returns>如果函数调用成功，则返回值为非零值,如果函数失败，返回值是零,为了得到扩展的错误信息，请调用GetLastError函数 。</returns>
      [return: MarshalAs(UnmanagedType.Bool)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
      private static extern bool SetLocalTime([In] ref SLocalSystemTime systemTime);
      /// <summary>
      /// 构造函数，创建一个指定NTP服务器的NTP时间获取实例。
      /// </summary>
      /// <param name="ntpServer">指定的NTP服务器。</param>
      public NtpTimeGetter(string ntpServer)
      {
         _dateTime = DateTime.Now;
         _ntpServer = ntpServer;
         _doneEvent = new ManualResetEvent(false);
      }
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
         catch (Exception exception)
         {
            if (exception != null) throw exception.InnerException;
            else throw new NetworkNotAvailableException();
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
      /// 将Time属性所包含的时间更新本地计算机时间，不过这个方法可能需要以管理员身份运行或者提升特权。
      /// </summary>
      /// <param name="win32ErrorCode">需要传递并且用于开发者参考的错误代码。</param>
      /// <param name="win32ErrorInformation">需要传递并且用于开发者参考的错误消息。</param>
      /// <returns>如果操作成功则会返回true，否则会返回false。</returns>
      public bool UpdateLocalTime(ref long win32ErrorCode, ref string win32ErrorInformation)
      {
         bool result = true;
         win32ErrorCode = 0x0000;
         win32ErrorInformation = Win32ApiHelper.FormatErrorCode(0x0000);
         SLocalSystemTime systemTime = new SLocalSystemTime()
         {
            Year = Convert.ToUInt16(Time.Year),
            Month = Convert.ToUInt16(Time.Month),
            DayOfWeek = Convert.ToUInt16(Time.DayOfWeek),
            Day = Convert.ToUInt16(Time.Day),
            Hour = Convert.ToUInt16(Time.Hour),
            Minute = Convert.ToUInt16(Time.Minute),
            Second = Convert.ToUInt16(Time.Second),
            Miliseconds = Convert.ToUInt16(Time.Millisecond)
         };
         result = SetLocalTime(ref systemTime);
         if (!result)
         {
            win32ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
            win32ErrorInformation = Win32ApiHelper.FormatErrorCode(win32ErrorCode);
         }
         return result;
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
   }
}
