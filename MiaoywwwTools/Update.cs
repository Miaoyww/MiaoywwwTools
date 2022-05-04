using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MiaoywwwTools.MainWin
{
    internal class Update
    {
        public static string? CheckNeededUpdate()
        {
            string? api_version = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "UpdateApiVersion",
                "https://api.github.com/repos/Miaoywww/MiaoywwwTools/contents/MiaoywwwTools/version.json").ToString();
            string? api_download = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "UpdateApiDownload",
                "https://api.github.com/repos/Miaoywww/MiaoywwwTools/releases").ToString();

            HttpClient httpClientV = new HttpClient();
            httpClientV.DefaultRequestHeaders.Add("Accept", "application/vnd.github.VERSION.raw");
            httpClientV.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            var downloadV = httpClientV.GetStringAsync(api_version).Result;

            JObject? jsonContentV = JsonConvert.DeserializeObject(downloadV) as JObject;
            int? time = int.Parse(jsonContentV["MiaoywwwTools"]["time"].ToString());
            string? ver = jsonContentV["MiaoywwwTools"]["version"].ToString();
            if (int.Parse(GlobalV.AppVersion_time) < time)
            {
                return null;
            }
            else
            {
                return ver;
            }
        }

        public void GetUpdate()
        {
            string? api_version = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "UpdateApiVersion",
               "https://api.github.com/repos/Miaoywww/MiaoywwwTools/contents/MiaoywwwTools/version.json").ToString();
            string? api_download = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "UpdateApiDownload",
                "https://api.github.com/repos/Miaoywww/MiaoywwwTools/releases").ToString();
            string? api_way = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "UpdateWay",
                "github").ToString();
            HttpClient httpClientD = new HttpClient();
            httpClientD.DefaultRequestHeaders.Add("Accept", "application/vnd.github.VERSION.raw");
            httpClientD.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            var download = httpClientD.GetStringAsync(api_download).Result;
            JObject? jsonContentD;
            if (api_way == "github")
            {
                JArray jsonArrayD = JArray.Parse(download);
                jsonContentD = JObject.Parse(jsonArrayD[0].ToString());
            }
            else
            {
                jsonContentD = JsonConvert.DeserializeObject(download) as JObject;
            }
            string? downloadurl = jsonContentD["assets"][0]["browser_download_url"].ToString();
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = Environment.CurrentDirectory + @"\update.exe";
            process.Arguments = downloadurl;
            Process.Start(process);
            Environment.Exit(0);
        }
    }
}
