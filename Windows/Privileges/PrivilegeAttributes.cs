using System;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Privileges
{
   /// <summary>
   /// 权限属性常量集合类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class PrivilegeAttributes
   {
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_ENABLED = 0x00000002;
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_REMOVED = 0x00000004;
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;
   }
}
