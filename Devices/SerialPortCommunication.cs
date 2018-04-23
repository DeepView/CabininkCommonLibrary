using System;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Cabinink.TypeExtend;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.Devices
{
   /// <summary>
   /// COM串行端口的基础操作实现类，实现了打开和关闭COM端口等操作。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class SerialPortCommunication : IDisposable
   {
      private SerialPort _comDevice;//COM串行端口设备。
      /// <summary>
      /// 构造函数，创建一个默认构造的SerialPortCommunication实例。
      /// </summary>
      public SerialPortCommunication() => _comDevice = new SerialPort();
      /// <summary>
      /// 构造函数，创建一个指定端口名称的SerialPortCommunication实例。
      /// </summary>
      /// <param name="serialPortName">指定的端口名称。</param>
      public SerialPortCommunication(string serialPortName) => _comDevice = new SerialPort(serialPortName);
      /// <summary>
      /// 构造函数，创建一个指定端口名称和波特率的SerialPortCommunication实例。
      /// </summary>
      /// <param name="serialPortName">指定的端口名称。</param>
      /// <param name="baudRate">指定的波特率。</param>
      public SerialPortCommunication(string serialPortName, int baudRate) => _comDevice = new SerialPort(serialPortName, baudRate);
      /// <summary>
      /// 获取或设置当前实例的端口设备实例。
      /// </summary>
      public SerialPort COMDevice { get => _comDevice; set => _comDevice = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的串行端口的端口名称。
      /// </summary>
      public string Name { get => COMDevice.PortName; set => COMDevice.PortName = value; }
      /// <summary>
      /// 获取当前实例所表示的串行端口的端口状态，如果为true则表示端口已打开，否则表示端口已关闭。
      /// </summary>
      public bool IsOpened => COMDevice.IsOpen;
      /// <summary>
      /// 获取当前计算机所有的串行端口名称。
      /// </summary>
      public List<string> SerialPortNames => SerialPort.GetPortNames().ToList();
      /// <summary>
      /// 用默认的方式打开当前的串行端口。
      /// </summary>
      /// <returns>如果该操作正常或者成功打开端口，则会返回true，否则会返回false。</returns>
      public bool Open() => Open(COMDevice.BaudRate, Parity.None, 8, StopBits.One);
      /// <summary>
      /// 在重定义波特率，奇偶校验检查协议，数据位长度和停止位数的前提下打开当前的串行端口。
      /// </summary>
      /// <param name="baudRate">指定的串行波特率。</param>
      /// <param name="parity">指定的串行端口奇偶校验检查协议。</param>
      /// <param name="dataBits">指定的字节标准数据位长度。</param>
      /// <param name="stopBits">指定的字节标准停止位数。</param>
      /// <returns>如果该操作正常或者成功打开端口，则会返回true，否则会返回false。</returns>
      /// <exception cref="NotFoundAnyCOMDeviceException">当实例未找到任何COM设备或者可用接口时，则将会抛出这个异常。</exception>
      public bool Open(int baudRate, Parity parity, int dataBits, StopBits stopBits)
      {
         bool result = false;
         if (SerialPortNames.Count <= 0) throw new NotFoundAnyCOMDeviceException();
         else
         {
            if (!IsOpened)
            {
               COMDevice.BaudRate = baudRate;
               COMDevice.Parity = parity;
               COMDevice.DataBits = dataBits;
               COMDevice.StopBits = stopBits;
               try
               {
                  COMDevice.Open();
                  result = true;
               }
               catch (Exception throwedException)
               {
                  if (throwedException != null) throw throwedException.InnerException;
                  else return false;
               }
            }
            if (!IsOpened) result = false;
            return result;
         }
      }
      /// <summary>
      /// 关闭当前的端口。
      /// </summary>
      /// <returns>如果该操作正常或者成功关闭端口，则会返回true，否则会返回false。</returns>
      public bool Close()
      {
         COMDevice.Close();
         return !IsOpened;
      }
      /// <summary>
      /// 向端口发送一组数据。
      /// </summary>
      /// <param name="data">需要被发送的数据。</param>
      /// <returns>如果这组数据已经被发送出去，则将会返回true，否则返回false。</returns>
      public bool SendData(byte[] data) => SendData(data, 0, data.Length);
      /// <summary>
      /// 向端口发送一组数据，并指定偏移量和发送量。
      /// </summary>
      /// <param name="data">需要被发送的数据。</param>
      /// <param name="offset">指定的偏移量。</param>
      /// <param name="count">指定的发送量。</param>
      /// <returns>如果这组数据已经被发送出去，则将会返回true，否则返回false。</returns>
      /// <exception cref="NotFoundEnabledPortOrInterfaceException">当串行端口或者相关接口未启用时，则将会抛出这个异常。</exception>
      public bool SendData(byte[] data, int offset, int count)
      {
         bool result = false;
         if (!IsOpened) throw new NotFoundEnabledPortOrInterfaceException();
         try
         {
            COMDevice.Write(data, offset, count);
            result = true;
         }
         catch (Exception throwedException) { if (throwedException != null) result = false; }
         return result;
      }
      /// <summary>
      /// 向端口发送一组原始的数据，并指定偏移量和发送量，以及数据转换的类型。
      /// </summary>
      /// <param name="originalData">需要被发送的原始数据。</param>
      /// <param name="offset">指定的偏移量。</param>
      /// <param name="count">指定的发送量。</param>
      /// <param name="sendType">需要被转换的类型。</param>
      /// <returns>如果这组数据已经被发送出去，则将会返回true，否则返回false。</returns>
      public bool SendData(string originalData, int offset, int count, EDataConvertedType sendType)
      {
         byte[] data;
         originalData.Trim();
         switch (sendType)
         {
            case EDataConvertedType.Hexadecimal:
               data = ((ExString)originalData).ToHexadecimalArray();
               break;
            case EDataConvertedType.AsciiCode:
               data = Encoding.ASCII.GetBytes(originalData);
               break;
            case EDataConvertedType.UTF8Code:
               data = Encoding.UTF8.GetBytes(originalData);
               break;
            case EDataConvertedType.Unicode:
               data = Encoding.Unicode.GetBytes(originalData);
               break;
            default:
               data = ((ExString)originalData).ToHexadecimalArray();
               break;
         }
         return SendData(data, offset, count);
      }
      /// <summary>
      /// 从端口接收一组数据。
      /// </summary>
      /// <returns>如果接收到了数据，则这个操作将会返回已经获取的数据所对应字节数组表达形式。</returns>
      public byte[] ReceiveData() => ReceiveData(0, COMDevice.BytesToRead);
      /// <summary>
      /// 从端口接收一组数据，并在接收数据之前指定偏移量和接收量。
      /// </summary>
      /// <param name="offset">指定的偏移量。</param>
      /// <param name="count">指定的接收量。</param>
      /// <returns>如果接收到了数据，则这个操作将会返回已经获取的数据所对应字节数组表达形式。</returns>
      public byte[] ReceiveData(int offset, int count)
      {
         byte[] received = new byte[COMDevice.BytesToRead];
         COMDevice.Read(received, offset, count);
         return received;
      }
      /// <summary>
      /// 获取当前实例的字符串表达形式。
      /// </summary>
      /// <returns>这个方法会返回当前实例的全局名称，即包含命名空间的类名称。</returns>
      public override string ToString() => GetType().Namespace + ".SerialPortCommunication";
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose()
      {
         if (IsOpened) Close();
         ((IDisposable)COMDevice).Dispose();
      }
   }
   /// <summary>
   /// 数据发送或者接收的类型的枚举。
   /// </summary>
   public enum EDataConvertedType : int
   {
      /// <summary>
      /// 十六进制数据。
      /// </summary>
      [EnumerationDescription("Hex")]
      Hexadecimal = 0x0000,
      /// <summary>
      /// ASCII数据。
      /// </summary>
      [EnumerationDescription("ASCII")]
      AsciiCode = 0x0001,
      /// <summary>
      /// UTF8编码的字符数据。
      /// </summary>
      [EnumerationDescription("UTF-8")]
      UTF8Code = 0x0002,
      /// <summary>
      /// Unicode编码的字符数据。
      /// </summary>
      [EnumerationDescription("Unicode")]
      Unicode = 0x0003
   }
   /// <summary>
   /// 当没找到任何已经打开的端口或者接口时用于抛出的异常。
   /// </summary>
   [Serializable]
   public class NotFoundEnabledPortOrInterfaceException : Exception
   {
      public NotFoundEnabledPortOrInterfaceException() : base("未找到任何启用或者打开的端口或者接口！") { }
      public NotFoundEnabledPortOrInterfaceException(string message) : base(message) { }
      public NotFoundEnabledPortOrInterfaceException(string message, Exception inner) : base(message, inner) { }
      protected NotFoundEnabledPortOrInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   [Serializable]
   public class NotFoundAnyCOMDeviceException : Exception
   {
      public NotFoundAnyCOMDeviceException() : base("未找到任何串行端口或者相关设备！") { }
      public NotFoundAnyCOMDeviceException(string message) : base(message) { }
      public NotFoundAnyCOMDeviceException(string message, Exception inner) : base(message, inner) { }
      protected NotFoundAnyCOMDeviceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
