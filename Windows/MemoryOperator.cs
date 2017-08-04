using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Cabinink.Windows
{
   /// <summary>
   /// 内存输入输出的类，但这个类不能被继承，另外在使用这个类所包含的函数之前，建议了解一些相关的技术知识。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class MemoryOperator
   {
      private const int TOKEN_QUERY = 0x00000008;//令牌查询。
      private const int PROCESS_ALL_ACCESS = 0x1f0fff;//获取最高权限。
      private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;//权限提升标记。
      /// <summary>
      /// 获取当前进程的一个伪句柄。
      /// </summary>
      /// <returns>获取当前进程的一个伪句柄，只要当前进程需要一个进程句柄，就可以使用这个伪句柄。该句柄可以复制，但不可继承。</returns>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern IntPtr GetCurrentProcess();
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
      /// 从指定内存中读取字节集数据。
      /// </summary>
      /// <param name="processHandle">指定进程的进程句柄。</param>
      /// <param name="baseAddress">内存地址。</param>
      /// <param name="buffer">本地进程中内存地址. 函数将读取的内容写入此处。</param>
      /// <param name="size">要传送的字节数。</param>
      /// <param name="numberOfBytesRead">实际传送的字节数。</param>
      /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
      public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr baseAddress, IntPtr buffer, int size, IntPtr numberOfBytesRead);
      /// <summary>
      /// 打开一个已存在的进程对象，并返回进程的句柄。
      /// </summary>
      /// <param name="desiredAccess">指定这个句柄要求的访问方法。</param>
      /// <param name="inheritHandle">指示这个函数是否继承句柄。</param>
      /// <param name="processId">进程标示符。</param>
      /// <returns>如果操作成功，返回值为指定进程的句柄，否则返回值为空，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
      [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
      public static extern IntPtr OpenProcess(int desiredAccess, bool inheritHandle, int processId);
      /// <summary>
      /// 关闭包含句柄的对象。
      /// </summary>
      /// <param name="handle">指定的非托管对象的句柄。</param>
      /// <returns>该操作结束之后将会返回一个错误码，如果这个错误码为0则表示操作正常，非0表示操作出现异常。</returns>
      /// <remarks>关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。操作结束之后返回非零表示成功，零表示失败。会设置GetLastError。</remarks>
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
      private static extern int CloseHandle(SafeFileHandle handle);
      /// <summary>
      /// 将指定数据写入到内存。
      /// </summary>
      /// <param name="processHandle">指定进程的句柄，如参数传数据为INVALID_HANDLE_VALUE，目标进程为自身进程。</param>
      /// <param name="baseAddress">需要写入的内存地址，写入之前，此函数将先检查目标地址是否可用，并能容纳待写入的数据。</param>
      /// <param name="buffer">指向要写的数据的指针。</param>
      /// <param name="size">要写入的字节数。</param>
      /// <param name="numberOfBytesWritten">实际写入的字节数。</param>
      /// <returns></returns>
      [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
      private static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr baseAddress, int[] buffer, int size, IntPtr numberOfBytesWritten);
      /// <summary>
      /// 从指定进程中获取指定内存地址所存储的值。
      /// </summary>
      /// <param name="baseAddress">需要读取的内存地址。</param>
      /// <param name="processName">进程名称。</param>
      /// <returns>该操作执行没有任何异常之后，则将会返回参数baseAddress指定内存地址所包含的值。</returns>
      /// <exception cref="MemoryIOException">如果发生操作权限不够或者其他失败问题时，则将会抛出这个异常。</exception>
      public static long Read(long baseAddress, string processName)
      {
         try
         {
            byte[] buffer = new byte[4];
            IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, ProcessManager.GetProcessIdByImageName(processName));
            ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero);
            CloseHandle(new SafeFileHandle(hProcess, true));
            return Marshal.ReadInt64(byteAddress);
         }
         catch
         {
            throw new MemoryIOException();
         }
      }
      /// <summary>
      /// 通过指定的PID从这个进程中获取指定内存地址所存储的值。
      /// </summary>
      /// <param name="baseAddress">需要读取的内存地址。</param>
      /// <param name="processId">进程标识符。</param>
      /// <returns>该操作执行没有任何异常之后，则将会返回参数baseAddress指定内存地址所包含的值。</returns>
      /// <exception cref="MemoryIOException">如果发生操作权限不够或者其他失败问题时，则将会抛出这个异常。</exception>
      public static long Read(long baseAddress, int processId)
      {
         try
         {
            byte[] buffer = new byte[4];
            IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero);
            CloseHandle(new SafeFileHandle(hProcess, true));
            return Marshal.ReadInt64(byteAddress);
         }
         catch
         {
            throw new MemoryIOException();
         }
      }
      /// <summary>
      /// 将数据写入某个进程的指定的内存地址。
      /// </summary>
      /// <param name="baseAddress">需要写入的内存地址。</param>
      /// <param name="processName">进程名称。</param>
      /// <param name="value">需要写入到内存的值。</param>
      /// <exception cref="MemoryIOException">如果发生操作权限不够或者其他失败问题时，则将会抛出这个异常。</exception>
      public static void Write(long baseAddress, string processName, int value)
      {
         try
         {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, ProcessManager.GetProcessIdByImageName(processName));
            WriteProcessMemory(hProcess, (IntPtr)baseAddress, new int[] { value }, 4, IntPtr.Zero);
            CloseHandle(new SafeFileHandle(hProcess, true));
         }
         catch
         {
            throw new MemoryIOException();
         }
      }
      /// <summary>
      /// 将数据写入PID所对应的某个进程中指定的内存地址。
      /// </summary>
      /// <param name="baseAddress">需要写入的内存地址。</param>
      /// <param name="processId">进程标识符。</param>
      /// <param name="value">需要写入到内存的值。</param>
      /// <exception cref="MemoryIOException">如果发生操作权限不够或者其他失败问题时，则将会抛出这个异常。</exception>
      public static void Write(long baseAddress, int processId, int value)
      {
         try
         {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            WriteProcessMemory(hProcess, (IntPtr)baseAddress, new int[] { value }, 4, IntPtr.Zero);
            CloseHandle(new SafeFileHandle(hProcess, true));
         }
         catch
         {
            throw new MemoryIOException();
         }
      }
      /// <summary>
      /// 获取指定变量的物理内存地址，不过该方法可能不适用于引用类型和其他托管类型变量。
      /// </summary>
      /// <param name="variable">需要获取物理内存地址的变量。</param>
      /// <returns>该操作将会返回一个以句柄形式存储的内存地址，如果需要将这个地址转换为整型数据，则需要在返回值进行ToInt32或者ToInt64方法的访问。</returns>
      public static IntPtr GetVariableMemoryAddress(object variable)
      {
         GCHandle handle = GCHandle.Alloc(variable);
         return GCHandle.ToIntPtr(handle);
      }
      /// <summary>
      /// 获取指定Int64变量的物理内存地址。
      /// </summary>
      /// <param name="variable">需要获取物理内存地址的Int64变量。</param>
      /// <returns>该操作将会返回一个以指针形式存储的内存地址，但是这个方法涉及到不安全代码的操作，因此在使用这个方法需要慎重考虑。</returns>
      [CLSCompliant(false)]
      public static unsafe long* GetVariableMemoryAddress(long variable) => &variable;
      /// <summary>
      /// 获取指定UInt64变量的物理内存地址。
      /// </summary>
      /// <param name="variable">需要获取物理内存地址的UInt64变量。</param>
      /// <returns>该操作将会返回一个以指针形式存储的内存地址，但是这个方法涉及到不安全代码的操作，因此在使用这个方法需要慎重考虑。</returns>
      [CLSCompliant(false)]
      public static unsafe ulong* GetVariableMemoryAddress(ulong variable) => &variable;
      /// <summary>
      /// 获取指定Boolean变量的物理内存地址。
      /// </summary>
      /// <param name="variable">需要获取物理内存地址的Boolean变量。</param>
      /// <returns>该操作将会返回一个以指针形式存储的内存地址，但是这个方法涉及到不安全代码的操作，因此在使用这个方法需要慎重考虑。</returns>
      [CLSCompliant(false)]
      public static unsafe bool* GetVariableMemoryAddress(bool variable) => &variable;
      /// <summary>
      /// 获取指定Double变量的物理内存地址。
      /// </summary>
      /// <param name="variable">需要获取物理内存地址的Double变量。</param>
      /// <returns>该操作将会返回一个以指针形式存储的内存地址，但是这个方法涉及到不安全代码的操作，因此在使用这个方法需要慎重考虑。</returns>
      [CLSCompliant(false)]
      public static unsafe double* GetVariableMemoryAddress(double variable) => &variable;
      /// <summary>
      /// 获取指定Char变量的物理内存地址。
      /// </summary>
      /// <param name="variable">需要获取物理内存地址的Char变量。</param>
      /// <returns>该操作将会返回一个以指针形式存储的内存地址，但是这个方法涉及到不安全代码的操作，因此在使用这个方法需要慎重考虑。</returns>
      [CLSCompliant(false)]
      public static unsafe char* GetVariableMemoryAddress(char variable) => &variable;
      /// <summary>
      /// 提升调用方的操作权限，这个操作在调用方执行一次即可。
      /// </summary>
      /// <param name="callerProcessHandle">调用方所属进程的进程句柄。</param>
      /// <remarks>调用方在这里不仅仅局限于当前应用程序集或者当前进程，而且还包含其他的第三方应用程序（或者第三方应用程序的进程）。</remarks>
      public static void AdjustCallerPrivilege(IntPtr callerProcessHandle)
      {
         OpenProcessToken(callerProcessHandle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, IntPtr.Zero);
      }
   }
   /// <summary>
   /// 内存IO操作出现问题时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class MemoryIOException : Exception
   {
      public MemoryIOException() : base("未授权的操作，或者存在其他无法捕获的内存操作异常。") { }
      public MemoryIOException(string message) : base(message) { }
      public MemoryIOException(string message, Exception inner) : base(message, inner) { }
      protected MemoryIOException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
