using WorkspaceManagerTool.Models.QuickAccess;
using Ookii.Dialogs.WinForms;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;
using WorkspaceManagerTool.Controllers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ookii.Dialogs.WinForms;

namespace WorkspaceManagerTool.Views {


    public partial class QuickAccessCreationPanel : UserControl, INotifyPropertyChanged {

        private string path;
        private string name;
        private string description;
        private string group;
        private Color color;

        public string PathText {
            get => path;
            set => SetProperty(ref path, value);
        }
        public string NameText {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string DescriptionText {
            get => description;
            set => SetProperty(ref description, value);
        }
        public string GroupText {
            get => group;
            set => SetProperty(ref group, value);
        }
        public Color ColorBrush {
            get => color;
            set => SetProperty(ref color, value);
        }
        public Color DefaultColor => Color.FromRgb(17, 166, 143);

        public QuickAccessCreationPanel() {
            DataContext = this;
            InitializeComponent();            
        }

        public QuickAccessCreationPanel(QuickAccess qa) {
            DataContext = this;
            InitializeComponent();

            if (qa == null) {
                return;
            }

            PathText = qa.Path;
            NameText = qa.Name;
            DescriptionText = qa.Description;
            GroupText = qa.Group.Name;
            ColorBrush = Color.FromRgb(qa.Group.Color.Color.R, qa.Group.Color.Color.G, qa.Group.Color.Color.B);
            _ColorPicker.SelectedColor = ColorBrush;
        }

        #region Events handlers

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Select all text when textbox gets focus
        /// </summary>
        private void OnPalabraGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Add focus when the user clicks on the textbox
        /// </summary>
        private void OnPalabraPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox textBox) {
                if (!textBox.IsKeyboardFocusWithin) {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }

        private void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) {
            if (_ColorPicker.SelectedColor.HasValue) {
                ColorBrush = _ColorPicker.SelectedColor.Value;
            }
        }

        private void BrowseClick(object sender, EventArgs e) {

            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.Description = "Selecciona la ruta para crear el acceso directo";
            fbd.UseDescriptionForTitle = true;
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                PathText = fbd.SelectedPath;
            }

        }

    }
}
