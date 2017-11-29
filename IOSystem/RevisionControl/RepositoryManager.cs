using System;
using System.IO;
using Cabinink.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cabinink.IOSystem.Security;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem.RevisionControl
{
   /// <summary>
   /// 本地文件仓库管理器。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class RepositoryManager : IObjectAccessSwitch
   {
      private string _repoName;//文件仓库名称。
      private string _description;//仓库详细信息。
      private string _projectDirectory;//项目目录所在路径。
      private string _localRepoDirectory;//本地仓库所在路径。
      private ERepositoryStatue _statue;//仓库状态。
      /// <summary>
      /// 构造函数，创建一个指定仓库名称和注释的仓库管理器。
      /// </summary>
      /// <param name="repositoryName">指定的仓库名称。</param>
      /// <param name="description">当前仓库的注释。</param>
      /// <exception cref="ArgumentNullException">当仓库名称字符串长度为0或者为null时，则会抛出这个异常。</exception>
      public RepositoryManager(string repositoryName, string description)
      {
         if (string.IsNullOrWhiteSpace(repositoryName)) throw new ArgumentNullException("不允许空参数或者长度为0的字符串！");
         _repoName = repositoryName;
         _description = description;
         _projectDirectory = string.Empty;
         _localRepoDirectory = _projectDirectory;
      }
      /// <summary>
      /// 构造函数，创建一个指定仓库名称、注释、项目目录和本地仓库存储目录的仓库管理器。
      /// </summary>
      /// <param name="repositoryName">指定的仓库名称。</param>
      /// <param name="description">当前仓库的注释。</param>
      /// <param name="projectDirectory">版本控制器对应的项目所在目录。</param>
      /// <param name="localRepositoryDirectory">版本控制器本地仓库目录。</param>
      /// <exception cref="ArgumentNullException">当仓库名称字符串长度为0或者为null时，则会抛出这个异常。</exception>
      /// <exception cref="DirectoryNotFoundException">当指定的项目所在目录不存在时，则会抛出这个异常。</exception>
      public RepositoryManager(string repositoryName, string description, string projectDirectory, string localRepositoryDirectory)
      {
         if (string.IsNullOrWhiteSpace(repositoryName)) throw new ArgumentNullException("不允许空参数或者长度为0的字符串！");
         if (!FileOperator.DirectoryExists(projectDirectory)) throw new DirectoryNotFoundException("指定的项目目录不存在！");
         else
         {
            _repoName = repositoryName;
            _description = description;
            _projectDirectory = projectDirectory;
            _localRepoDirectory = localRepositoryDirectory;
         }
      }
      /// <summary>
      /// 获取或设置当前仓库的名称。
      /// </summary>
      public string Name { get => _repoName; set => _repoName = value; }
      /// <summary>
      /// 获取或设置当前实例的仓库注释信息。
      /// </summary>
      public string Description { get => _description; set => _description = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的仓库所对应的本地项目路径。
      /// </summary>
      public string ProjectDirectory { get => _projectDirectory; set => _projectDirectory = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的仓库的仓库目录。
      /// </summary>
      public string LocalRepositoryDirectory { get => _localRepoDirectory; set => _localRepoDirectory = value; }
      /// <summary>
      /// 获取或设置(private权限)当前实例的仓库状态。
      /// </summary>
      public ERepositoryStatue Statue { get => _statue; private set => _statue = value; }
      /// <summary>
      /// 执行当前仓库的初始化操作以及相关准备。
      /// </summary>
      /// <exception cref="DirectoryIsExistedException">当需要创建的仓库目录已存在时，则会抛出这个异常。</exception>
      public void Initialize()
      {
         if (!FileOperator.DirectoryExists(LocalRepositoryDirectory)) FileOperator.CreateDirectory(LocalRepositoryDirectory);
         else throw new DirectoryIsExistedException();
         AccessRuleSwitchControl(false);
      }
      /// <summary>
      /// 打开当前仓库，并解除安全访问规则。
      /// </summary>
      public void Open()
      {
         AccessRuleSwitchControl(true);
         Statue = ERepositoryStatue.Opened;
      }
      /// <summary>
      /// 关闭当前仓库，并追加安全访问规则。
      /// </summary>
      public void Close()
      {
         AccessRuleSwitchControl(false);
         Statue = ERepositoryStatue.Closed;
      }
      /// <summary>
      /// 将本地项目克隆到仓库之中。
      /// </summary>
      /// <param name="isCheckProjectIntegrity">指示该操作完成后，是否检查仓库内部文件的完整性。</param>
      /// <returns>如果该操作成功，则会返回true，否则会返回false。</returns>
      public bool Clone(bool isCheckProjectIntegrity)
      {
         bool isComplete = true;
         try
         {
            FileOperator.CopyDirectory(ProjectDirectory, LocalRepositoryDirectory, false);
         }
         catch { isComplete = false; }
         if (isCheckProjectIntegrity)
         {
            List<string> allProjFile = FileOperator.TraverseWith(ProjectDirectory, false);
            List<string> allLocalRepoFile = Traverse();
            if (allProjFile.Count != allLocalRepoFile.Count) isComplete = false;
            else
            {
               Parallel.For(0, allLocalRepoFile.Count, (index, interrupt) =>
               {
                  string pfAutograph = new FileSignature(allProjFile[index]).GetMD5String();
                  string lrfAutograph = new FileSignature(allLocalRepoFile[index]).GetMD5String();
                  if (pfAutograph != lrfAutograph)
                  {
                     isComplete = false;
                     interrupt.Stop();
                  }
               });
            }
         }
         return isComplete;
      }
      /// <summary>
      /// 遍历当前仓库内的所有文件，但不包含文件夹。
      /// </summary>
      /// <returns>该操作会获取仓库下存储的所有文件的文件地址，并将这些地址添加到List中返回给用户。</returns>
      public List<string> Traverse() => FileOperator.TraverseWith(LocalRepositoryDirectory, false);
      /// <summary>
      /// 修改仓库的安全访问规则。
      /// </summary>
      /// <param name="accessSwitch">安全访问规则控制，如果为true表示解除安全访问规则，否则表示追加安全访问规则。</param>
      private void AccessRuleSwitchControl(bool accessSwitch)
      {
         string domain = EnvironmentInformation.GetComputerName();
         string usrname = domain + @"\" + EnvironmentInformation.GetCurrentUserName();
         ERuleUpdateMode append = ERuleUpdateMode.Append;
         ERuleUpdateMode remove = ERuleUpdateMode.Remove;
         AccessControlSections acs = AccessControlSections.All;
         InheritanceFlags flags = InheritanceFlags.ContainerInherit;
         FileSystemRights rights = FileSystemRights.FullControl;
         AccessControlType act_allow = AccessControlType.Allow;
         AccessControlType act_deny = AccessControlType.Deny;
         if (!accessSwitch)
         {
            IOAccessRuleManagement.UpdateDirectoryAccessRule(LocalRepositoryDirectory, usrname, remove, acs, flags, rights, act_allow);
            IOAccessRuleManagement.UpdateDirectoryAccessRule(LocalRepositoryDirectory, usrname, append, acs, flags, rights, act_deny);
         }
         else
         {
            IOAccessRuleManagement.UpdateDirectoryAccessRule(LocalRepositoryDirectory, usrname, remove, acs, flags, rights, act_deny);
            IOAccessRuleManagement.UpdateDirectoryAccessRule(LocalRepositoryDirectory, usrname, append, acs, flags, rights, act_allow);
         }
      }
   }
   /// <summary>
   /// 描述仓库状态的枚举。
   /// </summary>
   public enum ERepositoryStatue : int
   {
      /// <summary>
      /// 表示仓库已打开。
      /// </summary>
      [EnumerationDescription("已打开")]
      Opened = 0x0000,
      /// <summary>
      /// 表示仓库已关闭。
      /// </summary>
      [EnumerationDescription("已关闭")]
      Closed = 0xffff
   }
}
