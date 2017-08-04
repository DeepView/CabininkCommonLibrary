using System;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Cabinink.IOSystem
{
   /// <summary>
   /// 非托管内核对象安全属性结构类，该类的部分方法不支持Visual Basic代码访问。
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public class SecurityAttributes
   {
      /// <summary>
      /// 结构大小。
      /// </summary>
      public int Length;
      /// <summary>
      /// 安全描述符。
      /// </summary>
      public IntPtr SecurityFlag;
      /// <summary>
      /// 安全描述的对象能否被新创建句柄的进程所继承。
      /// </summary>
      public bool IsInheritHandle;
   }
   /// <summary>
   /// 包含了用于异步输入输出的信息的结构类。
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public class OverLapped
   {
      /// <summary>
      /// 预留给操作系统使用。它指定一个独立于系统的状态，当GetOverlappedResult函数返回时没有设置扩展错误信息ERROR_IO_PENDING时有效。
      /// </summary>
      [CLSCompliant(false)]
      public uint Internal;
      /// <summary>
      /// 预留给操作系统使用。它指定长度的数据转移，当GetOverlappedResult函数返回TRUE时有效。
      /// </summary>
      [CLSCompliant(false)]
      public uint InternalHigh;
      /// <summary>
      /// 该文件的位置是从文件起始处的字节偏移量。调用进程设置这个成员之前调用ReadFile或WriteFile函数。当读取或写入命名管道和通信设备时这个成员被忽略设为零。
      /// </summary>
      [CLSCompliant(false)]
      public uint Offset;
      /// <summary>
      /// 指定文件传送的字节偏移量的高位字。当读取或写入命名管道和通信设备时这个成员被忽略设为零。
      /// </summary>
      [CLSCompliant(false)]
      public uint OffsetHigh;
      /// <summary>
      /// 指针，指向文件传送位置。
      /// </summary>
      public IntPtr Pointer;
      /// <summary>
      /// 在转移完成时处理一个事件设置为有信号状态。调用进程集这个成员在调用ReadFile、WriteFile、TransactNamedPipe、ConnectNamedPipe函数之前。
      /// </summary>
      public IntPtr EventHandle;
   }
   /// <summary>
   /// 非托管内核对象访问基础类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class UnmanagedKernelObjectBase
   {
      private string _localUrl;//需要打开的非托管内核对象的本地地址。
      private SafeFileHandle _fileHandle;//非托管内核对象的文件句柄。
      /// <summary>
      /// 打开或者创建文件。
      /// </summary>
      /// <param name="fileUrl">打开或者需要被创建的文件的本地路径。</param>
      /// <param name="accessedObject">指定类型的访问对象。如果为GENERIC_READ表示允许对设备进行读访问；如果为GENERIC_WRITE表示允许对设备进行写访问（可组合使用）；如果为零，表示只允许获取与一个设备有关的信息。</param>
      /// <param name="sharedMode">共享模式，如果是零表示不共享；如果是FILE_SHARE_DELETE表示随后打开操作对象会成功只要删除访问请求；如果是FILE_SHARE_READ随后打开操作对象会成功只有请求读访问；如果是FILE_SHARE_WRITE随后打开操作对象会成功只有请求写访问。</param>
      /// <param name="securityAttributes">指向一个SECURITY_ATTRIBUTES结构的指针，定义了文件的安全特性。</param>
      /// <param name="catchedExceptionMethod">当创建文件遇到诸如文件已存在等异常的时候，该如何处理这些异常。</param>
      /// <param name="flagAttributes">文件被创建之后所赋予的文件属性。</param>
      /// <param name="objectHandle">一个文件或设备句柄，表示按这个参数给出的句柄为模板创建文件（就是将该句柄文件拷贝到lpFileName指定的路径，然后再打开）。它将指定该文件的属性扩展到新创建的文件上面，这个参数可用于将某个新文件的属性设置成与现有文件一样，并且这样会忽略dwAttrsAndFlags。通常这个参数设置为NULL，为空表示不使用模板，一般为空。</param>
      /// <returns>打开文件对象之后获得的安全文件句柄。</returns>
      /// <remarks>打开或者创建文件，但是实际上这是一个多功能的函数，可打开或创建以下对象，并返回可访问的句柄：控制台，通信资源，目录（只读打开），磁盘驱动器，文件，邮槽，管道。函数如执行成功，则返回文件句柄。另外，CreateFile的涵义是创建File这个内核对象，而不是创建物理磁盘上的“文件”。在Win32 API中有一系列操作内核对象的函数，创建内核对象的函数大多命名为CreateXxxx型。详细信息可访问https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx。</remarks>
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      private static extern SafeFileHandle CreateFile(string fileUrl, uint accessedObject, uint sharedMode, IntPtr securityAttributes, uint catchedExceptionMethod, uint flagAttributes, IntPtr objectHandle);
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
      /// WriteFile函数将数据写入一个文件。也可将这个函数应用于对通信设备、管道、套接字以及邮槽的处理。不支持Visual Basic代码访问。
      /// </summary>
      /// <param name="fileHandle">一个文件的句柄。</param>
      /// <param name="buffer">一个指向将写入文件的数据缓冲区的指针。</param>
      /// <param name="numberOfByteToWrite">要写入数据的字节数量。如写入零字节，表示什么都不写入，但会更新文件的“上一次修改时间”。针对位于远程系统的命名管道，限制在65535个字节以内。</param>
      /// <param name="numberOfByteToWriteFinal">实际写入文件的字节数量（此变量是用来返回的）。</param>
      /// <param name="overLapped">倘若在指定FILE_FLAG_OVERLAPPED的前提下打开文件，这个参数就必须引用一个特殊的结构。那个结构定义了一次异步写操作。否则，该参数应置为空（将声明变为ByVal As Long，并传递零值）。</param>
      /// <returns>TRUE（非零）表示成功，否则返回零。会设置GetLastError。</returns>
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      public unsafe static extern bool WriteFile(SafeFileHandle fileHandle, void* buffer, uint numberOfByteToWrite, ref uint numberOfByteToWriteFinal, OverLapped overLapped);
      /// <summary>
      /// 从文件指针指向的位置开始将数据读出到一个文件中，且支持同步和异步操作。不支持Visual Basic代码访问。
      /// </summary>
      /// <param name="fileHandle">需要读入数据的文件指针,这个指针指向的文件必须是GENERIC_READ访问属性的文件。</param>
      /// <param name="buffer">接收数据的缓冲区。</param>
      /// <param name="numberOfByteToRead">指定要读取的字节数。</param>
      /// <param name="numberOfByteToReadFinal">指向一个DWORD类型变量的指针，用来接收读取的字节数。如果下一个参数为NULL，那么一定要传入这个参数。</param>
      /// <param name="overLapped">OVERLAPPED结构体指针,如果文件是以FILE_FLAG_OVERLAPPED方式打开的话,那么这个指针就不能为NULL。</param>
      /// <returns>TRUE（非零）表示成功，否则返回零。会设置GetLastError。如启动的是一次异步读操作，则函数会返回零值，并将ERROR_IO_PENDING设置成GetLastError的结果。如结果不是零值，但读入的字节数小于nNumberOfBytesToRead参数指定的值，表明早已抵达了文件的结尾。</returns>
      /// <remarks>如果文件打开方式没有指明FILE_FLAG_OVERLAPPED的话，当程序调用成功时，它将实际读出文件的字节数保存到lpNumberOfBytesRead指明的地址空间中。FILE_FLAG_OVERLAPPED允许对文件进行重叠操作。如果文件要交互使用的话，当函数调用完毕时要记得调整文件指针。从文件中读出数据。与fread函数相比，这个函数要明显灵活的多。该函数能够操作通信设备、管道、套接字以及邮槽。</remarks>
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      public unsafe static extern bool ReadFile(SafeFileHandle fileHandle, void* buffer, uint numberOfByteToRead, ref uint numberOfByteToReadFinal, OverLapped overLapped);
      /// <summary>
      /// 判断文件长度。不支持Visual Basic代码访问。
      /// </summary>
      /// <param name="fileHandle">文件的句柄。</param>
      /// <param name="fileSizeHigh">指定一个长整数，用于装载一个64位文件长度的头32位。如这个长度没有超过2^32个字节，则该参数可以设为NULL。</param>
      /// <returns>如果函数调用成功，则返回值为文件大小的低位双字，lpFileSizeHigh返回文件大小的高阶双字。如果函数返回值为INVALID_FILE_SIZE，并且GetLastError函数返回值非NO_ERROR，则函数调用失败。</returns>
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      public static extern uint GetFileSize(SafeFileHandle fileHandle, ref IntPtr fileSizeHigh);
      /// <summary>
      /// 构造函数，创建一个具有指定文件对象地址的实例。
      /// </summary>
      /// <param name="fileUrl">指定的文件对象地址。</param>
      /// <exception cref="ArgumentNullException">当fileUrl参数为空字符串时，则会抛出这个异常。</exception>
      public UnmanagedKernelObjectBase(string fileUrl)
      {
         if (string.IsNullOrEmpty(fileUrl)) throw new ArgumentNullException("fileUrl", "参数不能为空字符串！");
         _localUrl = fileUrl;
      }
      /// <summary>
      /// 获取或设置当前实例所包含的非托管内核对象的文件地址。
      /// </summary>
      /// <exception cref="ArgumentNullException">当fileUrl参数为空字符串时，则会抛出这个异常。</exception>
      public string UnmanagedKernelObjectUrl
      {
         get => _localUrl;
         set
         {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("fileUrl", "参数不能为空字符串！");
            _localUrl = value;
         }
      }
      /// <summary>
      /// 获取当前实例中已经加载的内核对象的句柄。
      /// </summary>
      public SafeFileHandle Handle
      {
         get { return _fileHandle; }
         private set { _fileHandle = value; }
      }
      /// <summary>
      /// 通过指定的访问控制符号和共享模式来创建或者加载当前实例所指向的非托管内核对象。
      /// </summary>
      /// <param name="access">非托管内核对象的访问控制。</param>
      /// <param name="sharedMode">非托管内核对象的共享模式。</param>
      [CLSCompliant(false)]
      public void Load(EKernelObjectAccess access, EShareMode sharedMode)
      {
         SecurityAttributes attributes = null;
         Load(access, sharedMode, ref attributes, ECreationDisposition.OpenExisting, EFlagsAndAttributes.Normal, IntPtr.Zero);
      }
      /// <summary>
      /// 通过指定的访问控制符号、共享模式、安全属性结构、创建模式、属性和设备句柄来创建或者加载当前实例所指向的非托管内核对象。
      /// </summary>
      /// <param name="access">非托管内核对象的访问控制。</param>
      /// <param name="sharedMode">非托管内核对象的共享模式。</param>
      /// <param name="attributes">非托管内核对象安全属性结构。</param>
      /// <param name="createMode">创建非托管内核对象的模式。</param>
      /// <param name="objectAttributes">非托管内核对象的属性与标记。</param>
      /// <param name="handle">一个文件或设备句柄，表示按这个参数给出的句柄为模板创建文件。</param>
      [CLSCompliant(false)]
      [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
      public void Load(EKernelObjectAccess access, EShareMode sharedMode, ref SecurityAttributes attributes, ECreationDisposition createMode, EFlagsAndAttributes objectAttributes, IntPtr handle)
      {
         GCHandle attributesGCHandle = GCHandle.Alloc(attributes);
         Handle = CreateFile(
                     UnmanagedKernelObjectUrl,
                     (uint)access,
                     (uint)sharedMode,
                     GCHandle.ToIntPtr(attributesGCHandle),
                     (uint)createMode,
                     (uint)objectAttributes,
                     handle
         );
         attributesGCHandle.Free();
         if (Handle.IsInvalid) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
      }
      /// <summary>
      /// 关闭当前的内核对象。
      /// </summary>
      /// <returns>如果这个操作成功，则返回True，否则返回False。</returns>
      [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
      public bool Close()
      {
         if (Handle.IsInvalid) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
         return CloseHandle(Handle) != 0 ? true : false;
      }
   }
   /// <summary>
   /// 创建内核对象的模式，这个枚举用于表示创建非托管内核对象的模式。
   /// </summary>
   public enum ECreationDisposition : int
   {
      /// <summary>
      /// 创建新的对象，若对象存在则抛出异常。
      /// </summary>
      CreateNew = 0x0001,
      /// <summary>
      /// 忽略CreateNew的异常来创建对象。
      /// </summary>
      CreateAlways = 0x0002,
      /// <summary>
      /// 打开一个存在的对象。
      /// </summary>
      OpenExisting = 0x0003,
      /// <summary>
      /// 打开一个对象，若对象不存在则忽略相应的异常。
      /// </summary>
      OpenAlways = 0x0004,
      /// <summary>
      /// 若对象存在则修改对象大小为0。
      /// </summary>
      TruncateExisting = 0x0005
   }
   /// <summary>
   /// 内核对象访问控制，这个枚举用于表示非托管内核对象的访问控制。
   /// </summary>
   [CLSCompliant(false)]
   public enum EKernelObjectAccess : uint
   {
      /// <summary>
      /// 只允许获取设备相关的信息。
      /// </summary>
      GenericGetInfo = 0x00000000,
      /// <summary>
      /// 允许对设备进行读访问。
      /// </summary>
      GenericRead = 0x80000000,
      /// <summary>
      /// 允许对设备进行写访问。
      /// </summary>
      GenericWrite = 0x40000000
   }
   /// <summary>
   /// 内核对象的共享模式，这个枚举用于实现非托管内核对象的共享模式。
   /// </summary>
   public enum EShareMode : int
   {
      /// <summary>
      /// 防止其他进程打开一个文件或设备。
      /// </summary>
      Monopoly = 0x0000,
      /// <summary>
      /// 允许在文件或设备上进行随后的打开操作以请求读取访问权限。
      /// </summary>
      Read = 0x0001,
      /// <summary>
      /// 允许在文件或设备上进行后续的打开操作以请求写访问。
      /// </summary>
      Write = 0x0002,
      /// <summary>
      /// 允许在文件或设备上进行后续的打开操作来请求删除访问。
      /// </summary>
      Delete = 0x0004
   }
   /// <summary>
   /// 内核对象的属性与标记，这个枚举用于表示非托管内核对象的属性与标记。
   /// </summary>
   [CLSCompliant(false)]
   public enum EFlagsAndAttributes : uint
   {
      /// <summary>
      /// 只读。
      /// </summary>
      OnlyRead = 0x00000001,
      /// <summary>
      /// 隐藏。
      /// </summary>
      Hidden = 0x00000002,
      /// <summary>
      /// 系统级别。
      /// </summary>
      SystemLevel = 0x00000004,
      /// <summary>
      /// 应归档。
      /// </summary>
      Archive = 0x00000020,
      /// <summary>
      /// 常规属性。
      /// </summary>
      Normal = 0x00000080,
      /// <summary>
      /// 临时存储。
      /// </summary>
      Temporary = 0x00000100,
      /// <summary>
      /// 数据不能立即使用。
      /// </summary>
      Offline = 0x00001000,
      /// <summary>
      /// 被加密。
      /// </summary>
      Encrypted = 0x00004000,
      /// <summary>
      /// 该文件正在被打开或创建用于备份或恢复操作。
      /// </summary>
      BackupSemantics = 0x02000000,
      /// <summary>
      /// 该文件将被删除后，它的所有处理都是关闭的，其中包括指定的句柄和任何其他的打开或重复的句柄。
      /// </summary>
      DeleteOnClose = 0x04000000,
      /// <summary>
      /// 该文件或设备正在被打开，没有系统缓存，用于读取和写入数据。
      /// </summary>
      NoBuffering = 0x20000000,
      /// <summary>
      /// 被请求的文件数据，但它应该继续位于远程存储。
      /// </summary>
      OpenNoRecall = 0x00100000,
      /// <summary>
      /// 正常的重分析点处理不会发生。
      /// </summary>
      OpenReparsePoint = 0x00200000,
      /// <summary>
      /// 该文件或设备正在被打开或创建为异步I/O。
      /// </summary>
      Overlapped = 0x40000000,
      /// <summary>
      /// 访问将根据POSIX规则。
      /// </summary>
      PosixSemantics = 0x00100000,
      /// <summary>
      /// 访问的目的是随机的。
      /// </summary>
      RandomAccess = 0x10000000,
      /// <summary>
      /// 该文件或设备正在使用。
      /// </summary>
      SessionAware = 0x00800000,
      /// <summary>
      /// 针对连续访问对文件缓冲进行优化。
      /// </summary>
      SequentialScan = 0x08000000,
      /// <summary>
      /// 操作系统不得推迟对文件的写操作。
      /// </summary>
      WriteThrought = 0x80000000
   }
}
