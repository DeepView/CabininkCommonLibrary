> - ***注意：以下说明文本的英语翻译均来自机器翻译。***
> - ***Note: the following translations of the text are from Machine Translation.***
1. 如果在编译源代码并想这些代码生效时，你可能需要将Unmanaged Code Dependency目录下的所有第三方依赖项添加到代码引用中！（If you compile the source code and want these code to take effect, you may need to add all third party dependencies in the Unmanaged Code Dependency directory to the code reference!）
2. 以下代码依赖项需要手动复制到项目的生成目标文件夹中，否则在使用某些相关功能，可能将会导致一些未知的异常：（This list contains code dependencies that need to be manually copied to the project's generated target folders, otherwise some of the related functions may lead to some unknown exceptions:）
> - Unmanaged Code Dependency\SQLite.Interop.dll
> - Unmanaged Code Dependency\Microsoft.WindowsAPICodePack.DirectX.dll

举例说明，如果您的项目的生成目标文件夹是（For example, if the target folder of your project is）
> C:\CodeSolution\CSharpProjects\StudentManagementPlatform\StudentManagementPlatform\bin\Release

那么你则需要将上面所提到的文件手动复制到这个目录中去，否则可能会出现一些异常，比如说SQL语法错误等异常。（Then you need to manually copy the files mentioned above to this directory, otherwise there may be some exceptions, such as SQL syntax errors, such as exceptions.）

3. 建议将Cabinink Common Library所有的依赖项在您的项目中进行引用（SQLite.Interop.dll和Microsoft.WindowsAPICodePack.DirectX.dll需要手动添加到项目输出目录），这样可以保证您在使用某些功能不会出现其他的异常（诸如视频播放等），特别是与DirectX相关的依赖项，毕竟目前这个版本在DirectX中增加了以下的依赖：（It is suggested that all of Cabinink Common Library's dependencies be referenced in your project (SQLite.Interop.dll and Microsoft.WindowsAPICodePack.DirectX.dll need to be added manually to the project output directory) so that you can ensure that you do not have any other exceptions (such as video playback, etc.), especially with DirectX. Lai yuan, after all, this version has increased the following dependency in DirectX:）
> - Microsoft.DirectX.Direct3D.dll
> - Microsoft.DirectX.AudioVideoPlayback.dll

上述的这些依赖项需要Microsoft DirectX SDK的支持，如果您的计算机没有安装Microsoft DirectX SDK，请[点击这里](https://www.microsoft.com/en-us/download/details.aspx?id=6812)进行下载，若您的网络浏览器无法点击这个超链接，请手动复制以下链接前往进行下载：（These dependencies require Microsoft DirectX SDK support, if your computer does not install Microsoft DirectX SDK, please [click here](https://www.microsoft.com/en-us/download/details.aspx?id=6812) to download, if your web browser cannot click the hyperlink, please manually copy the following link to download:）
> https://www.microsoft.com/en-us/download/details.aspx?id=6812

4. 如果元组操作出现代码错误或者其他异常，可能是因为项目文件中未包含System.ValueTuple.dll的引用信息，如果出现这种状况，只需要将*Unmanaged Code Dependency\System.ValueTuple.dll*添加到引用即可，如果添加引用失败，请尝试以下操作：
在Visual Studio中打开程序包管理控制台（Package Manager Console）。
在打开的控制台执行命令：（If there is a code error or other exception in a tuple operation, it may be because the reference information of the System.ValueTuple.dll is not included in the project file. If this is the case, only the Unmanaged Code Dependency\System.ValueTuple.dll is added to the reference. If the additional reference fails, please try the following operation: Open the package management console (Package Manager Console) in Visual Studio. Execute commands at the open console:）
```powershell
Install-Package System.ValueTuple -Version 4.3.1
```
为什么需要这样做？因为在C# 7.0中，引入了一个新的泛型类型ValueTuple<T>来解决泛型相关的问题，这个类型位于一个单独的dll（System.ValueTuple）中，所以就需要通过nuget来将这个依赖项引入到你当前的项目中。（Why do you need to do this? Because in C# 7, a new generic type, ValueTuple, is introduced to solve generics related problems, which are in a separate DLL (System.ValueTuple), so it is necessary to introduce this dependency into your current project by nuget.）

5. 从这个版本开始一直到正式版之前，都不会提供帮助文档，如果不清楚某些代码的使用，可以在对象浏览器查看详情。（No help documents are provided until this version has been made until the official version. If it is not clear about the use of some code, you can see the details in the object browser.）

6. 如果你使用的是早期版本的Visual Studio，那么某些能够在Visual Studio 2017中没有语法错误的代码风格，将会在你所使用的版本中失效。举例说明，在Visual Studio 2017中，一个单行代码的属性可以用如下的格式书写：（If you are using an earlier version of Visual Studio, then some code styles that do not have a grammatical error in Visual Studio 2017 will fail in the version you use. For example, in Visual Studio 2017, the properties of a single line code can be written in the following format:）
```csharp
public Video DirectXVideoInstance { get => _video; set => _video = value; }
```
那么在早期版本的Visual Studio中请修改为：（Then, in the earlier version of Visual Studio, please change it to:）
```csharp
public Video DirectXVideoInstance { get { return _video; } set { _video = value; } }
```
另外，实例属性的赋值，在Visual Studio 2017中可以用下面的格式去描述：（In addition, the assignment of instance attributes can be described in the following format in Visual Studio 2017:）
```csharp
SDevicesMode devM = new SDevicesMode
{
   Size = (short)Marshal.SizeOf(typeof(SDevicesMode))
};
```
那么上述代码在早期版本的Visual Studio中则需要修改为：（The above code needs to be modified in the earlier version of Visual Studio:）
```csharp
SDevicesMode devM = new SDevicesMode();
devM.Size = (short)Marshal.SizeOf(typeof(SDevicesMode));
```
以上的新语法格式均来自与C# 7.0，如果你需要了解更多C# 7.0或者Visual Studio 2017的新特性，请[点击这里](https://www.cnblogs.com/cncc/p/7698543.html)进行详细了解，如果您的网络浏览器无法访问刚才的链接，请手动复制以下链接前往进行访问：（All of the above new syntax formats come from C# 7. If you need to know more about the new features of C# 7 or Visual Studio 2017, please [click here](https://www.cnblogs.com/cncc/p/7698543.html) for a detailed understanding. If your web browser cannot access the link, please copy the following link manually for access:）
> https://www.cnblogs.com/cncc/p/7698543.html