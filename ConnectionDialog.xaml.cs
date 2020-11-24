namespace EntityFramework6CoreContextDriver
{
    using System;
    using System.IO;
    using System.Windows;
    using LINQPad.Extensibility.DataContext;

    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window
    {
        IConnectionInfo _cxInfo;

        public ConnectionDialog(IConnectionInfo cxInfo)
        {
            _cxInfo = cxInfo;
            DataContext = cxInfo;
            InitializeComponent();
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        void BrowseAssembly(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = "Choose custom assembly",
                DefaultExt = ".dll",
            };

            if (dialog.ShowDialog() == true)
                _cxInfo.CustomTypeInfo.CustomAssemblyPath = dialog.FileName;
        }

        public IDatabaseInfo DatabaseInfo => _cxInfo.DatabaseInfo;

        public bool Persist
        {
            get => _cxInfo.Persist;
            set => _cxInfo.Persist = value;
        }

        public string DisplayName
        {
            get => _cxInfo.DisplayName;
            set
            {
                if (_cxInfo.DisplayName == value) return;
                _cxInfo.DisplayName = DisplayName;
            }
        }

        public ICustomTypeInfo CustomTypeInfo => _cxInfo.CustomTypeInfo;

        void ChooseType(object sender, RoutedEventArgs e)
        {
            string assemPath = _cxInfo.CustomTypeInfo.CustomAssemblyPath;
            if (assemPath.Length == 0)
            {
                MessageBox.Show("First enter a path to an assembly.");
                return;
            }

            if (!File.Exists(assemPath))
            {
                MessageBox.Show("File '" + assemPath + "' does not exist.");
                return;
            }

            string[] customTypes;
            try
            {
                customTypes = _cxInfo.CustomTypeInfo.GetCustomTypesInAssembly("System.Data.Entity.DbContext");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obtaining custom types: " + ex.Message);
                return;
            }

            if (customTypes.Length == 0)
            {
                MessageBox.Show("There are no public types in that assembly."); // based on.........
                return;
            }

            string result =
                (string) LINQPad.Extensibility.DataContext.UI.Dialogs.PickFromList("Choose Custom Type", customTypes);
            if (result != null) _cxInfo.CustomTypeInfo.CustomTypeName = result;
        }
    }
}