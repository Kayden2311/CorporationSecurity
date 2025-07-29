using System.Windows;
using System.Linq;
using CorporationSecurity.Models;

namespace CorporationSecurity
{
    public partial class AssetDetailWindow : Window
    {
        public int AssetId { get; private set; }
        public string AssetName => NameBox.Text.Trim();
        public int? CategoryId => CategoryBox.SelectedValue as int?;
        public string Description => DescriptionBox.Text.Trim();

        public AssetDetailWindow(Asset asset)
        {
            InitializeComponent();
            AssetId = asset.Id;
            using (var context = new CorporationSecurityContext())
            {
                var categories = context.Categories.Select(c => new { c.Id, c.Name }).ToList();
                CategoryBox.ItemsSource = categories;
                CategoryBox.DisplayMemberPath = "Name";
                CategoryBox.SelectedValuePath = "Id";
            }
            NameBox.Text = asset.Name;
            CategoryBox.SelectedValue = asset.CategoryId;
            DescriptionBox.Text = asset.Description;
            CreatedDateBox.Text = asset.CreatedDate.ToString("dd/MM/yyyy");
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