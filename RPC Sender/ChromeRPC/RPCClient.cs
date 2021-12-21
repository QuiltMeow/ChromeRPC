using DiscordRPC;
using DiscordRPC.Message;
using System;
using System.Text;

namespace ChromeRPC
{
    public class RPCClient
    {
        public const int DISCORD_PIPE = -1;
        public const string CLIENT_ID = "691118728695251035";

        private static readonly DiscordRPC.Logging.LogLevel logLevel;

        private DiscordRpcClient rpc;
        private RichPresence current;

        public bool isRunning
        {
            get;
            private set;
        }

        static RPCClient()
        {
            logLevel = DiscordRPC.Logging.LogLevel.Trace;
        }

        public void start()
        {
            isRunning = true;
            rpc = new DiscordRpcClient(CLIENT_ID, DISCORD_PIPE, new DiscordRPC.Logging.ConsoleLogger(logLevel, true), false);

            rpc.OnReady += onReady;
            rpc.OnClose += onClose;
            rpc.OnError += onError;
            rpc.OnConnectionEstablished += onConnectionEstablished;
            rpc.OnConnectionFailed += onConnectionFailed;
            rpc.OnPresenceUpdate += onPresenceUpdate;

            rpc.Initialize();
            Program.form.toggleUIState(true);
        }

        public void updateRP(RichPresence rp)
        {
            if (isRunning && rpc != null)
            {
                current = rp;
                rpc.SetPresence(current);
            }
        }

        public void clearRP()
        {
            if (isRunning && rpc != null)
            {
                current = null;
                rpc.ClearPresence();
            }
        }

        public void disconnect()
        {
            if (isRunning && rpc != null)
            {
                Program.form.appendOutput("[操作] 正在關閉 RPC 客戶端");
                rpc.Dispose();
                isRunning = false;
            }
            Program.form.toggleUIState(true);
        }

        public void invoke()
        {
            if (isRunning && rpc != null)
            {
                rpc.Invoke();
            }
        }

        private void onReady(object sender, ReadyMessage args)
        {
            Program.form.appendOutput("[事件] Discord 準備完成 RPC 版本 : " + args.Version);
            Program.form.appendOutput("[資訊] 正在顯示目標帳號 " + args.User.Username + "#" + args.User.Discriminator + " 之自訂狀態");
        }

        private void onClose(object sender, CloseMessage args)
        {
            Program.form.appendOutput("[事件] Discord 中斷連線 (" + args.Code + ") : " + args.Reason);
        }

        private void onError(object sender, ErrorMessage args)
        {
            Program.form.appendOutput("[事件] Discord 發生錯誤 (" + args.Code + ") : " + args.Message);
            Program.form.stop();
        }

        private void onConnectionEstablished(object sender, ConnectionEstablishedMessage args)
        {
            Program.form.appendOutput("[事件] 管線連線已建立 有效管線 #" + args.ConnectedPipe);
        }

        private void onConnectionFailed(object sender, ConnectionFailedMessage args)
        {
            Program.form.appendOutput("[事件] 管線連線失敗 無法連線至管線 #" + args.FailedPipe);
            Program.form.stop();
        }

        private void onPresenceUpdate(object sender, PresenceMessage args)
        {
            Program.form.appendOutput(Environment.NewLine + "[自訂狀態更新]");
            if (current != null)
            {
                Program.form.appendOutput(getRPStatus(current) + Environment.NewLine);
            }
            else
            {
                Program.form.appendOutput("清除狀態");
            }
        }

        public static string getRPStatus(RichPresence rp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-> 詳細資訊 : ").AppendLine(rp.Details);
            sb.Append("-> 狀態 : ").AppendLine(rp.State);

            sb.Append("-> 大圖示 : ").Append(rp.Assets.LargeImageKey).Append(" 說明 : ").AppendLine(rp.Assets.LargeImageText);
            sb.Append("-> 小圖示 : ").Append(rp.Assets.SmallImageKey).Append(" 說明 : ").Append(rp.Assets.SmallImageText);
            return sb.ToString();
        }

        public static RichPresence createRP(RPCRequestData data)
        {
            if (data.action == "clear")
            {
                return null;
            }

            RichPresence rp = new RichPresence()
            {
                Details = data.host.Substring(0, data.host.Length > 128 ? 128 : data.host.Length),
                State = data.title.Substring(0, data.title.Length > 128 ? 128 : data.title.Length),
                Assets = new Assets()
                {
                    LargeImageKey = "chrome",
                    LargeImageText = "Chrome 瀏覽器",
                    SmallImageKey = "meow",
                    SmallImageText = "喵嗚"
                }
            };
            return rp;
        }
    }
}