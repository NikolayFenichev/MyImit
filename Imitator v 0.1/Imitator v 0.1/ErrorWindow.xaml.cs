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
using System.Windows.Shapes;

/*Окно ошибки*/

namespace Imitator_v_0._1
{
    /// <summary>
    /// Логика взаимодействия для ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow(int numberUvs)
        {
            InitializeComponent();

            TextBlock txbNumberUvs = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16,
                Margin = new Thickness(62, 68, 60, 55),
            };

            txbNumberUvs.Text = Convert.ToString(numberUvs);

        }

        private void Button_KeyUp(object sender, KeyEventArgs e)
        {
            this.Close();
        }
    }
}
