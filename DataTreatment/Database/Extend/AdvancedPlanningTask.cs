using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cabinink.DataTreatment.Database.Extend
{
   public struct SExecuteTimeCycle
   {
      private EExecuteFrequency frequency;
      private DateTime executeTime;
   }
   public class AdvancedPlanningTask : PlanningTask
   {
      private SExecuteTimeCycle _executeTimeCycle;
      public AdvancedPlanningTask(DBOIEncapsulation dbOperator, string planningTaskName, DateTime executeTime, string description) : base(dbOperator, planningTaskName, executeTime, description)
      {

      }
   }
   /// <summary>
   /// 计划任务执行频率的模式枚举。
   /// </summary>
   public enum EExecuteFrequency : int
   {
      /// <summary>
      /// 默认频率，只执行一次。
      /// </summary>
      [EnumerationDescription("默认频率，只执行一次")]
      OnlyOnce = 0x0000,
      /// <summary>
      /// 每周开始执行一次。
      /// </summary>
      [EnumerationDescription("每周开始执行一次")]
      OnceAWeek = 0x0001,
      /// <summary>
      /// 每个月开始执行一次。
      /// </summary>
      [EnumerationDescription("每个月开始执行一次")]
      OnceAMonth = 0x0002,
      /// <summary>
      /// 每个季度开始执行一次。
      /// </summary>
      [EnumerationDescription("每个季度开始执行一次")]
      OnceAQuarter = 0x0003,
      /// <summary>
      /// 每年开始执行一次。
      /// </summary>
      [EnumerationDescription("每年开始执行一次")]
      OnceAYear = 0x0004,
      /// <summary>
      /// 每天都执行一次。
      /// </summary>
      [EnumerationDescription("每天都执行一次")]
      EveryDay = 0x0005,
      /// <summary>
      /// 一年的奇数天执行。
      /// </summary>
      [EnumerationDescription("一年的奇数天执行")]
      OddDayOfYear = 0x0006,
      /// <summary>
      /// 一年的偶数天执行。
      /// </summary>
      [EnumerationDescription("一年的偶数天执行")]
      EvenDayOfYear = 0x0007,
      /// <summary>
      /// 每个月的奇数天执行。
      /// </summary>
      [EnumerationDescription("每个月的奇数天执行")]
      OddDayOfMonth = 0x0008,
      /// <summary>
      /// 每个月的偶数天执行。
      /// </summary>
      [EnumerationDescription("每个月的偶数天执行")]
      EvenDayOfMonth = 0x0009,
      /// <summary>
      /// 每周的奇数天执行。
      /// </summary>
      [EnumerationDescription("每周的奇数天执行")]
      OddDayOfWeekday = 0x000a,
      /// <summary>
      /// 每周的偶数天执行。
      /// </summary>
      [EnumerationDescription("每周的偶数天执行")]
      EvenDayOfWeekday = 0x000b,
      /// <summary>
      /// 指定每周的某一天执行。
      /// </summary>
      [EnumerationDescription("指定每周的某一天执行")]
      DesignatedDayOfWeek = 0x000c,
      /// <summary>
      /// 指定每月的某一天执行。
      /// </summary>
      [EnumerationDescription("指定每月的某一天执行")]
      DesignatedDayOfMonth = 0x000d,
      /// <summary>
      /// 指定每年的某一天执行。
      /// </summary>
      [EnumerationDescription("指定每年的某一天执行")]
      DesignatedDayOfYear = 0x000e,
      /// <summary>
      /// 用户定义每周的某些天执行。
      /// </summary>
      [EnumerationDescription("用户定义每周的某些天执行")]
      UserDefinitionDaysOfWeek = 0x000f,
      /// <summary>
      /// 用户定义每月的某些天执行。
      /// </summary>
      [EnumerationDescription("用户定义每月的某些天执行")]
      UserDefinitionDaysOfMonth = 0x0010,
      /// <summary>
      /// 用户定义每年的某些天执行。
      /// </summary>
      [EnumerationDescription("用户定义每年的某些天执行")]
      UserDefinitionDaysOfYear = 0x0011
   }
}
