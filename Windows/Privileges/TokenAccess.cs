using System;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Privileges
{
   /// <summary>
   /// 权限令牌通道常量集合类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class TokenAccess
   {
      [CLSCompliant(false)]
      public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
      [CLSCompliant(false)]
      public const uint STANDARD_RIGHTS_READ = 0x00020000;
      [CLSCompliant(false)]
      public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
      [CLSCompliant(false)]
      public const uint TOKEN_DUPLICATE = 0x0002;
      [CLSCompliant(false)]
      public const uint TOKEN_IMPERSONATE = 0x0004;
      [CLSCompliant(false)]
      public const uint TOKEN_QUERY = 0x0008;
      [CLSCompliant(false)]
      public const uint TOKEN_QUERY_SOURCE = 0x0010;
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_GROUPS = 0x0040;
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_SESSIONID = 0x0100;
      [CLSCompliant(false)]
      public const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
      [CLSCompliant(false)]
      public const uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
          TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
          TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
          TOKEN_ADJUST_SESSIONID);
   }
}
