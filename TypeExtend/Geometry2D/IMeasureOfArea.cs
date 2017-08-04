namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 提供平面图形面积或者三维规则性物体的表面积计算的接口。
   /// </summary>
   public interface IMeasureOfArea
   {
      /// <summary>
      /// 获取当前平面图形的面积或者三维规则形物体的表面积。
      /// </summary>
      double MeasureOfArea { get; }
   }
}
