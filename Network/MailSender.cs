using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Mime;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace Cabinink.Network
{
   /// <summary>
   /// 用于发送电子邮件的类
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class MailSender
   {
      private MailMessage _mailMessage;//主要处理发送邮件的内容（如：收发人地址、标题、主体、图片等等）
      private SmtpClient _smtpClient;//主要处理用smtp方式发送此邮件的配置信息（如：邮件服务器、发送端口号、验证方式等等）
      private int _port;//发送邮件所用的端口号（htmp协议默认为25）
      private string _server;//发件箱的邮件服务器地址（IP形式或字符串形式均可）
      private string _senderPassword;//发件箱的密码
      private string _senderUserName;//发件箱的用户名（即 @符号前面的字符串，例如：hello@163.com，用户名为：hello）
      private bool _enableSsl;//是否对邮件内容进行socket层加密传输
      private bool _authentication; //是否对发件人邮箱进行密码验证
      /// <summary>
      /// 构造函数，通过MailMessage实例初始化当前的电子邮件发送实例。
      /// </summary>
      /// <param name="messageInstance">一个MailMessage实例，用于存储收件人，邮件主题，邮件主题等属性的参数。</param>
      /// <param name="server">发件箱的邮件服务器地址。</param>
      /// <param name="userName">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）。</param>
      /// <param name="password">发件人邮箱密码。</param>
      /// <param name="port">发送邮件所用的端口号（stmp协议默认为25）。</param>
      /// <param name="enableSsl">true表示对邮件内容进行socket层加密传输，false表示不加密。</param>
      /// <param name="isCheckedPassword">true表示对发件人邮箱进行密码验证，false表示不对发件人邮箱进行密码验证。</param>
      public MailSender(MailMessage messageInstance, string server, string userName, string password, string port, bool enableSsl, bool isCheckedPassword)
      {
         _mailMessage = messageInstance;
         _server = server;
         _senderUserName = userName;
         _senderPassword = password;
         _port = Convert.ToInt32(port);
         _enableSsl = enableSsl;
         _authentication = isCheckedPassword;
      }
      /// <summary>
      /// 构造函数，初始化当前的电子邮件发送实例，默认启用SSL加密和密码检查。
      /// </summary>
      /// <param name="server">发件箱的邮件服务器地址。</param>
      /// <param name="to">收件人地址（可以是多个收件人，程序中是以“;"进行区分的）。</param>
      /// <param name="from">发件人地址。</param>
      /// <param name="subject">邮件标题。</param>
      /// <param name="body">邮件内容（可以以html格式进行设计）。</param>
      /// <param name="userName">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）。</param>
      /// <param name="password">发件人邮箱密码。</param>
      /// <param name="port">发送邮件所用的端口号（stmp协议默认为25）。</param>
      public MailSender(string server, string to, string from, string subject, string body, string userName, string password, string port)
      {
         _mailMessage = new MailMessage();
         _mailMessage.To.Add(to);
         _mailMessage.From = new MailAddress(from);
         _mailMessage.Subject = subject;
         _mailMessage.Body = body;
         _mailMessage.IsBodyHtml = true;
         _mailMessage.BodyEncoding = Encoding.UTF8;
         _mailMessage.Priority = MailPriority.Normal;
         _server = server;
         _senderUserName = userName;
         _senderPassword = password;
         _port = Convert.ToInt32(port);
         _enableSsl = true;
         _authentication = true;
      }
      /// <summary>
      /// 构造函数，初始化当前的电子邮件发送实例。
      /// </summary>
      /// <param name="server">发件箱的邮件服务器地址。</param>
      /// <param name="to">收件人地址（可以是多个收件人，程序中是以“;"进行区分的）。</param>
      /// <param name="from">发件人地址。</param>
      /// <param name="subject">邮件标题。</param>
      /// <param name="body">邮件内容（可以以html格式进行设计）。</param>
      /// <param name="userName">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）。</param>
      /// <param name="password">发件人邮箱密码。</param>
      /// <param name="port">发送邮件所用的端口号（stmp协议默认为25）。</param>
      /// <param name="enableSsl">true表示对邮件内容进行socket层加密传输，false表示不加密。</param>
      /// <param name="isCheckedPassword">true表示对发件人邮箱进行密码验证，false表示不对发件人邮箱进行密码验证。</param>
      public MailSender(string server, string to, string from, string subject, string body, string userName, string password, string port, bool enableSsl, bool isCheckedPassword)
      {
         _mailMessage = new MailMessage();
         _mailMessage.To.Add(to);
         _mailMessage.From = new MailAddress(from);
         _mailMessage.Subject = subject;
         _mailMessage.Body = body;
         _mailMessage.IsBodyHtml = true;
         _mailMessage.BodyEncoding = Encoding.UTF8;
         _mailMessage.Priority = MailPriority.Normal;
         _server = server;
         _senderUserName = userName;
         _senderPassword = password;
         _port = Convert.ToInt32(port);
         _enableSsl = enableSsl;
         _authentication = isCheckedPassword;
      }
      /// <summary>
      /// 获取或者设置当前实例的邮件附件。
      /// </summary>
      /// <exception cref="NullReferenceException">当赋予附件集合属性的值为null或者Count=0时，则会抛出这个异常。</exception>
      public AttachmentCollection Attachments
      {
         get => MailMessage.Attachments;
         set
         {
            if (value == null) throw new NullReferenceException("不允许空的附件集！");
            else Parallel.For(0, value.Count, (index) => MailMessage.Attachments.Add(value[index]));
         }
      }
      /// <summary>
      /// 获取或设置当前实例的SMTP客户端。
      /// </summary>
      public MailMessage MailMessage { get => _mailMessage; set => _mailMessage = value; }
      /// <summary>
      /// 获取或设置当前实例的SMPT协议。
      /// </summary>
      public SmtpClient SmtpClient { get => _smtpClient; set => _smtpClient = value; }
      /// <summary>
      /// 获取或设置当前实例用于发送邮件的端口号。
      /// </summary>
      public string Port { get => _port.ToString(); set => _port = Convert.ToInt32(value); }
      /// <summary>
      /// 获取或设置当前实例发件箱的邮件服务器地址，这个地址的格式允许为IP形式或字符串形式。
      /// </summary>
      public string Server { get => _server; set => _server = value; }
      /// <summary>
      /// 获取或设置当前实例的发件人的邮箱帐号密码。
      /// </summary>
      public string SenderPassword { get => _senderPassword; set => _senderPassword = value; }
      /// <summary>
      /// 获取或设置当前实例的发件人的用户名称。
      /// </summary>
      public string SenderUserName { get => _senderUserName; set => _senderUserName = value; }
      /// <summary>
      /// 获取或设置当前实例的SSL启用标识。
      /// </summary>
      public bool EnableSSL { get => _enableSsl; set => _enableSsl = value; }
      /// <summary>
      /// 获取或设置当前实例发件者邮箱密码验证标识。
      /// </summary>
      public bool Authentication { get => _authentication; set => _authentication = value; }
      /// <summary>
      /// 向电子邮件中添加附件。
      /// </summary>
      /// <param name="attachments">附件的路径集合，以分号分隔。</param>
      public void AddAttachments(string attachments)
      {
         string[] path = attachments.Split(';');
         Attachment data;
         ContentDisposition disposition;
         for (int i = 0; i < path.Length; i++)
         {
            data = new Attachment(path[i], MediaTypeNames.Application.Octet);
            disposition = data.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(path[i]);
            disposition.ModificationDate = File.GetLastWriteTime(path[i]);
            disposition.ReadDate = File.GetLastAccessTime(path[i]);
            MailMessage.Attachments.Add(data);
         }
      }
      /// <summary>
      /// 从电子邮件中移除所有的附件。
      /// </summary>
      public void ClearAttachments()
      {
         MailMessage.Attachments.Clear();
      }
      /// <summary>
      /// 发送一封电子邮件。
      /// </summary>
      /// <exception cref="CannotSendMailException">当出现其他异常导致邮件发送失败时，则会统一抛出的异常</exception>
      public void Send()
      {
         try
         {
            if (_mailMessage != null)
            {
               _smtpClient = new SmtpClient()
               {
                  Host = _server,
                  Port = _port,
                  UseDefaultCredentials = false,
                  EnableSsl = _enableSsl
               };
               if (_authentication)
               {
                  NetworkCredential credential = new NetworkCredential(_senderUserName, _senderPassword);
                  _smtpClient.Credentials = credential.GetCredential(_smtpClient.Host, _smtpClient.Port, "NTLM");
               }
               else
               {
                  _smtpClient.Credentials = new NetworkCredential(_senderUserName, _senderPassword);
               }
               _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
               _smtpClient.Send(_mailMessage);
            }
         }
         catch
         {
            throw new CannotSendMailException();
         }
      }
   }
   /// <summary>
   /// 无法发送电子邮件时抛出的异常。
   /// </summary>
   [Serializable]
   public class CannotSendMailException : Exception
   {
      public CannotSendMailException() : base("电子邮件发送失败！") { }
      public CannotSendMailException(string message) : base(message) { }
      public CannotSendMailException(string message, Exception inner) : base(message, inner) { }
      protected CannotSendMailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
