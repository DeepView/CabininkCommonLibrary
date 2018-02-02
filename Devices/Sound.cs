using System;
using System.IO;
using Cabinink.IOSystem;
using Cabinink.TypeExtend;
using System.Runtime.InteropServices;
namespace Cabinink.Devices
{
   /// <summary>
   /// 音频播放类，用于在Windows指定有效的音频设备中播放音频。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Sound : IDisposable
   {
      private ExString _soundFileUrl;//音频资源文件的文件地址。
      private bool disposedValue = false;//检测冗余调用
      /// <summary>
      /// 用于在Windows中播放音频。
      /// </summary>
      /// <param name="sound">音频资源描述符。</param>
      /// <param name="instanceHandle">应用程序的实例句柄，除非sound的指向一个资源标识符（即flag被定义为SND_RESOURCE，在这里为ESoundFlgas.ResourceOrAtom），否则必须设置为NULL。</param>
      /// <param name="flag">标志的组合。</param>
      /// <returns>若成功则函数返回true，否则返回false。</returns>
      [DllImport("winmm.dll", SetLastError = true)]
      private static extern bool PlaySound(string sound, UIntPtr instanceHandle, uint flag);
      /// <summary>
      /// 构造函数，初始化一个指定音频资源文件的Sound实例。
      /// </summary>
      /// <param name="soundFileUrl">指定的音频资源文件。</param>
      /// <exception cref="FileNotFoundException">当参数soundFileUrl指定的文件未找到时，则会抛出这个异常。</exception>
      public Sound(ExString soundFileUrl)
      {
         if (!FileOperator.FileExists(soundFileUrl)) throw new FileNotFoundException("指定的文件找不到！", soundFileUrl);
         else _soundFileUrl = soundFileUrl;
      }
      /// <summary>
      /// 获取或设置当前实例的音频资源文件地址。
      /// </summary>
      public ExString SourceFile
      {
         get => _soundFileUrl;
         set
         {
            if (!FileOperator.FileExists(value)) throw new FileNotFoundException("指定的文件找不到！", value);
            else _soundFileUrl = value;
         }
      }
      /// <summary>
      /// 播放实例指定的音频文件。
      /// </summary>
      public void Play() => Play(ESoundFlags.FileName | ESoundFlags.Synchronously | ESoundFlags.NoStop);
      /// <summary>
      /// 播放实例指定的音频文件，并指定播放标识。
      /// </summary>
      /// <param name="flags">指定的标识或者标志组合。</param>
      [CLSCompliant(false)]
      public void Play(ESoundFlags flags) => PlaySound(SourceFile, UIntPtr.Zero, (uint)flags);
      #region IDisposable Support
      /// <summary>
      /// 释放该对象引用的所有内存资源。
      /// </summary>
      /// <param name="disposing">用于指示是否释放托管资源。</param>
      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               _soundFileUrl.Dispose();
               PlaySound(null, UIntPtr.Zero, (uint)ESoundFlags.FileName);
               GC.Collect();
            }
            disposedValue = true;
         }
      }
      /// <summary>
      /// 析构函数，释放该对象引用的所有内存资源和非托管资源。
      /// </summary>
      ~Sound() => Dispose(false);
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      #endregion
   }
   /// <summary>
   /// 音频播放标识枚举。
   /// </summary>
   [Flags]
   [CLSCompliant(false)]
   public enum ESoundFlags : uint
   {
      /// <summary>
      /// 同步播放（默认）。
      /// </summary>  
      [EnumerationDescription("同步播放")]
      Synchronously = 0x0000,
      /// <summary>
      /// 异步播放。
      /// </summary>  
      [EnumerationDescription("异步播放")]
      Asynchronously = 0x0001,
      /// <summary>
      /// 在没有找到音频的情况下保持静音。
      /// </summary>  
      [EnumerationDescription("无任务时静音")]
      NoDefault = 0x0002,
      /// <summary>
      /// 首参数指向一个内存文件。
      /// </summary>  
      [EnumerationDescription("指向内存文件")]
      Memory = 0x0004,
      /// <summary>
      /// 循环播放声音直到指定下一个音频播放任务。
      /// </summary>  
      [EnumerationDescription("循环播放")]
      Loop = 0x0008,
      /// <summary>
      /// 不要停止任何正在播放的声音。
      /// </summary>  
      [EnumerationDescription("禁止停止播放")]
      NoStop = 0x0010,
      /// <summary>
      /// 停止正在播放的任务。
      /// </summary>  
      [EnumerationDescription("停止播放")]
      StopPlaying = 0x40,
      /// <summary>
      /// 当设备繁忙时则不需要等待。
      /// </summary>  
      [EnumerationDescription("无需等待")]
      NoWait = 0x00002000,
      /// <summary>
      /// 资源描述字符串为注册表别名。
      /// </summary>  
      [EnumerationDescription("注册表别名")]
      Alias = 0x00010000,
      /// <summary>
      /// 资源描述字符串为预定义ID。
      /// </summary>  
      [EnumerationDescription("预定义ID")]
      AliasIsPredefinedId = 0x00110000,
      /// <summary>
      /// 资源描述字符串为文件地址。
      /// </summary>  
      [EnumerationDescription("文件地址")]
      FileName = 0x00020000,
      /// <summary>
      /// 资源描述字符串为资源名称或者Atom。
      /// </summary>  
      [EnumerationDescription("资源名或者ATOM")]
      ResourceOrAtom = 0x00040004
   }
}
