using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using CorporationSecurity.Models;
using Microsoft.EntityFrameworkCore;

namespace CorporationSecurity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            ErrorTextBlock.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Please enter both username and password.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            using (var context = new CorporationSecurityContext())
            {
                // For demo: compare plain text password (replace with hash check in production)
                var user = context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Username == username && u.PasswordHash == password && u.IsActive);
                if (user != null)
                {
                    Homepage homepage = new Homepage(user);
                    homepage.Show();
                    this.Close();
                }
                else
                {
                    ErrorTextBlock.Text = "Invalid username or password.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
            }
        }
    }
}