using INVOXWorkspaceManager.Models.QuickAccess;
using Ookii.Dialogs.WinForms;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
 
namespace INVOXWorkspaceManager.Views {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewQuickAccessDialog : Window {

        public string PathText {
            get { return _PathTextBox.Text; }
            private set { _PathTextBox.Text = value; }
        }
        public string NameText {
            get { return _NameTextBox.Text; }
            private set {
                _NameTextBox.Text = value;
                _NameTextBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
        public string DescriptionText {
            get { return _DescriptionTextBox.Text; }
            private set { _DescriptionTextBox.Text = value; }
        }

        public NewQuickAccessDialog() {
            InitializeComponent();            
        }

        public NewQuickAccessDialog(QuickAccess qa) {
            InitializeComponent();
            PathText = qa.Path;
            NameText = qa.Name;
            DescriptionText = qa.Description;
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

        private void BrowseClick(object sender, EventArgs e) {

            var browser = new VistaFolderBrowserDialog();
            var result = browser.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(browser.SelectedPath)) {
                PathText = browser.SelectedPath;
                _PathTextBox.ClearValue(TextBox.BackgroundProperty);
            }
        }



        private void CheckPath_TextBox(object sender, TextChangedEventArgs e) {
            if (!Directory.Exists(PathText)) {
                _PathTextBox.Background = Brushes.Salmon;
                _NameTextBox.IsEnabled = false;
                _DescriptionTextBox.IsEnabled = false;
                _AcceptButton.IsEnabled = false;
                return;
            }

            _PathTextBox.ClearValue(TextBox.BackgroundProperty);
            _NameTextBox.IsEnabled = true;
        }

        private void CheckName_TextBox(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrWhiteSpace(NameText)) {
                _NameTextBox.Background = Brushes.Salmon;
                _DescriptionTextBox.IsEnabled = false;
                _AcceptButton.IsEnabled = false;
                return;
            }

            _NameTextBox.ClearValue(TextBox.BackgroundProperty);
            _DescriptionTextBox.IsEnabled = true;
            _AcceptButton.IsEnabled = true;
        }


        private void AcceptClick(object sender, EventArgs e) {

            // Validación de inputs
            if (string.IsNullOrWhiteSpace(PathText)) {
                _PathTextBox.BorderBrush = Brushes.Red;
                this.DialogResult = false;
            } else if (string.IsNullOrWhiteSpace(NameText)) {
                _NameTextBox.BorderBrush = Brushes.Red;
                this.DialogResult = false;

            } else {
                this.DialogResult = true;
            }

        }

        private void CancelClick(object sender, EventArgs e) {
            this.DialogResult = false;
        }
    }
}
