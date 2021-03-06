﻿using System;
using System.Text;
using System.Linq;
using Cabinink.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cabinink.DataTreatment.Database;
namespace Cabinink.IOSystem.RevisionControl
{
   /// <summary>
   /// 活动记录器的表示类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class ActivityRecorder : ILogOperator
   {
      private DateTime _recordTime;//记录时间。
      private long _activityId;//活动编号。
      private string _branch;//分支名称。
      private string _accountId;//活动发起的账户ID。
      private EActivityType _activityType;//活动类型。
      private RepositoryManager _repositoryManager;//活动记录器需要记录的仓库。
      private string _description;//活动注释。
      private const string VCS_LOGDB_FILE = @"\vcsdb.sqlite3";//本地文件版本控制系统工作日志数据库名称。
      private const string VCS_ACTIVITY_RECORD_LOG_DBTABLE = @"activityRecordLog";//本地文件版本控制系统活动记录数据表名称。
      /// <summary>
      /// 构造函数，创建一个指定活动记录时间、活动编号，分支名称，发起账户ID和活动类型的活动记录器实例。
      /// </summary>
      /// <param name="recordTime">指定的活动时间。</param>
      /// <param name="activityId">指定的活动编号。</param>
      /// <param name="branch">指定的分支名称。</param>
      /// <param name="accountId">指定的发起账户ID。</param>
      /// <param name="activityType">指定的活动类型。</param>
      /// <param name="repositoryManager">指定记录的版本仓库。</param>
      public ActivityRecorder(DateTime recordTime, long activityId, string branch, string accountId, EActivityType activityType, RepositoryManager repositoryManager)
      {
         _recordTime = recordTime;
         _activityId = activityId;
         _activityType = activityType;
         _repositoryManager = repositoryManager;
         if (string.IsNullOrWhiteSpace(branch)) _branch = @"master";
         else _branch = branch;
         if (string.IsNullOrWhiteSpace(accountId)) _accountId = EnvironmentInformation.GetCurrentUserName();
         else _accountId = accountId;
      }
      /// <summary>
      /// 构造函数，创建一个指定活动记录时间、活动编号，分支名称，发起账户ID、活动类型和活动注释的活动记录器实例。
      /// </summary>
      /// <param name="recordTime">指定的活动时间。</param>
      /// <param name="activityId">指定的活动编号。</param>
      /// <param name="branch">指定的分支名称。</param>
      /// <param name="accountId">指定的发起账户ID。</param>
      /// <param name="activityType">指定的活动类型。</param>
      /// <param name="repositoryManager">指定记录的版本仓库。</param>
      /// <param name="description">指定的活动注释信息。</param>
      public ActivityRecorder(DateTime recordTime, long activityId, string branch, string accountId, EActivityType activityType, RepositoryManager repositoryManager, string description)
      {
         string activityTypeString = EnumerationDescriptionAttribute.GetEnumDescription(activityType);
         _recordTime = recordTime;
         _activityId = activityId;
         _activityType = activityType;
         _repositoryManager = repositoryManager;
         if (string.IsNullOrWhiteSpace(branch)) _branch = @"master";
         else _branch = branch;
         if (string.IsNullOrWhiteSpace(accountId)) _accountId = EnvironmentInformation.GetCurrentUserName();
         else _accountId = accountId;
         if (string.IsNullOrWhiteSpace(description)) _description = _recordTime.ToString() + "--" + activityTypeString;
         else _description = description;
      }
      /// <summary>
      /// 获取或设置当前实例的活动记录时间。
      /// </summary>
      public DateTime RecordTime { get => _recordTime; set => _recordTime = value; }
      /// <summary>
      /// 获取或设置当前实例的活动编号。
      /// </summary>
      public long ActivityId { get => _activityId; set => _activityId = value; }
      /// <summary>
      /// 获取或设置当前实例的分支名称。
      /// </summary>
      public string Branch { get => _branch; set => _branch = value; }
      /// <summary>
      /// 获取或设置当前实例的活动发起账户的账户ID。
      /// </summary>
      public string AccountId { get => _accountId; set => _accountId = value; }
      /// <summary>
      /// 获取或设置当前实例的活动类型。
      /// </summary>
      public EActivityType ActivityType { get => _activityType; set => _activityType = value; }
      /// <summary>
      /// 获取或设置当前实例的活动注释信息。
      /// </summary>
      public string Description { get => _description; set => _description = value; }
      /// <summary>
      /// 获取当前实例的活动记录日志所在的数据库的文件地址。
      /// </summary>
      public string RecordDbUrl => _repositoryManager.LocalRepositoryDirectory + VCS_LOGDB_FILE;
      /// <summary>
      /// 获取本地文件版本控制系统活动记录数据表名称。
      /// </summary>
      internal static string LogDbTableName => VCS_ACTIVITY_RECORD_LOG_DBTABLE;

      public void ClearLog()
      {

      }
      public void UpdateLog()
      {
         SQLiteDBOIEncapsulation sqlite = new SQLiteDBOIEncapsulation(new Uri(RecordDbUrl));
         sqlite.InitializeConnection();
         sqlite.Connect();
         CreateLogDataTable();
         //TODO:这里的代码需要继续编辑。
         //TODO:由于这个命名空间的代码没有去编辑，因此在继续编辑的时候需要花费时间去解析以前的代码。
         sqlite.Disconnect();
      }
      private void CreateLogDataTable()
      {
         SQLiteDBOIEncapsulation sqlite = new SQLiteDBOIEncapsulation(new Uri(RecordDbUrl));
         sqlite.InitializeConnection();
         sqlite.Connect();
         if (!sqlite.DataTableExists(VCS_ACTIVITY_RECORD_LOG_DBTABLE))
         {
            string sqlPart = @"recordTime BIGINT, activityId BIGINT, branch TEXT, accountId TEXT, activityType INT, description TEXT";
            sqlite.ExecuteSql("create table " + VCS_ACTIVITY_RECORD_LOG_DBTABLE + "(" + sqlPart + ");");
         }
         sqlite.Disconnect();
      }
   }
   /// <summary>
   /// VSC活动类型枚举。
   /// </summary>
   public enum EActivityType : int
   {
      /// <summary>
      /// 推送操作。
      /// </summary>
      [EnumerationDescription("推送")]
      Push = 0x0000,
      /// <summary>
      /// 拉取操作。
      /// </summary>
      [EnumerationDescription("拉取")]
      Pull = 0x0001,
      /// <summary>
      /// 提交操作。
      /// </summary>
      [EnumerationDescription("提交")]
      Commit = 0x0002
   }
}
