using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 矩形描述类，用于描述一个在数学和图形学范围上的矩形。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class ExRectangle : Quadrilateral, ISize
   {
      /// <summary>
      /// 构造函数，通过一个浮点尺寸实例和一个原点坐标创建一个矩形实例。
      /// </summary>
      /// <param name="size">指定的浮点尺寸实例。</param>
      public ExRectangle(SizeF size) : base()
      {
         LeftTopVertex = new ExPoint2D();
         LeftBottomVertex = (0, -size.Height);
         RightTopVertex = (size.Width, 0);
         RightBottomVertex = (size.Width, -size.Height);
      }
      /// <summary>
      /// 构造函数，通过一个指定的RectangleF实例来创建当前的矩形实例。
      /// </summary>
      /// <param name="rectangle">指定的RectangleF实例。</param>
      public ExRectangle(RectangleF rectangle) : base()
      {
         LeftTopVertex = (rectangle.X, rectangle.Y);
         LeftBottomVertex = (rectangle.X, -rectangle.Height);
         RightTopVertex = (rectangle.Width, rectangle.Y);
         RightBottomVertex = (rectangle.Width, -rectangle.Height);
      }
      /// <summary>
      /// 构造函数，通过一个指定的左上角顶点和一个浮点尺寸实例创建当前的矩形实例。
      /// </summary>
      /// <param name="ltVertex">一个左上角顶点。</param>
      /// <param name="size">指定的浮点尺寸实例。</param>
      public ExRectangle(ExPoint2D ltVertex, SizeF size) : base()
      {
         LeftTopVertex = ltVertex;
         LeftBottomVertex = (0, ltVertex.YPosition + (-size.Height));
         RightTopVertex = (ltVertex.XPosition + size.Width, ltVertex.YPosition);
         RightBottomVertex = (ltVertex.XPosition + size.Width, ltVertex.YPosition + (-size.Height));
      }
      /// <summary>
      /// 构造函数，通过矩形的四个顶点来创建当前矩形实例。
      /// </summary>
      /// <param name="ltVertex">左上角顶点。</param>
      /// <param name="lbVertex">左下角顶点。</param>
      /// <param name="rtVertex">右上角顶点。</param>
      /// <param name="rbVertex">右下角顶点。</param>
      public ExRectangle(ExPoint2D ltVertex, ExPoint2D lbVertex, ExPoint2D rtVertex, ExPoint2D rbVertex) : base()
      {
         LeftTopVertex = ltVertex;
         LeftBottomVertex = lbVertex;
         RightTopVertex = rtVertex;
         RightBottomVertex = rbVertex;
         (StraightLine2D L1, StraightLine2D L2) lines = GetDiagonals();
         bool distanceIsEqual = lines.L1.GetDistanceWith() != lines.L2.GetDistanceWith();
         bool includeAngleIsZero = GetDiagonalIncludedAngle().Size == 0;
         if (distanceIsEqual || includeAngleIsZero) throw new NonStandardGraphicalException();
      }
      /// <summary>
      /// 获取当前矩形实例尺寸大小的长度分量。
      /// </summary>
      public double Height => new StraightLine2D(LeftTopVertex, LeftBottomVertex).GetDistanceWith();
      /// <summary>
      /// 获取当前矩形实例尺寸大小的宽度分量。
      /// </summary>
      public double Width => new StraightLine2D(LeftTopVertex, RightTopVertex).GetDistanceWith();
      /// <summary>
      /// 获取当前实例所表示矩形的位置，这个位置一般都是矩形左上角的顶点。
      /// </summary>
      public ExPoint2D Position => LeftTopVertex;
      /// <summary>
      /// 获取当前矩形的面积。
      /// </summary>
      public override double MeasureOfArea => Height * Width;
      /// <summary>
      /// 获取当前矩形的周长。
      /// </summary>
      public override double Perimeter => Height * 2 + Width * 2;
      /// <summary>
      /// 获取当前矩形的对角线。
      /// </summary>
      /// <returns>该操作会返回一个以元组形式保存的对角线集合。</returns>
      public (StraightLine2D diagonal01, StraightLine2D diagonal02) GetDiagonals() => ((LeftTopVertex, RightBottomVertex), (LeftBottomVertex, RightTopVertex));
      /// <summary>
      /// 获取矩形对角线之间的夹角。
      /// </summary>
      /// <returns>该操作会返回一个以弧度制作为计量单位的对角线夹角。</returns>
      public Angle GetDiagonalIncludedAngle()
      {
         (StraightLine2D L1, StraightLine2D L2) lines = GetDiagonals();
         Angle angle = new Angle(EAngleMeteringMode.Radian)
         {
            Size = lines.L1.GetIntersectionLineAngle(lines.L2)
         };
         return angle;
      }
   }
   /// <summary>
   /// 当一个图形的定义不符合逻辑规范时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class NonStandardGraphicalException : Exception
   {
      public NonStandardGraphicalException() : base("图形定义不符合逻辑规范！") { }
      public NonStandardGraphicalException(string message) : base(message) { }
      public NonStandardGraphicalException(string message, Exception inner) : base(message, inner) { }
      protected NonStandardGraphicalException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
