using System;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend
{
   [Serializable]
   [ComVisible(true)]
   [DebuggerDisplay("ExArray = Length:{Length};Count:{Count}")]
   public class ExArray<T>
   {
      private List<T> _list;
      public ExArray() => _list = new List<T>(10);
      public ExArray(int capacity) => _list = new List<T>(capacity);
      public ExArray(T[] array)
      {
         _list = new List<T>();
         foreach (T item in array) _list.Add(item);
      }
      public ExArray(List<T> list) => _list = list;
   }
}
