using System;
using Cabinink.IOSystem;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Applications
{
   /// <summary>
   /// 应用程序配置基类，用于实现一个应用程序的配置存储、应用、读取和保存等功能，不过这个类必须被继承下去，不可作为实例应用。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public abstract class AppSettings : IApplicationBasicIO
   {
      private string _appSettingFileUrl;//应用程序配置文件路径。
      /// <summary>
      /// 构造函数，创建一个指定应用程序配置文件的应用程序配置实例。
      /// </summary>
      /// <param name="appSettingFileUrl">指定的应用程序配置文件地址，如果这个文件地址不存在，那么会自动创建一个空的配置文件。</param>
      public AppSettings(string appSettingFileUrl)
      {
         if (!FileOperator.FileExists(appSettingFileUrl)) FileOperator.CreateFile(appSettingFileUrl);
         _appSettingFileUrl = appSettingFileUrl;
      }
      /// <summary>
      /// 获取当前实例所表示的应用程序配置的配置文件路径。
      /// </summary>
      public string SettingsFile => _appSettingFileUrl;
      /// <summary>
      /// 从当前实例包含的文件中读取应用程序的配置字段。
      /// </summary>
      /// <param name="exceptionInformation">如果该方法产生了异常，那么这个参数将会保存一个异常信息供开发者参考。</param>
      /// <returns>如果操作没有任何异常发生，切操作成功，将会返回true，否则返回false。</returns>
      public abstract bool Read(ref string exceptionInformation);
      /// <summary>
      /// 将当前实例包含的所有配置字段存储到这个实例包含的配置文件之中。
      /// </summary>
      /// <param name="exceptionInformation">如果该方法产生了异常，那么这个参数将会保存一个异常信息供开发者参考。</param>
      /// <returns>如果操作没有任何异常发生，切操作成功，将会返回true，否则返回false。</returns>
      public abstract bool Write(ref string exceptionInformation);
      /// <summary>
      /// 加载应用程序的默认配置并从配置字段的级别上覆盖当前的应用程序配置实例。
      /// </summary>
      /// <param name="settings">应用程序的默认配置。</param>
      public abstract void LoadDefaultSettings(AppSettings settings);
   }
}
