using Autodesk.Revit.UI;
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
        private static string apiToken = "gv_sk_5RWCUIW_jnnipeBeq15Z2SDQLkkUSAI94eYbZG1ZsaI";
        private string filePath { get; set; }
        private static string url = "https://api-cpsk-superapp.gip.su/api/gip-vision/v1/session/by_plane/";
        public async Task Main()
        {
            using (var http = new HttpClient())
            using (var form = new MultipartFormDataContent())
            using (var stream = File.OpenRead(filePath))
            using (var fileContent = new StreamContent(stream))
            {
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, "model_file", Path.GetFileName(filePath));
                http.DefaultRequestHeaders.Add("X-API-Key", apiToken);

                var response = await http.PostAsync(url, form);
                var body = await response.Content.ReadAsStringAsync();

                TaskDialog.Show("asdf", $"{(int)response.StatusCode}\n{body}");
            }
        }
    }
}
