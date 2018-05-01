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
      /// <summary>
      /// 特权默认启用。
      /// </summary>
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
      /// <summary>
      /// 特权启用。
      /// </summary>
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_ENABLED = 0x00000002;
      /// <summary>
      /// 特权被移除。
      /// </summary>
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_REMOVED = 0x00000004;
      /// <summary>
      /// 特权被用来访问一个对象或服务。这个标志被用于标识有关特权，因为通过一组客户端应用程序，可能包含不必要的特权。
      /// </summary>
      [CLSCompliant(false)]
      public const uint SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;
   }
}
