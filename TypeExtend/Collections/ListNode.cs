using System;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Collections
{
   /// <summary>
   /// 链表等结构的节点描述类。
   /// </summary>
   /// <typeparam name="T">用于表示节点所包含元素的数据类型。</typeparam>
   [Serializable]
   [ComVisible(true)]
   public class ListNode<T>
   {
      private T _element;//当前节点存储的数据。
      private ListNode<T> _next;//一个ListNode<T>实例，指向下一个节点，即后继。
      /// <summary>
      /// 构造函数，创建一个内容和后继为空的节点。
      /// </summary>
      public ListNode()
      {
         _element = default(T);
         _next = null;
      }
      /// <summary>
      /// 构造函数，创建一个存储指定元素和后继的节点。
      /// </summary>
      /// <param name="element">节点需要存储的数据。</param>
      /// <param name="next">一个ListNode&lt;T&gt;实例，指向下一个节点。</param>
      public ListNode(T element, ListNode<T> next)
      {
         _element = element;
         _next = next;
      }
      /// <summary>
      /// 获取或设置当前实例的节点数据。
      /// </summary>
      public T Element { get => _element; set => _element = value; }
      /// <summary>
      /// 获取或设置当前实例的后继。
      /// </summary>
      public ListNode<T> Next { get => _next; set => _next = value; }
      /// <summary>
      /// 将当前节点的数据初始化。
      /// </summary>
      public void ElementToDefault() => Element = default(T);
      /// <summary>
      /// 将当前节点的后继指向NULL。
      /// </summary>
      public void NextToNull() => Next = null;
      /// <summary>
      /// 将指针移动到下一个后继，即当前元素后继的后继。
      /// </summary>
      public void BackwardsPointer() => Next = Next.Next;
   }
}
