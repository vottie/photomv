using System;
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
using System.Diagnostics;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace Photomv
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger log = Logger.GetInstance("./photomv.log", true);
        SetupFile ini = new SetupFile("./photomv.ini");
        public MainWindow()
        {
            PhotoMVSingleton pmvMgr = PhotoMVSingleton.GetInstance();

            // write
            // ini["section", "key"] = "vallue";

            // read ini file
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
            string errfile = ini["section", "errfile"];
            string mode = ini["section", "mode"];

            // Set Parameter
            if (mode == "debug")
            {
                pmvMgr.Mode = "debug";
            }

            var now = System.DateTime.Now;
            string fmt = now.ToString("yyyyMMddHHmmss");
            // log file
            if (logfile == "")
            {
                ini["section", "logfile"] = "photomv_success.txt";
            }
            string ext = System.IO.Path.GetExtension(logfile);
            string tmpname = System.IO.Path.GetFileNameWithoutExtension(logfile);
            logfile = tmpname + "_" + fmt + ext;
            if (File.Exists(logfile) == false)
            {
                File.CreateText(logfile);
                pmvMgr.Logfile = logfile;
            }
            // error file
            if (errfile == "")
            {
                ini["section", "errfile"] = "photomv_error.txt";
            }

            string errtmpname = System.IO.Path.GetFileNameWithoutExtension(errfile);
            errfile = errfile + "_" + fmt + ext;
            if (File.Exists(errfile) == false)
            {
                File.CreateText(errfile);
                pmvMgr.Errfile = errfile;
            }

            //Stream log_file = File.Create("photomv_trace.log");
            //TextWriterTraceListener myTextListener = new TextWriterTraceListener(log_file);
            //TextWriterTraceListener myTextListener = new TextWriterTraceListener();
            //Trace.Listeners.Add(myTextListener);

            //Trace.Write("MainWindow start");

            InitializeComponent();
            log.Info("PhotoMV mode={0}", pmvMgr.Mode);

            CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, CommandExecuted)
            );

            if ((inDir1 != "") && (outDir1 != ""))
            {
                textBox1.Text = inDir1;
                textBox2.Text = outDir1;
            }
        }

        private void InputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog("入力フォルダ選択");
            dialog.IsFolderPicker = true;
            var ret = dialog.ShowDialog();

            if (ret == CommonFileDialogResult.Ok)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void OutputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog("出力フォルダ選択");
            dialog.IsFolderPicker = true;
            var ret = dialog.ShowDialog();

            if (ret == CommonFileDialogResult.Ok)
            {
                textBox2.Text = dialog.FileName;
            }
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            log.Info("PhotoMV exit");
            this.Close();    
        }


        void CommandExecuted(object sender, RoutedEventArgs e)
        {
            // = Logger.GetInstance()
            // MessageBox.Show("Hello world");
            string inDir = textBox1.Text;
            string outDir = textBox2.Text;
            bool isRename = (bool)RenameCheck.IsChecked;
            PhotoMVSingleton mgr = PhotoMVSingleton.GetInstance();
            mgr.IsRename = isRename;

            log.Info("MainWindow.CommandExecuted start");
            PhotoMVAction pmv = new PhotoMVAction(inDir, outDir);
            pmv.Execute();
            log.Info("MainWindow.CommandExecuted end");

            Trace.Flush();
        }

        protected virtual void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // record history
            ini["section", "input_dir1"] = textBox1.Text;
            ini["section", "output_dir1"] = textBox2.Text;
            return;
        }
    }
}