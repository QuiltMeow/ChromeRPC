using System.Text;

namespace ChromeRPC
{
    public class RPCRequestData
    {
        public string action
        {
            get;
            set;
        }

        public string host
        {
            get;
            set;
        }

        public string title
        {
            get;
            set;
        }

        public string url
        {
            get;
            set;
        }

        public override string ToString()
        {
            return new StringBuilder().Append("動作 : ").AppendLine(action)
                .Append("主機 : ").AppendLine(host)
                .Append("標題 : ").AppendLine(title)
                .Append("網址 : ").AppendLine(url).ToString();
        }
    }
}