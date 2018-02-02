using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cabinink.DataTreatment.Database;
namespace Cabinink.IOSystem.RevisionControl
{
   /// <summary>
   /// 暂存工作区类，用于执行本地文件版本控制系统的文件更改暂存工作。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class StagingArea : ILogOperator
   {
      private FileStateMonitor _monitor;//工作区状态监视器。
      private RepositoryManager _repoManager;//文件仓库管理。
      private string _stagingDirectory;//暂存工作区的本地目录路径。
      private long _lastTimeMonitorChangedCount;//上一次文件状态监视更新计数。
      private long _storageTimeTicks;//暂存区更新时间。
      private const string VCS_LOGDB_FILE = @"\vcsdb.sqlite3";//本地文件版本控制系统工作日志数据库名称。
      private const string VCS_STAGING_LOG_DBTABLE = @"stagingLog";//本地文件版本控制系统暂存区日志数据表名称。
      /// <summary>
      /// 构造函数，创建一个指定监视器和项目仓库的暂存区。
      /// </summary>
      /// <param name="monitor">指定的文件状态监视器。</param>
      /// <param name="repository">指定的项目仓库实例。</param>
      public StagingArea(FileStateMonitor monitor, RepositoryManager repository)
      {
         if (monitor == null || repository == null) throw new NullReferenceException();
         else
         {
            DirectoryInfo dir;
            _monitor = monitor;
            _repoManager = repository;
            _stagingDirectory = _repoManager.LocalRepositoryDirectory + @"\#stage";
            _lastTimeMonitorChangedCount = _monitor.ChangedCount;
            _storageTimeTicks = -1;
            dir = new DirectoryInfo(_stagingDirectory)
            {
               Attributes = FileAttributes.Hidden
            };
            File.SetAttributes(_stagingDirectory, dir.Attributes | FileAttributes.Hidden & ~FileAttributes.ReadOnly);
         }
      }
      /// <summary>
      /// 获取或设置（private权限）当前暂存区所包含的文件状态监视器。
      /// </summary>
      public FileStateMonitor Monitor { get => _monitor; private set => _monitor = value; }
      /// <summary>
      /// 获取或设置（private权限）当前暂存区所包含的暂存工作区目录。
      /// </summary>
      public string StagingDirectory { get => _stagingDirectory; private set => _stagingDirectory = value; }
      /// <summary>
      /// 获取或设置（private权限）当前暂存区的文件暂存操作的时间。
      /// </summary>
      public long StorageTime { get => _storageTimeTicks; private set => _storageTimeTicks = value; }
      /// <summary>
      /// 检查项目目录中是否存在更改。
      /// </summary>
      /// <returns>如果项目目录存在更改，则返回true，否则返回false。</returns>
      public bool HasChanged() => _lastTimeMonitorChangedCount < Monitor.ChangedCount;
      /// <summary>
      /// 清除文件状态监视器中的所有监视记录。
      /// </summary>
      public void ClearMonitorRecords() => Monitor.Records.Clear();
      /// <summary>
      /// 获取本地文件版本控制系统暂存区日志数据表名称。
      /// </summary>
      internal static string StagingLogDbTableName => VCS_STAGING_LOG_DBTABLE;
      /// <summary>
      /// 将已经更改的文件更新并存储到暂存工作区。
      /// </summary>
      /// <returns>如果暂存工作区的文件已经被更新，则返回true，否则返回false。</returns>
      public bool UpdateStage()
      {
         bool result = false;
         if (HasChanged())
         {
            List<(FileSystemEventArgs, DateTime)> records = Monitor.Records;
            Parallel.For(0, records.Count, (index) =>
            {
               StorageTime = DateTime.Now.Ticks;
               if (!FileOperator.DirectoryExists(StorageTime.ToString()))
               {
                  FileOperator.CreateDirectory(StorageTime.ToString());
               }
               string sourceUrl = records[index].Item1.FullPath;
               string targetUrl = StagingDirectory + @"\" + records[index].Item2.Ticks.ToString();
               FileOperator.CopyFile(sourceUrl, targetUrl, true);
            });
            result = true;
         }
         return result;
      }
      /// <summary>
      /// 向数据库中更新本次的暂存操作记录。
      /// </summary>
      public void UpdateLog()
      {
         string sqlSentence = @"create table " + StagingLogDbTableName + @"(storageTime BIGINT, changedTime BIGINT, fileUrl TEXT);";
         SQLiteDBOIEncapsulation sqlite = new SQLiteDBOIEncapsulation(new Uri(StagingDirectory + VCS_LOGDB_FILE));
         sqlite.InitializeConnection();
         sqlite.Connect();
         StorageTime = DateTime.Now.Ticks;
         if (!sqlite.DataTableExists("stagingLog")) sqlite.ExecuteSql(sqlSentence);
         for (int i = 0; i < Monitor.Records.Count; i++)
         {
            long changedTime = Monitor.Records[i].Item2.Ticks;
            string fileUrl = Monitor.Records[i].Item1.FullPath;
            if (sqlite.RecordExists("stagingLog", "fileUrl='" + Monitor.Records[i].Item1.FullPath + "'"))
            {
               sqlSentence = @"update stagingLog set storageTime=" +
                  StorageTime + @", changedTime=" +
                  changedTime + @" where fileUrl='" + fileUrl + ";";
               sqlite.ExecuteSql(sqlSentence);
            }
            else
            {
               sqlSentence = @"insert into stagingLog values(" +
                  StorageTime + ", " + changedTime + ", " + fileUrl + ");";
               sqlite.ExecuteSql(sqlSentence);
            }
         }
         sqlite.Disconnect();
      }
      /// <summary>
      /// 清空数据库中的暂存操作记录。
      /// </summary>
      public void ClearLog()
      {
         SQLiteDBOIEncapsulation sqlite = new SQLiteDBOIEncapsulation(new Uri(StagingDirectory + VCS_LOGDB_FILE));
         sqlite.InitializeConnection();
         sqlite.Connect();
         sqlite.ExecuteSql("delete from stagingLog;");
         sqlite.Disconnect();
      }
   }
}
