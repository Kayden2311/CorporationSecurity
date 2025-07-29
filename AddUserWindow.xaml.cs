using System.Windows;
using System.Windows.Controls;
using CorporationSecurity.Models;
using System.Linq;

namespace CorporationSecurity
{
    public partial class AddUserWindow : Window
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public int RoleId { get; private set; }
        public bool IsActive { get; private set; }
        public AddUserWindow()
        {
            InitializeComponent();
            using (var context = new CorporationSecurityContext())
            {
                var roles = context.Roles.ToList();
                RoleBox.ItemsSource = roles;
                RoleBox.DisplayMemberPath = "Name";
                RoleBox.SelectedValuePath = "Id";
                RoleBox.SelectedIndex = 0;
            }
            IsActiveBox.IsChecked = true;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Username = UsernameBox.Text.Trim();
            PasswordHash = PasswordBox.Password.Trim();
            FirstName = FirstNameBox.Text.Trim();
            LastName = LastNameBox.Text.Trim();
            Email = EmailBox.Text.Trim();
            RoleId = (int)(RoleBox.SelectedValue ?? 0);
            IsActive = IsActiveBox.IsChecked == true;
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) || RoleId == 0)
            {
                MessageBox.Show("Please fill all fields.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new CorporationSecurityContext())
            {
                if (context.Users.Any(u => u.Username == Username))
                {
                    MessageBox.Show("Username already exists.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (context.Users.Any(u => u.Email == Email))
                {
                    MessageBox.Show("Email already exists.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            DialogResult = true;
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 