using System;
using System.IO;
using System.Text;
using System.Linq;
using Cabinink.TypeExtend;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cabinink.IOSystem.Security;
using System.Runtime.Serialization;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem
{
   /// <summary>
   /// 包含ShellExecuteEx使用的信息的一个非托管结构。
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public struct SShellExecuteInfo
   {
      /// <summary>
      /// 结构体所占用的大小，SHELLEXECUTEINFO结构的CLR实现版本，详情请参考https://msdn.microsoft.com/en-us/library/bb759784%28VS.85%29.aspx?f=255&amp;MSPPError=-2147217396。
      /// </summary>
      public int SizeOfStructure;
      /// <summary>
      /// 表明其他结构成员的内容和有效性的标志。
      /// </summary>
      [CLSCompliant(false)]
      public uint Flags;
      /// <summary>
      /// 可选值，父窗口句柄，用于显示执行此功能时系统可能产生的任何消息框，该值可以是NULL。
      /// </summary>
      public IntPtr Handle;
      /// <summary>
      /// 用于指定要执行的操作，可用Verb的集合取决于特定的文件或文件夹。
      /// </summary>
      [MarshalAs(UnmanagedType.LPStr)]
      public string Verb;
      /// <summary>
      /// 空终止字符串的地址，它指定ShellExecuteEx将执行由Verb参数指定的操作的文件或对象的名称。
      /// </summary>
      [MarshalAs(UnmanagedType.LPStr)]
      public string FileUrl;
      /// <summary>
      /// 包含应用程序参数的以空字符结尾的字符串的地址。
      /// </summary>
      [MarshalAs(UnmanagedType.LPStr)]
      public string Parameters;
      /// <summary>
      /// 指定工作目录名称的以空字符结尾的字符串的地址。
      /// </summary>
      [MarshalAs(UnmanagedType.LPStr)]
      public string Directory;
      /// <summary>
      /// 指定应用程序在打开时如何显示的标志。
      /// </summary>
      public int ShowFlags;
      /// <summary>
      /// 如果设置了SEE_MASK_NOCLOSEPROCESS并且ShellExecuteEx调用成功，则它将此成员设置为大于32的值，如果该函数失败，则将其设置为指示失败原因的SE_ERR_XXX错误值。虽然这个参数为了兼容16位Windows应用程序而声明为HINSTANCE，但它并不是真正的HINSTANCE，它只能转换为int，并与32或以下SE_ERR_XXX错误代码进行比较。
      /// </summary>
      public IntPtr InstanceApp;
      /// <summary>
      /// 一个ITEMIDLIST结构（PCIDLIST_ABSOLUTE）的地址，用于包含唯一标识要执行的文件的项目标识符列表。
      /// </summary>
      public IntPtr ItemIdList;
      /// <summary>
      /// 以空值终止的字符串的地址。
      /// </summary>
      [MarshalAs(UnmanagedType.LPStr)]
      public string Class;
      /// <summary>
      /// 文件类型的注册表项句柄，此注册表项的访问权限应设置为KEY_READ。
      /// </summary>
      public IntPtr RegistryHandle;
      /// <summary>
      /// 与应用程序关联的键盘快捷键，低位字是虚拟键码，高位字是修饰符标志（HOTKEYF_），有关修饰符标志的列表，请参阅WM_SETHOTKEY消息的描述。
      /// </summary>
      [CLSCompliant(false)]
      public uint ShortcutKey;
      /// <summary>
      /// 文件类型图标的句柄。
      /// </summary>
      public IntPtr Icon;
      /// <summary>
      /// 新启动的应用程序的句柄。
      /// </summary>
      public IntPtr Process;
   }
   /// <summary>
   /// 文件操作类，用于实现文件的常用操作，例如创建文、打开、读写文件等等。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class FileOperator
   {
      private const int DEFAULT_BUFFER_SIZE = 4096;//默认文件缓冲大小
      private const int SW_SHOW = 5;//API常量，激活窗口并以当前的大小和位置显示它。
      private const uint SEE_MASK_INVOKEIDLIST = 12;//API常量，使用所选项目快捷菜单处理程序的IContextMenu界面。
      private const string DRIVE_FORMAT_NTFS = "NTFS";//表示NTFS文件系统。
      private const string DRIVE_FORMAT_FAT12 = "FAT12";//表示FAT12文件系统。
      private const string DRIVE_FORMAT_FAT16 = "FAT16";//表示FAT16文件系统。
      private const string DRIVE_FORMAT_FAT32 = "FAT32";//表示FAT32文件系统。
      /// <summary>
      /// 对指定的文件执行操作。
      /// </summary>
      /// <param name="executeInfo">一个指向SShellExecuteInfo（Windows编程下为SHELLEXECUTEINFO结构）结构的指针，该结构包含并接收有关正在执行的应用程序的信息。</param>
      /// <returns>如果函数成功，返回值为非零，如果函数失败，返回值为零。若想获得更多错误信息，请调用GetLastError函数。</returns>
      [DllImport("shell32.dll")]
      private static extern bool ShellExecuteEx(ref SShellExecuteInfo executeInfo);
      /// <summary>
      /// 仅通过一个文件路径来创建一个文件
      /// </summary>
      /// <param name="fileUrl">指定的本地文件路径，即需要被创建的文件应该保存于本地磁盘的哪一个位置。</param>
      /// <remarks>该重载版本，以及CreateFile(stringl, FileMode)、CreateFile(string fileUrl, FileMode)和CreateFile(string fileUrl, FileMode, FileAccess, FileShare)都可能会触发完整参数版本中所设置的异常触发条件。</remarks>
      public static void CreateFile(string fileUrl) => CreateFile(fileUrl, FileMode.CreateNew);
      /// <summary>
      /// 通过文件路径、创建模式来创建一个文件。
      /// </summary>
      /// <param name="fileUrl">指定的本地文件路径，即需要被创建的文件应该保存于本地磁盘的哪一个位置。</param>
      /// <param name="mode">一个确定如何打开或创建文件的常数。</param>
      public static void CreateFile(string fileUrl, FileMode mode)
      {
         CreateFile(fileUrl, mode, FileAccess.ReadWrite, FileShare.None);
      }
      /// <summary>
      /// 通过文件路径、创建模式、访问模式和进程共享模式来创建一个文件。
      /// </summary>
      /// <param name="fileUrl">指定的本地文件路径，即需要被创建的文件应该保存于本地磁盘的哪一个位置。</param>
      /// <param name="mode">一个确定如何打开或创建文件的常数。</param>
      /// <param name="access">一个常数，用于确定创建文件时访问这个文件的方式。</param>
      /// <param name="share">确定文件将如何由进程共享。</param>
      public static void CreateFile(string fileUrl, FileMode mode, FileAccess access, FileShare share)
      {
         CreateFile(fileUrl, mode, access, share, DEFAULT_BUFFER_SIZE, false);
      }
      /// <summary>
      /// 通过文件路径、创建模式、访问模式、进程共享模式、指定的文件缓冲大小和I/O异步开关来创建一个文件。
      /// </summary>
      /// <param name="fileUrl">指定的本地文件路径，即需要被创建的文件应该保存于本地磁盘的哪一个位置。</param>
      /// <param name="mode">一个确定如何打开或创建文件的常数。</param>
      /// <param name="access">一个常数，用于确定创建文件时访问这个文件的方式。</param>
      /// <param name="share">确定文件将如何由进程共享。</param>
      /// <param name="bufferSize">一个大于零的正Int32值，表示缓冲区大小。其中，默认缓冲区大小为4096。</param>
      /// <param name="isAsync">指定使用异步 I/O 还是同步I/O。但是，请注意，基础操作系统可能不支持异步I/O，因此在指定true后，根据所用平台，句柄可能同步打开。</param>
      /// <exception cref="FileIsExistedException">如果fileUrl参数指定的文件已经存在，并且mode的值为FileMode.CreateNew，则会引发该异常。</exception>
      public static void CreateFile(string fileUrl, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool isAsync)
      {
         if (mode == FileMode.CreateNew && FileExists(fileUrl)) throw new FileIsExistedException(new Uri(fileUrl));
         FileStream fStream = new FileStream(fileUrl, mode, access, share, bufferSize, isAsync);
         fStream.Close();
      }
      /// <summary>
      /// 获取指定的文件是否存在
      /// </summary>
      /// <param name="fileUrl">需用用于判定文件存在性的文件路径。</param>
      /// <returns>如果这个文件存在，则会返回true，否则会返回false。</returns>
      public static bool FileExists(string fileUrl) => File.Exists(fileUrl);
      /// <summary>
      /// 通过一个文件路径来读取文件内容。
      /// </summary>
      /// <param name="fileUrl">需要读取文本的文件路径。</param>
      /// <returns>如果该操作没有产生任何错误，则将会以GB2312的编码格式返回该文件的全部文件内容文本。</returns>
      public static string ReadFileContext(string fileUrl)
      {
         return ReadFileContext(fileUrl, true, Encoding.GetEncoding("GB2312"));
      }
      /// <summary>
      /// 通过文件路径，读取标志和编码格式来读取一个文件。
      /// </summary>
      /// <param name="fileUrl">需要读取文本的文件路径。</param>
      /// <param name="isReadToEnd">用于决定是否读取到文件末尾，如果该值为true，操作无异常后将返回文件的全部内容，否则只会返回一行文件内容文本。</param>
      /// <param name="encoding">读取文件时需要采用的文件内容编码格式。</param>
      /// <returns>如果该操作没有产生任何错误，则将会返回该文件的全部或者部分文件内容文本。</returns>
      /// <exception cref="FileNotFoundException">如果fileUrl参数指定的文件不存在时，则会引发这个异常。</exception>
      public static string ReadFileContext(string fileUrl, bool isReadToEnd, Encoding encoding)
      {
         if (!FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到！", fileUrl);
         string fContext = string.Empty;
         StreamReader fReader = new StreamReader(fileUrl, encoding);
         if (isReadToEnd) fContext = fReader.ReadToEnd(); else fContext = fReader.ReadLine();
         fReader.Close();
         return fContext;
      }
      /// <summary>
      /// 通过指定的追加指示向指定文件存储文本内容。
      /// </summary>
      /// <param name="fileUrl">需要存储文本的文件路径。</param>
      /// <param name="writedContext">需要写入文件的文本。</param>
      /// <param name="isAppend">一个Boolean类型参数，用于决定该操作应该以追加还是覆盖的方式存储文本，如果该参数为true，则该操作会以追加方式存储文本，否则会以覆盖方式存储文本。</param>
      public static void WriteFile(string fileUrl, string writedContext, bool isAppend)
      {
         WriteFile(fileUrl, writedContext, isAppend, Encoding.GetEncoding("GB2312"));
      }
      /// <summary>
      /// 通过一个追加指示和编码格式向指定的文件存储文本内容。
      /// </summary>
      /// <param name="fileUrl">需要存储文本的文件路径。</param>
      /// <param name="writedContext">需要写入文件的文本。</param>
      /// <param name="isAppend">一个Boolean类型参数，用于决定该操作应该以追加还是覆盖的方式存储文本，如果该参数为true，则该操作会以追加方式存储文本，否则会以覆盖方式存储文本。</param>
      /// <param name="encoding">存储文本的编码格式。</param>
      /// <exception cref="FileNotFoundException">如果fileUrl参数指定的文件不存在时，则会引发这个异常。</exception>
      public static void WriteFile(string fileUrl, string writedContext, bool isAppend, Encoding encoding)
      {
         if (!FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到！", fileUrl);
         StreamWriter fWriter = new StreamWriter(fileUrl, isAppend, encoding);
         fWriter.Write(writedContext);
         fWriter.Close();
      }
      /// <summary>
      /// 删除路径所指定的文件。
      /// </summary>
      /// <param name="fileUrl">需要被删除的文件。</param>
      /// <exception cref="FileNotFoundException">如果fileUrl参数指定的文件不存在时，则会引发这个异常。</exception>
      /// <remarks>需要注意的是，这个操作在Windows中属于不可逆操作，如果这个操作已经正常执行，则这个文件将无法被恢复，因此建议开发者在使用此操作时，应该在UI级别上询问用户是否执行这个操作。</remarks>
      public static void DeleteFile(string fileUrl)
      {
         if (!FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到！", fileUrl);
         File.Delete(fileUrl);
      }
      /// <summary>
      /// 重命名指定的文件的文件名称。
      /// </summary>
      /// <param name="fileUrl">需要被重命名的文件。</param>
      /// <param name="newFileName">新的文件名，这个文件名不包含文件的父目录和文件扩展名。</param>
      public static void RenameFile(string fileUrl, string newFileName)
      {
         RenameFile(fileUrl, newFileName + GetFileExtension(fileUrl));
      }
      /// <summary>
      /// 重命名指定的文件的文件名称，同时也包括文件扩展名。
      /// </summary>
      /// <param name="fileUrl">需要被重命名的文件。</param>
      /// <param name="newFileName">新的文件名，这个文件名不包含文件的父目录和文件扩展名。</param>
      /// <param name="extensionName">新的文件扩展名，但是要记得在扩展名前面加上分割点，比如说：“.exe”，“.docx”，“.document-ci”。</param>
      public static void RenameFile(string fileUrl, string newFileName, string extensionName)
      {
         FileSystem.RenameFile(fileUrl, newFileName + extensionName);
      }
      /// <summary>
      /// 复制文件到指定的地址。
      /// </summary>
      /// <param name="sourceFileUrl">需要复制的文件的文件地址。</param>
      /// <param name="targetFileUrl">复制的目标地址，而非目标目录，允许文件名称不同（即允许重命名）。</param>
      /// <param name="isOverwrite">如果目标地址所表示的文件在复制之前就存在，则这个值指示是否允许覆盖这个文件。</param>
      /// <exception cref="UnsupportedFileSizeException">当源文件的文件大小超出复制目标所在驱动器所支持的最大文件大小，则将会抛出这个异常。</exception>
      public static void CopyFile(string sourceFileUrl, string targetFileUrl, bool isOverwrite)
      {
         if (GetFileSize(sourceFileUrl) > GetSupportedMaximalFileSize(GetDriveLetterAccessUrl(targetFileUrl)))
         {
            throw new UnsupportedFileSizeException();
         }
         FileSystem.CopyFile(sourceFileUrl, targetFileUrl, isOverwrite);
      }
      /// <summary>
      /// 移动文件到指定的地址。
      /// </summary>
      /// <param name="sourceFileUrl">需要移动的文件的文件地址。</param>
      /// <param name="targetFileUrl">移动的目标地址，而非目标目录，允许文件名称不同（即允许重命名）。</param>
      public static void MoveFile(string sourceFileUrl, string targetFileUrl) => MoveFile(sourceFileUrl, targetFileUrl, false);
      /// <summary>
      /// 移动文件到指定的地址，并决定是否覆盖目标同名文件。
      /// </summary>
      /// <param name="sourceFileUrl">需要移动的文件的文件地址。</param>
      /// <param name="targetFileUrl">移动的目标地址，而非目标目录，允许文件名称不同（即允许重命名）。</param>
      /// <param name="isOverwrite">如果目标地址所表示的文件在移动之前就存在，则这个值指示是否允许覆盖这个文件。</param>
      /// <exception cref="DirectoryIsExistedException">当目录已经存在时，则将会抛出这个异常。</exception>
      /// <exception cref="UnsupportedFileSizeException">当源文件的文件大小超出复制目标所在驱动器所支持的最大文件大小，则将会抛出这个异常。</exception>
      public static void MoveFile(string sourceFileUrl, string targetFileUrl, bool isOverwrite)
      {
         if (FileExists(targetFileUrl))
         {
            if (isOverwrite) DeleteFile(targetFileUrl);
            else throw new DirectoryIsExistedException();
         }
         if (GetFileSize(sourceFileUrl) > GetSupportedMaximalFileSize(GetDriveLetterAccessUrl(targetFileUrl)))
         {
            throw new UnsupportedFileSizeException();
         }
         File.Move(sourceFileUrl, targetFileUrl);
      }
      /// <summary>
      /// 删除指定目录下的所有文件。
      /// </summary>
      /// <param name="directory">指定的目录。</param>
      public static void DeleteFiles(string directory)
      {
         DirectoryInfo dInfo = new DirectoryInfo(directory);
         FileSystemInfo[] fsInfo = dInfo.GetFileSystemInfos();
         for (int i = 0; i < fsInfo.Length; i++) DeleteFile(directory + @"\" + fsInfo[i].ToString());
      }
      /// <summary>
      /// 复制占用空间比较大的文件。
      /// </summary>
      /// <param name="sourceFileUrl">需要被复制的文件的文件地址。</param>
      /// <param name="targetFileUrl">复制操作的目标地址。</param>
      /// <param name="transmissionSize">每一次传输的大小，通常这个值设定为1024即可。</param>
      /// <exception cref="UnsupportedFileSizeException">当源文件的文件大小超出复制目标所在驱动器所支持的最大文件大小，则将会抛出这个异常。</exception>
      public static void CopyBigFile(string sourceFileUrl, string targetFileUrl, int transmissionSize)
      {
         if (GetFileSize(sourceFileUrl) > GetSupportedMaximalFileSize(GetDriveLetterAccessUrl(targetFileUrl)))
         {
            throw new UnsupportedFileSizeException();
         }
         FileStream fileToCreate = new FileStream(targetFileUrl, FileMode.Create);
         FileStream fOpen = new FileStream(sourceFileUrl, FileMode.Open, FileAccess.Read);
         FileStream tOpen = new FileStream(targetFileUrl, FileMode.Append, FileAccess.Write);
         int fileSize;
         fileToCreate.Close();
         fileToCreate.Dispose();
         if (transmissionSize < fOpen.Length)
         {
            byte[] buffer = new byte[transmissionSize];
            int copied = 0;
            while (copied <= ((int)fOpen.Length - transmissionSize))
            {
               fileSize = fOpen.Read(buffer, 0, transmissionSize);
               fOpen.Flush();
               tOpen.Write(buffer, 0, transmissionSize);
               tOpen.Flush();
               tOpen.Position = fOpen.Position;
               copied += fileSize;
            }
            int left = (int)fOpen.Length - copied;
            fileSize = fOpen.Read(buffer, 0, left);
            fOpen.Flush();
            tOpen.Write(buffer, 0, left);
            tOpen.Flush();
         }
         else
         {
            byte[] buffer = new byte[fOpen.Length];
            fOpen.Read(buffer, 0, (int)fOpen.Length);
            fOpen.Flush();
            tOpen.Write(buffer, 0, (int)fOpen.Length);
            tOpen.Flush();
         }
         fOpen.Close();
         tOpen.Close();
      }
      /// <summary>
      /// 移动占用空间比较大的文件。
      /// </summary>
      /// <param name="sourceFileUrl">需要被移动的文件的文件地址。</param>
      /// <param name="targetFileUrl">移动操作的目标地址。</param>
      /// <param name="transmissionSize">每一次传输的大小，通常这个值设定为1024即可。</param>
      /// <param name="isCheckFileIntegrity">移动文件之后是否执行完整性检验。</param>
      /// <exception cref="AbortedCheckOperationException">当文件完整性检查失败或者结果不符合要求时，则会抛出这个异常。</exception>
      public static void MoveBigFile(string sourceFileUrl, string targetFileUrl, int transmissionSize, bool isCheckFileIntegrity)
      {
         FileSignature sourceSignature = new FileSignature(sourceFileUrl);
         FileSignature targetSignature = new FileSignature(targetFileUrl);
         ExString srcMd5Value = string.Empty;
         ExString targetMd5Value = string.Empty;
         CopyBigFile(sourceFileUrl, targetFileUrl, transmissionSize);
         if (isCheckFileIntegrity)
         {
            srcMd5Value = sourceSignature.GetMD5String();
            targetMd5Value = targetSignature.GetMD5String();
            if (!srcMd5Value.Equals(targetMd5Value)) throw new AbortedCheckOperationException();
         }
         DeleteFile(sourceFileUrl);
      }
      /// <summary>
      /// 创建一个新的目录。
      /// </summary>
      /// <param name="directory">需要被创建的目录。</param>
      /// <exception cref="DirectoryNotFoundException">当参数directory所指定的目录已存在时，则会引发该异常。</exception>
      public static void CreateDirectory(string directory)
      {
         if (DirectoryExists(directory)) throw new DirectoryIsExistedException("指定的目录已经存在！", directory);
         Directory.CreateDirectory(directory);
      }
      /// <summary>
      /// 指示指定的目录是否存在。
      /// </summary>
      /// <param name="directory">需要被用于判定的目录。</param>
      /// <returns>如果参数directory所指定的目录已经存在则会返回true，否则将会返回false。</returns>
      public static bool DirectoryExists(string directory) => Directory.Exists(directory);
      /// <summary>
      /// 删除一个指定的空目录。
      /// </summary>
      /// <param name="directory">需要被删除的空目录。</param>
      /// <exception cref="IsNotEmptyDirectoryException">当参数directory所指定的目录不是空目录时，则会引发这个异常。</exception>
      /// <exception cref="DirectoryNotFoundException">当参数directory所指定的目录找不到时，则会引发这个异常。</exception>
      /// <remarks>在这里，空目录指的是其目录下不存在其他的文件夹（子目录）或者文件，以及两者的组合。</remarks>
      public static void DeleteEmptyDirectory(string directory)
      {
         if (Directory.GetDirectories(directory).GetLength(0) != 0) throw new IsNotEmptyDirectoryException();
         if (!DirectoryExists(directory)) throw new DirectoryNotFoundException("指定的目录不存在！");
         Directory.Delete(directory);
      }
      /// <summary>
      /// 删除一个指定目录，并包含这个目录下的所有子目录和文件。
      /// </summary>
      /// <param name="directory">需要被删除的目录。</param>
      public static void DeleteDirectory(string directory)
      {
         if (DirectoryExists(directory))
         {
            foreach (string @item in Directory.GetFileSystemEntries(directory))
            {
               if (FileExists(@item)) DeleteFile(@item);
               else DeleteDirectory(@item);
            }
            DeleteEmptyDirectory(directory);
         }
      }
      /// <summary>
      /// 复制一个指定的目录（文件夹）到另一个目录下。
      /// </summary>
      /// <param name="sourceDirectory">需要被复制的目录，其中允许包含子目录和文件。</param>
      /// <param name="targetDirectory">存储的目标区域。</param>
      /// <param name="isOverwrite">如果目标目录已存在，是否覆盖。</param>
      /// <exception cref="DirectoryNotFoundException">当需要被复制的源目录不存在时，则会抛出这个异常。</exception>
      /// <exception cref="DirectoryIsExistedException">当参数isOverwrite为false并且目标目录已存在时，则将会抛出这个异常。</exception>
      public static void CopyDirectory(string sourceDirectory, string targetDirectory, bool isOverwrite)
      {
         if (!DirectoryExists(sourceDirectory)) throw new DirectoryNotFoundException("找不到源目录！");
         if (!isOverwrite && DirectoryExists(targetDirectory)) throw new DirectoryIsExistedException();
         string[] sourceFilesPath = Directory.GetFileSystemEntries(sourceDirectory);
         for (int i = 0; i < sourceFilesPath.Length; i++)
         {
            string sourceFilePath = sourceFilesPath[i];
            string directoryName = Path.GetDirectoryName(sourceFilePath);
            string[] forlders = directoryName.Split('\\');
            string lastDirectory = forlders[forlders.Length - 1];
            string dest = Path.Combine(targetDirectory, lastDirectory);
            if (FileExists(sourceFilePath))
            {
               string sourceFileName = Path.GetFileName(sourceFilePath);
               if (!DirectoryExists(dest)) CreateDirectory(dest);
               CopyFile(sourceFilePath, Path.Combine(dest, sourceFileName), isOverwrite);
            }
            else CopyDirectory(sourceFilePath, dest, isOverwrite);
         }
      }
      /// <summary>
      /// 返回不具有扩展名的指定路径字符串的文件名。
      /// </summary>
      /// <param name="fileUrl">需要被获取文件名的文件地址。</param>
      /// <returns>由System.IO.Path.GetFileName返回的字符串，但不包括最后的句点以及之后的所有字符。</returns>
      /// <remarks>注意，这个方法不会验证文件的存在性和其他相关有效性</remarks>
      public static string GetFileNameWithoutExtension(string fileUrl) => Path.GetFileNameWithoutExtension(fileUrl);
      /// <summary>
      /// 获取一个包含后缀名的文件名。
      /// </summary>
      /// <param name="fileUrl">需要被获取文件名的文件的本地地址。</param>
      /// <returns>如果不抛出任何异常，则该操作会返回一个不包含目录的文件名。</returns>
      public static string GetFileName(string fileUrl) => GetFileNameWithoutExtension(fileUrl) + GetFileExtension(fileUrl);
      /// <summary>
      /// 获取一个文件的后缀名。
      /// </summary>
      /// <param name="fileUrl">用于被获取后缀名的文件地址。</param>
      /// <returns>如果没有异常发生，则将返回这个文件的后缀名，假设一个文件地址为C:\Windows\System32\cmd.exe，则这个操作返回的文件后缀名为.exe，而非exe。</returns>
      public static string GetFileExtension(string fileUrl) => new FileInfo(fileUrl).Extension;
      /// <summary>
      /// 获取一个文件的大小，以字节（Byte）为单位。
      /// </summary>
      /// <param name="fileUrl">需要被获取文件大小的文件。</param>
      /// <returns>该操作会返回一个长整形数据long（Int64），用于存放参数fileUrl所对应文件的文件大小。</returns>
      public static long GetFileSize(string fileUrl) => new FileInfo(fileUrl).Length;
      /// <summary>
      /// 获取指定目录下面的所有文件名（包含路径和扩展名）。
      /// </summary>
      /// <param name="directory">指定的目录。</param>
      /// <returns>如果不产生异常，该操作则会返回一个包含文件名集合的列表，如果该目录下没有任何文件则返回一个空列表。</returns>
      /// <exception cref="DirectoryNotFoundException">当参数directory所指定的目录找不到时，则会引发这个异常。</exception>
      public static List<string> GetFiles(string directory)
      {
         if (!DirectoryExists(directory)) throw new DirectoryNotFoundException("指定的目录找不到");
         string[] arrList = Directory.GetFiles(directory);
         return arrList.ToList();
      }
      /// <summary>
      /// 获取指定目录下符合类型筛选的所有文件名（包含路径和扩展名）。
      /// </summary>
      /// <param name="directory">指定的目录。</param>
      /// <param name="extensionList">指定的类型集合，比如说：new string[] { ".doc", ".docx" }。</param>
      /// <returns>如果不产生异常，该操作则会返回一个包含符合类型筛选的文件名集合的列表，如果该目录下没有任何文件则返回一个空列表。</returns>
      public static List<string> GetFiles(string directory, string[] extensionList)
      {
         List<string> fList = new List<string>();
         List<string> tempFileList = GetFiles(directory);
         Parallel.ForEach(tempFileList, (file) =>
         {
            Parallel.ForEach(extensionList, (extension) =>
            {
               if (GetFileExtension(file) == extension) fList.Add(file);
            });
         });
         return fList;
      }
      /// <summary>
      /// 遍历指定目录下的文件。
      /// </summary>
      /// <param name="directory">需要被遍历的目录。</param>
      /// <param name="isTraverseDirectory">指示是否将遍历的文件夹路径也存储到返回值中。</param>
      /// <returns>该操作会返回一个可能包含文件（或者也包含文件夹）路径集合的列表，这个列表保存了当前目录下的所有文件（和文件夹）路径。</returns>
      public static List<string> TraverseWith(string directory, bool isTraverseDirectory)
      {
         List<string> result = new List<string>();
         DirectoryInfo dir = new DirectoryInfo(directory);
         DirectoryInfo[] dirSub = dir.GetDirectories();
         if (dirSub.Length <= 0)
         {
            Parallel.ForEach(dir.GetFiles("*.*", System.IO.SearchOption.TopDirectoryOnly), (fInfo) =>
            {
               result.Add(dir + @"\" + fInfo.ToString());
            });
         }
         int t = 1;
         Parallel.ForEach(dirSub, (dInfo) =>
         {
            TraverseWith(dir + @"\" + dInfo.ToString(), isTraverseDirectory);
            if (isTraverseDirectory) result.Add(dir + @"\" + dInfo.ToString());
            if (t == 1)
            {
               Parallel.ForEach(dir.GetFiles("*.*", System.IO.SearchOption.TopDirectoryOnly), (fInfo) =>
               {
                  result.Add(dir + @"\" + fInfo.ToString());
               });
               t = t + 1;
            }
         });
         return result;
      }
      /// <summary>
      /// 在指定的目录中搜寻完全一样的文件。
      /// </summary>
      /// <param name="fileUrl">需要进行匹配的样本文件的文件地址。</param>
      /// <param name="directory">需要进行搜寻的目录，即匹配的范围。</param>
      /// <param name="isSorted">是否将匹配结果进行排序之后并作为返回值提交给用户。</param>
      /// <returns>如果搜寻到完全一样的文件，则会返回这些文件地址所构成的List实例。</returns>
      /// <remarks>该方法的原理是基于哈希验证的，但为了提高函数的效率，因此采用了文件名称对比、文件大小对比和文件SHA1代码对比来实现更加高效的运行，因为在哈希代码上完全相同的文件，其文件名称（不包含目录）和文件大小是完全相同的，但是当需要进行匹配的文件大小过大（比如说一个Windows离线安装镜像），则该操作将会非常的消耗时间，因此为了避免这种情况导致的应用程序性能下降，建议使用异步操作模式保证其他诸如UI线程的正常工作。</remarks>
      public static List<string> SearchIdenticalFiles(string fileUrl, string directory, bool isSorted)
      {
         List<string> searchResult = new List<string>();
         List<string> traversedFiles = TraverseWith(directory, false);
         const int CRITICAL_FILE_SIZE = 50000000;
         Parallel.For(0, traversedFiles.Count, (index, interrupt) =>
         {
            try
            {
               if (GetFileName(traversedFiles[index]) == GetFileName(fileUrl))
               {
                  long sourceSize = GetFileSize(fileUrl);
                  long targetSize = GetFileSize(traversedFiles[index]);
                  if (sourceSize == targetSize)
                  {
                     string srcHashCode = string.Empty;
                     string tgtHashCode = srcHashCode;
                     bool fileSizeCondition = sourceSize >= CRITICAL_FILE_SIZE || targetSize >= CRITICAL_FILE_SIZE;
                     Action getSrcHash = () => srcHashCode = new FileSignature(fileUrl).GetSHA1String();
                     Action getTgtHash = () => tgtHashCode = new FileSignature(traversedFiles[index]).GetSHA1String();
                     if (fileSizeCondition) Parallel.Invoke(new Action[] { getSrcHash, getTgtHash });
                     else
                     {
                        getSrcHash.Invoke();
                        getTgtHash.Invoke();
                     }
                     if (srcHashCode.Equals(tgtHashCode)) searchResult.Add(traversedFiles[index]);
                  }
               }
            }
            catch (Exception throwedException)
            {
               if (throwedException != null) throw throwedException.InnerException;
               interrupt.Stop();
            }
         });
         if (isSorted) searchResult.Sort();
         return searchResult;
      }
      /// <summary>
      /// 获取指定目录下的所有子目录的名称。
      /// </summary>
      /// <param name="directory">指定的目录。</param>
      /// <returns>如果不产生异常，则该操作会返回一个包含子目录集合的列表，如果该目录下没有任何子目录，则将会返回一个空列表。</returns>
      /// <exception cref="DirectoryNotFoundException">当参数directory所指定的目录找不到时，则会引发这个异常。</exception>
      public static List<string> GetSubDirectories(string directory)
      {
         if (DirectoryExists(directory) == false) throw new DirectoryNotFoundException("指定的目录找不到");
         return Directory.GetDirectories(directory).ToList();
      }
      /// <summary>
      /// 获取指定文件夹的名称，但不会返回完整名称。
      /// </summary>
      /// <param name="directory">指定的目录。</param>
      /// <returns>操作成功之后将会返回一个文件夹名称字符串。</returns>
      public static string GetFolderName(string directory) => new DirectoryInfo(directory).Name;
      /// <summary>
      /// 获取指定文件所在的目录
      /// </summary>
      /// <param name="fileUrl">需要被获取目录的文件地址。</param>
      /// <returns>这个操作会返回一个字符串，该字符串存储了这个操作所获取的目录，如果操作失败可能会引发相关的异常。</returns>
      public static string GetFatherDirectory(string fileUrl) => Directory.GetParent(fileUrl).FullName;
      /// <summary>
      /// 获取指定目录的大小。
      /// </summary>
      /// <param name="directory">需要获取大小的目录。</param>
      /// <returns>如果这个操作没有任何异常，则会返回一个存储了目录尺寸的Int64实例。</returns>
      /// <exception cref="DirectoryNotFoundException">当指定的目录不存在时，则将会抛出这个异常。</exception>
      public static long GetDirectorySize(string directory)
      {
         if (!DirectoryExists(directory)) throw new DirectoryNotFoundException("指定的目录不存在！");
         long directorySize = 0;
         DirectoryInfo dInfo = new DirectoryInfo(directory);
         foreach (FileInfo fInfo in dInfo.GetFiles()) directorySize += fInfo.Length;
         DirectoryInfo[] dInfos = dInfo.GetDirectories();
         if (dInfos.Length > 0)
         {
            for (int i = 0; i < dInfos.Length; i++)
            {
               directorySize += GetDirectorySize(dInfos[i].FullName);
            }
         }
         return directorySize;
      }
      /// <summary>
      /// 显示指定文件的文件属性对话框。
      /// </summary>
      /// <param name="fileUrl">指定的文件，如果这个文件不存在，则将会抛出异常。</param>
      public static void ShowFileAttributesDialog(string fileUrl)
      {
         if (!FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到", fileUrl);
         else
         {
            SShellExecuteInfo info = new SShellExecuteInfo();
            info.SizeOfStructure = Marshal.SizeOf(info);
            info.Verb = "properties";
            info.FileUrl = fileUrl;
            info.ShowFlags = SW_SHOW;
            info.Flags = SEE_MASK_INVOKEIDLIST;
            ShellExecuteEx(ref info);
         }
      }
      /// <summary>
      /// 获取指定驱动器能支持的最大文件大小。
      /// </summary>
      /// <param name="driveLetter">指定驱动器所对应的盘符。</param>
      /// <returns>该操作会返回一个64位无符号整型数据，这个数据表示了参数driveLetter所对应的驱动器能够支持的最大文件大小。</returns>
      [CLSCompliant(false)]
      public static long GetSupportedMaximalFileSize(string driveLetter) => GetSupportedMaximalFileSize(GetDriveFormat(driveLetter));
      /// <summary>
      /// 获取不同文件系统下支持的最大文件大小。
      /// </summary>
      /// <param name="driveFormat">指定的文件系统。</param>
      /// <returns>该操作会返回一个64位无符号整型数据，这个数据表示了参数driveFormat表示的文件系统所支持的最大文件大小。</returns>
      [CLSCompliant(false)]
      public static long GetSupportedMaximalFileSize(EDriveFormat driveFormat)
      {
         long maximalFileSize = 0;
         switch (driveFormat)
         {
#if UNIX_LINUX_MAC
            case EDriveFormat.LinuxExt:
               maximalFileSize = 2147483648;
               break;
            case EDriveFormat.LinuxExt2:
               maximalFileSize = 2199023255552;
               break;
            case EDriveFormat.LinuxExt3:
            case EDriveFormat.LinuxExt4:
            case EDriveFormat.LinuxJFS:
               maximalFileSize = 17592186044416;
               break;
#endif
#if WINDOWS
            case EDriveFormat.Win32NTFS:
               maximalFileSize = 68719476736;
               break;
            case EDriveFormat.Win32FAT12:
               maximalFileSize = 8388608;
               break;
            case EDriveFormat.Win32FAT16:
               maximalFileSize = 2147483648;
               break;
            case EDriveFormat.Win32FAT32:
               maximalFileSize = 4294967296;
               break;
            case EDriveFormat.Win32NTFSv5:
            default:
               maximalFileSize = 2199023255552;
               break;
#endif
         }
         return maximalFileSize;
      }
      /// <summary>
      /// 通过指定的本地文件路径或者目录获取盘符。
      /// </summary>
      /// <param name="fileUrlOrDirectory">指定的本地文件路径或者目录。</param>
      /// <returns>该操作将会返回一个有效的盘符，比如说“C:”。</returns>
      public static string GetDriveLetterAccessUrl(string fileUrlOrDirectory)
      {
         bool isAbsoluteUri = new Uri(fileUrlOrDirectory).IsAbsoluteUri;
         if (!isAbsoluteUri) throw new ArgumentException("参数无效或者文件地址格式错误！", "fileUrlOrDirectory");
         return fileUrlOrDirectory.Substring(0, 2);
      }
#if WINDOWS
      /// <summary>
      /// 获取指定分区（或者驱动器）的驱动器格式或文件系统。
      /// </summary>
      /// <param name="driveLetter">指定驱动器所对应的盘符。</param>
      /// <returns>该操作将会返回指定驱动器的文件系统，在Windows环境下，这个操作将不会获取UNIX以及衍生操作系统所支持的文件系统。</returns>
      public static EDriveFormat GetDriveFormat(string driveLetter)
      {
         EDriveFormat format = EDriveFormat.Win32NTFS;
         DriveInfo dInfo = new DriveInfo(driveLetter);
         string formatStr = dInfo.DriveFormat;
         switch (formatStr)
         {
            case DRIVE_FORMAT_FAT12:
               format = EDriveFormat.Win32FAT12;
               break;
            case DRIVE_FORMAT_FAT16:
               format = EDriveFormat.Win32FAT16;
               break;
            case DRIVE_FORMAT_FAT32:
               format = EDriveFormat.Win32FAT32;
               break;
            case DRIVE_FORMAT_NTFS:
               format = EDriveFormat.Win32NTFS;
               break;
            default:
               throw new UnsupportedDriveFormatException();
         }
         return format;
      }
#endif
   }
   public enum EDriveFormat : int
   {
#if WINDOWS
      /// <summary>
      /// Windows NT文件系统，现在Windows上运用最多的文件系统。
      /// </summary>
      [EnumerationDescription("NTFS")]
      Win32NTFS = 0x0000,
      /// <summary>
      /// 早期版本的FAT文件系统。
      /// </summary>
      [EnumerationDescription("FAT12")]
      Win32FAT12 = 0x0001,
      /// <summary>
      /// 基于FAT12改进的FAT16文件系统。
      /// </summary>
      [EnumerationDescription("FAT16")]
      Win32FAT16 = 0x0002,
      /// <summary>
      /// FAT32文件系统，现常用基于USB的Windows Preinstallation Environment启动介质的文件系统。
      /// </summary>
      [EnumerationDescription("FAT32")]
      Win32FAT32 = 0x0003,
      /// <summary>
      /// 可恢复的Windows NT文件系统。
      /// </summary>
      [EnumerationDescription("NTFS 5.0")]
      Win32NTFSv5 = 0x0004,
#endif
#if UNIX_LINUX_MAC
      /// <summary>
      /// 延伸文件系统，是Linux上常用的文件系统。
      /// </summary>
      [EnumerationDescription("ext")]
      LinuxExt = 0x1000,
      /// <summary>
      /// 改进后的第二代延伸文件系统。
      /// </summary>
      [EnumerationDescription("ext2")]
      LinuxExt2 = 0x1001,
      /// <summary>
      /// 第三代延伸文件系统。
      /// </summary>
      [EnumerationDescription("ext3")]
      LinuxExt3 = 0x1002,
      /// <summary>
      /// 第四代延伸文件系统。
      /// </summary>
      [EnumerationDescription("ext4")]
      LinuxExt4 = 0x1003,
      /// <summary>
      /// 字节级日志文件系统。
      /// </summary>
      [EnumerationDescription("JFS")]
      LinuxJFS = 0x1004
#endif
   }
   /// <summary>
   /// 在文件已经存在时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class FileIsExistedException : Exception
   {
      public FileIsExistedException() : base("指定的文件已存在！") { }
      public FileIsExistedException(string message) : base(message) { }
      public FileIsExistedException(Uri fileUri) : base("文件" + fileUri.LocalPath + "已经存在！") { }
      public FileIsExistedException(string message, Exception inner) : base(message, inner) { }
      protected FileIsExistedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 在目录已经存在时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class DirectoryIsExistedException : Exception
   {
      public DirectoryIsExistedException() : base("指定的目录已存在！") { }
      public DirectoryIsExistedException(string message) : base(message) { }
      public DirectoryIsExistedException(string message, string directory) : base(message + "\n参数：" + directory) { }
      public DirectoryIsExistedException(string message, Exception inner) : base(message, inner) { }
      protected DirectoryIsExistedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当目录下存在其他文件或者文件夹时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class IsNotEmptyDirectoryException : Exception
   {
      public IsNotEmptyDirectoryException() : base("指定的目录下仍存在其他文件夹或者文件！") { }
      public IsNotEmptyDirectoryException(string message) : base(message) { }
      public IsNotEmptyDirectoryException(string message, Exception inner) : base(message, inner) { }
      protected IsNotEmptyDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当检查动作发生意外或者失败时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class AbortedCheckOperationException : Exception
   {
      public AbortedCheckOperationException() : base("检查动作执行失败！") { }
      public AbortedCheckOperationException(string message) : base(message) { }
      public AbortedCheckOperationException(string message, Exception inner) : base(message, inner) { }
      protected AbortedCheckOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当遇到不支持的文件系统或者驱动器格式时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class UnsupportedDriveFormatException : Exception
   {
      public UnsupportedDriveFormatException() : base("不支持的文件系统或者驱动器格式！") { }
      public UnsupportedDriveFormatException(string message) : base(message) { }
      public UnsupportedDriveFormatException(string message, Exception inner) : base(message, inner) { }
      protected UnsupportedDriveFormatException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当遇到驱动器不支持的文件大小时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class UnsupportedFileSizeException : Exception
   {
      public UnsupportedFileSizeException() : base("不支持的文件大小！") { }
      public UnsupportedFileSizeException(string message) : base(message) { }
      public UnsupportedFileSizeException(string message, Exception inner) : base(message, inner) { }
      protected UnsupportedFileSizeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
