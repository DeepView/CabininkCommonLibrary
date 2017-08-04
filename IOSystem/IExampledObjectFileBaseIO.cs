using System.Text;
using Cabinink.TypeExtend;
namespace Cabinink.IOSystem
{
   /// <summary>
   /// 适用于可实例化对象的文件基础输入输出接口。
   /// </summary>
   public interface IExampledObjectFileBaseIO
   {
      /// <summary>
      /// 按照默认的方式读取文件内容。
      /// </summary>
      void Read();
      /// <summary>
      /// 通过指定的编码方式来读取文件内容。
      /// </summary>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件读取的编码方式。</param>
      void Read(Encoding encoding);
      /// <summary>
      /// 向文件中存取指定的需要存取的文件内容。
      /// </summary>
      void Write();
      /// <summary>
      /// 通过指定的文件内容存取方式来存取文件内容。
      /// </summary>
      /// <param name="isAppend">用于决定文件内容的存取方式，如果这个参数值为true，则意味着该操作将会以追加的方式把文本内容追加到文件末尾，反之将会以覆盖原本内容的方式存取文件。</param>
      void Write(bool isAppend);
      /// <summary>
      /// 通过指定的文件编码方式来存取文件内容。
      /// </summary>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件存取的编码方式。</param>
      void Write(Encoding encoding);
      /// <summary>
      /// 通过指定的文件内容存取方式和编码方式来存取文件内容。
      /// </summary>
      /// <param name="isAppend">用于决定文件内容的存取方式，如果这个参数值为true，则意味着该操作将会以追加的方式把文本内容追加到文件末尾，反之将会以覆盖原本内容的方式存取文件。</param>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件存取的编码方式</param>
      void Write(bool isAppend, Encoding encoding);
   }
}
