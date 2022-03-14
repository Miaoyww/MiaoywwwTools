﻿using MiaoywwwTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace RandomDrawLib
{
    public class RaDraw
    {
        public StreamReader reader;

        public void Read()
        {
            // 哦这该死的设定
            // 这里不得用 ./ca
            if (File.Exists("/Resources/Data/stdata.json"))
            {
                WinMessage winMessage = new WinMessage();
                winMessage.SetMessage("错误", "未找到/Resources/Data/stdata.json文件", "close", "yes");
                winMessage.ShowDialog();
            }
            else
            {
                // 这里的文件目录必须得用 ./
                reader = File.OpenText(@"./Resources/Data/stdata.json");
            }
        }

        public JObject GetStdataContent()
        {
            return (JObject)JToken.ReadFrom(new JsonTextReader(reader));
        }

        public string[]? GetRandomResult()
        {
            if (Read == null)
            {
                WinMessage winMessage = new WinMessage();
                winMessage.SetMessage("调试", "先使用Read", "close", "yes");
                winMessage.ShowDialog();
                return null;
            }
            else
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                if (jsonObject != null)
                {
                    Random rd = new Random();
                    int randomtimes = jsonObject.Count;

                    int[] randomNumberList = new int[randomtimes];
                    string[,] std = new string[randomtimes, 2];
                    for (int i = 0; i < randomNumberList.Length; i++)
                    {
                        std[i, 0] = jsonObject[i.ToString()]["name"].ToString();
                        std[i, 1] = jsonObject[i.ToString()]["grade"].ToString(); ;
                    }
                    // 通过减半冒泡达到初步打乱的效果
                    for (int i = 0; i < randomNumberList.Length / 2; i++)
                    {
                        for (int j = 0; j < randomNumberList.Length / 2 - i - 1; j++)
                        {
                            if (int.Parse(std[j, 1]) > int.Parse(std[j + 1, 1]))
                            {
                                // 2022/3/14 bug，导致人名不对成绩，因为仅将成绩更换了
                                string[] temp0 = new string[2];  // 数据更换的第一方
                                string[] temp1 = new string[2];  // 数据更换的第二方
                                temp0[0] = std[j, 0];  // 人名
                                temp0[1] = std[j, 1]; // 成绩

                                temp1[0] = std[j + 1, 0];
                                temp1[1] = std[j + 1, 1];

                                std[j, 0] = temp1[0];
                                std[j, 1] = temp1[1];

                                std[j + 1, 0] = temp0[0];
                                std[j + 1, 1] = temp0[1];
                            }
                        }
                    }
                    for (int i = 0; i < randomtimes; i++)
                    {
                        randomNumberList[i] = rd.Next(jsonObject.Count);
                    }
                    int selectnumber = randomNumberList[rd.Next(randomNumberList.Length)];
                    string[] result = new string[3];
                    result[0] = std[selectnumber, 0].ToString();
                    result[1] = std[selectnumber, 1].ToString();
                    result[2] = selectnumber.ToString();
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public int GetStdataLentgh()
        {
            using (StreamReader reader = File.OpenText(@"./Resources/Data/stdata.json"))
            {
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
}