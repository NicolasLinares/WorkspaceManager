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
using FolderQuickAccess = WorkspaceManagerTool.Models.QuickAccess;
using WorkspaceManagerTool.Models;
using Microsoft.Win32;

namespace WorkspaceManagerTool.Views {


    public partial class QuickAccess_CreationPanel : UserControl, INotifyPropertyChanged {
        private string panelTitle;
        private string name;
        private QuickAccessType resourceType;
        private string path;
        private string description;
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
        public QuickAccessType ResourceType {
            get => resourceType;
            set => SetProperty(ref resourceType, value);
        }
        public string PathText {
            get => path;
            set => SetProperty(ref path, value);
        }
        public string DescriptionText {
            get => description;
            set => SetProperty(ref description, value);
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


        private string DefaultName => "Nuevo acceso directo";
        private QuickAccessType DefaultResourceType => QuickAccessType.DIRECTORY;
        private string DefaultPath => "Ruta sin definir";
        private string DefaultDescription => "";
        private Group DefaultGroup => new Group("Nuevo", new SolidColorBrush(Color.FromRgb(17, 166, 143)));

        public QuickAccess_CreationPanel(ObservableCollection<Group> groups, Group SelectedFilter = null) {
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
        public QuickAccess_CreationPanel(GroupableResource qaToEdit, ObservableCollection<Group> groups) {
            DataContext = this;
            InitializeComponent();

            PanelTitle = "Editar acceso directo";
            SetDefaultValues();
            if (qaToEdit != null) {

                _PathText.Text = (qaToEdit as QuickAccess).Path;
                NameText = qaToEdit.Name;
                DescriptionText = qaToEdit.Description;
                SelectedGroupOption = qaToEdit.Group;
            }
            if (groups != null && groups.Count > 0) {
                ComboBoxGroupOptions = new ObservableCollection<Group>(groups.OrderBy(gr => gr.Name));
            }
        }
        private void SetDefaultValues() {
            NameText = DefaultName;
            DescriptionText = DefaultDescription;
            ResourceType = DefaultResourceType;
            _PathText.Text = DefaultPath;
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

        private void DescriptionCounter_Action(object sender, EventArgs e) {
            if (_DescriptionTextBox.Text.Length <= 0) {
                _DescriptionCounter.Text = string.Empty;
                return;
            }
            _DescriptionCounter.Text = string.Format("{0}/{1}", _DescriptionTextBox.Text.Length, _DescriptionTextBox.MaxLength);
        }

        private void ChangeResourceType_Action(object sender, EventArgs e) {
            if (ResourceType == QuickAccessType.FILE) {
                ResourceType = QuickAccessType.DIRECTORY;
                _ResourceTypeSwitch_Button.Content = FindResource("Folder");
                _ResourceTypeSwitch_Button.ToolTip = "Acceso directo a una carpeta";
                return;
            }
            ResourceType = QuickAccessType.FILE;
            _ResourceTypeSwitch_Button.Content = FindResource("File");
            _ResourceTypeSwitch_Button.ToolTip = "Acceso directo a un fichero";
        }

        private void CheckResourceType_Action(object sender, EventArgs e) {
            PathText = _PathText.Text;
            try {
                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(PathText);
                //detect whether its a directory or file
                if (attr.HasFlag(FileAttributes.Directory)) {
                    if (ResourceType != QuickAccessType.DIRECTORY) {
                        ChangeResourceType_Action(this, new EventArgs());
                    }
                    return;
                }

                if (ResourceType != QuickAccessType.FILE) {
                    ChangeResourceType_Action(this, new EventArgs());
                    return;
                }
            } catch (Exception) {
                return;
            }
        }

        private void Browse_Action(object sender, EventArgs e) {
            if (ResourceType == QuickAccessType.FILE) {
                BrowseFile();
                return;
            }
            BrowseDirectory();

            void BrowseDirectory() {
                VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                fbd.Description = "Seleccionar carpeta";
                fbd.SelectedPath = PathText;
                fbd.UseDescriptionForTitle = true;
                fbd.ShowNewFolderButton = true;
                var result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    PathText = fbd.SelectedPath;
                }
            }
            void BrowseFile() {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = null; // Filter files by extension
                dlg.Title = "Seleccionar fichero";
                dlg.InitialDirectory = PathText;
                var result = dlg.ShowDialog();
                if (result == true) {
                    PathText = dlg.FileName;
                }
            }
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
        public GroupableResource GetQuickAccess() {
            // Set default values if empty
            if (string.IsNullOrWhiteSpace(NameText)) {
                NameText = DefaultName;
            }
            if (string.IsNullOrWhiteSpace(PathText)) {
                PathText = DefaultPath;
            }
            if (SelectedGroupOption == null) {
                SelectedGroupOption = DefaultGroup;
            }
            if (string.IsNullOrWhiteSpace(DescriptionText)) {
                DescriptionText = DefaultDescription;
            }
            return new FolderQuickAccess(PathText, NameText, DescriptionText, SelectedGroupOption, ResourceType);
        }

        private void ClosePanel_Action(object sender, EventArgs e) {
            this.Visibility = Visibility.Collapsed;
            HandlerClosePanel?.Invoke(this, e);
        }

        private void SaveChanges_Action(object sender, EventArgs e) {
            CheckResourceType_Action(this, new EventArgs());

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
