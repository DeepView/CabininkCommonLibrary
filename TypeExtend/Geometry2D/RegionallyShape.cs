using System;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 存在面积的图形抽象类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public abstract class RegionallyShape : Shape2D, IMeasureOfArea, IPerimeter, ITranslation2D, IRotate
   {
      /// <summary>
      /// 获取当前平面图形的面积或者三维规则形物体的表面积。
      /// </summary>
      public abstract double MeasureOfArea { get; }
      /// <summary>
      /// 获取当前平面图形的周长。
      /// </summary>
      public abstract double Perimeter { get; }
      /// <summary>
      /// 将当前图形在横轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      public abstract void AbscissaTranslation(double offset);
      /// <summary>
      /// 将当前图形在纵轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      public abstract void OrdinateTranslation(double offset);
      /// <summary>
      /// 将整个图形进行坐标平移。
      /// </summary>
      /// <param name="offsetPoint">偏移量，即在横轴和纵轴上平移的长度。</param>
      public abstract void Translation(ExPoint2D offsetPoint);
      /// <summary>
      /// 将当前的图形围绕指定点旋转指定角度。
      /// </summary>
      /// <param name="axisPoint">二维点旋转的中心点。</param>
      /// <param name="angle">逆时针旋转的角度。</param>
      /// <returns>这个操作会返回一个通过逆时针旋转后获取到的新的图形实例。</returns>
      public abstract Shape2D Rotate(ExPoint2D axisPoint, Angle angle);
      /// <summary>
      /// 获取当前实例的字符串表达形式。
      /// </summary>
      /// <returns>该操作将会返回一个字符串值，这个值表示当前实例包含命名空间的完整名称。</returns>
      public override string ToString() => @"Cabinink.TypeExtend.Geometry2D.RegionallyShape";
   }
}
