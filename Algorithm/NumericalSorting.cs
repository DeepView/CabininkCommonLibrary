using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.Algorithm
{
   /// <summary>
   /// 数值排序类，实现了几种常用的数值排序方法。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class NumericalSorting
   {
      /// <summary>
      /// 冒泡排序法。
      /// </summary>
      /// <param name="sortedList">需要被排序的32位整型数据。</param>
      public static void BubbleSort(ref List<int> sortedList)
      {
         int temp, count = sortedList.Count;
         for (int i = 0; i < count; ++i)
         {
            for (int j = 0; j + 1 < count; ++j)
            {
               if (sortedList[j] > sortedList[j + 1])
               {
                  temp = sortedList[j];
                  sortedList[j] = sortedList[j + 1];
                  sortedList[j + 1] = temp;
               }
            }
         }
      }
      /// <summary>
      /// 插入排序法。
      /// </summary>
      /// <param name="sortedList">需要被排序的32位整型数据。</param>
      /// <param name="token">用于传播有关应取消操作的通知的令牌。</param>
      public static void InsertionSort(ref List<int> sortedList, CancellationToken token)
      {
         int currentLocation, currentValue, insertionLocation, count = sortedList.Count;
         sortedList.Insert(0, 0);
         for (int location = 1; location < count + 1; ++location)
         {
            currentLocation = location;
            insertionLocation = location - 1;
            currentValue = sortedList[currentLocation];
            while (sortedList[insertionLocation] > currentValue)
            {
               sortedList[currentLocation] = sortedList[insertionLocation];
               --currentLocation;
               --insertionLocation;
            }
            sortedList[currentLocation] = currentValue;
         }
         sortedList.Remove(0);
      }
      /// <summary>
      /// 支点排序法（即快速排序法）。
      /// </summary>
      /// <param name="sortedList">需要被排序的32位整型数据。</param>
      /// <param name="begin">排序的初始边界。</param>
      /// <param name="last">排序的终止边界。</param>
      /// <param name="pivot">指定的一个支点值。</param>
      public static void PivotSort(ref List<int> sortedList, int begin, int last, int pivot)
      {
         if (begin < last)
         {
            pivot = sortedList[last];
            int location = begin, bound = last;
            while (location < bound)
            {
               if (sortedList[location] < pivot) ++location;
               else
               {
                  sortedList[bound] = sortedList[location];
                  sortedList[location] = sortedList[bound - 1];
                  --bound;
               }
            }
            sortedList[bound] = pivot;
            PivotSort(ref sortedList, begin, bound - 1, pivot);
            PivotSort(ref sortedList, bound + 1, last, pivot);
         }
      }
   }
}
