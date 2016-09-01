using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
//using Microsoft.Win32;

namespace TreeViewTest
{
    /// <summary>
    /// Start.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Start : Window
    {
        int cCount = 0;
        List<long> ctime = new List<long>();
        List<long> cdelay = new List<long>();

        List<long> saved = new List<long>();  // 저장된 데이터를 읽어와서 저장함

        int rCount = 0;
        List<long> rtime = new List<long>();  // 버튼 누를 당시의 시간을 기록함
        List<long> rdelay = new List<long>();  // 버튼과 버튼 사이의 딜레이를 기록함

        public int angle = 0;

        public Start()
        {
            InitializeComponent();
            string _Filestr = @"bin\key_pref";
            FileInfo fi = new FileInfo(_Filestr);
            if (fi.Exists == false)
            {
                MessageBox.Show("환영합니다! 자신만의 리듬을 등록해주세요!");
            }
        }

        public void Forget_Password(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(@"bin\BackUpPassword.txt"))
            {
                BackUpPasswordConfirm open = new BackUpPasswordConfirm();
                open.Show();
                this.Close();
            }
            else
                MessageBox.Show("이런! 비상용 암호가 등록되지 않았군요!");
        }  

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseDown += delegate
            {
                try
                { DragMove(); }
                catch (InvalidOperationException error)
                { return; }
            };
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Sex");
        }

        private void Close(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void Knock_Click(object sender, RoutedEventArgs e)
        {
            Asshole_Rotate();
            string _Filestr = @"bin\key_pref";
            FileInfo fi = new FileInfo(_Filestr);

            if (fi.Exists == true)
            {
                Button_Certificate();
            }
            else if(fi.Exists == false)
            {
                Button_Regist();
            }
        }

        private void go_Safe_Click(object sender, RoutedEventArgs e)
        {
            string _Filestr = @"bin\key_pref";
            FileInfo fi = new FileInfo(_Filestr);

            if (fi.Exists)
            {
                Compare();
            }
            else
            {
                SaveData();
                Clear();
                MessageBox.Show("당신만의 리듬이 등록되었습니다");
            }
            angle = 0;
            Console.WriteLine("Go inside");
        }

        private void Button_Certificate()   // delay 배열에 시간차 저장
        {
            long NowTime = DateTime.Now.Ticks / 1000;
            ctime.Add(NowTime);
            if (cCount >= 1)  // 데이터 수가 2개 이상이면
            {
                cdelay.Add(Tools.calibrate(ctime[cCount] - ctime[cCount - 1], 1300));  // n 번째 데이터에서 n-1번째 데이터를 빼서 딜레이에 넣는다. 
            }
            cCount++;  // 배열의 인덱스 1부터 시작 
        }

        public void Button_Regist()   // delay 배열에 시간차 저장
        {
            long NowTime = DateTime.Now.Ticks / 1000;
            rtime.Add(NowTime);
            if (rCount >= 1)  // 데이터 수가 2개 이상이면
            {
                rdelay.Add(Tools.calibrate(rtime[rCount] - rtime[rCount - 1], 1300));  // n 번째 데이터에서 n-1번째 데이터를 빼서 딜레이에 넣는다. 
            }
            rCount++;  // 배열의 인덱스 1부터 시작 
        }
        
        private void Compare()   // 비교
        {
            int same = 0;
            LoadData();
            List<float> savedPercent = getPercent(saved);
            List<float> savedCertificate = getPercent(cdelay);
            if(savedPercent.Count == savedCertificate.Count)
            {
                for (int i = 0; i <= savedPercent.Count - 1; i++)
                {
                    if (savedPercent[i] - savedCertificate[i] < 7 && savedPercent[i] - savedCertificate[i] > -7)
                        same++;
                }
                if (same >= (savedPercent.Count)-1)
                {
                    Tools.generateAESModule(cdelay);
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                else
                {
                    Clear();
                    MessageBox.Show("아닙니다! 다시 해보세요");
                }
            }
            else
            {
                Clear();
                MessageBox.Show("아닙니다! 다시 해보세요");
            }
        }

        public List<float> getPercent(List<long> delay)
        {
            long sum = delay.Sum();
            List<float> percent = new List<float>();
            foreach(long each in delay)
            {
                percent.Add(((float)each / (float)sum) * 100);
            }
            return percent;
        }

        public void Clear()
        {
            saved.Clear();
            ctime.Clear();
            cdelay.Clear();
            cCount = 0;
            rCount = 0;
            rdelay.Clear();
        }

        public void LoadData()
        {
            string[] data = File.ReadAllLines(Consts.file_path);
            foreach (string k in data)
                saved.Add(long.Parse(k));
        }

        public void SaveData()  // delay 배열에서 시간차 읽은 후 저장함
        {
            StreamWriter fw = new StreamWriter(Consts.file_path, false);
            for (int i = 0; i <= rCount - 2; i++)
            {
                fw.WriteLine(rdelay[i]);
            }
            fw.Close();
        }
        
        public void Asshole_Rotate()
        {
            DoubleAnimation da = new DoubleAnimation();
            RotateTransform rt = new RotateTransform();
            da.From = angle;
            da.To = angle+30;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.03));
            버튼.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, da);
            angle += 30;
        }

    }
}
