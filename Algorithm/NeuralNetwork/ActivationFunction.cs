using System;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm.NeuralNetwork
{
   /// <summary>
   /// 人工神经网络激活函数层的静态实现类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class ActivationFunction
   {
      /// <summary>
      /// Sigmoid函数。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <returns>该操作会返回一个介于0和1之间的目标数据。</returns>
      public static double Sigmoid(double sourceData) => 1 / (1 + Math.Pow(MathConstant.UniversalConstant, -sourceData));
      /// <summary>
      /// 双曲正切函数。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <returns>该操作会返回一个介于0和1之间的目标数据。</returns>
      public static double HyperbolicTan(double sourceData)
      {
         double dividend = Math.Pow(MathConstant.UniversalConstant, sourceData) - Math.Pow(MathConstant.UniversalConstant, -sourceData);
         double denominator = Math.Pow(MathConstant.UniversalConstant, sourceData) + Math.Pow(MathConstant.UniversalConstant, -sourceData);
         return dividend / denominator;
      }
      /// <summary>
      /// 阈值函数，如果输入的源数据在0和1之间，则将会抛出异常。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <returns>该函数只存在两个返回值，要么是0要么是1。</returns>
      /// <exception cref="ArgumentOutOfRangeException">当源数据在0和1之间，则将会抛出这个异常。</exception>
      public static int HLThreshold(double sourceData)
      {
         int threshold = 0;
         if (sourceData < 0) threshold = 0;
         else if (sourceData >= 1) threshold = 1;
         else throw new ArgumentOutOfRangeException("运算出现溢出性异常。");
         return threshold;
      }
      /// <summary>
      /// 纯线性函数，作为数学方程式f(x)=x的实现。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <returns>该方法的返回值与输入的源数据相同，方法的内部实现在数学层面上不参与任何计算。</returns>
      public static double PurelyLiner(double sourceData) => sourceData;
      /// <summary>
      /// ReLU函数，但是使用这个函数在进行网络训练时，整个神经网络可能将会显得无比脆弱。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <returns>该操作会返回一个大于等于0的目标数据。</returns>
      /// <remarks>ReLU的全称是Rectified Linear Units，是一种后来才出现的激活函数。 可以看到，当sourceData小于0时，ReLU硬饱和，而当sourceData大于0时，则不存在饱和问题。所以，ReLU能够在sourceData>0时保持梯度不衰减，从而缓解梯度消失问题。这让我们能够直接以监督的方式训练深度神经网络，而无需依赖无监督的逐层预训练。然而，随着训练的推进，部分输入会落入硬饱和区，导致对应权重无法更新。这种现象被称为“神经元死亡”。与Sigmoid类似，ReLU的输出均值也大于0，偏移现象和神经元死亡会共同影响网络的收敛性。</remarks>
      public static double ReLU(double sourceData) => sourceData < 0 ? 0 : sourceData;
      /// <summary>
      /// ELU函数，针对于ReLU的改进函数。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <param name="alpha">用于实现函数图像左侧软饱和性的参数，参数建议设置为小于1大于0的范围。</param>
      /// <returns>该操作将会得到一个类似于指数函数的函数结果。</returns>
      /// <remarks>该函数融合了Sigmoid和ReLU，左侧具有软饱和性，右侧无饱和性。右侧线性部分使得ELU能够缓解梯度消失，而左侧软饱能够让ELU对输入产生变化以及具有较高的噪声鲁棒性。</remarks>
      public static double ELU(double sourceData, double alpha)
      {
         double result = sourceData;
         if (sourceData < 0) result = alpha * (Math.Pow(MathConstant.UniversalConstant, sourceData) - 1);
         return result;
      }
      /// <summary>
      /// Parametric ReLU函数。
      /// </summary>
      /// <param name="sourceData">需要被处理的源数据。</param>
      /// <param name="alpha">用于实现函数图像左侧平缓线性的参数，该参数建议初始化为0.25，不采用正则，参数建议设置为小于1大于0的范围，如果这个参数被设置为0.01，则该函数为Leaky ReLU函数。</param>
      /// <returns>该函数会基于条件可能会返回不同类别的线性函数的结果。</returns>
      public static double ParametricReLU(double sourceData, double alpha) => sourceData < 0 ? alpha * sourceData : sourceData;
   }
   /// <summary>
   /// 用于表示学习过程需要采用何种激活函数的枚举。
   /// </summary>
   public enum EActivationFunctionUsing : int
   {
      /// <summary>
      /// 采用Sigmoid函数。
      /// </summary>
      [EnumerationDescription("Sigmoid函数")]
      Sigmoid = 0x0000,
      /// <summary>
      /// 采用双曲正切函数。
      /// </summary>
      [EnumerationDescription("双曲正切函数")]
      HyperbolicTan = 0x0001,
      /// <summary>
      /// 采用阈值函数。
      /// </summary>
      [EnumerationDescription("阈值函数")]
      HLThreshold = 0x0002,
      /// <summary>
      /// 采用纯线性函数。
      /// </summary>
      [EnumerationDescription("纯线性函数")]
      PurelyLiner = 0x0003,
      /// <summary>
      /// 采用规整化线性单元函数。
      /// </summary>
      [EnumerationDescription("规整化线性单元函数")]
      ReLU = 0x0004,
      /// <summary>
      /// 采用指数线性单元函数。
      /// </summary>
      [EnumerationDescription("指数线性单元函数")]
      ELU = 0x0005,
      /// <summary>
      /// 采用参数化规整线性单元函数。
      /// </summary>
      [EnumerationDescription("参数化规整线性单元函数")]
      ParametricReLU = 0x0006
   }
}
