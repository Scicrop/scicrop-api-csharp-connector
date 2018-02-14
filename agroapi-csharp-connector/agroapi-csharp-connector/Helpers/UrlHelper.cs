using SciCrop.AgroAPI.Connector.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SciCrop.AgroAPI.Connector.Helpers
{
    public class UrlHelper
    {

        private static UrlHelper instance;

        private UrlHelper() { }

        public static UrlHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UrlHelper();
                }
                return instance;
            }
        }

        public string GetJsonFromUrl(String url)
        {
            string json = null;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(url);
            }

            return json;
        }

        public string PostScicropEntityJsonBA(string rest, ScicropEntity se, string username, string password)
        {
            string jsonStr = null;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://engine.scicrop.com/scicrop-engine-web/api/v1/" + rest);

                string restCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

                httpWebRequest.Headers.Add("Authorization", "Basic " + restCredentials);


                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(se.ToJson());
                    
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    jsonStr = streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {

                throw;
            }
            
            
            return jsonStr;
        }

        public string GetScicropEntityJsonBA(string rest, string username, string password)
        {
            string jsonStr = null;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://engine.scicrop.com/scicrop-engine-web/api/v1/" + rest);

                string restCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

                httpWebRequest.Headers.Add("Authorization", "Basic " + restCredentials);


                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

      

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    jsonStr = streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {

                throw;
            }


            return jsonStr;
        }
    }
}
