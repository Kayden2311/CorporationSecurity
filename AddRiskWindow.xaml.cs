using System.Windows;
using CorporationSecurity.Models;
using System;
using System.Linq;

namespace CorporationSecurity
{
    public partial class AddRiskWindow : Window
    {
        private readonly int _assetId;
        private readonly int _userId;
        public AddRiskWindow(int assetId, int userId)
        {
            InitializeComponent();
            _assetId = assetId;
            _userId = userId;
            using (var context = new CorporationSecurityContext())
            {
                var categories = context.RiskCategories.Select(rc => new { rc.Id, rc.Name }).ToList();
                RiskCategoryBox.ItemsSource = categories;
                RiskCategoryBox.DisplayMemberPath = "Name";
                RiskCategoryBox.SelectedValuePath = "Id";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string desc = DescriptionBox.Text.Trim();
            string mitigation = MitigationBox.Text.Trim();
            if (RiskCategoryBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a risk category.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int riskCategoryId = (int)RiskCategoryBox.SelectedValue;
            if (string.IsNullOrWhiteSpace(desc) ||
                !double.TryParse(ImpactBox.Text, out double impact) || impact < 0 || impact > 1 ||
                !double.TryParse(LikelihoodBox.Text, out double likelihood) || likelihood < 0 || likelihood > 1)
            {
                MessageBox.Show("Please enter valid risk information. Impact and Likelihood must be between 0 and 1.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new CorporationSecurityContext())
            {
                var risk = new Risk
                {
                    AssetId = _assetId,
                    RiskCategoryId = riskCategoryId,
                    Description = desc,
                    Impact = impact,
                    Likelihood = likelihood,
                    Mitigation = mitigation,
                    CreatedBy = _userId
                };
                context.Risks.Add(risk);
                context.SaveChanges();
                // AuditLog
                var auditLog = new AuditLog
                {
                    UserId = _userId,
                    Action = $"Added Risk: {desc} for AssetId: {_assetId}"
                };
                context.AuditLogs.Add(auditLog);
                context.SaveChanges();
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