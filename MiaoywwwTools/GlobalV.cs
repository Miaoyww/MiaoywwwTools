using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiaoywwwTools;
using MiaoywwwTools.Tools;
using MiaoywwwTools.Tools.RandomDraw;
using Newtonsoft.Json.Linq;

namespace MiaoywwwTools
{
    static class GlobalV
    {
        public static bool MessageBoxHided;
        public static ShowResult showResult = new();
        public static bool Login;  // ToolsRr的Result窗口是否登录
        public static bool FaceChanged; // Home的头像是否已经修改
        public static bool CleanUpFace;    // 清除Home的头像
        public static bool AppRestart;  // 重启
        public static string AppVersion_ver;
        public static string AppVersion_time;
        public static JObject? Index_To_Hitokoto = new()
        {
            { "0", "random" },
            { "1", "a" },
            { "2", "b" },
            { "3", "c" },
            { "4", "d" },
            { "5", "e" },
            { "6", "f" },
            { "7", "g" },
            { "8", "h" },
            { "9", "i" },
            { "10", "j" },
            { "11", "k" },
            { "12", "l" },

        };
        public static JObject? Hitokoto_To_Index = new()
        {
            {"random", "0" },
            {"a", "1" },
            {"b", "2" },
            {"c", "3" },
            {"d", "4" },
            {"e", "5" },
            {"f", "6" },
            {"g", "7" },
            {"h", "8" },
            {"i", "9" },
            {"j", "10" },
            {"k", "11" },
            {"l", "12" },

        };
    }
}
