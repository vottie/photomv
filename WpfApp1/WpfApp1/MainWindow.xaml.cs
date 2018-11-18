using System;
using System.Collections.Generic;
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
            InitializeComponent();

            CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, CommandExecuted)
            );
        }

        void CommandExecuted(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello world");
            string inDir = textBox1.Text;
            string outDir = textBox2.Text;

            PhotoMVAction pmv = new PhotoMVAction(inDir, outDir);
            pmv.execute();
        }
    }
}