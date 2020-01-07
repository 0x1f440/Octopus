using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace Octopus
{
    class Client
    {
        NetworkStream stream;
        StreamWriter writer;
        StreamReader reader;

        private delegate void SafeCallDelegate(string text);
        private static Client instance;
        public static Client Instance
        {
            get
            {
                if (instance == null)
                    instance = new Client();

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public TcpClient client;
        string channel, host, name;
        int port;


        public void StartClient(string server, string name)
        {
            Instance = this;

            ConnectToServer(server);

            Task login = Task.Run(() => Login(name));
            login.Wait();

            StartReceivingFromServer();
        }

        private void ConnectToServer(string server)
        {
            client = new TcpClient();

            string[] temp = server.Split('/',':','\\');
            host = temp[0];
            port = int.Parse(temp[1]);
            client.Connect(host, port);

            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        private void Login(string name)
        {
            SendRawToServer("NICK " + name);
            SendRawToServer("USER " + name + " " + name + " " + host + " :realname");
            this.name = name;

            ReceiveFromServer();
            ReceiveFromServer();
        }

        private void StartReceivingFromServer()
        {
            var receiveThread = new Thread(new ThreadStart(ReceiveFromServerThread));
            receiveThread.Start();
            MainWindow.Instance.ChangeNickname(name);
        }

        private string ReceiveFromServer()
        {
            byte[] buffer = new byte[1024];
            int dataLength;
            string output;

            dataLength = stream.Read(buffer, 0, buffer.Length);
            output = Encoding.UTF8.GetString(buffer, 0, dataLength);
			Console.WriteLine(output);

			if (output.Split()[0] == "PING"){
				RespondToPing(output);
				return "";
			}

            return output;
        }

        private void ReceiveFromServerThread()
        {
			string[] s;
            while (true)
			{
				s = ReceiveFromServer().Split(new string[] { "PRIVMSG" }, StringSplitOptions.None);

				if (s.Length > 0 && s.Length == 2)
				{
					string name = s[0].Split('!')[0].Substring(1);
					string[] temp = s[1].Split(new char[] { ':' }, 2);
					string channel = temp[0];
					string msg = temp[1];

					Application.Current.Dispatcher.Invoke(() => {
						MainWindow.Instance.AppendToChatbox(name, channel, msg);
					});
					
				}
				
                Thread.Sleep(1);
            }
        }

        private void SendRawToServer(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        public void SendToServer(string message)
        {
            writer.WriteLine(CheckCommands(message));
            writer.Flush();
        }

        private string CheckCommands(string message)
        {
            string result;

            if (message[0] == '/')
            {
                string command = message.Split(' ')[0];

                switch (command.ToUpper())
                {
                    case "/JOIN":
                        result = JoinChannel(message.Split(' ')[1]);
                        break;

                    default:
                        result = "There's no such command...";
                        break;
                }
            }
            else
            {
                result = "PRIVMSG " + channel + " " + message;
            }
            return result;
        }

        private string JoinChannel(string channelName)
        {
            channel = channelName;
            MainWindow.Instance.ChangeChannelName(channelName);

            return "JOIN " + channelName;
        }

        private void RespondToPing(string pingMessage)
        {
            SendRawToServer("PONG " + pingMessage.Split('\n')[0].Substring(5));
			Console.WriteLine("Pong sent");
        }
    }

}
