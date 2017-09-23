using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Management;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace Cabinink.Network
{
   /// <summary>
   /// 网络诊断相关操作的不可继承类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class InternetDiagnosticsTool
   {
      private const int DEFAULT_PING_LIMIT = 4;//默认的PING操作次数。
      private const int DEFAULT_TIMEOUT_TICKS = 100;//默认的PING操作超时时间。
      private const string LOCAL_HOST_ADDRESS = @"127.0.0.1";//本地主机地址，即回环地址。
      /// <summary>
      /// 向回环地址发送四次ICMP数据包（Ping）。
      /// </summary>
      /// <returns>所有操作完成之后将会返回一个浮点数，这个浮点数代表了整个操作的成功率，如果发送四次ICMP数据包，有三次成功，则返回0.75，换句话说就是75%的成功率。</returns>
      public static double Ping()
      {
         return Ping(LOCAL_HOST_ADDRESS, DEFAULT_PING_LIMIT, DEFAULT_TIMEOUT_TICKS);
      }
      /// <summary>
      /// 向指定的目标地址发送指定次数和指定超时时间的ICMP数据包。
      /// </summary>
      /// <param name="target">接收主机数据包的目标地址或者域名，换句话说，在这里域名和IP地址是可以通用的，比如说127.0.0.1和localhost是等效的。</param>
      /// <param name="limit">发送ICMP数据包的次数，即Ping的次数。</param>
      /// <param name="timeOut">以毫秒为单位的超时间隔。默认值为100毫秒。</param>
      /// <returns></returns>
      public static double Ping(string target, int limit, int timeOut)
      {
         Ping ping = new Ping();
         double pDone = 0;
         for (int i = 0; i < limit; i++)
         {
            if (ping.Send(target, timeOut).Status == IPStatus.Success) pDone += 1;
         }
         return pDone / limit;
      }
      /// <summary>
      /// 尝试将包含指定数据缓冲区的Internet控制消息协议(ICMP)回送消息发送到具有指定地址的计算机，并接收来自该计算机的相应ICMP回送答复消息。此重载允许您指定操作的超时值，并控制ICMP回显消息数据包的碎片和生存时间值。
      /// </summary>
      /// <param name="target">接收主机数据包的目标地址或者域名，换句话说，在这里域名和IP地址是可以通用的，比如说127.0.0.1和localhost是等效的。</param>
      /// <param name="timeOut">以毫秒为单位的超时间隔。默认值为100毫秒。</param>
      /// <param name="buffer">一个Byte数组，它包含要与ICMP回送消息一起发送并在ICMP回送应答消息中返回的数据。该数组包含的字节数不能超过65500个字节。</param>
      /// <param name="pingOptions">用于控制ICMP回显消息数据包的碎片和生存时间值的PingOptions对象。</param>
      /// <returns>一个PingReply对象。如果已收到ICMP回送应答消息，此对象将提供有关该消息的信息；如果没有收到ICMP回送应答消息，此对象将提供失败的原因。如果数据包的大小超过最大传输单元(MTU)，该方法将返回PacketTooBig。</returns>
      public static PingReply Ping(string target, int timeOut, byte[] buffer, PingOptions pingOptions)
      {
         return new Ping().Send(target, timeOut, buffer, pingOptions);
      }
      /// <summary>
      /// 检测当前计算机所在的网络是否可用。
      /// </summary>
      /// <returns>如果当前网络可用则 返回true，否则将会返回false。</returns>
      /// <remarks>这个操作只能够检测到当前计算机是否接入某个局域网，或者是否被所接入的的网关或者ISP分配了有效的IP地址，但却不能检测当前网络的连通性，换句话说，这个操作是基于整个局域网或者相对于整个网关环境的，而非针对于整个互联网。如果要检测当前所在网络的连通性，请使用Ping方法或者Ping方法的重载版本。</remarks>
      public static bool NetworkAvailable()
      {
         return new Microsoft.VisualBasic.Devices.Network().IsAvailable;
      }
      /// <summary>
      /// 获取当前计算机所在网络的网关地址信息。
      /// </summary>
      /// <returns>如果操作无异常，该方法将会返回一个或者多个网关地址。</returns>
      /// <exception cref="NetworkNotAvailableException">如果当前网络不可用时，则会抛出该异常。</exception>
      public static string[] GetGatewayAddresses()
      {
         if (!NetworkAvailable()) throw new NetworkNotAvailableException();
         ManagementObjectCollection managementObjects;
         managementObjects = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
         List<string> gatewayAddresses = new List<string>();
         foreach (ManagementObject managementObject in managementObjects)
         {
            if (Convert.ToBoolean(managementObject["ipEnabled"]))
            {
               gatewayAddresses.Add(managementObject["DefaultIPGateway"].ToString());
            }
         }
         return gatewayAddresses.ToArray();
      }
      /// <summary>
      /// 返回指定主机的Internet协议地址（IP地址）。
      /// </summary>
      /// <param name="target">需要解析的主机名或IP地址。</param>
      /// <returns>一个string类型的List泛型列表，该类型保存由target参数指定的主机的IP地址。</returns>
      /// <exception cref="NetworkNotAvailableException">如果当前网络不可用时，则会抛出这个异常。</exception>
      public static List<string> GetHostAddress(string target)
      {
         if (!NetworkAvailable()) throw new NetworkNotAvailableException();
         List<string> ipAddressStringCollection = new List<string>();
         List<IPAddress> ipAddressCollection = Dns.GetHostAddresses(target).ToList();
         for (int i = 0; i < ipAddressCollection.Count; i++)
         {
            ipAddressStringCollection[i] = ipAddressCollection[i].ToString();
         }
         return ipAddressStringCollection;
      }
      /// <summary>
      /// 获取当前计算机的外部网络IP地址。
      /// </summary>
      /// <returns>该操作将会返回当前计算机所在网络的外部IP地址的字符串表达形式。</returns>
      public static string GetOutsideNetAddress() => GetOutsideNetAddress(@"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
      /// <summary>
      /// 获取当前计算机的外部网络IP地址，但是这个方法需要指定一个有效的UA字符串。
      /// </summary>
      /// <param name="userAgent">指定该方法的用户代理（User Agent）字符串。</param>
      /// <returns>该操作将会返回当前计算机所在网络的外部IP地址的字符串表达形式。</returns>
      public static string GetOutsideNetAddress(string userAgent)
      {
         string outsideIpAddress = string.Empty;
         HttpWebRequest httpWebRequest = null;
         HttpWebResponse httpWebResponse = null;
         Encoding encoding = Encoding.GetEncoding("gb2312");
         try
         {
            httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.ip138.com/ips138.asp");
            httpWebRequest.Accept = "*/*";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = userAgent;
            httpWebRequest.Timeout = 2 * 60 * 1000;
            httpWebRequest.Headers["Accept-Encoding"] = "gzip,deflate";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
               Stream responseStream = httpWebResponse.GetResponseStream();
               StreamReader streamReader = new StreamReader(responseStream, encoding);
               string html = streamReader.ReadToEnd();
               Match match;
               string pattern = "(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)";
               match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
               outsideIpAddress = match.ToString();
               streamReader.Close();
               responseStream.Close();
               httpWebRequest.Abort();
               httpWebResponse.Close();
            }
         }
         catch
         {
            if (httpWebRequest != null) httpWebRequest.Abort();
            if (httpWebResponse != null) httpWebResponse.Close();
         }
         return outsideIpAddress;
      }
   }
   /// <summary>
   /// 当目前的网络不可用时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class NetworkNotAvailableException : Exception
   {
      public NetworkNotAvailableException() : base("当前网络不可用！") { }
      public NetworkNotAvailableException(string message) : base(message) { }
      public NetworkNotAvailableException(string message, Exception inner) : base(message, inner) { }
      protected NetworkNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
