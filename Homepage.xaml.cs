using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CorporationSecurity.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CorporationSecurity
{
    public partial class Homepage : Window
    {
        public class SidebarItem
        {
            public string Icon { get; set; }
            public string Name { get; set; }
            public List<string> PermissionRoles { get; set; }
        }
        public class CategoryGroup
        {
            public string CategoryName { get; set; }
            public List<AssetCardModel> Assets { get; set; }
        }
        public class AssetCardModel
        {
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public System.DateTime CreatedDate { get; set; }
        }

        private List<CategoryGroup> _allGroups;
        private List<string> _allCategories;
        private List<SidebarItem> _allSidebarItems = new List<SidebarItem>
        {
            new SidebarItem { Icon = "📦", Name = "Assets", PermissionRoles = new List<string>{"System Administrator","Risk Manager","Operations Staff"} },
            new SidebarItem { Icon = "⚠️", Name = "Risks", PermissionRoles = new List<string>{"System Administrator","Risk Manager","Operations Staff"} },
            new SidebarItem { Icon = "🛡️", Name = "Controls", PermissionRoles = new List<string>{"System Administrator","Risk Manager","Operations Staff"} },
            // Đã xóa Asset Categories và Risk Categories
            new SidebarItem { Icon = "👤", Name = "Users", PermissionRoles = new List<string>{"System Administrator"} },
            new SidebarItem { Icon = "📜", Name = "Audit Logs", PermissionRoles = new List<string>{"System Administrator"} },
            // Đã xóa Roles
        };

        public User CurrentUser { get; set; }

        public Homepage(User user)
        {
            InitializeComponent();
            Loaded += Homepage_Loaded;
            CurrentUser = user;
            // Sidebar permission logic
            string roleName = user?.Role?.Name ?? "";
            var sidebarItems = _allSidebarItems.Where(item => item.PermissionRoles.Contains(roleName)).ToList();
            SidebarPanel.ItemsSource = sidebarItems;
            SidebarPanel.MouseLeftButtonUp += SidebarPanel_MouseLeftButtonUp;
            this.DataContext = this;
        }

        private void Homepage_Loaded(object sender, RoutedEventArgs e)
        {
            // Show default instruction with icon
            var stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new System.Windows.Thickness(0, 120, 0, 0)
            };
            var icon = new TextBlock
            {
                Text = "💡",
                FontSize = 48,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new System.Windows.Thickness(0, 0, 0, 8)
            };
            var instruction = new TextBlock
            {
                Text = "Select a service to begin",
                FontSize = 20,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stack.Children.Add(icon);
            stack.Children.Add(instruction);
            MainContentArea.Content = stack;
        }

        private void SidebarPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Find which sidebar item was clicked
            var fe = e.OriginalSource as FrameworkElement;
            if (fe?.DataContext is SidebarItem item)
            {
                if (item.Name == "Assets")
                {
                    ShowAssetContent();
                }
                else if (item.Name == "Risks")
                {
                    ShowRiskContent();
                }
                else if (item.Name == "Controls")
                {
                    ShowControlContent();
                }
                else if (item.Name == "Users")
                {
                    ShowUserContent();
                }
                else if (item.Name == "Audit Logs")
                {
                    ShowAuditLogsContent();
                }
                else
                {
                    MainContentArea.Content = new TextBlock
                    {
                        Text = $"{item.Name} feature coming soon...",
                        FontSize = 18,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                        FontWeight = FontWeights.SemiBold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new System.Windows.Thickness(0, 120, 0, 0)
                    };
                }
            }
        }

        private void ShowAssetContent()
        {
            // Create a grid with two rows: filter (fixed), asset content (scrollable)
            var grid = new Grid { Margin = new System.Windows.Thickness(32, 32, 32, 32) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var filterBox = new ComboBox { Width = 220, FontSize = 15, Margin = new System.Windows.Thickness(0, 0, 0, 18) };
            filterBox.SelectionChanged += AssetFilterBox_SelectionChanged;
            Grid.SetRow(filterBox, 0);
            grid.Children.Add(filterBox);

            var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
            Grid.SetRow(scroll, 1);
            grid.Children.Add(scroll);

            // Content inside scroll: icon + add button + asset cards
            var scrollGrid = new Grid();
            scrollGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            scrollGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            scroll.Content = scrollGrid;

            // Row 0: icon + add button
            var iconRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 12), VerticalAlignment = VerticalAlignment.Center };
            var icon = new TextBlock { Text = "💻", FontSize = 32, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), VerticalAlignment = VerticalAlignment.Center };
            iconRow.Children.Add(icon);
            var addBtn = new Button
            {
                Content = new TextBlock { Text = "Add Asset", FontSize = 15, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff")), HorizontalAlignment = HorizontalAlignment.Center, TextAlignment = TextAlignment.Center },
                MinWidth = 120
            };
            addBtn.Style = (Style)this.FindResource("GreenRoundedButton");
            addBtn.Click += (s, e) => {
                var addWindow = new AddAssetWindow { Owner = this };
                if (addWindow.ShowDialog() == true)
                {
                    using (var context = new CorporationSecurityContext())
                    {
                        var asset = new Asset
                        {
                            Name = addWindow.AssetName,
                            CategoryId = addWindow.CategoryId.Value,
                            Description = addWindow.Description,
                            CreatedBy = CurrentUser.Id
                        };
                        context.Assets.Add(asset);
                        context.SaveChanges();
                        // Ghi AuditLog cho hành động tạo asset
                        var auditLog = new AuditLog
                        {
                            UserId = CurrentUser.Id,
                            Action = $"Created Asset: {asset.Name}"
                        };
                        context.AuditLogs.Add(auditLog);
                        context.SaveChanges();
                    }
                    // Reload asset list
                    ShowAssetContent();
                }
            };
            iconRow.Children.Add(addBtn);
            Grid.SetRow(iconRow, 0);
            scrollGrid.Children.Add(iconRow);

            var assetPanel = new ItemsControl();
            var itemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
            assetPanel.ItemsPanel = itemsPanel;
            assetPanel.ItemTemplate = (DataTemplate)FindResource("AssetCardTemplate");
            assetPanel.Margin = new System.Windows.Thickness(0, 8, 0, 0);
            assetPanel.ItemContainerStyle = new Style(typeof(ContentPresenter));
            assetPanel.ItemContainerStyle.Setters.Add(new EventSetter(UIElement.MouseLeftButtonUpEvent, new System.Windows.Input.MouseButtonEventHandler(AssetCard_Click)));
            Grid.SetRow(assetPanel, 1);
            scrollGrid.Children.Add(assetPanel);

            MainContentArea.Content = grid;

            using (var context = new CorporationSecurityContext())
            {
                var assets = context.Assets
                    .Select(a => new AssetCardModel
                    {
                        Name = a.Name,
                        CategoryName = a.Category != null ? a.Category.Name : "Uncategorized",
                        Description = a.Description,
                        CreatedDate = a.CreatedDate
                    })
                    .ToList();

                _allGroups = assets
                    .GroupBy(a => a.CategoryName)
                    .Select(g => new CategoryGroup
                    {
                        CategoryName = g.Key,
                        Assets = g.ToList()
                    })
                    .OrderBy(g => g.CategoryName)
                    .ToList();

                _allCategories = _allGroups.Select(g => g.CategoryName).OrderBy(x => x).ToList();
                filterBox.ItemsSource = new List<string> { "All" }.Concat(_allCategories).ToList();
                filterBox.SelectedIndex = 0;

                // Show all assets grouped by category
                ShowAssetsGroupedByCategory(assetPanel);
            }

            filterBox.Tag = assetPanel;
        }

        private void ShowAssetsGroupedByCategory(ItemsControl assetPanel)
        {
            // Flatten all assets for 'All' view
            assetPanel.ItemsSource = _allGroups.SelectMany(g => g.Assets).ToList();
        }

        private void ShowAssetsForCategory(ItemsControl assetPanel, string category)
        {
            var group = _allGroups.FirstOrDefault(g => g.CategoryName == category);
            if (group != null)
            {
                assetPanel.ItemsSource = group.Assets;
            }
            else
            {
                assetPanel.ItemsSource = null;
            }
        }

        private void AssetFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filterBox = sender as ComboBox;
            var assetPanel = filterBox?.Tag as ItemsControl;
            if (filterBox == null || assetPanel == null) return;
            string selected = filterBox.SelectedItem?.ToString();
            if (selected == "All")
            {
                ShowAssetsGroupedByCategory(assetPanel);
            }
            else
            {
                ShowAssetsForCategory(assetPanel, selected);
            }
        }

        private void AssetCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var cp = sender as ContentPresenter;
            if (cp?.Content is AssetCardModel card)
            {
                using (var context = new CorporationSecurityContext())
                {
                    var asset = context.Assets.Include("Category").Include("Risks").FirstOrDefault(a => a.Name == card.Name && a.Category.Name == card.CategoryName);
                    if (asset != null)
                    {
                        ShowAssetDetailPanel(asset);
                    }
                }
            }
        }

        // Thêm các trường để quản lý trạng thái UI Risk inline
        private int? editingRiskId = null;
        private bool isAddingRisk = false;

        private void ShowRiskContent()
        {
            var grid = new Grid { Margin = new System.Windows.Thickness(32, 32, 32, 32), HorizontalAlignment = HorizontalAlignment.Stretch, Width = double.NaN, MaxWidth = 1000 };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Filters
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Search
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Risk list

            // Row 0: Filters
            var filterPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0,0,0,12) };
            var categoryBox = new ComboBox { Width = 200, Margin = new System.Windows.Thickness(0,0,12,0) };
            var assetBox = new ComboBox { Width = 200, Margin = new System.Windows.Thickness(0,0,12,0) };
            using (var context = new CorporationSecurityContext())
            {
                var categories = context.RiskCategories.Select(c => new { Id = c.Id, Name = c.Name }).ToList<object>();
                var catList = new List<object> { new { Id = 0, Name = "All Categories" } };
                catList.AddRange(categories);
                categoryBox.ItemsSource = catList;
                categoryBox.DisplayMemberPath = "Name";
                categoryBox.SelectedValuePath = "Id";
                categoryBox.SelectedValue = 0;
                var assets = context.Assets.Select(a => new { Id = a.Id, Name = a.Name }).ToList<object>();
                var assetList = new List<object> { new { Id = 0, Name = "All Assets" } };
                assetList.AddRange(assets);
                assetBox.ItemsSource = assetList;
                assetBox.DisplayMemberPath = "Name";
                assetBox.SelectedValuePath = "Id";
                assetBox.SelectedValue = 0;
            }
            filterPanel.Children.Add(categoryBox);
            filterPanel.Children.Add(assetBox);
            Grid.SetRow(filterPanel, 0);
            grid.Children.Add(filterPanel);

            // Row 1: Search box
            var searchPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0,0,0,18) };
            var searchBox = new TextBox { Width = 220, FontSize = 14, VerticalContentAlignment = VerticalAlignment.Center };
            searchPanel.Children.Add(searchBox);
            Grid.SetRow(searchPanel, 1);
            grid.Children.Add(searchPanel);

            // Row 3: Risk list (StackPanel chứa các group)
            var riskScroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var riskStack = new StackPanel();
            riskScroll.Content = riskStack;
            Grid.SetRow(riskScroll, 3);
            grid.Children.Add(riskScroll);

            void LoadRisks()
            {
                using (var context = new CorporationSecurityContext())
                {
                    int selectedCat = categoryBox.SelectedValue is int catVal ? catVal : 0;
                    int selectedAsset = assetBox.SelectedValue is int assetVal ? assetVal : 0;
                    string keyword = searchBox.Text?.Trim().ToLower() ?? "";
                    var risks = context.Risks
                        .Include(r => r.Asset)
                        .Include(r => r.RiskCategory)
                        .Where(r => (selectedCat == 0 || r.RiskCategoryId == selectedCat) && (selectedAsset == 0 || r.AssetId == selectedAsset))
                        .Select(r => new {
                            r.Description,
                            r.Impact,
                            r.Likelihood,
                            r.Mitigation,
                            r.CreatedDate,
                            CategoryName = r.RiskCategory != null ? r.RiskCategory.Name : "(No Category)",
                            AssetName = r.Asset != null ? r.Asset.Name : "(No Asset)"
                        })
                        .ToList()
                        .Where(r => string.IsNullOrEmpty(keyword) ||
                            (r.Description?.ToLower().Contains(keyword) ?? false) ||
                            (r.AssetName?.ToLower().Contains(keyword) ?? false) ||
                            (r.Mitigation?.ToLower().Contains(keyword) ?? false))
                        .OrderBy(r => r.CategoryName)
                        .ThenBy(r => r.AssetName)
                        .ToList();

                    // Xóa các group cũ
                    riskStack.Children.Clear();

                    if (selectedCat == 0) // All Categories: show all risks in one WrapPanel, no headers
                    {
                        var itemsControl = new ItemsControl
                        {
                            ItemsSource = risks,
                            ItemTemplate = CreateRiskCardTemplate(),
                            MaxWidth = 2 * 340 + 32 // 2 cards + margin
                        };
                        var wrapPanelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                        wrapPanelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                        itemsControl.ItemsPanel = new ItemsPanelTemplate(wrapPanelFactory);
                        riskStack.Children.Add(itemsControl);
                    }
                    else // Specific category: group by category, show header
                    {
                        var grouped = risks.GroupBy(r => r.CategoryName).ToList();
                        foreach (var group in grouped)
                        {
                            // Header
                            var header = new TextBlock
                            {
                                Text = group.Key,
                                FontWeight = FontWeights.Bold,
                                FontSize = 15,
                                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                                Margin = new Thickness(0, 16, 0, 4),
                                Width = 700
                            };
                            riskStack.Children.Add(header);

                            // ItemsControl với WrapPanel cho risk card
                            var itemsControl = new ItemsControl
                            {
                                ItemsSource = group.ToList(),
                                ItemTemplate = CreateRiskCardTemplate()
                            };
                            var wrapPanelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                            wrapPanelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                            itemsControl.ItemsPanel = new ItemsPanelTemplate(wrapPanelFactory);
                            riskStack.Children.Add(itemsControl);
                        }
                    }
                }
            }
            categoryBox.SelectionChanged += (s, e) => LoadRisks();
            assetBox.SelectionChanged += (s, e) => LoadRisks();
            searchBox.TextChanged += (s, e) => LoadRisks();
            LoadRisks();

            MainContentArea.Content = grid;
        }

        // Helper tạo DataTemplate cho risk card
        private DataTemplate CreateRiskCardTemplate()
        {
            // Sử dụng style giống AssetCardTemplate
            var cardTemplate = new DataTemplate();
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.MarginProperty, new Thickness(16, 0, 16, 24));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.SetValue(Border.BackgroundProperty, Brushes.White);
            border.SetValue(Border.PaddingProperty, new Thickness(12));
            border.SetValue(Border.WidthProperty, 320.0);
            border.SetValue(Border.MinWidthProperty, 220.0);
            border.SetValue(Border.MaxWidthProperty, 340.0);

            var stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            var assetText = new FrameworkElementFactory(typeof(TextBlock));
            assetText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("AssetName") { StringFormat = "Asset: {0}" });
            assetText.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            assetText.SetValue(TextBlock.FontSizeProperty, 12.0);
            stack.AppendChild(assetText);

            var descText = new FrameworkElementFactory(typeof(TextBlock));
            descText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Description"));
            descText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            descText.SetValue(TextBlock.FontSizeProperty, 15.0);
            descText.SetValue(TextBlock.MarginProperty, new Thickness(0, 4, 0, 4));
            stack.AppendChild(descText);

            var impactText = new FrameworkElementFactory(typeof(TextBlock));
            impactText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Impact") { StringFormat = "Impact: {0}" });
            impactText.SetValue(TextBlock.FontSizeProperty, 12.0);
            stack.AppendChild(impactText);

            var likeText = new FrameworkElementFactory(typeof(TextBlock));
            likeText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Likelihood") { StringFormat = "Likelihood: {0}" });
            likeText.SetValue(TextBlock.FontSizeProperty, 12.0);
            stack.AppendChild(likeText);

            var mitiText = new FrameworkElementFactory(typeof(TextBlock));
            mitiText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Mitigation") { StringFormat = "Mitigation: {0}" });
            mitiText.SetValue(TextBlock.FontSizeProperty, 12.0);
            stack.AppendChild(mitiText);

            var dateText = new FrameworkElementFactory(typeof(TextBlock));
            dateText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("CreatedDate") { StringFormat = "Created: {0:dd/MM/yyyy}" });
            dateText.SetValue(TextBlock.FontSizeProperty, 11.0);
            stack.AppendChild(dateText);

            border.AppendChild(stack);
            cardTemplate.VisualTree = border;
            return cardTemplate;
        }

        private void ShowAssetDetailPanel(Asset asset)
        {
            var grid = new Grid {
                Margin = new System.Windows.Thickness(32, 32, 32, 32),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = double.NaN,
                MaxWidth = 900
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Back
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Asset info
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Risks label + Add
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Risks list

            // KHAI BÁO TRƯỚC các biến dùng cho asset info
            var nameBox = new TextBox { Text = asset.Name, FontSize = 16, Margin = new System.Windows.Thickness(0,0,0,8) };
            var categoryBox = new ComboBox { FontSize = 16, Margin = new System.Windows.Thickness(0,0,0,8), Width = 220 };
            var descBox = new TextBox { Text = asset.Description, FontSize = 15, AcceptsReturn = true, Height = 60, Margin = new System.Windows.Thickness(0,0,0,8) };

            // Row 0: Back
            var topPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };
            var backBtn = new Button { Content = "← Assets", Margin = new System.Windows.Thickness(0,0,12,0), MinWidth = 90, FontSize = 14 };
            backBtn.Click += (s, e) => ShowAssetContent();
            topPanel.Children.Add(backBtn);
            Grid.SetRow(topPanel, 0);
            grid.Children.Add(topPanel);

            // Row 1: Asset info
            var assetPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new System.Windows.Thickness(0, 18, 0, 18) };
            using (var context = new CorporationSecurityContext())
            {
                var categories = context.Categories.Select(c => new { c.Id, c.Name }).ToList();
                categoryBox.ItemsSource = categories;
                categoryBox.DisplayMemberPath = "Name";
                categoryBox.SelectedValuePath = "Id";
                categoryBox.SelectedValue = asset.CategoryId;
            }
            assetPanel.Children.Add(new TextBlock { Text = "Asset Name", FontWeight = FontWeights.Bold });
            assetPanel.Children.Add(nameBox);
            assetPanel.Children.Add(new TextBlock { Text = "Category", FontWeight = FontWeights.Bold });
            assetPanel.Children.Add(categoryBox);
            assetPanel.Children.Add(new TextBlock { Text = "Description", FontWeight = FontWeights.Bold });
            assetPanel.Children.Add(descBox);
            // Nút Save ở dưới cùng assetPanel
            var saveBtn = new Button {
                Content = "Save",
                Style = (Style)this.FindResource("GreenRoundedButton"),
                MinWidth = 100,
                FontSize = 14,
                Margin = new System.Windows.Thickness(0, 12, 0, 0)
            };
            saveBtn.Click += (s, e) => {
                using (var context = new CorporationSecurityContext())
                {
                    var dbAsset = context.Assets.FirstOrDefault(a => a.Id == asset.Id);
                    if (dbAsset != null)
                    {
                        dbAsset.Name = nameBox.Text.Trim();
                        dbAsset.CategoryId = (int)categoryBox.SelectedValue;
                        dbAsset.Description = descBox.Text.Trim();
                        context.SaveChanges();
                        // AuditLog
                        var auditLog = new AuditLog { UserId = CurrentUser.Id, Action = $"Updated Asset: {dbAsset.Name}" };
                        context.AuditLogs.Add(auditLog);
                        context.SaveChanges();
                        MessageBox.Show("Asset updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            };
            assetPanel.Children.Add(saveBtn);
            Grid.SetRow(assetPanel, 1);
            grid.Children.Add(assetPanel);

            // Row 2: Risks label + Add
            var riskHeader = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, Margin = new System.Windows.Thickness(0,0,0,8) };
            riskHeader.Children.Add(new TextBlock { Text = "Risks for this Asset", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), Margin = new System.Windows.Thickness(0,0,12,0) });
            var addRiskBtn = new Button { Content = "Add Risk", Style = (Style)this.FindResource("GreenRoundedButton"), MinWidth = 100, FontSize = 14 };
            addRiskBtn.Click += (s, e) => {
                var addRiskWin = new AddRiskWindow(asset.Id, CurrentUser.Id) { Owner = this };
                if (addRiskWin.ShowDialog() == true)
                {
                    using (var context = new CorporationSecurityContext())
                    {
                        var updatedAsset = context.Assets.Include(a => a.Category).Include(a => a.Risks).FirstOrDefault(a => a.Id == asset.Id);
                        ShowAssetDetailPanel(updatedAsset);
                    }
                }
            };
            riskHeader.Children.Add(addRiskBtn);
            Grid.SetRow(riskHeader, 2);
            grid.Children.Add(riskHeader);

            // Row 3: Risks list (không còn form Add inline)
            var risksPanel = new StackPanel { Orientation = Orientation.Vertical };
            var risks = asset.Risks.ToList();
            foreach (var risk in risks)
            {
                if (editingRiskId == risk.Id)
                {
                    // Edit form inline
                    var editPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new System.Windows.Thickness(0,0,0,12) };
                    var descBoxEdit = new TextBox { Text = risk.Description, Margin = new System.Windows.Thickness(0,0,0,6) };
                    var impactBoxEdit = new TextBox { Text = risk.Impact.ToString(), Margin = new System.Windows.Thickness(0,0,0,6) };
                    var likelihoodBoxEdit = new TextBox { Text = risk.Likelihood.ToString(), Margin = new System.Windows.Thickness(0,0,0,6) };
                    var mitigationBoxEdit = new TextBox { Text = risk.Mitigation, Margin = new System.Windows.Thickness(0,0,0,6) };
                    var saveBtnEdit = new Button { Content = "Save", Style = (Style)this.FindResource("GreenRoundedButton"), Margin = new System.Windows.Thickness(0,0,6,0) };
                    var cancelBtnEdit = new Button { Content = "Cancel" };
                    var btnPanelEdit = new StackPanel { Orientation = Orientation.Horizontal };
                    btnPanelEdit.Children.Add(saveBtnEdit);
                    btnPanelEdit.Children.Add(cancelBtnEdit);
                    editPanel.Children.Add(new TextBlock { Text = "Edit Risk", FontWeight = FontWeights.Bold });
                    editPanel.Children.Add(new TextBlock { Text = "Description" });
                    editPanel.Children.Add(descBoxEdit);
                    editPanel.Children.Add(new TextBlock { Text = "Impact (0-1)" });
                    editPanel.Children.Add(impactBoxEdit);
                    editPanel.Children.Add(new TextBlock { Text = "Likelihood (0-1)" });
                    editPanel.Children.Add(likelihoodBoxEdit);
                    editPanel.Children.Add(new TextBlock { Text = "Mitigation" });
                    editPanel.Children.Add(mitigationBoxEdit);
                    editPanel.Children.Add(btnPanelEdit);
                    saveBtnEdit.Click += (s, e) => {
                        if (string.IsNullOrWhiteSpace(descBoxEdit.Text) || !double.TryParse(impactBoxEdit.Text, out double impact) || !double.TryParse(likelihoodBoxEdit.Text, out double likelihood))
                        {
                            MessageBox.Show("Please enter valid risk information.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        using (var context = new CorporationSecurityContext())
                        {
                            var dbRisk = context.Risks.FirstOrDefault(r => r.Id == risk.Id);
                            if (dbRisk != null)
                            {
                                dbRisk.Description = descBoxEdit.Text.Trim();
                                dbRisk.Impact = impact;
                                dbRisk.Likelihood = likelihood;
                                dbRisk.Mitigation = mitigationBoxEdit.Text.Trim();
                                context.SaveChanges();
                                // AuditLog
                                var auditLog = new AuditLog { UserId = CurrentUser.Id, Action = $"Edited Risk: {dbRisk.Description} for Asset: {asset.Name}" };
                                context.AuditLogs.Add(auditLog);
                                context.SaveChanges();
                            }
                        }
                        editingRiskId = null;
                        using (var context = new CorporationSecurityContext())
                        {
                            var updatedAsset = context.Assets.Include(a => a.Category).Include(a => a.Risks).FirstOrDefault(a => a.Id == asset.Id);
                            ShowAssetDetailPanel(updatedAsset);
                        }
                    };
                    cancelBtnEdit.Click += (s, e) => { editingRiskId = null; ShowAssetDetailPanel(asset); };
                    risksPanel.Children.Add(editPanel);
                }
                else
                {
                    // Risk card
                    var border = new Border
                    {
                        Margin = new System.Windows.Thickness(0,0,0,12),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                        BorderThickness = new System.Windows.Thickness(1),
                        CornerRadius = new CornerRadius(8),
                        Background = Brushes.White,
                        Padding = new System.Windows.Thickness(12)
                    };
                    var stack = new StackPanel { Orientation = Orientation.Vertical };
                    stack.Children.Add(new TextBlock { Text = risk.Description, FontWeight = FontWeights.Bold });
                    stack.Children.Add(new TextBlock { Text = $"Impact: {risk.Impact}" });
                    stack.Children.Add(new TextBlock { Text = $"Likelihood: {risk.Likelihood}" });
                    stack.Children.Add(new TextBlock { Text = $"Mitigation: {risk.Mitigation}" });
                    var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0,6,0,0) };
                    var editBtn = new Button { Content = "Edit", MinWidth = 70, FontSize = 13, Margin = new System.Windows.Thickness(0,0,6,0) };
                    var delBtn = new Button { Content = "Delete", MinWidth = 70, FontSize = 13 };
                    editBtn.Click += (s, e) => {
                        var editRiskWin = new EditRiskWindow(risk.Id, CurrentUser.Id) { Owner = this };
                        if (editRiskWin.ShowDialog() == true)
                        {
                            using (var context = new CorporationSecurityContext())
                            {
                                var updatedAsset = context.Assets.Include(a => a.Category).Include(a => a.Risks).FirstOrDefault(a => a.Id == asset.Id);
                                ShowAssetDetailPanel(updatedAsset);
                            }
                        }
                    };
                    delBtn.Click += (s, e) => {
                        if (MessageBox.Show("Are you sure you want to delete this risk?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                using (var context = new CorporationSecurityContext())
                                {
                                    var dbRisk = context.Risks.FirstOrDefault(r => r.Id == risk.Id);
                                    if (dbRisk != null)
                                    {
                                        context.Risks.Remove(dbRisk);
                                        context.SaveChanges();
                                        // AuditLog
                                        var auditLog = new AuditLog { UserId = CurrentUser.Id, Action = $"Deleted Risk: {dbRisk.Description} for Asset: {asset.Name}" };
                                        context.AuditLogs.Add(auditLog);
                                        context.SaveChanges();
                                    }
                                }
                                using (var context = new CorporationSecurityContext())
                                {
                                    var updatedAsset = context.Assets.Include(a => a.Category).Include(a => a.Risks).FirstOrDefault(a => a.Id == asset.Id);
                                    ShowAssetDetailPanel(updatedAsset);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    "Cannot delete this risk because there are related controls. Please delete all controls linked to this risk first.",
                                    "Delete Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }
                        }
                    };
                    btnPanel.Children.Add(editBtn);
                    btnPanel.Children.Add(delBtn);
                    stack.Children.Add(btnPanel);
                    border.Child = stack;
                    risksPanel.Children.Add(border);
                }
            }
            Grid.SetRow(risksPanel, 3);
            grid.Children.Add(risksPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = grid
            };
            MainContentArea.Content = scrollViewer;
        }

        private void ShowControlContent()
        {
            var grid = new Grid { Margin = new System.Windows.Thickness(32, 32, 32, 32) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Filters
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Add Button
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content

            // Header
            var header = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 18), VerticalAlignment = VerticalAlignment.Center };
            var icon = new TextBlock { Text = "🛡️", FontSize = 32, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), VerticalAlignment = VerticalAlignment.Center };
            header.Children.Add(icon);
            header.Children.Add(new TextBlock { Text = "Controls", FontSize = 22, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), Margin = new System.Windows.Thickness(12,0,0,0), VerticalAlignment = VerticalAlignment.Center });
            Grid.SetRow(header, 0);
            grid.Children.Add(header);

            // Filters
            var filterPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0,0,0,18) };
            var assetBox = new ComboBox { Width = 200, Margin = new System.Windows.Thickness(0,0,12,0), FontSize = 14 };
            var riskBox = new ComboBox { Width = 200, Margin = new System.Windows.Thickness(0,0,12,0), FontSize = 14 };

            using (var context = new CorporationSecurityContext())
            {
                var assets = context.Assets.Select(a => new { Id = a.Id, Name = a.Name }).ToList();
                var assetList = new List<object> { new { Id = 0, Name = "All Assets" } };
                assetList.AddRange(assets);
                assetBox.ItemsSource = assetList;
                assetBox.DisplayMemberPath = "Name";
                assetBox.SelectedValuePath = "Id";
                assetBox.SelectedIndex = 0;
            }

            filterPanel.Children.Add(new TextBlock { Text = "Asset:", VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.SemiBold, Margin = new System.Windows.Thickness(0,0,8,0) });
            filterPanel.Children.Add(assetBox);
            filterPanel.Children.Add(new TextBlock { Text = "Risk:", VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.SemiBold, Margin = new System.Windows.Thickness(12,0,8,0) });
            filterPanel.Children.Add(riskBox);
            Grid.SetRow(filterPanel, 1);
            grid.Children.Add(filterPanel);

            // List controls (KHAI BÁO TRƯỚC, TRƯỚC addBtn, event handler, hàm nội bộ)
            var controlPanel = new ItemsControl();
            var itemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
            controlPanel.ItemsPanel = itemsPanel;
            controlPanel.Margin = new System.Windows.Thickness(0, 8, 0, 0);

            // Add Control Button
            var addControlBtn = new Button
            {
                Content = "Add Control",
                Style = (Style)this.FindResource("GreenRoundedButton"),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new System.Windows.Thickness(0, 0, 0, 18)
            };
            addControlBtn.Click += (s, e) => {
                // Check if asset and risk are selected
                if (!(assetBox.SelectedValue is int assetId) || assetId == 0 || 
                    !(riskBox.SelectedValue is int riskId) || riskId == 0)
                {
                    MessageBox.Show("Please select both an asset and a risk before adding a control.", 
                        "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Get asset and risk names for display
                string assetName = "";
                string riskDescription = "";
                using (var context = new CorporationSecurityContext())
                {
                    var asset = context.Assets.FirstOrDefault(a => a.Id == assetId);
                    var risk = context.Risks.FirstOrDefault(r => r.Id == riskId);
                    if (asset != null) assetName = asset.Name;
                    if (risk != null) riskDescription = risk.Description;
                }

                var addControlWindow = new AddControlWindow(riskId, assetId, assetName, riskDescription) { Owner = this };
                if (addControlWindow.ShowDialog() == true)
                {
                    using (var context = new CorporationSecurityContext())
                    {
                        var control = new CorporationSecurity.Models.Control
                        {
                            RiskId = riskId,
                            Description = addControlWindow.Description,
                            Effectiveness = addControlWindow.Effectiveness,
                            CreatedBy = CurrentUser.Id
                        };
                        context.Controls.Add(control);
                        context.SaveChanges();

                        // Audit logging
                        var auditLog = new AuditLog
                        {
                            UserId = CurrentUser.Id,
                            Action = $"Added Control: {control.Description} for Risk: {riskDescription}"
                        };
                        context.AuditLogs.Add(auditLog);
                        context.SaveChanges();
                    }
                    // Reload controls
                    LoadControls(assetBox, riskBox, controlPanel);
                }
            };
            Grid.SetRow(addControlBtn, 2);
            grid.Children.Add(addControlBtn);
            controlPanel.Margin = new System.Windows.Thickness(0, 8, 0, 0);

            // DataTemplate cho control card
            var cardTemplate = new DataTemplate();
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.MarginProperty, new Thickness(16, 0, 16, 24));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.SetValue(Border.BackgroundProperty, Brushes.White);
            border.SetValue(Border.PaddingProperty, new Thickness(12));
            border.SetValue(Border.WidthProperty, 320.0);
            border.SetValue(Border.MinWidthProperty, 220.0);
            border.SetValue(Border.MaxWidthProperty, 340.0);

            var stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            // Risk description (không có tiền tố)
            var riskText = new FrameworkElementFactory(typeof(TextBlock));
            riskText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("RiskDescription"));
            riskText.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            riskText.SetValue(TextBlock.FontSizeProperty, 13.0);
            riskText.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 0, 4));
            stack.AppendChild(riskText);

            var descText = new FrameworkElementFactory(typeof(TextBlock));
            descText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Description"));
            descText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            descText.SetValue(TextBlock.FontSizeProperty, 15.0);
            descText.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 0, 4));
            stack.AppendChild(descText);

            var effText = new FrameworkElementFactory(typeof(TextBlock));
            effText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Effectiveness") { StringFormat = "Status: {0}" });
            effText.SetValue(TextBlock.FontSizeProperty, 13.0);
            effText.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")));
            stack.AppendChild(effText);

            var dateText = new FrameworkElementFactory(typeof(TextBlock));
            dateText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("CreatedDate") { StringFormat = "Created: {0:dd/MM/yyyy}" });
            dateText.SetValue(TextBlock.FontSizeProperty, 11.0);
            stack.AppendChild(dateText);

            border.AppendChild(stack);
            cardTemplate.VisualTree = border;
            controlPanel.ItemTemplate = cardTemplate;

            // Sự kiện click vào control card để edit
            controlPanel.ItemContainerStyle = new Style(typeof(ContentPresenter));
            controlPanel.ItemContainerStyle.Setters.Add(new EventSetter(UIElement.MouseLeftButtonUpEvent, new System.Windows.Input.MouseButtonEventHandler((s, e) => {
                var cp = s as ContentPresenter;
                if (cp?.Content != null)
                {
                    // Lấy controlId từ anonymous object
                    var controlObj = cp.Content;
                    var idProp = controlObj.GetType().GetProperty("Id");
                    if (idProp == null) return;
                    int controlId = (int)idProp.GetValue(controlObj);
                    using (var context = new CorporationSecurityContext())
                    {
                        var control = context.Controls.FirstOrDefault(c => c.Id == controlId);
                        if (control == null) return;
                        var detailWin = new ControlDetailWindow(control) { Owner = this };
                        if (detailWin.ShowDialog() == true)
                        {
                            control.Description = detailWin.Description;
                            control.Effectiveness = detailWin.Effectiveness;
                            context.SaveChanges();
                            // AuditLog
                            var auditLog = new Models.AuditLog {
                                UserId = CurrentUser.Id,
                                Action = $"Edited Control: {control.Description}",
                                Timestamp = System.DateTime.Now
                            };
                            context.AuditLogs.Add(auditLog);
                            context.SaveChanges();
                            LoadControls(assetBox, riskBox, controlPanel);
                        }
                    }
                }
            })));

            Grid.SetRow(controlPanel, 3);
            grid.Children.Add(controlPanel);

            void UpdateRiskDropdown(ComboBox assetBox, ComboBox riskBox, ItemsControl controlPanel)
            {
                if (!(assetBox.SelectedValue is int assetId) || assetId == 0)
                {
                    riskBox.ItemsSource = null;
                    riskBox.SelectedIndex = -1;
                    controlPanel.ItemsSource = null;
                    return;
                }
                using (var context = new CorporationSecurityContext())
                {
                    var risks = context.Risks
                        .Where(r => r.AssetId == assetId)
                        .Select(r => new { Id = r.Id, Description = r.Description })
                        .ToList();
                    riskBox.ItemsSource = risks;
                    riskBox.DisplayMemberPath = "Description";
                    riskBox.SelectedValuePath = "Id";
                    riskBox.SelectedIndex = -1;
                }
            }

            void LoadControls(ComboBox assetBox, ComboBox riskBox, ItemsControl controlPanel)
            {
                using (var context = new CorporationSecurityContext())
                {
                    if (!(assetBox.SelectedValue is int assetId) || assetId == 0 || !(riskBox.SelectedValue is int riskId) || riskId == 0)
                    {
                        controlPanel.ItemsSource = null;
                        return;
                    }
                    var controls = context.Controls
                        .Include(c => c.Risk)
                        .ThenInclude(r => r.Asset)
                        .Where(c => c.Risk.AssetId == assetId && c.RiskId == riskId)
                        .OrderByDescending(c => c.CreatedDate)
                        .Select(c => new
                        {
                            c.Id,
                            c.Description,
                            c.Effectiveness,
                            c.CreatedDate,
                            AssetName = c.Risk != null && c.Risk.Asset != null ? c.Risk.Asset.Name : "(No Asset)",
                            RiskDescription = c.Risk != null ? c.Risk.Description : "(No Risk)"
                        })
                        .ToList();
                    controlPanel.ItemsSource = controls;
                }
            }

            assetBox.SelectionChanged += (s, e) => {
                UpdateRiskDropdown(assetBox, riskBox, controlPanel);
                LoadControls(assetBox, riskBox, controlPanel);
            };
            riskBox.SelectionChanged += (s, e) => LoadControls(assetBox, riskBox, controlPanel);

            // Initial load
            UpdateRiskDropdown(assetBox, riskBox, controlPanel);
            LoadControls(assetBox, riskBox, controlPanel);

            MainContentArea.Content = grid;
        }

        private void ShowUserContent()
        {
            var grid = new Grid { Margin = new System.Windows.Thickness(32, 32, 32, 32) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // DataGrid

            // Header
            var header = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 18), VerticalAlignment = VerticalAlignment.Center };
            var icon = new TextBlock { Text = "👤", FontSize = 32, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), VerticalAlignment = VerticalAlignment.Center };
            header.Children.Add(icon);
            header.Children.Add(new TextBlock { Text = "Users", FontSize = 22, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), Margin = new System.Windows.Thickness(12,0,0,0), VerticalAlignment = VerticalAlignment.Center });
            Grid.SetRow(header, 0);
            grid.Children.Add(header);

            // Add User button
            var addBtn = new Button {
                Content = "Add User",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new System.Windows.Thickness(0,0,0,18),
                Padding = new System.Windows.Thickness(18,6,18,6),
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            addBtn.Click += (s, e) => {
                var addWin = new AddUserWindow() { Owner = this };
                if (addWin.ShowDialog() == true)
                {
                    using (var context = new CorporationSecurityContext())
                    {
                        var user = new CorporationSecurity.Models.User {
                            Username = addWin.Username,
                            PasswordHash = addWin.PasswordHash,
                            FirstName = addWin.FirstName,
                            LastName = addWin.LastName,
                            Email = addWin.Email,
                            RoleId = addWin.RoleId,
                            IsActive = addWin.IsActive
                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                    ShowUserContent();
                }
            };
            // Thêm vào grid (trước DataGrid)
            Grid.SetRow(addBtn, 0);
            grid.Children.Add(addBtn);

            // DataGrid
            var dataGrid = new DataGrid {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                Margin = new System.Windows.Thickness(0,0,0,0),
                RowHeight = 36,
                FontSize = 13,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.FullRow
            };
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Username", Binding = new System.Windows.Data.Binding("Username") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Password", Binding = new System.Windows.Data.Binding("PasswordHash") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "First Name", Binding = new System.Windows.Data.Binding("FirstName") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Last Name", Binding = new System.Windows.Data.Binding("LastName") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("Email") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Role", Binding = new System.Windows.Data.Binding("Role.Name") });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Active", Binding = new System.Windows.Data.Binding("IsActive") });
            // Bỏ cột LastLogin, thêm cột Action
            var actionCol = new DataGridTemplateColumn { Header = "Action" };
            var btnFactory = new FrameworkElementFactory(typeof(Button));
            btnFactory.SetValue(Button.ContentProperty, "Edit");
            btnFactory.SetValue(Button.MarginProperty, new System.Windows.Thickness(4,4,4,4));
            btnFactory.SetValue(Button.PaddingProperty, new System.Windows.Thickness(8,2,8,2));
            btnFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")));
            btnFactory.SetValue(Button.ForegroundProperty, Brushes.White);
            btnFactory.SetValue(Button.FontWeightProperty, FontWeights.Bold);
            btnFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler((s, e) => {
                var btn = s as Button;
                var row = btn?.DataContext;
                if (row != null)
                {
                    var user = row as CorporationSecurity.Models.User;
                    if (user != null)
                    {
                        var editWin = new EditUserWindow(user) { Owner = this };
                        if (editWin.ShowDialog() == true)
                        {
                            using (var context = new CorporationSecurityContext())
                            {
                                var dbUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
                                if (dbUser != null)
                                {
                                    dbUser.FirstName = editWin.FirstName;
                                    dbUser.LastName = editWin.LastName;
                                    dbUser.Email = editWin.Email;
                                    dbUser.RoleId = editWin.RoleId;
                                    dbUser.IsActive = editWin.IsActive;
                                    if (!string.IsNullOrEmpty(editWin.PasswordHash) && editWin.PasswordHash != dbUser.PasswordHash)
                                    {
                                        dbUser.PasswordHash = editWin.PasswordHash;
                                    }
                                    context.SaveChanges();
                                }
                            }
                            // Reload user list
                            ShowUserContent();
                        }
                    }
                }
            }));
            var cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = btnFactory;
            actionCol.CellTemplate = cellTemplate;
            dataGrid.Columns.Add(actionCol);
            Grid.SetRow(dataGrid, 1);
            grid.Children.Add(dataGrid);

            using (var context = new CorporationSecurityContext())
            {
                var users = context.Users.Include(u => u.Role).ToList();
                dataGrid.ItemsSource = users;
            }

            MainContentArea.Content = grid;
        }

        private void ShowAuditLogsContent()
        {
            var grid = new Grid { Margin = new System.Windows.Thickness(32, 32, 32, 32) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // DataGrid

            // Header
            var header = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0, 0, 0, 18), VerticalAlignment = VerticalAlignment.Center };
            var icon = new TextBlock { Text = "📜", FontSize = 32, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), VerticalAlignment = VerticalAlignment.Center };
            header.Children.Add(icon);
            header.Children.Add(new TextBlock { Text = "Audit Logs", FontSize = 22, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27ae60")), Margin = new System.Windows.Thickness(12,0,0,0), VerticalAlignment = VerticalAlignment.Center });
            Grid.SetRow(header, 0);
            grid.Children.Add(header);

            // DataGrid
            var dataGrid = new DataGrid {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                Margin = new System.Windows.Thickness(0,0,0,0),
                RowHeight = 36,
                FontSize = 13,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.FullRow,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new System.Windows.Data.Binding("UserFullName"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Action", Binding = new System.Windows.Data.Binding("Action"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Timestamp", Binding = new System.Windows.Data.Binding("Timestamp") { StringFormat = "dd/MM/yyyy HH:mm:ss" }, Width = new DataGridLength(1.2, DataGridLengthUnitType.Star) });
            Grid.SetRow(dataGrid, 1);
            grid.Children.Add(dataGrid);

            using (var context = new CorporationSecurityContext())
            {
                var logs = context.AuditLogs
                    .Include(l => l.User)
                    .OrderByDescending(l => l.Timestamp)
                    .Select(l => new {
                        UserFullName = l.User.FirstName + " " + l.User.LastName,
                        l.Action,
                        l.Timestamp
                    })
                    .ToList();
                dataGrid.ItemsSource = logs;
            }

            MainContentArea.Content = grid;
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }

    public class PasswordMaskConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string pwd && !string.IsNullOrEmpty(pwd))
                return new string('*', pwd.Length);
            return string.Empty;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
} 