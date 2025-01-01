using System.Net;
using System.Net.Mail;

namespace MailUtils
{
    public static class MailProider
    {

        /// <summary>
        /// Gửi email sử dụng máy chủ SMTP Google (smtp.gmail.com)
        /// </summary>
        public static async Task<bool> SendGmail(string _from
                                                    , string _to
                                                    , string _subject
                                                    , string _body
                                                    , string _gmailsend
                                                    , string _gmailpassword)
        {
            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);

            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                try
                {
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_gmailsend, "fshmviptcouvdgat");
                    client.EnableSsl = true;
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return true;
            }
        }
    }
}