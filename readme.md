项目说明(Project Description)
===========================
> - ***注意：以下说明文本的英语翻译均来自机器翻译。***
> - ***Note: the following translations of the text are from Machine Translation.***

关于这个项目（About This Project）
------------------------------
这是一个基于Microsoft.Net的应用程序扩展集，它可以用于快速开发一些Windows桌面应用程序，是一个非常不错的工具！
（This is a Microsoft .Net Application extend library, it can fast used developed some Windows Desktop Application(Win32 Application), and this is a very good toolbox! ）

标签（Tags）
----------
> - 微软.NET应用程序框架（.NET Framework）
> - 自封装应用程序扩展库（Self Encapsulation Library）
> - Win32应用程序集（Win32 Assembly）
> - 已封装的编程开发接口（Encapsulated Programming Interface）

命名空间概述（Summary of Namespace）
--------------------------------
以下命名空间概述信息均参考自我上传至VSTS中的项目master分支。（The following namespace overview information refers to the project Master branch that uploads itself to VSTS.）

命名空间（Namespace）                     |概述（Summary）
----------------------------------------|-----------------------------------------------------------------------------------------
Cabinink                                |应用程序集的根命名空间。（The assembly's root namespace.）
Cabinink.Algorithm                      |与算法相关的命名空间。（The namespace associated with the algorithm.）
Cabinink.Algorithm.IntelligentLearning  |人工智能与深度学习相关的命名空间。（AI is associated with deep learning namespaces.）
Cabinink.DataTreatment                  |一些基本的数据操作。（Some basic operation of the data.）
Cabinink.DataTreatment.Database         |包含一些数据库系统操作的命名空间。（A namespace that contains some database system operations.）
Cabinink.DataTreatment.Database.Extend  |数据库操作调用扩展。（The database operation calls the extension.）
Cabinink.DataTreatment.DocumentData     |文档数据操作。（About the operation of the document data.）
Cabinink.DataTreatment.ORMapping        |ORM框架的基础性实现。（The underlying implementation of the ORM framework.）
Cabinink.DataTreatment.WebData          |实现基本的网络数据读取操作。（Implements basic network data read operations.）
Cabinink.Devices                        |本地设备管理以及操作。（Local device management and operations.）
Cabinink.IOSystem                       |本地文件输入输出操作。（Local file I/O operation.）
Cabinink.IOSystem.Security              |文件安全相关类所在的命名空间。（The namespace in which the file security class resides.）
Cabinink.Network                        |网络操作以及相关功能实现。（Network operations and related functions are implemented.）
Cabinink.TypeExtend                     |CLR基础类型扩展。（CLR Foundation type Extension.）
Cabinink.TypeExtend.Collections         |基本的集合类型扩展与增强。（Basic collection type extensions and enhancements.）
Cabinink.TypeExtend.Geometry2D          |基础平面几何相关操作以及描述。（Basic plane geometry related operations and descriptions.）
Cabinink.Windows                        |与Windows操作系统相关的操作。（Operations related to the Windows OS.）
Cabinink.Windows.Applications           |Windows应用程序的相关操作以及特征实现。（The operations and feature implementations of Windows apps.）
Cabinink.Windows.Energy                 |能源操作相关的命名空间。(The namespace associated with the energy operation.)
Cabinink.Windows.Privileges             |Windows系统权限相关操作。(Actions related to Windows system permissions.)

编译时注意事项（Things to Note When Compiling）
-------------------------------------------

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

7. 如果您在您的项目中访问了Cabinink.Devices命名空间下的SoundPlayer和VideoPlayer等引用了DirectX SDK的类，那么你的应用程序可能无法正确运行或者无法通过调试，因为访问这些功能将会触发一些代码安全机制，因此您需要将您的项目的app.config文件中的&lt;startup&gt;标记添加**useLegacyV2RuntimeActivationPolicy**属性：（If you visited Cabinink in your project. The Devices under the namespace SoundPlayer VideoPlayer and quoting the DirectX SDK classes, then your application may not be able to run correctly or not through debugging, because access to these functions will trigger some code security mechanism, so you need to put your project in the app. In the config file &lt;startup&gt; tag to add **useLegacyV2RuntimeActivationPolicy** properties）
```xml
<startup useLegacyV2RuntimeActivationPolicy="true">
    <!--This is your app's configure code.-->
</startup>
```