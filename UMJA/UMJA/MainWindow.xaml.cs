using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

using Umja;

namespace UMJA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            //var parser = new Parser();
            //parser.Parse("C:\\Users\\forol\\OneDrive\\Desktop\\UmjaHoffentlichFunktioniertsJetzt\\UMJA\\UMJA");
            tb_pfolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        }

        public string CsvPathLuke { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  tb_file_Copy.Text = Path.GetFileName(tb_file_Copy3.Text);
            string filename = tb_file_Copy3.Text;
            string folder = tb_pfolder.Text;     //CSV
            if (tb_file_Copy3.Text == "" || tb_file_Copy3.Text == null)
            {
                MessageBox.Show("Filename field is empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                var parser = new Parser();
                parser.Parse(filename, folder);
                var compiler = new Compiler();
                compiler.ReadCsv(folder);
            }
            CsvPathLuke = folder;
        }
        private void Tb_file_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FileDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);
            }
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (null != e.Data && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = e.Data.GetData(DataFormats.FileDrop) as string[];
                string filename = ((string[])((DataObject)e.Data).GetData("FileName"))[0];
                // handle the files here!


                tb_file_Copy3.Text = filename;
            }
        }

        private void File_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;


            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to exit?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {

            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void Tb_pfolder_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.DefaultExt = ".graphml";
            openFileDialog.Filter = "graphml (*.graphml)|*.graphml";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)

            {
                tb_file_Copy3.Text = openFileDialog.FileName;

                //foreach (string filename in openFileDialog.FileNames)
                //    lbFiles.Items.Add(Path.GetFileName(filename));

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            //OpenFileDialog openFileDialog = new OpenFileDialog();

            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //if (openFileDialog.ShowDialog() == true)

            //{
            //    tb_pfolder.Text = Path.GetDirectoryName(openFileDialog.FileName);

            //    //foreach (string filename in openFileDialog.FileNames)
            //    //    lbFiles.Items.Add(Path.GetFileName(filename));


            OpenFileDialog folderBrowser = new OpenFileDialog();

            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == true)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                tb_pfolder.Text = folderPath;

            }




        }
    }
}

