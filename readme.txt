1. ����ڱ���Դ���벢����Щ������Чʱ���������Ҫ��Unmanaged Code DependencyĿ¼�µ����е�������������ӵ����������У�
2. ���´�����������Ҫ�ֶ����Ƶ���Ŀ������Ŀ���ļ����У�������ʹ��ĳЩ��ع��ܣ����ܽ��ᵼ��һЩδ֪���쳣��
> - Unmanaged Code Dependency\SQLite.Interop.dll
> - Unmanaged Code Dependency\Microsoft.WindowsAPICodePack.DirectX.dll

����˵�������������Ŀ������Ŀ���ļ�����
> C:\CodeSolution\CSharpProjects\StudentManagementPlatform\StudentManagementPlatform\bin\Release

��ô������Ҫ���������ᵽ���ļ��ֶ����Ƶ����Ŀ¼��ȥ��������ܻ����һЩ�쳣������˵SQL�﷨������쳣��

3. ���齫Cabinink Common Library���е���������������Ŀ�н������ã�SQLite.Interop.dll��Microsoft.WindowsAPICodePack.DirectX.dll��Ҫ�ֶ���ӵ���Ŀ���Ŀ¼�����������Ա�֤����ʹ��ĳЩ���ܲ�������������쳣��������Ƶ���ŵȣ����ر�����DirectX��ص�������Ͼ�Ŀǰ����汾��DirectX�����������µ�������
> - Microsoft.DirectX.Direct3D.dll
> - Microsoft.DirectX.AudioVideoPlayback.dll

��������Щ��������ҪMicrosoft DirectX SDK��֧�֣�������ļ����û�а�װMicrosoft DirectX SDK����[�������](https://www.microsoft.com/en-us/download/details.aspx?id=6812)�������أ�����������������޷������������ӣ����ֶ�������������ǰ���������أ�
> https://www.microsoft.com/en-us/download/details.aspx?id=6812

4. ���Ԫ��������ִ��������������쳣����������Ϊ��Ŀ�ļ���δ����System.ValueTuple.dll��������Ϣ�������������״����ֻ��Ҫ��*Unmanaged Code Dependency\System.ValueTuple.dll*��ӵ����ü��ɣ�����������ʧ�ܣ��볢�����²�����
��Visual Studio�д򿪳�����������̨��Package Manager Console����
�ڴ򿪵Ŀ���ִ̨�����
```powershell
Install-Package System.ValueTuple -Version 4.3.1
```
Ϊʲô��Ҫ����������Ϊ��C# 7.0�У�������һ���µķ�������ValueTuple<T>�����������ص����⣬�������λ��һ��������dll��System.ValueTuple���У����Ծ���Ҫͨ��nuget����������������뵽�㵱ǰ����Ŀ�С�

5. ������汾��ʼһֱ����ʽ��֮ǰ���������ṩ�����ĵ�����������ĳЩ�����ʹ�ã������ڶ���������鿴���顣

6. �����ʹ�õ������ڰ汾��Visual Studio����ôĳЩ�ܹ���Visual Studio 2017��û���﷨����Ĵ����񣬽���������ʹ�õİ汾��ʧЧ������˵������Visual Studio 2017�У�һ�����д�������Կ��������µĸ�ʽ��д��
```csharp
public Video DirectXVideoInstance { get => _video; set => _video = value; }
```
��ô�����ڰ汾��Visual Studio�����޸�Ϊ��
```csharp
public Video DirectXVideoInstance { get { return _video; } set { _video = value; } }
```
���⣬ʵ�����Եĸ�ֵ����Visual Studio 2017�п���������ĸ�ʽȥ������
```csharp
SDevicesMode devM = new SDevicesMode
{
   Size = (short)Marshal.SizeOf(typeof(SDevicesMode))
};
```
��ô�������������ڰ汾��Visual Studio������Ҫ�޸�Ϊ��
```csharp
SDevicesMode devM = new SDevicesMode();
devM.Size = (short)Marshal.SizeOf(typeof(SDevicesMode));
```
���ϵ����﷨��ʽ��������C# 7.0���������Ҫ�˽����C# 7.0����Visual Studio 2017�������ԣ���[�������](https://www.cnblogs.com/cncc/p/7698543.html)������ϸ�˽⣬�����������������޷����ʸղŵ����ӣ����ֶ�������������ǰ�����з��ʣ�
> https://www.cnblogs.com/cncc/p/7698543.html