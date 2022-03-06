﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace RandomDraw
{
    internal class RaDraw
    {
        public string[]? GetRandomResult()
        {
            if (File.Exists("/data/stdata.json"))
            {
                throw new Exception("未找到文件/data/stdata.json");
            }
            using (StreamReader reader = File.OpenText(@"./data/stdata.json"))
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                if (jsonObject != null)
                {
                    Random rd = new Random();
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8602
                    int rdnumber = rd.Next(0, jsonObject[propertyName: "content"].Count());
                    string[] result = new string[2];
                    result[0] = jsonObject[rdnumber.ToString()]["grade"].ToString();
                    result[1] = jsonObject[rdnumber.ToString()]["name"].ToString();
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
