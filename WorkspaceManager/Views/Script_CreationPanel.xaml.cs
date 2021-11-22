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
        private bool pinned;
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
        public bool Pinned {
            get => pinned;
            set => SetProperty(ref pinned, value);
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

        private string DEFAULT_NAME => "Nuevo script";
        private string DEFAULT_DESCRIPTION => "";
        private string DEFAULT_SCRIPT => "echo 'Hello World!'";
        private Group DEFAULT_GROUP => new Group("Nuevo", new SolidColorBrush(Color.FromRgb(17, 166, 143)));

        public Script_CreationPanel(ObservableCollection<Group> groups, Group SelectedFilter = null) {
            DataContext = this;
            InitializeComponent();

            PanelTitle = "Crear acceso directo";
            SetDefaultValues();
            if (SelectedFilter != null) {
                SelectedGroupOption = SelectedFilter;
            }
            if (groups != null && groups.Count > 0) {
                groups.RemoveAt(0);
                ComboBoxGroupOptions = new ObservableCollection<Group>(groups.OrderBy(gr => gr.Name));
            } else {
                ComboBoxGroupOptions = new ObservableCollection<Group> {
                    DEFAULT_GROUP
                };
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
                Pinned = scriptToEdit.Pinned;
            }
            if (groups != null && groups.Count > 0) {
                groups.RemoveAt(0);
                ComboBoxGroupOptions = new ObservableCollection<Group>(groups.OrderBy(gr => gr.Name));
            }
        }
        private void SetDefaultValues() {
            NameText = DEFAULT_NAME;
            DescriptionText = DEFAULT_DESCRIPTION;
            Pinned = false;
            ScriptText = DEFAULT_SCRIPT;
            SelectedGroupOption = DEFAULT_GROUP;
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
                var newgrp = dialog.GetGroup();
                if (newgrp.Equals(CONSTANTS.AllGroup) || ComboBoxGroupOptions.Contains(newgrp)) {
                    MessageBox.Show("El grupo creado ya existe.", "Grupo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ComboBoxGroupOptions.Add(newgrp);
                ComboBoxGroupOptions = new ObservableCollection<Group>(ComboBoxGroupOptions.OrderBy(gr => gr.Name));
                SelectedGroupOption = newgrp;
            }
        }
        public Script GetScript() {
            // Set default values if empty
            if (string.IsNullOrWhiteSpace(NameText)) {
                NameText = DEFAULT_NAME;
            }
            if (string.IsNullOrWhiteSpace(DescriptionText)) {
                DescriptionText = DEFAULT_DESCRIPTION;
            }
            if (string.IsNullOrWhiteSpace(ScriptText)) {
                ScriptText = DEFAULT_SCRIPT;
            }
            return new Script(NameText, DescriptionText, ScriptText, SelectedGroupOption, Pinned);
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
