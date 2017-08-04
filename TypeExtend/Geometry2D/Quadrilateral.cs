using System;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 四边形抽象表达类，用于描述一个四边形的抽象概念。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public abstract class Quadrilateral : IMeasureOfArea, IPerimeter, IEquatable<Quadrilateral>
   {
      private ExPoint2D _leftTopVertex;//左上角的（或者第一个）顶点。
      private ExPoint2D _leftBottomVertex;//左下角的（或者第二个）顶点。
      private ExPoint2D _rightTopVertex;//右上角的（或者第三个）顶点。
      private ExPoint2D _rightBottomVertex;//右下角的（或者第四个）顶点。
      /// <summary>
      /// 获取或设置当前实例所表示的四边形的左上角（或者第一个）顶点。
      /// </summary>
      public virtual ExPoint2D LeftTopVertex { get => _leftTopVertex; set => _leftTopVertex = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的四边形的左下角（或者第二个）顶点。
      /// </summary>
      public virtual ExPoint2D LeftBottomVertex { get => _leftBottomVertex; set => _leftBottomVertex = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的四边形的右上角（或者第三个）顶点。
      /// </summary>
      public virtual ExPoint2D RightTopVertex { get => _rightTopVertex; set => _rightTopVertex = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的四边形的右下角（或者第四个）顶点。
      /// </summary>
      public virtual ExPoint2D RightBottomVertex { get => _rightBottomVertex; set => _rightBottomVertex = value; }
      /// <summary>
      /// 获取当前四边形的面积。
      /// </summary>
      public abstract double MeasureOfArea { get; }
      /// <summary>
      /// 获取当前四边形的周长。
      /// </summary>
      public abstract double Perimeter { get; }
      /// <summary>
      /// 判断当前四边形是否与另一个四边形相同。
      /// </summary>
      /// <param name="other">用于作比较的另一个四边形。</param>
      /// <returns>如果当前四边形与另外一个四边形相同，则将会返回true，否则返回false。</returns>
      /// <remarks>该操作不仅仅包含了四边形的形状和大小的判定，还包含了每一个顶点的坐标判定，因此如果将此函数作为形状大小的判定依据，则可能会导致判定结果上出现与数学逻辑不相同的结果。</remarks>
      public virtual bool Equals(Quadrilateral other)
      {
         bool ltvEqual = LeftTopVertex.Equals(other.LeftTopVertex);
         bool lbvEqual = LeftBottomVertex.Equals(other.LeftBottomVertex);
         bool rtvEqual = RightTopVertex.Equals(other.RightTopVertex);
         bool rbvEqual = RightBottomVertex.Equals(other.RightBottomVertex);
         return ltvEqual && lbvEqual && rtvEqual && rbvEqual;
      }
   }
}
