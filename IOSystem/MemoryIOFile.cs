using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem
{
   /// <summary>
   /// 内存IO文件，使用该类可以最大限度的提高文件IO的性能。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   [DebuggerDisplay("MemoryIOFile = FileUrl:{FileUrl};StreamLength:{StreamLength}")]
   public class MemoryIOFile : IExampledObjectFileBaseIO, IEquatable<MemoryIOFile>, IDisposable
   {
      private string _fileUrl;//指定的文件地址。
      private string _fileContext;//可用于读取和修改的文件内容。
      private MemoryStream _memoryStream;//内存流。
      private bool _disposedValue = false;//检测冗余调用。
      private const int DEFAULT_MEMORY_ALLOC_SIZE = 64;//内存流的默认空间初始分配长度。
      /// <summary>
      /// 构造函数，创建一个指定文件地址的MemoryIOFile实例。
      /// </summary>
      /// <param name="fileUrl">指定的文件地址。</param>
      /// <exception cref="FileNotFoundException">当指定的文件找不到时，则将会抛出这个异常。</exception>
      public MemoryIOFile(string fileUrl)
      {
         if (!FileOperator.FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到！", fileUrl);
         else
         {
            _fileUrl = fileUrl;
            _memoryStream = new MemoryStream(DEFAULT_MEMORY_ALLOC_SIZE);
         }
      }
      /// <summary>
      /// 构造函数，创建一个指定文件地址的MemoryIOFile实例，并根据参数createdThenNotExists决定当文件不存在时是否创建新文件。
      /// </summary>
      /// <param name="fileUrl">指定的文件地址。</param>
      /// <param name="createdThenNotExists">用于决定是否在检测到文件不存在时来创建新文件。</param>
      /// <exception cref="FileNotFoundException">当指定的文件找不到时，则将会抛出这个异常。</exception>
      public MemoryIOFile(string fileUrl, bool createdThenNotExists)
      {
         bool isThrowedExceotion = false;
         if (createdThenNotExists)
         {
            if (!FileOperator.FileExists(fileUrl)) FileOperator.CreateFile(fileUrl);
            isThrowedExceotion = false;
         }
         else
         {
            if (!FileOperator.FileExists(fileUrl))
            {
               isThrowedExceotion = true;
               throw new FileNotFoundException("指定的文件找不到！", fileUrl);
            }
         }
         if (!isThrowedExceotion)
         {
            _fileUrl = fileUrl;
            _memoryStream = new MemoryStream();
         }
      }
      /// <summary>
      /// 获取或设置当前实例的文件地址。
      /// </summary>
      /// <exception cref="FileNotFoundException">当指定的文件找不到时，则将会抛出这个异常。</exception>
      public string FileUrl
      {
         get => _fileUrl;
         set
         {
            if (!FileOperator.FileExists(value)) throw new FileNotFoundException("指定的文件找不到！", value);
            else _fileUrl = value;
         }
      }
      /// <summary>
      /// 获取或设置（set代码不可见）当前实例的文件内容上下文。
      /// </summary>
      public string FileContext { get => _fileContext; private set => _fileContext = value; }
      /// <summary>
      /// 获取当前实例的内存流长度。
      /// </summary>
      public long StreamLength => _memoryStream.Length;
      /// <summary>
      /// 获取当前实例所包含的内存流实例。
      /// </summary>
      public MemoryStream Stream => _memoryStream;
      /// <summary>
      /// 将当前文件的文件内容写入到内存流。
      /// </summary>
      /// <remarks>在执行这个方法之前，您首先需要读取文件的内容上下文，换句话说，执行该方法之前，需要先执行Read方法（或者其重载版本）。</remarks>
      public void WriteToStream()
      {
         byte[] buffer = new UnicodeEncoding().GetBytes(FileContext);
         _memoryStream.Write(buffer, 0, buffer.Length);
      }
      /// <summary>
      /// 将当前内存流的内容全部取出。
      /// </summary>
      /// <returns>该操作会返回一个完整的内存流数据。</returns>
      /// <remarks>如果您在访问这个方法之前未执行WriteToStream方法，则可能会引发一系列的异常，这是因为当前类在初始化的时候并没有向Stream中写入任何有效的数据。</remarks>
      public byte[] ReadFromStream()
      {
         byte[] buffer = new byte[StreamLength];
         _memoryStream.Read(buffer, 0, buffer.Length);
         return buffer;
      }
      /// <summary>
      /// 将字节流以GB2312的编码格式更新到文件内容上下文。
      /// </summary>
      /// <param name="buffer">需要更新的字节流，这个字节流可以是来自ReadFromStream方法的返回结果。</param>
      public void UpdateToContext(byte[] buffer) => UpdateToContext(buffer, "GB2312");
      /// <summary>
      /// 将字节流以指定的编码格式更新到文件内容上下文。
      /// </summary>
      /// <param name="buffer">需要更新的字节流，这个字节流可以是来自ReadFromStream方法的返回结果。</param>
      /// <param name="encoding">指定的编码格式。</param>
      public void UpdateToContext(byte[] buffer, Encoding encoding) => FileContext = encoding.GetString(buffer);
      /// <summary>
      /// 将字节流以指定编码格式名称所对应的编码格式更新到文件内容上下文。
      /// </summary>
      /// <param name="buffer">需要更新的字节流，这个字节流可以是来自ReadFromStream方法的返回结果。</param>
      /// <param name="encodingName">指定编码格式所对应的编码格式名称。</param>
      public void UpdateToContext(byte[] buffer, string encodingName) => FileContext = Encoding.GetEncoding(encodingName).GetString(buffer);
      /// <summary>
      /// 按照默认的方式读取文件内容。
      /// </summary>
      public void Read() => Read(Encoding.GetEncoding("GB2312"));
      /// <summary>
      /// 通过指定的编码方式来读取文件内容。
      /// </summary>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件读取的编码方式。</param>
      public void Read(Encoding encoding) => FileContext = FileOperator.ReadFileContext(FileUrl, true, encoding);
      /// <summary>
      /// 向文件中存取指定的需要存取的文件内容。
      /// </summary>
      public void Write() => Write(false);
      /// <summary>
      /// 通过指定的文件内容存取方式来存取文件内容。
      /// </summary>
      /// <param name="isAppend">用于决定文件内容的存取方式，如果这个参数值为true，则意味着该操作将会以追加的方式把文本内容追加到文件末尾，反之将会以覆盖原本内容的方式存取文件。</param>
      public void Write(bool isAppend) => Write(isAppend, Encoding.GetEncoding("GB2312"));
      /// <summary>
      /// 通过指定的文件编码方式来存取文件内容。
      /// </summary>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件存取的编码方式。</param>
      public void Write(Encoding encoding) => Write(false, encoding);
      /// <summary>
      /// 通过指定的文件内容存取方式和编码方式来存取文件内容。
      /// </summary>
      /// <param name="isAppend">用于决定文件内容的存取方式，如果这个参数值为true，则意味着该操作将会以追加的方式把文本内容追加到文件末尾，反之将会以覆盖原本内容的方式存取文件。</param>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件存取的编码方式</param>
      public void Write(bool isAppend, Encoding encoding) => FileOperator.WriteFile(FileUrl, FileContext, isAppend, encoding);
      /// <summary>
      /// 确定此实例是否与另一个指定的MemoryIOFile实例是否相同。
      /// </summary>
      /// <param name="other">另外一个MemoryIOFile实例</param>
      /// <returns>如果匹配结果表示相同则返回true，否则返回false。</returns>
      /// <remarks>该方法是基于FileUrl属性，FileContext属性和Stream属性综合判断而执行，若需要其他方式的匹配模式，请尝试重载或者重写这个方法。</remarks>
      public virtual bool Equals(MemoryIOFile other) => FileUrl.Equals(other.FileUrl) && FileContext.Equals(other.FileContext) && Stream.Equals(other.Stream);
      #region IDisposable Support
      /// <summary>
      /// 释放该对象引用的所有内存资源。
      /// </summary>
      /// <param name="disposing">用于指示是否释放托管资源。</param>
      protected virtual void Dispose(bool disposing)
      {
         int urlMaxGene = GC.GetGeneration(FileUrl);
         int contextMaxGene = GC.GetGeneration(FileContext);
         int maxGene = urlMaxGene >= contextMaxGene ? urlMaxGene : contextMaxGene;
         if (!_disposedValue)
         {
            if (disposing)
            {
               Stream.Dispose();
               FileUrl = null;
               FileContext = null;
               bool condition = GC.CollectionCount(urlMaxGene) == 0 && GC.CollectionCount(contextMaxGene) == 0;
               if (condition) GC.Collect(maxGene, GCCollectionMode.Forced, true);
            }
            _disposedValue = true;
         }
      }
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose() => Dispose(true);
      #endregion
   }
}
