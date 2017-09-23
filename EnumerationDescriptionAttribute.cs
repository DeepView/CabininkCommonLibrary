using System;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Cabinink
{
   /// <summary>
   /// 枚举注释特性类，用于标注指定枚举的注释。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
   public sealed class EnumerationDescriptionAttribute : Attribute
   {
      private string _description;//对应枚举的注释。
      /// <summary>
      /// 构造函数，创建一个指定枚举注释的实例。
      /// </summary>
      /// <param name="description">枚举值对应的注释。</param>
      public EnumerationDescriptionAttribute(string description) : base() => _description = description;
      /// <summary>
      /// 获取当前实例的枚举注释。
      /// </summary>
      public string Description => _description;
      /// <summary>
      /// 枚举注释获取，用于获取指定枚举的注释。
      /// </summary>
      /// <param name="enumeration">需要被获取注释的枚举。</param>
      /// <returns>该操作会返回参数value对应的枚举某一个枚举值的注释。</returns>
      public static string GetEnumDescription(Enum enumeration)
      {
         if (enumeration == null) throw new ArgumentException("value");
         string description = enumeration.ToString();
         FieldInfo fieldInfo = enumeration.GetType().GetField(description);
         EnumerationDescriptionAttribute[] attributes =
            (EnumerationDescriptionAttribute[])fieldInfo.GetCustomAttributes(
               typeof(EnumerationDescriptionAttribute), false);
         if (attributes != null && attributes.Length > 0) description = attributes[0].Description;
         return description;
      }
   }
}
