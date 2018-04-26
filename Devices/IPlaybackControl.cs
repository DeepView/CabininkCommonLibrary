namespace Cabinink.Devices
{
   /// <summary>
   /// 媒体文件的基本播放控制接口。
   /// </summary>
   public interface IPlaybackControl
   {
      /// <summary>
      /// 获取当前播放实例的媒体播放状态。
      /// </summary>
      EPlayerStatue PlayerStatue { get; }
      /// <summary>
      /// 暂停播放当前的媒体文件。
      /// </summary>
      void Pause();
      /// <summary>
      /// 开始或者继续播放当前的媒体文件。
      /// </summary>
      void Play();
      /// <summary>
      /// 停止播放当前的媒体文件。
      /// </summary>
      void Stop();
      /// <summary>
      /// 当播放器对象准备好时停止播放。
      /// </summary>
      void StopWhenReady();
   }
}