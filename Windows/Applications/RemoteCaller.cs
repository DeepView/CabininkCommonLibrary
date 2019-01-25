using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.Windows.Applications
{
   /// <summary>
   /// 用于实现远程调用的Router简易实现类。
   /// </summary>
   /// <example>
   /// 这个类常用于远程访问某个地址，或者调用本地程序集的接口，在使用这个类之前，说先要进行单例化：
   /// <code>
   /// RemoteCaller router = RemoteCaller.CreateInstance();
   /// </code>
   /// 当然你可以在访问CreateInstance方法指定更加详细的参数。
   /// 如果你需要打开一个Web页面，那么可以通过以下方式进行访问，然后将会得到一个存放Web页面的HTML代码或者请求的Json代码的字符串：
   /// <code>
   /// string original = @"https://visualstudio.microsoft.com/zh-hans/";
   /// Uri webUrl = new Uri(original);
   /// Console("Result:\n\n{0}", router.Via(webUrl, out HttpStatusCode code));
   /// </code>
   /// 当然如果你不需要获取Via方法捕获到的HTTP状态码，则可以替换为这个方法的重载版本：
   /// <code>
   /// Uri webUrl = new Uri(@"https://visualstudio.microsoft.com/zh-hans/");
   /// object obj = router.Via(webUrl);
   /// </code>
   /// 如果你需要访问一个FTP、本地文件或者本地目录，那么，只需要将上面代码中string变量original的值替换掉即可，比如说：
   /// <code>
   /// string original = @"C:\Windows\System32\Notepad.exe";
   /// </code>
   /// Via方法支持通过URL的方式访问程序集（当前程序集或者指定的程序集）中的公开的静态方法，一般要实现其有效访问，需要为其对应的实例附加一个Protocol。
   /// <para>值得说明的是，每一个类的Protocol类，其类名称后面必须是Protocol结尾，且必须实现IRemoteCallProtocol接口。比如说存在如下一个User类：</para>
   /// <code>
   /// public class User
   /// {
   ///    private int _id;
   ///    private string _name;
   ///    public User(int id, string name)
   ///    {
   ///       _id = id;
   ///       _name = name;
   ///    }
   ///    public override string ToString() => "User[Id=" + _id + ";Name=" + _name + "]";
   /// }
   /// </code>
   /// 那么我们就要为这个User类实现一个UserProtocol类，并实现IRemoteCallProtocol接口，示例代码如下：
   /// <code>
   /// public class UserProtocol : IRemoteCallProtocol&lt;User&gt;
   /// {
   ///    public User Ctor(IDictionary&lt;string, object&gt; parameters)
   ///    {
   ///       bool isGotId = parameters.TryGetValue("id", out object id);
   ///       bool isGotName = parameters.TryGetValue("name", out object name);
   ///       User usr = new User(Convert.ToInt32(id), Convert.ToString(name));
   ///       return usr;
   ///    }
   ///    public static User Init(IDictionary&lt;string, object&gt; parameters)
   ///    {
   ///       UserProtocol userProtocol = new UserProtocol();
   ///       return userProtocol.Ctor(parameters);
   ///    }
   /// }
   /// </code>
   /// <para>注意，需要通过URL访问的接口必须为静态接口！</para>
   /// 当我们需要通过一个URL初始化一个User实例的时候，那么就可以通过下面的代码实现：
   /// <code>
   /// RemoteCaller router = RemoteCaller.CreateInstance(Assembly.GetExecutingAssembly());
   /// string original = @"app://Namespace.User/Init?id=0&amp;name=cabinink";
   /// Uri nativebUrl = new Uri(original);
   /// object obj = router.Via(nativeUrl, out HttpStatusCode code);
   /// Console.WriteLine("{0}", obj.ToString());
   /// </code>
   /// 在上述代码呈现的链接里面，Namespace表示User类所在的命名空间，假设User类在MyApp.Core命名空间下，那么你的链接应该替换成如下所示：
   /// <code>
   /// string original = @"app://MyApp.Core.User/Init?id=0&amp;name=cabinink"
   /// </code>
   /// 最后需要注意的是，一定要区分这类URL的字符串大小写。
   /// </example>
   [Serializable]
   [ComVisible(true)]
   public class RemoteCaller
   {
      private string _appSchemeName;//应用程序的URL方案名称。
      private bool _isMustProtocol;//当URL访问的是应用程序集，则指定是否需要进行协议化访问。
      private Dictionary<string, string> _schemes;//可用的方案集合。
      private Assembly _assembly;//用于访问的程序集。
      private static RemoteCaller _router = null;//一个RemoteCaller实例。
      private static readonly object _routerLock = new object();//防止多线程调用CreateInstance造成构造异常的线程锁。
      private const string DEFAULT_APP_SCHEME_NAME = @"app";//常量，表示一个默认的应用程序URL方案名称。
      /// <summary>
      /// 获取或设置（set代码块对外不可见，因为是private）当前实例的可用方案集合。
      /// </summary>
      public Dictionary<string, string> Schemes
      {
         get => _schemes;
         private set
         {
            value = new Dictionary<string, string>
            {
               { "OpenHttp", "http" },
               { "OpenSslHttp", "https" },
               { "OpenFtp", "ftp" },
               { "OpenLocalFile", "file" },
               { "OpenNative", AppSchemeName }
            };
            if (_schemes == null) _schemes = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例的应用程序URL方案名称。
      /// </summary>
      public string AppSchemeName
      {
         get => _appSchemeName;
         set
         {
            if (string.IsNullOrWhiteSpace(value)) value = DEFAULT_APP_SCHEME_NAME;
            _appSchemeName = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例中的Via方法在访问应用程序集的时候，是否遵循协议化访问规则。
      /// </summary>
      public bool IsMustProtocol { get => _isMustProtocol; set => _isMustProtocol = value; }
      /// <summary>
      /// 获取或设置当前实例中用于访问或者作为被调用方所在的程序集。
      /// </summary>
      public Assembly Assembly { get => _assembly; set => _assembly = value; }
      /// <summary>
      /// 打开一个URL，并根据URL Scheme（URL方案）做出相应的访问决策。
      /// </summary>
      /// <param name="url">指定的一个有效URL。</param>
      /// <returns>该操作将会返回一个object对象，这个对象是打开URL执行决策之后所得到的返回值。</returns>
      public object Via(Uri url) => Via(url, out HttpStatusCode code);
      /// <summary>
      /// 打开一个URL，并根据URL Scheme（URL方案）做出相应的访问决策，并更新传递的HTTP状态码。
      /// </summary>
      /// <param name="url">指定的一个有效URL。</param>
      /// <param name="httpStatusCode">HTTP请求的状态码。</param>
      /// <returns>该操作将会返回一个object对象，这个对象是打开URL执行决策之后所得到的返回值。</returns>
      public object Via(Uri url, out HttpStatusCode httpStatusCode)
      {
         httpStatusCode = HttpStatusCode.OK;
         object result = new object();
         if (url == null)
         {
            httpStatusCode = HttpStatusCode.NotFound;
            result = null;
         }
         string selector = (from q in Schemes where q.Value == url.Scheme select q.Key).ToList()[0];
         if (selector == null)
         {
            httpStatusCode = HttpStatusCode.BadRequest;
            result = null;
         }
         BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
         MethodInfo method = GetType().GetMethod(selector, flags);
         result = method.Invoke(_router, new object[] { url });
         return result;
      }
      /// <summary>
      /// 通过默认URL Scheme并传入当前程序集来创建一个RemoteCaller实例。
      /// </summary>
      /// <returns>该操作将会得到一个新的RemoteCaller实例。</returns>
      public static RemoteCaller CreateInstance() => CreateInstance(null, Assembly.GetExecutingAssembly());
      /// <summary>
      /// 通过指定有效的URL Scheme并传入当前程序集来创建一个RemoteCaller实例。
      /// </summary>
      /// <param name="appSchemeName">指定的有效URL Scheme。</param>
      /// <returns>该操作将会得到一个新的RemoteCaller实例。</returns>
      public static RemoteCaller CreateInstance(string appSchemeName) => CreateInstance(appSchemeName, Assembly.GetExecutingAssembly());
      /// <summary>
      /// 通过默认URL Scheme并传入符合CLS的程序集来创建一个RemoteCaller实例。
      /// </summary>
      /// <param name="assembly">指定的符合CLS（通用语言规范）的程序集。</param>
      /// <returns>该操作将会得到一个新的RemoteCaller实例。</returns>
      public static RemoteCaller CreateInstance(Assembly assembly) => CreateInstance(null, assembly);
      /// <summary>
      /// 通过指定有效的URL Scheme并传入符合CLS的程序集来创建一个RemoteCaller实例。
      /// </summary>
      /// <param name="appSchemeName">指定的有效URL Scheme。</param>
      /// <param name="assembly">指定的符合CLS（通用语言规范）的程序集。</param>
      /// <returns>该操作将会得到一个新的RemoteCaller实例。</returns>
      public static RemoteCaller CreateInstance(string appSchemeName, Assembly assembly)
      {
         if (_router == null)
         {
            lock (_routerLock)
            {
               if (_router == null)
               {
                  using (CodeExecutedTimestampResult result = new CodeExecutedTimestampResult("Debug"))
                  {
                     _router = new RemoteCaller
                     {
                        AppSchemeName = appSchemeName,
                        Schemes = new Dictionary<string, string>(),
                        Assembly = assembly,
                        IsMustProtocol = true
                     };
                  }
               }
            }
         }
         return _router;
      }
      /// <summary>
      /// 打开一个本地页面或者入口。
      /// </summary>
      /// <param name="nativeUrl">一个基于程序集方案的URL。</param>
      /// <returns>该操作将会返回执行其详细决策之后所得到的返回结果。</returns>
      protected virtual object OpenNative(Uri nativeUrl)
      {
         object invokeResult = null;
         string originalUrl = nativeUrl.OriginalString;
         int offset = originalUrl.Length - nativeUrl.Scheme.Length - (nativeUrl.Authority.Length + nativeUrl.PathAndQuery.Length);
         string className = originalUrl.Substring(nativeUrl.Scheme.Length + offset, nativeUrl.Host.Length);
         string protocolClassName = IsMustProtocol ? className + "Protocol" : className;
         string methodName = nativeUrl.LocalPath.Substring(1);
         string parametersString = nativeUrl.Query.Substring(1);
         string[] psArray = parametersString.Split(new char[] { '&' });
         Dictionary<string, object> paramList = new Dictionary<string, object>();
         for (int i = 0; i < psArray.Length; i++)
         {
            string[] kvArray = psArray[i].Split(new char[] { '=' });
            paramList.Add(kvArray.First(), kvArray.Last());
         }
         string completeMethodString = className + "." + methodName;
         object obj = Assembly.CreateInstance(protocolClassName, true);
         Type type = obj.GetType();
         if (IsMustProtocol)
         {
            if (type.GetInterface("Cabinink.Windows.Applications.IRemoteCallProtocol`1", true) == null)
            {
               throw new DataTreatment.ORMapping.NotSupportedTypeException();
            }
         }
         BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
         MethodInfo mi = type.GetMethod(methodName, flags);
         invokeResult = mi.Invoke(null, new object[] { paramList });
         return invokeResult;
      }
      /// <summary>
      /// 打开一个本地文件或者本地目录。
      /// </summary>
      /// <param name="fileOrDirectoryUrl">指定的一个文件或者目录的地址。</param>
      /// <returns>该操作将会得到一个打开该文件或者目录的进程。</returns>
      protected virtual Process OpenLocalFile(Uri fileOrDirectoryUrl) => ProcessManager.CreateProcess(fileOrDirectoryUrl.LocalPath);
      /// <summary>
      /// 打开一个FTP服务器。
      /// </summary>
      /// <param name="ftpUrl">指定的FTP地址。</param>
      /// <returns>该操作将会得到一个打开这个FTP地址的进程。</returns>
      protected virtual Process OpenFtp(Uri ftpUrl)
      {
         Process process = new Process();
         if (ftpUrl == null) process = null;
         else process = Process.Start(ftpUrl.AbsoluteUri);
         return process;
      }
      /// <summary>
      /// 打开一个Web页面。
      /// </summary>
      /// <param name="webUrl">指定的Web页面地址。</param>
      /// <returns>该操作将会返回所请求地址返回的页面HTML代码或者Json代码。</returns>
      protected virtual string OpenWeb(Uri webUrl)
      {
         string htmlContext = string.Empty;
         using (StreamReader reader = new StreamReader(WebRequest.Create(webUrl).GetResponse().GetResponseStream()))
         {
            htmlContext = reader.ReadToEnd();
         }
         return htmlContext;
      }
      /// <summary>
      /// 打开一个HTTP链接。
      /// </summary>
      /// <param name="httpUrl">指定的HTTP链接。</param>
      /// <returns>该操作将会返回所请求地址返回的页面HTML代码或者Json代码。</returns>
      protected virtual string OpenHttp(Uri httpUrl) => OpenWeb(httpUrl);
      /// <summary>
      /// 打开一个基于SSL的安全HTTP链接。
      /// </summary>
      /// <param name="httpsUrl">所制定的基于SSL的安全HTTP链接。</param>
      /// <returns>该操作将会返回所请求地址返回的页面HTML代码或者Json代码。</returns>
      protected virtual string OpenSslHttp(Uri httpsUrl) => OpenWeb(httpsUrl);
   }
   /// <summary>
   /// 当URL的方案名称不匹配时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class SchemeNotMatchedException : Exception
   {
      public SchemeNotMatchedException() : base("URL方案名称不匹配！") { }
      public SchemeNotMatchedException(string message) : base(message) { }
      public SchemeNotMatchedException(string message, Exception inner) : base(message, inner) { }
      protected SchemeNotMatchedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}