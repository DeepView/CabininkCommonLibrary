using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;
namespace Cabinink.Windows
{
   [Serializable]
   [ComVisible(true)]
   public sealed class SystemUserManagement
   {
      private static readonly string _envirPath = "WinNT://" + Environment.MachineName;//环境变量，保存当前设备名称。
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
