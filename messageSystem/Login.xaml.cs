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

namespace messageSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

       

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {

            //Simple log in for prototype/testing

            if (txtPassword.Text == "password" && txtUsername.Text == "username")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
                   

            }
            else {

                MessageBox.Show("Error, username = username and password = password");

                }

        }
    }
}
