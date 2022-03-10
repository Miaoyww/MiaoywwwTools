using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using MiaoywwwTools;

namespace RandomDrawLib
{
    internal class RaDraw
    {
        public string[]? GetRandomResult()
        {
            // 哦这该死的设定
            // 这里不得用 ./
            if (File.Exists("/Resources/Data/stdata.json"))
            {
                WinMessage winMessage = new WinMessage();
                winMessage.SetMessage("错误", "未找到/Resources/Data/stdata.json文件", "close", "yes");
                winMessage.ShowDialog();
                return null;
            }
            else
            {
                
                // 这里的文件目录必须得用 ./
                using (StreamReader reader = File.OpenText(@"./Resources/Data/stdata.json"))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    if (jsonObject != null)
                    {
                        Random rd = new Random();
                        int rdnumber = rd.Next(0, jsonObject.Count);
                        string[] result = new string[2];
                        result[0] = jsonObject[propertyName: rdnumber.ToString()]["name"].ToString();
                        result[1] = jsonObject[propertyName: rdnumber.ToString()]["grade"].ToString();
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
        }
    }
}
