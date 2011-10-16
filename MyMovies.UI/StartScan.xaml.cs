using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Helper;

namespace MyMovies
{
    /// <summary>
    /// Interaction logic for StartScan.xaml
    /// </summary>
    public partial class StartScan : Window
    {
        public StartScan()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Reactor.Run(
                TbPath.Text,
                path => Scanner.GetFiles(path).Select(Scanner.ParseMovieName).ToList(),
                movies =>{
                    if (movies.Any())
                    {
                        new Scan(movies).Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("No movie found");
                    }
                },
                ex => MessageBox.Show(ex.Message),
                null);
        }
    }
}
