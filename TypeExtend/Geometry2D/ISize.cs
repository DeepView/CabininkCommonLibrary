namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 用于获取一个图形尺寸的长度与宽度的接口实现。
   /// </summary>
   public interface ISize
   {
      /// <summary>
      /// 获取当前实例尺寸大小的长度分量。
      /// </summary>
      double Height { get; }
      /// <summary>
      /// 获取当前实例尺寸大小的宽度分量。
      /// </summary>
      double Width { get; }
   }
}