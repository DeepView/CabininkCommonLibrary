using System;
using System.Reflection;
using System.Diagnostics;
namespace Cabinink
{
   /// <summary>
   /// 该类用于获取当前程序集（在这里泛指Cabinink Common Library，CCL）的基础程序集信息
   /// </summary>
   public sealed class AssemblyInformation
   {
      /// <summary>
      /// 获取当前应用程序的友好名称
      /// </summary>
      public static string ApplicationName => CurrentAssembly.GetName().Name;
      /// <summary>
      /// 获取当前应用程序的完全限定名称
      /// </summary>
      public static string AssemblyFullName => CurrentAssembly.FullName;
      /// <summary>
      /// 获取支持当前程序集运行的公共语言运行时版本的字符串表达形式
      /// </summary>
      public static string RuntimeVersion => CurrentAssembly.ImageRuntimeVersion.Substring(1);
      /// <summary>
      /// 获取当前程序集的文件版本的字符串表达形式
      /// </summary>
      public static string FileVersion => FileVersionInfo.GetVersionInfo(CurrentAssembly.Location).FileVersion;
      /// <summary>
      /// 获取当前程序集的产品版本的字符串表达形式
      /// </summary>
      public static string ProductionVersion => FileVersionInfo.GetVersionInfo(CurrentAssembly.Location).ProductVersion;
      /// <summary>
      /// 获取编码与生成当前应用程序或者程序集的集成开发环境的名称
      /// </summary>
      public static string IdeName => @"Microsoft Visual Studio";
      /// <summary>
      /// 获取当前程序集的版权信息
      /// </summary>
      public static string Copyright => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(CurrentAssembly, typeof(AssemblyCopyrightAttribute))).Copyright;
      /// <summary>
      /// 获取当前应用程序所在的目录
      /// </summary>
      public static string AssemblyDirectory => Environment.CurrentDirectory;
      /// <summary>
      /// 获取包含当前执行的代码的程序集
      /// </summary>
      private static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();
   }
}
