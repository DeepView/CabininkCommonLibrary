using Microsoft.DirectX.AudioVideoPlayback;
namespace Cabinink.Devices
{
   public interface IPlaybackProgressControl
   {
      /// <summary>
      /// 获取当前播放实例的媒体长度（即最大播放时间，单位：秒）。
      /// </summary>
      double Length { get; }
      /// <summary>
      /// 获取或设置当前播放实例的播放进度（单位：秒）。
      /// </summary>
      double Position { get; set; }
      /// <summary>
      /// 获取当前播放实例其媒体文件在停止播放时的播放进度（单位：秒）。
      /// </summary>
      double StopPosition { get; }
      /// <summary>
      /// 寻求特定的播放位置。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <returns>该操作会返回操作之后的媒体文件播放进度。</returns>
      double SeekCurrentPosition(double time);
      /// <summary>
      /// 寻求特定的播放位置，并指定探寻标识。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <param name="flags">指定的探寻标识。</param>
      /// <returns>该操作会返回操作之后的媒体文件播放进度。</returns>
      double SeekCurrentPosition(double time, SeekPositionFlags flags);
      /// <summary>
      /// 在播放中设置新的停止位置。
      /// </summary>
      /// <param name="time">播放的新停止时间。</param>
      /// <returns>该操作将会返回新的停止位置。</returns>
      double SeekStopPosition(double time);
      /// <summary>
      /// 在播放中设置新的停止位置，并指定探寻标识。
      /// </summary>
      /// <param name="time">指定的时间位置。</param>
      /// <param name="flags">指定的探寻标识。</param>
      /// <returns>该操作将会返回新的停止位置。</returns>
      double SeekStopPosition(double time, SeekPositionFlags flags);
   }
}