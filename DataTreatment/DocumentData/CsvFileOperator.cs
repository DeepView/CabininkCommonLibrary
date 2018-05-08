using System;
using System.IO;
using System.Linq;
using Cabinink.IOSystem;
using System.Collections;
using System.Diagnostics;
using Cabinink.TypeExtend;
using Cabinink.IOSystem.Security;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.DataTreatment.DocumentData
{
   /// <summary>
   /// 逗号分隔符文件操作类，可用于实现一些最基本的CSV文件操作，不过需要注意的是，该类继承自Cabinink.IOSystem.Security.IOSecurityFile类，因此存在与之相同的权限管理操作。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   [DebuggerDisplay("CsvFileOperator = FileUrl:{FileUrl};SecurityFlag:{SecurityFlag}")]
   public class CsvFileOperator : IOSecurityFile, ICollection<string>
   {
      private List<string> _csvDataElements;//用于存放CSV文件元素的泛型列表。
      /// <summary>
      /// 构造函数，创建一个指定文件的CSV文件操作实例，该操作会自动启用写操作通道（即自动执行OpenWritePassageway方法），与之对应的，Dispose方法也会自动执行CloseWritePassageway方法关闭写操作通道。
      /// </summary>
      /// <param name="csvFileUrl">指定的CSV文件的文件地址。</param>
      /// <exception cref="FileTypeNotLegitimateException">如果文件类型不符合匹配条件，则将会抛出这个异常。</exception>
      public CsvFileOperator(string csvFileUrl) : base(csvFileUrl, true)
      {
         if (FileOperator.GetFileExtension(csvFileUrl) != ".csv") throw new FileTypeNotLegitimateException();
         else
         {
            _csvDataElements = new List<string>();
            OpenWritePassageway();
         }
      }
      /// <summary>
      /// 构造函数，创建一个指定文件的CSV文件操作实例，如果指定的CSV文件不存在则将会抛出异常，该操作会自动启用写操作通道（即自动执行OpenWritePassageway方法），与之对应的，Dispose方法也会自动执行CloseWritePassageway方法关闭写操作通道。
      /// </summary>
      /// <param name="csvFileUrl">指定的CSV文件的文件地址。</param>
      /// <param name="createdThenNotExists">用于决定是否在检测到文件不存在时来创建新文件。</param>
      /// <exception cref="FileNotFoundException">当参数fileUrl指定的文件找不到，并且不允许根据参数createdThenNotExists决定当文件不存在时是否创建新文件的情况下，则会抛出这个异常。</exception>
      /// <exception cref="FileTypeNotLegitimateException">如果文件类型不符合匹配条件，则将会抛出这个异常。</exception>
      public CsvFileOperator(string csvFileUrl, bool createdThenNotExists) : base(csvFileUrl, createdThenNotExists)
      {
         if (!createdThenNotExists && !FileOperator.FileExists(csvFileUrl)) throw new FileNotFoundException("指定的文件无法找到！", csvFileUrl);
         if (FileOperator.GetFileExtension(csvFileUrl) != ".csv") throw new FileTypeNotLegitimateException();
         else
         {
            _csvDataElements = new List<string>();
            OpenWritePassageway();
         }
      }
      /// <summary>
      /// 获取当前实例所操作的CSV文件转换为List实例之后的CSV元素数量。
      /// </summary>
      public int Count => Elements.Count;
      /// <summary>
      /// 获取或设置当前实例所操作的CSV文件转换为List实例之后的CSV元素集合。
      /// </summary>
      public List<string> Elements
      {
         get
         {
            char[] separator = new char[] { ',' };
            _csvDataElements = FileContext.Split(separator).ToList();
            return _csvDataElements;
         }
         set => _csvDataElements = value;
      }
      /// <summary>
      /// 获取当前实例所包含的ICollection泛型接口是否为只读。
      /// </summary>
      public bool IsReadOnly => ((ICollection<string>)Elements).IsReadOnly;
      /// <summary>
      /// 获取或设置指定索引所对应的CSV元素。
      /// </summary>
      /// <param name="index">指定的索引，但是这个索引必须在当前实例的列表的有效索引范围之内才有效。</param>
      /// <exception cref="ArgumentOutOfRangeException">当指定索引超出列表的检索范围之后，则会抛出这个异常。</exception>
      public ExString this[int index]
      {
         get
         {
            if ((index < 0) || (index > Count)) throw new ArgumentOutOfRangeException("index", "指定索引所对应的元素不存在！");
            return Elements[index];
         }
         set
         {
            if ((index < 0) || (index > Count)) throw new ArgumentOutOfRangeException("index", "指定索引所对应的元素不存在！");
            Elements[index] = value;
         }
      }
      /// <summary>
      /// 将指定的CSV元素添加到CSV元素列表末尾。
      /// </summary>
      /// <param name="item">需要添加的指定CSV元素。</param>
      public void Add(string item) => Elements.Add(item);
      /// <summary>
      /// 从当前实例的CSV列表中移除所有的元素。
      /// </summary>
      public void Clear() => Elements.Clear();
      /// <summary>
      /// 确定某个CSV元素是否在当前实例的CSV列表之中。
      /// </summary>
      /// <param name="item">用于被确定存在性的CSV元素。</param>
      /// <returns>如果这个元素在当前实例的CSV列表之中存在，该操作将会返回true，否则会返回false。</returns>
      public bool Contains(string item) => Elements.Contains(item);
      /// <summary>
      /// 从目标数组的指定索引处开始，将整个CSV列表复制到兼容的一维数组。
      /// </summary>
      /// <param name="array">一维数组，它是从CSV列表复制的元素的目标，这个一维数组必须具有从零开始的索引。</param>
      /// <param name="arrayIndex">数组中从零开始的索引，从此处开始复制。</param>
      public void CopyTo(string[] array, int arrayIndex) => Elements.CopyTo(array, arrayIndex);
      /// <summary>
      /// 返回循环访问当前实例包含的CSV列表的枚举数。
      /// </summary>
      /// <returns>用于System.Collections.Generic.List&lt;T&gt;.Enumerator的System.Collections.Generic.List&lt;T&gt;。</returns>
      public IEnumerator<string> GetEnumerator() => Elements.GetEnumerator();
      /// <summary>
      /// 从当前实例包含的CSV列表中移除item匹配的第一个元素。
      /// </summary>
      /// <param name="item">需要被匹配并移除的元素。</param>
      /// <returns>如果这个操作成功，则将会返回true，否则返回false。</returns>
      public bool Remove(string item) => Elements.Remove(item);
      /// <summary>
      /// 从当前实例包含的CSV列表中移除参数index指定索引对应的元素。
      /// </summary>
      /// <param name="index">指定的索引。</param>
      public void Remove(int index) => Elements.RemoveAt(index);
      /// <summary>
      /// 返回循环访问集合的枚举数。
      /// </summary>
      /// <returns>一个可用于循环访问集合的System.Collections.IEnumerator对象。</returns>
      IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();
      /// <summary>
      /// 将CSV列表中的所有元素更新到FileContext中以方便保存。
      /// </summary>
      /// <exception cref="CodeSecurityNotMatchException">当代码操作在允许的构造逻辑或者操作安全范围外时，则会抛出这个异常。</exception>
      public void UpdateFileContext()
      {
         if (SecurityFlag == EFileSecurityFlag.FileIsLocked) throw new CodeSecurityNotMatchException();
         else
         {
            string tempCsvString = string.Empty;
            for (int i = 0; i < Count; i++) tempCsvString = tempCsvString + Elements[i] + ",";
            FileContext = tempCsvString.Substring(0, tempCsvString.Length - 1);
         }
      }
      /// <summary>
      /// 通过文件MD5和文件路径判断两个CSV文件是否相同。
      /// </summary>
      /// <param name="other">用于比较的另一个CSV文件。</param>
      /// <returns>如果两个CSV文件的文件路径和MD5都相同，则操作的这两个CSV文件属于同一个文件，那么这个操作就会返回true，否则将会返回false。</returns>
      public override bool Equals(IOSecurityFile other) => base.Equals((CsvFileOperator)other);
      /// <summary>
      /// 释放该对象引用的所有内存资源。
      /// </summary>
      /// <param name="disposing">用于指示是否释放托管资源。</param>
      protected override void Dispose(bool disposing)
      {
         base.Dispose(disposing);
         CloseWritePassageway();
      }
   }
   /// <summary>
   /// 当文件类型不符合匹配条件时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class FileTypeNotLegitimateException : Exception
   {
      public FileTypeNotLegitimateException() : base("文件类型不匹配！") { }
      public FileTypeNotLegitimateException(string message) : base(message) { }
      public FileTypeNotLegitimateException(string message, Exception inner) : base(message, inner) { }
      protected FileTypeNotLegitimateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
