using System;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm
{
   /// <summary>
   /// 常用的数学常量密封类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class MathConstant
   {
      /// <summary>
      /// 圆周率。
      /// </summary>
      /// <returns>返回圆周率常量。</returns>
      /// <remarks>圆周率（Circumference Ratio）。</remarks>
      public static double CircumferenceRatio => 3.14159265358979;
      /// <summary>
      /// 自然常数。
      /// </summary>
      /// <returns>返回自然常数。</returns>
      /// <remarks>自然常数（Universal Constant）。</remarks>
      public static double UniversalConstant => 2.71828188284591;
      /// <summary>
      /// 欧拉常数。
      /// </summary>
      /// <returns>返回欧拉常数。</returns>
      /// <remarks>欧拉常数（Euler-Mascheroni Constant）。</remarks>
      public static double EulerMascheroniConstant => 0.577215664901533;
      /// <summary>
      /// 费根鲍姆常数。
      /// </summary>
      /// <returns>返回费根鲍姆常数。</returns>
      /// <remarks>费根鲍姆常数（Feigenbaum Constant）。</remarks>
      public static double FeigenbaumConstant => 4.66920160910299;
      /// <summary>
      /// 黄金分割数。
      /// </summary>
      /// <returns>返回黄金分割数。</returns>
      /// <remarks>黄金分割数（Golden Section Number）。</remarks>
      public static double GoldenSectionNumber => 0.618033988749895;
      /// <summary>
      /// 卡特兰数。
      /// </summary>
      /// <returns>返回卡特兰数。</returns>
      /// <remarks>卡特兰数（Catalan Number）。</remarks>
      public static double CatalanNumber => 0.915965594177219;
      /// <summary>
      /// 卡钦常数
      /// </summary>
      /// <returns>返回卡钦常数。</returns>
      /// <remarks>卡钦常数（Khinchin Constant）。</remarks>
      public static double KhinchinConstant => 2.68545200106531;
      /// <summary>
      /// 孪生质数常数。
      /// </summary>
      /// <returns>返回孪生质数常数。</returns>
      /// <remarks>孪生质数常数（Twin Prime Constant）。</remarks>
      public static double TwinPrimeConstant => 0.66016181584687;
      /// <summary>
      /// 毕达哥拉斯常数。
      /// </summary>
      /// <returns>返回毕达哥拉斯常数。</returns>
      /// <remarks>毕达哥拉斯常数（Pythagoras Constant）。</remarks>
      public static double PythagorasConstant => 1.4142135623731;
      /// <summary>
      /// Meissel-Mertens常数。
      /// </summary>
      /// <returns>返回Meissel-Mertens常数。</returns>
      /// <remarks>Meissel-Mertens常数（Meissel-Mertens Constant）。</remarks>
      public static double MeisselMertensConstant => 0.261497212847643;
      /// <summary>
      /// 塑胶数。
      /// </summary>
      /// <returns>返回塑胶数。</returns>
      /// <remarks>塑胶数（Plastic Number）。</remarks>
      public static double PlasticNumber => 1.32471795724475;
      /// <summary>
      /// 兰道-拉马努金常数
      /// </summary>
      /// <returns>返回兰道-拉马努金常数。</returns>
      /// <remarks>兰道-拉马努金常数（Landau-Ramanujan Constant）。</remarks>
      public static double LandauRamanujanConstant => 0.764223653589221;
   }
}
