using System;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm
{
   /// <summary>
   /// 常用的物理常量密封类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class PhysicsConstant
   {
      /// <summary>
      /// 真空光速，单位m/s。
      /// </summary>
      public static int LightSpeed => 299792458;
      /// <summary>
      /// 真空电容率，单位F/m。
      /// </summary>
      public static double Permittivity => 8.854187817e-12;
      /// <summary>
      /// 引力常量，单位m^3/(kg·s^2)。
      /// </summary>
      public static double GravitationalConstant => 6.673e-11;
      /// <summary>
      /// 普朗克常量，单位J·s。
      /// </summary>
      public static double PlanckConstantWithJS => 6.62606957e-34;
      /// <summary>
      /// 普朗克常量，单位eV·s。
      /// </summary>
      public static double PlanckConstantWithEVS => 4.13566743e-15;
      /// <summary>
      /// 基本电荷，单位C。
      /// </summary>
      public static double ElementaryCharge => 1.602176565e-19;
      /// <summary>
      /// 玻尔兹曼常数，单位J/K。
      /// </summary>
      public static double BoltzmannConstant => 1.3806505e-23;
      /// <summary>
      /// 天文单位，单位m。
      /// </summary>
      public static long AstronomicalUnit => 149597870660;
      /// <summary>
      /// 秒差距，单位l.y.。
      /// </summary>
      public static double ParsecWithLY => 3.2615637771418798291;
      /// <summary>
      /// 秒差距，单位AU。
      /// </summary>
      public static double ParsecWithAU => 206264.806245480309553;
      /// <summary>
      /// 光年，单位m。
      /// </summary>
      public static long LightYear => 9460730472580800;
   }
}
