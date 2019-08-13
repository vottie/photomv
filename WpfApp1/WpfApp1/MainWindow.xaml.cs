﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using photomv;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SetupFile ini = new SetupFile("./photomv.ini");

            // write
            // ini["section", "key"] = "vallue";

            // read
            string inDir5 = ini["section", "input_dir5"];
            string inDir4 = ini["section", "input_dir4"];
            string inDir3 = ini["section", "input_dir3"];
            string inDir2 = ini["section", "input_dir2"];
            string inDir1 = ini["section", "input_dir1"];
            string outDir5 = ini["section", "output_dir5"];
            string outDir4 = ini["section", "output_dir4"];
            string outDir3 = ini["section", "output_dir3"];
            string outDir2 = ini["section", "output_dir2"];
            string outDir1 = ini["section", "output_dir1"];
            string logfile = ini["section", "logfile"];

            if (logfile == "")
            {
                ini["section", "logfile"] = "photomv.log";
            }
            if (File.Exists(logfile) == false)
            {
                File.CreateText(logfile);
            }
            Console.WriteLine(logfile);

            
            InitializeComponent();

            CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, CommandExecuted)
            );
        }

        void CommandExecuted(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("Hello world");
            string inDir = textBox1.Text;
            string outDir = textBox2.Text;

            PhotoMVAction pmv = new PhotoMVAction(inDir, outDir);
            pmv.execute();
        }
    }
}