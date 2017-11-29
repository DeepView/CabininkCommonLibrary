using System;
using Cabinink.Algorithm;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 数学角度类，用于描述一个角度。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Angle : UnregoinallyShape, IEquatable<Angle>
   {
      private double _angleSize;//角度大小。
      private EAngleMeteringMode _meteringMode;//计量单位（弧度制或者角度制）。
      /// <summary>
      /// 构造函数，创建一个指定计量单位的角度实例。
      /// </summary>
      /// <param name="meteringMode">指定的角度计量单位。</param>
      public Angle(EAngleMeteringMode meteringMode)
      {
         _angleSize = 0;
         _meteringMode = meteringMode;
      }
      /// <summary>
      /// 获取或设置当前实例的角度大小。
      /// </summary>
      public double Size { get => _angleSize; set => _angleSize = value; }
      /// <summary>
      /// 获取当前图形是否被允许在Graphics对象中绘制。
      /// </summary>
      public override bool IsAllowDrawing => !base.IsAllowDrawing;
      /// <summary>
      /// 获取或设置当前实例的角度计量单位。
      /// </summary>
      public EAngleMeteringMode MeteringMode { get => _meteringMode; set => _meteringMode = value; }
      /// <summary>
      /// 角度转换，并更新当前角度实例。
      /// </summary>
      /// <remarks>该操作会将角度制转换为弧度制，或者是将弧度制转换为角度制，无论是哪种操作，都会更新整个实例。另外，转换计量单位时，也会转换角度大小的值，但实际上这个角度并没有变化，只是单位变化了，举例说明：在这里2π rnd和360°是等效的。</remarks>
      public void Convert()
      {
         if (MeteringMode == (EAngleMeteringMode)0xffff)
         {
            MeteringMode = 0x0000;
            Size = ConvertToRadin(Size);
         }
         else
         {
            MeteringMode = (EAngleMeteringMode)0xffff;
            Size = ConvertToAngleSys(Size);
         }
      }
      /// <summary>
      /// 获取旋转指定周期的角度，换句话说，该函数用于获取旋转360度倍数的角度实例。
      /// </summary>
      /// <param name="rotateLimit">旋转的倍数，这个值可以为负数，如果为负数则表示这个操作会返回一个反向旋转360度整数倍的角度实例。</param>
      /// <returns>该操作会返回一个旋转整数倍360度的角度实例。</returns>
      public Angle GetCycleRotateAngle(int rotateLimit) => this + GetPerigon() * rotateLimit;
      /// <summary>
      /// 获取当前Angle实例所表示的角度的正弦值。
      /// </summary>
      /// <returns>该操作会返回当前角度实例的正弦值，但这个值在数学上可能并不精确。</returns>
      public double GetSinusoidal()
      {
         if (MeteringMode != 0x0000) Convert();
         return Math.Sin(Size);
      }
      /// <summary>
      /// 获取当前Angle实例所表示的角度的余弦值。
      /// </summary>
      /// <returns>该操作会返回当前角度实例的余弦值，但这个值在数学上可能并不精确。</returns>
      public double GetCosinusoidal()
      {
         if (MeteringMode != 0x0000) Convert();
         return Math.Cos(Size);
      }
      /// <summary>
      /// 获取当前Angle实例所表示的角度的正切值。
      /// </summary>
      /// <returns>该操作会返回当前角度实例的正切值，但这个值在数学上可能并不精确。</returns>
      public double GetTangent()
      {
         if (MeteringMode != 0x0000) Convert();
         return Math.Tan(Size);
      }
      /// <summary>
      /// 获取当前Angle实例所表示的角度的双曲正弦值。
      /// </summary>
      /// <returns>该操作会返回当前角度实例的双曲正弦值，但这个值在数学上可能并不精确。</returns>
      public double GetHyperbolicSin()
      {
         if (MeteringMode != 0x0000) Convert();
         return Math.Sinh(Size);
      }
      /// <summary>
      /// 获取当前Angle实例所表示的角度的双曲余弦值。
      /// </summary>
      /// <returns>该操作会返回当前角度实例的双曲余弦值，但这个值在数学上可能并不精确。</returns>
      public double GetHyperbolicCos()
      {
         if (MeteringMode != 0x0000) Convert();
         return Math.Cosh(Size);
      }
      /// <summary>
      /// 获取当前Angle实例所表示的角度的双曲正切值。
      /// </summary>
      /// <returns>该操作会返回当前角度实例的双曲正切值，但这个值在数学上可能并不精确。</returns>
      public double GetHyperbolicTan()
      {
         if (MeteringMode != 0x0000) Convert();
         return Math.Tanh(Size);
      }
      /// <summary>
      /// 获取一个直角，可用作角度实例的常量。
      /// </summary>
      /// <returns>该操作会返回一个直角（90°）的角度实例。</returns>
      public static Angle GetRightAngle() => new Angle((EAngleMeteringMode)0xffff) { Size = 90 };
      /// <summary>
      /// 获取一个平角，可用作角度实例的常量。
      /// </summary>
      /// <returns>该操作会返回一个平角（180°）的角度实例。</returns>
      public static Angle GetBoxer() => GetRightAngle() * 2;
      /// <summary>
      /// 获取一个圆周角，可用作角度实例的常量。
      /// </summary>
      /// <returns>该操作会返回一个圆周角（360°）的角度实例。</returns>
      public static Angle GetPerigon() => GetBoxer() * 2;
      /// <summary>
      /// 将弧度制角度转换为角度制计量的角度。
      /// </summary>
      /// <param name="radAngle">一个弧度制角度。</param>
      /// <returns>该函数将会返回一个通过公式计算得出的转换结果。</returns>
      public static double ConvertToAngleSys(double radAngle) => 180 * radAngle / MathConstant.CircumferenceRatio;
      /// <summary>
      /// 将角度制角度转换为弧度制计量的角度。
      /// </summary>
      /// <param name="degAngle">一个角度制角度。</param>
      /// <returns>该函数将会返回一个通过公式计算得出的转换结果。</returns>
      public static double ConvertToRadin(double degAngle) => degAngle * MathConstant.CircumferenceRatio / 180;
      /// <summary>
      /// 比较当前角度是否与另外一个角度是否相同。
      /// </summary>
      /// <param name="other">用于作比较的另外一个角度实例。</param>
      /// <returns>如果当前角度相等，则会返回true，否则会返回false。</returns>
      public bool Equals(Angle other) => Size == other.Size && MeteringMode == other.MeteringMode;
      /// <summary>
      /// 加法操作符重载。
      /// </summary>
      /// <param name="a1">加法运算符左侧的运算元。</param>
      /// <param name="a2">加法运算符右侧的运算元。</param>
      /// <returns>该操作会实现两个角度的加法运算，并返回一个新的角度实例。</returns>
      public static Angle operator +(Angle a1, Angle a2)
      {
         Angle result = new Angle(0x0000);
         if (a1.MeteringMode == a2.MeteringMode) result.Size = a1.Size + a2.Size;
         else
         {
            if (a1.MeteringMode != 0x0000) a1.Convert();
            if (a2.MeteringMode != 0x0000) a2.Convert();
            result.Size = a1.Size + a2.Size;
         }
         return result;
      }
      /// <summary>
      /// 减法操作符重载。
      /// </summary>
      /// <param name="a1">减法运算符左侧的运算元。</param>
      /// <param name="a2">减法运算符右侧的运算元。</param>
      /// <returns>该操作会实现两个角度的减法运算，并返回一个新的角度实例。</returns>
      public static Angle operator -(Angle a1, Angle a2)
      {
         Angle result = new Angle(0x0000);
         if (a1.MeteringMode == a2.MeteringMode) result.Size = a1.Size - a2.Size;
         else
         {
            if (a1.MeteringMode != 0x0000) a1.Convert();
            if (a2.MeteringMode != 0x0000) a2.Convert();
            result.Size = a1.Size - a2.Size;
         }
         return result;
      }
      /// <summary>
      /// 乘法操作符重载。
      /// </summary>
      /// <param name="angle">增加倍率的角度实例。</param>
      /// <param name="rate">增加的倍率。</param>
      /// <returns>该操作会实现一个角度与一个浮点数的乘法运算，并返回一个新的角度实例。</returns>
      public static Angle operator *(Angle angle, double rate) => new Angle(angle.MeteringMode) { Size = angle.Size * rate };
      /// <summary>
      /// 除法操作符重载。
      /// </summary>
      /// <param name="angle">用于被均衡分割的角度实例。</param>
      /// <param name="copies">分割的份数。</param>
      /// <returns>该操作会实现一个角度与一个浮点数的除法运算，并返回一个新的角度实例。</returns>
      public static Angle operator /(Angle angle, double copies) => new Angle(angle.MeteringMode) { Size = angle.Size / copies };
   }
}
