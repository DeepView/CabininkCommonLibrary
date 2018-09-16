using System;
using System.IO;
using Microsoft.Win32;
using Cabinink.IOSystem;
using System.Security.Permissions;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Applications
{
   /// <summary>
   /// Windows开机自启动帮助类，用于创建或者删除开机自启动应用程序。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class StartupHelper
   {
      private const string HKEY_STARTUP_SUBPATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";//用于注册表开机自启动的路径。
      /// <summary>
      /// 通过指定的应用程序路径在注册表中创建一个自启动项目。
      /// </summary>
      /// <param name="executableFileUrl">指定的应用程序路径。</param>
      /// <remarks>需要在调用方访问Cabinink.Windows.Privileges.PrivilegeGetter.NeedAdministratorsPrivilege()方法，然后才能调用此方法，否则将会抛出System.Security.SecurityException异常。</remarks>
      public static void CreateStartupWithRegistry(string executableFileUrl)
      {
         string assemblyName = FileOperator.GetFileNameWithoutExtension(executableFileUrl);
         CreateStartupWithRegistry(executableFileUrl, string.Empty, assemblyName, false);
      }
      /// <summary>
      /// 通过指定的应用程序路径和启动参数在注册表中创建一个自启动项目。
      /// </summary>
      /// <param name="executableFileUrl">指定的应用程序路径。</param>
      /// <param name="startupArguments">指定应用程序的启动参数。</param>
      /// <remarks>需要在调用方访问Cabinink.Windows.Privileges.PrivilegeGetter.NeedAdministratorsPrivilege()方法，然后才能调用此方法，否则将会抛出System.Security.SecurityException异常。</remarks>
      public static void CreateStartupWithRegistry(string executableFileUrl, string startupArguments)
      {
         string assemblyName = FileOperator.GetFileNameWithoutExtension(executableFileUrl);
         CreateStartupWithRegistry(executableFileUrl, startupArguments, assemblyName, false);
      }
      /// <summary>
      /// 通过指定的应用程序路径，启动参数，应用程序名称和全局自启动标识在注册表中创建一个自启动项目。
      /// </summary>
      /// <param name="executableFileUrl">指定的应用程序路径。</param>
      /// <param name="startupArguments">指定应用程序的启动参数。</param>
      /// <param name="assemblyName">指定的应用程序名称。</param>
      /// <param name="isGlobalStartup">全局自启动标识，用于决定是否将自启动项目添加到全局启动（HKEY_LOCAL_MACHINE）还是用户启动（HKEY_CURRENT_USER）中。</param>
      /// <remarks>需要在调用方访问Cabinink.Windows.Privileges.PrivilegeGetter.NeedAdministratorsPrivilege()方法，然后才能调用此方法，否则将会抛出System.Security.SecurityException异常。</remarks>
      /// <exception cref="FileNotFoundException">当指定的应用程序文件无法找到或者不存在时，则将会抛出这个异常。</exception>
      [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
      public static void CreateStartupWithRegistry(string executableFileUrl, string startupArguments, string assemblyName, bool isGlobalStartup)
      {
         if (!FileOperator.FileExists(executableFileUrl)) throw new FileNotFoundException("指定的可执行文件不存在！", executableFileUrl);
         RegistryKey registry = isGlobalStartup ? Registry.LocalMachine : Registry.CurrentUser;
         RegistryKey run = registry.CreateSubKey(HKEY_STARTUP_SUBPATH);
         executableFileUrl = "\x22" + executableFileUrl + "\x22";
         assemblyName = string.IsNullOrWhiteSpace(assemblyName) ? FileOperator.GetFileNameWithoutExtension(executableFileUrl) : assemblyName;
         run.SetValue(assemblyName, !string.IsNullOrWhiteSpace(startupArguments) ? executableFileUrl + " " + startupArguments : executableFileUrl);
         run.Close();
         registry.Close();
      }
      /// <summary>
      /// 通过指定的应用程序名称和全局自启动标识从注册表中删除一个自启动项目。
      /// </summary>
      /// <param name="assemblyName">指定的应用程序名称。</param>
      /// <param name="isGlobalStartup">全局自启动标识，用于决定将自启动项目从全局启动（HKEY_LOCAL_MACHINE）中删除还是用户启动（HKEY_CURRENT_USER）中删除。</param>
      public static void DeleteStartupWithRegistry(string assemblyName, bool isGlobalStartup)
      {
         RegistryKey registry = isGlobalStartup ? Registry.LocalMachine : Registry.CurrentUser;
         RegistryKey run = registry.CreateSubKey(HKEY_STARTUP_SUBPATH);
         run.DeleteValue(assemblyName, false);
         run.Close();
         registry.Close();
      }
      /// <summary>
      /// 通过指定的应用程序路径在启动目录中创建一个自启动项目。
      /// </summary>
      /// <param name="executableFileUrl">指定的应用程序路径。</param>
      public static void CreateStartupWithBootMenu(string executableFileUrl)
      {
         string assemblyName = FileOperator.GetFileNameWithoutExtension(executableFileUrl);
         CreateStartupWithBootMenu(executableFileUrl, assemblyName);
      }
      /// <summary>
      /// 通过指定的应用程序路径和应用程序名称在启动目录中创建一个自启动项目。
      /// </summary>
      /// <param name="executableFileUrl">指定的应用程序路径。</param>
      /// <param name="assemblyName">指定的应用程序名称或者是启动目录中这个自启动项目的快捷方式名称。</param>
      public static void CreateStartupWithBootMenu(string executableFileUrl, string assemblyName)
      {
         string bootMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
         assemblyName = string.IsNullOrWhiteSpace(assemblyName) ? FileOperator.GetFileNameWithoutExtension(executableFileUrl) : assemblyName;
         ShortcutCreator shortcutCreator = new ShortcutCreator(assemblyName, executableFileUrl)
         {
            Icon = executableFileUrl,
            SourceDirectory = FileOperator.GetFatherDirectory(executableFileUrl)
         };
         shortcutCreator.CreateLocalShortcut(bootMenuPath, EWindowStyle.NormalFocus);
      }
      /// <summary>
      /// 通过指定的应用程序名称从启动目录中删除一个自启动项目。
      /// </summary>
      /// <param name="assemblyName">指定的应用程序名称或者是启动目录中这个自启动项目的快捷方式名称。</param>
      public static void DeleteStartupWithBootMenu(string assemblyName)
      {
         string bootMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
         string willDeletedInkFile = bootMenuPath + "\\" + assemblyName + ".lnk";
         FileOperator.DeleteFile(willDeletedInkFile);
      }
   }
}
