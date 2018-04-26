using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm.IntelligentLearning
{
   /// <summary>
   /// BP深度学习神经网络算法类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class BPDeepLearningNetwork
   {
      private double[][] _layer;//神经网络各层节点。
      private double[][] _layerError;//神经网络各节点误差。
      private List<List<List<double>>> _layerWeight;//各层节点权重。
      private List<List<List<double>>> _layerWeightDelta;//各层节点权重动量。
      private double _momentumCoefficient;//动量系数。
      private double _learningCoefficient;//学习系数。
      private double[] _sampleData;//样本数据。
      /// <summary>
      /// 构造函数，初始化当前BP神经网络的基本配置。
      /// </summary>
      /// <param name="layerNum">用于设置神经网络的层数和每层节点数。</param>
      /// <param name="learningCoefficient">指定的学习步长或者学习系数。</param>
      /// <param name="momentumCoefficient">指定的动量系数。</param>
      public BPDeepLearningNetwork(int[] layerNum, double learningCoefficient, double momentumCoefficient)
      {
         Random random = new Random();
         _momentumCoefficient = momentumCoefficient;
         _learningCoefficient = learningCoefficient;
         _layer = new double[layerNum.Length][];
         _layerError = new double[layerNum.Length][];
         _layerWeight = new List<List<List<double>>>(layerNum.Length);
         _layerWeightDelta = new List<List<List<double>>>(layerNum.Length);
         for (int l = 0; l < layerNum.Length; l++)
         {
            _layer[l] = new double[layerNum[l]];
            _layerError[l] = new double[layerNum[l]];
            if (l + 1 < layerNum.Length)
            {
               _layerWeight[l] = new List<List<double>>(layerNum.Length);
               _layerWeightDelta[l] = new List<List<double>>(layerNum.Length);
               for (int index = 0; index < _layerWeight[l][index].Count; index++)
               {
                  _layerWeight[l][index] = new List<double>(layerNum[l + 1]);
                  _layerWeightDelta[l][index] = new List<double>(layerNum[l + 1]);
               }
               for (int j = 0; j < layerNum[l] + 1; j++) for (int i = 0; i < layerNum[l + 1]; i++) _layerWeight[l][j][i] = random.NextDouble();
            }
         }
      }
      /// <summary>
      /// 获取或设置当前实例的样本数据。
      /// </summary>
      public double[] SampleData { get => _sampleData; set => _sampleData = value; }
      /// <summary>
      /// 获取或设置当前实例的神经网络各层节点。
      /// </summary>
      public double[][] Layer { get => _layer; set => _layer = value; }
      /// <summary>
      /// 获取或设置当前实例的动量系数。
      /// </summary>
      public double MomentumCoefficient { get => _momentumCoefficient; set => _momentumCoefficient = value; }
      /// <summary>
      /// 获取或设置当前实例的学习系数。
      /// </summary>
      public double LearningCoefficient { get => _learningCoefficient; set => _learningCoefficient = value; }
      /// <summary>
      /// 获取或设置当前实例的神经网络各节点误差。
      /// </summary>
      private double[][] LayerError { get => _layerError; set => _layerError = value; }
      /// <summary>
      /// 获取或设置当前实例的神经网络各层节点权重。
      /// </summary>
      private List<List<List<double>>> LayerWeight { get => _layerWeight; set => _layerWeight = value; }
      /// <summary>
      /// 获取或设置当前实例的神经网络各层节点权重动量。
      /// </summary>
      private List<List<List<double>>> LayerWeightDelta { get => _layerWeightDelta; set => _layerWeightDelta = value; }
      /// <summary>
      /// 逐层向前计算输出。
      /// </summary>
      /// <returns>该操作会返回一个样本数据的检验结果。</returns>
      public double[] ComputeOut()
      {
         for (int l = 1; l < Layer.Length; l++)
         {
            for (int j = 0; j < Layer[l].Length; j++)
            {
               double z = LayerWeight[l - 1][Layer[l - 1].Length][j];
               for (int i = 0; i < _layer[l - 1].Length; i++)
               {
                  _layer[l - 1][i] = l == 1 ? SampleData[i] : Layer[l - 1][i];
                  z += LayerWeight[l - 1][i][j] * Layer[l - 1][i];
               }
               Layer[l][j] = 1 / (1 + Math.Exp(-z));
            }
         }
         return Layer[Layer.Length - 1];
      }
      /// <summary>
      /// 逐层反向计算误差并修改权重。
      /// </summary>
      /// <param name="target">指定的目标数据。</param>
      public void UpdateWeight(double[] target)
      {
         int l = Layer.Length - 1;
         for (int j = 0; j < LayerError[l].Length; j++) LayerError[l][j] = Layer[l][j] * (1 - Layer[l][j]) * (target[j] - Layer[l][j]);
         while (l-- > 0)
         {
            for (int j = 0; j < LayerError[l].Length; j++)
            {
               double z = 0.0;
               for (int i = 0; i < LayerError[l + 1].Length; i++)
               {
                  z = z + l > 0 ? LayerError[l + 1][i] * LayerWeight[l][j][i] : 0;
                  LayerWeightDelta[l][j][i] = MomentumCoefficient * LayerWeightDelta[l][j][i] + LearningCoefficient * LayerError[l + 1][i] * Layer[l][j];//隐含层动量调整  
                  LayerWeight[l][j][i] += LayerWeightDelta[l][j][i];//隐含层权重调整  
                  if (j == _layerError[l].Length - 1)
                  {
                     LayerWeightDelta[l][j + 1][i] = MomentumCoefficient * LayerWeightDelta[l][j + 1][i] + LearningCoefficient * LayerError[l + 1][i];//截距动量调整  
                     LayerWeight[l][j + 1][i] += LayerWeightDelta[l][j + 1][i];//截距权重调整  
                  }
               }
               LayerError[l][j] = z * Layer[l][j] * (1 - Layer[l][j]);//记录误差  
            }
         }
      }
      /// <summary>
      /// 开始训练当前的BP深度学习神经网络。
      /// </summary>
      /// <param name="target">指定的目标数据。</param>
      public void Train(double[] target)
      {
         double[] output = ComputeOut();
         UpdateWeight(target);
      }
   }
}
