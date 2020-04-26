using System.Collections;
using System.Collections.Generic;
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


        public static string ExecuteGet(string actionUrl)
        {
            string respuesta;

            var solicitudAlApi = (HttpWebRequest)WebRequest.Create(actionUrl);
            solicitudAlApi.Method = WebRequestMethods.Http.Get;
            solicitudAlApi.Accept = "application/json";
            solicitudAlApi.ContentType = "application/json";

            using (var respuestaDelApi = (HttpWebResponse)solicitudAlApi.GetResponse())
            {
                var stream = respuestaDelApi.GetResponseStream();
                if (stream == null) return null;
                var reader = new StreamReader(stream);
                respuesta = reader.ReadToEnd();
                stream.Dispose();
            }

            return respuesta;
        }
    }
}