using System;
using Cabinink.Algorithm;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 圆的表示类，用于描述一个平面几何中的圆。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Circular : IPerimeter, IMeasureOfArea, ITranslation2D, IEquatable<Circular>
   {
      private ExPoint2D _center;//圆心定义
      private double _radius;//圆的半径
      /// <summary>
      /// 构造函数，创建一个指定半径且圆心位置为(0,0)的圆。
      /// </summary>
      /// <param name="radius">指定的半径。</param>
      /// <exception cref="IllegalRadiusDefinitionException">当半径设定值小于或者等于0时，则将会抛出这个异常。</exception>
      public Circular(double radius)
      {
         if (radius <= 0) throw new IllegalRadiusDefinitionException();
         else
         {
            _radius = radius;
            _center = new ExPoint2D();
         }
      }
      /// <summary>
      /// 构造函数，创建一个指定半径和圆心位置的圆。
      /// </summary>
      /// <param name="center">圆心的坐标。</param>
      /// <param name="radius">指定的半径。</param>
      /// <exception cref="IllegalRadiusDefinitionException">当半径设定值小于或者等于0时，则将会抛出这个异常。</exception>
      public Circular(ExPoint2D center, double radius)
      {
         if (radius <= 0) throw new IllegalRadiusDefinitionException();
         else
         {
            _radius = radius;
            _center = center;
         }
      }
      /// <summary>
      /// 获取或设置当前圆的圆心。
      /// </summary>
      public ExPoint2D Center { get => _center; set => _center = value; }
      /// <summary>
      /// 获取或设置当前圆的半径。
      /// </summary>
      /// <exception cref="IllegalRadiusDefinitionException">当半径设定值小于或者等于0时，则将会抛出这个异常。</exception>
      public double Radius
      {
         get => _radius;
         set
         {
            if (value <= 0) throw new IllegalRadiusDefinitionException();
            else _radius = value;
         }
      }
      /// <summary>
      /// 获取当前圆的周长。
      /// </summary>
      public double Perimeter => MathConstant.CircumferenceRatio * Radius * 2;
      /// <summary>
      /// 获取当前圆的面积。
      /// </summary>
      public double MeasureOfArea => MathConstant.CircumferenceRatio * Math.Pow(Radius, 2);
      /// <summary>
      /// 判断两个圆之间的距离关系。
      /// </summary>
      /// <param name="circular">用于作判断的另外一个圆。</param>
      /// <returns>该操作将会返回两圆之间的距离关系，这个关系要么是相离（HaveDistance），要么是相切（Intersect），要么就是相交（NotDistance）。</returns>
      public EIntersectStatus IntersectStatus(Circular circular)
      {
         EIntersectStatus iStatus = 0;
         if (GetEdgeDistance(circular) > 0) iStatus = EIntersectStatus.HaveDistance;
         else if (GetEdgeDistance(circular) == 0) iStatus = EIntersectStatus.Intersect;
         else if (GetEdgeDistance(circular) < 0) iStatus = EIntersectStatus.NotDistance;
         return iStatus;
      }
      /// <summary>
      /// 获取两个圆的圆心距离。
      /// </summary>
      /// <param name="circular">用于作比较的另外一个圆。</param>
      /// <returns>该操作将会返回一个浮点数，这个浮点数表示这两个圆的圆心距离。</returns>
      public double GetCenterDistance(Circular circular) => Center.GetDistance(circular.Center);
      /// <summary>
      /// 获取两个圆的边缘距离。
      /// </summary>
      /// <param name="circular">用于作比较的另外一个圆。</param>
      /// <returns>该操作将会返回一个浮点数，这个浮点数表示这两个圆的边缘距离。</returns>
      public double GetEdgeDistance(Circular circular) => GetCenterDistance(circular) - (Radius + circular.Radius);
      /// <summary>
      /// 将当前的圆在横轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      public void AbscissaTranslation(double offset) => ((ITranslation2D)Center).AbscissaTranslation(offset);
      /// <summary>
      /// 将当前的圆在纵轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      public void OrdinateTranslation(double offset) => ((ITranslation2D)Center).OrdinateTranslation(offset);
      /// <summary>
      /// 将整个圆进行坐标平移。
      /// </summary>
      /// <param name="offsetPoint">偏移量，即在横轴和纵轴上平移的长度。</param>
      public void Translation(ExPoint2D offsetPoint) => ((ITranslation2D)Center).Translation(offsetPoint);
      /// <summary>
      /// 判断两个圆是否相同，值得注意的是，这里是从数学的层面上进行判断，而非代码逻辑上去判断。
      /// </summary>
      /// <param name="other">用于被比较的另一个圆。</param>
      /// <returns>如果判断结果表示两个圆相同，则返回true，否则返回false。</returns>
      /// <remarks>在这里，进行两个圆的相同性质判断的原理，是根据判断圆心位置和半径是否相等来实现的，如果圆心位置和半径都相同，则表示这两个圆也是相同的，如果两个条件任意一个无法满足，则这两个圆为不相同。</remarks>
      public bool Equals(Circular other) => Center.Equals(other.Center) && Radius == other.Radius;
      /// <summary>
      /// 获取当前实例的字符串表达形式。
      /// </summary>
      /// <returns>该操作将会返回一个字符串值，这个值表示当前实例包含命名空间的完整名称。</returns>
      public override string ToString() => @"Cabinink.TypeExtend.Geometry2D.Circular";
   }
   /// <summary>
   /// 定义一个枚举，这个枚举表示两个圆相交之后的结果。
   /// </summary>
   public enum EIntersectStatus : int
   {
      /// <summary>
      /// 两圆相交。
      /// </summary>
      Intersect = -0x0001,
      /// <summary>
      /// 两圆相切。
      /// </summary>
      NotDistance = 0x0000,
      /// <summary>
      /// 两圆相离。
      /// </summary>
      HaveDistance = 0x0001
   }
   /// <summary>
   /// 当出现非法的半径定义时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class IllegalRadiusDefinitionException : Exception
   {
      public IllegalRadiusDefinitionException() : base("非法的半径定义！") { }
      public IllegalRadiusDefinitionException(string message) : base(message) { }
      public IllegalRadiusDefinitionException(string message, Exception inner) : base(message, inner) { }
      protected IllegalRadiusDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
