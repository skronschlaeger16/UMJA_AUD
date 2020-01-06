using System;
using UMJA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        UMJA.MainWindow mwin = new MainWindow();

        [TestMethod]
        public void TestMethod1()
        {
            //use my Testfile

            /*string path = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
            }
            */

            var reader = new StreamReader(@"C:\Users\lukas\Desktop\filteredData.csv");
            //var reader = new StreamReader(path);

            string line = reader.ReadLine();//to skip first line
            line = reader.ReadLine();
            string[] split = line.Split(';');

            string folder = split[0];
            string className = split[1];
            string globals = split[2];
            string methods = split[3];
            bool getterSetter = bool.Parse(split[4].ToLower());
            string classType = split[5];
            List<string> getterNames = new List<string>();
            List<string> setterNames = new List<string>();

            Assert.Equals(folder, "net.htlgrieskirchen.pos2.plf.retrosteam.main");
            Assert.Equals(className, "Main");
            Assert.IsTrue(globals.Contains("- static SCANNER : Scanner"));
            Assert.IsTrue(globals.Contains("- static STORE : Store"));
            Assert.IsTrue(globals.Contains(" - static user : User"));
            Assert.IsTrue(methods.Contains("+ static main(args : String[]) : void"));
            Assert.IsFalse(getterSetter);
            Assert.Equals(classType, "Class");



        }
    }
}
