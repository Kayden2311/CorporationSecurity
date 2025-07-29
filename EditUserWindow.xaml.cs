using System.Windows;
using System.Windows.Controls;
using CorporationSecurity.Models;
using System.Linq;

namespace CorporationSecurity
{
    public partial class EditUserWindow : Window
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public int RoleId { get; private set; }
        public bool IsActive { get; private set; }
        public string PasswordHash { get; private set; }
        private User _user;
        public EditUserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            FirstNameBox.Text = user.FirstName;
            LastNameBox.Text = user.LastName;
            EmailBox.Text = user.Email;
            IsActiveBox.IsChecked = user.IsActive;
            PasswordHash = user.PasswordHash;
            PasswordBox.Text = user.PasswordHash;
            using (var context = new CorporationSecurityContext())
            {
                var roles = context.Roles.ToList();
                RoleBox.ItemsSource = roles;
                RoleBox.DisplayMemberPath = "Name";
                RoleBox.SelectedValuePath = "Id";
                RoleBox.SelectedValue = user.RoleId;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FirstName = FirstNameBox.Text.Trim();
            LastName = LastNameBox.Text.Trim();
            Email = EmailBox.Text.Trim();
            RoleId = (int)(RoleBox.SelectedValue ?? _user.RoleId);
            IsActive = IsActiveBox.IsChecked == true;
            if (!string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                PasswordHash = PasswordBox.Text.Trim();
            }
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email))
            {
                MessageBox.Show("Please fill all fields.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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