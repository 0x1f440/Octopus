using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void SendMessage()
        {
            Client.Instance.SendToServer(chatInputBox.Text);
            chatInputBox.Clear();
        }

        public void AppendToChatbox(string message)
        {
			if(message != "")
				chatContentBox.Text += message + '\n';
        }
    }
}
