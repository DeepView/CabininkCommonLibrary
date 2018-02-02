using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Collections
{
   /// <summary>
   /// 双向链表表示类。
   /// </summary>
   /// <typeparam name="T">用于表示链表所存放数据的数据类型。</typeparam>
   [Serializable]
   [ComVisible(true)]
   [DebuggerDisplay("BiDirectionalLinkedList={Count:Count}")]
   public class BiDirectionalLinkedList<T>
   {
      private ListNode<T> _headNode;//链表的表头节点。
      /// <summary>
      /// 构造函数，创建一个链表表头为空的双向链表。
      /// </summary>
      public BiDirectionalLinkedList() => _headNode = null;
      /// <summary>
      /// 构造函数，创建一个通过指定数组填充的双向链表。
      /// </summary>
      /// <param name="elements">用于填充数据的数组。</param>
      public BiDirectionalLinkedList(T[] elements)
      {
         _headNode = null;
         AddRange(elements);
      }
      /// <summary>
      /// 构造函数，创建一个通过指定List&lt;T&gt;列表实例填充的双向链表。
      /// </summary>
      /// <param name="elements">用于填充数据的List&lt;T&gt;列表实例。</param>
      public BiDirectionalLinkedList(List<T> elements)
      {
         _headNode = null;
         AddRange(elements.ToArray());
      }
      /// <summary>
      /// 获取当前实例所表示的双向链表所拥有元素的数量。
      /// </summary>
      public int Count
      {
         get
         {
            ListNode<T> node = _headNode;
            int count = 0;
            while (node != null)
            {
               count++;
               node = node.Next;
            }
            return count;
         }
      }
      /// <summary>
      /// 获取当前实例所表示的双向链表指定索引所对应的节点。
      /// </summary>
      /// <param name="index">指定的索引。</param>
      public ListNode<T> this[int index]
      {
         get
         {
            ListNode<T> node = _headNode;
            int counter = 0;
            if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "索引超出检索范围！");
            else
            {
               while (counter < index)
               {
                  counter++;
                  node = node.Next;
               }
            }
            return node;
         }
      }
      /// <summary>
      /// 在指定索引所对应的节点插入一个元素。
      /// </summary>
      /// <param name="index">指定的索引。</param>
      /// <param name="element">需要插入的元素。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool Insert(int index, T element)
      {
         int countBeforeIns = Count;
         int counter = 0;
         ListNode<T> node = _headNode;
         ListNode<T> inserted = new ListNode<T>(element, null);
         if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "索引超出检索范围！");
         else
         {
            if (index == 0)
            {
               inserted.Next = _headNode;
               _headNode = inserted;
            }
            while (counter < index - 1)
            {
               counter++;
               node = node.Next;
            }
            inserted.Next = node.Next;
            node.Next = inserted;
         }
         return countBeforeIns < Count ? true : false;
      }
      /// <summary>
      /// 从链表中移除指定索引所对应的节点。
      /// </summary>
      /// <param name="index">需要被移除的节点所对应的索引。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool Remove(int index)
      {
         int countBeforeRemove = Count;
         int counter = 0;
         ListNode<T> node = _headNode;
         if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "索引超出检索范围！");
         else
         {
            if (index == 0) _headNode.BackwardsPointer();
            while (counter < index - 1)
            {
               counter++;
               node = node.Next;
            }
            node.BackwardsPointer();
         }
         return countBeforeRemove > Count ? true : false;
      }
      /// <summary>
      /// 从链表中移除指定元素所对应的节点，这个操作只会移除第一个匹配到的节点。
      /// </summary>
      /// <param name="element">用于匹配并移除节点的元素。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool Remove(T element)
      {
         int countBeforeRemove = Count;
         while (_headNode.Element.Equals(element)) _headNode = _headNode.Next;
         ListNode<T> node = _headNode;
         while (node.Next.Next != null)
         {
            if (node.Next.Element.Equals(element))
            {
               node.BackwardsPointer();
               continue;
            }
            node = node.Next;
         }
         if (node.Next.Element.Equals(element)) node.NextToNull();
         return countBeforeRemove > Count ? true : false;
      }
      /// <summary>
      /// 在链表的尾部添加新的节点。
      /// </summary>
      /// <param name="element">需要在链表尾部添加的新节点所包含的元素。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool Add(T element)
      {
         int countBeforeAdd = Count;
         ListNode<T> node = new ListNode<T>();
         ListNode<T> inserted = new ListNode<T>(element, null);
         if (_headNode == null) _headNode = inserted;
         node = _headNode;
         while (node.Next != null) node = node.Next;
         node.Next = inserted;
         return countBeforeAdd < Count ? true : false;
      }
      /// <summary>
      /// 在链表尾部批量添加新的节点。
      /// </summary>
      /// <param name="elements">需要在链表尾部添加的新节点所包含的元素数组。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool AddRange(T[] elements)
      {
         int countBeforeAdd = Count;
         for (int i = 0; i < elements.Length; i++) Add(elements[i]);
         return countBeforeAdd < Count ? true : false;
      }
      /// <summary>
      /// 获取指定元素所对应节点的第一个索引。
      /// </summary>
      /// <param name="element">进行节点匹配的元素。</param>
      /// <returns>如果操作成功，则返回这个元素所对应节点的索引，否则返回-1。</returns>
      public int FirstIndexOf(T element)
      {
         ListNode<T> node = _headNode;
         int counter = 0;
         while (node.Next != null)
         {
            if (node.Element.Equals(element)) return counter;
            counter++;
            node = node.Next;
         }
         if (!node.Element.Equals(element)) counter++;
         return counter >= Count ? -1 : counter;
      }
      /// <summary>
      /// 获取指定元素所对应节点的最后一个索引。
      /// </summary>
      /// <param name="element">进行节点匹配的元素。</param>
      /// <returns>如果操作成功，则返回这个元素所对应节点的索引，否则返回-1。</returns>
      public int LastIndexOf(T element)
      {
         ListNode<T> node = _headNode;
         int index = -1;
         int counter = 0;
         while (node.Next != null)
         {
            if (node.Element.Equals(element)) index = counter;
            counter++;
            node = node.Next;
         }
         if (!node.Element.Equals(element)) index = counter;
         return index;
      }
      /// <summary>
      /// 替换指定节点所对应元素的元素值，这个操作会替换所有匹配到的元素的元素值。
      /// </summary>
      /// <param name="replaced">需要被替换的元素。</param>
      /// <param name="element">被替换的元素的新元素值。</param>
      public void Replace(T replaced, T element)
      {
         ListNode<T> node = _headNode;
         while (node.Next != null)
         {
            if (node.Element.Equals(replaced)) node.Element = element;
            node = node.Next;
         }
         if (node.Element.Equals(replaced)) node.Element = element;
      }
      /// <summary>
      /// 将链表的所有节点进行一次反转排序操作。
      /// </summary>
      public void Reverse()
      {
         T[] array = ToArray();
         array.Reverse();
         if (Clear()) AddRange(array);
      }
      /// <summary>
      /// 同时获取指定元素所对应节点的第一个索引和最后一个索引。
      /// </summary>
      /// <param name="element">进行节点匹配的元素。</param>
      /// <returns>如果操作成功，则返回这个元素所对应节点的第一个索引和最后一个索引所构成的元组，如果未匹配到，则会返回(-1,-1)。</returns>
      public (int, int) IndexOf(T element) => (FirstIndexOf(element), LastIndexOf(element));
      /// <summary>
      /// 判断当前的链表是否为空链表。
      /// </summary>
      /// <returns>如果链表为空链表，则返回true，否则返回false。</returns>
      public bool IsEmpty() => _headNode == null ? true : false;
      /// <summary>
      /// 清除当前链表的所有节点。
      /// </summary>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool Clear()
      {
         _headNode = null;
         return Count == 0 ? true : false;
      }
      /// <summary>
      /// 获取当前链表的数组表达形式。
      /// </summary>
      /// <returns>该操作会返回一个当前链表所对应的数组实例。</returns>
      public T[] ToArray()
      {
         T[] array = new T[Count];
         ListNode<T> node = _headNode;
         int index = 0;
         while (node.Next != null)
         {
            array[index++] = node.Element;
            node = node.Next;
         }
         array[Count - 1] = node.Element;
         return array;
      }
      /// <summary>
      /// 获取当前链表的List&lt;T&gt;表达形式。
      /// </summary>
      /// <returns>该操作会返回一个当前链表所对应的List&lt;T&gt;实例。</returns>
      public List<T> ToList() => ToArray().ToList();
   }
}
