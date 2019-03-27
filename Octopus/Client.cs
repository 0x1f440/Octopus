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

        private delegate void SafeCallDelegate(string text);

        public TcpClient client;
        string channel;


        public void StartClient(string name)
        {
            Instance = this;

            ConnectToServer();

            Task login = Task.Run(() => Login(name));
            login.Wait();

            StartReceivingFromServer();
        }

        private void ConnectToServer()
        {
            client = new TcpClient();
            client.Connect("irc.ozinger.org", 6666);

            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        private void Login(string name)
        {
            SendRawToServer("NICK " + name);
            SendRawToServer("USER " + name + " " + name + " irc.ozinger.org :realname");

            ReceiveFromServer();
            ReceiveFromServer();
        }

        private void StartReceivingFromServer()
        {
            var receiveThread = new Thread(new ThreadStart(ReceiveFromServerThread));
            receiveThread.Start();
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
			string s;
            while (true)
			{
				s = ReceiveFromServer();

				if (s.Length > 0){
					Application.Current.Dispatcher.Invoke(() => {
						MainWindow.Instance.AppendToChatbox(s);
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
