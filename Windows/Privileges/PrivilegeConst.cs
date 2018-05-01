using System;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Privileges
{
   /// <summary>
   /// Windows操作系统权限常量集合类，其中包含了几乎所有的Windows操作系统权限字符串。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class PrivilegeConst
   {
      /// <summary>
      /// 替换进程级记号，允许初始化一个进程，以取代与已启动的子进程相关的默认令牌。
      /// </summary>
      public const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
      /// <summary>
      /// 产生安全审核，允许将条目添加到安全日志。
      /// </summary>
      public const string SE_AUDIT_NAME = "SeAuditPrivilege";
      /// <summary>
      /// 需要执行备份操作，这种特权使系统能够将所有读访问控制授予任何文件，而不考虑为文件指定的访问控制列表(ACL)。
      /// </summary>
      public const string SE_BACKUP_NAME = "SeBackupPrivilege";
      /// <summary>
      /// 跳过遍历检查，允许用户来回移动目录，但是不能列出文件夹的内容。
      /// </summary>
      public const string SE_CHANGE_NOTIFY_NAME = "SeChangeNotifyPrivilege";
      /// <summary>
      /// 在终端服务会话期间，在全局命名空间中创建命名文件映射对象的要求，默认情况下，管理员、服务和本地系统帐户启用此特权。
      /// </summary>
      public const string SE_CREATE_GLOBAL_NAME = "SeCreateGlobalPrivilege";
      /// <summary>
      /// 创建页面文件，允许用户创建和改变一个分页文件的大小。
      /// </summary>
      public const string SE_CREATE_PAGEFILE_NAME = "SeCreatePagefilePrivilege";
      /// <summary>
      /// 创建永久共享对象。
      /// </summary>
      public const string SE_CREATE_PERMANENT_NAME = "SeCreatePermanentPrivilege";
      /// <summary>
      /// 需要创建一个符号链接。
      /// </summary>
      public const string SE_CREATE_SYMBOLIC_LINK_NAME = "SeCreateSymbolicLinkPrivilege";
      /// <summary>
      /// 创建令牌对象，允许进程调用NtCreateToken()或者是其他的Token。
      /// </summary>
      public const string SE_CREATE_TOKEN_NAME = "SeCreateTokenPrivilege";
      /// <summary>
      /// 需要调试和调整另一个帐户拥有的进程的内存。
      /// </summary>
      public const string SE_DEBUG_NAME = "SeDebugPrivilege";
      /// <summary>
      /// 需要将用户和计算机帐户标记为委托授权。
      /// </summary>
      public const string SE_ENABLE_DELEGATION_NAME = "SeEnableDelegationPrivilege";
      /// <summary>
      /// 在身份验证后模拟客户端。
      /// </summary>
      public const string SE_IMPERSONATE_NAME = "SeImpersonatePrivilege";
      /// <summary>
      /// 更改优先级时，只有获得此权限后才能设置进程优先级为“实时”。
      /// </summary>
      public const string SE_INC_BASE_PRIORITY_NAME = "SeIncreaseBasePriorityPrivilege";
      /// <summary>
      /// 调整进程的内存配额。
      /// </summary>
      public const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
      /// <summary>
      /// 需要为在用户环境中运行的应用程序分配更多内存。
      /// </summary>
      public const string SE_INC_WORKING_SET_NAME = "SeIncreaseWorkingSetPrivilege";
      /// <summary>
      /// 装载和卸载设备驱动程序，允许动态地加载和卸载设备驱动程序，安装即插即用设备的驱动程序时需要此特权。
      /// </summary>
      public const string SE_LOAD_DRIVER_NAME = "SeLoadDriverPrivilege";
      /// <summary>
      /// 内存中锁定页，允许使用进程在物理内存中保存数据，从而避免系统将这些数据分页保存到磁盘的虚拟内存中.采用此策略会减少可用的随机存取内存(RAM)总数，从而可能极大地影响系统性能。
      /// </summary>
      public const string SE_LOCK_MEMORY_NAME = "SeLockMemoryPrivilege";
      /// <summary>
      /// 域中添加工作站，用于识别Active Directory中已有的帐户和组。
      /// </summary>
      public const string SE_MACHINE_ACCOUNT_NAME = "SeMachineAccountPrivilege";
      /// <summary>
      /// 需要启用卷管理特权。
      /// </summary>
      public const string SE_MANAGE_VOLUME_NAME = "SeManageVolumePrivilege";
      /// <summary>
      /// 配置单一进程，允许使用性能监视工具来监视非系统进程的性能。
      /// </summary>
      public const string SE_PROF_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
      /// <summary>
      /// 需要修改对象的强制完整性级别。
      /// </summary>
      public const string SE_RELABEL_NAME = "SeRelabelPrivilege";
      /// <summary>
      /// 从远端系统强制关机，允许从网络上的远程位置关闭计算机。
      /// </summary>
      public const string SE_REMOTE_SHUTDOWN_NAME = "SeRemoteShutdownPrivilege";
      /// <summary>
      /// 还原文件和目录，允许绕过文件及目录权限来恢复备份文件。
      /// </summary>
      public const string SE_RESTORE_NAME = "SeRestorePrivilege";
      /// <summary>
      /// 管理审核和安全日志，允许指定文件，Active Directory对象和注册表项之类的单个资源的对象访问审核选项，还可以查看和清除安全日志。
      /// </summary>
      public const string SE_SECURITY_NAME = "SeSecurityPrivilege";
      /// <summary>
      /// 需要关闭本地系统。
      /// </summary>
      public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
      /// <summary>
      /// 域控制器需要使用轻量级目录访问协议目录同步服务，此特权使持有者可以读取目录中的所有对象和属性，而不考虑对象和属性的保护，默认情况下，它被分配给域控制器上的管理员和本地系统帐户。
      /// </summary>
      public const string SE_SYNC_AGENT_NAME = "SeSyncAgentPrivilege";
      /// <summary>
      /// 修改固件环境值，查看，修改环境变量SET命令。
      /// </summary>
      public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
      /// <summary>
      /// 配置系统性能，允许监视系统进程的性能。
      /// </summary>
      public const string SE_SYSTEM_PROFILE_NAME = "SeSystemProfilePrivilege";
      /// <summary>
      /// 需要修改系统时间。
      /// </summary>
      public const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";
      /// <summary>
      /// 获得文件或对象的所有权，包括 Active Directory 对象，文件和文件夹，打印机，注册表项，进程和线程。
      /// </summary>
      public const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
      /// <summary>
      /// 此特权将其持有者标识为可信计算机基础的一部分，一些受信任的受保护子系统被授予此特权。
      /// </summary>
      public const string SE_TCB_NAME = "SeTcbPrivilege";
      /// <summary>
      /// 需要调整与计算机内部时钟相关联的时区。
      /// </summary>
      public const string SE_TIME_ZONE_NAME = "SeTimeZonePrivilege";
      /// <summary>
      /// 需要访问凭据管理器作为可信调用者。
      /// </summary>
      public const string SE_TRUSTED_CREDMAN_ACCESS_NAME = "SeTrustedCredManAccessPrivilege";
      /// <summary>
      /// 需要打开笔记本电脑。
      /// </summary>
      public const string SE_UNDOCK_NAME = "SeUndockPrivilege";
      /// <summary>
      /// 需要从终端设备读取未经请求的输入。
      /// </summary>
      public const string SE_UNSOLICITED_INPUT_NAME = "SeUnsolicitedInputPrivilege";
   }
}
