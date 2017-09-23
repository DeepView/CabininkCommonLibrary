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
      OnlyOnce = 0x0000,
      /// <summary>
      /// 每周开始执行一次。
      /// </summary>
      OnceAWeek = 0x0001,
      /// <summary>
      /// 每个月开始执行一次。
      /// </summary>
      OnceAMonth = 0x0002,
      /// <summary>
      /// 每个季度开始执行一次。
      /// </summary>
      OnceAQuarter = 0x0003,
      /// <summary>
      /// 每年开始执行一次。
      /// </summary>
      OnceAYear = 0x0004,
      /// <summary>
      /// 每天都执行一次。
      /// </summary>
      EveryDay = 0x0005,
      /// <summary>
      /// 一年的奇数天执行。
      /// </summary>
      OddDayOfYear = 0x0006,
      /// <summary>
      /// 一年的偶数天执行。
      /// </summary>
      EvenDayOfYear = 0x0007,
      /// <summary>
      /// 每个月的奇数天执行。
      /// </summary>
      OddDayOfMonth = 0x0008,
      /// <summary>
      /// 每个月的偶数天执行。
      /// </summary>
      EvenDayOfMonth = 0x0009,
      /// <summary>
      /// 每周的奇数天执行。
      /// </summary>
      OddDayOfWeekday = 0x000a,
      /// <summary>
      /// 每周的偶数天执行。
      /// </summary>
      EvenDayOfWeekday = 0x000b,
      /// <summary>
      /// 指定每周的某一天执行。
      /// </summary>
      DesignatedDayOfWeek = 0x000c,
      /// <summary>
      /// 指定每月的某一天执行。
      /// </summary>
      DesignatedDayOfMonth = 0x000d,
      /// <summary>
      /// 指定每年的某一天执行。
      /// </summary>
      DesignatedDayOfYear = 0x000e,
      /// <summary>
      /// 用户定义每周的某些天执行。
      /// </summary>
      UserDefinitionDaysOfWeek = 0x000f,
      /// <summary>
      /// 用户定义每月的某些天执行。
      /// </summary>
      UserDefinitionDaysOfMonth = 0x0010,
      /// <summary>
      /// 用户定义每年的某些天执行。
      /// </summary>
      UserDefinitionDaysOfYear = 0x0011
   }
}
