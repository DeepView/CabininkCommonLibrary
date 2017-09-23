using Cabinink.TypeExtend;
namespace Cabinink.IOSystem.Security
{
   /// <summary>
   /// 文件操作安全接口。
   /// </summary>
   public interface IFileOperatingSecurity
   {
      /// <summary>
      /// 获取或设置当前实例的权限密码。
      /// </summary>
      ExString JurisdictionPassword { get; set; }
      /// <summary>
      /// 获取当前实例的安全标识符。
      /// </summary>
      EFileSecurityFlag SecurityFlag { get; }
      /// <summary>
      /// 撤销当前实例的IO操作权限。
      /// </summary>
      /// <returns>用于说明当前操作是否成功，如果为true则表示操作正常且成功，反之操作失败。</returns>
      bool RevokeJurisdiction();
      /// <summary>
      /// 恢复当前实例的IO操作权限，但必须需要提供权限密码进行身份验证，如果验证通过，才会恢复操作权限。
      /// </summary>
      /// <param name="password">在进行权限恢复之前需要进行身份验证的有效密码。</param>
      /// <returns>用于说明当前操作是否成功，如果为true则表示操作正常且成功，反之操作失败。</returns>
      bool RrecoveryJurisdiction(ExString password);
      /// <summary>
      /// 更新用于操作当前实例的IO权限密码。
      /// </summary>
      /// <param name="oldPassword">需要用户提供的旧密码。</param>
      /// <param name="newPassword">需要用户设置的新密码。</param>
      /// <returns>用于说明当前操作是否成功，如果为true则表示操作正常且成功，反之操作失败。</returns>
      bool UpdatePassword(ExString oldPassword, ExString newPassword);
      /// <summary>
      /// 清除当前实例IO权限操作的密码。
      /// </summary>
      /// <param name="password">在清除密码之前需要进行身份验证的密码。</param>
      void ClearPassword(ExString password);
   }
   /// <summary>
   /// 文件操作安全标识符枚举。
   /// </summary>
   public enum EFileSecurityFlag : int
   {
      /// <summary>
      /// 操作被授权允许。
      /// </summary>
      OperationIsAuthorized = 0x0000,
      /// <summary>
      /// 文件被锁定，无法操作。
      /// </summary>
      FileIsLocked = 0xffff
   }
}
