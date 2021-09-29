using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkspaceManagerTool.Models;

namespace WorkspaceManagerTool.Views {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Group_CreationDialog : Window {

        private Color DEFAULT_COLOR => Color.FromRgb(255, 255, 255);
        private Group DEFAULT_GROUP => new Group("Nuevo", new SolidColorBrush(DEFAULT_COLOR));

        public string GroupName {
            get {
                return _GroupName.Text;
            }
            private set {
                _GroupName.Text = value;
            }
        }
        public SolidColorBrush ColorSelected { get; set; }

        public Group_CreationDialog() {
            InitializeComponent();

            GroupName = DEFAULT_GROUP.Name;
            ColorSelected = DEFAULT_GROUP.Color;
            _ColorPicker.SelectedColor = DEFAULT_COLOR;
        }

        public Group_CreationDialog(Group groupToEdit) {
            InitializeComponent();

            GroupName = groupToEdit.Name;
            ColorSelected = groupToEdit.Color;
            _ColorPicker.SelectedColor = groupToEdit.Color.Color;
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

        private void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) {
            if (_ColorPicker.SelectedColor.HasValue) {
                ColorSelected = new SolidColorBrush(_ColorPicker.SelectedColor.Value);
            }
        }

        private void Create_Action(object sender, EventArgs e) {
            // Input validation
            if (string.IsNullOrWhiteSpace(GroupName)) {
                this.DialogResult = false;
                return;
            }
            this.DialogResult = true;
        }

        private void Cancel_Action(object sender, EventArgs e) {
            this.DialogResult = false;
        }

        public Group GetGroup() {
            // Set default values if empty
            if (string.IsNullOrWhiteSpace(GroupName)) {
                GroupName = DEFAULT_GROUP.Name;
            }

            return new Group(GroupName, ColorSelected);
        }

        private Color GetContrastColor(Color color) {
            byte d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (luminance > 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromRgb(d, d, d);

            // _NameTextBox.Foreground = new SolidColorBrush(ContrastColor(_ColorPicker.SelectedColor.Value));
        }
    }
}
