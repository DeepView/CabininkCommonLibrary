using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Cabinink.TypeExtend.Geometry2D;
using System.Runtime.InteropServices;
namespace Cabinink.Devices
{
   /// <summary>
   /// 用于方便作为Win32Api的RECT类型传递的矩形结构。
   /// </summary>
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   public struct SWin32ApiRectangle
   {
      /// <summary>
      /// 表示当前矩形的左边位置。
      /// </summary>
      public int Left;
      /// <summary>
      /// 表示当前矩形的顶部位置。
      /// </summary>
      public int Top;
      /// <summary>
      /// 表示当前矩形的右边位置。
      /// </summary>
      public int Right;
      /// <summary>
      /// 表示当前矩形的底部位置。
      /// </summary>
      public int Bottom;
      /// <summary>
      /// 构造函数，创建一个指定尺寸的矩形。
      /// </summary>
      /// <param name="left">矩形的水平位置，或者是矩形的左边位置。</param>
      /// <param name="top">矩形的垂直位置，或者是矩形的顶部位置。</param>
      /// <param name="right">矩形的右边位置。</param>
      /// <param name="bottom">矩形的底部位置。</param>
      public SWin32ApiRectangle(int left, int top, int right, int bottom)
      {
         Left = left;
         Top = top;
         Right = right;
         Bottom = bottom;
      }
      /// <summary>
      /// 运算符重载，将指定的SWin32ApiRectangle结构实例转换为System.Drawing.Rectangle实例。
      /// </summary>
      /// <param name="rect">用于转换的SWin32ApiRectangle结构实例。</param>
      /// <returns>该操作将会转换一个数学意义上与SWin32ApiRectangle结构实例相同的System.Drawing.Rectangle实例。</returns>
      public static implicit operator Rectangle(SWin32ApiRectangle rect)
      {
         return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
      }
      /// <summary>
      /// 运算符重载，将指定的System.Drawing.Rectangle实例转换为SWin32ApiRectangle结构实例。
      /// </summary>
      /// <param name="rect">用于转换的System.Drawing.Rectangle实例。</param>
      /// <returns>该操作将会转换一个数学意义上与System.Drawing.Rectangle实例相同的SWin32ApiRectangle结构实例。</returns>
      public static implicit operator SWin32ApiRectangle(Rectangle rect)
      {
         return new SWin32ApiRectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
      }
   }
   /// <summary>
   /// 鼠标相关操作表示类，在这里之所以用位置指示器来作为当前类的名称，是为了防止与CLR的鼠标实例名称相冲突。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class PositionIndicator
   {
      private Mouse _mouse;//当前鼠标的CLR实例。
      /// <summary>
      /// 综合鼠标移动和按钮点击。
      /// </summary>
      /// <param name="flags">标志位集，指定点击按钮和鼠标动作的多种情况，此参数可以是下列值的某种组合：
      /// <para>MOUSEEVENTF_ABSOLUTE：0x8000，dx和dy参数含有规范化的绝对坐标。如果不设置，这些参数含有相对数据：相对于上次位置的改动位置。此标志可设置，也可不设置，不管鼠标的类型或与系统相连的类似于鼠标的设备的类型如何。</para>
      /// <para>MOUSEEVENTF_MOVE：0x0001，这表示鼠标移动。</para>
      /// <para>MOUSEEVENTF_LEFTDOWN：0x0002，这表示鼠标左键按下。</para>
      /// <para>MOUSEEVENTF_LEFTUP：0x0004，这表示鼠标左键松开。</para>
      /// <para>MOUSEEVENTF_RIGHTDOWN：0x0008，这表示鼠标右键按下。</para>
      /// <para>MOUSEEVENTF_RIGHTUP：0x0010。这表示鼠标右键松开。</para>
      /// <para>MOUSEEVENTF_MIDDLEDOWN：0x0020，这表示鼠标中键按下。</para>
      /// <para>MOUSEEVENTF_MIDDLEUP：0x0040，这表示鼠标中键松开。</para>
      /// <para>MOUSEEVENTF_WHEEL：0x0800，这表示鼠标轮被滚动，如果鼠标有一个滚轮，滚动的数量由buttons给出。</para>
      /// <para>MOUSEEVENTF_XDOWN：0x0080，这表示鼠标侧边X按键按下。</para>
      /// <para>MOUSEEVENTF_XUP：0x0100，这表示鼠标侧边X按键松开。</para>
      /// <para>MOUSEEVENTF_HWHEEL：0x1000，这表示鼠标中键被倾斜（MSDN原文：The wheel button is tilted）。</para>
      /// </param>
      /// <param name="dx">指定鼠标沿x轴的绝对位置或者从上次鼠标事件产生以来移动的数量，依赖于MOUSEEVENTF_ABSOLUTE的设置。给出的绝对数据作为鼠标的实际X坐标；给出的相对数据作为移动的mickeys数。一个mickey表示鼠标移动的数量，表明鼠标已经移动。</param>
      /// <param name="dy">指定鼠标沿y轴的绝对位置或者从上次鼠标事件产生以来移动的数量，依赖于MOUSEEVENTF_ABSOLUTE的设置。给出的绝对数据作为鼠标的实际y坐标，给出的相对数据作为移动的mickeys数。</param>
      /// <param name="buttons">如果flags为MOUSEEVENTF_WHEEL，则buttons指定鼠标轮移动的数量。正值表明鼠标轮向前转动，即远离用户的方向；负值表明鼠标轮向后转动，即朝向用户。一个轮击定义为WHEEL_DELTA，即120。如果dwFlagsS不是MOUSEEVENTF_WHEEL，则buttons应为零。</param>
      /// <param name="extraInfo">指定与鼠标事件相关的附加32位值。应用程序调用函数GetMessageExtraInfo来获得此附加信息。</param>
      /// <returns></returns>
      [DllImport("user32.dll", SetLastError = true)]
#pragma warning disable IDE1006
      private static extern int mouse_event(int flags, int dx, int dy, int buttons, int extraInfo);
#pragma warning restore IDE1006
      /// <summary>
      /// 将鼠标光标的活动范围限制在屏幕上指定的一个有效矩形区域内。
      /// </summary>
      /// <param name="rectangle">指定的有效矩形区域。</param>
      /// <returns>该操作会返回一个bool类型，用于表示这个操作是否成功。</returns>
      [DllImport("user32.dll", SetLastError = true)]
      private static extern bool ClipCursor(ref SWin32ApiRectangle rectangle);
      /// <summary>
      /// 构造函数，实例化当前的鼠标类对象。
      /// </summary>
      public PositionIndicator() => _mouse = new Mouse();
      /// <summary>
      /// 获取当前实例所表示的鼠标是否拥有滚轮。
      /// </summary>
      public bool HasScroll => _mouse.WheelExists;
      /// <summary>
      /// 获取当前实例所表示的鼠标是否交换了两个主要的鼠标按键。
      /// </summary>
      public bool ButtonsIsSwapped => _mouse.ButtonsSwapped;
      /// <summary>
      /// 获取当前实例所表示的鼠标其滚轮在滚动时所滚动的行数。
      /// </summary>
      public int WheelScrollLines => _mouse.WheelScrollLines;
      /// <summary>
      /// 获取当前实例所表示的鼠标其光标在监视器上的像素位置。
      /// </summary>
      public ExPoint2D Position => (Control.MousePosition.X, Control.MousePosition.Y);
      /// <summary>
      /// 获取当前鼠标已经按下的按键。
      /// </summary>
      /// <returns>该操作会返回一个鼠标按键集合，用于表示目前时间有哪些鼠标按键正处于按下的状态。</returns>
      public MouseButtons GetDownMouseButtons() => Control.MouseButtons;
      /// <summary>
      /// 向操作系统发送一个鼠标消息。
      /// </summary>
      /// <param name="position">鼠标光标在监视器上的坐标。</param>
      /// <param name="flags">标志位集，指定点击按钮和鼠标动作的多种情况，此参数可以是下列值的某种组合：
      /// <para>MOUSEEVENTF_ABSOLUTE：0x8000，dx和dy参数含有规范化的绝对坐标。如果不设置，这些参数含有相对数据：相对于上次位置的改动位置。此标志可设置，也可不设置，不管鼠标的类型或与系统相连的类似于鼠标的设备的类型如何。</para>
      /// <para>MOUSEEVENTF_MOVE：0x0001，这表示鼠标移动。</para>
      /// <para>MOUSEEVENTF_LEFTDOWN：0x0002，这表示鼠标左键按下。</para>
      /// <para>MOUSEEVENTF_LEFTUP：0x0004，这表示鼠标左键松开。</para>
      /// <para>MOUSEEVENTF_RIGHTDOWN：0x0008，这表示鼠标右键按下。</para>
      /// <para>MOUSEEVENTF_RIGHTUP：0x0010。这表示鼠标右键松开。</para>
      /// <para>MOUSEEVENTF_MIDDLEDOWN：0x0020，这表示鼠标中键按下。</para>
      /// <para>MOUSEEVENTF_MIDDLEUP：0x0040，这表示鼠标中键松开。</para>
      /// <para>MOUSEEVENTF_WHEEL：0x0800，这表示鼠标轮被滚动，如果鼠标有一个滚轮，滚动的数量由buttons给出。</para>
      /// <para>MOUSEEVENTF_XDOWN：0x0080，这表示鼠标侧边X按键按下。</para>
      /// <para>MOUSEEVENTF_XUP：0x0100，这表示鼠标侧边X按键松开。</para>
      /// <para>MOUSEEVENTF_HWHEEL：0x1000，这表示鼠标中键被倾斜（MSDN原文：The wheel button is tilted）。</para>
      /// </param>
      /// <param name="data">如果flags为MOUSEEVENTF_WHEEL，则data指定鼠标轮移动的数量。正值表明鼠标轮向前转动，即远离用户的方向；负值表明鼠标轮向后转动，即朝向用户。一个轮击定义为WHEEL_DELTA，即120。如果dwFlagsS不是MOUSEEVENTF_WHEEL，则data应为零。</param>
      /// <param name="extraInfo">指定与鼠标事件相关的附加32位值。应用程序调用函数GetMessageExtraInfo来获得此附加信息。</param>
      /// <param name="exceptionInfo">该操作如果产生了Win32Api异常，那么该参数则用来获取描述这个异常的文本信息，如果操作成功执行，会返回“操作已成功”之类的信息。</param>
      /// <returns>如果该操作已经成功达到了用户的目标，则会返回true，否则返回false。</returns>
      public bool SendMouseMessage(ExPoint2D position, int flags, int data, int extraInfo, ref string exceptionInfo)
      {
         int dx = (int)position.XPosition;
         int dy = (int)position.YPosition;
         long win32ErrorCode = 0;
         Action executed = new Action(delegate { mouse_event(flags, dx, dy, data, extraInfo); });
         exceptionInfo = string.Empty;
         CodeHelper.IsWritedWin32ErrorCode(executed, out win32ErrorCode);
         exceptionInfo = Win32ApiHelper.FormatErrorCode(win32ErrorCode);
         return win32ErrorCode == 0 ? true : false;
      }
      /// <summary>
      /// 将鼠标光标的活动范围限制在指定的Rectangle实例所表示的矩形内。
      /// </summary>
      /// <param name="rectangle">指定的Rectangle实例，用于描述一个矩形。</param>
      /// <returns>该操作会返回一个bool类型，用于表示这个操作是否成功。</returns>
      public bool Restrict(Rectangle rectangle)
      {
         SWin32ApiRectangle rect = rectangle;
         return ClipCursor(ref rect);
      }
      /// <summary>
      /// 将鼠标光标的活动范围限制在指定的ExRectangle实例所表示的矩形内。
      /// </summary>
      /// <param name="rectangle">指定的ExRectangle实例，用于描述一个矩形。</param>
      /// <returns>该操作会返回一个bool类型，用于表示这个操作是否成功。</returns>
      public bool Restrict(ExRectangle rectangle)
      {
         int x = (int)rectangle.Position.XPosition;
         int y = (int)rectangle.Position.YPosition;
         int width = (int)rectangle.Width;
         int height = (int)rectangle.Height;
         Rectangle clrRect = new Rectangle(new Point(x, y), new Size(width, height));
         SWin32ApiRectangle rect = clrRect;
         return ClipCursor(ref rect);
      }
      /// <summary>
      /// 将鼠标光标的活动范围限制在由指定位置和尺寸所描述的矩形范围之内。
      /// </summary>
      /// <param name="position">鼠标光标活动限制的基准位置。</param>
      /// <param name="size">鼠标光标活动的范围。</param>
      /// <returns>该操作会返回一个bool类型，用于表示这个操作是否成功。</returns>
      public bool Restrict(Point position, Size size) => Restrict(new Rectangle(position, size));
      /// <summary>
      /// 解除鼠标光标的活动范围限制。
      /// </summary>
      /// <returns>该操作会返回一个bool类型，用于表示这个操作是否成功。</returns>
      public bool Relieve()
      {
         Monitor monitor = new Monitor();
         Rectangle clrRect = new Rectangle(0, 0, (int)monitor.Width, (int)monitor.Height);
         SWin32ApiRectangle rect = clrRect;
         return ClipCursor(ref rect);
      }
   }
}
