using SkinSharp;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ChromeRPC
{
    public static class Program
    {
        public static MainForm form
        {
            get;
            private set;
        }

        public static RPCClient rpcClient
        {
            get;
            private set;
        }

        private static SkinH_Net skin;

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                skin = new SkinH_Net();
                skin.AttachEx("Skin.she", "");
            }
            catch
            {
            }

            string name = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(name).Length > 1)
            {
                MessageBox.Show("已經有其他程式實例正在執行", "資訊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            rpcClient = new RPCClient();
            form = new MainForm();
            Application.Run(form);
        }
    }
}