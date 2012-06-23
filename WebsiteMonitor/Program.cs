using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebsiteMonitor
{
    class Program
    {
        private static WebsiteMonitorConfiguration _config = (WebsiteMonitorConfiguration)ConfigurationManager.GetSection("websitemonitor");

        static void Main(string[] args)
        {
            foreach (WebsiteMonitorConfiguration.WebsiteElement website in _config.Websites)
            {
                Console.WriteLine("Checking " + website.Name+"...");

                Regex requiredContentRegex = null;
                Regex forbiddenContentRegex = null;
                if (!string.IsNullOrEmpty(website.RequiredContent))
                {
                    try
                    {
                        requiredContentRegex = new Regex(website.RequiredContent,RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to create RequiredContent Regex "+ex);
                    }
                }
                if (!string.IsNullOrEmpty(website.ForbiddenContent))
                {
                    try
                    {
                        forbiddenContentRegex = new Regex(website.ForbiddenContent, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to create ForbiddenContent Regex " + ex);
                    }
                }

                //try pinging the website a max of 3 times
                int maxRetry = 2;
                string message = string.Empty;
                bool result = true;
                while (maxRetry >= 0)
                {
                    result = CheckWebsite(website, requiredContentRegex, forbiddenContentRegex, out message);

                    if (result)
                    {
                        break;
                    }
                    else
                    {
                        --maxRetry;
                    }
                }

                //if it failed all 3 times, send an alert.
                if (!result)
                {
                    WebsiteAlertEmail.Send(_config, website,message);
                }
            }
        }

        private static bool CheckWebsite(WebsiteMonitorConfiguration.WebsiteElement website, Regex requiredContentRegex, Regex forbiddenContentRegex,out string message)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(website.Url,UriKind.Absolute));
            request.AllowAutoRedirect = true;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode!=HttpStatusCode.OK)
                {
                    message = "Expected 200 result, got " + (int) response.StatusCode;
                    return false;
                }
                else
                {
                    if (requiredContentRegex!=null || forbiddenContentRegex!=null)
                    {
                        using (TextReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string content = reader.ReadToEnd();

                            if (requiredContentRegex!=null)
                            {
                                //if the required content was not found, then send an alert
                                if (!requiredContentRegex.IsMatch(content))
                                {
                                    message = "Required content not found";
                                    return false;
                                }
                            }

                            if (forbiddenContentRegex != null)
                            {
                                //if the forbidden content was found, then send an alert
                                if (forbiddenContentRegex.IsMatch(content))
                                {
                                    message = "Forbidden content found";
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Unexpected exception "+ex;
                return false;
            }

            message = "OK";
            return true;
        }
    }
}
