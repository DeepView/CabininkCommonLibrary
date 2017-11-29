using System;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Applications
{
   /// <summary>
   /// 应用程序设置选项类。
   /// </summary>
   /// <typeparam name="T">设置选项在存储和读取时需要容纳数据的数据类型。</typeparam>
   [Serializable]
   [ComVisible(true)]
   public class SettingsOption<T> : IEquatable<SettingsOption<T>>
   {
      private string _name;//设置选项的名称。
      private string _description;//设置选项的注释或者帮助信息。
      private T _value;//设置选项的具体值。
      private EEffectiveMode _effectiveMode;//设置选项的生效模式。
      /// <summary>
      /// 构造函数，创建一个只包含设置选项名称和选项值的应用程序设置选项实例。
      /// </summary>
      /// <param name="name">指定的设置选项名称。</param>
      /// <param name="value">指定的选项值。</param>
      public SettingsOption(string name, T value)
      {
         if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("设置选项名称不能为空！", "name");
         else _name = name;
         if (value == null) throw new NullReferenceException();
         else _value = value;
         _description = string.Empty;
         _effectiveMode = EEffectiveMode.Immediately;
      }
      /// <summary>
      /// 构造函数，创建一个包含设置选项名称、选项值、设置选项注释信息和生效模式的应用程序设置选项实例。
      /// </summary>
      /// <param name="name">指定的设置选项名称。</param>
      /// <param name="description">指定的设置选项注释或者帮助信息。</param>
      /// <param name="value">指定的选项值。</param>
      /// <param name="effectiveMode">指定的生效模式。</param>
      public SettingsOption(string name, string description, T value, EEffectiveMode effectiveMode)
      {
         if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("设置选项名称不能为空！", "name");
         else _name = name;
         if (value == null) throw new NullReferenceException();
         else _value = value;
         _description = description;
         _effectiveMode = effectiveMode;
      }
      /// <summary>
      /// 获取或设置当前实例的设置选项名称。
      /// </summary>
      public string Name
      {
         get => _name;
         set
         {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("设置选项名称不能为空！", "name");
            else _name = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例的设置选项注释或者帮助信息。
      /// </summary>
      public string Description { get => _description; set => _description = value; }
      /// <summary>
      /// 获取或设置当前实例的选项值。
      /// </summary>
      public T Value
      {
         get => _value;
         set
         {
            if (value == null) throw new NullReferenceException();
            else _value = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例的设置选项生效模式。
      /// </summary>
      public EEffectiveMode EffectiveMode { get => _effectiveMode; set => _effectiveMode = value; }
      /// <summary>
      /// 将指定的SettingsOption实例与当前实例做相等性比较。
      /// </summary>
      /// <param name="other">另外一个SettingsOption实例。</param>
      /// <returns>如果当前实例和目标实例相同，则返回true，否则返回false。</returns>
      public bool Equals(SettingsOption<T> other)
      {
         bool e_name = Name == other.Name;
         bool e_desc = Description == other.Description;
         bool e_val = Value.Equals(other.Value);
         bool e_mode = EffectiveMode == other.EffectiveMode;
         return e_name && e_desc && e_val && e_mode;
      }
   }
   /// <summary>
   /// 应用程序设置项的生效模式的枚举。
   /// </summary>
   public enum EEffectiveMode : int
   {
      /// <summary>
      /// 立即生效。
      /// </summary>
      [EnumerationDescription("立即生效")]
      Immediately = 0x0000,
      /// <summary>
      /// 必须在重新启动应用程序生效。
      /// </summary>
      [EnumerationDescription("重新启动应用程序生效")]
      MustResetApplication = 0x0001,
      /// <summary>
      /// 必须在重新启动计算机或者操作系统生效。
      /// </summary>
      [EnumerationDescription("重新启动计算机生效")]
      MustResetComputer = 0x0002
   }
}
