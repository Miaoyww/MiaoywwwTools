using MiaoywwwTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace RandomDrawLib
{
    internal class RaDraw
    {
        public string[]? GetRandomResult()
        {
            if (File.Exists("./Resources/Data/stdata.json"))
            {
                WinMessage winMessage = new WinMessage();
                winMessage.SetMessage("警告", "未找到文件./Resources/Data/stdata.json", "close", "yes");
                winMessage.ShowDialog();
                return null;
            }
            else
            {
                using (StreamReader reader = File.OpenText(@"./data/stdata.json"))
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