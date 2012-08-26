using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace SkJukeBox_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An Unhandled Exception was thrown, please take this screen and send to me at quocfreelancer@gmail.com.\nThank\n" + e.Exception.Message, "Oops error was thrown :(", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.SendEmail(e.Exception.Message + "\n" + e.Exception.InnerException + "\n" + e.Exception.StackTrace);
            e.Handled = true;
        }

        void SendEmail(string msgBody)
        {
            var mailMessage = new System.Net.Mail.MailMessage();
            SmtpClient mailClient = new SmtpClient("smtp.gmail.com", 587);
            mailClient.Timeout = 15000;
            mailClient.Credentials = new NetworkCredential("quocnm89@gmail.com", "26051989quoc");
            mailClient.EnableSsl = true;
            mailMessage.IsBodyHtml = false;
            mailMessage.From = new MailAddress("quocnm89@gmail.com");
            mailMessage.Subject = "Sk JukeBox WPF Error Logging";
            mailMessage.Body = msgBody;
            mailMessage.To.Add("quocfreelancer@gmail.com");
            mailClient.Send(mailMessage);
        }
    }
}
