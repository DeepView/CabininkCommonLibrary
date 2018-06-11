using System;
using System.IO;
using Cabinink.IOSystem;
using System.Runtime.InteropServices;
using Microsoft.DirectX.AudioVideoPlayback;
namespace Cabinink.Devices
{
   /// <summary>
   /// 音频播放类，可用于播放一些基本格式的音频。
   /// </summary>
   /// <remarks>
   /// 由于这个类使用了DirectX SDK的相关技术，因此您需要在您的项目中进行一些设置，这是由于在DirectX SDK中，适用于.NET Framework的代码采用了早期版本的技术，因此您需要在项目的app.config文件的startup标记添加useLegacyV2RuntimeActivationPolicy属性，其详细的配置代码如下所示：
   /// <example>
   /// <code>
   /// &lt;startup useLegacyV2RuntimeActivationPolicy="true"&gt;
   /// &lt;!--This is your app's configure code.--&gt;
   /// &lt;/startup&gt;
   /// </code>
   /// </example>
   /// </remarks>
   [Serializable]
   [ComVisible(true)]
   public class SoundPlayer : IPlaybackControl, IVolumeControl, IPlaybackProgressControl, IDisposable
   {
      private Audio _audio;//一个用于播放音频的Microsoft.DirectX.AudioVideoPlayback.Audio实例。
      private string _soundFileUrl;//指定的音频文件地址。
      public const int VOLUME_RATE_CONST = 100;//音量倍率常量。
      /// <summary>
      /// 构造函数，创建一个空的音频播放类的实例。
      /// </summary>
      public SoundPlayer() => DirectXAudioInstance = null;
      /// <summary>
      /// 构造函数，创建一个指定音频文件地址的SoundPlayer实例。
      /// </summary>
      /// <param name="soundFileUrl">指定的音频文件地址。</param>
      /// <exception cref="FileNotFoundException">当指定的音频文件找不到时，则将会抛出这个异常。</exception>
      public SoundPlayer(string soundFileUrl)
      {
         if (!FileOperator.FileExists(soundFileUrl)) throw new FileNotFoundException("指定的音频文件找不到！", soundFileUrl);
         else
         {
            DirectXAudioInstance = new Audio(soundFileUrl, false);
            _soundFileUrl = soundFileUrl;
         }
      }
      /// <summary>
      /// 构造函数，创建一个指定音频文件地址和渲染目标和SoundPlayer实例，并决定是否自动播放这个音频。
      /// </summary>
      /// <param name="soundFileUrl">指定的音频文件地址。</param>
      /// <param name="isAutoPlay">用于指示是否自动播放参数soundFileUrl指定的音频。</param>
      /// <exception cref="FileNotFoundException">当指定的音频文件找不到时，则将会抛出这个异常。</exception>
      public SoundPlayer(string soundFileUrl, bool isAutoPlay)
      {
         if (!FileOperator.FileExists(soundFileUrl)) throw new FileNotFoundException("指定的音频文件找不到！", soundFileUrl);
         else
         {
            DirectXAudioInstance = new Audio(soundFileUrl, isAutoPlay);
            _soundFileUrl = soundFileUrl;
         }
      }
      /// <summary>
      /// 获取或设置当前实例中包含的Microsoft.DirectX.AudioVideoPlayback.Audio实例。
      /// </summary>
      public Audio DirectXAudioInstance { get => _audio; set => _audio = value; }
      /// <summary>
      /// 获取当前播放实例的音频文件地址。
      /// </summary>
      public string FileUrl => _soundFileUrl;
      /// <summary>
      /// 获取或设置当前媒体播放器的媒体音量。
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">当音量设置超出范围时，则将会抛出这个异常。</exception>
      public int Volume
      {
         get => DirectXAudioInstance.Volume / VOLUME_RATE_CONST + VOLUME_RATE_CONST;
         set
         {
            if (value < 0 || value > 100) throw new ArgumentOutOfRangeException("value", "音量设置超出范围！");
            else DirectXAudioInstance.Volume = -((VOLUME_RATE_CONST - value) * VOLUME_RATE_CONST);
         }
      }
      /// <summary>
      /// 获取当前播放实例的媒体播放状态。
      /// </summary>
      public EPlayerStatue PlayerStatue
      {
         get
         {
            EPlayerStatue statue = EPlayerStatue.Unknown;
            if (DirectXAudioInstance.Playing) statue = EPlayerStatue.Playing;
            if (DirectXAudioInstance.Paused) statue = EPlayerStatue.Paused;
            if (DirectXAudioInstance.Stopped) statue = EPlayerStatue.Stoped;
            return statue;
         }
      }
      /// <summary>
      /// 获取当前播放实例的媒体长度（即最大播放时间，单位：秒）。
      /// </summary>
      public double Length => DirectXAudioInstance.Duration;
      /// <summary>
      /// 获取当前播放实例其媒体文件在停止播放时的播放进度（单位：秒）。
      /// </summary>
      public double StopPosition => DirectXAudioInstance.StopPosition;
      /// <summary>
      /// 获取或设置当前播放实例的播放进度（单位：秒）。
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">当用户指定的播放进度不在有效范围内，则将会抛出这个异常。</exception>
      public double Position
      {
         get => DirectXAudioInstance.CurrentPosition;
         set
         {
            bool condition = value > Length || value < 0;
            if (condition) throw new ArgumentOutOfRangeException("value", "需要调整的播放进度必须在有效范围内！");
            else DirectXAudioInstance.CurrentPosition = value;
         }
      }
      /// <summary>
      /// 暂停播放当前的媒体文件。
      /// </summary>
      public void Pause() => DirectXAudioInstance.Pause();
      /// <summary>
      /// 开始或者继续播放当前的媒体文件。
      /// </summary>
      public void Play() => DirectXAudioInstance.Play();
      /// <summary>
      /// 停止播放当前的媒体文件。
      /// </summary>
      public void Stop() => DirectXAudioInstance.Stop();
      /// <summary>
      /// 当播放器对象准备好时停止播放。
      /// </summary>
      public void StopWhenReady() => DirectXAudioInstance.StopWhenReady();
      /// <summary>
      /// 获取当前正在播放的媒体文件的声道偏移量。
      /// </summary>
      /// <returns>该操作会返回一个32位整数数值，这个数值如果为负数则表示为当前正在播放的媒体文件的音轨被设置为了左声道，正数为右声道，0为立体声。</returns>
      public int GetChannelDeviation() => DirectXAudioInstance.Balance / 100;
      /// <summary>
      /// 通过声道偏移枚举来获取正在播放的媒体文件的声道偏移量。
      /// </summary>
      /// <param name="channelDeviation">在方法执行之后，用于描述当前正在播放的媒体文件的声道。</param>
      /// <returns>该操作会返回一个32位整数数值，但这个数值为声道偏移量的绝对值，具体判定方式取决于channelDeviation这个引用传递的参数，如果这个最终参数值为EChannelDeviation.LeftChannel则表示为当前正在播放的媒体文件的音轨被设置为了左声道，参数最终值为EChannelDeviation.Stereo则表示为立体声，最终参数值为EChannelDeviation.RightChannel则表示右声道。</returns>
      public int GetChannelDeviation(ref EChannelDeviation channelDeviation)
      {
         int deviationAmount = GetChannelDeviation();
         if (deviationAmount > 0) channelDeviation = EChannelDeviation.RightChannel;
         if (deviationAmount == 0) channelDeviation = EChannelDeviation.Stereo;
         if (deviationAmount < 0) channelDeviation = EChannelDeviation.LeftChannel;
         return Math.Abs(deviationAmount);
      }
      /// <summary>
      /// 设置当前正在播放的媒体文件的音轨的声道。
      /// </summary>
      /// <param name="channelDeviation">设置音轨的声道偏移方向。</param>
      public void SetChannelDeviation(EChannelDeviation channelDeviation)
      {
         int deviationAmount = 100;
         SetChannelDeviation(channelDeviation, deviationAmount);
      }
      /// <summary>
      /// 设置当前正在播放的媒体文件的音轨的声道偏移量。
      /// </summary>
      /// <param name="deviationAmount">设置音轨的偏移量，如果为负数则表示为左声道，正数为右声道，0为立体声。</param>
      /// <exception cref="ArgumentOutOfRangeException">当声道偏移量超出范围时，则将会抛出这个异常。</exception>
      public void SetChannelDeviation(int deviationAmount)
      {
         int converted = deviationAmount * VOLUME_RATE_CONST;
         if (deviationAmount < -100 || deviationAmount > 100)
         {
            throw new ArgumentOutOfRangeException("deviationAmount", "声道偏移量超出范围！");
         }
         else DirectXAudioInstance.Balance = converted;
      }
      /// <summary>
      /// 通过声道偏移枚举来设置当前正在播放的媒体文件的音轨的声道偏移量。
      /// </summary>
      /// <param name="channelDeviation">设置音轨的声道偏移方向。</param>
      /// <param name="deviationAmount">设置音轨的偏移量。</param>
      /// <exception cref="ArgumentOutOfRangeException">当声道偏移量超出范围时，则将会抛出这个异常。</exception>
      public void SetChannelDeviation(EChannelDeviation channelDeviation, int deviationAmount)
      {
         if (deviationAmount < 0 || deviationAmount > 100)
         {
            throw new ArgumentOutOfRangeException("deviationAmount", "声道偏移量超出范围！");
         }
         else
         {
            switch (channelDeviation)
            {
               case EChannelDeviation.LeftChannel:
                  DirectXAudioInstance.Balance = -(deviationAmount * VOLUME_RATE_CONST);
                  break;
               case EChannelDeviation.Stereo:
                  DirectXAudioInstance.Balance = deviationAmount - deviationAmount;
                  break;
               case EChannelDeviation.RightChannel:
                  DirectXAudioInstance.Balance = deviationAmount * VOLUME_RATE_CONST;
                  break;
               default:
                  DirectXAudioInstance.Balance = deviationAmount - deviationAmount;
                  break;
            }
         }
      }
      /// <summary>
      /// 寻求特定的播放位置。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <returns>该操作会返回操作之后的媒体文件播放进度。</returns>
      public double SeekCurrentPosition(double time) => DirectXAudioInstance.SeekCurrentPosition(time, SeekPositionFlags.AbsolutePositioning);
      /// <summary>
      /// 寻求特定的播放位置，并指定探寻标识。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <param name="flags">指定的探寻标识。</param>
      /// <returns>该操作会返回操作之后的媒体文件播放进度。</returns>
      public double SeekCurrentPosition(double time, SeekPositionFlags flags) => DirectXAudioInstance.SeekCurrentPosition(time, flags);
      /// <summary>
      /// 在播放中设置新的停止位置。
      /// </summary>
      /// <param name="time">播放的新停止时间。</param>
      /// <returns>该操作将会返回新的停止位置。</returns>
      public double SeekStopPosition(double time) => DirectXAudioInstance.SeekStopPosition(time, SeekPositionFlags.AbsolutePositioning);
      /// <summary>
      /// 在播放中设置新的停止位置，并指定探寻标识。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <param name="flags">指定的探寻标识。</param>
      /// <returns>该操作将会返回新的停止位置。</returns>
      public double SeekStopPosition(double time, SeekPositionFlags flags) => DirectXAudioInstance.SeekStopPosition(time, flags);
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose() => DirectXAudioInstance.Dispose();
   }
}
