using System;
using System.IO;
using System.Drawing;
using Cabinink.IOSystem;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;
using Microsoft.DirectX.AudioVideoPlayback;
namespace Cabinink.Devices
{
   /// <summary>
   /// 视频播放类，可用于播放一些基本格式的视频。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class VideoPlayer : IDisposable
   {
      private Video _video;//一个用于播放视频的Microsoft.DirectX.AudioVideoPlayback.Video实例。
      private string _videoFileUrl;//指定的视频文件地址。
      public const int VOLUME_RATE_CONST = 100;//音量倍率常量。
      /// <summary>
      /// 构造函数，创建一个空的视频播放类的实例。
      /// </summary>
      public VideoPlayer() => DirectXVideoInstance = null;
      /// <summary>
      /// 构造函数，创建一个指定视频文件地址的VideoPlayer实例。
      /// </summary>
      /// <param name="videoFileUrl">指定的视频文件地址。</param>
      /// <exception cref="FileNotFoundException">当指定的视频文件找不到时，则将会抛出这个异常。</exception>
      public VideoPlayer(string videoFileUrl)
      {
         if (!FileOperator.FileExists(videoFileUrl)) throw new FileNotFoundException("指定的视频文件找不到！", videoFileUrl);
         else
         {
            DirectXVideoInstance = new Video(videoFileUrl, false);
            _videoFileUrl = videoFileUrl;
         }
      }
      /// <summary>
      /// 构造函数，创建一个指定视频文件地址和渲染目标的VideoPlayer实例。
      /// </summary>
      /// <param name="videoFileUrl">指定的视频文件地址。</param>
      /// <param name="owner">指定的视频渲染目标，即视频图像的呈现容器。</param>
      /// <exception cref="FileNotFoundException">当指定的视频文件找不到时，则将会抛出这个异常。</exception>
      /// <exception cref="NullReferenceException">当呈现容器的对象引用为NULL时，则将会抛出这个异常。</exception>
      public VideoPlayer(string videoFileUrl, Control owner)
      {
         if (!FileOperator.FileExists(videoFileUrl)) throw new FileNotFoundException("指定的视频文件找不到！", videoFileUrl);
         else
         {
            DirectXVideoInstance = new Video(videoFileUrl, false);
            _videoFileUrl = videoFileUrl;
         }
         if (owner == null) throw new NullReferenceException("视频图像渲染目标不可用！");
         else DirectXVideoInstance.Owner = owner;
      }
      /// <summary>
      /// 构造函数，创建一个指定视频文件地址和渲染目标和VideoPlayer实例，并决定是否自动播放这个视频。
      /// </summary>
      /// <param name="videoFileUrl">指定的视频文件地址。</param>
      /// <param name="owner">指定的视频渲染目标，即视频图像的呈现容器。</param>
      /// <param name="isAutoPlay">用于指示是否自动播放参数videoFileUrl指定的视频。</param>
      /// <exception cref="FileNotFoundException">当指定的视频文件找不到时，则将会抛出这个异常。</exception>
      /// <exception cref="NullReferenceException">当呈现容器的对象引用为NULL时，则将会抛出这个异常。</exception>
      public VideoPlayer(string videoFileUrl, Control owner, bool isAutoPlay)
      {
         if (!FileOperator.FileExists(videoFileUrl)) throw new FileNotFoundException("指定的视频文件找不到！", videoFileUrl);
         else
         {
            DirectXVideoInstance = new Video(videoFileUrl, isAutoPlay);
            _videoFileUrl = videoFileUrl;
         }
         if (owner == null) throw new NullReferenceException("视频图像渲染目标不可用！");
         else DirectXVideoInstance.Owner = owner;
      }
      /// <summary>
      /// 获取当前播放实例的视频文件地址。
      /// </summary>
      public string FileUrl => _videoFileUrl;
      /// <summary>
      /// 获取或设置当前实例的Microsoft.DirectX.AudioVideoPlayback.Video实例。
      /// </summary>
      public Video DirectXVideoInstance { get => _video; set => _video = value; }
      /// <summary>
      /// 获取当前播放实例的媒体播放状态。
      /// </summary>
      public EPlayerStatue PlayerStatue
      {
         get
         {
            EPlayerStatue statue = EPlayerStatue.Unknown;
            if (DirectXVideoInstance.Playing) statue = EPlayerStatue.Playing;
            if (DirectXVideoInstance.Paused) statue = EPlayerStatue.Paused;
            if (DirectXVideoInstance.Stopped) statue = EPlayerStatue.Stoped;
            return statue;
         }
      }
      /// <summary>
      /// 获取或设置当前播放实例的视频呈现容器。
      /// </summary>
      /// <exception cref="NullReferenceException">当呈现容器的对象引用为NULL时，则将会抛出这个异常。</exception>
      public Control Owner
      {
         get => DirectXVideoInstance.Owner;
         set
         {
            if (value == null) throw new NullReferenceException("视频图像渲染目标不可用！");
            else DirectXVideoInstance.Owner = value;
         }
      }
      /// <summary>
      /// 获取或设置当前播放实例的视频渲染尺寸。
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">当渲染尺寸小于40*30（MinWidth=40，MinHeight=30）时，则会抛出这个异常。</exception>
      public Size RenderingSize
      {
         get => new Size(Owner.Width, Owner.Height);
         set
         {
            bool condition = value.Width <= 40 || value.Height <= 30;
            if (condition) throw new ArgumentOutOfRangeException("value", "渲染尺寸不能小于40*30（MinWidth=40，MinHeight=30）！");
            else
            {
               Owner.Width = value.Width;
               Owner.Height = value.Height;
            }
         }
      }
      /// <summary>
      /// 获取或设置当前播放实例的音量，设置范围为[0, 100]。
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">如果音量设置超出范围，则将会抛出这个异常。</exception>
      public int Volume
      {
         get => DirectXVideoInstance.Audio.Volume / VOLUME_RATE_CONST + VOLUME_RATE_CONST;
         set
         {
            if (value < 0 || value > 100) throw new ArgumentOutOfRangeException("value", "音量设置超出范围！");
            else DirectXVideoInstance.Audio.Volume = -((VOLUME_RATE_CONST - value) * VOLUME_RATE_CONST);
         }
      }
      /// <summary>
      /// 获取当前播放实例的媒体长度（即最大播放时间，单位：秒）。
      /// </summary>
      public double Length => DirectXVideoInstance.Duration;
      /// <summary>
      /// 获取或设置当前播放实例的播放进度（单位：秒）。
      /// </summary>
      public double Position { get => DirectXVideoInstance.CurrentPosition; set => DirectXVideoInstance.CurrentPosition = value; }
      /// <summary>
      /// 获取或设置当前播放实例的全屏状态。
      /// </summary>
      public bool FullScreen { get => DirectXVideoInstance.Fullscreen; set => DirectXVideoInstance.Fullscreen = value; }
      /// <summary>
      /// 开始或者继续播放当前的视频。
      /// </summary>
      public void Play() => DirectXVideoInstance.Play();
      /// <summary>
      /// 暂停播放当前的视频。
      /// </summary>
      public void Pause() => DirectXVideoInstance.Pause();
      /// <summary>
      /// 停止播放当前的视频。
      /// </summary>
      public void Stop() => DirectXVideoInstance.Stop();
      /// <summary>
      /// 当视频对象准备好时停止播放。
      /// </summary>
      public void StopWhenReady() => DirectXVideoInstance.StopWhenReady();
      /// <summary>
      /// 在视频呈现容器中显示鼠标光标。
      /// </summary>
      public void ShowCursor() => DirectXVideoInstance.ShowCursor();
      /// <summary>
      /// 设置当前正在播放的视频的音轨的声道。
      /// </summary>
      /// <param name="channelDeviation">设置音轨的声道偏移方向。</param>
      public void SetChannelDeviation(EChannelDeviation channelDeviation)
      {
         int deviationAmount = 100;
         SetChannelDeviation(channelDeviation, deviationAmount);
      }
      /// <summary>
      /// 设置当前正在播放的视频的音轨的声道偏移量。
      /// </summary>
      /// <param name="deviationAmount">设置音轨的偏移量，如果为负数则表示为左声道，正数为右声道，0为立体声。</param>
      /// <exception cref="ArgumentOutOfRangeException">当声道偏移量超出范围时，则将会抛出这个异常。</exception>
      public void SetChannelDeviation(int deviationAmount)
      {
         int converted = deviationAmount * VOLUME_RATE_CONST;
         if (deviationAmount < 0 || deviationAmount > 100)
         {
            throw new ArgumentOutOfRangeException("deviationAmount", "声道偏移量超出范围！");
         }
         else DirectXVideoInstance.Audio.Balance = converted;
      }
      /// <summary>
      /// 通过声道偏移枚举来设置当前正在播放的视频的音轨的声道偏移量。
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
                  DirectXVideoInstance.Audio.Balance = -(deviationAmount * VOLUME_RATE_CONST);
                  break;
               case EChannelDeviation.Stereo:
                  DirectXVideoInstance.Audio.Balance = deviationAmount - deviationAmount;
                  break;
               case EChannelDeviation.RightChannel:
                  DirectXVideoInstance.Audio.Balance = deviationAmount * VOLUME_RATE_CONST;
                  break;
               default:
                  DirectXVideoInstance.Audio.Balance = deviationAmount - deviationAmount;
                  break;
            }
         }
      }
      /// <summary>
      /// 获取当前正在播放的视频的声道偏移量。
      /// </summary>
      /// <returns>该操作会返回一个32位整数数值，这个数值如果为负数则表示为当前正在播放的视频的音轨被设置为了左声道，正数为右声道，0为立体声。</returns>
      public int GetChannelDeviation() => DirectXVideoInstance.Audio.Balance / 100;
      /// <summary>
      /// 通过声道偏移枚举来获取正在播放的视频的声道偏移量。
      /// </summary>
      /// <param name="channelDeviation">在方法执行之后，用于描述当前正在播放的视频的声道。</param>
      /// <returns>该操作会返回一个32位整数数值，但这个数值为声道偏移量的绝对值，具体判定方式取决于channelDeviation这个引用传递的参数，如果这个最终参数值为EChannelDeviation.LeftChannel则表示为当前正在播放的视频的音轨被设置为了左声道，参数最终值为EChannelDeviation.Stereo则表示为立体声，最终参数值为EChannelDeviation.RightChannel则表示右声道。</returns>
      public int GetChannelDeviation(ref EChannelDeviation channelDeviation)
      {
         int deviationAmount = GetChannelDeviation();
         if (deviationAmount > 0) channelDeviation = EChannelDeviation.RightChannel;
         if (deviationAmount == 0) channelDeviation = EChannelDeviation.Stereo;
         if (deviationAmount < 0) channelDeviation = EChannelDeviation.LeftChannel;
         return Math.Abs(deviationAmount);
      }
      /// <summary>
      /// 获取当前视频的理论最大渲染尺寸。
      /// </summary>
      /// <returns>该方法会返回一个Size实例，表示一个渲染尺寸。</returns>
      public Size GetMaximumRenderingSize() => DirectXVideoInstance.MaximumIdealSize;
      /// <summary>
      /// 获取当前视频的理论最小渲染尺寸。
      /// </summary>
      /// <returns>该方法会返回一个Size实例，表示一个渲染尺寸。</returns>
      public Size GetMinimumRenderingSize() => DirectXVideoInstance.MinimumIdealSize;
      /// <summary>
      /// 获取当前视频的默认渲染尺寸。
      /// </summary>
      /// <returns>该方法会返回一个Size实例，表示一个渲染尺寸。</returns>
      public Size GetDefaultRenderingSize() => DirectXVideoInstance.DefaultSize;
      /// <summary>
      /// 检索播放期间视频文件每帧的平均时间。
      /// </summary>
      /// <returns>该方法会返回一个浮点数，表示一个播放期间每帧的平均时间。</returns>
      public double GetAverageTimePerFrame() => DirectXVideoInstance.AverageTimePerFrame;
      /// <summary>
      /// 检索播放期间视频文件的平均刷新率。
      /// </summary>
      /// <returns>该方法会返回一个浮点数，表示一个播放期间视频文件的平均刷新率。</returns>
      public double GetAverageRefreshRate() => 1 / GetAverageTimePerFrame();
      /// <summary>
      /// 寻求特定的播放位置。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <returns>该操作会返回操作之后的视频播放进度。</returns>
      public double SeekCurrentPosition(double time) => DirectXVideoInstance.SeekCurrentPosition(time, SeekPositionFlags.AbsolutePositioning);
      /// <summary>
      /// 寻求特定的播放位置，并指定探寻标识。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <param name="flags">指定的探寻标识。</param>
      /// <returns>该操作会返回操作之后的视频播放进度。</returns>
      public double SeekCurrentPosition(double time, SeekPositionFlags flags) => DirectXVideoInstance.SeekCurrentPosition(time, flags);
      /// <summary>
      /// 在播放中设置新的停止位置。
      /// </summary>
      /// <param name="time">播放的新停止时间。</param>
      /// <returns>该操作将会返回新的停止位置。</returns>
      public double SeekStopPosition(double time) => DirectXVideoInstance.SeekStopPosition(time, SeekPositionFlags.AbsolutePositioning);
      /// <summary>
      /// 在播放中设置新的停止位置，并指定探寻标识。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <param name="flags">指定的探寻标识。</param>
      /// <returns>该操作将会返回新的停止位置。</returns>
      public double SeekStopPosition(double time, SeekPositionFlags flags) => DirectXVideoInstance.SeekStopPosition(time, flags);
      /// <summary>
      /// 启用视频以触​​发TextureReadyToRender事件以生成用于在Microsoft Direct3D中呈现视频的纹理对象。
      /// </summary>
      /// <param name="graphicsDevice">指定的Microsoft.DirectX.Direct3D.Device呈现视频纹理的父Direct3D.Device对象。</param>
      public void RenderToTexture(Device graphicsDevice) => DirectXVideoInstance.RenderToTexture(graphicsDevice);
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose() => DirectXVideoInstance.Dispose();
   }
   /// <summary>
   /// 音频视频的播放状态枚举。
   /// </summary>
   public enum EPlayerStatue : int
   {
      /// <summary>
      /// 已停止音频或者视频的播放。
      /// </summary>
      [EnumerationDescription("已停止")]
      Stoped = 0x0000,
      /// <summary>
      /// 正在播放指定的音频或者视频。
      /// </summary>
      [EnumerationDescription("正在播放")]
      Playing = 0x0001,
      /// <summary>
      /// 指定的音频或者视频已经暂停播放。
      /// </summary>
      [EnumerationDescription("已暂停")]
      Paused = 0x0002,
      /// <summary>
      /// 未知的播放状态。
      /// </summary>
      [EnumerationDescription("未知状态")]
      Unknown = 0xffff
   }
   /// <summary>
   /// 声道设置的枚举。
   /// </summary>
   public enum EChannelDeviation : int
   {
      /// <summary>
      /// 设置为左声道。
      /// </summary>
      [EnumerationDescription("左声道")]
      LeftChannel = -0xffff,
      /// <summary>
      /// 设置为立体声。
      /// </summary>
      [EnumerationDescription("立体声")]
      Stereo = 0x0000,
      /// <summary>
      /// 设置为右声道。
      /// </summary>
      [EnumerationDescription("右声道")]
      RightChannel = 0xffff
   }
}
