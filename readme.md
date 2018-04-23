> - ***ע�⣺����˵���ı���Ӣ�﷭������Ի������롣***
> - ***Note: the following translations of the text are from Machine Translation.***
1. ����ڱ���Դ���벢����Щ������Чʱ���������Ҫ��Unmanaged Code DependencyĿ¼�µ����е�������������ӵ����������У���If you compile the source code and want these code to take effect, you may need to add all third party dependencies in the Unmanaged Code Dependency directory to the code reference!��
2. ���´�����������Ҫ�ֶ����Ƶ���Ŀ������Ŀ���ļ����У�������ʹ��ĳЩ��ع��ܣ����ܽ��ᵼ��һЩδ֪���쳣����This list contains code dependencies that need to be manually copied to the project's generated target folders, otherwise some of the related functions may lead to some unknown exceptions:��
> - Unmanaged Code Dependency\SQLite.Interop.dll
> - Unmanaged Code Dependency\Microsoft.WindowsAPICodePack.DirectX.dll

����˵�������������Ŀ������Ŀ���ļ����ǣ�For example, if the target folder of your project is��
> C:\CodeSolution\CSharpProjects\StudentManagementPlatform\StudentManagementPlatform\bin\Release

��ô������Ҫ���������ᵽ���ļ��ֶ����Ƶ����Ŀ¼��ȥ��������ܻ����һЩ�쳣������˵SQL�﷨������쳣����Then you need to manually copy the files mentioned above to this directory, otherwise there may be some exceptions, such as SQL syntax errors, such as exceptions.��

3. ���齫Cabinink Common Library���е���������������Ŀ�н������ã�SQLite.Interop.dll��Microsoft.WindowsAPICodePack.DirectX.dll��Ҫ�ֶ���ӵ���Ŀ���Ŀ¼�����������Ա�֤����ʹ��ĳЩ���ܲ�������������쳣��������Ƶ���ŵȣ����ر�����DirectX��ص�������Ͼ�Ŀǰ����汾��DirectX�����������µ���������It is suggested that all of Cabinink Common Library's dependencies be referenced in your project (SQLite.Interop.dll and Microsoft.WindowsAPICodePack.DirectX.dll need to be added manually to the project output directory) so that you can ensure that you do not have any other exceptions (such as video playback, etc.), especially with DirectX. Lai yuan, after all, this version has increased the following dependency in DirectX:��
> - Microsoft.DirectX.Direct3D.dll
> - Microsoft.DirectX.AudioVideoPlayback.dll

��������Щ��������ҪMicrosoft DirectX SDK��֧�֣�������ļ����û�а�װMicrosoft DirectX SDK����[�������](https://www.microsoft.com/en-us/download/details.aspx?id=6812)�������أ�����������������޷������������ӣ����ֶ�������������ǰ���������أ���These dependencies require Microsoft DirectX SDK support, if your computer does not install Microsoft DirectX SDK, please [click here](https://www.microsoft.com/en-us/download/details.aspx?id=6812) to download, if your web browser cannot click the hyperlink, please manually copy the following link to download:��
> https://www.microsoft.com/en-us/download/details.aspx?id=6812

4. ���Ԫ��������ִ��������������쳣����������Ϊ��Ŀ�ļ���δ����System.ValueTuple.dll��������Ϣ�������������״����ֻ��Ҫ��*Unmanaged Code Dependency\System.ValueTuple.dll*��ӵ����ü��ɣ�����������ʧ�ܣ��볢�����²�����
��Visual Studio�д򿪳�����������̨��Package Manager Console����
�ڴ򿪵Ŀ���ִ̨�������If there is a code error or other exception in a tuple operation, it may be because the reference information of the System.ValueTuple.dll is not included in the project file. If this is the case, only the Unmanaged Code Dependency\System.ValueTuple.dll is added to the reference. If the additional reference fails, please try the following operation: Open the package management console (Package Manager Console) in Visual Studio. Execute commands at the open console:��
```powershell
Install-Package System.ValueTuple -Version 4.3.1
```
Ϊʲô��Ҫ����������Ϊ��C# 7.0�У�������һ���µķ�������ValueTuple<T>�����������ص����⣬�������λ��һ��������dll��System.ValueTuple���У����Ծ���Ҫͨ��nuget����������������뵽�㵱ǰ����Ŀ�С���Why do you need to do this? Because in C# 7, a new generic type, ValueTuple, is introduced to solve generics related problems, which are in a separate DLL (System.ValueTuple), so it is necessary to introduce this dependency into your current project by nuget.��

5. ������汾��ʼһֱ����ʽ��֮ǰ���������ṩ�����ĵ�����������ĳЩ�����ʹ�ã������ڶ���������鿴���顣��No help documents are provided until this version has been made until the official version. If it is not clear about the use of some code, you can see the details in the object browser.��

6. �����ʹ�õ������ڰ汾��Visual Studio����ôĳЩ�ܹ���Visual Studio 2017��û���﷨����Ĵ����񣬽���������ʹ�õİ汾��ʧЧ������˵������Visual Studio 2017�У�һ�����д�������Կ��������µĸ�ʽ��д����If you are using an earlier version of Visual Studio, then some code styles that do not have a grammatical error in Visual Studio 2017 will fail in the version you use. For example, in Visual Studio 2017, the properties of a single line code can be written in the following format:��
```csharp
public Video DirectXVideoInstance { get => _video; set => _video = value; }
```
��ô�����ڰ汾��Visual Studio�����޸�Ϊ����Then, in the earlier version of Visual Studio, please change it to:��
```csharp
public Video DirectXVideoInstance { get { return _video; } set { _video = value; } }
```
���⣬ʵ�����Եĸ�ֵ����Visual Studio 2017�п���������ĸ�ʽȥ��������In addition, the assignment of instance attributes can be described in the following format in Visual Studio 2017:��
```csharp
SDevicesMode devM = new SDevicesMode
{
   Size = (short)Marshal.SizeOf(typeof(SDevicesMode))
};
```
��ô�������������ڰ汾��Visual Studio������Ҫ�޸�Ϊ����The above code needs to be modified in the earlier version of Visual Studio:��
```csharp
SDevicesMode devM = new SDevicesMode();
devM.Size = (short)Marshal.SizeOf(typeof(SDevicesMode));
```
���ϵ����﷨��ʽ��������C# 7.0���������Ҫ�˽����C# 7.0����Visual Studio 2017�������ԣ���[�������](https://www.cnblogs.com/cncc/p/7698543.html)������ϸ�˽⣬�����������������޷����ʸղŵ����ӣ����ֶ�������������ǰ�����з��ʣ���All of the above new syntax formats come from C# 7. If you need to know more about the new features of C# 7 or Visual Studio 2017, please [click here](https://www.cnblogs.com/cncc/p/7698543.html) for a detailed understanding. If your web browser cannot access the link, please copy the following link manually for access:��
> https://www.cnblogs.com/cncc/p/7698543.html