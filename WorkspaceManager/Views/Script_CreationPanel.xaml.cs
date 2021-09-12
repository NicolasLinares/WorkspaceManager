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
using System.Linq;

namespace WorkspaceManagerTool.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Script_CreationPanel : UserControl, INotifyPropertyChanged {
        private string panelTitle;
        private string name;
        private string description;
        private string script;
        private Group selectedGroupOption;
        private ObservableCollection<Group> groupsOptions;

        public string PanelTitle {
            get => panelTitle;
            set => SetProperty(ref panelTitle, value);
        }
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
        public Group SelectedGroupOption {
            get => selectedGroupOption;
            set {
                SetProperty(ref selectedGroupOption, value);
            }
        }
        public ObservableCollection<Group> ComboBoxGroupOptions {
            get => groupsOptions;
            set => SetProperty(ref groupsOptions, value);
        }

        private string DefaultName => "Nuevo script";
        private string DefaultDescription => "";
        private string DefaultScript => "echo 'Hello World!'";
        private Group DefaultGroup => new Group("Nuevo", new SolidColorBrush(Color.FromRgb(17, 166, 143)));

        public Script_CreationPanel(ObservableCollection<Group> groups, Group SelectedFilter = null) {
            DataContext = this;
            InitializeComponent();

            PanelTitle = "Crear acceso directo";
            SetDefaultValues();
            if (SelectedFilter != null) {
                SelectedGroupOption = SelectedFilter;
            }
            if (groups != null && groups.Count > 0) {
                ComboBoxGroupOptions = new ObservableCollection<Group>(groups.OrderBy(gr => gr.Name));
            } else {
                ComboBoxGroupOptions = new ObservableCollection<Group>();
                ComboBoxGroupOptions.Add(DefaultGroup);
            }
        }
        public Script_CreationPanel(GroupableResource scriptToEdit, ObservableCollection<Group> groups) {
            DataContext = this;
            InitializeComponent();

            PanelTitle = "Editar acceso directo";
            SetDefaultValues();
            if (scriptToEdit != null) {
                NameText = scriptToEdit.Name;
                DescriptionText = scriptToEdit.Description;
                ScriptText = (scriptToEdit as Script).Commands;
                SelectedGroupOption = scriptToEdit.Group;
            }
            if (groups != null && groups.Count > 0) {
                ComboBoxGroupOptions = new ObservableCollection<Group>(groups.OrderBy(gr => gr.Name));
            }
        }
        private void SetDefaultValues() {
            NameText = DefaultName;
            DescriptionText = DefaultDescription;
            ScriptText = DefaultScript;
            SelectedGroupOption = DefaultGroup;
            ComboBoxGroupOptions = new ObservableCollection<Group>();
        }

        #region Handlers
        public event EventHandler HandlerClosePanel;
        public event EventHandler HandlerSaveChanges;
        #endregion

        #region Actions
        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectedGroupOption = (sender as ComboBox).SelectedItem as Group;
        }
        private void CreateGroup_Action(object sender, EventArgs e) {
            Group_CreationDialog dialog = new Group_CreationDialog();
            if (dialog.ShowDialog() == true) {
                var newgrp = new Group(dialog.NameText, dialog.ColorSelected);
                ComboBoxGroupOptions.Add(newgrp);
                ComboBoxGroupOptions = new ObservableCollection<Group>(ComboBoxGroupOptions.OrderBy(gr => gr.Name));
                SelectedGroupOption = newgrp;
            }
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
            return new Script(NameText, DescriptionText, ScriptText, SelectedGroupOption);
        }

        public void DescriptionCounter_Action(object sender, EventArgs e) {
            if (_DescriptionTextBox.Text.Length <= 0) {
                _DescriptionCounter.Text = string.Empty;
                return;
            }
            _DescriptionCounter.Text = string.Format("{0}/{1}", _DescriptionTextBox.Text.Length, _DescriptionTextBox.MaxLength);
        }

        private void ClosePanel_Action(object sender, EventArgs e) {
            this.Visibility = Visibility.Collapsed;
            HandlerClosePanel?.Invoke(this, e);
        }

        private void SaveChanges_Action(object sender, EventArgs e) {
            this.Visibility = Visibility.Collapsed;
            HandlerSaveChanges?.Invoke(this, e);
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
