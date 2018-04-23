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
   public class BiDirectionalLinkedList<T> : IDisposable
   {
      private ListNode<T> _headNode;//链表的表头节点。
      private bool _disposedValue = false;//检测冗余调用
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
            ListNode<T> node = Head;
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
      /// <exception cref="ArgumentOutOfRangeException">当参数index指定的索引超出范围时，则会抛出这个异常。</exception>
      public ListNode<T> this[int index]
      {
         get
         {
            ListNode<T> node = Head;
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
      /// 获取或设置（set代码对外不可见，因为权限为private）当前链表中的第一个节点。
      /// </summary>
      public ListNode<T> Head { get => _headNode; private set => _headNode = value; }
      /// <summary>
      /// 获取当前链表中的最后一个节点。
      /// </summary>
      public ListNode<T> Tail
      {
         get
         {
            ListNode<T> node = Head;
            while (node.Next != null) node = node.Next;
            return node;
         }
      }
      /// <summary>
      /// 在指定索引所对应的节点插入一个元素。
      /// </summary>
      /// <param name="index">指定的索引。</param>
      /// <param name="element">需要插入的元素。</param>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      /// <exception cref="ArgumentOutOfRangeException">当参数index指定的索引超出范围时，则会抛出这个异常。</exception>
      public bool Insert(int index, T element)
      {
         int countBeforeIns = Count;
         int counter = 0;
         ListNode<T> node = Head;
         ListNode<T> inserted = new ListNode<T>(element);
         if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "索引超出检索范围！");
         else
         {
            if (index == 0)
            {
               inserted.Next = Head;
               Head = inserted;
               return true;
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
      /// <exception cref="ArgumentOutOfRangeException">当参数index指定的索引超出范围时，则会抛出这个异常。</exception>
      public bool Remove(int index)
      {
         int countBeforeRemove = Count;
         int counter = 0;
         ListNode<T> node = Head;
         if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "索引超出检索范围！");
         else
         {
            if (index == 0)
            {
               Head.BackwardsPointer();
               return true;
            }
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
         while (Head.Element.Equals(element)) Head = Head.Next;
         ListNode<T> node = Head;
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
         ListNode<T> inserted = new ListNode<T>(element);
         if (Head == null)
         {
            Head = inserted;
            return true;
         }
         node = Head;
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
         ListNode<T> node = Head;
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
         ListNode<T> node = Head;
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
         ListNode<T> node = Head;
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
         ListNode<T> node = Head;
         ListNode<T> nhNode = Head;
         ListNode<T> tempNode = node;
         node = node.Next;
         nhNode.NextToNull();
         while (node.Next != null)
         {
            tempNode = node;
            node = node.Next;
            tempNode.Next = nhNode;
            nhNode = tempNode;
         }
         tempNode = node;
         tempNode.Next = nhNode;
         nhNode = node;
         Head = nhNode;
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
      public bool IsEmpty() => Head == null ? true : false;
      /// <summary>
      /// 清除当前链表的所有节点。
      /// </summary>
      /// <returns>如果操作成功，则返回true，否则返回false。</returns>
      public bool Clear()
      {
         Head = null;
         return Count == 0 ? true : false;
      }
      /// <summary>
      /// 获取当前链表的数组表达形式。
      /// </summary>
      /// <returns>该操作会返回一个当前链表所对应的数组实例。</returns>
      public T[] ToArray()
      {
         T[] array = new T[Count];
         ListNode<T> node = Head;
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
      /// <summary>
      /// 获取当前类的字符串表达形式。
      /// </summary>
      /// <returns>该操作返回当前类的字符串表达形式，这个字符串是当前类的一段Debug描述文本。</returns>
      public override string ToString() => "BiDirectionalLinkedList:{Count=" + Count + "};";
      #region IDisposable Support
      /// <summary>
      /// 释放该对象引用的所有内存资源。
      /// </summary>
      /// <param name="disposing">用于指示是否释放托管资源。</param>
      protected virtual void Dispose(bool disposing)
      {
         int headNodeMaxGene = GC.GetGeneration(Head);
         if (!_disposedValue)
         {
            if (disposing)
            {
               ListNode<T> node;
               while (null != Head)
               {
                  node = Head;
                  Head = Head.Next;
                  node = null;
               }
               bool condition = GC.CollectionCount(headNodeMaxGene) == 0;
               if (condition) GC.Collect(headNodeMaxGene, GCCollectionMode.Forced, true);
            }
            _disposedValue = true;
         }
      }
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose() => Dispose(true);
      #endregion
   }
}
