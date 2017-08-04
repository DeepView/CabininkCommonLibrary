using System;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 描述二维直线的扩展类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class StraightLine2D : IEquatable<StraightLine2D>
   {
      [CLSCompliant(false)]
      protected ExPoint2D _anyPoint01;//直线上任意的一个二维点。
      [CLSCompliant(false)]
      protected ExPoint2D _anyPoint02;//直线上另外一个任意二维点。
      /// <summary>
      /// 构造函数，定义一条经过笛卡尔平面直角坐标系原点的直线。
      /// </summary>
      /// <param name="nonOriginPoint">在任意象限和轴上的非原点的任意二维点。</param>
      /// <exception cref="UnknownDirectionLineException">当无法确定所定义的直线的斜率时，则会抛出这个异常。</exception>
      public StraightLine2D(ExPoint2D nonOriginPoint)
      {
         if (new ExPoint2D().Equals(nonOriginPoint)) throw new UnknownDirectionLineException();
         else
         {
            _anyPoint01 = new ExPoint2D();
            _anyPoint02 = nonOriginPoint;
         }
      }
      /// <summary>
      /// 构造函数，定义一条穿过任意两个二维点的直线。
      /// </summary>
      /// <param name="point01">任意一个二维点。</param>
      /// <param name="point02">另外一个任意二维点。</param>
      /// <exception cref="UnknownDirectionLineException">当无法确定所定义的直线的斜率时，则会抛出这个异常。</exception>
      public StraightLine2D(ExPoint2D point01, ExPoint2D point02)
      {
         if (point01.Equals(point02)) throw new UnknownDirectionLineException();
         else
         {
            _anyPoint01 = point01;
            _anyPoint02 = point02;
         }
      }
      /// <summary>
      /// 获取或设置当前二维直线所穿过的第一个二维点，这个点已经在该实例中已定义（protected ExPoint2D _anyPoint01）。
      /// </summary>
      /// <exception cref="UnknownDirectionLineException">当无法确定所定义的直线的斜率时，则会抛出这个异常。</exception>
      public ExPoint2D FirstPoint
      {
         get => _anyPoint01;
         set
         {
            if (_anyPoint01.Equals(LastPoint)) throw new UnknownDirectionLineException();
            else _anyPoint01 = value;
         }
      }
      /// <summary>
      /// 获取或设置当前二维直线所穿过的第二个二维点，这个点已经在该实例中已定义（protected ExPoint2D _anyPoint01）。
      /// </summary>
      /// <exception cref="UnknownDirectionLineException">当无法确定所定义的直线的斜率时，则会抛出这个异常。</exception>
      public ExPoint2D LastPoint
      {
         get => _anyPoint01;
         set
         {
            if (_anyPoint02.Equals(FirstPoint)) throw new UnknownDirectionLineException();
            else _anyPoint02 = value;
         }
      }
      /// <summary>
      /// 获取当前的二维直线的斜率。
      /// </summary>
      /// <exception cref="SlopeIsNaNException">当斜率无意义时（即直线任意两个不相等的点都在横轴上时），则会抛出这个异常。</exception>
      /// <remarks>当前函数的内部实现是基于斜截式直线方程y=kx+b实现，因此在某些特殊情况可能会引发一些无法预料的异常。</remarks>
      public double Slope
      {
         get
         {
            if (LastPoint.XPosition - FirstPoint.XPosition == 0) throw new SlopeIsNaNException();
            else return (LastPoint.YPosition - FirstPoint.YPosition) / (LastPoint.XPosition - FirstPoint.XPosition);
         }
      }
      /// <summary>
      /// 获取当前的二维直线的截距。
      /// </summary>
      /// <exception cref="InterceptIsNaNException">当直线与纵轴平行或者重合时，则会抛出这个异常。</exception>
      /// <remarks>当前函数的内部实现是基于斜截式直线方程y=kx+b实现，因此在某些特殊情况可能会引发一些无法预料的异常。</remarks>
      public double Intercept
      {
         get
         {
            if (LastPoint.YPosition == FirstPoint.YPosition) throw new InterceptIsNaNException();
            else return FirstPoint.YPosition - Slope * FirstPoint.XPosition;
         }
      }
      /// <summary>
      /// 计算当前的二维直线穿越的象限集合。
      /// </summary>
      /// <returns>该操作将会返回一个全整型数据元组，如果元组中第三个分量为0则表示该直线只穿越了两个象限，如果全为0则表示该直线为横轴或者纵轴。</returns>
      /// <remarks>当前函数的内部实现是基于斜截式直线方程y=kx+b实现，因此在某些特殊情况可能会引发一些无法预料的异常。</remarks>
      [CLSCompliant(false)]
      public (uint quadrant01, uint quadrant02, uint quadrant03) GetCrossedQuadrant()
      {
         (uint q1, uint q2, uint q3) quadrants = (1, 2, 3);
         if (IsCPWithVerticalAxis() || IsCPWithHorizontalAxis()) quadrants = (0, 0, 0);
         else
         {
            if (Slope > 0)
            {
               if (Intercept > 0) quadrants = (1, 2, 3);
               else if (Intercept < 0) quadrants = (1, 3, 4);
               else if (Intercept == 0) quadrants = (1, 3, 0);
            }
            else if (Slope < 0)
            {
               if (Intercept > 0) quadrants = (1, 2, 4);
               else if (Intercept < 0) quadrants = (2, 3, 4);
               else if (Intercept == 0) quadrants = (2, 4, 0);
            }
         }
         return quadrants;
      }
      /// <summary>
      /// 获取两条相交二维直线的夹角，使用弧度作为计量单位。
      /// </summary>
      /// <param name="line">用于比较和计算的二维直线。</param>
      /// <returns>该操作将会返回这两条直线的夹角，但是在夹角为0的情况下，该函数的结果无法作为判断这两条直线是否重合或者平行的有效依据。</returns>
      public double GetIntersectionLineAngle(StraightLine2D line)
      {
         double alpha_s = Slope;
         double beta_s = line.Slope;
         if (IsCPWithHorizontalAxis() && line.IsCPWithVerticalAxis()) return 90;
         else if (line.IsCPWithHorizontalAxis() && IsCPWithVerticalAxis()) return 90;
         else return Math.Atan(Math.Abs(alpha_s - beta_s) / (1 + alpha_s * beta_s));
      }
      /// <summary>
      /// 判断指定的点是否在当前的二维直线上。
      /// </summary>
      /// <param name="point">用于被判断的二维点。</param>
      /// <returns>如果参数point指定的点在这当前二维直线上，则会返回true，否则返回false。</returns>
      public bool IsMemberPoint(ExPoint2D point) => GetIntersectionLineAngle((FirstPoint, point)) == 0 ? true : false;
      /// <summary>
      /// 获取当前二维直线与制定的二维直线相交时的交点。
      /// </summary>
      /// <param name="line">指定的二维直线。</param>
      /// <returns>如果不抛出任何异常，则该操作将会返回一个两条直线的交点。</returns>
      /// <remarks>当前函数的内部实现是基于斜截式直线方程y=kx+b实现，因此在某些特殊情况可能会引发一些无法预料的异常。</remarks>
      public ExPoint2D GetCrossPoint(StraightLine2D line)
      {
         ExPoint2D cross = new ExPoint2D()
         {
            XPosition = (Intercept - line.Intercept) / (line.Slope - Slope)
         };
         cross.YPosition = Slope * cross.XPosition + Intercept;
         return cross;
      }
      /// <summary>
      /// 判断两条二维直线之间的关系。
      /// </summary>
      /// <param name="line">用于判断的另一条二维直线。</param>
      /// <returns>该操作如果不抛出任何异常，则将会返回这两条直线之间的关系，这个关系要么是重合，要么是相交，要么就是平行。</returns>
      /// <remarks>当前函数的内部实现是基于斜截式直线方程y=kx+b实现，因此在某些特殊情况可能会引发一些无法预料的异常。</remarks>
      public EDoubleLineRelationship GetRelationship(StraightLine2D line)
      {
         EDoubleLineRelationship relationship = EDoubleLineRelationship.Intersect;
         if (GetIntersectionLineAngle(line) == 0)
         {
            if (IsMemberPoint(line.FirstPoint)) relationship = EDoubleLineRelationship.Coincidence;
            else relationship = EDoubleLineRelationship.Parallel;
         }
         return relationship;
      }
      /// <summary>
      /// 获取实例中初始化的那两个点之间的距离。
      /// </summary>
      /// <returns>该操作将会返回初始化时指定的两个被直线经过的二维点之间的距离。</returns>
      public double GetDistanceWith() => FirstPoint.GetDistance(LastPoint);
      /// <summary>
      /// 判断当前直线是否平行或者与重合于笛卡尔平面直角坐标系的横轴（X轴）。
      /// </summary>
      /// <returns>如果当前直线平行或者与重合于笛卡尔平面直角坐标系的横轴，则会返回true，否则会返回false。</returns>
      public bool IsCPWithHorizontalAxis() => FirstPoint.YPosition == LastPoint.YPosition ? true : false;
      /// <summary>
      /// 判断当前直线是否平行或者与重合于笛卡尔平面直角坐标系的纵轴（Y轴）。
      /// </summary>
      /// <returns>如果当前直线平行或者与重合于笛卡尔平面直角坐标系的纵轴，则会返回true，否则会返回false。</returns>
      public bool IsCPWithVerticalAxis() => FirstPoint.XPosition == LastPoint.XPosition ? true : false;
      /// <summary>
      /// 将当前直线根据指定的点旋转指定角度。
      /// </summary>
      /// <param name="axisPoint">旋转直线的轴心点。</param>
      /// <param name="angle">旋转的角度大小。</param>
      /// <returns>该操作会返回一条围绕指定点旋转后的二维直线实例。</returns>
      public StraightLine2D Rotate(ExPoint2D axisPoint, Angle angle) => (FirstPoint.Rotate(axisPoint, angle), LastPoint.Rotate(axisPoint, angle));
      /// <summary>
      /// 确定此实例是否与另一个指定的StraightLine2D对象具有相同的值。
      /// </summary>
      /// <param name="other">要与此实例进行比较的二维直线实例。</param>
      /// <returns>如果参数的值与此实例的值相同，则为value；否则为false。如果value为null，则此方法返回false。</returns>
      public bool Equals(StraightLine2D other)
      {
         bool isEqual = false;
         double k1 = Slope, k2 = other.Slope, b1 = Intercept, b2 = other.Intercept;
         if (k1 == k2 && b1 == b2) isEqual = true;
         return isEqual;
      }
      /// <summary>
      /// 隐式转换操作符重载（To StraightLine2D）。
      /// </summary>
      /// <param name="line">隐式转换操作符的源类型。</param>
      public static implicit operator StraightLine2D((ExPoint2D fPoint, ExPoint2D lPoint) line) => new StraightLine2D(line.fPoint, line.lPoint);
      /// <summary>
      /// 隐式转换操作符重载（To (ExPoint2D, ExPoint2D)）。
      /// </summary>
      /// <param name="line">隐式转换操作符的源类型。</param>
      public static implicit operator (ExPoint2D, ExPoint2D) (StraightLine2D line) => (line.FirstPoint, line.LastPoint);
      /// <summary>
      /// 返回StraightLine2D实例的特定字符串表达形式。
      /// </summary>
      /// <returns>一个特定的字符串表达形式，比如说存在一个直线斜率和纵轴偏移量为3和5的直线，则这个字符串为StraightLine2D:y=3x+5。</returns>
      public override string ToString() => "StraightLine2D:y=" + Slope + "x+" + Intercept;
   }
   /// <summary>
   /// 几何学基础角度计量制度枚举。
   /// </summary>
   public enum EAngleMeteringMode : int
   {
      /// <summary>
      /// 弧度制。
      /// </summary>
      Radian = 0x0000,
      /// <summary>
      /// 角度制。
      /// </summary>
      AngleSystem = 0xffff
   }
   /// <summary>
   /// 两条直线之间的关系枚举。
   /// </summary>
   public enum EDoubleLineRelationship : int
   {
      /// <summary>
      /// 两条直线重合。
      /// </summary>
      Coincidence = 0x0000,
      /// <summary>
      /// 两条直线平行。
      /// </summary>
      Parallel = 0x0001,
      /// <summary>
      /// 两条直线相交。
      /// </summary>
      Intersect = 0x0002
   }
   /// <summary>
   /// 当无法确定直线的方向或者与坐标系横轴的夹角时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class UnknownDirectionLineException : Exception
   {
      public UnknownDirectionLineException() : base("无法确定直线的方向或者与坐标系横轴的夹角！") { }
      public UnknownDirectionLineException(string message) : base(message) { }
      public UnknownDirectionLineException(string message, Exception inner) : base(message, inner) { }
      protected UnknownDirectionLineException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当直线的斜率为非数字时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class SlopeIsNaNException : Exception
   {
      public SlopeIsNaNException() : base("斜率为非数字或者无意义！") { }
      public SlopeIsNaNException(string message) : base(message) { }
      public SlopeIsNaNException(string message, Exception inner) : base(message, inner) { }
      protected SlopeIsNaNException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当直线的截距为非数字时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class InterceptIsNaNException : Exception
   {
      public InterceptIsNaNException() { }
      public InterceptIsNaNException(string message) : base(message) { }
      public InterceptIsNaNException(string message, Exception inner) : base(message, inner) { }
      protected InterceptIsNaNException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
