using MiaoywwwTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace RandomDrawLib
{
    public class RaDraw
    {
        public string keypath
        {
            get
            {
                return @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\Tools\RandomDraw";
            }
            set
            {
                keypath = value;
            }
        }

        public JObject Read()
        {
            // 哦这该死的设定
            // 这里不得用 ./ca
            if (File.Exists("./Resources/Data/stdata.json"))
            {
                // 这里的文件目录必须得用 ./
                StreamReader reader = File.OpenText(@"./Resources/Data/stdata.json");
                return (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
            else
            {
                MessageBox.ShowDialog("未找到/Resources/Data/stdata.json文件");
                return null;
            }
        }

        public int GetRandomNumber(int start, int end)
        {
            Random random = new();
            return random.Next(start, end);
        }

        public List<object> GetListResult(JObject jsonObject)
        {
            JObject result = GetRandomResult(jsonObject);
            if (result != null)
            {
                List<object> array = new List<object>();
                jsonObject.Remove(result["select"].ToString());
                array.Add(result);
                array.Add(jsonObject);
                return array;
            }
            else
            {
                return null;
            }
        }

        public JObject? GetRandomResult(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                Random rd = new();
                List<int> randomNumberList = new List<int>();
                foreach (JProperty property in jsonObject.Properties())
                {
                    randomNumberList.Add(int.Parse(property.Name));
                }
                int selectnumber = rd.Next(randomNumberList.Count);
                if (randomNumberList.Count == 0)
                {
                    return null;
                }
                else
                {
                    JObject result = new JObject();
                    result["select"] = randomNumberList[selectnumber].ToString();
                    result["name"] = jsonObject[randomNumberList[selectnumber].ToString()]["name"].ToString();
                    result["grade"] = jsonObject[randomNumberList[selectnumber].ToString()]["grade"].ToString();
                    return result;
                }
            }
            else
            {
                return null;
            }
        }

        public int GetStdataLentgh()
        {
            using StreamReader reader = File.OpenText(@"./Resources/Data/stdata.json");
            JObject jsonObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            if (jsonObject != null)
            {
                int result = jsonObject.Count;
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}