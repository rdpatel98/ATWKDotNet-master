using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using HtmlAgilityPack;

namespace ITEK.EMAILHELPER
{
    public class EmailMessage
    {

        public string Message { get; set; }
        public string Subject { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public bool IsHtml { get; set; }
        public bool EnableSSL { get; set; }
        public string ReplyTo { get; set; }

        public string fromEmailID;
        public string username;
        public string password;
        public string smtpServer;
        public int smtpPort;

        public EmailMessage()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            IsHtml = true;
        }


        public bool SendEmail(string _smtpServer,int _port,string _fromEmailID,bool _enableSSL=true ,string _username="",string _password="",List<string> attachments=null)
        {
            try
            {
                smtpServer = _smtpServer;
                smtpPort = _port;
                username = _username;
                password = _password;
                fromEmailID = _fromEmailID;
                SmtpClient client = new SmtpClient(smtpServer, smtpPort);
                MailMessage message = new MailMessage();
                message.Subject = Subject;
                message.From = new MailAddress(fromEmailID);
                message.IsBodyHtml = IsHtml;
                message.Body = Message;
                if (!string.IsNullOrEmpty(ReplyTo))
                {
                    message.ReplyToList.Add(ReplyTo);
                }
                //HandleInlineImages(message);

                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {
                        message.Attachments.Add(new Attachment(item));
                    }
                }
               
                //Add to
                foreach (var item in To)
                {
                    if (string.IsNullOrEmpty(item) == false)
                    {
                        message.To.Add(new MailAddress(item));
                    }
                }

                //cc
                foreach (var item in Cc)
                {
                    if (string.IsNullOrEmpty(item) == false)
                    {
                        message.CC.Add(new MailAddress(item));
                    }
                }
                //bcc
                foreach (var item in Bcc)
                {
                    if (string.IsNullOrEmpty(item) == false)
                    {
                        message.Bcc.Add(new MailAddress(item));
                    }
                }
                client.EnableSsl = _enableSSL;
               // client.UseDefaultCredentials = false;
                if (string.IsNullOrEmpty(username))
                {
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else
                {
                    client.Credentials = new NetworkCredential(username, password);
                }
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                client.Timeout = Int32.MaxValue;
                client.Send(message);
            }
            catch (Exception)
            {
                
                return false;
            }
            return true;
        }

        public void HandleInlineImages(MailMessage message)
        {
            try
            {
                string inlineMessage = message.Body;
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(inlineMessage);
                var imgs = doc.DocumentNode.SelectNodes("//img");

                if (imgs != null)
                {
                    foreach (var img in imgs)
                    {
                        string orig = img.Attributes["src"].Value;
                        Uri u = new Uri(orig, UriKind.RelativeOrAbsolute);
                        Attachment temp = new Attachment(u.LocalPath);
                        message.Attachments.Add(temp);
                        img.SetAttributeValue("src", "cid:" + temp.ContentId);
                    }
                }

                message.Body = doc.DocumentNode.OuterHtml;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public  bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
            {
               // if (System.Windows.Forms.MessageBox.Show("The server certificate is not valid.\nAccept?", "Certificate Validation", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    return true;
                //else
                //    return false;
            }
        }


    }
}
