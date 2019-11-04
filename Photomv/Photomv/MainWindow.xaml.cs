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

        private static Logger log = Logger.GetInstance(Common.TRCFILE, true);
        SetupFile ini = new SetupFile(Common.INIFILE);
        public MainWindow()
        {
            PhotoMVSingleton pmvMgr = PhotoMVSingleton.GetInstance();

            // write
            // ini["section", "key"] = "vallue";

            // read ini file
            string inDir1  = ini["section", "input_dir1"];
            string outDir1 = ini["section", "output_dir1"];
            string logfile = ini["section", "logfile"];
            string errfile = ini["section", "errfile"];
            string mode = ini["section", "mode"];

            /**
            // Set Parameter
            if (mode == "debug")
            {
                pmvMgr.Mode = "debug";
            }
            else
            {
                log.LogLv = Logger.Level.INFO;
            }
            */

            var now = System.DateTime.Now;
            string fmt = now.ToString("yyyyMMdd_HHmmss");
            // log file
            if (logfile == "")
            {
                ini["section", "logfile"] = Common.LOGILE;
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
                ini["section", "errfile"] = Common.ERRFILE;
            }

            string errtmpname = System.IO.Path.GetFileNameWithoutExtension(errfile);
            errfile = errfile + "_" + fmt + ext;
            if (File.Exists(errfile) == false)
            {
                File.CreateText(errfile);
                pmvMgr.Errfile = errfile;
            }

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
            string inDir = textBox1.Text;
            string outDir = textBox2.Text;
            bool isRename = (bool)RenameCheck.IsChecked;
            bool isDebug = (bool)Test.IsChecked;
            PhotoMVSingleton mgr = PhotoMVSingleton.GetInstance();
            mgr.IsRename = isRename;
            if (isDebug)
            {
                mgr.Mode = "debug";
                log.LogLv = Logger.Level.DEBUG;
            }
            else
            {
                log.LogLv = Logger.Level.INFO;
            }

            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;

            log.Info("MainWindow.CommandExecuted start PID={0} TID={1}", pid, tid);
            PhotoMVAction pmv = new PhotoMVAction(inDir, outDir);
            Task<int> task = Task.Run(() =>
            {
                return pmv.Execute();
            });
            log.Info("MainWindow.CommandExecuted end");

            Trace.Flush();

            if (PhotoMVSingleton.GetInstance().Result == 0)
            {
                resultmsg.Text = "正常終了しました。";
            }
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