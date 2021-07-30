using Ookii.Dialogs.WinForms;
using System;
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

        public NewQuickAccessDialog(string path, string name, string description) {
            PathText = path;
            NameText = name;
            DescriptionText = description;
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

        private void BrowseClick(object sender, EventArgs e) {

            var fbd = new VistaFolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK && string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                return;
            }

            PathText = fbd.SelectedPath;
            _PathTextBox.ClearValue(TextBox.BorderBrushProperty);
        }


        private void BrowseInput(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(PathText)) {
                return;
            }
            _PathTextBox.ClearValue(TextBox.BorderBrushProperty);
        }


        private void AcceptClick(object sender, EventArgs e) {

            // Validación de inputs
            if (string.IsNullOrWhiteSpace(PathText)) {
                _PathTextBox.BorderBrush = Brushes.Red;
            } else if (string.IsNullOrWhiteSpace(NameText)) {
                _NameTextBox.BorderBrush = Brushes.Red;
            } else {
                this.DialogResult = true;
            }

        }
        private void CancelClick(object sender, EventArgs e) {
            this.DialogResult = false;
        }
    }
}
