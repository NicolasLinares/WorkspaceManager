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

        private string branchText {
            get { return _BranchNameTextBox.Text; }
            set { _BranchNameTextBox.Text = value; }
        }
        private string repoLocationText {
            get { return _RepoLocationTextBox.Text; }
            set { _RepoLocationTextBox.Text = value; }
        }
        private string currentBranchText {
            get { return _CurrentBranchTextBox.Text; }
            set { _CurrentBranchTextBox.Text = value; }
        }
        private bool? cleanCheck {
            get { return _CleanRadioButton.IsChecked; }
            set { _CleanRadioButton.IsChecked = value; }
        }
        private bool? fullcleanCheck {
            get { return _FullCleanRadioButton.IsChecked; }
            set { _FullCleanRadioButton.IsChecked = value; }
        }
        private bool? onlydevCheck {
            get { return _OnlyDevRadioButton.IsChecked; }
            set { _OnlyDevRadioButton.IsChecked = value; }
        }
        private bool? notjsCheck {
            get { return _NotjsRadioButton.IsChecked; }
            set { _NotjsRadioButton.IsChecked = value; }
        }
        private bool? notdocCheck {
            get { return _NotdocRadioButton.IsChecked; }
            set { _NotdocRadioButton.IsChecked = value; }
        }

        private string summaryText {
            get { return _SummaryTextBox.Text; }
            set { _SummaryTextBox.Text = value; }
        }

        private DeploymentsController deploymentsController { get; set; }

        public MainWindow() {
            deploymentsController = DeploymentsController.GetInstance();

            InitializeComponent();
        }

        /// <summary>
        /// Select all text when textbox gets focus
        /// </summary>
        private void OnPalabraGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null) {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Add focus when the user clicks on the textbox
        /// </summary>
        private void OnPalabraPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null) {
                if (!textBox.IsKeyboardFocusWithin) {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }


        private void AcceptClick(object sender, EventArgs e) {
            deploymentsController.CreateEnvironment();
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

            repoLocationText = fbd.SelectedPath;

            BrowseDirectory(repoLocationText);
        }

        private void BrowseInput(object sender, EventArgs e) {

            if (string.IsNullOrWhiteSpace(repoLocationText)) {
                return;
            }

            BrowseDirectory(repoLocationText);
        }

        private void BrowseDirectory(string selectedPath) {

            try {
                deploymentsController.SetCurrentWorkspace(selectedPath);

                currentBranchText = deploymentsController.GetCurrentWorkspace();
                _CurrentBranchTextBox.Visibility = Visibility.Visible;
                _CurrentBranchTextBox.Foreground = new SolidColorBrush(Colors.Blue);
            } catch (NullReferenceException ex) {
                currentBranchText = "Directory does not exists";
                _CurrentBranchTextBox.Visibility = Visibility.Visible;
                _CurrentBranchTextBox.Foreground = new SolidColorBrush(Colors.Red);
            } catch (NotScriptsInWorkspaceException ex) {
                currentBranchText = ex.Message;
                _CurrentBranchTextBox.Visibility = Visibility.Visible;
                _CurrentBranchTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void UpdateSummary() {
            if (_SummaryTextBox != null)
                summaryText = deploymentsController.GetSummary();
        }


        #region Clean options

        private void CleanOption_Checked(object sender, RoutedEventArgs e) {
            if (_CleanRadioButton != null && cleanCheck == true) {
                deploymentsController.SetCleanOption(CleanOptions.CLEAN);
                UpdateSummary();
                return;
            }

            if (_FullCleanRadioButton != null && fullcleanCheck == true) {
                deploymentsController.SetCleanOption(CleanOptions.FULLCLEAN);
                UpdateSummary();
                return;
            }

            deploymentsController.SetCleanOption(CleanOptions.NOTCLEAN);
            UpdateSummary();
        }


        #endregion

        #region Build options

        private void NewBranchInput(object sender, EventArgs e) {

            if (string.IsNullOrWhiteSpace(branchText)) {
                return;
            }

            deploymentsController.SetNewBranch(branchText);
            UpdateSummary();
        }

        private void BuildOption_Checked(object sender, RoutedEventArgs e) {

            if (_NotjsRadioButton != null && notjsCheck == true) {
                deploymentsController.SetBuildOption(BuildOptions.NOTJS);
                UpdateSummary();
                return;
            }

            if (_NotdocRadioButton != null && notdocCheck == true) {
                deploymentsController.SetBuildOption(BuildOptions.NOTDOC);
                UpdateSummary();
                return;
            }

            deploymentsController.SetBuildOption(BuildOptions.ONLYDEV);
            UpdateSummary();
        }

        #endregion

    }
}
