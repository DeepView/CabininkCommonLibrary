1. 如果在编译源代码并想这些代码生效时，你可能需要将Unmanaged Code Dependency目录下的所有第三方依赖项添加到代码引用中！
2. 以下代码依赖项需要手动复制到项目的生成目标文件夹中，否则在使用某些相关功能，可能将会导致一些未知的异常：
> - Unmanaged Code Dependency\SQLite.Interop.dll
> - Unmanaged Code Dependency\Microsoft.WindowsAPICodePack.DirectX.dll

举例说明，如果您的项目的生成目标文件夹是
> C:\CodeSolution\CSharpProjects\StudentManagementPlatform\StudentManagementPlatform\bin\Release

那么你则需要将上面所提到的文件手动复制到这个目录中去，否则可能会出现一些异常，比如说SQL语法错误等异常。

3. 建议将Cabinink Common Library所有的依赖项在您的项目中进行引用（SQLite.Interop.dll和Microsoft.WindowsAPICodePack.DirectX.dll需要手动添加到项目输出目录），这样可以保证您在使用某些功能不会出现其他的异常（诸如视频播放等），特别是与DirectX相关的依赖项，毕竟目前这个版本在DirectX中增加了以下的依赖：
> - Microsoft.DirectX.Direct3D.dll
> - Microsoft.DirectX.AudioVideoPlayback.dll

上述的这些依赖项需要Microsoft DirectX SDK的支持，如果您的计算机没有安装Microsoft DirectX SDK，请[点击这里](https://www.microsoft.com/en-us/download/details.aspx?id=6812)进行下载，若您的网络浏览器无法点击这个超链接，请手动复制以下链接前往进行下载：
> https://www.microsoft.com/en-us/download/details.aspx?id=6812

4. 如果元组操作出现代码错误或者其他异常，可能是因为项目文件中未包含System.ValueTuple.dll的引用信息，如果出现这种状况，只需要将*Unmanaged Code Dependency\System.ValueTuple.dll*添加到引用即可，如果添加引用失败，请尝试以下操作：
在Visual Studio中打开程序包管理控制台（Package Manager Console）。
在打开的控制台执行命令：
```powershell
Install-Package System.ValueTuple -Version 4.3.1
```
为什么需要这样做？因为在C# 7.0中，引入了一个新的泛型类型ValueTuple<T>来解决泛型相关的问题，这个类型位于一个单独的dll（System.ValueTuple）中，所以就需要通过nuget来将这个依赖项引入到你当前的项目中。

5. 从这个版本开始一直到正式版之前，都不会提供帮助文档，如果不清楚某些代码的使用，可以在对象浏览器查看详情。

6. 如果你使用的是早期版本的Visual Studio，那么某些能够在Visual Studio 2017中没有语法错误的代码风格，将会在你所使用的版本中失效。举例说明，在Visual Studio 2017中，一个单行代码的属性可以用如下的格式书写：
```csharp
public Video DirectXVideoInstance { get => _video; set => _video = value; }
```
那么在早期版本的Visual Studio中请修改为：
```csharp
public Video DirectXVideoInstance { get { return _video; } set { _video = value; } }
```
另外，实例属性的赋值，在Visual Studio 2017中可以用下面的格式去描述：
```csharp
SDevicesMode devM = new SDevicesMode
{
   Size = (short)Marshal.SizeOf(typeof(SDevicesMode))
};
```
那么上述代码在早期版本的Visual Studio中则需要修改为：
```csharp
SDevicesMode devM = new SDevicesMode();
devM.Size = (short)Marshal.SizeOf(typeof(SDevicesMode));
```
以上的新语法格式均来自与C# 7.0，如果你需要了解更多C# 7.0或者Visual Studio 2017的新特性，请[点击这里](https://www.cnblogs.com/cncc/p/7698543.html)进行详细了解，如果您的网络浏览器无法访问刚才的链接，请手动复制以下链接前往进行访问：
> https://www.cnblogs.com/cncc/p/7698543.html