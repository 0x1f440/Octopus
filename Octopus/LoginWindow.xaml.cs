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
using System.Windows.Shapes;

namespace Octopus
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            loginButton.Click += Login;
        }
       
        private void Login(object sender, EventArgs e)
        {
			Dispatcher.Invoke(() =>
			{
				Client.Instance.StartClient(serverBox.Text, nicknameBox.Text);
			});

            Close();
        }
    }
}
