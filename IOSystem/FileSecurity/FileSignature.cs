using System;
using System.IO;
using System.Linq;
using System.Text;
using Cabinink.TypeExtend;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem.FileSecurity
{
   /// <summary>
   /// 文件指纹（或者是文件签名）校验类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class FileSignature
   {
      private string _fileUrl;//本地文件地址。
      private List<uint> _crc32Table;//CRC32校验列表。
      /// <summary>
      /// 构造函数，用于初始化一个指定文件地址的FileSignature实例，并初始化CRC32校验列表。
      /// </summary>
      /// <param name="fileUrl">指定的本地文件地址。</param>
      /// <exception cref="FileNotFoundException">当参数fileUrl指定的文件找不到时，则会抛出这个异常。</exception>
      public FileSignature(string fileUrl)
      {
         if (!FileOperator.FileExists(fileUrl)) throw new FileNotFoundException(fileUrl, "指定的文件找不到！");
         else _fileUrl = fileUrl;
         _crc32Table = new uint[] {
                        0x0, 0x77073096, 0xee0e612cu, 0x990951bau, 0x76dc419, 0x706af48f, 0xe963a535u,
                        0x9e6495a3u, 0xedb8832, 0x79dcb8a4, 0xe0d5e91eu, 0x97d2d988u, 0x9b64c2b,
                        0x7eb17cbd, 0xe7b82d07u, 0x90bf1d91u, 0x1db71064, 0x6ab020f2, 0xf3b97148u,
                        0x84be41deu, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551u, 0x83d385c7u, 0x136c9856,
                        0x646ba8c0, 0xfd62f97au, 0x8a65c9ecu, 0x14015c4f, 0x63066cd9, 0xfa0f3d63u,
                        0x8d080df5u, 0x3b6e20c8, 0x4c69105e, 0xd56041e4u, 0xa2677172u, 0x3c03e4d1,
                        0x4b04d447, 0xd20d85fdu, 0xa50ab56bu, 0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6u,
                        0xacbcf940u, 0x32d86ce3, 0x45df5c75, 0xdcd60dcfu, 0xabd13d59u, 0x26d930ac,
                        0x51de003a, 0xc8d75180u, 0xbfd06116u, 0x21b4f4b5, 0x56b3c423, 0xcfba9599u,
                        0xb8bda50fu, 0x2802b89e, 0x5f058808, 0xc60cd9b2u, 0xb10be924u, 0x2f6f7c87,
                        0x58684c11, 0xc1611dabu, 0xb6662d3du, 0x76dc4190, 0x1db7106, 0x98d220bcu,
                        0xefd5102au, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5u, 0xe8b8d433u, 0x7807c9a2,
                        0xf00f934, 0x9609a88eu, 0xe10e9818u, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97u,
                        0xe6635c01u, 0x6b6b51f4, 0x1c6c6162, 0x856530d8u, 0xf262004eu, 0x6c0695ed,
                        0x1b01a57b, 0x8208f4c1u, 0xf50fc457u, 0x65b0d9c6, 0x12b7e950, 0x8bbeb8eau,
                        0xfcb9887cu, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3u, 0xfbd44c65u, 0x4db26158,
                        0x3ab551ce, 0xa3bc0074u, 0xd4bb30e2u, 0x4adfa541, 0x3dd895d7, 0xa4d1c46du,
                        0xd3d6f4fbu, 0x4369e96a, 0x346ed9fc, 0xad678846u, 0xda60b8d0u, 0x44042d73,
                        0x33031de5, 0xaa0a4c5fu, 0xdd0d7cc9u, 0x5005713c, 0x270241aa, 0xbe0b1010u,
                        0xc90c2086u, 0x5768b525, 0x206f85b3, 0xb966d409u, 0xce61e49fu, 0x5edef90e,
                        0x29d9c998, 0xb0d09822u, 0xc7d7a8b4u, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3bu,
                        0xc0ba6cadu, 0xedb88320u, 0x9abfb3b6u, 0x3b6e20c, 0x74b1d29a, 0xead54739u,
                        0x9dd277afu, 0x4db2615, 0x73dc1683, 0xe3630b12u, 0x94643b84u, 0xd6d6a3e,
                        0x7a6a5aa8, 0xe40ecf0bu, 0x9309ff9du, 0xa00ae27, 0x7d079eb1, 0xf00f9344u,
                        0x8708a3d2u, 0x1e01f268, 0x6906c2fe, 0xf762575du, 0x806567cbu, 0x196c3671,
                        0x6e6b06e7, 0xfed41b76u, 0x89d32be0u, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6fu,
                        0x8ebeeff9u, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8u, 0xa1d1937eu, 0x38d8c2c4,
                        0x4fdff252, 0xd1bb67f1u, 0xa6bc5767u, 0x3fb506dd, 0x48b2364b, 0xd80d2bdau,
                        0xaf0a1b4cu, 0x36034af6, 0x41047a60, 0xdf60efc3u, 0xa867df55u, 0x316e8eef,
                        0x4669be79, 0xcb61b38cu, 0xbc66831au, 0x256fd2a0, 0x5268e236, 0xcc0c7795u,
                        0xbb0b4703u, 0x220216b9, 0x5505262f, 0xc5ba3bbeu, 0xb2bd0b28u, 0x2bb45a92,
                        0x5cb36a04, 0xc2d7ffa7u, 0xb5d0cf31u, 0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0u,
                        0xec63f226u, 0x756aa39c, 0x26d930a, 0x9c0906a9u, 0xeb0e363fu, 0x72076785,
                        0x5005713, 0x95bf4a82u, 0xe2b87a14u, 0x7bb12bae, 0xcb61b38, 0x92d28e9bu,
                        0xe5d5be0du, 0x7cdcefb7, 0xbdbdf21, 0x86d3d2d4u, 0xf1d4e242u, 0x68ddb3f8,
                        0x1fda836e, 0x81be16cdu, 0xf6b9265bu, 0x6fb077e1, 0x18b74777, 0x88085ae6u,
                        0xff0f6a70u, 0x66063bca, 0x11010b5c, 0x8f659effu, 0xf862ae69u, 0x616bffd3,
                        0x166ccf45, 0xa00ae278u, 0xd70dd2eeu, 0x4e048354, 0x3903b3c2, 0xa7672661u,
                        0xd06016f7u, 0x4969474d, 0x3e6e77db, 0xaed16a4au, 0xd9d65adcu, 0x40df0b66,
                        0x37d83bf0, 0xa9bcae53u, 0xdebb9ec5u, 0x47b2cf7f, 0x30b5ffe9, 0xbdbdf21cu,
                        0xcabac28au, 0x53b39330, 0x24b4a3a6, 0xbad03605u, 0xcdd70693u, 0x54de5729,
                        0x23d967bf, 0xb3667a2eu, 0xc4614ab8u, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37u,
                        0xc30c8ea1u, 0x5a05df1b, 0x2d02ef8d
         }.ToList();
      }
      /// <summary>
      /// 获取或设置当前实例的本地文件路径。
      /// </summary>
      /// <exception cref="FileNotFoundException">当内置参数value指定的文件找不到时，则会抛出这个异常。</exception>
      public string FileUrl
      {
         get => _fileUrl;
         set
         {
            if (!FileOperator.FileExists(value)) throw new FileNotFoundException(value, "指定的文件找不到！");
            else FileUrl = value;
         }
      }
      /// <summary>
      /// 获取当前文件的MD5值。
      /// </summary>
      /// <returns>该操作如果无异常发生，将会返回当前实例指定文件的MD5值文本。</returns>
      public ExString GetMD5String()
      {
         FileStream fStream = new FileStream(FileUrl, FileMode.Open);
         MD5 md5 = new MD5CryptoServiceProvider();
         byte[] rByteArrayVal = md5.ComputeHash(fStream);
         fStream.Close();
         StringBuilder sBuilder = new StringBuilder();
         int i = 0;
         while (i < rByteArrayVal.Length)
         {
            sBuilder.Append(rByteArrayVal[i].ToString("x2"));
            Math.Max(System.Threading.Interlocked.Increment(ref i), i - 1);
         }
         return new ExString(sBuilder.ToString().ToUpper());
      }
      /// <summary>
      /// 获取当前文件的SHA1值。
      /// </summary>
      /// <returns>该操作如果无异常发生，将会返回当前实例指定文件的SHA1值文本。</returns>
      public ExString GetSHA1String()
      {
         string strHashData = string.Empty;
         byte[] arrbytHashValue;
         FileStream oFileStream;
         SHA1CryptoServiceProvider osha1 = new SHA1CryptoServiceProvider();
         oFileStream = new FileStream(FileUrl.Replace("\"", ""), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
         arrbytHashValue = osha1.ComputeHash(oFileStream);
         oFileStream.Close();
         strHashData = BitConverter.ToString(arrbytHashValue);
         strHashData = strHashData.Replace("-", string.Empty);
         return new ExString(strHashData);
      }
      /// <summary>
      /// 获取当前文件的CRC32值。
      /// </summary>
      /// <returns>该操作如果无异常发生，将会返回当前实例指定文件的CRC32值文本。</returns>
      public ExString GetCRC32String()
      {
         uint crc = 0xffffffffu;
         FileStream fp1;
         byte ch;
         fp1 = new FileStream(FileUrl, FileMode.Open);
         long len = fp1.Length;
         long i = 0;
         while (i < len)
         {
            ch = (byte)fp1.ReadByte();
            crc = ((crc >> 8) & 0xffffff) ^ _crc32Table[((int)crc ^ ch) & 0xff];
            Math.Max(System.Threading.Interlocked.Increment(ref i), i - 1);
         }
         crc = crc ^ 0xffffffffu;
         fp1.Close();
         fp1.Dispose();
         return crc.ToString("X");
      }
      /// <summary>
      /// 返回FileSignature实例的字符串表达形式，这种表达形式能够兼容String类型，但是无法作为更广泛用途的使用源。
      /// </summary>
      /// <returns>一个FileSignature实例的字符串表达形式。</returns>
      public override string ToString()
      {
         return "Cabinink.IOSystem.FileSecurity.FileSignature";
      }
   }
}
