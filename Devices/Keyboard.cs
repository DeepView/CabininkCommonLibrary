using System;
using System.Runtime.InteropServices;
namespace Cabinink.Devices
{
   /// <summary>
   /// 键盘基础操作类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class Keyboard
   {
      /// <summary>
      /// 阻塞键盘及鼠标事件到达当前应用程序。
      /// </summary>
      /// <param name="block">该参数指明函数的目的。如果参数为true，则鼠标和键盘事件将被阻塞。如果参数为false，则鼠标和键盘事件不被阻塞。注意，只有当该线程成功调用阻塞后才能解除阻塞。</param>
      [DllImport("user32.dll")]
      private static extern void BlockInput([In]bool block);
      /// <summary>
      /// 是否允许键盘或者鼠标这些输入设备的消息到达当前应用程序。
      /// </summary>
      /// <param name="isEnabled">指示当前操作是否允许键鼠消息进入应用程序。</param>
      public static void IsEnableInputSource(bool isEnabled) => BlockInput(!isEnabled);
      /// <summary>
      /// 指示当前键盘设备是否启用了大写锁定。
      /// </summary>
      /// <returns>该操作会返回一个Boolean数据，如果为true，则表示大写锁定已弃用，否则表示大写锁定已禁用。</returns>
      public static bool IsUppercaseLock() => new Microsoft.VisualBasic.Devices.Keyboard().CapsLock;
      /// <summary>
      /// 指示当前键盘设备是否启用了滚动锁定。
      /// </summary>
      /// <returns>该操作会返回一个Boolean数据，如果为true，则表示滚动锁定已弃用，否则表示滚动锁定已禁用。</returns>
      public static bool IsScrollLock() => new Microsoft.VisualBasic.Devices.Keyboard().ScrollLock;
      /// <summary>
      /// 指示当前键盘设备是否启用了数字键盘锁定。
      /// </summary>
      /// <returns>该操作会返回一个Boolean数据，如果为true，则表示数字键盘锁定已弃用，否则表示数字键盘锁定已禁用。</returns>
      public static bool IsNumberLock() => new Microsoft.VisualBasic.Devices.Keyboard().NumLock;
      /// <summary>
      /// 指示当前键盘设备是否已按下Alt键。
      /// </summary>
      /// <returns>该操作会返回一个Boolean数据，如果为true，则表示Alt按键已经按下，否则表示这个按键未被按下。</returns>
      public static bool AlterKeyIsDown() => new Microsoft.VisualBasic.Devices.Keyboard().AltKeyDown;
      /// <summary>
      /// 指示当前键盘设备是否已按下Ctrl键。
      /// </summary>
      /// <returns>该操作会返回一个Boolean数据，如果为true，则表示Ctrl按键已经按下，否则表示这个按键未被按下。</returns>
      public static bool ControlKeyIsDown() => new Microsoft.VisualBasic.Devices.Keyboard().CtrlKeyDown;
      /// <summary>
      /// 指示当前键盘设备是否已按下Shift键。
      /// </summary>
      /// <returns>该操作会返回一个Boolean数据，如果为true，则表示Shift按键已经按下，否则表示这个按键未被按下。</returns>
      public static bool ShiftKeyIsDown() => new Microsoft.VisualBasic.Devices.Keyboard().ShiftKeyDown;
      /// <summary>
      /// 向活动窗口发送一个或多个按键。
      /// </summary>
      /// <param name="keys">需要发送的按键或者按键集合。</param>
      public static void SendKeys(string keys) => new Microsoft.VisualBasic.Devices.Keyboard().SendKeys(keys);
   }
}
