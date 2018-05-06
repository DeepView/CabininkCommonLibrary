using System;
using System.IO;
using System.Text;
using System.Linq;
using Cabinink.IOSystem;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend
{
   /// <summary>
   /// 扩展字符串类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class ExString : IDisposable, IComparable<string>, IComparable, IEquatable<ExString>, IEquatable<string>, IConvertible, ICloneable, IEnumerable
   {
      private string _clrString;//当前实例需要操作的核心字符串。
      private bool _disposedValue = false;//检测冗余调用。
      /// <summary>
      /// 构造函数，创建一个空字符串的扩展字符串实例。
      /// </summary>
      public ExString() => _clrString = string.Empty;
      /// <summary>
      /// 构造函数，创建一个指定字符串的扩展字符串实例。
      /// </summary>
      /// <param name="clrString">指定的字符串，用于给当前实例赋值。</param>
      public ExString(string clrString) => _clrString = clrString;
      /// <summary>
      /// 构造函数，创建一个由指定StringBuilder所指示的扩展字符串实例。
      /// </summary>
      /// <param name="stringBuilder">指定的StringBuilder实例。</param>
      public ExString(StringBuilder stringBuilder) => _clrString = stringBuilder.ToString();
      /// <summary>
      /// 构造函数，将当前实例初始化为由Unicode字符数组指示的值。
      /// </summary>
      /// <param name="chars">需要用于初始化当前实例的字符数组。</param>
      [CLSCompliant(false)]
      public ExString(char[] chars) => _clrString = new string(chars);
      /// <summary>
      /// 构造函数，将当前实例初始化为由指定的带符号字节数组指针指示的值。
      /// </summary>
      /// <param name="unicodeBytes">带符号字节数组指针。</param>
      [CLSCompliant(false)]
      public unsafe ExString(sbyte* unicodeBytes) => _clrString = new string(unicodeBytes);
      /// <summary>
      /// 构造函数，将当前实例初始化为由指向Unicode字符数组的指定指针指示的值。
      /// </summary>
      /// <param name="chars">指向以null终止的Unicode字符数组的指针。</param>
      [CLSCompliant(false)]
      public unsafe ExString(char* chars) => _clrString = new string(chars);
      /// <summary>
      /// 构造函数，从指定的文件中以指定编码方式读取文件内容文本并用来实例化当前的实例。
      /// </summary>
      /// <param name="fileUrl">被用来获取文本的文件的文件地址。</param>
      /// <param name="encoding">读取文件的编码格式。</param>
      public ExString(string fileUrl, Encoding encoding) => _clrString = FileOperator.ReadFileContext(fileUrl, true, encoding);
      /// <summary>
      /// 获取或设置当前实例的字符串文本。
      /// </summary>
      public string StringContext { get => _clrString; set => _clrString = value; }
      /// <summary>
      /// 获取当前实例的字符串文本长度。
      /// </summary>
      public int Length => _clrString.Length;
      /// <summary>
      /// 获取或设置当前实例中给定索引所对应的字符。
      /// </summary>
      /// <param name="index">指定的索引，这个索引对应着字符串中的某一个字符，用于获取或者决定这个字符的值。</param>
      public char this[int index]
      {
         get => StringContext[index];
         set
         {
            char[] stringChars = StringContext.ToCharArray();
            stringChars[index] = value;
         }
      }
      /// <summary>
      /// 将当前实例重置为空字符串状态，而非NULL状态。
      /// </summary>
      public void ResetToEmpty() => StringContext = string.Empty;
      /// <summary>
      /// 判定当前字符串是否为空字符串。
      /// </summary>
      /// <returns>如果当前实例为空字符串（Empty）则会返回true，否则会返回false。</returns>
      public bool IsEmpty() => Length == 0 ? true : false;
      /// <summary>
      /// 在当前实例获取指定的字符串，起始位置为index，长度等于索引开始到字符串结尾的长度。
      /// </summary>
      /// <param name="index">开始获取字符串的起始位置，索引0表示第一位字符。</param>
      /// <returns>该方法将会返回获取的指定长度的字符串。</returns>
      public ExString SubString(int index)
      {
         return new ExString(StringContext.Substring(index));
      }
      /// <summary>
      /// 在当前实例获取指定的字符串，起始位置为index，截取长度为length。
      /// </summary>
      /// <param name="index">开始获取字符串的起始位置，索引0表示第一位字符。</param>
      /// <param name="length">获取字符串的长度。</param>
      /// <returns>该方法将会返回获取的指定长度的字符串。</returns>
      /// <exception cref="LengthOverflowException">当length参数指定的长度超出了当前实例的长度时，则会抛出这个异常。</exception>
      /// <exception cref="ArgumentOutOfRangeException">当index参数的值超出了实例长度范围，则会抛出这个异常。</exception> 
      public ExString SubString(int index, int length)
      {
         if (length > Length - index + 1) throw new LengthOverflowException();
         if (index < 0 || index > Length) throw new ArgumentOutOfRangeException("index", "索引超出实例长度范围！");
         return new ExString(StringContext.Substring(index, length));
      }
      /// <summary>
      /// 在当前实例获取指定的字符串并作为字符数组返回，起始位置为index，截取长度为length。
      /// </summary>
      /// <param name="index">开始获取字符串的起始位置，索引0表示第一位字符。</param>
      /// <param name="length">获取字符串的长度。</param>
      /// <returns>该方法将会返回获取的指定长度的字符串所对应的字符数组。</returns>
      public char[] SubStringForReturnChars(int index, int length)
      {
         return SubString(index, length).StringContext.ToArray();
      }
      /// <summary>
      /// 在当前实例获取指定的字符串并作为字符指针并返回，起始位置为index，截取长度为length。
      /// </summary>
      /// <param name="index">开始获取字符串的起始位置，索引0表示第一位字符。</param>
      /// <param name="length">获取字符串的长度。</param>
      /// <returns>该方法将会返回获取的指定长度的字符串所对应的字符指针。</returns>
      /// <remarks>当前方法使用到了指针技术，换句话说，这里的所有代码属于不安全代码，所以该代码在内存管理方面无法受到CLR托管规则的约束，因此无法保证线程的安全性，请谨慎使用。</remarks>
      [CLSCompliant(false)]
      public unsafe char* SubStringForReturnPointer(int index, int length)
      {
         char* charsPointer;
         char[] chars = SubStringForReturnChars(index, length);
         GCHandle resultPointer = GCHandle.Alloc(chars);
         charsPointer = (char*)GCHandle.ToIntPtr(resultPointer).ToPointer();
         resultPointer.Free();
         return charsPointer;
      }
      /// <summary>
      /// 首部截取指定长度的字符串。
      /// </summary>
      /// <param name="interceptLength">需要截取的长度。</param>
      /// <returns>操作完成并没有任何异常抛出之后，则会返回一个从当前实例索引0开始截取指定长度的ExString字符串实例。</returns>
      public ExString FirstIntercept(int interceptLength) => SubString(0, interceptLength);
      /// <summary>
      /// 尾部截取指定长度的字符串。
      /// </summary>
      /// <param name="interceptLength">需要截取的长度。</param>
      /// <returns>操作完成并没有任何异常抛出之后，则会返回一个从当前实例尾部截取指定长度的ExString字符串实例。</returns>
      public ExString TailIntercept(int interceptLength) => SubString(Length - interceptLength);
      /// <summary>
      /// 删除实例的前导空格，但是会忽略尾部空格，并且原有实例不会发生变化，操作之后会返回一个新的实例。
      /// </summary>
      /// <returns>如果操作无异常，将会返回一个无前导空格的新实例。</returns>
      public ExString DeleteLeadingSpaces()
      {
         ExString beforeString = StringContext;
         ExString afterString = string.Empty;
         for (int i = 0; i < Length; i++) afterString = ((string)this).TrimStart(new char[] { ' ' });
         return afterString;
      }
      /// <summary>
      /// 删除实例的尾部空格，但是会忽略前导空格，并且原有实例不会发生变化，操作之后会返回一个新的实例。
      /// </summary>
      /// <returns>如果操作无异常，将会返回一个无尾部空格的新实例。</returns>
      public ExString DeleteTrailingSpaces()
      {
         ExString beforeString = StringContext;
         ExString afterString = string.Empty;
         for (int i = 0; i < Length; i++) afterString = ((string)this).TrimEnd(new char[] { ' ' });
         return afterString;
      }
      /// <summary>
      /// 删除前导和尾部空格，并且原有实例不会发生变化，操作之后会返回一个新的实例。
      /// </summary>
      /// <returns>如果操作无异常，将会返回一个无前导和尾部空格的新实例。</returns>
      public ExString DeleteNodeSpace()
      {
         return DeleteLeadingSpaces().DeleteTrailingSpaces();
      }
      /// <summary>
      /// 删除实例中的所有空格，并且原有实例不会发生变化，操作之后会返回一个新的实例。
      /// </summary>
      /// <returns>如果操作无异常，则将会返回一个不存在空格的新实例。</returns>
      public ExString DeleteAllSpace()
      {
         return ((string)this).Replace(" ", string.Empty);
      }
      /// <summary>
      /// 将指定索引所对应的字符转换为ASCII代码。
      /// </summary>
      /// <param name="index">指定的索引。</param>
      /// <returns>如果操作无异常，该操作将会返回一个byte类型的ASCII代码。</returns>
      /// <exception cref="ArgumentOutOfRangeException">当index参数的值超出了实例长度范围，则会抛出这个异常。</exception> 
      public int ToAsciiCode(int index)
      {
         if (index < 0 || index > Length) throw new ArgumentOutOfRangeException("index", "索引超出实例长度范围！");
         return ((string)SubString(index, 1)).ToArray()[0];
      }
      /// <summary>
      /// 返回当前扩展字符串实例的ASCII序列。
      /// </summary>
      /// <param name="isAppendSpace">指示当前实例中每一个字符所对应的ASCII代码后面是否追加一个空格。</param>
      /// <returns>如果该操作没有产生任何异常，则会得到一个当前实例的ASCII序列字符串。</returns>
      public ExString ToAsciiSerial(bool isAppendSpace)
      {
         ExString serial = new ExString();
         int ascii = 0;
         string asciiString = string.Empty;
         for (int i = 0; i < Length; i++)
         {
            ascii = ToAsciiCode(i);
            asciiString = ascii.ToString("D4");
            if (isAppendSpace) serial = serial + asciiString + " ";
            else serial += asciiString;
         }
         if (isAppendSpace) return serial.DeleteTrailingSpaces();
         else return serial;
      }
      /// <summary>
      /// 将当前字符串实例转换为字节码数组，并允许中文转换。
      /// </summary>
      /// <param name="encoding">指定的编码格式。</param>
      /// <returns>执行这个操作之后会获取当前实例所表示字符串中每一个字符的字节码的有序集合，在这里值得一提的是，这个有序集合的顺序和当前字符串实例中字符排序是相吻合的。</returns>
      public byte[] ToBytecodeArray(Encoding encoding) => encoding.GetBytes(StringContext);
      /// <summary>
      /// 将当前的字符串实例转换为十六进制字节码数组。
      /// </summary>
      /// <returns>执行这个操作之后会获取当前实例所表示字符串中每一个字符的十六进制字节码的有序集合。</returns>
      public byte[] ToHexadecimalArray()
      {
         ExString hexString = ReplaceAll(" ", "");
         if ((Length % 2) != 0) hexString += " ";
         byte[] code = new byte[hexString.Length / 2];
         for (int i = 0; i < code.Length; i++)
         {
            code[i] = Convert.ToByte(hexString.SubString(i * 2, 2).ReplaceAll(" ", ""), 16);
         }
         return code;
      }
      /// <summary>
      /// 返回当前扩展字符串实例的十六进制ASCII序列。
      /// </summary>
      /// <returns>如果该操作没有产生任何异常，则会得到一个当前实例的十六进制ASCII序列字符串。</returns>
      public ExString ToAsciiHexSerial()
      {
         char[] strArray = ConvertToCharArray();
         StringBuilder sBuilder = new StringBuilder();
         for (int i = 0; i < strArray.Length; i++) sBuilder.AppendFormat("{0:X4}\x20", strArray[i]);
         return new ExString(sBuilder.ToString()).DeleteNodeSpace();
      }
      /// <summary>
      /// 检查指定的字符是否在当前ExString所表示的实例之中。
      /// </summary>
      /// <param name="keywordChar">需要被检索的字符。</param>
      /// <param name="location">被检索字符在实例中的位置（索引），如果该操作返回true，则这个引用参数将会是一个表示被检索字符的位置，否则这个参数将为-1。</param>
      /// <returns>如果参数KeywordChar指定的字符在当前实例中存在，则返回true，否则返回false。</returns>
      public bool IsFoundChar(char keywordChar, ref int location)
      {
         bool founded = false;
         int locationByVal = location;
         locationByVal = -1;
         List<char> converted = StringContext.ToArray().ToList();
         Parallel.For(0, converted.Count, (index, interrupt) =>
         {
            if (converted[index] == keywordChar)
            {
               founded = true;
               locationByVal = index;
               interrupt.Stop();
            }
         });
         location = locationByVal;
         return founded;
      }
      /// <summary>
      /// 检查指定的字符串是否在当前ExString所表示的实例之中。
      /// </summary>
      /// <param name="keywordString">需要被检索的字符串。</param>
      /// <param name="location">被检索字符串的第一个字符在实例中的位置（索引），如果该操作返回true，则这个引用参数将会是一个表示被检索字符串的第一个字符的位置，否则这个参数将为-1。</param>
      /// <returns>如果参数KeywordString指定的字符串在当前实例中存在，则返回true，否则返回false。</returns>
      public bool IsFoundString(string keywordString, ref int location)
      {
         bool founded = false;
         location = -1;
         for (int i = 0; i < Length - keywordString.Length; i++)
         {
            if (SubString(i, keywordString.Length) == keywordString)
            {
               founded = true;
               location = i;
               break;
            }
         }
         return founded;
      }
      /// <summary>
      /// 替换实例中指定索引的字符串。
      /// </summary>
      /// <param name="index">指定的索引，指示这个操作该从哪一个字符开始执行替换操作。</param>
      /// <param name="newString">替换进去的字符串。</param>
      /// <returns>这个操作无异常产生之后，将会得到一个替换成功的新的扩展字符串实例。</returns>
      /// <remarks>该方法并没有明确指定替换的长度。不过在该方法的有效代码中，替换长度是由newString参数决定的，如果newString字符串长度大于索引之后的字符串的长度，则会替换屌这个索引之后的所有字符串。否则将会替换newString参数表示的字符串的长度的字符串。</remarks>
      public ExString ReplaceOf(int index, string newString)
      {
         ExString reslut = new ExString();
         if (newString.Length > Length - index + 1) reslut = ReplaceOf(index, Length - index + 1, newString);
         else reslut = ReplaceOf(index, newString.Length, newString);
         return reslut;
      }
      /// <summary>
      /// 替换实例中指定索引和指定长度的字符串。
      /// </summary>
      /// <param name="index">指定的索引，指示这个操作该从哪一个字符开始执行替换操作。</param>
      /// <param name="length">需要替换的长度，这个如果需要替换两个字符，则这个长度就等于2，这个长度与newString参数指定的字符串的长度没有任何关系。</param>
      /// <param name="newString">替换进去的字符串。</param>
      /// <returns>这个操作无异常产生之后，将会得到一个替换成功的新的扩展字符串实例。</returns>
      /// <exception cref="LengthOverflowException">当length参数指定的长度超出了当前实例的长度时，则会抛出这个异常。</exception>
      /// <exception cref="ArgumentOutOfRangeException">当index参数的值超出了实例长度范围，则会抛出这个异常。</exception> 
      public ExString ReplaceOf(int index, int length, string newString)
      {
         if (length > Length - index + 1) throw new LengthOverflowException();
         if (index < 0 || index > Length) throw new ArgumentOutOfRangeException("index", "索引超出实例长度范围！");
         ExString precursorString = SubString(0, index + 1);
         ExString subsequentString = SubString(index + length);
         return new ExString(precursorString + newString + subsequentString);
      }
      /// <summary>
      /// 返回一个新字符串，其中当前实例中出现的所有指定字符串都替换为另一个指定的字符串。
      /// </summary>
      /// <param name="oldString">要替换的字符串。</param>
      /// <param name="newString">替换进去的字符串。</param>
      /// <returns>等效于当前字符串（除了oldString的所有实例都已替换为newString外）的字符串。如果在当前实例中找不到oldString，此方法返回未更改的当前实例。</returns>
      /// <remarks>如果newString是null，出现的所有oldString会删除。此方法执行序号（区分大小写和不区分区域性的）搜索以查找oldString。因为此方法返回修改后的字符串，您可以链接在一起对连续调用ReplaceAll方法，以执行多个替换项上的原始字符串。</remarks>
      public ExString ReplaceAll(string oldString, string newString)
      {
         return StringContext.Replace(oldString, newString);
      }
      /// <summary>
      /// 返回一个新的字符串，在此实例中的指定的索引位置插入指定的字符串。
      /// </summary>
      /// <param name="index">插入的从零开始的索引位置。</param>
      /// <param name="inserted">要插入的字符串。</param>
      /// <returns>与此实例等效的一个新字符串，但在该字符串的index位置处插入了inserted。</returns>
      /// <remarks>如果index大于当前实例的长度，则inserted将会插入到实例文本的末尾处。</remarks>
      public ExString InsertBy(int index, string inserted)
      {
         return StringContext.Insert(index, inserted);
      }
      /// <summary>
      /// 基于参数中的字符将字符串拆分为多个子字符串。
      /// </summary>
      /// <param name="separator">分隔此字符串中子字符串的字符、不包含分隔符的字符或null。</param>
      /// <returns>一个数组，其元素包含此实例中的子字符串，这些子字符串由separator中的字符分隔。 </returns>
      /// <remarks>该方法用于分隔为多个子字符串由一组已知的字符分隔的字符串，返回的数组元素中不包括分隔符字符。</remarks>
      public string[] SegmentationBy(char separator)
      {
         return SegmentationBy(new char[] { separator });
      }
      /// <summary>
      /// 基于数组中的字符将字符串拆分为多个子字符串。
      /// </summary>
      /// <param name="separators">分隔此字符串中子字符串的字符数组、不包含分隔符的空数组或null。</param>
      /// <returns>一个数组，其元素包含此实例中的子字符串，这些子字符串由separator中的一个或多个字符分隔。</returns>
      /// <remarks>该方法等效于String.Split方法。</remarks>
      public string[] SegmentationBy(char[] separators)
      {
         return StringContext.Split(separators);
      }
      /// <summary>
      /// 基于多个索引将字符串拆分为多个子字符串。
      /// </summary>
      /// <param name="indexes">用于分割字符串的索引集合。</param>
      /// <returns>该方法用于分隔为多个子字符串由一组已知的字符分隔的字符串。</returns>
      /// <exception cref="ArgumentOutOfRangeException">如果参数indexes包含的任意索引，或者包含索引数量过多时，则会抛出这个异常。</exception>
      public string[] SegmentationBy(int[] indexes)
      {
         string[] result;
         Parallel.For(0, Length, (index, interrupt) =>
         {
            if (indexes[index] > Length - 1)
            {
               int exLocation = index + 1;
               throw new ArgumentOutOfRangeException("参数indexes中第" + exLocation + "个索引超出当前实例长度！");
            }
         });
         if (indexes.Count() > Length) throw new ArgumentOutOfRangeException("参数indexes包含的索引数量过多！");
         else
         {
            int loopLimit = 0;
            ExString tempStr = this;
            for (int i = 0; i < tempStr.Length; i++)
            {
               if (loopLimit > 0) tempStr = tempStr.InsertBy(indexes[i], "\x1");
               else tempStr = tempStr.InsertBy(indexes[i + 1], "\x1");
               loopLimit++;
            }
            result = SegmentationBy('\x1');
         }
         return result;
      }
      /// <summary>
      /// 将此实例中的字符复制到Unicode字符数组。
      /// </summary>
      /// <returns>元素为此实例的各字符的Unicode字符数组。如果此实例是空字符串，则返回的数组为空且长度为零。</returns>
      public char[] ConvertToCharArray()
      {
         return StringContext.ToCharArray();
      }
      /// <summary>
      /// 将此实例中的字符复制到Unicode字符指针中。
      /// </summary>
      /// <returns>该方法将会返回当前实例所转换的字符数组所对应的字符指针。</returns>
      /// <remarks>当前方法使用到了指针技术，换句话说，这里的所有代码属于不安全代码，所以该代码在内存管理方面无法受到CLR托管规则的约束，因此无法保证线程的安全性，请谨慎使用。</remarks>
      [CLSCompliant(false)]
      public unsafe char* ConvertToCharPointer()
      {
         char* charsPointer;
         char[] charsArray = ConvertToCharArray();
         GCHandle resultPointer = GCHandle.Alloc(charsArray);
         charsPointer = (char*)GCHandle.ToIntPtr(resultPointer).ToPointer();
         resultPointer.Free();
         return charsPointer;
      }
      /// <summary>
      /// 返回此实例的TypeCode。
      /// </summary>
      /// <returns>枚举常数，它是实现该接口的类或值类型的TypeCode。</returns>
      public TypeCode GetTypeCode()
      {
         return StringContext.GetTypeCode();
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的Boolean值。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的Boolean值。</returns>
      public bool ToBoolean(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToBoolean(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的Unicode字符。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的Unicode字符。</returns>
      public char ToChar(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToChar(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的8位有符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的8位有符号整数。</returns>
      [CLSCompliant(false)]
      public sbyte ToSByte(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToSByte(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将该实例的值转换为等效的8位无符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与该实例的值等效的8位无符号整数。</returns>
      public byte ToByte(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToByte(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的16位有符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的16位有符号整数。</returns>
      public short ToInt16(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToInt16(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将该实例的值转换为等效的16位无符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与该实例的值等效的16位无符号整数。</returns>
      [CLSCompliant(false)]
      public ushort ToUInt16(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToUInt16(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的32位有符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的32位有符号整数。</returns>
      public int ToInt32(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToInt32(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将该实例的值转换为等效的32位无符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与该实例的值等效的32位无符号整数。</returns>
      [CLSCompliant(false)]
      public uint ToUInt32(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToUInt32(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的64位有符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的64位有符号整数。</returns>
      public long ToInt64(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToInt64(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将该实例的值转换为等效的64位无符号整数。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与该实例的值等效的64位无符号整数。</returns>
      [CLSCompliant(false)]
      public ulong ToUInt64(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToUInt64(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的单精度浮点数字。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的单精度浮点数字。</returns>
      public float ToSingle(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToSingle(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的双精度浮点数字。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的双精度浮点数字。</returns>
      public double ToDouble(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToDouble(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的Decimal数字。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的Decimal数字。</returns>
      public decimal ToDecimal(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToDecimal(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的DateTime。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的DateTime实例。</returns>
      public DateTime ToDateTime(IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToDateTime(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为等效的String。
      /// </summary>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>与此实例的值等效的String实例。</returns>
      public string ToString(IFormatProvider provider)
      {
         return StringContext.ToString(provider);
      }
      /// <summary>
      /// 使用指定的区域性特定格式设置信息将此实例的值转换为具有等效值的指定Type的Object。
      /// </summary>
      /// <param name="conversionType"></param>
      /// <param name="provider">IFormatProvider接口实现，提供区域性特定的格式设置信息。</param>
      /// <returns>要将此实例的值转换为的Type。</returns>
      public object ToType(Type conversionType, IFormatProvider provider)
      {
         return ((IConvertible)StringContext).ToType(conversionType, provider);
      }
      /// <summary>
      /// 将此实例与指定的String对象进行比较，并指示此实例在排序顺序中是位于指定的字符串之前、之后还是与其出现在同一位置。
      /// </summary>
      /// <param name="other">要与此实例进行比较的字符串。</param>
      /// <returns>一个32位带符号整数，该整数指示此实例在排序顺序中是位于other参数之前、之后还是与其出现在同一位置。</returns>
      public int CompareTo(string other)
      {
         return StringContext.CompareTo(other);
      }
      /// <summary>
      /// 将此实例与指定的ExString对象进行比较，并指示此实例在排序顺序中是位于指定的字符串之前、之后还是与其出现在同一位置。
      /// </summary>
      /// <param name="other">要与此实例进行比较的扩展字符串。</param>
      /// <returns>一个32位带符号整数，该整数指示此实例在排序顺序中是位于other参数之前、之后还是与其出现在同一位置。</returns>
      public int CompareTo(ExString other)
      {
         return CompareTo(other.StringContext);
      }
      /// <summary>
      /// 将此实例与指定的object进行比较，并指示此实例在排序顺序中是位于指定的字符串之前、之后还是与其出现在同一位置。
      /// </summary>
      /// <param name="obj">要与此实例进行比较的object实例。</param>
      /// <returns>一个32位带符号整数，该整数指示此实例在排序顺序中是位于other参数之前、之后还是与其出现在同一位置。</returns>
      public int CompareTo(object obj) => StringContext.CompareTo(obj);
      /// <summary>
      /// 返回对此ExString实例的引用。
      /// </summary>
      /// <returns>作为此实例副本的新对象。</returns>
      public object Clone()
      {
         return StringContext.Clone();
      }
      /// <summary>
      /// 检索一个可以循环访问此ExString扩展字符串中的每一个字符的对象。
      /// </summary>
      /// <returns>该操作会返回一个枚举器对象。</returns>
      public IEnumerator GetEnumerator() => StringContext.GetEnumerator();
      /// <summary>
      /// 确定此实例是否与另一个指定的ExString对象具有相同的值。
      /// </summary>
      /// <param name="other">要与此实例进行比较的扩展字符串。</param>
      /// <returns>如果true参数的值与此实例的值相同，则为value；否则为false。如果value为null，则此方法返回false。</returns>
      public bool Equals(ExString other)
      {
         return StringContext.Equals(other.StringContext);
      }
      /// <summary>
      /// 确定此实例是否与指定的string对象具有相同的值。
      /// </summary>
      /// <param name="other">要与此实例进行比较的扩展字符串。</param>
      /// <returns>如果true参数的值与此实例的值相同，则为value；否则为false。如果value为null，则此方法返回false。</returns>
      public bool Equals(string other)
      {
         return StringContext.Equals(other);
      }
      /// <summary>
      /// 确定此实例是否与指定的对象（也必须是ExString对象）具有相同的值。
      /// </summary>
      /// <param name="obj">要与此实例进行比较的字符串。</param>
      /// <returns>如果true是一个obj且其值与此实例相等，则为ExString；否则为false。如果obj为null，则此方法返回false。</returns>
      public override bool Equals(object obj)
      {
         return StringContext.Equals(obj);
      }
      /// <summary>
      /// 返回String的此实例；不执行实际转换。
      /// </summary>
      /// <returns>当前的字符串。</returns>
      public override string ToString() => StringContext.ToString();
      /// <summary>
      /// 返回当前对象的哈希代码。
      /// </summary>
      /// <returns>该函数会返回一个有效的哈希代码。</returns>
      public override int GetHashCode() => base.GetHashCode() + StringContext.GetHashCode();
      /// <summary>
      /// 隐式转换操作符重载（To ExString）。
      /// </summary>
      /// <param name="v">隐式转换操作符的源类型。</param>
      public static implicit operator ExString(string v) => new ExString(v);
      /// <summary>
      /// 隐式转换操作符重载（To String）。
      /// </summary>
      /// <param name="v">隐式转换操作符的源类型。</param>
      public static implicit operator string(ExString v) => v.StringContext;
      /// <summary>
      /// 随机生成指定长度的字符串。
      /// </summary>
      /// <param name="length">需要生成的字符串的长度。</param>
      /// <param name="isIncludeSymbol">决定这个随机字符串是否包含在ASCII码中的标点符号。</param>
      /// <returns>该方法在执行之后会返回一个随机生成的字符串。</returns>
      public static ExString GenerateRandomString(int length, bool isIncludeSymbol)
      {
         DateTime utcNow = DateTime.UtcNow;
         int seed = utcNow.Day * utcNow.Hour * utcNow.Minute * utcNow.Second * utcNow.Millisecond;
         return GenerateRandomString(length, seed, isIncludeSymbol);
      }
      /// <summary>
      /// 通过一个种子值来随机生成指定长度的字符串。
      /// </summary>
      /// <param name="length">需要生成的字符串的长度。</param>
      /// <param name="seed">用于随机生成的种子值，这个种子值可随意设置，但是范围需要在Int32的有效范围中。</param>
      /// <param name="isIncludeSymbol">决定这个随机字符串是否包含在ASCII码中的标点符号。</param>
      /// <returns>该方法在执行之后会返回一个随机生成的字符串。</returns>
      public static ExString GenerateRandomString(int length, int seed, bool isIncludeSymbol)
      {
         ExString randomString = string.Empty;
         Random random = new Random(seed);
         string numAndLetter = "0123456789ABCDEFGHIJKMLNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
         string onlySymbol = "~`!@#$%^&*()_+-=[]\\{}|;':\x22,./<>?";
         if (isIncludeSymbol) randomString = numAndLetter + onlySymbol;
         else randomString = numAndLetter;
         string returnValue = string.Empty;
         for (int i = 0; i < length; i++)
         {
            int randomInt = random.Next(0, randomString.Length - 1);
            returnValue += randomString[randomInt];
         }
         return returnValue;
      }
      /// <summary>
      /// 加密指定的ExString实例。
      /// </summary>
      /// <param name="plaintext">需要被加密的明文。</param>
      /// <param name="key">用于加密的密钥，长度必须为8位。</param>
      /// <returns>如果操作无异常，该操作将会返回一个plaintext参数所对应的密文。</returns>
      /// <exception cref="OverflowException">当密钥的长度不等于8时，则会抛出这个异常！</exception>
      public static ExString Encrypt(ExString plaintext, ExString key)
      {
         if (key.Length != 8) throw new OverflowException("密钥的长度必须为8！");
         using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
         {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plaintext);
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
               cs.Write(inputByteArray, 0, inputByteArray.Length);
               cs.FlushFinalBlock();
               cs.Close();
            }
            ms.Close();
            return Convert.ToBase64String(ms.ToArray());
         }
      }
      /// <summary>
      /// 解密指定的ExString密文实例。
      /// </summary>
      /// <param name="ciphertext">需要被解密的密文。</param>
      /// <param name="key">用于解密的密钥，这个密钥和加密的密钥相同，长度必须为8位。</param>
      /// <returns>如果操作无异常，该操作将会返回一个ciphertext参数所对应的明文。</returns>
      /// <exception cref="OverflowException">当密钥的长度不等于8时，则会抛出这个异常！</exception>
      public static ExString Decrypt(ExString ciphertext, ExString key)
      {
         if (key.Length != 8) throw new OverflowException("密钥的长度必须为8！");
         byte[] inputByteArray = Convert.FromBase64String(ciphertext);
         using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
         {
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
            {
               cs.Write(inputByteArray, 0, inputByteArray.Length);
               cs.FlushFinalBlock();
               cs.Close();
            }
            ms.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
         }
      }
      #region IDisposable Support
      /// <summary>
      /// 释放该对象引用的所有内存资源。
      /// </summary>
      /// <param name="disposing">用于指示是否释放托管资源。</param>
      protected virtual void Dispose(bool disposing)
      {
         int clsStrMaxGene = GC.GetGeneration(_clrString);
         if (!_disposedValue)
         {
            if (disposing)
            {
               _clrString = null;
               bool condition = GC.CollectionCount(clsStrMaxGene) == 0;
               if (condition) GC.Collect(clsStrMaxGene, GCCollectionMode.Forced, true);
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
   /// <summary>
   /// 当指定的长度超出了实例的长度时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class LengthOverflowException : Exception
   {
      public LengthOverflowException() : base("指定长度超出实例长度范围！") { }
      public LengthOverflowException(string message) : base(message) { }
      public LengthOverflowException(string message, Exception inner) : base(message, inner) { }
      protected LengthOverflowException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
