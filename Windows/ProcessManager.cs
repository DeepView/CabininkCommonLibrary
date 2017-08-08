using System;
using System.IO;
using System.Security;
using Cabinink.IOSystem;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
using Cabinink.Windows.Energy;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Cabinink.Windows
{
   /// <summary>
   /// 用来存放快照进程信息的一个结构体，存放进程信息和调用成员输出进程信息。
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public struct SProcessEntry32
   {
      /// <summary>
      /// 这个结构的长度，以字节为单位，初始化一个实例以后调用Process32First函数，设置成员的大小。
      /// </summary>
      [CLSCompliant(false)]
      public uint Size;
      /// <summary>
      /// 此进程的引用计数，这个成员已经不再被使用，总是设置为零。
      /// </summary>
      [CLSCompliant(false)]
      public uint Usage;
      /// <summary>
      /// 进程标识符，即PID。
      /// </summary>
      [CLSCompliant(false)]
      public uint ProcessId;
      /// <summary>
      /// 进程默认堆ID，这个成员已经不再被使用，总是设置为零。
      /// </summary>
      public IntPtr DefaultHeapId;
      /// <summary>
      /// 进程模块ID，这个成员已经不再被使用，总是设置为零。
      /// </summary>
      [CLSCompliant(false)]
      public uint ModuleId;
      /// <summary>
      /// 此进程开启的线程计数。
      /// </summary>
      [CLSCompliant(false)]
      public uint Threads;
      /// <summary>
      /// 父进程的进程标识符。
      /// </summary>
      [CLSCompliant(false)]
      public uint ParentProcessId;
      /// <summary>
      /// 线程优先权，当前进程创建的任何一个线程的基础优先级，即在当前进程内创建线程的话，其基本优先级的值。
      /// </summary>
      public int PriorityClassBase;
      /// <summary>
      /// 进程标记，这个成员已经不再被使用，总是设置为零。
      /// </summary>
      [CLSCompliant(false)]
      public uint Flags;
      /// <summary>
      /// 进程的可执行文件名称。要获得可执行文件的完整路径，应调用Module32First函数，再检查其返回的MODULEENTRY32结构的szExePath成员。但是，如果被调用进程是一个64位程序，您必须调用QueryFullProcessImageName函数去获取64位进程的可执行文件完整路径名。
      /// </summary>
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string ExecuteFileName;
   }
   /// <summary>
   /// 进程管理类，这个类包含了一系列的进程操作的静态方法。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class ProcessManager
   {
      private static Hashtable _processHwnd = null;//进程句柄哈希表。
      private const int ERROR_SUCCESS = 0;//当成功时需要返回的错误码。
      /// <summary>
      /// 窗口枚举委托。
      /// </summary>
      /// <param name="handle">窗口的句柄。</param>
      /// <param name="lParam">需要传递的特定参数。</param>
      /// <returns>当操作成功是返回true，否则返回false。</returns>
      [CLSCompliant(false)]
      public delegate bool WindowEnumProc(IntPtr handle, uint lParam);
      /// <summary>
      /// 获取调用线程最近的错误代码值。
      /// </summary>
      /// <returns>Int32</returns>
      /// <remarks>该函数返回调用线程最近的错误代码值，错误代码以单线程为基础来维护的，多线程不重写各自的错误代码值。</remarks>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern Int32 GetLastError();
      /// <summary>
      /// 获取当前进程的一个伪句柄。
      /// </summary>
      /// <returns>获取当前进程的一个伪句柄，只要当前进程需要一个进程句柄，就可以使用这个伪句柄。该句柄可以复制，但不可继承。</returns>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern IntPtr GetCurrentProcess();
      /// <summary>
      /// 关闭一个内核对象。
      /// </summary>
      /// <param name="handle">需要被关闭的对象。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      /// <remarks>关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。在CreateThread成功之后会返回一个hThread的handle，且内核对象的计数加1，CloseHandle之后，引用计数减1，当变为0时，系统删除内核对象。若在线程执行完之后，没有调用CloseHandle，在进程执行期间，将会造成内核对象的泄露，相当于句柄泄露，但不同于内存泄露，这势必会对系统的效率带来一定程度上的负面影响。但当进程结束退出后，系统会自动清理这些资源。</remarks>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      private static extern bool CloseHandle(IntPtr handle);
      /// <summary>
      /// 查看系统权限的特权值。
      /// </summary>
      /// <param name="systemName">需要查看的系统，本地系统直接用NULL（Nothing）。</param>
      /// <param name="jurisdictionName">指向一个以零结尾的字符串，指定特权的名称。</param>
      /// <param name="luidPointer">接收所返回的制定特权名称的信息。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      /// <remarks>查看指定系统权限的特权值，如果操作成功则返回true，否则返回false，与此同时，还会将接收的信息反馈到LocallyUniqueIdentifier结构类里面。</remarks>
      [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool LookupPrivilegeValue(string systemName, string jurisdictionName, [Out()] SLocallyUniqueIdentifier luidPointer);
      /// <summary>
      /// 打开与进程相关联的访问令牌。
      /// </summary>
      /// <param name="handle">要修改访问权限的进程句柄。</param>
      /// <param name="operationType">指定你要进行的操作类型。</param>
      /// <param name="tokenPointer">返回的访问令牌指针。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      /// <remarks>打开与进程相关联的访问令牌，当修改权限的时候需要用到这个句柄，操作成功返回true，否则返回false。</remarks>
      [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
      private static extern bool OpenProcessToken([In()]IntPtr handle, [In()]Int32 operationType, [Out()] IntPtr tokenPointer);
      /// <summary>
      /// 启用或禁止指定访问令牌的特权。
      /// </summary>
      /// <param name="tokenPointer">包含特权的句柄。</param>
      /// <param name="disable">禁用所有权限标志。</param>
      /// <param name="newStateInfo">新特权信息。</param>
      /// <param name="bufferSize">缓冲数据大小,以字节为单位的PreviousState的缓存区。</param>
      /// <param name="privileges">接收被改变特权当前状态的Buffer。</param>
      /// <param name="retunValueSize">接收PreviousState缓存区要求的大小。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      /// <remarks>启用或禁用特权一个有TOKEN_ADJUST_PRIVILEGES访问的访问令牌，成功返回true，否则返回false。</remarks>
      [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
      private static extern bool AdjustTokenPrivileges(IntPtr tokenPointer, bool disable, STokenPrivileges newStateInfo, Int32 bufferSize, STokenPrivileges privileges, Int32 retunValueSize);
      /// <summary>
      /// 设定线程的优先级。
      /// </summary>
      /// <param name="thread">需要被调整优先级的线程的句柄。</param>
      /// <param name="priority">返回带有THREAD_PRIORITY_???前缀的某个函数，它定义了线程的优级。</param>
      /// <returns>如果执行成功，返回0，如果执行失败，返回非0，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      /// <remarks>设定线程的优先级，注意，在Windows里面，线程的优先级同进程的优先级类组合在一起就决定了线程的实际优先级。</remarks>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern long SetThreadPriority(long thread, long priority);
      /// <summary>
      /// 通过获取进程信息为指定的进程、进程使用的堆[HEAP]、模块[MODULE]、线程建立一个快照。
      /// </summary>
      /// <param name="flags">用来指定“快照”中需要返回的对象，可以是TH32CS_SNAPPROCESS等。</param>
      /// <param name="processId">一个进程ID号，用来指定要获取哪一个进程的快照，当获取系统进程列表或获取 当前进程快照时可以设为0。</param>
      /// <returns>调用成功，返回快照的句柄，调用失败，返回INVALID_HANDLE_VALUE，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("kernel32.dll")]
      private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processId);
      /// <summary>
      /// 进程获取函数，当我们利用函数CreateToolhelp32Snapshot()获得当前运行进程的快照后，我们可以利用process32First函数来获得第一个进程的句柄。
      /// </summary>
      /// <param name="handle">一个快照句柄，存放以前调用的CreateToolhelp32Snapshot函数的返回值。</param>
      /// <param name="pe">一个指向ProcessEntry32结构。它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("kernel32.dll")]
      private static extern int Process32First(IntPtr handle, ref SProcessEntry32 pe);
      /// <summary>
      /// 一个进程获取函数，当我们利用函数CreateToolhelp32Snapshot()获得当前运行进程的快照后,我们可以利用Process32Next函数来获得下一个进程的句柄。
      /// </summary>
      /// <param name="handle">一个快照句柄，存放以前调用的CreateToolhelp32Snapshot函数的返回值。</param>
      /// <param name="pe">一个指向ProcessEntry32结构。它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("kernel32.dll")]
      private static extern int Process32Next(IntPtr handle, ref SProcessEntry32 pe);
      /// <summary>
      /// 找出某个窗口的创建者（线程或进程），返回创建者的标志符。
      /// </summary>
      /// <param name="handle">被查找窗口的句柄。</param>
      /// <param name="id">进程号的存放地址（变量地址）。</param>
      /// <returns>返回线程号，注意，id 是存放进程号的变量。返回值是线程号，id是进程号存放处。</returns>
      [DllImport("kernel32.dll")]
      private static extern int GetWindowThreadProcessId(IntPtr handle, out int id);
      /// <summary>
      /// 该函数将指定的消息发送到一个或多个窗口。
      /// </summary>
      /// <param name="windowHandle">其窗口程序将接收消息的窗口的句柄。如果此参数为HWND_BROADCAST，则消息将被发送到系统中所有顶层窗口，包括无效或不可见的非自身拥有的窗口、被覆盖的窗口和弹出式窗口，但消息不被发送到子窗口。</param>
      /// <param name="message">指定被发送的消息。</param>
      /// <param name="wParam">指定附加的消息特定信息。</param>
      /// <param name="lParam">指定附加的消息特定信息。</param>
      /// <returns>返回值指定消息处理的结果，依赖于所发送的消息。</returns>
      /// <remarks>此函数为指定的窗口调用窗口程序，直到窗口程序处理完消息再返回。而和函数PostMessage不同，PostMessage是将一个消息寄送到一个线程的消息队列后就立即返回。另外，需要用HWND_BROADCAST通信的应用程序应当使用函数RegisterWindowMessage来为应用程序间的通信取得一个唯一的消息。</remarks>
      [DllImport("user32.dll", EntryPoint = "SendMessage")]
      private static extern int SendMessage(int windowHandle, int message, int wParam, string lParam);
      /// <summary>
      /// 该函数枚举所有屏幕上的顶层窗口，并将窗口句柄传送给应用程序定义的回调函数。
      /// </summary>
      /// <param name="enumFunc">指向一个应用程序定义的回调函数指针。</param>
      /// <param name="lParam">指定一个传递给回调函数的应用程序定义值。</param>
      /// <returns>如果函数成功，返回值为非零，如果函数失败，返回值为零。若想获得更多错误信息，请调用GetLastError函数。</returns>
      [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
      private static extern bool EnumWindows(WindowEnumProc enumFunc, uint lParam);
      /// <summary>
      /// 获得一个指定子窗口的父窗口句柄。
      /// </summary>
      /// <param name="handle">函数要获得该子窗口的父窗口句柄。</param>
      /// <returns>如果函数成功，返回值为父窗口句柄。如果窗口无父窗口，则函数返回NULL。若想获得更多错误信息，请调用GetLastError函数。</returns>
      [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
      private static extern IntPtr GetParent(IntPtr handle);
      /// <summary>
      /// 该函数确定给定的窗口句柄是否标识一个已存在的窗口。
      /// </summary>
      /// <param name="handle">用于作判定的窗口的句柄。</param>
      /// <returns>如果窗口句柄标识了一个已存在的窗口，返回值为非零；如果窗口句柄标识的窗口不存在，返回值为零。</returns>
      [DllImport("user32.dll", EntryPoint = "IsWindow")]
      private static extern bool IsWindow(IntPtr handle);
      /// <summary>
      /// 设置调用线程的最后一个错误代码。
      /// </summary>
      /// <param name="errorCode">线程的最后一个错误代码。</param>
      [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
      private static extern void SetLastError(uint errorCode);
      /// <summary>
      /// 创建一个指定文件地址的进程。
      /// </summary>
      /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
      /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
      public static Process CreateProcess(string executeFileUrl)
      {
         return CreateProcess(executeFileUrl, EProcessPriority.RealTime);
      }
      /// <summary>
      /// 创建一个指定文件地址的进程，同时会允许指定一个进程的优先级。
      /// </summary>
      /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
      /// <param name="priority">需要被创建的进程的优先级。</param>
      /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
      /// <exception cref="FileNotFoundException">当指定的Windows可执行文件找不到时，则会抛出这个异常。</exception>
      public static Process CreateProcess(string executeFileUrl, EProcessPriority priority)
      {
         Process retval;
         if (!FileOperator.FileExists(executeFileUrl)) throw new FileNotFoundException("指定的Windows可执行文件不存在！");
         retval = Process.Start(executeFileUrl);
         retval.PriorityClass = (ProcessPriorityClass)priority;
         return retval;
      }
      /// <summary>
      /// 创建一个进程，在创建这个进程的时候会指定相关的权限和域。
      /// </summary>
      /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
      /// <param name="userName">启动进程时使用的用户名。</param>
      /// <param name="password">一个SecureString实例，它包含启动进程时要使用的密码。</param>
      /// <param name="domian">启动进程时要使用的域。</param>
      /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
      /// <exception cref="FileNotFoundException">当指定的Windows可执行文件找不到时，则会抛出这个异常。</exception>
      public static Process CreateProcess(string executeFileUrl, string userName, SecureString password, string domian)
      {
         if (!FileOperator.FileExists(executeFileUrl)) throw new FileNotFoundException("指定的Windows可执行文件不存在！");
         return Process.Start(executeFileUrl, userName, password, domian);
      }
      /// <summary>
      /// 创建一个进程，在创建这个进程会指定一些命令行参数，以及权限和域。
      /// </summary>
      /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
      /// <param name="arguments">启动该进程时传递的命令行实参。</param>
      /// <param name="userName">启动进程时使用的用户名。</param>
      /// <param name="password">一个SecureString实例，它包含启动进程时要使用的密码。</param>
      /// <param name="domian">启动进程时要使用的域。</param>
      /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
      public static Process CreateProcess(string executeFileUrl, string arguments, string userName, SecureString password, string domian)
      {
         return CreateProcess(executeFileUrl, arguments, userName, password, domian, EProcessPriority.RealTime);
      }
      /// <summary>
      /// 创建一个拥有指定优先级的进程，在创建这个进程会指定一些命令行参数，以及权限和域。
      /// </summary>
      /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
      /// <param name="arguments">启动该进程时传递的命令行实参。</param>
      /// <param name="userName">启动进程时使用的用户名。</param>
      /// <param name="password">一个SecureString实例，它包含启动进程时要使用的密码。</param>
      /// <param name="domian">启动进程时要使用的域。</param>
      /// <param name="priority">需要被创建的进程的优先级。</param>
      /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
      /// <exception cref="FileNotFoundException">当指定的Windows可执行文件找不到时，则会抛出这个异常。</exception>
      public static Process CreateProcess(string executeFileUrl, string arguments, string userName, SecureString password, string domian, EProcessPriority priority)
      {
         Process retval;
         if (!FileOperator.FileExists(executeFileUrl)) throw new FileNotFoundException("指定的Windows可执行文件不存在！");
         retval = Process.Start(executeFileUrl, arguments, userName, password, domian);
         retval.PriorityClass = (ProcessPriorityClass)priority;
         return retval;
      }
      /// <summary>
      /// 终结一个通过进程映像名称指定的进程。
      /// </summary>
      /// <param name="processName">需要被终结的进程的映像名称。</param>
      /// <returns>如果这个进程不存在或者因为其他原因，则会导致进程终结失败，终结失败会返回false。</returns>
      public static bool KillProcess(string processName)
      {
         bool retval = true;
         try
         {
            if (ProcessExists(processName) == false) retval = false;
            else
            {
               Parallel.ForEach(Process.GetProcesses(), (item) =>
               {
                  if (item.ProcessName == processName) item.Kill();
               });
            }
         }
         catch
         {
            retval = false;
         }
         return retval;
      }
      /// <summary>
      /// 终结一个通过进程标识符指定的进程
      /// </summary>
      /// <param name="processId">需要被终结的进程的标识符。</param>
      /// <returns>如果这个进程不存在或者因为其他原因，则会导致进程终结失败，终结失败会返回false。</returns>
      public static bool KillProcess(int processId)
      {
         bool retval = true;
         try
         {
            Parallel.ForEach(Process.GetProcesses(), (item) => { if (item.Id == processId) item.Kill(); });
         }
         catch
         {
            retval = false;
         }
         return retval;
      }
      /// <summary>
      /// 检查指定的进程是否存在。
      /// </summary>
      /// <param name="processName">需要被终结的进程的映像名称。</param>
      /// <returns>如果指定的进程存在则返回true，反之返回false。</returns>
      public static bool ProcessExists(string processName)
      {
         Process[] prc = Process.GetProcesses();
         bool retval = false;
         Parallel.ForEach(prc, (pr, stat) =>
         {
            if (processName == pr.ProcessName) retval = true;
            stat.Stop();
         });
         return retval;
      }
      /// <summary>
      /// 更改进程的优先级，但是这个操作可能存在风险，更改成功与否，都会返回一个Boolean值。
      /// </summary>
      /// <param name="processName">需要被修改优先级的进程的映像名称。</param>
      /// <param name="priority">需要被修改的进程优先级。</param>
      /// <returns>如果这个方法的操作成功则为true，反之为false。</returns>
      public static bool ChangeProcessPriority(string processName, EProcessPriority priority)
      {
         bool retval = true;
         Process[] ps = Process.GetProcesses();
         foreach (Process p in ps)
         {
            if (p.ProcessName == processName)
            {
               try
               {
                  p.PriorityClass = (ProcessPriorityClass)priority;
               }
               catch
               {
                  retval = false;
               }
            }
         }
         return retval;
      }
      /// <summary>
      /// 更改由指定句柄所表示的线程的优先级。
      /// </summary>
      /// <param name="threadHandle">指定线程的句柄。</param>
      /// <param name="priority">需要设置的线程优先级。</param>
      /// <returns>如果这个方法的操作成功则为true，反之为false。</returns>
      public static bool ChangeThreadPriority(IntPtr threadHandle, EThreadPriority priority)
      {
         return SetThreadPriority(threadHandle.ToInt64(), (long)priority) != 0 ? true : false;
      }
      /// <summary>
      /// 修改指定进程的权限。
      /// </summary>
      /// <param name="processName">指定的待修改进程。</param>
      /// <param name="systemName">需要被操作的系统，弱势本地系统，这里可以指定为空（NULL Or Nothing）。</param>
      /// <param name="operationType">待操作的类型，这个操作类型通畅可以用TOKEN_ADJUST_PRIVILEGES代替。</param>
      /// <param name="privileges">指定的权限（特权）。</param>
      /// <returns>如果这个方法的操作成功则为true，反之为false。</returns>
      /// <remarks>该操作允许用户修改指定的进程的权限，不过这个操作可能存在风险，所以在提升权限的时候要谨慎操作。</remarks>
      public static bool UpdateProcessPrivileges(string processName, string systemName, Int32 operationType, STokenPrivileges privileges)
      {
         IntPtr hwnd_t = IntPtr.Zero;
         bool rv;
         LookupPrivilegeValue(systemName, processName, privileges.Privileges.ParticularLuid);
         OpenProcessToken(GetCurrentProcess(), operationType, hwnd_t);
         AdjustTokenPrivileges(hwnd_t, false, privileges, 100000, new STokenPrivileges(), 0);
         rv = GetLastError() == ERROR_SUCCESS ? true : false;
         CloseHandle(hwnd_t);
         return rv;
      }
      /// <summary>
      /// 根据窗口标题来获取进程的ID。
      /// </summary>
      /// <param name="windowTitle">指定的窗口标题。</param>
      /// <returns>该操作会返回指定窗口标题所属进程的进程标识符（PID）。</returns>
      public static int GetProcessIdByWindowTitle(string windowTitle)
      {
         int processId = 0;
         Process[] arrayProcess = Process.GetProcesses();
         foreach (Process p in arrayProcess)
         {
            if (p.MainWindowTitle.IndexOf(windowTitle) != -1)
            {
               processId = p.Id;
               break;
            }
         }
         return processId;
      }
      /// <summary>
      /// 根据进程名称来获取进程的ID。
      /// </summary>
      /// <param name="processName">指定进程的名称，注意这里的进程名称不需要在后面添加后缀名。</param>
      /// <returns>该操作会返回指定进程名称所对应的进程标识符（PID）。</returns>
      public static int GetProcessIdByImageName(string processName)
      {
         int processId = 0;
         Process[] arrayProcess = Process.GetProcessesByName(processName);
         foreach (Process process in arrayProcess)
         {
            processId = process.Id;
            return processId;
         }
         return processId;
      }
      /// <summary>
      /// 根据窗体标题查找窗口句柄，另外这个操作支持标题模糊匹配。
      /// </summary>
      /// <param name="windowTitle">指定的窗口标题。</param>
      /// <returns>该操作会返回指定窗口标题所对应的窗口句柄。</returns>
      public static IntPtr GetWindowHandleByTitle(string windowTitle)
      {
         Process[] ps = Process.GetProcesses();
         foreach (Process p in ps)
         {
            if (p.MainWindowTitle.IndexOf(windowTitle) != -1) return p.MainWindowHandle;
         }
         return IntPtr.Zero;
      }
      /// <summary>
      /// 根据指定的进程名称获取这个进程的句柄。
      /// </summary>
      /// <param name="processName">指定的进程名称，注意这里的进程名称不需要在后面添加后缀名。</param>
      /// <returns>该操作会返回一个进程句柄，如果操作出现异常，则可能会返回一个Zero句柄。</returns>
      public static IntPtr GetHandleByImageName(string processName)
      {
         List<SProcessEntry32> list = new List<SProcessEntry32>();
         IntPtr handle = CreateToolhelp32Snapshot(0x2, 0);
         IntPtr processHandle = IntPtr.Zero;
         if ((int)handle > 0)
         {
            SProcessEntry32 pe32 = new SProcessEntry32();
            pe32.Size = (uint)Marshal.SizeOf(pe32);
            int bMore = Process32First(handle, ref pe32);
            while (bMore == 1)
            {
               IntPtr temp = Marshal.AllocHGlobal((int)pe32.Size);
               Marshal.StructureToPtr(pe32, temp, true);
               SProcessEntry32 pe32_s = (SProcessEntry32)Marshal.PtrToStructure(temp, typeof(SProcessEntry32));
               Marshal.FreeHGlobal(temp);
               list.Add(pe32_s);
               if (pe32_s.ExecuteFileName == processName)
               {
                  bMore = 2;
                  processHandle = GetCurrentWindowHandle(pe32_s.ProcessId);
                  break;
               }
               bMore = Process32Next(handle, ref pe32);
            }
         }
         return processHandle;
      }
      /// <summary>
      /// 获取当前窗口的句柄。
      /// </summary>
      /// <param name="processId">窗口所在进程的PID。</param>
      /// <returns>该操作会返回一个窗口句柄，如果操作出现异常，则可能会返回一个Zero句柄。</returns>
      [CLSCompliant(false)]
      public static IntPtr GetCurrentWindowHandle(uint processId)
      {
         IntPtr ptrWnd = IntPtr.Zero;
         uint uiPid = processId;
         object objWnd = _processHwnd[uiPid];
         if (objWnd != null)
         {
            ptrWnd = (IntPtr)objWnd;
            if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd)) return ptrWnd;
            else ptrWnd = IntPtr.Zero;
         }
         bool bResult = EnumWindows(new WindowEnumProc(EnumWindowsProc), uiPid);
         if (!bResult && Marshal.GetLastWin32Error() == 0)
         {
            objWnd = _processHwnd[uiPid];
            if (objWnd != null) ptrWnd = (IntPtr)objWnd;
         }
         return ptrWnd;
      }
      /// <summary>
      /// 窗口枚举，用于作为API函数EnumWindows的回调参数。
      /// </summary>
      /// <param name="handle">窗口的句柄。</param>
      /// <param name="lParam">用于传递的特定参数。</param>
      /// <returns>如果该操作成功，则返回true，否则返回false。</returns>
      [CLSCompliant(false)]
      public static bool EnumWindowsProc(IntPtr handle, uint lParam)
      {
         int uiPid = 0;
         if (GetParent(handle) == IntPtr.Zero)
         {
            GetWindowThreadProcessId(handle, out uiPid);
            if (uiPid == lParam)
            {
               _processHwnd.Add(uiPid, handle);
               SetLastError(0);
               return false;
            }
         }
         return true;
      }
   }
   /// <summary>
   /// 进程优先级枚举，这个枚举用来表示进程在执行态的优先级。
   /// </summary>
   public enum EProcessPriority : int
   {
      /// <summary>
      /// 指定进程的优先级在 Normal 之上，但在 High 之下。
      /// </summary>
      AboveNormal = 32768,
      /// <summary>
      /// 指定进程的优先级在 Idle 之上，但在 Normal 之下。
      /// </summary>
      BelowNormal = 16384,
      /// <summary>
      /// 指定进程执行必须立即执行的时间关键任务。
      /// </summary>
      High = 128,
      /// <summary>
      /// 指定此进程的线程只能在系统空闲时运行。
      /// </summary>
      Idle = 64,
      /// <summary>
      /// 指定进程没有特殊的安排需求。
      /// </summary>
      Normal = 32,
      /// <summary>
      /// 指定进程拥有可能的最高优先级。
      /// </summary>
      RealTime = 256
   }
   /// <summary>
   /// 线程优先级枚举，这个枚举用来表示线程在运行的时候的优先级。
   /// </summary>
   public enum EThreadPriority : int
   {
      /// <summary>
      /// 开始后台处理模式。
      /// </summary>
      BackgroundBegin = 65536,
      /// <summary>
      /// 终止后台处理模式。
      /// </summary>
      BackgroundEnd = 131072,
      /// <summary>
      /// 略微高于正常。
      /// </summary>
      AboveNormal = 1,
      /// <summary>
      /// 略微低于正常。
      /// </summary>
      BelowNormal = -1,
      /// <summary>
      /// 极高的优先级。
      /// </summary>
      Highest = 2,
      /// <summary>
      /// 空闲时运行。
      /// </summary>
      Idle = -15,
      /// <summary>
      /// 最低的优先级。
      /// </summary>
      Lowest = -2,
      /// <summary>
      /// 正常的优先级。
      /// </summary>
      Normal = 0,
      /// <summary>
      /// 基于时间关键性的优先级。
      /// </summary>
      TimeCritical = 15
   }
}
