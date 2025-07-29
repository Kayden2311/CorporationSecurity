using System.Windows;
using CorporationSecurity.Models;
using System;
using System.Linq;

namespace CorporationSecurity
{
    public partial class EditRiskWindow : Window
    {
        private readonly int _riskId;
        private readonly int _userId;
        public EditRiskWindow(int riskId, int userId)
        {
            InitializeComponent();
            _riskId = riskId;
            _userId = userId;
            using (var context = new CorporationSecurityContext())
            {
                var categories = context.RiskCategories.Select(rc => new { rc.Id, rc.Name }).ToList();
                RiskCategoryBox.ItemsSource = categories;
                RiskCategoryBox.DisplayMemberPath = "Name";
                RiskCategoryBox.SelectedValuePath = "Id";
                var risk = context.Risks.FirstOrDefault(r => r.Id == _riskId);
                if (risk != null)
                {
                    DescriptionBox.Text = risk.Description;
                    ImpactBox.Text = risk.Impact.ToString();
                    LikelihoodBox.Text = risk.Likelihood.ToString();
                    MitigationBox.Text = risk.Mitigation;
                    RiskCategoryBox.SelectedValue = risk.RiskCategoryId;
                }
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
                var risk = context.Risks.FirstOrDefault(r => r.Id == _riskId);
                if (risk != null)
                {
                    risk.RiskCategoryId = riskCategoryId;
                    risk.Description = desc;
                    risk.Impact = impact;
                    risk.Likelihood = likelihood;
                    risk.Mitigation = mitigation;
                    context.SaveChanges();
                    // AuditLog
                    var auditLog = new AuditLog
                    {
                        UserId = _userId,
                        Action = $"Edited Risk: {desc} (Id: {_riskId})"
                    };
                    context.AuditLogs.Add(auditLog);
                    context.SaveChanges();
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