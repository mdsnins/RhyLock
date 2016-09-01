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
using System.Windows.Shapes;

namespace TreeViewTest
{
    /// <summary>
    /// BackUpPasswordConfirm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BackUpPasswordConfirm : Window
    {
        public BackUpPasswordConfirm()
        {
            InitializeComponent();
        }

        private string encrypt(string pw)
        {
            AESCrypt aesc = new AESCrypt(Tools.generateKey(pw.GetHashCode()), Tools.generateIV(pw.GetHashCode()));
            return aesc.EncryptString(pw);
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Drag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseDown += delegate
            {
                try
                {
                    DragMove();
                }
                catch (InvalidOperationException error)
                {
                    return;
                }
            };
        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            button.Source = new BitmapImage(new Uri(@"\Images\Card_0001_회색-버튼-2.png", UriKind.Relative));
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            button.Source = new BitmapImage(new Uri(@"\Images\Card_0002_회색-버튼-1.png", UriKind.Relative));
        }

        private void button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StreamReader reader = new StreamReader(@"bin\BackUpPassword.txt", System.Text.Encoding.Default);
            string BackUpPassword = reader.ReadToEnd();

            string password = encrypt(textBox.Text);
            Console.WriteLine(password);
            Console.WriteLine(BackUpPassword);

            reader.Close();
            if(password + "\r\n" == BackUpPassword)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                Start start = new Start();
                start.Show();
                this.Close();
            }
        }
    }
}
