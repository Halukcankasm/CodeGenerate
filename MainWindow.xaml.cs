using CodeGenerate.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeGenerate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool generateBtnEnabled { get; set; }
        private string CodeGenerate { get; set; }
        private string VMCodeGenerate { get; set; }
        public string SelectedDataBase { get; set; }
        public string SelectedShema { get; set; }
        public string SelectedTable { get; set; }
        public ObservableCollection<string> DatabaseNames { get; set; }

        public ObservableCollection<string> tableNames;

        public ObservableCollection<string> TableNames
        {
            get
            {
                return tableNames;
            }
            set
            {
                tableNames = value;
                OnPropertyChanged("TableNames");
            }
        }


        public ObservableCollection<string> shemasName;

        public ObservableCollection<string> ShemasNames
        {
            get
            {
                return shemasName;
            }
            set
            {
                shemasName = value;
                OnPropertyChanged("ShemasNames");
            }
        }


        private MainVM vm;
        public MainWindow()
        {
            vm = new MainVM();
            DatabaseNames =  vm.GetDatabaseName();

            SelectedDataBase = "";
            SelectedShema = "";
            SelectedTable = "";
            CodeGenerate = "";
            VMCodeGenerate = "";
            generateBtnEnabled = false;

            TableNames = new ObservableCollection<string>();
            ShemasNames = new ObservableCollection<string>();

            InitializeComponent();
        }
        private void dbComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                SelectedDataBase = (sender as ComboBox).SelectedItem.ToString();

                ShemasNames.Clear();

                ShemasNames = vm.GetShemasNames(SelectedDataBase);
            }
            
        }

        private void SHComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                SelectedShema = (sender as ComboBox).SelectedItem.ToString();

                TableNames.Clear();
                TableNames = vm.GetTablesNames(SelectedDataBase, SelectedShema);
            }

        }
        private void TBComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                SelectedTable = (sender as ComboBox).SelectedItem.ToString();
            }
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedTable))
            {
                try
                {
                    CodeGenerate = vm.CodeGenerate(SelectedDataBase, SelectedShema, SelectedTable);
                    CreateCodeCs();

                    VMCodeGenerate = vm.VMCodeGenerate(SelectedDataBase, SelectedShema, SelectedTable);
                    VMCreateCodeCs();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Tablo Seçilmeli");
            }
        }

        private void CreateCodeCs()
        {
            string path = "D:\\Source\\CodeGenerate";
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }

            string currentPath = path + $"\\{SelectedTable}.cs";
            FileStream fs;

            if (!File.Exists(currentPath))
            {
                fs = new FileStream(currentPath, FileMode.Create);
            }
            else
            {
                fs = new FileStream(currentPath, FileMode.Append);
            }
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(CodeGenerate);
            sw.Close();
        }

        private void VMCreateCodeCs()
        {
            string path = "D:\\Source\\CodeGenerate";
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }

            string currentPath = path + $"\\{SelectedTable}VM.cs";
            FileStream fs;

            if (!File.Exists(currentPath))
            {
                fs = new FileStream(currentPath, FileMode.Create);
            }
            else
            {
                fs = new FileStream(currentPath, FileMode.Append);
            }
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(VMCodeGenerate);
            sw.Close();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion


    }
}
