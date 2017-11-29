using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Cabinink.Windows;
namespace Cabinink.IOSystem.RevisionControl
{
   [Serializable]
   [ComVisible(true)]
   public class ActivityRecorder
   {
      private DateTime _recordTime;
      private long _activityId;
      private string _branch;
      private string _accountId;
      private EActivityType _activityType;
      private string _description;
      public ActivityRecorder(DateTime recordTime, long activityId, string branch, string accountId, EActivityType activityType)
      {
         _recordTime = recordTime;
         _activityId = activityId;
         _activityType = activityType;
         if (string.IsNullOrWhiteSpace(branch)) _branch = @"master";
         else _branch = branch;
         if (string.IsNullOrWhiteSpace(accountId)) _accountId = EnvironmentInformation.GetCurrentUserName();
         else _accountId = accountId;
      }
      public ActivityRecorder(DateTime recordTime, long activityId, string branch, string accountId, EActivityType activityType, string description)
      {
         string activityTypeString = EnumerationDescriptionAttribute.GetEnumDescription(activityType);
         _recordTime = recordTime;
         _activityId = activityId;
         _activityType = activityType;
         if (string.IsNullOrWhiteSpace(branch)) _branch = @"master";
         else _branch = branch;
         if (string.IsNullOrWhiteSpace(accountId)) _accountId = EnvironmentInformation.GetCurrentUserName();
         else _accountId = accountId;
         if (string.IsNullOrWhiteSpace(description)) _description = _recordTime.ToString() + "--" + activityTypeString;
         else _description = description;
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
