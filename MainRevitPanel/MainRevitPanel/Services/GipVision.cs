using Autodesk.Revit.UI;
using MainRevitPanel.UI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Services
{
    public class GipVision
    {
        public string apiToken { get; set; }
        private string filePath { get; set; }
        private static string url = "https://api-cpsk-superapp.gip.su/";
        public void Main()
        {
            string url_by_panel = url + "api/gip-vision/v1/session/by_plane/";
            using (var http = new HttpClient())
            using (var form = new MultipartFormDataContent())
            using (var stream = File.OpenRead(filePath))
            using (var fileContent = new StreamContent(stream))
            {
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, "model_file", Path.GetFileName(filePath));
                http.DefaultRequestHeaders.Add("X-API-Key", apiToken);

                var response = http.PostAsync(url_by_panel, form).Result;
                var body = response.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(body);

                var model = new GipVisionModel { pin_code = (string)json["onetime_code"], url = (string)json["deeplink"] };
                TaskDialog.Show("asdf", $"{(int)response.StatusCode}\n{body}");
            }
        }
        public void LoadData(string path, string key)
        {
            filePath = path;
            apiToken = key;
        }

        public int CheckAPIKey(string key)
        {
            try
            {
                string url_Check = url + "api/gip-vision/v1/health/";
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("X-API-Key", key);

                    int response = (int)http.GetAsync(url_Check).Result.StatusCode;

                    return response;
                }
            }
            catch
            {
                return 404;
            }
            
        }

    }
}
