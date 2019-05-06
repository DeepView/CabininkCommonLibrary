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
   public struct SGammaSlope
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
   /// 相关设备初始化和打印机环境信息的结构体，对应Win32Api中的DEVMODE结构。
   /// </summary>
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
   public struct SDevicesMode
   {
      /// <summary>
      /// 驱动程序支持的设备名称，这个字符串在设备驱动程序之间是相互不同的。
      /// </summary>
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string DeviceName;
      /// <summary>
      /// 初始化数据的版本数字，这个结构就基于这些数据。
      /// </summary>
      public short SpecVersion;
      /// <summary>
      /// 打印机驱动程序开发商分配的打印机驱动程序版本号。
      /// </summary>
      public short DriverVersion;
      /// <summary>
      /// SDevicesInitAndPrinterEnvironment结构的大小，以字节为单位，不包括DriverData（与设备有关）成员。
      /// </summary>
      public short Size;
      /// <summary>
      /// 当前结构后面的私有驱动程序数据的数目，以字节为单位，如果设备驱动程序不使用该设备独有的信息，就把这个成员设为零。
      /// </summary>
      public short DriverExtra;
      /// <summary>
      /// 指定了SDevicesInitAndPrinterEnvironment结构的其余成员中哪些已被初始化，第0位（定义为DM）ORIENTATION）代表dmOrientation，第1位（定义为 DM_PAPERSIZE）代表dmPaperSize等等，打印机驱动出现仅支持那些适合打印技术的成员。
      /// </summary>
      public int Fields;
      /// <summary>
      /// 选择纸的方向，这个成员可以为DMORIENT_PORTRAIT（1）或DMORIENT_ LANDSCAPE（2）。
      /// </summary>
      public short Orientation;
      /// <summary>
      /// 选择将用于打印的纸张大小。
      /// </summary>
      public short PaperSize;
      /// <summary>
      /// 重定义由PaperSize成员指定的纸张长度，可用于自定义纸张大小，也可以用于点阵打印机，这种打印机能打出任意长度的纸张，这些值与这个结构中其他指定物理长度的值都是以0.1毫米为单位的。
      /// </summary>
      public short PaperLength;
      /// <summary>
      /// 重载由PaperSize成员指定的纸张宽度。
      /// </summary>
      public short PaperWidth;
      /// <summary>
      /// 指定了打印输出的缩放因子，实际的页面大小为物理纸张的大小乘以Scale/100。
      /// </summary>
      public short Scale;
      /// <summary>
      ///  如果设备支持多页拷贝，则选择了要打印的拷贝数目。
      /// </summary>
      public short Copies;
      /// <summary>
      /// 保留，必须为0。
      /// </summary>
      public short DefaultSource;
      /// <summary>
      /// 指定了打印机的分辨率。
      /// </summary>
      public short PrintQuality;
      /// <summary>
      /// 对于彩色打印机，在彩色和单色之间切换。
      /// </summary>
      public short Color;
      /// <summary>
      /// 为支持双面打印的打印机选择双面打印方式。
      /// </summary>
      public short Duplex;
      /// <summary>
      /// 指定了打印机在y方向的分辨率，以每英寸的点数为单位，如果打印机对该成员进行了初始化，PrintQuality成员指定了打印机在x方向的分辨率，以每英寸点数为单位。
      /// </summary>
      public short YResolution;
      /// <summary>
      /// 指明如何打印TrueType字体。
      /// </summary>
      public short TTOption;
      /// <summary>
      ///  指定在打印多份拷贝的时候是否使用校对。
      /// </summary>
      public short Collate;
      /// <summary>
      /// 指定了要使用的格式名字，例如Letter或Legal，这些名字的完整集合可以通过Windows的EnumForms函数获得。
      /// </summary>
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string FormName;
      /// <summary>
      /// 用于将结构对齐到DWORD边界，不能使用或引用这个成员。它的名字和用法是保留的，在以后的版本中可能会变化。
      /// </summary>
      public short LogPixels;
      /// <summary>
      /// 指定了显示设备的颜色分辨率，以像素的位数为单位，例如16色使用4位，256色使用8位，而65536色使用16位。
      /// </summary>
      public int BitsPerPel;
      /// <summary>
      /// 可见设备表面的以像素为单位的宽度。
      /// </summary>
      public int PelsWidth;
      /// <summary>
      /// 可见设备表面的以像素为单位的高度。
      /// </summary>
      public int PelsHeight;
      /// <summary>
      /// 指定了设备的显示模式。
      /// </summary>
      public int DisplayFlags;
      /// <summary>
      /// 显示设备的特定模式所使用的以赫兹为单位的频率（每秒的周期数）。
      /// </summary>
      public int DisplayFrequency;
   }
   /// <summary>
   /// 计算机显示器表示类，该类不适合于多显示器设备。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Monitor : ISize
   {
      private int _deviceContext;//当前彩色显示设备上下文。
      private Computer _thisComputer;//表示当前的计算机。
      private const short SYSTEM_DEFAULT_BRIGHTNESS_WITH_GAMMA = 127;//操作系统默认的基于Gamma的亮度值。
      private const int SM_DIGITIZER = 94;//API常量，用于指定设备支持的数字化器输入类型的位掩码的常量。
      private const int SM_MAXIMUMTOUCHES = 95;//API常量，用于获取当前设备支持的最大触摸点的数量的常量。
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
      public static extern int GetDeviceGammaRamp(IntPtr hdc, ref SGammaSlope ramp);
      /// <summary>
      /// 获取具有支持硬件中可下载伽马斜坡的驱动程序的Direct彩色显示器上的伽玛坡道。
      /// </summary>
      /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
      /// <param name="ramp">Gamma坡道值结构。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
      /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
      [DllImport("gdi32.dll")]
      public static extern int SetDeviceGammaRamp(IntPtr hdc, ref SGammaSlope ramp);
      /// <summary>
      /// 该函数检索一指定窗口的客户区域或整个屏幕的显示设备上下文环境的句柄。
      /// </summary>
      /// <param name="handle">设备上下文环境被检索的窗口的句柄，如果该值为NULL，GetDC则检索整个屏幕的设备上下文环境。</param>
      /// <returns>操作如果成功，返回指定窗口客户区的设备上下文环境；如果失败，返回值为Null。</returns>
      [DllImport("user32.dll")]
      public static extern IntPtr GetDC(IntPtr handle);
      /// <summary>
      /// 该函数把缺省显示设备的设置改变为由devMode设定的图形模式，要改变一个特定显示设备的设置，请使用ChangeDisplaySettingsEx函数。
      /// </summary>
      /// <param name="devMode">指向一个描述转变图表的SDevicesInitAndPrinterEnvironment的指针。</param>
      /// <param name="flags">表明了图形模式如何改变。</param>
      /// <returns>该函数会返回一个整数，这个整数表示了该操作执行之后的状态。</returns>
      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      private static extern int ChangeDisplaySettings([In]ref SDevicesMode devMode, int flags);
      /// <summary>
      /// 获取显示设备的一个图形模式设备，通过对该函数一系列的调用可以得到显示设备所有的图形模式信息。
      /// </summary>
      /// <param name="deviceName">指向一个以null的结尾的字符串，该字符串指定了显示设备。</param>
      /// <param name="modeNum">表明要检索的信息类型。</param>
      /// <param name="devMode">SDevicesInitAndPrinterEnvironment结构的指针，该结构存储指定图形模式的信息，在调用EnumDisplaySettings之前，设置dmSize为sizeof(SDevicesInitAndPrinterEnvironment)，并且以字节为单位，设置DriveExtra元素为接收专用驱动数据可用的附加空间。</param>
      /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref SDevicesMode devMode);
      /// <summary>
      /// 该函数用来访问使用设备描述表的设备数据，应用程序指定相应设备描述表的句柄和说明该函数访问数据类型的索引来访问这些数据。
      /// </summary>
      /// <param name="hdc">设备上下文环境的句柄。</param>
      /// <param name="index">指定返回项，该参数取下列一值，可以取的值如下所示：
      /// <para>DRIVERVERSION = 0</para>
      /// <para>TECHNOLOGY = 2</para>
      /// <para>HORZSIZE = 4</para>
      /// <para>VERTSIZE = 6</para>
      /// <para>HORZRES = 8</para>
      /// <para>VERTRES = 10</para>
      /// <para>BITSPIXEL = 12</para>
      /// <para>PLANES = 14</para>
      /// <para>NUMBRUSHES = 16</para>
      /// <para>NUMPENS = 18</para>
      /// <para>NUMMARKERS = 20</para>
      /// <para>NUMFONTS = 22</para>
      /// <para>NUMCOLORS = 24</para>
      /// <para>PDEVICESIZE = 26</para>
      /// <para>CURVECAPS = 28</para>
      /// <para>LINECAPS = 30</para>
      /// <para>POLYGONALCAPS = 32</para>
      /// <para>TEXTCAPS = 34</para>
      /// <para>CLIPCAPS = 36</para>
      /// <para>RASTERCAPS = 38</para>
      /// <para>ASPECTX = 40</para>
      /// <para>ASPECTY = 42</para>
      /// <para>ASPECTXY = 44</para>
      /// <para>SHADEBLENDCAPS = 45</para>
      /// <para>LOGPIXELSX = 88</para>
      /// <para>LOGPIXELSY = 90</para>
      /// <para>SIZEPALETTE = 104</para>
      /// <para>NUMRESERVED = 106</para>
      /// <para>COLORRES = 108</para>
      /// <para>PHYSICALWIDTH = 110</para>
      /// <para>PHYSICALHEIGHT = 111</para>
      /// <para>PHYSICALOFFSETX = 112</para>
      /// <para>PHYSICALOFFSETY = 113</para>
      /// <para>SCALINGFACTORX = 114</para>
      /// <para>SCALINGFACTORY = 115</para>
      /// <para>VREFRESH = 116</para>
      /// <para>DESKTOPVERTRES = 117</para>
      /// <para>DESKTOPHORZRES = 118</para>
      /// <para>BLTALIGNMENT = 119</para></param>
      /// <returns>返回值指定所需项的值，当索引为BITSPIXEL且设备有15bpp或16bpp时，返回值为16。</returns>
      [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      private static extern int GetDeviceCaps(IntPtr hdc, int index);
      /// <summary>
      /// 该函数返回桌面窗口的句柄，桌面窗口覆盖整个屏幕，是一个要在其上绘制所有的图标和其他窗口的区域。
      /// </summary>
      /// <returns>函数返回桌面窗口的句柄。</returns>
      [DllImport("user32.dll")]
      public extern static IntPtr GetDesktopWindow();
      /// <summary>
      /// 检索显示元素和系统配置设置的各种系统度量。
      /// </summary>
      /// <param name="index">要检索的系统度量或配置设置。</param>
      /// <returns>如果函数操作成功，返回值是请求的系统度量或配置设置。如果函数操作失败，返回值为0。GetLastError不会为该函数提供扩展错误信息。</returns>
      /// <remarks>如果需要获取该函数的更多信息，可以参考https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics。</remarks>
      [DllImport("user32.dll")]
      private static extern int GetSystemMetrics(int index);
      /// <summary>
      /// 构造函数，初始化当前的计算机显示器实例。
      /// </summary>
      public Monitor()
      {
         _thisComputer = new Computer();
         _deviceContext = DeviceContext.ToInt32();
      }
      /// <summary>
      /// 获取当前显示设备的设备上下文。
      /// </summary>
      public IntPtr DeviceContext => Graphics.FromHwnd(IntPtr.Zero).GetHdc();
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
      /// 获取或设置当前图形监视器的像素深度。
      /// </summary>
      public int BitsPerPixel
      {
         get => _thisComputer.Screen.BitsPerPixel;
         set => ChangeResolving((int)Width, (int)Height, RefreshRate, value);
      }
      /// <summary>
      /// 获取或设置当前显示器的刷新率。
      /// </summary>
      public int RefreshRate
      {
         get
         {
            IntPtr desktopDC = GetDC(GetDesktopWindow());
            return GetDeviceCaps(desktopDC, 116);
         }
         set => ChangeResolving((int)Width, (int)Height, value, BitsPerPixel);
      }
      /// <summary>
      /// 获取当前实例所对应的主显示器是否是触摸屏。
      /// </summary>
      /// <param name="numberOfTouchPoint">该参数会传出一个硬件能支持的触摸点的数量，如果硬件不支持触摸屏，则该参数值为0。</param>
      /// <returns>该操作将会返回一个Boolean值，这个值用于表示当前设备是否支持触摸屏操作，如果这个值为true，则表示该显示设备支持触摸操作，否则表示该设备不支持触摸操作，或者触摸设备被操作系统禁用或卸载，或者触摸设备已损坏或未检测到。</returns>
      public bool IsTouchScreen(out int numberOfTouchPoint)
      {
         bool isTouch = true;
         byte digitizerStatus = (byte)GetSystemMetrics(SM_DIGITIZER);
         if ((digitizerStatus & (0x80 + 0x40)) == 0)
         {
            isTouch = false;
            numberOfTouchPoint = 0;
         }
         else numberOfTouchPoint = GetSystemMetrics(SM_MAXIMUMTOUCHES);
         return isTouch;
      }
      /// <summary>
      /// 获取当前计算机显示器的全屏幕快照。
      /// </summary>
      /// <param name="isCopyToClipboard">指示是否将获取的屏幕快照写入Windows剪贴板。</param>
      /// <returns>该操作会返回一个和显示器尺寸相同的Bitmap格式的屏幕快照。</returns>
      public Bitmap Snap(bool isCopyToClipboard) => Snap(LTCoordinate, RBCoordinate, isCopyToClipboard);
      /// <summary>
      /// 获取当前计算机显示器的局部屏幕快照，该方法与显示设备没有太多关联，因此多个Monitor实例进行屏幕快照操作是一件没有意义的事情。
      /// </summary>
      /// <param name="ltPoint">获取屏幕快照的左上角坐标。</param>
      /// <param name="rbPoint">获取屏幕快照的右下角坐标。</param>
      /// <param name="isCopyToClipboard">指示是否将获取的屏幕快照写入Windows剪贴板。</param>
      /// <returns>该操作会返回一个指定区域的Bitmap格式的屏幕快照。</returns>
      public Bitmap Snap(ExPoint2D ltPoint, ExPoint2D rbPoint, bool isCopyToClipboard)
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
      /// 设置当前图形监视器的分辨率。
      /// </summary>
      /// <param name="width">指定分辨率的宽度。</param>
      /// <param name="height">指定分辨率的高度。</param>
      /// <returns>该操作会返回一个Win32Api错误代码，如果这个错误代码为非0，则表示操作失败，否则表示操作成功。</returns>
      public long ChangeResolving(int width, int height) => ChangeResolving(width, height, RefreshRate, BitsPerPixel);
      /// <summary>
      /// 设置当前图形监视器的分辨率，并重新指定屏幕刷新率和像素色彩深度。
      /// </summary>
      /// <param name="width">指定分辨率的宽度。</param>
      /// <param name="height">指定分辨率的高度。</param>
      /// <param name="refreshRate">指定的屏幕刷新率。</param>
      /// <param name="bitsPerPixel">指定的像素色彩深度。</param>
      /// <returns>该操作会返回一个Win32Api错误代码，如果这个错误代码为非0，则表示操作失败，否则表示操作成功。</returns>
      public long ChangeResolving(int width, int height, int refreshRate, int bitsPerPixel)
      {
         SDevicesMode devM = new SDevicesMode
         {
            Size = (short)Marshal.SizeOf(typeof(SDevicesMode))
         };
         bool mybool = EnumDisplaySettings(null, 0, ref devM);
         devM.PelsHeight = height;
         devM.PelsWidth = width;
         devM.DisplayFrequency = refreshRate;
         devM.BitsPerPel = bitsPerPixel;
         long result = ChangeDisplaySettings(ref devM, 0);
         return result;
      }
      /// <summary>
      /// 从Gamma值的基础上设置当前显示设备的亮度。
      /// </summary>
      /// <param name="gamma">需要调整的Gamma值，有效范围是[0, 255]。</param>
      /// <returns>该操作如果达到了用户需要的效果，则会返回true，否则会返回false。</returns>
      public unsafe bool ChangeBrightnessWithGamma(short gamma)
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
               if (arrayVal > ushort.MaxValue) arrayVal = ushort.MaxValue;
               *idx = (short)arrayVal;
               idx++;
            }
         }
         return SetDeviceGammaRamp(_deviceContext, gArray);
      }
      /// <summary>
      /// 重置当前显示设备的亮度，该操作是基于ChangeBrightnessWithGamma方法实现的。
      /// </summary>
      public void ResetBrightnessWithGamma() => ChangeBrightnessWithGamma(SYSTEM_DEFAULT_BRIGHTNESS_WITH_GAMMA);
   }
}
