﻿using System;
using System.IO;
using System.Text;
using Cabinink.Windows;
using System.Diagnostics;
using Cabinink.TypeExtend;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem.Security
{
   /// <summary>
   /// IO操作安全文件类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   [DebuggerDisplay("IOSecurityFile = FileUrl:{FileUrl};SecurityFlag:{SecurityFlag.ToString()}")]
   public class IOSecurityFile : IFileOperatingSecurity, IExampledObjectFileBaseIO, ISaveAsUnencryptedCopy, IEquatable<IOSecurityFile>
   {
      private string _securityFileUrl;//IO操作安全文件的文件路径。
      private ExString _fileContext;//IO操作安全文件的文件内容。
      private ExString _jurisdictionPassword;//适用于操作安全的权限密码。
      private EFileSecurityFlag _securityFlag;//文件操作安全标识符。
      private int _codeSecurityFlag;//代码安全标识符。
      private bool _isApplyAccessRule;//指示是否应用文件安全访问规则。
      private const int CODE_SECURITY_FLAG_STOP = 0x0000;//代码安全标识符，操作非法。
      private const int CODE_SECURITY_FLAG_NORMAL = 0xffff;//代码安全标识符，操作合法。
      private const string FILE_SECURITY_KEY = @"cabinink";//文件加密和解密用的安全密钥。
      /// <summary>
      /// 构造函数，创建一个指定文件路径的IO操作安全文件操作实例。
      /// </summary>
      /// <param name="fileUrl">用于被操作的IO操作安全文件的文件地址。</param>
      /// <exception cref="FileNotFoundException">当参数fileUrl指定的文件找不到时，则会抛出这个异常。</exception>
      public IOSecurityFile(string fileUrl)
      {
         if (!FileOperator.FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到！", fileUrl);
         ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
         _securityFileUrl = fileUrl;
         _fileContext = string.Empty;
         _isApplyAccessRule = true;
      }
      /// <summary>
      /// 构造函数，创建一个指定文件路径的IO操作安全文件操作实例，并根据参数createdThenNotExists决定当文件不存在时是否创建新文件。
      /// </summary>
      /// <param name="fileUrl">用于被操作的IO操作安全文件的文件地址，如果这个文件不存在，会重新创建一个新的文件地址为当前参数的文件。</param>
      /// <param name="createdThenNotExists">用于决定是否在检测到文件不存在时来创建新文件。</param>
      /// <exception cref="FileNotFoundException">当参数fileUrl指定的文件找不到，并且不允许根据参数createdThenNotExists决定当文件不存在时是否创建新文件的情况下，则会抛出这个异常。</exception>
      public IOSecurityFile(string fileUrl, bool createdThenNotExists)
      {
         if (createdThenNotExists)
         {
            if (!FileOperator.FileExists(fileUrl))
            {
               ExString ciphertext = ExString.Encrypt(string.Empty, FILE_SECURITY_KEY);
               FileOperator.CreateFile(fileUrl);
               FileOperator.WriteFile(fileUrl, ciphertext, false);
            }
         }
         else
         {
            if (!FileOperator.FileExists(fileUrl)) throw new FileNotFoundException("指定的文件找不到！", fileUrl);
         }
         ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
         _securityFileUrl = fileUrl;
         _fileContext = string.Empty;
         _isApplyAccessRule = true;
      }
      /// <summary>
      /// 获取或设置当前实例的IO操作安全文件地址。
      /// </summary>
      /// <exception cref="FileNotFoundException">当参数fileUrl指定的文件找不到时，则会抛出这个异常。</exception>
      public string FileUrl
      {
         get => _securityFileUrl;
         set
         {
            if (!FileOperator.FileExists(value)) throw new FileNotFoundException("指定的文件找不到！", value);
            _securityFileUrl = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例的文件内容上下文。
      /// </summary>
      /// <exception cref="CodeSecurityNotMatchException">当代码操作在允许的构造逻辑或者操作安全范围外时，则会抛出这个异常。</exception>
      public string FileContext
      {
         get => _fileContext;
         set
         {
            if (_codeSecurityFlag == CODE_SECURITY_FLAG_STOP) throw new CodeSecurityNotMatchException();
            _fileContext = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例的IO权限密码。
      /// </summary>
      /// <exception cref="CodeSecurityNotMatchException">当代码操作在允许的构造逻辑或者操作安全范围外时，则会抛出这个异常。</exception>
      public ExString JurisdictionPassword
      {
         get => _jurisdictionPassword;
         set
         {
            if (_codeSecurityFlag == CODE_SECURITY_FLAG_STOP) throw new CodeSecurityNotMatchException();
            else _jurisdictionPassword = value;
         }
      }
      /// <summary>
      /// 获取当前实例的安全标识符。
      /// </summary>
      public EFileSecurityFlag SecurityFlag => _securityFlag;
      /// <summary>
      /// 获取或设置当前实例是否应用Windows文件安全访问规则。
      /// </summary>
      public bool IsApplyAccessRule { get => _isApplyAccessRule; set => _isApplyAccessRule = value; }
      /// <summary>
      /// 加载当前文件的IO操作安全文件密码，这个方法不指定具体实现，需要通过开发者完成，因为当前方法无法定义密码存储源是哪一种文件类型或者存储方式。
      /// </summary>
      /// <param name="getPasswordFunction">用于加载当前文件的IO操作安全密码的委托，值得注意的是，该委托的返回值必须是一个从逻辑上正确的文件IO安全密码（毕竟该方法无法直接判断密码的有效性和正确性），因为这个委托的Result会赋值给JurisdictionPassword属性。</param>
      public void LoadPassword(Func<string> getPasswordFunction)
      {
         ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
         JurisdictionPassword = getPasswordFunction.Invoke();
         ResetCodeSecurityFlag();
      }
      /// <summary>
      /// 从数据库或者文件中加载当前文件的IO操作安全文件密码，这个方法不指定具体实现，需要通过开发者完成，不过该方法可以模糊定义密码来源的代码规范，比如说一个数据库（需要指定一个数据库连接字符串）或者一个文本文件（需要指定一个有效的文件路径）。
      /// </summary>
      /// <param name="getPasswordFromDbOrFile">用于加载当前文件的IO操作安全密码的带参数委托，值得注意的是，该委托的返回值必须是一个从逻辑上正确的文件IO安全密码（毕竟该方法无法直接判断密码的有效性和正确性），因为这个委托的Result会赋值给JurisdictionPassword属性。</param>
      /// <param name="passwordMemoriedDbConnStrOrFileUrl">用于传递getPasswordFromDbOrFile委托中从逻辑上有效的字符串参数，这个参数在这里可以作为一个数据库连接字符串，也可以作为一个文本文件的地址或者作为一个OLEDB的数据库文件地址等等。</param>
      public void LoadPassword(Func<string, string> getPasswordFromDbOrFile, string passwordMemoriedDbConnStrOrFileUrl)
      {
         if (string.IsNullOrWhiteSpace(passwordMemoriedDbConnStrOrFileUrl))
         {
            throw new NullReferenceException("不允许该参数为空或者为空白，因为这个参数决定了加载密码的密码存储源！");
         }
         ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
         JurisdictionPassword = getPasswordFromDbOrFile.Invoke(passwordMemoriedDbConnStrOrFileUrl);
         ResetCodeSecurityFlag();
      }
      /// <summary>
      /// 清除当前实例的IO权限密码。
      /// </summary>
      /// <param name="password">在清除密码之前需要进行身份验证的密码。</param>
      /// <exception cref="PasswordNotMatchException">当源密码与目标密码不匹配时，则会抛出这个异常。</exception>
      public void ClearPassword(ExString password)
      {
         if (!JurisdictionPassword.Equals(password)) throw new PasswordNotMatchException();
         else
         {
            ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
            JurisdictionPassword = string.Empty;
            ResetCodeSecurityFlag();
         }
      }
      /// <summary>
      /// 按照默认的方式读取IO操作安全文件的文件内容。
      /// </summary>
      /// <returns>该操作如果不存在异常抛出，则会返回所操作文件的文件内容。</returns>
      public void Read() => Read(Encoding.GetEncoding("GB2312"));
      /// <summary>
      /// 通过指定的编码方式来读取IO操作安全文件的文件内容。
      /// </summary>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件读取的编码方式。</param>
      /// <returns>该操作如果不存在异常抛出，则会返回所操作文件的文件内容。</returns>
      /// <exception cref="CodeSecurityNotMatchException">当代码操作在允许的构造逻辑或者操作安全范围外时，则会抛出这个异常。</exception>
      public void Read(Encoding encoding)
      {
         if (_codeSecurityFlag == CODE_SECURITY_FLAG_STOP) throw new CodeSecurityNotMatchException();
         FileContext = ExString.Decrypt(FileOperator.ReadFileContext(FileUrl, true, encoding), FILE_SECURITY_KEY);
      }
      /// <summary>
      /// 创建并保存一个当前文件的内容未加密的文件副本。
      /// </summary>
      /// <param name="fileUrl">文件副本的地址。</param>
      /// <param name="encoding">文件内容需要采用的编码格式。</param>
      /// <exception cref="FileIsExistedException">如果参数fileUrl指定的文件地址存在时，则会抛出这个异常。</exception>
      public void SaveAsUnencryptedCopy(string fileUrl, Encoding encoding)
      {
         if (FileOperator.FileExists(fileUrl)) throw new FileIsExistedException();
         else
         {
            FileOperator.CreateFile(fileUrl, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            FileOperator.WriteFile(fileUrl, FileContext, false, encoding);
         }
      }
      /// <summary>
      /// 撤销当前实例的IO操作权限。
      /// </summary>
      /// <returns>用于说明当前操作是否成功，如果为true则表示操作正常且成功，反之操作失败。</returns>
      public bool RevokeJurisdiction()
      {
         bool result = true;
         string domain = EnvironmentInformation.GetComputerName();
         string usrname = domain + @"\" + EnvironmentInformation.GetCurrentUserName();
         try
         {
            ResetCodeSecurityFlag();
            if (IsApplyAccessRule)
            {
               ERuleUpdateMode append = ERuleUpdateMode.Append;
               ERuleUpdateMode remove = ERuleUpdateMode.Remove;
               IOAccessRuleManagement.UpdateFileAccessRule(FileUrl, usrname, remove, FileSystemRights.FullControl, AccessControlType.Allow);
               IOAccessRuleManagement.UpdateFileAccessRule(FileUrl, usrname, append, FileSystemRights.FullControl, AccessControlType.Deny);
            }
         }
         catch (Exception ex)
         {
            if (ex != null) result = false;
         }
         return result;
      }
      /// <summary>
      /// 恢复当前实例的IO操作权限，但必须需要提供权限密码进行身份验证，如果验证通过，才会恢复操作权限。
      /// </summary>
      /// <param name="password">在进行权限恢复之前需要进行身份验证的有效密码。</param>
      /// <returns>用于说明当前操作是否成功，如果为true则表示操作正常且成功，反之操作失败。</returns>
      public bool RrecoveryJurisdiction(ExString password)
      {
         bool result = true;
         string domain = EnvironmentInformation.GetComputerName();
         string usrname = domain + @"\" + EnvironmentInformation.GetCurrentUserName();
         if (JurisdictionPassword.Equals(password))
         {
            ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
            if (IsApplyAccessRule)
            {
               ERuleUpdateMode append = ERuleUpdateMode.Append;
               ERuleUpdateMode remove = ERuleUpdateMode.Remove;
               IOAccessRuleManagement.UpdateFileAccessRule(FileUrl, usrname, remove, FileSystemRights.FullControl, AccessControlType.Deny);
               IOAccessRuleManagement.UpdateFileAccessRule(FileUrl, usrname, append, FileSystemRights.FullControl, AccessControlType.Allow);
            }
         }
         else result = false;
         return result;
      }
      /// <summary>
      /// 更新用于操作当前实例的IO权限密码。
      /// </summary>
      /// <param name="oldPassword">需要用户提供的旧密码。</param>
      /// <param name="newPassword">需要用户设置的新密码。</param>
      /// <returns>用于说明当前操作是否成功，如果为true则表示操作正常且成功，反之操作失败。</returns>
      public bool UpdatePassword(ExString oldPassword, ExString newPassword)
      {
         bool result = true;
         if (!JurisdictionPassword.Equals(oldPassword)) result = false;
         else
         {
            ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_NORMAL);
            JurisdictionPassword = newPassword;
            ResetCodeSecurityFlag();
         }
         return result;
      }
      /// <summary>
      /// 向IO操作安全文件中存取指定的需要存取的文件内容。
      /// </summary>
      public void Write() => Write(false);
      /// <summary>
      /// 通过指定的IO操作安全文件内容存取方式来存取文件内容。
      /// </summary>
      /// <param name="isAppend">用于决定文件内容的存取方式，如果这个参数值为true，则意味着该操作将会以追加的方式把文本内容追加到文件末尾，反之将会以覆盖原本内容的方式存取文件。</param>
      public void Write(bool isAppend) => Write(isAppend, Encoding.GetEncoding("GB2312"));
      /// <summary>
      /// 通过指定的IO操作安全文件编码方式来存取文件内容。
      /// </summary>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件存取的编码方式。</param>
      public void Write(Encoding encoding) => Write(false, encoding);
      /// <summary>
      /// 通过指定的IO操作安全文件内容存取方式和编码方式来存取文件内容。
      /// </summary>
      /// <param name="isAppend">用于决定文件内容的存取方式，如果这个参数值为true，则意味着该操作将会以追加的方式把文本内容追加到文件末尾，反之将会以覆盖原本内容的方式存取文件。</param>
      /// <param name="encoding">指定的编码方式，这个编码决定了文件存取的编码方式</param>
      /// <exception cref="CodeSecurityNotMatchException">当代码操作在允许的构造逻辑或者操作安全范围外时，则会抛出这个异常。</exception>
      public void Write(bool isAppend, Encoding encoding)
      {
         if (_codeSecurityFlag == CODE_SECURITY_FLAG_STOP) throw new CodeSecurityNotMatchException();
         string ciphertext = ExString.Encrypt(FileContext, FILE_SECURITY_KEY);
         FileOperator.WriteFile(FileUrl, ciphertext, isAppend, encoding);
      }
      /// <summary>
      /// 变更代码安全标识符。
      /// </summary>
      /// <param name="flag">用于被变更的代码安全标识符。</param>
      /// <exception cref="ArgumentOutOfRangeException">当参数超出范围时，则会抛出这个异常。</exception>
      private void ChangeCodeSecurityFlag(int flag)
      {
         _codeSecurityFlag = flag;
         if (_codeSecurityFlag == CODE_SECURITY_FLAG_NORMAL) _securityFlag = EFileSecurityFlag.OperationIsAuthorized;
         else _securityFlag = EFileSecurityFlag.FileIsLocked;
      }
      /// <summary>
      /// 复位代码安全标识符。
      /// </summary>
      private void ResetCodeSecurityFlag() => ChangeCodeSecurityFlag(CODE_SECURITY_FLAG_STOP);
      /// <summary>
      /// 通过文件MD5和文件路径判断两个文件是否相同。
      /// </summary>
      /// <param name="other">用于比较的另一个文件。</param>
      /// <returns>如果两个文件的文件路径和MD5都相同，则操作的这两个文件属于同一个文件，那么这个操作就会返回true，否则将会返回false。</returns>
      public virtual bool Equals(IOSecurityFile other)
      {
         bool isEqual = false;
         FileSignature fSignature = new FileSignature(FileUrl);
         FileSignature otherFSignature = new FileSignature(other.FileUrl);
         if (fSignature.GetMD5String().Equals(otherFSignature.GetMD5String()) && FileUrl.Equals(other.FileUrl)) isEqual = true;
         return isEqual;
      }
   }
   /// <summary>
   /// 安全标志不匹配或者出现在代码安全范围之外的操作时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class CodeSecurityNotMatchException : Exception
   {
      public CodeSecurityNotMatchException() : base("不允许在代码安全范围外进行操作！") { }
      public CodeSecurityNotMatchException(string message) : base(message) { }
      public CodeSecurityNotMatchException(string message, Exception inner) : base(message, inner) { }
      protected CodeSecurityNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当密码不匹配时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class PasswordNotMatchException : Exception
   {
      public PasswordNotMatchException() : base("源密码与目标密码不匹配！") { }
      public PasswordNotMatchException(string message) : base(message) { }
      public PasswordNotMatchException(string message, Exception inner) : base(message, inner) { }
      protected PasswordNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}