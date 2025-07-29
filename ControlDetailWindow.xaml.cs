using System.Windows;
using System.Windows.Controls;
using ModelControl = CorporationSecurity.Models.Control;

namespace CorporationSecurity
{
    public partial class ControlDetailWindow : Window
    {
        public string Description { get; private set; }
        public string Effectiveness { get; private set; }
        private ModelControl _control;
        public ControlDetailWindow(ModelControl control)
        {
            InitializeComponent();
            _control = control;
            DescriptionBox.Text = control.Description;
            EffectivenessBox.SelectedIndex = control.Effectiveness switch
            {
                "Pass" => 0,
                "Fail" => 1,
                "Pending" => 2,
                _ => 0
            };
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Description = DescriptionBox.Text.Trim();
            if (string.IsNullOrEmpty(Description) || EffectivenessBox.SelectedItem == null)
            {
                MessageBox.Show("Please enter description and select effectiveness.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Effectiveness = (EffectivenessBox.SelectedItem as ComboBoxItem)?.Content.ToString();
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