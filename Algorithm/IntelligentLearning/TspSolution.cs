using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm.IntelligentLearning
{
   /// <summary>
   /// 旅行商问题算法解决方案类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class TspSolution
   {
      private int _importanceOfInformation;//对信息量的重视程度。
      private int _importanceOfElicInformation;//启发式信息的受重视程度。
      private double _pheromoneVolatilizationRate;//信息素的挥发速度。
      private double[,] _cityDistances;//城市距离矩阵。
      private double[,] _pheromones;//信息素矩阵。
      private Queue<int> _nextCityList;//用于存放下一步可行城市。
      private Queue<int> _closedList;//用于存放已经访问过的城市。
      private Queue<int> _optimalPath;//储存较好的路径。
      private int _proTime = 0;//指定的PROTIME。
      /// <summary>
      /// 构造函数，初始化当前的TSP算法解决方案实例。
      /// </summary>
      /// <param name="cityDistances">城市距离矩阵。</param>
      /// <param name="pheromoneVolatilizationRate"> 信息素的挥发速度</param>
      /// <param name="importanceOfInformation">对信息量的重视程度。</param>
      /// <param name="importanceOfElicInformation">启发式信息的受重视程度。</param>
      public TspSolution(double[,] cityDistances, double pheromoneVolatilizationRate, int importanceOfInformation, int importanceOfElicInformation)
      {
         _nextCityList = new Queue<int>();
         _closedList = new Queue<int>();
         _optimalPath = new Queue<int>();
         _importanceOfInformation = importanceOfInformation;
         _importanceOfElicInformation = importanceOfElicInformation;
         _pheromoneVolatilizationRate = pheromoneVolatilizationRate;
         int temp = Convert.ToInt32(Math.Sqrt(cityDistances.Length));
         _cityDistances = new double[temp, temp];
         _pheromones = new double[temp, temp];
         for (int i = 0; i < temp; i++) for (int j = 0; j < temp; j++) _cityDistances[i, j] = cityDistances[i, j];
         for (int i = 0; i < temp; i++) for (int j = 0; j < temp; j++) if (i != j) _pheromones[i, j] = (double)1 / (temp * temp - temp);
      }
      /// <summary>
      /// 获取当前实例所表示的TSP解决方案的最佳路径。
      /// </summary>
      public Queue<int> OptimalPath => _optimalPath;
      /// <summary>
      /// 获取或设置当前实例所表示的TSP解决方案的城市距离矩阵。
      /// </summary>
      public double[,] Pheromones { get => _pheromones; set => _pheromones = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的TSP解决方案的信息素矩阵。
      /// </summary>
      public double[,] CityDistances { get => _cityDistances; set => _cityDistances = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的TSP解决方案的信息量的重视程度
      /// </summary>
      public int ImportanceOfInformation { get => _importanceOfInformation; set => _importanceOfInformation = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的TSP解决方案的启发式信息量的重视程度
      /// </summary>
      public int ImportanceOfElicInformation { get => _importanceOfElicInformation; set => _importanceOfElicInformation = value; }
      /// <summary>
      /// 获取或设置当前实例所表示的TSP解决方案的信息素的挥发速度。
      /// </summary>
      public double PheromoneVolatilizationRate { get => _pheromoneVolatilizationRate; set => _pheromoneVolatilizationRate = value; }
      /// <summary>
      /// 通过指定的链表计算出其对应的总路径。
      /// </summary>
      /// <param name="closedList">指定的链表。</param>
      /// <returns>该操作会返回一个通过量表计算出的其对应的总路径。</returns>
      public double GetWeight(Queue<int> closedList)
      {
         lock (this)
         {
            double sum = 0;
            int[] temp_Array = new int[closedList.Count];
            temp_Array = closedList.ToArray();
            for (int i = 0; i < Convert.ToInt32(temp_Array.Length) - 1; i++)
            {
               sum = sum + CityDistances[temp_Array[i], temp_Array[i + 1]];
            }
            sum = sum + CityDistances[temp_Array[temp_Array.Length - 1], temp_Array[0]];
            return sum;
         }
      }
      /// <summary>
      /// 更新这个解决方案的最佳路径，更新之后的路径可以在OptimalPath属性中获取。
      /// </summary>
      public void UpdateOptimalPath()
      {
         _optimalPath.Clear();
         for (int i = 0; i < 4; i++)
         {
            for (int j = 0; j < Convert.ToInt32(Math.Sqrt(CityDistances.Length)); j++)
            {
               _nextCityList.Enqueue(0);
               _closedList.Clear();
               while (_nextCityList.Count != 0 && _closedList.Count != Convert.ToInt32(Math.Sqrt(CityDistances.Length)))
               {
                  int temp = _nextCityList.Dequeue();
                  _proTime = temp;
                  _closedList.Enqueue(temp);
                  if (_nextCityList.Count == 0 && _closedList.Count == Convert.ToInt32(Math.Sqrt(CityDistances.Length)))
                  {
                     if (_optimalPath.Count == 0)
                     {
                        int[] temp_Array = new int[Convert.ToInt32(Math.Sqrt(CityDistances.Length))];
                        temp_Array = _closedList.ToArray();
                        for (int k = 0; k < Convert.ToInt32(Math.Sqrt(CityDistances.Length)); k++)
                        {
                           _optimalPath.Enqueue(temp_Array[k]);
                        }
                     }
                     if (GetWeight(_optimalPath) > GetWeight(_closedList))
                     {
                        _optimalPath.Clear();
                        int[] temp_Array = new int[Convert.ToInt32(Math.Sqrt(CityDistances.Length))];
                        temp_Array = _closedList.ToArray();
                        for (int k = 0; k < Convert.ToInt32(Math.Sqrt(CityDistances.Length)); k++)
                        {
                           _optimalPath.Enqueue(temp_Array[k]);
                        }
                     }
                  }
                  GetNextCity();
                  ChoiceRoute();
               }
            }
            ChangePheromones(_optimalPath);
         }
      }
      /// <summary>
      /// 改变信息素矩阵。
      /// </summary>
      /// <param name="closedList">指定的较好的路径。</param>
      private void ChangePheromones(Queue<int> closedList)
      {
         lock (this)
         {
            int[] temp_Array = new int[closedList.Count];
            temp_Array = closedList.ToArray();
            for (int i = 0; i < closedList.Count - 1; i++)
            {
               Pheromones[temp_Array[i], temp_Array[i + 1]] = Pheromones[temp_Array[i], temp_Array[i + 1]] + PheromoneVolatilizationRate / ((1 - PheromoneVolatilizationRate) * Convert.ToInt32(GetWeight(closedList) + 1));
            }
            Pheromones[temp_Array[temp_Array.Length - 1], temp_Array[0]] = Pheromones[temp_Array[temp_Array.Length - 1], temp_Array[0]] + PheromoneVolatilizationRate / ((1 - PheromoneVolatilizationRate) * Convert.ToInt32(GetWeight(closedList)));
            for (int i = 0; i < closedList.Count; i++)
            {
               for (int j = 0; j < closedList.Count; j++)
               {
                  Pheromones[i, j] = (1 - PheromoneVolatilizationRate) * Pheromones[i, j];
               }
            }
         }
      }
      /// <summary>
      /// 产生到指定城市后下一个可走城市的集合，并将城市编号加入到nextCityList中，产生的城市不可以已经存在closedList中。
      /// </summary>
      private void GetNextCity()
      {
         _nextCityList.Clear();
         int temp_int = Convert.ToInt32(Math.Sqrt(CityDistances.Length));
         for (int i = 0; i < temp_int; i++)
         {
            if (_closedList.Contains(i) == false)
            {
               _nextCityList.Enqueue(i);
            }
         }
      }
      /// <summary>
      /// 选择应该行走路径。
      /// </summary>
      /// <returns>该操作会返回一个选择的城市。</returns>
      private int ChoiceRoute()
      {
         int index = 0;
         Random random = new Random();
         double random_value = (double)random.NextDouble();
         int[] temp_Array = new int[_nextCityList.Count];
         temp_Array = _nextCityList.ToArray();
         double sum_Message = 0;
         for (int i = 0; i < _nextCityList.Count; i++)
         {
            double eta = 1 / CityDistances[_proTime, temp_Array[i]];
            sum_Message = sum_Message + Math.Pow(Pheromones[_proTime, temp_Array[i]], ImportanceOfInformation) * Math.Pow(eta, ImportanceOfElicInformation);
         }
         double temp = 0;
         for (int j = 0; j < _nextCityList.Count; j++)
         {
            double eta = 1 / CityDistances[_proTime, temp_Array[j]];
            temp = temp + Math.Pow(Pheromones[_proTime, temp_Array[j]], ImportanceOfInformation) * Math.Pow(eta, ImportanceOfElicInformation) / sum_Message;
            if (temp > random_value)
            {
               index = temp_Array[j];
               break;
            }
         }
         _nextCityList.Clear();
         _nextCityList.Enqueue(index);
         return index;
      }
   }
}
