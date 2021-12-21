using DiscordRPC;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ChromeRPC
{
    public partial class MainForm : Form
    {
        private void processRPCRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/")
            {
                try
                {
                    string rawData;
                    using (StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8))
                    {
                        rawData = reader.ReadToEnd();
                    }
                    RPCRequestData rpc = JsonConvert.DeserializeObject<RPCRequestData>(rawData);
                    RichPresence rp = RPCClient.createRP(rpc);
                    if (rp != null)
                    {
                        Program.rpcClient.updateRP(rp);
                    }
                    else
                    {
                        Program.rpcClient.clearRP();
                    }
                    context.Response.StatusCode = 200;
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    appendOutput("處理請求時發生例外狀況 : " + ex.Message);
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }

        private void HTTPServerStatusChange(HTTPServer.ServerStatus status)
        {
            switch (status)
            {
                case HTTPServer.ServerStatus.START:
                    {
                        Program.rpcClient.start();
                        break;
                    }
                case HTTPServer.ServerStatus.STOP:
                    {
                        Program.rpcClient.disconnect();
                        break;
                    }
            }
        }

        private void registerHTTPServerEvent()
        {
            HTTPServer.processEvent += processRPCRequest;
            HTTPServer.statusEvent += HTTPServerStatusChange;
            HTTPServer.logEvent += appendOutput;
        }

        public MainForm()
        {
            InitializeComponent();
            registerHTTPServerEvent();
        }

        private void start()
        {
            HTTPServer.startHTTPServer();
        }

        public void stop()
        {
            HTTPServer.stopHTTPServer();
        }

        public void appendOutput(string message)
        {
            Invoke((MethodInvoker)delegate ()
            {
                txtOutput.AppendText(message + Environment.NewLine);
            });
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            Program.rpcClient.invoke();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop();
        }

        public void toggleUIState(bool enable)
        {
            Invoke((MethodInvoker)delegate ()
            {
                btnControl.Enabled = enable;
            });
        }

        private void btnControl_Click(object sender, EventArgs e)
        {
            btnControl.Enabled = false;
            switch (btnControl.Text)
            {
                case "啟動":
                    {
                        btnControl.Text = "停止";
                        start();
                        break;
                    }
                case "停止":
                    {
                        btnControl.Text = "啟動";
                        stop();
                        break;
                    }
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                niIcon.Visible = true;

                niIcon.BalloonTipText = "Chrome RPC 仍在執行中";
                niIcon.ShowBalloonTip(3000);
            }
        }

        private void niIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            niIcon.Visible = false;
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}