using System;
using System.IO;
using Cabinink.IOSystem;
using IWshRuntimeLibrary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using Cabinink.DataTreatment.DocumentData;
namespace Cabinink.Windows.Applications
{
   [Serializable]
   [ComVisible(true)]
   public class ShortcutCreator
   {
      private string _name;//快捷方式的名称。
      private Uri _webOrFileUrl;//快捷方式所对应的源文件或者Web地址。
      private string _localDirectory;//如果_webOrFileUrl所封装的是本地文件地址，则该字段会保存这个文件地址所在的目录。
      private string _icon;//图标文件所在的本地文件地址，如果这个文件包含了多个图标，则以“icon_source_file, icon_index”的字符串格式更新该字段。
      private string _description;//注释内容，用于保存其他相关的信息。
      /// <summary>
      /// 构造函数，创建一个空的快捷方式生成实例。
      /// </summary>
      public ShortcutCreator()
      {
         _name = "Shortcut_" + base.GetHashCode();
         _webOrFileUrl = new Uri("http://localhost/");
         _localDirectory = string.Empty;
         _icon = null;
         _description = _name;
      }
      /// <summary>
      /// 构造函数，创建一个指定快捷方式名称和指向源地址的快捷方式生成实例。
      /// </summary>
      /// <param name="name">指定的快捷方式名称。</param>
      /// <param name="webOrFileUrl">快捷方式所对应的源文件或者Web地址。</param>
      public ShortcutCreator(string name, string webOrFileUrl)
      {
         _name = name;
         _webOrFileUrl = new Uri(webOrFileUrl);
         _localDirectory = FileOperator.GetFatherDirectory(webOrFileUrl);
         _icon = null;
         _description = webOrFileUrl;
      }
      /// <summary>
      /// 构造函数，创建一个指定快捷方式名称，指向源地址，工作目录，图标文件地址和注释的快捷方式生成实例。
      /// </summary>
      /// <param name="name">指定的快捷方式名称。</param>
      /// <param name="webOrFileUrl">快捷方式所对应的源文件或者Web地址。</param>
      /// <param name="localDirectory">如果_webOrFileUrl所封装的是本地文件地址，则该字段会保存这个文件地址所在的目录。</param>
      /// <param name="iconFileUrl">图标文件所在的本地文件地址，如果这个文件包含了多个图标，则以“icon_source_file, icon_index”的字符串格式更新该字段。</param>
      /// <param name="description">指定的注释内容，用于保存其他相关的信息。</param>
      public ShortcutCreator(string name, string webOrFileUrl, string localDirectory, string iconFileUrl, string description)
      {
         _name = name;
         _webOrFileUrl = new Uri(webOrFileUrl);
         _localDirectory = localDirectory;
         _icon = iconFileUrl;
         _description = description;
      }
      /// <summary>
      /// 获取或设置当前快捷方式的名称。
      /// </summary>
      public string Name { get => _name; set => _name = value; }
      /// <summary>
      /// 获取或设置当前快捷方式所对应的源文件或者Web地址。
      /// </summary>
      /// <exception cref="FileNotFoundException">当指定的文件不存在时，则会抛出这个异常。</exception>
      public Uri SourceUrl
      {
         get => _webOrFileUrl;
         set
         {
            if (value.IsFile && !FileOperator.FileExists(value.LocalPath))
            {
               throw new FileNotFoundException("指定的文件找不到", value.LocalPath);
            }
            _webOrFileUrl = value;
         }
      }
      /// <summary>
      /// 获取或设置当前快捷方式的本地文件地址所在的目录，但这个属性需要在SourceUrl字段保存了本地文件地址的情况下才会有效。
      /// </summary>
      /// <exception cref="DirectoryNotFoundException">当指定的目录不存在时，则会抛出这个异常。</exception>
      public string SourceDirectory
      {
         get => _localDirectory;
         set
         {
            if (!FileOperator.DirectoryExists(value)) throw new DirectoryNotFoundException("找不到这个目录！");
            else _localDirectory = value;
         }
      }
      /// <summary>
      /// 获取或设置当前快捷方式的图标。
      /// </summary>
      /// <remarks>如果指定的文件包含了多个图标，则以“icon_source_file, icon_index”的字符串格式更新该属性，另外，如果不指定图标，则这个属性设置为null即可。</remarks>
      public string Icon { get => _icon; set => _icon = value; }
      /// <summary>
      /// 获取或设置当前实例快捷方式的注释内容。
      /// </summary>
      public string Description { get => _description; set => _description = value; }
      /// <summary>
      /// 获取当前快捷方式的源地址是否为本地文件。
      /// </summary>
      public bool IsLocalFileShortcut => SourceUrl.IsFile;
      /// <summary>
      /// 根据指定的目标目录和窗口启动样式来创建快捷方式。
      /// </summary>
      /// <param name="shortcutDirectory">用于保存快捷方式的目录，比如说桌面，或者其他目录，举例：C:\Users\Cabinink\Desktop\。</param>
      /// <param name="windowStyle">这个参数用于决定打开快捷方式后，窗口该如何显示，可以为以下任意值之一：
      /// <para>WshWindowStyle.WshHide：0，隐藏窗体运行。</para>
      /// <para>WshWindowStyle.WshNormalFocus：1，通过正常的模式打开窗体。</para>
      /// <para>WshWindowStyle.WshMinimizedFocus：2，最小化窗体运行。</para>
      /// <para>WshWindowStyle.WshMaximizedFocus：3，最大化窗体运行。</para>
      /// <para>WshWindowStyle.WshNormalNoFocus：4，以正常的模式打开窗体，但是会取消焦点。</para>
      /// <para>WshWindowStyle.WshMinimizedNoFocus：6，最小化窗体运行，但是会取消焦点。</para>
      /// </param>
      /// <returns>该操作如果成功的创建了这个快捷方式，则会返回true，否则将会返回false，这个检测是基于文件存在性来实现的。</returns>
      [CLSCompliant(false)]
      public bool CreateLocalShortcut(string shortcutDirectory, EWindowStyle windowStyle)
      {
         WshShell shell = new WshShell();
         string shortcutUrl = Path.Combine(shortcutDirectory, string.Format("{0}.lnk", Name));
         IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutUrl);
         shortcut.TargetPath = SourceUrl.LocalPath;
         shortcut.WorkingDirectory = Path.GetDirectoryName(SourceDirectory);
         shortcut.WindowStyle = (int)windowStyle;
         shortcut.Description = Description;
         shortcut.IconLocation = string.IsNullOrWhiteSpace(Icon) ? SourceUrl.LocalPath : Icon;
         if (!IsLocalFileShortcut) throw new UrlIsNotLocalFileException();
         else shortcut.Save();
         return FileOperator.FileExists(shortcutUrl);
      }
      /// <summary>
      /// 根据指定的窗口启动样式在桌面创建快捷方式。
      /// </summary>
      /// <param name="windowStyle">这个参数用于决定打开快捷方式后，窗口该如何显示，可以为以下任意值之一：
      /// <para>WshWindowStyle.WshHide：0，隐藏窗体运行。</para>
      /// <para>WshWindowStyle.WshNormalFocus：1，通过正常的模式打开窗体。</para>
      /// <para>WshWindowStyle.WshMinimizedFocus：2，最小化窗体运行。</para>
      /// <para>WshWindowStyle.WshMaximizedFocus：3，最大化窗体运行。</para>
      /// <para>WshWindowStyle.WshNormalNoFocus：4，以正常的模式打开窗体，但是会取消焦点。</para>
      /// <para>WshWindowStyle.WshMinimizedNoFocus：6，最小化窗体运行，但是会取消焦点。</para>
      /// </param>
      /// <returns>该操作如果成功的创建了这个快捷方式，则会返回true，否则将会返回false，这个检测是基于文件存在性来实现的。</returns>
      [CLSCompliant(false)]
      public bool CreateDesktopLocalShortcut(EWindowStyle windowStyle)
      {
         string desktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
         return CreateLocalShortcut(desktopDirectory, windowStyle);
      }
      /// <summary>
      /// 根据指定的目标目录创建指定的Internet快捷方式。
      /// </summary>
      /// <param name="shortcutDirectory">用于保存Internet快捷方式的目录，比如说桌面，或者其他目录，举例：C:\Users\Cabinink\Desktop\。</param>
      /// <returns>该操作如果成功的创建了这个Internet快捷方式，则会返回true，否则将会返回false，这个检测是基于文件存在性来实现的。</returns>
      public bool CreateInternetShortcut(string shortcutDirectory)
      {
         string shortcutUrl = Path.Combine(shortcutDirectory, string.Format("{0}.url", Name));
         FileOperator.CreateFile(shortcutUrl);
         InitializationFileOperator.WriteValue(shortcutUrl, "InternetShortcut", "URL", SourceUrl.AbsoluteUri);
         InitializationFileOperator.WriteValue(shortcutUrl, "InternetShortcut", "IconFile", Icon);
         InitializationFileOperator.WriteValue(shortcutUrl, "InternetShortcut", "IconIndex", "1");
         InitializationFileOperator.WriteValue(shortcutUrl, "InternetShortcut", "Roamed", "-1");
         return FileOperator.FileExists(shortcutUrl);
      }
      /// <summary>
      /// 在收藏夹里面创建指定的Internet快捷方式。
      /// </summary>
      /// <returns>该操作如果成功的创建了这个Internet快捷方式，则会返回true，否则将会返回false，这个检测是基于文件存在性来实现的。</returns>
      public bool CreateFavoriteInternetShortcut()
      {
         string favoritesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
         return CreateInternetShortcut(favoritesDirectory);
      }
   }
   /// <summary>
   /// 窗体显示模式枚举。
   /// </summary>
   public enum EWindowStyle : int
   {
      /// <summary>
      /// 隐藏窗体运行。
      /// </summary>
      [EnumerationDescription("隐藏")]
      Hide = 0x0000,
      /// <summary>
      /// 通过正常的模式打开窗体。
      /// </summary>
      [EnumerationDescription("常规")]
      NormalFocus = 0x0001,
      /// <summary>
      /// 最小化窗体运行。
      /// </summary>
      [EnumerationDescription("最小化")]
      MinimizedFocus = 0x0002,
      /// <summary>
      /// 最大化窗体运行。
      /// </summary>
      [EnumerationDescription("最大化")]
      MaximizedFocus = 0x0003,
      /// <summary>
      /// 以正常的模式打开窗体，但是会取消焦点。
      /// </summary>
      [EnumerationDescription("非活动常规")]
      NormalNoFocus = 0x0004,
      /// <summary>
      /// 最小化窗体运行，但是会取消焦点。
      /// </summary>
      [EnumerationDescription("非活动最小化")]
      MinimizedNoFocus = 0x0006
   }
   /// <summary>
   /// 当指定Uri实例包含的地址不是本地文件地址时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class UrlIsNotLocalFileException : Exception
   {
      public UrlIsNotLocalFileException() : base("指定地址无法识别为本地文件或资源。") { }
      public UrlIsNotLocalFileException(string message) : base(message) { }
      public UrlIsNotLocalFileException(string message, Exception inner) : base(message, inner) { }
      protected UrlIsNotLocalFileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
