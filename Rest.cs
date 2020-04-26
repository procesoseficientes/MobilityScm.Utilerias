using System.Collections;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

namespace MobilityScm.Utilerias
{
    public static class Rest
    {
        private static readonly JavaScriptSerializer Jss = new JavaScriptSerializer();
        
        
        public static T ExecutePost<T>(string actionUrl, object body) where T : class
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(actionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Headers.Add("Authorization", "Basic ZDRydGgtdjRkM3I6SW50ZWdyYS5zNHA=");
            request.ContentType = "application/json";

            using (StreamWriter swJsonPayload = new StreamWriter(request.GetRequestStream()))
            {
                swJsonPayload.Write(Jss.Serialize(body));
                swJsonPayload.Flush();
                swJsonPayload.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream responseStreamContent = response.GetResponseStream())
            {
                if (responseStreamContent != null)
                {
                    using (StreamReader reader = new StreamReader(responseStreamContent))
                    {
                        var content = reader.ReadToEnd();
                        var result = Jss.Deserialize<T>(content);
                        return result;
                    }
                }
            }
            return null;
        }
    }
}