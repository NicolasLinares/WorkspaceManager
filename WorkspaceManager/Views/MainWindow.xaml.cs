using System;
using System.Windows;
using System.Windows.Input;
using INVOXWorkspaceManager.Controllers;
using System.Windows.Media;
using INVOXWorkspaceManager.Exceptions;
using Ookii.Dialogs.WinForms;
using INVOXWorkspaceManager.Models.Scripts;


namespace INVOXWorkspaceManager.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private string BranchText {
            get { return _BranchNameTextBox.Text; }
            set { _BranchNameTextBox.Text = value; }
        }
        private string RepoLocationText {
            get { return _RepoLocationTextBox.Text; }
            set { _RepoLocationTextBox.Text = value; }
        }
        private string CurrentBranchText {
            get { return _CurrentBranchTextBox.Text; }
            set { _CurrentBranchTextBox.Text = value; }
        }
        private bool? CleanCheck {
            get { return _CleanRadioButton.IsChecked; }
            set { _CleanRadioButton.IsChecked = value; }
        }
        private bool? FullcleanCheck {
            get { return _FullCleanRadioButton.IsChecked; }
            set { _FullCleanRadioButton.IsChecked = value; }
        }
        private bool? OnlydevCheck {
            get { return _OnlyDevRadioButton.IsChecked; }
            set { _OnlyDevRadioButton.IsChecked = value; }
        }
        private bool? NotjsCheck {
            get { return _NotjsRadioButton.IsChecked; }
            set { _NotjsRadioButton.IsChecked = value; }
        }
        private bool? NotdocCheck {
            get { return _NotdocRadioButton.IsChecked; }
            set { _NotdocRadioButton.IsChecked = value; }
        }

        private string SummaryText {
            get { return _SummaryTextBox.Text; }
            set { _SummaryTextBox.Text = value; }
        }

        private DeploymentsController DeploymentsController { get; set; }

        public MainWindow() {
            DeploymentsController = DeploymentsController.GetInstance();
            InitializeComponent();
        }

        /// <summary>
        /// Select all text when textbox gets focus
        /// </summary>
        private void OnPalabraGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (sender is System.Windows.Controls.TextBox textBox) {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Add focus when the user clicks on the textbox
        /// </summary>
        private void OnPalabraPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (sender is System.Windows.Controls.TextBox textBox) {
                if (!textBox.IsKeyboardFocusWithin) {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }


        private void AcceptClick(object sender, EventArgs e) {
            DeploymentsController.CreateEnvironment();
        }
        private void CancelClick(object sender, EventArgs e) {
            //deploymentsController.OpenFolder();
        }

        private void BrowseClick(object sender, EventArgs e) {
            
            var fbd = new VistaFolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK && string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                return;
            }

            RepoLocationText = fbd.SelectedPath;

            BrowseDirectory(RepoLocationText);
        }

        private void BrowseInput(object sender, EventArgs e) {

            if (string.IsNullOrWhiteSpace(RepoLocationText)) {
                return;
            }

            BrowseDirectory(RepoLocationText);
        }

        private void BrowseDirectory(string selectedPath) {

            try {
                DeploymentsController.SetCurrentWorkspace(selectedPath);

                CurrentBranchText = DeploymentsController.GetCurrentWorkspace();
                _CurrentBranchTextBox.Visibility = Visibility.Visible;
                _CurrentBranchTextBox.Foreground = new SolidColorBrush(Colors.Blue);
            } catch (NullReferenceException ex) {
                CurrentBranchText = "Directory does not exists";
                _CurrentBranchTextBox.Visibility = Visibility.Visible;
                _CurrentBranchTextBox.Foreground = new SolidColorBrush(Colors.Red);
            } catch (NotScriptsInWorkspaceException ex) {
                CurrentBranchText = ex.Message;
                _CurrentBranchTextBox.Visibility = Visibility.Visible;
                _CurrentBranchTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void UpdateSummary() {
            if (_SummaryTextBox != null)
                SummaryText = DeploymentsController.GetSummary();
        }


        #region Clean options

        private void CleanOption_Checked(object sender, RoutedEventArgs e) {
            if (_CleanRadioButton != null && CleanCheck == true) {
                DeploymentsController.SetCleanOption(CleanOptions.CLEAN);
                UpdateSummary();
                return;
            }

            if (_FullCleanRadioButton != null && FullcleanCheck == true) {
                DeploymentsController.SetCleanOption(CleanOptions.FULLCLEAN);
                UpdateSummary();
                return;
            }

            DeploymentsController.SetCleanOption(CleanOptions.NOTCLEAN);
            UpdateSummary();
        }


        #endregion

        #region Build options

        private void NewBranchInput(object sender, EventArgs e) {

            if (string.IsNullOrWhiteSpace(BranchText)) {
                return;
            }

            DeploymentsController.SetNewBranch(BranchText);
            UpdateSummary();
        }

        private void BuildOption_Checked(object sender, RoutedEventArgs e) {

            if (_NotjsRadioButton != null && NotjsCheck == true) {
                DeploymentsController.SetBuildOption(BuildOptions.NOTJS);
                UpdateSummary();
                return;
            }

            if (_NotdocRadioButton != null && NotdocCheck == true) {
                DeploymentsController.SetBuildOption(BuildOptions.NOTDOC);
                UpdateSummary();
                return;
            }

            DeploymentsController.SetBuildOption(BuildOptions.ONLYDEV);
            UpdateSummary();
        }

        #endregion

    }
}
