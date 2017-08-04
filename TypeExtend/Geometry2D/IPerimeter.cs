namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 提供平面图形周长计算需要的接口。
   /// </summary>
   public interface IPerimeter
   {
      /// <summary>
      /// 获取当前平面图形的周长。
      /// </summary>
      double Perimeter { get; }
   }
}
