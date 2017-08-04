using System;
using System.Management;
using Microsoft.VisualBasic.Devices;
using System.Runtime.InteropServices;
namespace Cabinink.Windows
{
   /// <summary>
   /// 用于获取当前Windows环境信息的类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class EnvironmentInformation
   {
      /// <summary>
      /// 获取操作系统的友好名称。
      /// </summary>
      public static string GetOperatingSystemName()
      {
         return new ComputerInfo().OSFullName;
      }
      /// <summary>
      /// 获取操作系统的版本字符串。
      /// </summary>
      public static string GetOperatingSystemVersion()
      {
         return new ComputerInfo().OSVersion;
      }
      /// <summary>
      /// 获取计算机的操作系统的平台标识符。
      /// </summary>
      public static string GetSystemPlatform()
      {
         return new ComputerInfo().OSPlatform;
      }
      /// <summary>
      /// 获取随操作系统安装的当前用户界面区域性文本。
      /// </summary>
      public static string GetInterfaceCultureString()
      {
         return new ComputerInfo().InstalledUICulture.NativeName;
      }
      /// <summary>
      /// 获取当前计算机的计算机名称（NetBIOS名称）。
      /// </summary>
      public static string GetComputerName()
      {
         return new Computer().Name;
      }
      /// <summary>
      /// 获取当前计算机所安装CPU的序列号。
      /// </summary>
      public static string GetProcessorSerial()
      {
         try
         {
            string cpuid = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
               cpuid = mo.Properties["ProcessorId"].Value.ToString();
            }
            moc = null;
            mc = null;
            return cpuid;
         }
         catch
         {
            return "无法获取CPU序列号";
         }
      }
      /// <summary>
      /// 获取当前计算机的类型文本。
      /// </summary>
      public static string GetComputerCategory()
      {
         try
         {
            string st = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
               st = mo["SystemType"].ToString();
            }
            moc = null;
            mc = null;
            return st;
         }
         catch
         {
            return "无法获取计算机类型";
         }
      }
   }
}
