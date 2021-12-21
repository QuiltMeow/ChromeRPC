using System;
using System.Net;
using System.Threading;

namespace ChromeRPC
{
    public class HTTPServer
    {
        public delegate void ServerLogEventHandler(string log);

        public static event ServerLogEventHandler logEvent;

        public delegate void ServerStatusEventHandler(ServerStatus status);

        public static event ServerStatusEventHandler statusEvent;

        public delegate void ServerProcessEventHandler(HttpListenerContext context);

        public static event ServerProcessEventHandler processEvent;

        public enum ServerStatus
        {
            START,
            ERROR,
            STOP
        }

        private static HTTPServer current;

        private int _port;
        private HttpListener _listener;
        private Thread _serverThread;

        private HTTPServer(int port)
        {
            logEvent?.Invoke("正在初始化伺服器 ...");
            _port = port;
            _serverThread = new Thread(new ThreadStart(listen));
            _serverThread.Start();
        }

        private void listen()
        {
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
                _listener.Start();
            }
            catch (Exception ex)
            {
                logEvent?.Invoke("伺服器啟動失敗 : " + ex);
                statusEvent?.Invoke(ServerStatus.ERROR);
                stopHTTPServer();
                return;
            }

            logEvent?.Invoke("伺服器已啟動 端口 : " + _port + " 準備接受請求");
            statusEvent?.Invoke(ServerStatus.START);
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    process(context);
                }
                catch
                {
                }
            }
        }

        private void process(HttpListenerContext context)
        {
            processEvent?.Invoke(context);
            context.Response.OutputStream.Close();
        }

        public static void startHTTPServer(int port = 8092)
        {
            if (current != null)
            {
                stopHTTPServer();
            }
            current = new HTTPServer(port);
        }

        private void stop()
        {
            try
            {
                _listener.Stop();
            }
            catch
            {
            }

            try
            {
                _serverThread.Abort();
                while (_serverThread.ThreadState != System.Threading.ThreadState.Aborted)
                {
                    Thread.Sleep(100);
                }
            }
            catch
            {
            }
            logEvent?.Invoke("伺服器已關閉");
        }

        public static void stopHTTPServer()
        {
            if (current != null)
            {
                logEvent?.Invoke("正在關閉伺服器");
                current.stop();
                current = null;
            }
            statusEvent?.Invoke(ServerStatus.STOP);
        }
    }
}