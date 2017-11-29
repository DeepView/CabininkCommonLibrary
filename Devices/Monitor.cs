using System;
using System.Drawing;
using Microsoft.VisualBasic.Devices;
using Cabinink.TypeExtend.Geometry2D;
using System.Runtime.InteropServices;
namespace Cabinink.Devices
{
   /// <summary>
   /// 用于调节Gamma通道的坡度结构。
   /// </summary>
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   public struct SRamp
   {
      /// <summary>
      /// 红色通道Gamma坡度值。
      /// </summary>
      [CLSCompliant(false)]
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
      public UInt16[] Red;
      /// <summary>
      /// 绿色通道Gamma坡度值。
      /// </summary>
      [CLSCompliant(false)]
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
      public UInt16[] Green;
      /// <summary>
      /// 蓝色通道Gamma坡度值。
      /// </summary>
      [CLSCompliant(false)]
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
      public UInt16[] Blue;
   }
   /// <summary>
   /// 计算机显示器表示类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Monitor : ISize
   {
      private int _deviceContext;//当前彩色显示设备上下文。
      private Computer _thisComputer;//表示当前的计算机。
      private const short SYSTEM_DEFAULT_BRIGHTNESS_WITH_GAMMA = 127;//操作系统默认的基于Gamma的亮度值
      /// <summary>
      /// 设置Direct彩色显示器上的伽玛斜坡, 其驱动程序支持硬件中可下载的伽玛坡道。
      /// </summary>
      /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
      /// <param name="ramp">一个缓冲区包含Gamma坡道指针。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
      /// <returns>如果这个操作无异常发生，并且达到了用户需要的效果，则会返回true，否则将会返回false。</returns>
      [DllImport("gdi32.dll")]
      private unsafe static extern bool SetDeviceGammaRamp(int hdc, void* ramp);
      /// <summary>
      /// 获取具有支持硬件中可下载伽马斜坡的驱动程序的Direct彩色显示器上的伽玛坡道。
      /// </summary>
      /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
      /// <param name="ramp">一个缓冲区包含Gamma坡道指针。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
      /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
      [DllImport("gdi32.dll")]
      private unsafe static extern bool GetDeviceGammaRamp(int hdc, void* ramp);
      /// <summary>
      /// 设置Direct彩色显示器上的伽玛斜坡, 其驱动程序支持硬件中可下载的伽玛坡道。
      /// </summary>
      /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
      /// <param name="ramp">Gamma坡道值结构。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
      /// <returns>如果这个操作无异常发生，并且达到了用户需要的效果，则会返回true，否则将会返回false。</returns>
      [DllImport("gdi32.dll")]
      public static extern int GetDeviceGammaRamp(IntPtr hdc, ref SRamp ramp);
      /// <summary>
      /// 获取具有支持硬件中可下载伽马斜坡的驱动程序的Direct彩色显示器上的伽玛坡道。
      /// </summary>
      /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
      /// <param name="ramp">Gamma坡道值结构。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
      /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
      [DllImport("gdi32.dll")]
      public static extern int SetDeviceGammaRamp(IntPtr hdc, ref SRamp ramp);
      /// <summary>
      /// 该函数检索一指定窗口的客户区域或整个屏幕的显示设备上下文环境的句柄。
      /// </summary>
      /// <param name="handle">设备上下文环境被检索的窗口的句柄，如果该值为NULL，GetDC则检索整个屏幕的设备上下文环境。</param>
      /// <returns>操作如果成功，返回指定窗口客户区的设备上下文环境；如果失败，返回值为Null。</returns>
      [DllImport("user32.dll")]
      public static extern IntPtr GetDC(IntPtr handle);
      /// <summary>
      /// 构造函数，初始化当前的计算机显示器实例。
      /// </summary>
      public Monitor()
      {
         _thisComputer = new Computer();
         _deviceContext = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();
      }
      /// <summary>
      /// 获取当前计算机显示器的高度。
      /// </summary>
      public double Height => _thisComputer.Screen.Bounds.Height;
      /// <summary>
      /// 获取当前计算机显示器的宽度。
      /// </summary>
      public double Width => _thisComputer.Screen.Bounds.Width;
      /// <summary>
      /// 获取当前计算机显示器的左上角坐标。
      /// </summary>
      public ExPoint2D LTCoordinate => new ExPoint2D(0, 0);
      /// <summary>
      /// 获取当前计算机显示器的右下角坐标。
      /// </summary>
      public ExPoint2D RBCoordinate => new ExPoint2D(Width - 1, Height - 1);
      /// <summary>
      /// 获取当前计算机显示器的工作区所对应的矩形。
      /// </summary>
      public ExRectangle WorkingArea
      {
         get
         {
            float height = _thisComputer.Screen.WorkingArea.Height;
            float width = _thisComputer.Screen.WorkingArea.Width;
            return new ExRectangle(new SizeF(width, height));
         }
      }
      /// <summary>
      /// 获取当前计算机显示器的设备名称。
      /// </summary>
      public string Name => _thisComputer.Screen.DeviceName;
      /// <summary>
      /// 获取当前计算机显示器的全屏幕快照。
      /// </summary>
      /// <param name="isCopyToClipboard">指示是否将获取的屏幕快照写入Windows剪贴板。</param>
      /// <returns>该操作会返回一个和显示器尺寸相同的Bitmap格式的屏幕快照。</returns>
      public Bitmap GetScreenSnapshot(bool isCopyToClipboard) => GetScreenSnapshot(LTCoordinate, RBCoordinate, isCopyToClipboard);
      /// <summary>
      /// 获取当前计算机显示器的局部屏幕快照，该方法与显示设备没有太多关联，因此多个Monitor实例进行屏幕快照操作是一件没有意义的事情。
      /// </summary>
      /// <param name="ltPoint">获取屏幕快照的左上角坐标。</param>
      /// <param name="rbPoint">获取屏幕快照的右下角坐标。</param>
      /// <param name="isCopyToClipboard">指示是否将获取的屏幕快照写入Windows剪贴板。</param>
      /// <returns>该操作会返回一个指定区域的Bitmap格式的屏幕快照。</returns>
      public Bitmap GetScreenSnapshot(ExPoint2D ltPoint, ExPoint2D rbPoint, bool isCopyToClipboard)
      {
         if ((ltPoint == rbPoint) || (ltPoint.Equals(rbPoint))) throw new AggregateException("不允许面积为零的截图区域");
         Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
         StraightLine2D line = new StraightLine2D(ltPoint, rbPoint);
         int x = (int)line.AbscissaMappingLength;
         int y = (int)line.OrdinateMappingLength;
         Bitmap img = new Bitmap(x + 1, y + 1);
         Graphics g = Graphics.FromImage(img);
         g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
         g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
         if ((ltPoint.XPosition < rbPoint.XPosition) && (ltPoint.YPosition < rbPoint.YPosition))
         {
            g.CopyFromScreen(ltPoint.ToClrPoint(), new Point(0, 0), new Size(x, y));
         }
         else if ((ltPoint.XPosition > rbPoint.XPosition) && (ltPoint.YPosition > rbPoint.YPosition))
         {
            g.CopyFromScreen(rbPoint.ToClrPoint(), new Point(0, 0), new Size(x, y));
         }
         else if ((ltPoint.XPosition < rbPoint.XPosition) && (ltPoint.YPosition > rbPoint.YPosition))
         {
            g.CopyFromScreen(new ExPoint2D(ltPoint.XPosition, rbPoint.YPosition).ToClrPoint(), new Point(0, 0), new Size(x, y));
         }
         else if ((ltPoint.YPosition < rbPoint.YPosition) && (ltPoint.XPosition > rbPoint.XPosition))
         {
            g.CopyFromScreen(new ExPoint2D(rbPoint.XPosition, ltPoint.YPosition).ToClrPoint(), new Point(0, 0), new Size(x, y));
         }
         if (isCopyToClipboard) _thisComputer.Clipboard.SetImage(img);
         return img;
      }
      /// <summary>
      /// 从Gamma值的基础上设置当前显示设备的亮度。
      /// </summary>
      /// <param name="gamma">需要调整的Gamma值，有效范围是[0, 255]。</param>
      /// <returns>该操作如果达到了用户需要的效果，则会返回true，否则会返回false。</returns>
      public unsafe bool SetBrightnessWithGamma(short gamma)
      {
         if (gamma > 255) gamma = 255;
         if (gamma < 0) gamma = 0;
         short* gArray = stackalloc short[3 * 256];
         short* idx = gArray;
         for (int j = 0; j < 3; j++)
         {
            for (int i = 0; i < 256; i++)
            {
               int arrayVal = i * (gamma + 128);
               if (arrayVal > 65535) arrayVal = 65535;
               *idx = (short)arrayVal;
               idx++;
            }
         }
         return SetDeviceGammaRamp(_deviceContext, gArray);
      }
      /// <summary>
      /// 重置当前显示设备的亮度，该操作是基于SetBrightnessWithGamma方法实现的。
      /// </summary>
      public void ResetBrightness() => SetBrightnessWithGamma(SYSTEM_DEFAULT_BRIGHTNESS_WITH_GAMMA);
   }
}
