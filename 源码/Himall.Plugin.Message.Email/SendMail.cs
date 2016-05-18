using Himall.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Himall.Plugin.Message.Email
{
	public class SendMail
	{
		private System.Net.Mail.SmtpClient SmtpClient = null;

		private MailAddress MailAddress_from = null;

		private MailAddress MailAddress_to = null;

		private MailMessage MailMessage_Mai = null;

		public SendMail(string serverHost, int Port, string mailFrom, string Password, string DisPlayText)
		{
            setSmtpClient(serverHost, Port);
            setAddressform(mailFrom, Password, DisPlayText);
		}

		public SendMail()
		{
			MessageEmailConfig config = EmailCore.GetConfig();
            setSmtpClient(config.SmtpServer, Convert.ToInt32(config.SmtpPort));
            setAddressform(config.SendAddress, config.Password, config.DisplayName);
		}

		private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Log.Debug(string.Concat(e.Error.Message, e.Error.InnerException.Message));
			}
		}

		public void SendEmail(string title, string[] mailto, string content, bool async = true)
		{
            MailMessage_Mai = new MailMessage();
			string[] strArrays = mailto;
			for (int i = 0; i < strArrays.Length; i++)
			{
                MailAddress_to = new MailAddress(strArrays[i]);
                MailMessage_Mai.To.Add(MailAddress_to);
			}
            MailMessage_Mai.From = MailAddress_from;
            MailMessage_Mai.Subject = title;
            MailMessage_Mai.IsBodyHtml = true;
            MailMessage_Mai.SubjectEncoding = Encoding.UTF8;
            MailMessage_Mai.Body = content;
            MailMessage_Mai.BodyEncoding = Encoding.UTF8;
            MailMessage_Mai.Attachments.Clear();
			if (!async)
			{
                SmtpClient.Send(MailMessage_Mai);
			}
			else
			{
                SmtpClient.SendCompleted += new SendCompletedEventHandler(SendMail.SendCompletedCallback);
                SmtpClient.SendAsync(MailMessage_Mai, "");
			}
		}

		private void setAddressform(string MailAddress, string MailPwd, string DisplayName)
		{
			NetworkCredential networkCredential = new NetworkCredential(MailAddress, MailPwd);
            MailAddress_from = new System.Net.Mail.MailAddress(MailAddress, DisplayName);
            SmtpClient.Credentials = networkCredential;
		}

		private void setSmtpClient(string ServerHost, int Port)
		{
            SmtpClient = new System.Net.Mail.SmtpClient()
			{
				Host = ServerHost,
				Port = Port
			};
		}
	}
}