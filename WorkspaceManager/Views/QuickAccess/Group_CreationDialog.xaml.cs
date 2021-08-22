using WorkspaceManagerTool.Models.QuickAccess;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
 
namespace WorkspaceManagerTool.Views.QuickAccess {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Group_CreationDialog : Window {

        private SolidColorBrush color;

        public string NameText {
            get { return _NameTextBox.Text; }
            private set {
                _NameTextBox.Text = value;
                _NameTextBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
        public SolidColorBrush ColorSelected {
            get { return color; }
            private set { color = value; }
        }

        public Group_CreationDialog() {
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

        private void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) {
            if (_ColorPicker.SelectedColor.HasValue) {
                ColorSelected = new SolidColorBrush(_ColorPicker.SelectedColor.Value);
            }
        }

        private void Create_Action(object sender, EventArgs e) {
            // Input validation
            if (string.IsNullOrWhiteSpace(NameText)) {
                this.DialogResult = false;
                return;
            }
            this.DialogResult = true;
        }

        private void Cancel_Action(object sender, EventArgs e) {
            this.DialogResult = false;
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
