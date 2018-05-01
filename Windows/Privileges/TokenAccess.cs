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
      /// <summary>
      /// 请求特权，组合DELETE、READ_CONTROL、WRITE_DAC和WRITE_OWNER访问。
      /// </summary>
      [CLSCompliant(false)]
      public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
      /// <summary>
      /// 读取特权，当前定义等价于READ_CONTROL。
      /// </summary>
      [CLSCompliant(false)]
      public const uint STANDARD_RIGHTS_READ = 0x00020000;
      /// <summary>
      /// 写入特权，当前定义等价于READ_CONTROL。
      /// </summary>
      [CLSCompliant(false)]
      public const uint STANDARD_RIGHTS_WRITE = 0x00020000;
      /// <summary>
      /// 需要将一个主标记附加到一个进程，完成此任务还需要SE_ASSIGNPRIMARYTOKEN_NAME特权。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
      /// <summary>
      /// 需要复制访问令牌。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_DUPLICATE = 0x0002;
      /// <summary>
      /// 需要将模拟访问令牌附加到流程。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_IMPERSONATE = 0x0004;
      /// <summary>
      /// 需要查询访问令牌。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_QUERY = 0x0008;
      /// <summary>
      /// 需要查询访问令牌的源。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_QUERY_SOURCE = 0x0010;
      /// <summary>
      /// 需要启用或禁用访问令牌中的特权。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
      /// <summary>
      /// 需要在访问令牌中调整组的属性。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_GROUPS = 0x0040;
      /// <summary>
      /// 需要更改访问令牌的默认所有者、主组或DACL。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
      /// <summary>
      /// 需要调整访问令牌的会话ID。需要使用SE_TCB_NAME特权。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_ADJUST_SESSIONID = 0x0100;
      /// <summary>
      /// 结合了STANDARD_RIGHTS_READ和TOKEN_QUERY。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
      /// <summary>
      /// 结合了STANDARD_RIGHTS_WRITE，TOKEN_ADJUST_PRIVILEGES，TOKEN_ADJUST_GROUPS和TOKEN_ADJUST_DEFAULT
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_WRITE = (STANDARD_RIGHTS_WRITE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT);
      /// <summary>
      /// 将所有可能的访问权限合并为一个令牌。
      /// </summary>
      [CLSCompliant(false)]
      public const uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
          TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
          TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID);
   }
}
