namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 平面图形旋转接口。
   /// </summary>
   public interface IRotate
   {
      /// <summary>
      /// 将当前的图形围绕指定点旋转指定角度。
      /// </summary>
      /// <param name="axisPoint">二维点旋转的中心点。</param>
      /// <param name="angle">逆时针旋转的角度。</param>
      /// <returns>这个操作会返回一个通过逆时针旋转后获取到的新的图形实例。</returns>
      Shape2D Rotate(ExPoint2D axisPoint, Angle angle);
   }
}