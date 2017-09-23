namespace Cabinink.IOSystem
{
   /// <summary>
   /// 对象访问通道开关控制接口。
   /// </summary>
   public interface IObjectAccessSwitch
   {
      /// <summary>
      /// 打开当前定义的对象的通道开关，并占用这个对象防止被外部对象更改。
      /// </summary>
      void Open();
      /// <summary>
      /// 关闭当前定义的对象的通道开关，并释放当前对象的所有内部对象占用，以及解除外部对象通道锁。
      /// </summary>
      void Close();
   }
}
