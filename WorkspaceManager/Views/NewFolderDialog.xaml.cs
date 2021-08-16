﻿using WorkspaceManagerTool.Models.QuickAccess;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
 
namespace WorkspaceManagerTool.Views {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewFolderDialog : Window {

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

        private SolidColorBrush color;

        public SolidColorBrush Color {
            get { return color; }
            private set { color = value; }
        }

        public NewFolderDialog() {
            InitializeComponent();            
        }

        public NewFolderDialog(Group folder) {
            InitializeComponent();
            NameText = folder.Name;
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


        private void ValidateName_TextBox(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrWhiteSpace(NameText)) {
                _NameTextBox.Background = Brushes.Salmon;
                _AcceptButton.IsEnabled = false;
                return;
            }

            _NameTextBox.ClearValue(TextBox.BackgroundProperty);
            _AcceptButton.IsEnabled = true;
        }

        private void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) {
            if (_ColorPicker.SelectedColor.HasValue) {
                Color = new SolidColorBrush(_ColorPicker.SelectedColor.Value);
            }
        }

        private void AcceptClick(object sender, EventArgs e) {

            // Validación de inputs
            if (string.IsNullOrWhiteSpace(NameText)) {
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