using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.IOSystem
{
   /// <summary>
   /// 文件状态监视器，用于监视指定目录下的文件状态，这个状态包含但不限制于，创建、删除文件，文件重命名，内容更新和属性变更等等。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class FileStateMonitor
   {
      private string _directory;//需要被监视的目录。
      private long _changedCount;//文件更改检查计数。
      private FileSystemWatcher _watcher;//用于当前实例的状态监视器。
      private List<(FileSystemEventArgs, DateTime)> _monitorRecords;//文件监视记录列表，存放了所有的文件监视记录。
      /// <summary>
      /// 构造函数，创建一个指定监视目录的文件状态监视器实例。
      /// </summary>
      /// <param name="directory">需要被监视的目录。</param>
      /// <param name="includeSubDirectories">指定当前监视器是否监视指定目录中的子目录。</param>
      /// <exception cref="DirectoryNotFoundException">当指定的目录找不到时，则将会抛出这个异常。</exception>
      public FileStateMonitor(string directory, bool includeSubDirectories)
      {
         if (!FileOperator.DirectoryExists(directory)) throw new DirectoryNotFoundException("找不到该目录！");
         else
         {
            _directory = directory;
            _monitorRecords = new List<(FileSystemEventArgs, DateTime)>();
            _watcher = new FileSystemWatcher()
            {
               Filter = @"*.*",
               NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime |
                  NotifyFilters.DirectoryName | NotifyFilters.FileName |
                  NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                  NotifyFilters.Security | NotifyFilters.Size,
               IncludeSubdirectories = includeSubDirectories
            };
            _changedCount = 0;
         }
      }
      /// <summary>
      /// 获取或设置当前实例的监视目录，不过在更改这个属性之前，请先执行EndWatch()方法暂停监视，否则该更改可能无法应用到监视器中。
      /// </summary>
      public string WatchedDirectory { get => _directory; set => _directory = value; }
      /// <summary>
      /// 获取或设置当前实例的文件监视记录。
      /// </summary>
      public List<(FileSystemEventArgs, DateTime)> Records { get => _monitorRecords; set => _monitorRecords = value; }
      /// <summary>
      /// 获取或设置（private权限）当前实例的文件更改检查计数。
      /// </summary>
      public long ChangedCount { get => _changedCount; private set => _changedCount = value; }
      /// <summary>
      /// 开始监视指定的目录下所有的文件和子文件夹。
      /// </summary>
      public void StartWatch() => StartWatch(@"*.*");
      /// <summary>
      /// 开始监视指定目录下指定的符合监视条件的文件和子文件夹。
      /// </summary>
      /// <param name="filter">监视条件，定义方法和Windows通配符相同。</param>
      public void StartWatch(string filter)
      {
         _watcher.Filter = filter;
         _watcher.Path = WatchedDirectory;
         _watcher.Changed += new FileSystemEventHandler(OnProcess);
         _watcher.Created += new FileSystemEventHandler(OnProcess);
         _watcher.Deleted += new FileSystemEventHandler(OnProcess);
         _watcher.Renamed += new RenamedEventHandler(OnRenamed);
         _watcher.EnableRaisingEvents = true;
      }
      /// <summary>
      /// 停止监视器的所有监视活动。
      /// </summary>
      public void EndWatch() => _watcher.EnableRaisingEvents = false;
      /// <summary>
      /// 当文件发生除了重命名之外的更改则触发。
      /// </summary>
      /// <param name="source">事件源。</param>
      /// <param name="triggerEvent">传递的事件。</param>
      public virtual void OnProcess(object source, FileSystemEventArgs triggerEvent)
      {
         switch (triggerEvent.ChangeType)
         {
            case WatcherChangeTypes.Created:
               OnCreated(source, triggerEvent);
               break;
            case WatcherChangeTypes.Deleted:
               OnChanged(source, triggerEvent);
               break;
            case WatcherChangeTypes.Changed:
               OnDeleted(source, triggerEvent);
               break;
            case WatcherChangeTypes.Renamed:
            case WatcherChangeTypes.All:
            default:
               break;
         }
      }
      /// <summary>
      /// 当文件被创建时触发。
      /// </summary>
      /// <param name="source">事件源。</param>
      /// <param name="triggerEvent">传递的事件。</param>
      public virtual void OnCreated(object source, FileSystemEventArgs triggerEvent)
      {
         Records.Add((triggerEvent, DateTime.Now));
         ChangedCount++;
      }
      /// <summary>
      /// 当文件内容发生更改时触发。
      /// </summary>
      /// <param name="source">事件源。</param>
      /// <param name="triggerEvent">传递的事件。</param>
      public virtual void OnChanged(object source, FileSystemEventArgs triggerEvent)
      {
         Records.Add((triggerEvent, DateTime.Now));
         ChangedCount++;
      }
      /// <summary>
      /// 当文件被删除时触发。
      /// </summary>
      /// <param name="source">事件源。</param>
      /// <param name="triggerEvent">传递的事件。</param>
      public virtual void OnDeleted(object source, FileSystemEventArgs triggerEvent)
      {
         Records.Add((triggerEvent, DateTime.Now));
         ChangedCount++;
      }
      /// <summary>
      /// 当文件被重命名时触发，另外重命名还会触发Changed。
      /// </summary>
      /// <param name="source">事件源。</param>
      /// <param name="triggerEvent">传递的事件。</param>
      public virtual void OnRenamed(object source, RenamedEventArgs triggerEvent)
      {
         Records.Add((triggerEvent, DateTime.Now));
         ChangedCount++;
      }
   }
}

