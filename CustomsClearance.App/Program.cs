using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomsClearance.Utils;

namespace CustomsClearance.App
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Messagers.Default.Register(MessageNames.HttpMessage,new Messager<HttpMessage>());
            Application.Run(new Form1());
        }
    }
}
