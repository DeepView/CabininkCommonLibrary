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
         int tempValue;
         for (int i = 0; i < sortedList.Count; ++i)
         {
            for (int j = 0; j + 1 < sortedList.Count; ++j)
            {
               if (sortedList[j] > sortedList[j + 1])
               {
                  tempValue = sortedList[j];
                  sortedList[j] = sortedList[j + 1];
                  sortedList[j + 1] = tempValue;
               }
            }
         }
      }
      /// <summary>
      /// 插入排序法。
      /// </summary>
      /// <param name="sortedList">需要被排序的32位整型数据。</param>
      public static void InsertionSort(ref List<int> sortedList)
      {
         int currentLocation, currentValue, insertionLocation;
         sortedList.Insert(0, 0);
         for (int location = 1; location < sortedList.Count; ++location)
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
      /// <summary>
      /// 选择排序法。
      /// </summary>
      /// <param name="sortedList">需要被排序的32位整型数据。</param>
      public static void SelectionSort(ref List<int> sortedList)
      {
         for (int i = 0; i < sortedList.Count; i++)
         {
            int index = 1;
            for (int j = i + 1; j < sortedList.Count; j++)
            {
               if (sortedList[j] < sortedList[index]) index = j;
            }
            if (index == i) continue;
            else
            {
               int tempValue = sortedList[index];
               sortedList[index] = sortedList[i];
               sortedList[i] = tempValue;
            }
         }
      }
      /// <summary>
      /// 希尔排序法。
      /// </summary>
      /// <param name="sortedList">需要被排序的32位整型数据。</param>
      public static void ShellSort(ref List<int> sortedList)
      {
         int increasement = sortedList.Count;
         do
         {
            increasement = increasement / 3 + 1;
            for(int i= 0; i < increasement; i++)
            {
               for (int j = i + increasement; j < sortedList.Count; j += increasement)
               {
                  if (sortedList[j] < sortedList[j - increasement])
                  {
                     int temp = sortedList[j];
                     int subLoopIncrement = 0;
                     for (int k = j - increasement; k >= 0 && temp < sortedList[k]; k -= increasement)
                     {
                        subLoopIncrement = k;
                        sortedList[k + increasement] = sortedList[k];
                     }
                     sortedList[subLoopIncrement + increasement] = temp;
                  }
               }
            }
         } while (increasement > 1);
      }
   }
}
