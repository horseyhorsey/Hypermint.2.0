using MahApps.Metro.Controls;
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

namespace Hypermint.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        public Shell()
        {
            InitializeComponent();

            //var hsPath = Properties.Settings.Default.HyperSpinPath;

            //if (hsPath == string.Empty)
            //{
            //    MessageBox.Show("HyperSpin Path DOesnt exist");
            //    Properties.Settings.Default.HyperSpinPath = @"I:\HyperSpin";
            //    Properties.Settings.Default.Save();
            //    Application.Current.Shutdown();
            //}
        }
    }
}
