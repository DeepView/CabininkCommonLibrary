using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
namespace Cabinink.Graphics.Direct2D
{
   /// <summary>
   /// Direct2D工厂基础类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   [Obsolete("现用于代码示例，无其他用处，将会在正式版本移除！")]
   public class Direct2DFactoryBase
   {
      private D2DFactory _d2dFactory;//D2D工厂对象，ID2D1Factory接口。
      private RenderTarget _renderTarget;//渲染窗口，ID2D1HwndRenderTarget接口。
      /// <summary>
      /// 构造函数，创建一个D2D工厂对象。
      /// </summary>
      public Direct2DFactoryBase()
      {
         _d2dFactory = D2DFactory.CreateFactory();
      }
      /// <summary>
      /// 从指定的控件里面创建设备资源。
      /// </summary>
      /// <param name="target">用于创建设备资源的控件。</param>
      public void CreateDeviceResource(Control target)
      {
         if (_renderTarget == null)
         {
            _renderTarget = _d2dFactory.CreateHwndRenderTarget(
                     new RenderTargetProperties(),
                     new HwndRenderTargetProperties(
                              target.Handle,
                              new SizeU((uint)target.Width, (uint)target.Height),
                              PresentOptions.None
                     )
            );
         }
      }
   }
}
