using System;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem.Security
{
   /// <summary>
   /// 文件系统安全访问规则管理，存放了一些用于重定义用户对某个文件或者目录的安全访问规则的方法。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class IOAccessRuleManagement
   {
      /// <summary>
      /// 更新某个用户访问指定文件的安全规则。
      /// </summary>
      /// <param name="fileUrl">需要添加访问规则的文件。</param>
      /// <param name="identity">用户帐户的名称，该参数必须标识当前机器或域上的有效帐户，并采用如下格式：DOMAIN\account。</param>
      /// <param name="updateMode">决定安全规则的更新方式。</param>
      /// <param name="rights">指定与访问规则关联的操作的类型。</param>
      /// <param name="controlType">值指定是允许还是拒绝该操作。</param>
      public static void UpdateFileAccessRule(string fileUrl, string identity, ERuleUpdateMode updateMode, FileSystemRights rights, AccessControlType controlType)
      {
         FileSecurity fSecurity = File.GetAccessControl(fileUrl);
         if (updateMode == ERuleUpdateMode.Append) fSecurity.AddAccessRule(new FileSystemAccessRule(identity, rights, controlType));
         else fSecurity.RemoveAccessRule(new FileSystemAccessRule(identity, rights, controlType));
         File.SetAccessControl(fileUrl, fSecurity);
      }
      /// <summary>
      /// 更新某个用户访问指定目录（文件夹）的安全规则，并指定继承标识。
      /// </summary>
      /// <param name="directory">需要添加访问规则的目录。</param>
      /// <param name="identify">用户帐户的名称，该参数必须标识当前机器或域上的有效帐户，并采用如下格式：DOMAIN\account。</param>
      /// <param name="updateMode">决定安全规则的更新方式。</param>
      /// <param name="controlSections">指定要保存或加载的安全描述符的哪些部分。</param>
      /// <param name="flags">安全访问规则的继承标识符，可以为多个，比如说可以传递InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit。</param>
      /// <param name="rights">指定与访问规则关联的操作的类型。</param>
      /// <param name="controlType">值指定是允许还是拒绝该操作。</param>
      public static void UpdateDirectoryAccessRule(string directory, string identify, ERuleUpdateMode updateMode, AccessControlSections controlSections, InheritanceFlags flags, FileSystemRights rights, AccessControlType controlType)
      {
         DirectoryInfo dInfo = new DirectoryInfo(directory);
         DirectorySecurity security = dInfo.GetAccessControl(controlSections);
         InheritanceFlags iFlags = flags;
         FileSystemAccessRule rule = new FileSystemAccessRule(identify, rights, iFlags, PropagationFlags.InheritOnly, controlType);
         if (updateMode == ERuleUpdateMode.Append) security.AddAccessRule(rule);
         else security.RemoveAccessRule(rule);
         dInfo.SetAccessControl(security);
      }
      /// <summary>
      /// 更改文件的所有者。
      /// </summary>
      /// <param name="fileUrl">需要更改所有者的文件。</param>
      /// <param name="domain">服务器或者本地计算机的名称。</param>
      /// <param name="ownerAccount">参数fileUrl指定文件的新所有者的账户名称。</param>
      public static void UpdateFileOwner(string fileUrl, string domain, string ownerAccount)
      {
         FileSecurity fSecurity = File.GetAccessControl(fileUrl);
         fSecurity.SetOwner(new NTAccount(domain, ownerAccount));
         File.SetAccessControl(fileUrl, fSecurity);
      }
      /// <summary>
      /// 更改目录的所有者。
      /// </summary>
      /// <param name="directory">需要更改所有者的目录。</param>
      /// <param name="domain">服务器或者本地计算机的名称。</param>
      /// <param name="ownerAccount">参数fileUrl指定目录的新所有者的账户名称。</param>
      public static void UpdateDirectoryOwner(string directory, string domain, string ownerAccount)
      {
         DirectorySecurity fSecurity = Directory.GetAccessControl(directory);
         fSecurity.SetOwner(new NTAccount(domain, ownerAccount));
         Directory.SetAccessControl(directory, fSecurity);
      }
   }
   /// <summary>
   /// 用于决定访问权限更新的模式的枚举。
   /// </summary>
   public enum ERuleUpdateMode : int
   {
      /// <summary>
      /// 追加访问权限。
      /// </summary>
      [EnumerationDescription("追加访问权限")]
      Append = 0x0000,
      /// <summary>
      /// 覆盖以前的访问权限（暂时作为保留性质的值）。
      /// </summary>
      [EnumerationDescription("覆盖以前的访问权限")]
      Cover = 0x00ff,
      /// <summary>
      /// 移除访问权限。
      /// </summary>
      [EnumerationDescription("移除访问权限")]
      Remove = 0xffff,
   }
}
