using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace TreeViewTest
{
    /// <summary>
    /// Tutorial.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Tutorial : Window
    {
        int state = 0;

        public Tutorial()
        {
            InitializeComponent();
            
            string _Filestr = @"bin\key_pref";
            FileInfo fi = new FileInfo(_Filestr);
            if (fi.Exists == true)
            {
                Start start = new Start();
                start.Show();
                this.Close();
            }
        }

        void a1(object sender, RoutedEventArgs e)
        {
            //this.a.Stop();
            this.a.Position = TimeSpan.FromMilliseconds(1);
            //this.a.Play();
        }

        void b1(object sender, RoutedEventArgs e)
        {
            //this.b.Stop();
            this.b.Position = TimeSpan.FromMilliseconds(1);
            //this.b.Play();
        }

        void c1(object sender, RoutedEventArgs e)
        {
            //this.c.Stop();
            this.c.Position = TimeSpan.FromMilliseconds(1);
            //this.c.Play();
        }

        void d1(object sender, RoutedEventArgs e)
        {
            //this.d.Stop();
            this.d.Position = TimeSpan.FromMilliseconds(1);
            //this.d.Play();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseDown += delegate
            {
                try
                { DragMove(); }
                catch (InvalidOperationException error)
                { return; }
            };
        }

        private void exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(state <= -376)
                state += 376;
            DoubleAnimation MoveAnimation = Resources["MoveAnimation"] as DoubleAnimation;
            MoveAnimation.To = state;
            Panels.BeginAnimation(Canvas.LeftProperty, MoveAnimation, HandoffBehavior.Compose);
        }

        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(state >= -376*5)
                state -= 376;
            else
            {
                Start start = new Start();
                start.Show();
                this.Close();
            }
            DoubleAnimation MoveAnimation = Resources["MoveAnimation"] as DoubleAnimation;
            MoveAnimation.To = state;
            Panels.BeginAnimation(Canvas.LeftProperty, MoveAnimation, HandoffBehavior.Compose);
        }
    }
}
