using System.Windows;
using System.Linq;
using CorporationSecurity.Models;

namespace CorporationSecurity
{
    public partial class AddAssetWindow : Window
    {
        public string AssetName => NameBox.Text.Trim();
        public int? CategoryId => CategoryBox.SelectedValue as int?;
        public string Description => DescriptionBox.Text.Trim();

        public AddAssetWindow()
        {
            InitializeComponent();
            using (var context = new CorporationSecurityContext())
            {
                var categories = context.Categories.Select(c => new { c.Id, c.Name }).ToList();
                CategoryBox.ItemsSource = categories;
                CategoryBox.DisplayMemberPath = "Name";
                CategoryBox.SelectedValuePath = "Id";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AssetName) || CategoryId == null)
            {
                MessageBox.Show("Please enter asset name and select a category.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
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