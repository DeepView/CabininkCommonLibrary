namespace Cabinink.Devices
{
   /// <summary>
   /// 媒体文件的音量和声道偏移控制接口。
   /// </summary>
   public interface IVolumeControl
   {
      /// <summary>
      /// 获取或设置当前媒体播放器的媒体音量。
      /// </summary>
      int Volume { get; set; }
      /// <summary>
      /// 获取当前正在播放的媒体文件的声道偏移量。
      /// </summary>
      /// <returns>该操作会返回一个32位整数数值，这个数值如果为负数则表示为当前正在播放的媒体文件的音轨被设置为了左声道，正数为右声道，0为立体声。</returns>
      int GetChannelDeviation();
      /// <summary>
      /// 通过声道偏移枚举来获取正在播放的媒体文件的声道偏移量。
      /// </summary>
      /// <param name="channelDeviation">在方法执行之后，用于描述当前正在播放的媒体文件的声道。</param>
      /// <returns>该操作会返回一个32位整数数值，但这个数值为声道偏移量的绝对值，具体判定方式取决于channelDeviation这个引用传递的参数，如果这个最终参数值为EChannelDeviation.LeftChannel则表示为当前正在播放的媒体文件的音轨被设置为了左声道，参数最终值为EChannelDeviation.Stereo则表示为立体声，最终参数值为EChannelDeviation.RightChannel则表示右声道。</returns>
      int GetChannelDeviation(ref EChannelDeviation channelDeviation);
      /// <summary>
      /// 设置当前正在播放的媒体文件的音轨的声道。
      /// </summary>
      /// <param name="channelDeviation">设置音轨的声道偏移方向。</param>
      void SetChannelDeviation(EChannelDeviation channelDeviation);
      /// <summary>
      /// 通过声道偏移枚举来设置当前正在播放的媒体文件的音轨的声道偏移量。
      /// </summary>
      /// <param name="channelDeviation">设置音轨的声道偏移方向。</param>
      /// <param name="deviationAmount">设置音轨的偏移量。</param>
      void SetChannelDeviation(EChannelDeviation channelDeviation, int deviationAmount);
      /// <summary>
      /// 设置当前正在播放的媒体文件的音轨的声道偏移量。
      /// </summary>
      /// <param name="deviationAmount">设置音轨的偏移量，如果为负数则表示为左声道，正数为右声道，0为立体声。</param>
      void SetChannelDeviation(int deviationAmount);
   }
}