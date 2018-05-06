using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm
{
   /// <summary>
   /// 用于实现常用的数学统计操作的类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class NumericalStatistics
   {
      /// <summary>
      /// 根据指定的浮点数集合来实现求和计算。
      /// </summary>
      /// <param name="elements">需要被用于求和计算的浮点数集合。</param>
      /// <returns>如果操作无异常，将会得到elements参数指定集合的和。</returns>
      /// <exception cref="CollectionNotEmptyException ">如果elements参数的元素数量等于零，则会抛出这个异常。</exception> 
      public static double Summation(List<double> elements)
      {
         double sum = 0;
         if (elements.Count == 0) throw new CollectionNotEmptyException();
         Parallel.For(0, elements.Count, (index) => sum += elements[index]);
         return sum;
      }
      /// <summary>
      /// 根据指定的浮点数集合来实现均值计算（算术平均值）。
      /// </summary>
      /// <param name="elements">需要被用于均值计算的浮点数集合。</param>
      /// <returns>如果操作无异常，将会得到elements参数指定集合的算术平均值。</returns>
      public static double Average(List<double> elements) => Summation(elements) / elements.Count;
      /// <summary>
      /// 根据指定的浮点数集合来实现均值计算（算术平均值），并通过reserved参数指定的无符号16位整形数来决定返回结果应该保留多少位小数。
      /// </summary>
      /// <param name="elements">需要被用于均值计算的浮点数集合。</param>
      /// <param name="reserved">用于决定计算结果应该保留多少位小数。</param>
      /// <returns>如果操作无异常，将会得到elements参数指定集合且保留reserved参数指定保留小数位数的算术平均值。</returns>
      [CLSCompliant(false)]
      public static double Average(List<double> elements, ushort reserved)
      {
         return Math.Round(Average(elements), reserved);
      }
      /// <summary>
      /// 根据指定的浮点数集合来实现加权平均数计算。
      /// </summary>
      /// <param name="elements">需要被用于加权平均数计算的浮点数集合。</param>
      /// <param name="weights">计算这个加权平均数所用到的权重集合。</param>
      /// <returns>如果操作无异常，将会得到被计算元素集合的加权平均数。</returns>
      /// <exception cref="WeightNotEqualOneException">如果当权重和不等于1时，则会抛出这个异常。</exception> 
      /// <exception cref="ArgumentException">当被计算元素量和权重元素量不相等时，则会抛出这个异常。</exception> 
      public static double WeightAverage(List<double> elements, List<double> weights)
      {
         if (Summation(weights) != 1) throw new WeightNotEqualOneException();
         if (elements.Count != weights.Count) throw new ArgumentException("权重元素量和计算元素量不相等！", "elements or weights");
         List<double> component = new List<double>();
         Parallel.For(0, elements.Count, (index) => component[index] = elements[index] * weights[index]);
         return Summation(component);
      }
      /// <summary>
      /// 根据指定的浮点数集合来计算这个集合的方差。
      /// </summary>
      /// <param name="elements">需要被用于方差计算的浮点数集合。</param>
      /// <param name="isSampleVariance">用于决定这个操作是否返回样本方差，如果该参数为true，则函数为样本方差计算。</param>
      /// <returns>如果操作无异常，该操作将会返回一个以浮点数表示的指定集合的（样本）方差。</returns>
      public static double Variance(List<double> elements, bool isSampleVariance)
      {
         double variance = 0;
         double average = Average(elements);
         List<double> component = new List<double>();
         Parallel.For(0, elements.Count, (index) => component[index] = Math.Pow(elements[index] - average, 2));
         if (isSampleVariance) variance = Summation(component) / (elements.Count - 1);
         else variance = Summation(component) / elements.Count;
         return variance;
      }
      /// <summary>
      /// 根据指定的浮点数集合来计算这个集合的标准差。
      /// </summary>
      /// <param name="elements">需要被用于标准差计算的浮点数集合。</param>
      /// <param name="isSampleStandardDeviation">用于决定这个操作是否返回样本标准差，如果该参数为true，则函数为样本标准差计算。</param>
      /// <returns>如果操作无异常，该操作将会返回一个以浮点数表示的指定集合的（样本）标准差。</returns>
      public static double StandardDeviation(List<double> elements, bool isSampleStandardDeviation)
      {
         return Math.Sqrt(Variance(elements, isSampleStandardDeviation));
      }
   }
   /// <summary>
   /// 当数组或者列表的元素数量为空时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class CollectionNotEmptyException : Exception
   {
      public CollectionNotEmptyException() : base("列表或者数组的元素数量不能为空！") { }
      public CollectionNotEmptyException(string message) : base(message) { }
      public CollectionNotEmptyException(string message, Exception inner) : base(message, inner) { }
      protected CollectionNotEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
   /// <summary>
   /// 当权重不等于1时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class WeightNotEqualOneException : Exception
   {
      public WeightNotEqualOneException() : base("权重之和必须为1。") { }
      public WeightNotEqualOneException(string message) : base(message) { }
      public WeightNotEqualOneException(string message, Exception inner) : base(message, inner) { }
      protected WeightNotEqualOneException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
