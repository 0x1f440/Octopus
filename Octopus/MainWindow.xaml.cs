using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Octopus
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public static MainWindow Instance;
        private TextBox chatInputBox;
        private TextBlock chatContentBox;

		public MainWindow()
		{
			InitializeComponent();
		    Instance = this;

            chatInputBox = inputBox;
            chatContentBox = chattingLog;

            chatInputBox.KeyUp += CheckEnterPressed;

            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            Left = 0;
            Top = 0;
            WindowState = WindowState.Normal;

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void CheckEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
        }

        public void ChangeChannelName(string name)
        {
            channelName.Text = name;
            chatInputBox.Clear();
        }

        public void ChangeNickname(string name)
        {
            nickname.Content = name;
        }

        private void SendMessage()
        {
            Client.Instance.SendToServer(chatInputBox.Text);
			AppendMessage("→ " + nickname.Content.ToString(), chatInputBox.Text); 
            chatInputBox.Clear();
        }

		private void AppendMessage(string name, string message){
			AppendToChatbox(name, " " + channelName.Text.ToString() ,chatInputBox.Text);
		}

        public void AppendToChatbox(string name, string channel, string message)
        {
			if(message != "")
            {
				if (!message.EndsWith("\n"))
					message += '\n';
				chatContentBox.Text += name + channel +" :: " + message;
                chatScrollViewer.ScrollToBottom();
            }
        }
    }
}
