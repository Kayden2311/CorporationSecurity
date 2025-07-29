using System;
using System.Windows;
using System.Windows.Controls;
using CorporationSecurity.Models;
using Microsoft.EntityFrameworkCore;

namespace CorporationSecurity
{
    public partial class AddControlWindow : Window
    {
        public string Description { get; private set; }
        public string Effectiveness { get; private set; }
        public string Notes { get; private set; }
        
        private int _riskId;
        private int _assetId;
        private string _assetName;
        private string _riskDescription;

        public AddControlWindow(int riskId, int assetId, string assetName, string riskDescription)
        {
            InitializeComponent();
            _riskId = riskId;
            _assetId = assetId;
            _assetName = assetName;
            _riskDescription = riskDescription;
            
            // Set the read-only fields
            AssetTextBox.Text = _assetName;
            RiskTextBox.Text = _riskDescription;
            
            // Set focus to description field
            DescriptionTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Please enter a control description.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return;
            }

            if (EffectivenessComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an effectiveness status.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EffectivenessComboBox.Focus();
                return;
            }

            try
            {
                // Get the selected effectiveness value
                var selectedItem = EffectivenessComboBox.SelectedItem as ComboBoxItem;
                Effectiveness = selectedItem?.Content?.ToString() ?? "Pending";
                
                // Set properties
                Description = DescriptionTextBox.Text.Trim();
                Notes = NotesTextBox.Text?.Trim() ?? "";

                // Set dialog result to true (success)
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving control: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 