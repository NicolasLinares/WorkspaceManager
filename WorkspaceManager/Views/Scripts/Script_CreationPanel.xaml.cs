using System;
using System.Windows;
using System.Windows.Input;
using WorkspaceManagerTool.Controllers;
using System.Windows.Media;
using WorkspaceManagerTool.Exceptions;
using WorkspaceManagerTool.Models.Scripts;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorkspaceManagerTool.Views.Scripts {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Script_CreationPanel : UserControl, INotifyPropertyChanged {

        private string name;
        private string description;
        private string script;

        public string NameText {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string DescriptionText {
            get => description;
            set => SetProperty(ref description, value);
        }
        public string ScriptText {
            get => script;
            set => SetProperty(ref script, value);
        }


        private string DefaultName => "Nuevo script";
        private string DefaultDescription => "";
        private string DefaultScript => "echo 'Hello World!'";

        public Script_CreationPanel() {
            DataContext = this;
            InitializeComponent();
        }

        public Script_CreationPanel(Script scriptToEdit) {
            DataContext = this;
            InitializeComponent();

            if (scriptToEdit == null) {
                return;
            }
            NameText = scriptToEdit.Name;
            DescriptionText = scriptToEdit.Description;
            ScriptText = scriptToEdit.Commands;
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

        private void CreateScript_Action(object sender, EventArgs e) {

        }

        private void Cancel_Action(object sender, EventArgs e) {
            
        }


        public Script GetScript() {
            // Set default values if empty
            if (string.IsNullOrWhiteSpace(NameText)) {
                NameText = DefaultName;
            }
            if (string.IsNullOrWhiteSpace(DescriptionText)) {
                DescriptionText = DefaultDescription;
            }
            if (string.IsNullOrWhiteSpace(ScriptText)) {
                ScriptText = DefaultScript;
            }
            return new Script(NameText, DescriptionText, ScriptText);
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
    }
}
