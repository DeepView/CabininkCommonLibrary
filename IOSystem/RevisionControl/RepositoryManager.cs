using System;
using System.IO;
using System.Linq;
using System.Text;
using Cabinink.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cabinink.IOSystem.Security;
using System.Security.AccessControl;
namespace Cabinink.IOSystem.RevisionControl
{
   public class RepositoryManager : IObjectAccessSwitch
   {
      private string _repoName;//文件仓库名称。
      private string _description;//仓库详细信息。
      private string _projectDirectory;//项目目录所在路径。
      private string _localRepoDirectory;//本地仓库所在路径。
      public RepositoryManager(string repositoryName, string description)
      {
         _repoName = repositoryName;
         _description = description;
         _projectDirectory = string.Empty;
         _localRepoDirectory = _projectDirectory;
      }
      public RepositoryManager(string repositoryName, string description, string projectDirectory, string localRepositoryDirectory)
      {
         if (!FileOperator.DirectoryExists(projectDirectory)) throw new DirectoryNotFoundException("指定的项目目录不存在！");
         else
         {
            _repoName = repositoryName;
            _description = description;
            _projectDirectory = projectDirectory;
            _localRepoDirectory = localRepositoryDirectory;
         }
      }
      public string Name { get => _repoName; set => _repoName = value; }
      public string Description { get => _description; set => _description = value; }
      public string ProjectDirectory { get => _projectDirectory; set => _projectDirectory = value; }
      public string LocalRepositoryDirectory { get => _localRepoDirectory; set => _localRepoDirectory = value; }
      public void Initialize()
      {
         if (!FileOperator.DirectoryExists(LocalRepositoryDirectory)) FileOperator.CreateDirectory(LocalRepositoryDirectory);
         else throw new DirectoryIsExistedException();
         AccessRuleSwitchControl(false);
      }
      public void Open() => AccessRuleSwitchControl(true);
      public void Close() => AccessRuleSwitchControl(false);
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
            List<string> allLocalRepoFile = FileOperator.TraverseWith(LocalRepositoryDirectory, false);
            if (allProjFile.Count != allLocalRepoFile.Count) isComplete = false;
            else
            {
               Parallel.For(0, allLocalRepoFile.Count, (index, interrupt) =>
               {
                  string pfAutograph = new FileSignature(allProjFile[index]).GetMD5String();
                  string lrfAutograph = new FileSignature(allLocalRepoFile[index]).GetMD5String();
                  if(pfAutograph!=lrfAutograph )
                  {
                     isComplete = false;
                     interrupt.Stop();
                  }
               });
            }
         }
         return isComplete;
      }
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
}
