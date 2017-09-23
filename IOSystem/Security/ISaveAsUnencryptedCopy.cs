using System.Text;
namespace Cabinink.IOSystem.Security
{
   /// <summary>
   /// 提供另存为未加密副本文件的接口。
   /// </summary>
   public interface ISaveAsUnencryptedCopy
   {
      /// <summary>
      /// 创建并保存一个当前文件的内容未加密的文件副本。
      /// </summary>
      /// <param name="fileUrl">文件副本的地址。</param>
      /// <param name="encoding">文件内容需要采用的编码格式。</param>
      void SaveAsUnencryptedCopy(string fileUrl, Encoding encoding);
   }
}