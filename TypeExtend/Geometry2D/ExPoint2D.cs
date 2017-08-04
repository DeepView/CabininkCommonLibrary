using System;
using System.Drawing;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 二维点描述类，可以表示一个在平面上的点。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class ExPoint2D : IEquatable<ExPoint2D>, ITranslation2D
   {
      private double _x;//二维点的横坐标。
      private double _y;//二维点的纵坐标。
      /// <summary>
      /// 构造函数，初始化一个X和Y都为0的二维点。
      /// </summary>
      public ExPoint2D()
      {
         _x = 0;
         _y = 0;
      }
      /// <summary>
      /// 构造函数，初始化一个指定X和Y的二维点。
      /// </summary>
      /// <param name="x">指定的二维点横坐标X。</param>
      /// <param name="y">指定的二维点纵坐标Y。</param>
      public ExPoint2D(double x, double y)
      {
         _x = x;
         _y = y;
      }
      /// <summary>
      /// 构造函数，通过一个System.Drawing.PointF结构实例来初始化当前的二维点。
      /// </summary>
      /// <param name="point">一个System.Drawing.PointF结构实例，用于初始化二维点。</param>
      public ExPoint2D(PointF point)
      {
         _x = point.X;
         _y = point.Y;
      }
      /// <summary>
      /// 获取或设置当前实例的二维点X坐标。
      /// </summary>
      public double XPosition { get => _x; set => _x = value; }
      /// <summary>
      /// 获取或设置当前实例的二维点Y坐标。
      /// </summary>
      public double YPosition { get => _y; set => _y = value; }
      /// <summary>
      /// 获取当前实例所表示的二维点是否为笛卡尔平面直角坐标系的原点。
      /// </summary>
      public bool IsOrigin => GetQuadrantInCoordinateSystem2D() == 0 ? true : false;
      /// <summary>
      /// 横坐标平移。
      /// </summary>
      /// <param name="offset">横坐标X的偏移量。</param>
      public void AbscissaTranslation(double offset) => XPosition += offset;
      /// <summary>
      /// 纵坐标平移。
      /// </summary>
      /// <param name="offset">纵坐标Y的偏移量。</param>
      public void OrdinateTranslation(double offset) => YPosition += offset;
      /// <summary>
      /// 坐标平移，也可以叫做坐标相加。
      /// </summary>
      /// <param name="offsetPoint">一个偏移点，这个点包含了横坐标X和纵坐标Y的偏移量。</param>
      public void Translation(ExPoint2D offsetPoint)
      {
         AbscissaTranslation(offsetPoint.XPosition);
         OrdinateTranslation(offsetPoint.YPosition);
      }
      /// <summary>
      /// 获取两点之间的距离。
      /// </summary>
      /// <param name="point">用于获取距离的另一个二维点。</param>
      /// <returns>该操作将会返回两个点之间的距离，这个返回结果类型为double。</returns>
      public double GetDistance(ExPoint2D point)
      {
         double xPow2 = Math.Pow(XPosition - point.XPosition, 2);
         double yPow2 = Math.Pow(YPosition - point.YPosition, 2);
         return Math.Abs(Math.Sqrt(xPow2 + yPow2));
      }
      /// <summary>
      /// 获取当前点在笛卡尔平面直角坐标系中的象限或者位置。
      /// </summary>
      /// <returns>如果操作返回1~4，则分别表示这个点在第一至第四象限，如果返回0表示该点处于原点上，若返回10或者-10则表示该点处于X正半轴或者X负半轴，若返回20或者-20则表示该点处于Y正半轴或者Y负半轴。</returns>
      public int GetQuadrantInCoordinateSystem2D()
      {
         int quadrantOrPosition;
         if (XPosition > 0 && YPosition > 0) quadrantOrPosition = 1;
         else if (XPosition < 0 && YPosition > 0) quadrantOrPosition = 2;
         else if (XPosition < 0 && YPosition < 0) quadrantOrPosition = 3;
         else if (XPosition > 0 && YPosition < 0) quadrantOrPosition = 4;
         else if (XPosition > 0 && YPosition == 0) quadrantOrPosition = 10;
         else if (XPosition < 0 && YPosition == 0) quadrantOrPosition = -10;
         else if (XPosition == 0 && YPosition > 0) quadrantOrPosition = 20;
         else if (XPosition == 0 && YPosition < 0) quadrantOrPosition = -20;
         else quadrantOrPosition = 0;
         return quadrantOrPosition;
      }
      /// <summary>
      /// 将当前的点围绕指定的中心点逆时针旋转指定角度。
      /// </summary>
      /// <param name="axisPoint">二维点旋转的中心点。</param>
      /// <param name="angle">逆时针旋转的角度。</param>
      /// <returns>这个操作会返回一个通过逆时针旋转后获取到的新的二维点实例。</returns>
      public ExPoint2D Rotate(ExPoint2D axisPoint, Angle angle)
      {
         double xPos = 0;
         double yPos = 0;
         double deltaX = XPosition - axisPoint.XPosition;
         double deltaY = YPosition - axisPoint.YPosition;
         xPos = deltaX * angle.GetCosinusoidal() + deltaY * angle.GetSinusoidal() + axisPoint.XPosition;
         yPos = deltaX * angle.GetSinusoidal() + deltaY * angle.GetCosinusoidal() + axisPoint.YPosition;
         return (xPos, yPos);
      }
      /// <summary>
      /// 获取两个点的中点坐标。
      /// </summary>
      /// <param name="point">用于获取中点坐标的另一个二维点。</param>
      /// <returns>该操作将会返回两个点之间的中点坐标，这个返回结果类型为PointF。</returns>
      public ExPoint2D GetMidpoint(ExPoint2D point) => ((XPosition + point.XPosition) / 2, (YPosition + point.YPosition) / 2);
      /// <summary>
      /// 隐式转换操作符重载（To ExPoint2D）。
      /// </summary>
      /// <param name="point">隐式转换操作符的源类型。</param>
      public static implicit operator ExPoint2D(PointF point) => new ExPoint2D(point);
      /// <summary>
      /// 隐式转换操作符重载（To PointF）。
      /// </summary>
      /// <param name="point">隐式转换操作符的源类型。</param>
      public static implicit operator PointF(ExPoint2D point) => new PointF((float)point.XPosition, (float)point.YPosition);
      /// <summary>
      /// 隐式转换操作符重载（To ExPoint2D）。
      /// </summary>
      /// <param name="pointLocation">隐式转换操作符的源类型。</param>
      public static implicit operator ExPoint2D((double x, double y) pointLocation) => new ExPoint2D(pointLocation.x, pointLocation.y);
      /// <summary>
      /// 隐式转换操作符重载（To (double, double)）。
      /// </summary>
      /// <param name="point">隐式转换操作符的源类型。</param>
      public static implicit operator (double, double) (ExPoint2D point) => (point.XPosition, point.YPosition);
      /// <summary>
      /// 隐式转换操作符重载（To ExPoint2D）。
      /// </summary>
      /// <param name="pointLocation">隐式转换操作符的源类型。</param>
      public static implicit operator ExPoint2D((int x, int y) pointLocation) => new ExPoint2D(pointLocation.x, pointLocation.y);
      /// <summary>
      /// 隐式转换操作符重载（To (int, int)）。
      /// </summary>
      /// <param name="point">隐式转换操作符的源类型。</param>
      public static implicit operator (int, int) (ExPoint2D point) => ((int)point.XPosition, (int)point.YPosition);
      /// <summary>
      /// 返回ExPoint2D实例的特定字符串表达形式。
      /// </summary>
      /// <returns>一个特定的字符串表达形式，比如说存在一个点的坐标为3和6，则这个字符串为ExPoint2D:X=3,Y=6。</returns>
      public override string ToString() => "ExPoint2D:X=" + XPosition.ToString() + ",Y=" + YPosition.ToString();
      /// <summary>
      /// 确定此实例是否与另一个指定的ExPoint2D对象具有相同的值。
      /// </summary>
      /// <param name="other">要与此实例进行比较的扩展二维点。</param>
      /// <returns>如果参数的值与此实例的值相同，则为value；否则为false。如果value为null，则此方法返回false。</returns>
      public bool Equals(ExPoint2D other)
      {
         bool isEqual = false;
         if (XPosition == other.XPosition && YPosition == other.YPosition) isEqual = true;
         return isEqual;
      }
   }
}
