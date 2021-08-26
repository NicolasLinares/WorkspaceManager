using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Exceptions;
using WorkspaceManagerTool.Controllers;
using WorkspaceManagerTool.Events;

namespace WorkspaceManagerTool.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Script_ExecutionPanel : UserControl, INotifyPropertyChanged {

        private string name;

        private string script;

        public string ScriptText {
            get => script;
            set => SetProperty(ref script, value);
        }
        public string NameText {
            get => name;
            set => SetProperty(ref name, value);
        }
        public Script ScriptSelected { get; set; }

        public Script_ExecutionPanel(GroupableResource script) {
            DataContext = this;
            InitializeComponent();

            ScriptSelected = script as Script;
            ScriptText = ScriptSelected.Commands;
            NameText = ScriptSelected.Name;
        }

        #region Actions


        public event EventHandler<ExecutionEvent> HandlerExecution;


        private void RunScript_Action(object sender, EventArgs e) {

            ExecutionEvent exec = new ExecutionEvent();
            exec.Script = ScriptSelected;
            (exec.Script as Script).Commands = ScriptText;
            HandlerExecution?.Invoke(this, exec);
        }

        private void ClosePanel_Action(object sender, EventArgs e) {
            this.Visibility = Visibility.Collapsed;
        }

        #endregion


        #region Events handlers

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
