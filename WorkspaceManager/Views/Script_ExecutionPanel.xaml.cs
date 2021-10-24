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
        private bool backgroundOption;
        private bool keepOpenOption;
        private bool closeOption;

        public string NameText {
            get => name;
            set => SetProperty(ref name, value);
        }
        public Script ScriptSelected { get; set; }
        public Script LastModifiedScriptText { get; set; }

        public bool BackgroundOption {
            get => backgroundOption;
            set => SetProperty(ref backgroundOption, value);
        }
        public bool KeepOpenOption {
            get => keepOpenOption;
            set => SetProperty(ref keepOpenOption, value);
        }
        public bool CloseOption {
            get => closeOption;
            set => SetProperty(ref closeOption, value);
        }

        public Script_ExecutionPanel(GroupableResource script) {
            DataContext = this;
            InitializeComponent();

            ScriptSelected = script as Script;
            LastModifiedScriptText = ScriptSelected;
            NameText = ScriptSelected.Name;
            _ScriptTextBox.Text = ScriptSelected.Commands;

            BackgroundOption = ScriptSelected.Options.ExecOption == ExecutionOption.BackgroundExecution;
            KeepOpenOption = ScriptSelected.Options.ExecOption == ExecutionOption.KeepOpenAfterFinish;
            CloseOption = ScriptSelected.Options.ExecOption == ExecutionOption.CloseAfterFinish;

        }


        #region Handlers
        public event EventHandler HandlerClosePanel;
        public event EventHandler<ExecutionEvent> HandlerExecution;
        public event EventHandler<ScriptEvent> HandlerChanges;
        #endregion

        #region Actions
        private void RunScript_Action(object sender, EventArgs e) {
            ExecutionEvent exec = new ExecutionEvent();
            exec.Script = new Script(ScriptSelected.Name, ScriptSelected.Description, _ScriptTextBox.Text, ScriptSelected.Group, ScriptSelected.Pinned);
            (exec.Script as Script).SetOptions(ScriptSelected.Options);
            HandlerExecution?.Invoke(this, exec);
        }
        private void ClosePanel_Action(object sender, EventArgs e) {
            this.Visibility = Visibility.Collapsed;
            HandlerClosePanel?.Invoke(this, e);
        }
        private void OpenCloseOptionsPanel_Action(object sender, EventArgs e) {

            if (_OptionsPanel.Visibility == Visibility.Visible) {
                _OptionsPanel.Visibility = Visibility.Collapsed;
                return;
            }
            _OptionsPanel.Visibility = Visibility.Visible;
        }


        private void ShowSaveButton_Action(object sender, EventArgs e) {
            if (!this.IsLoaded && !this.IsInitialized) {
                return;
            }

            if (!_ScriptTextBox.Text.Equals((ScriptSelected as Script).Commands)) {
                _SaveChanges_Button.Visibility = Visibility.Visible;

                return;
            }
            _SaveChanges_Button.Visibility = Visibility.Collapsed;
        }
        private void SaveChanges_Action(object sender, EventArgs e) {
            ScriptEvent exec = new ScriptEvent();
            exec.OldScript = ScriptSelected;
            exec.NewScript = new Script(ScriptSelected.Name, ScriptSelected.Description, _ScriptTextBox.Text, ScriptSelected.Group, ScriptSelected.Pinned);
            HandlerChanges?.Invoke(this, exec);
            // hide button because change has been applied
            _SaveChanges_Button.Visibility = Visibility.Collapsed;
            // now both has to be updated
            (LastModifiedScriptText as Script).Commands = _ScriptTextBox.Text;
            ScriptSelected = LastModifiedScriptText;
        }


        private void SetBackgroundOption(object sender, EventArgs e) {
            ScriptSelected.SetOptions(new Options(ExecutionOption.BackgroundExecution));
        }

        private void SetKeepOpenOption(object sender, EventArgs e) {
            ScriptSelected.SetOptions(new Options(ExecutionOption.KeepOpenAfterFinish));
        }

        private void SetCloseOption(object sender, EventArgs e) {
            ScriptSelected.SetOptions(new Options(ExecutionOption.CloseAfterFinish));
        }

        #endregion

        #region Notify Preperties changes

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
