using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    public class BlafxController : ApiController
    {
        public static string LastArk4Trade = "";
        public static string LastArk3Trade = "";
        public static string LastArk2Trade = "";
        private System.Net.HttpWebRequest CreateWebRequest(string url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                webRequest.Method = "GET";
                return webRequest;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private string BlafxServiceCall(string url)
        {
            try
            {
                System.Net.HttpWebRequest request = CreateWebRequest(url);
                // request.Credentials = New Net.NetworkCredential("health18", "health18")
                IAsyncResult asyncResult = request.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                string soapResult;
                using (System.Net.WebResponse webResponse = request.EndGetResponse(asyncResult))
                {
                    using (System.IO.StreamReader rd = new System.IO.StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
                return soapResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [System.Web.Http.HttpGet]
        public string TradeCheck()
        {
            string resp = "";
            string opentime= "";
            DateTime OpenDate;
            string ReturnMessage = "";
            try
            {
                resp = BlafxServiceCall("https://crm.blafx.com/crm/followConsumer/getOrder?mt5Login=1001015&orderType=1");
                JObject jArray2 = JsonConvert.DeserializeObject<JObject>(resp);
                if (jArray2.Count > 1)
                    //opentime = jArray2("rows")(0)("openTime").ToString();
                    opentime = jArray2["rows"]["openTime"].Value<string>();
                OpenDate = UnixToDateTime(opentime);
                if ((OpenDate.Day == DateTime.Now.Day & OpenDate.Month == DateTime.Now.Month) & LastArk2Trade != opentime)
                {
                    Sendmail("2");
                    ReturnMessage = "Ark 2 Traded Today";
                    LastArk2Trade = opentime;
                }
                else
                    ReturnMessage = "Ark 2 Not Traded Today";

                resp = BlafxServiceCall("https://crm.blafx.com/crm/followConsumer/getOrder?mt5Login=5506597&orderType=1");
                JObject jArray = JsonConvert.DeserializeObject<JObject>(resp);
                if (jArray.Count > 1)
                    //opentime = jArray("rows")(0)("openTime").ToString();
                    opentime = jArray["rows"]["openTime"].Value<string>();
                OpenDate = UnixToDateTime(opentime);
                if ((OpenDate.Day == DateTime.Now.Day & OpenDate.Month == DateTime.Now.Month) & LastArk4Trade != opentime)
                {
                    LastArk4Trade = opentime;
                    Sendmail("4");
                    ReturnMessage = "Ark 4 Traded Today";
                }
                else
                    ReturnMessage = "Ark 4 Not Traded Today";

                resp = BlafxServiceCall("https://crm.blafx.com/crm/followConsumer/getOrder?mt5Login=5501835&orderType=1");
                JObject jArray1 = JsonConvert.DeserializeObject<JObject>(resp);
                if (jArray1.Count > 1)
                    //opentime = jArray1("rows")(0)("openTime").ToString();
                    opentime = jArray1["rows"]["openTime"].Value<string>();
                OpenDate = UnixToDateTime(opentime);
                if ((OpenDate.Day == DateTime.Now.Day & OpenDate.Month == DateTime.Now.Month) & LastArk3Trade != opentime)
                {
                    LastArk3Trade = opentime;
                    Sendmail("3");
                    ReturnMessage = "Ark 3 Traded Today";
                }
                else
                    ReturnMessage = "Ark 3 Not Traded Today";


                return ReturnMessage;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private DateTime UnixToDateTime(string strUnixTime)
        {
            double nTimestamp = Convert.ToInt32(strUnixTime);
            System.DateTime nDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            nDateTime = nDateTime.AddSeconds(nTimestamp);
            return nDateTime;
        }
        private void Sendmail(string ArkType)
        {
            MailMessage mail = new MailMessage();

            // set the addresses
            mail.From = new MailAddress("nadersam@gmail.com");
            mail.To.Add("nadersam@gmail.com");
            mail.To.Add("Islam1076@gmail.com");

            // set the content
            mail.Subject = "ARK " + ArkType + " Trade";
            mail.Body = "Ark " + ArkType + " traded today";

            // set the server
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("nadersam@gmail.com", "krqsmjrjvwphildh");
            smtp.Port = 587;
            smtp.EnableSsl = true;

            // send the message
            try
            {
                smtp.Send(mail);
            }
            catch (Exception exc)
            {
            }
        }

    }
}
