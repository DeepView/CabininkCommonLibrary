using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;
namespace Cabinink.Windows
{
   /// <summary>
   /// Windows本地用户和本地安全组管理，不过基本上所有的操作都需要在管理员权限下运行，否则将会出现异常。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class OSAccountManagement
   {
      private static readonly string _envirPath = "WinNT://" + Environment.MachineName;//环境变量，保存当前设备名称。
      /// <summary>
      /// 检索服务器上的所有用户帐户的信息。
      /// </summary>
      /// <param name="serverName">指向常量字符串的指针，该字符串指定要在其上执行该功能的远程服务器的DNS或NetBIOS名称。如果此参数为NULL，则使用本地计算机。</param>
      /// <param name="level">指定数据的信息级别。</param>
      /// <param name="filter">指定要包括在枚举中的用户帐户类型的值。值为零表示应包括所有普通用户，信任数据和计算机帐户数据。</param>
      /// <param name="buffer">指向接收数据的缓冲区的指针。此数据的格式取决于level参数的值。</param>
      /// <param name="prefMaxLength">返回数据的首选最大长度（以字节为单位）。如果指定MAX_PREFERRED_LENGTH，则NetUserEnum函数将分配数据所需的内存量。如果在此参数中指定了另一个值，则可以限制该函数返回的字节数。如果缓冲区大小不足以容纳所有条目，则该函数返回ERROR_MORE_DATA。</param>
      /// <param name="entriesRead">指向一个值的指针，它接收实际枚举的元素的数量。</param>
      /// <param name="totalEntries">指向一个值的指针，该值接收当前恢复位置可能枚举的条目总数。请注意，应用程序应仅将此值视为提示。如果您的应用程序与Windows 2000或更高版本的域控制器进行通信，则应考虑使用 ADSI LDAP提供程序更有效地检索此类型的数据。ADSI LDAP提供程序实现一组支持各种ADSI接口的ADSI对象。</param>
      /// <param name="resumeHandle">指向包含用于继续现有用户搜索的简历句柄的值的指针。句柄在第一个通话时应为零，对于后续调用保持不变。如果此参数为NULL，则不存储恢复句柄。</param>
      /// <returns>如果函数成功，返回值为0，否则为非0。</returns>
      [DllImport("Netapi32.dll")]
      private extern static int NetUserEnum([MarshalAs(UnmanagedType.LPWStr)] string serverName, int level, int filter, out IntPtr buffer, int prefMaxLength, out int entriesRead, out int totalEntries, out int resumeHandle);
      /// <summary>
      /// 释放NetApiBufferAllocate功能分配以及其他网络管理功能在内部使用的内存以返回信息。
      /// </summary>
      /// <param name="buffer">通过调用NetApiBufferAllocate函数分配的另一个网络管理功能或内存先前返回的缓冲区的指针。</param>
      /// <returns>如果函数成功，返回值为0，否则为非0。</returns>
      [DllImport("Netapi32.dll")]
      extern static int NetApiBufferFree(IntPtr buffer);
      /// <summary>
      /// 新建Windows用户。
      /// </summary>
      /// <param name="accountName">需要创建的Windows用户的用户名称。</param>
      /// <param name="password">这个用户的登录密码。</param>
      /// <param name="securityGroupName">决定这个被创建的用户隶属于哪一个用户安全组。</param>
      /// <param name="description">当前用户的详细注释。</param>
      /// <param name="isNextLoginMustChangedPassword">决定这个用户在下次登录时是否必须修改密码。</param>
      /// <param name="isCanChangedPassword">决定这个用户是否能够被修改密码。</param>
      /// <param name="isAlwaysNoOverdue">决定这个用户是否永不过期。</param>
      public static void CreateAccount(string accountName, string password, string securityGroupName, string description, bool isNextLoginMustChangedPassword, bool isCanChangedPassword, bool isAlwaysNoOverdue)
      {
         using (DirectoryEntry dir = new DirectoryEntry(_envirPath))
         {
            using (DirectoryEntry user = dir.Children.Add(accountName, "User"))
            {
               user.Properties["FullName"].Add(accountName);
               user.Invoke("SetPassword", password);
               user.Invoke("Put", "Description", description);
               if (isNextLoginMustChangedPassword) user.Invoke("Put", "PasswordExpired", 1);
               if (isAlwaysNoOverdue) user.Invoke("Put", "UserFlags", 66049);
               if (isCanChangedPassword) user.Invoke("Put", "UserFlags", 0x0040);
               user.CommitChanges();
               using (DirectoryEntry grp = dir.Children.Find(securityGroupName, "group"))
               {
                  if (grp.Name != "")
                  {
                     grp.Invoke("Add", user.Path.ToString());
                  }
               }
            }
         }
      }
      /// <summary>
      /// 更新指定Windows用户的登录密码。
      /// </summary>
      /// <param name="accountName">需要被修改登录密码的Windows用户。</param>
      /// <param name="newPassword">修改之后的登录密码。</param>
      public static void UpdateAccountPassword(string accountName, string newPassword)
      {
         using (DirectoryEntry dir = new DirectoryEntry(_envirPath))
         {
            using (DirectoryEntry user = dir.Children.Find(accountName, "user"))
            {
               user.Invoke("SetPassword", new object[] { newPassword });
               user.CommitChanges();
            }
         }
      }
      /// <summary>
      /// 从操作系统中移除指定的Windows用户。
      /// </summary>
      /// <param name="accountName">需要被移除的Windows用户。</param>
      public static void RemoveAccount(string accountName)
      {
         using (DirectoryEntry dir = new DirectoryEntry(_envirPath))
         {
            using (DirectoryEntry user = dir.Children.Find(accountName, "User"))
            {
               dir.Children.Remove(user);
            }
         }
      }
      /// <summary>
      /// 新建Windows用户安全组。
      /// </summary>
      /// <param name="securityGroupName">需要被创建的Windows用户安全组。</param>
      /// <param name="description">当前用户安全组的详细注释。</param>
      public static void CreateSecurityGroup(string securityGroupName, string description)
      {
         using (DirectoryEntry dir = new DirectoryEntry(_envirPath))
         {
            using (DirectoryEntry group = dir.Children.Add(securityGroupName, "group"))
            {
               group.Invoke("Put", new object[] { "Description", description });
               group.CommitChanges();
            }
         }
      }
      /// <summary>
      /// 从操作系统中移除指定的Windows用户安全组。
      /// </summary>
      /// <param name="securityGroupName">需要被移出的Windows用户安全组。</param>
      public static void RemoveSecurityGroup(string securityGroupName)
      {
         using (DirectoryEntry dir = new DirectoryEntry(_envirPath))
         {
            using (DirectoryEntry group = dir.Children.Find(securityGroupName, "Group"))
            {
               dir.Children.Remove(group);
            }
         }
      }
   }
}
