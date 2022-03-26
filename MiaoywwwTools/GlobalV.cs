using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiaoywwwTools;
using MiaoywwwTools.Tools;
using MiaoywwwTools.Tools.RandomDraw;

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
    }
}
