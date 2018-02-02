using System;
using System.IO;
using Cabinink.IOSystem;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cabinink
{
   /// <summary>
   /// 该类用于获取程序集的基础程序集信息
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class AssemblyInformation
   {
      private string _assemblyUrl;//指定程序集所在的文件地址。
      /// <summary>
      /// 构造函数，创建一个指定程序集文件地址的程序集信息类实例。
      /// </summary>
      /// <param name="assemblyUrl">指定的程序集所在的文件地址。</param>
      /// <exception cref="FileNotFoundException">当参数assemblyUrl指定的文件不存在时，则将会抛出这个异常。</exception>
      public AssemblyInformation(string assemblyUrl)
      {
         if (!FileOperator.FileExists(assemblyUrl)) throw new FileNotFoundException();
         else _assemblyUrl = assemblyUrl;
      }
      /// <summary>
      /// 获取或设置当前实例的程序集文件地址。
      /// </summary>
      public string AssemblyUrl { get => _assemblyUrl; set => _assemblyUrl = value; }
      /// <summary>
      /// 根据当前实例中包含的程序集文件地址来获取相对应的程序集。
      /// </summary>
      /// <returns>该操作会获取一个Assembly实例，这个实例用于表示一个程序集。</returns>
      public Assembly GetAssembly() => Assembly.LoadFile(AssemblyUrl);
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的友好名称。
      /// </summary>
      public string ThisApplicationName => GetAssembly().GetName().Name;
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的完全限定名称。
      /// </summary>
      public string ThisAssemblyFullName => GetAssembly().FullName;
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的运行时版本。
      /// </summary>
      public string ThisRuntimeVersion => GetAssembly().ImageRuntimeVersion.Substring(1);
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的文件版本。
      /// </summary>
      public string ThisFileVersion => FileVersionInfo.GetVersionInfo(GetAssembly().Location).FileVersion;
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的产品版本。
      /// </summary>
      public string ThisProductionVersion => FileVersionInfo.GetVersionInfo(GetAssembly().Location).ProductVersion;
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的版权信息字符串。
      /// </summary>
      public string ThisCopyright => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(GetAssembly(), typeof(AssemblyCopyrightAttribute))).Copyright;
      /// <summary>
      /// 获取当前实例指定地址所对应程序集的所在目录。
      /// </summary>
      public string ThisAssemblyDirectory => FileOperator.GetFatherDirectory(AssemblyUrl);
      /// <summary>
      /// 获取当前应用程序（这里的应用程序指的是Cabinink Common Library）的友好名称。
      /// </summary>
      public static string ApplicationName => CurrentAssembly.GetName().Name;
      /// <summary>
      /// 获取当前应用程序（这里的应用程序指的是Cabinink Common Library）的完全限定名称。
      /// </summary>
      public static string AssemblyFullName => CurrentAssembly.FullName;
      /// <summary>
      /// 获取支持当前程序集（这里的程序集指的是Cabinink Common Library）运行的公共语言运行时版本的字符串表达形式。
      /// </summary>
      public static string RuntimeVersion => CurrentAssembly.ImageRuntimeVersion.Substring(1);
      /// <summary>
      /// 获取当前程序集（这里的程序集指的是Cabinink Common Library）的文件版本的字符串表达形式。
      /// </summary>
      public static string FileVersion => FileVersionInfo.GetVersionInfo(CurrentAssembly.Location).FileVersion;
      /// <summary>
      /// 获取当前程序集（这里的程序集指的是Cabinink Common Library）的产品版本的字符串表达形式。
      /// </summary>
      public static string ProductionVersion => FileVersionInfo.GetVersionInfo(CurrentAssembly.Location).ProductVersion;
      /// <summary>
      /// 获取编码与生成当前程序集（这里的程序集指的是Cabinink Common Library）的集成开发环境的名称。
      /// </summary>
      public static string IdeName => @"Microsoft Visual Studio";
      /// <summary>
      /// 获取当前程序集（这里的程序集指的是Cabinink Common Library）的版权信息。
      /// </summary>
      public static string Copyright => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(CurrentAssembly, typeof(AssemblyCopyrightAttribute))).Copyright;
      /// <summary>
      /// 获取当前应用程序（这里的应用程序指的是Cabinink Common Library）所在的目录。
      /// </summary>
      public static string AssemblyDirectory => Environment.CurrentDirectory;
      /// <summary>
      /// 获取包含当前执行的代码的程序集（这里的程序集指的是Cabinink Common Library）。
      /// </summary>
      private static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();
   }
}
