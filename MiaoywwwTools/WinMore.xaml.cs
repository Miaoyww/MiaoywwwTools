using System;
using System.Windows.Controls;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinMore.xaml 的交互逻辑
    /// </summary>
    public partial class WinMore : Page
    {
        public WinMore()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 切换窗口
        /// </summary>
        /// <param name="pagename"> 页面名称 </param>
        public void ChangePage(string pagename)
        {
            Type pageType = Type.GetType(pagename);
            if (pageType != null)
            {
                this.NestPage.Content = new Frame()
                {
                    Content = Activator.CreateInstance(pageType)
                };
            }
        }

        // 点名器
        private void Btn_RandomDraw_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.ToolsRr");
        }
    }
}
