using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace WebsiteMonitor
{
    class WebsiteAlertEmail
    {
        public static void Send(WebsiteMonitorConfiguration config,WebsiteMonitorConfiguration.WebsiteElement website,string errorMessage)
        {
            try
            {
                Console.WriteLine(website.Name + " appears to be down. Sending alert...");
                MailAddress from = new MailAddress(config.FromEmailAddress, "Website Monitor");

                MailMessage message = new MailMessage(from, new MailAddress(config.AlertEmailAddress))
                {
                    Body = "The website " + website.Name + " appears to be down. " + errorMessage + " This means that it either returned a non 200 http response when requested, or the page content returned was unexpected.",
                    Subject = website.Name + " appears to be down"
                };

                SmtpClient client = new SmtpClient();
                if (config.EmailuseSsl)
                {
                    client.EnableSsl = true;
                }
                client.Send(message);
            }
            catch (Exception ex)
            {
                //DOH!, couldn't send the email
                Console.WriteLine("Unable to send alert email "+ex);
            }
        }
    }
}
