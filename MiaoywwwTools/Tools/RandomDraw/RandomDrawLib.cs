using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace RandomDrawLib
{
    internal class RaDraw
    {
        public string[]? GetRandomResult()
        {
            if (File.Exists("/data/stdata.json"))
            {
                throw new Exception("未找到文件/data/stdata.json");
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
