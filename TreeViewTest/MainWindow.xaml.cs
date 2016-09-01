using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Interop;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Controls;

namespace TreeViewTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        AESCrypt aes;
        string DecryptDirectory;

        //Drag and drop handler
        QueryContinueDragEventHandler queryhandler;
        //Starting Drag point
        Point _startPoint;
        int WindowPageNum = 16;

        public List<String> FileDirectory = new List<string>();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void null_event(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            return;
        }

        public MainWindow()
        {
            InitializeComponent();
            List<long> a = new List<long>();
            a.Add(1234);
            Tools.generateAESModule(a);
            aes = Tools.getAESModule();

            //creating handler for Drag and Drop 
            queryhandler = new QueryContinueDragEventHandler(DragSourceQueryContinueDrag);
            Open();
        }

        public void Open()
        {
            StreamReader directory;
            int i = 0;
            string line;
            try
            {
                directory = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"bin\directory.txt");
            }
            catch (FileNotFoundException e)
            {
                return;
            }
            //String[] AllDirectories = new string[100];

            while ((line = directory.ReadLine()) != null)
            {
                if (i < WindowPageNum && i >= WindowPageNum - 16)
                    Add_Rect(line);
                FileDirectory.Add(line);
                i++;
            }

            directory.Close();
            //File.Delete("directory.txt");
        }

        private void Close(object sender, MouseEventArgs e)
        {
            File.Delete(@"bin\directory.txt");
            SaveDirectory();
            this.Close();
        }

        private void ChangePassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            File.Delete(@"bin\key_pref");
            Start start = new Start();
            start.Show();
            this.Close();
        }

        private void BackUp_Password_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackUpPassword backup = new BackUpPassword();
            backup.Show();
        }  

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowPageNum <= 16)
                return;
            else
                WindowPageNum -= 16;
            int i = 0;
            wrapPanel1.Children.Clear();
            string[] filenames = FileDirectory.ToArray();
            foreach (string filename in filenames)
            {
                if (i < WindowPageNum && i >= WindowPageNum - 16)
                {
                    if (filename.EndsWith(".enc"))
                        Add_Rect(filename);
                    else
                        Add_Rect(filename + ".enc");
                }
                i++;
            }
        }

        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowPageNum += 16;
            int i = 0;
            string[] filenames = FileDirectory.ToArray();
            wrapPanel1.Children.Clear();
            foreach (string filename in filenames)
            {
                if (i < WindowPageNum && i >= WindowPageNum - 16)
                {
                    if (filename.EndsWith(".enc"))
                        Add_Rect(filename);
                    else
                        Add_Rect(filename + ".enc");
                }
                i++;
            }
        }

        private void FileShowTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void FileShowTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            try
            {
                foreach (string filename in filenames)
                {
                    if (FileDirectory.Count < WindowPageNum && FileDirectory.Count >= WindowPageNum - 16) //0~15까지 16개
                    {
                        Add_Rect(filename);
                    }
                    FileDirectory.Add(filename);
                    Lock_Click(filename);
                }
            }
            catch(NullReferenceException error)
            {
                return;
            }
            e.Handled = true;
        }

        private void Lock_Click(string filename)
        {
            string DecryptDirectory = filename;
            if (Tools.isDirectory(DecryptDirectory))
            {
                string[] subfiles = Tools.getAllFileList(DecryptDirectory);
                foreach (string subfile in subfiles)
                {
                    if (subfile.Substring(subfile.Length - 4).CompareTo(".enc") == 0)
                    {
                        aes.DecryptFile(subfile);
                        File.Delete(subfile);
                    }
                    else
                    {
                        aes.EncryptFile(subfile);
                        File.Delete(subfile);
                    }
                }
            }
            else
            {
                if (DecryptDirectory.Substring(DecryptDirectory.Length - 4).CompareTo(".enc") == 0)
                {
                    aes.DecryptFile(DecryptDirectory);
                    File.Delete(DecryptDirectory);
                }
                else
                {
                    aes.EncryptFile(DecryptDirectory);
                    File.Delete(DecryptDirectory);
                }
            }
        }

        public string SelectedImagePath { get; set; }

        private void Add_Rect(string directory)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Height = 120;
            rectangle.Width = 135;
            rectangle.Tag = directory;
            rectangle.AllowDrop = true;
            rectangle.Fill = new ImageBrush {ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(@"bin\Door_0015_모서리가-둥근-직사각형-2-사본.png", UriKind.Relative))};

            TextBlock txBlock = new TextBlock();
            txBlock.Width = 125;
            txBlock.TextTrimming = TextTrimming.CharacterEllipsis;
            txBlock.Text = Tools.getFileNameFromPath(directory);
            txBlock.Foreground = new SolidColorBrush(Colors.Black);
            txBlock.Tag = rectangle.Tag;
            txBlock.TextAlignment = TextAlignment.Center;

            Canvas cv = new Canvas();
            cv.Height = 127;
            cv.Width = 135;
            cv.Tag = directory;
            cv.MouseMove += Window_MouseMove;
            Canvas.SetZIndex(cv, 7);
            cv.Children.Add(rectangle);
            cv.Children.Add(txBlock);
            Canvas.SetBottom(txBlock, 0);

            wrapPanel1.Children.Add(cv);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this._startPoint = e.GetPosition(null);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //drag is heppen 
            //Prepare for Drag and Drop 
            Point mpos = e.GetPosition(null);
            Vector diff = this._startPoint - mpos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || 
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                //hooking on Mouse Up
                InterceptMouse.m_hookID = InterceptMouse.SetHook(InterceptMouse.m_proc);

                //ataching the event for hadling drop
                this.QueryContinueDrag += queryhandler;
                //begin drag and drops 

                DataObject dataObj = new DataObject(this.text1);
                if ((sender as Canvas) == null)
                    return;
                if ((sender as Canvas).Tag.ToString().EndsWith(".enc"))
                    DecryptDirectory = (sender as Canvas).Tag.ToString();
                else if(Tools.isDirectory((sender as Canvas).Tag.ToString()))
                    DecryptDirectory = (sender as Canvas).Tag.ToString();
                else
                    DecryptDirectory = (sender as Canvas).Tag.ToString() + ".enc";
                DragDrop.DoDragDrop(this.text1, dataObj, DragDropEffects.Move);
                if(!File.Exists(DecryptDirectory))
                {
                    FileDirectory.RemoveAll(item => item == (sender as Canvas).Tag.ToString());
                    (sender as Canvas).Height = 0;
                    (sender as Canvas).Width = 0;
                    (sender as Canvas).Tag = null;
                    (sender as Canvas).Children.Clear();
                }
            }
        }

        private Brush GetIcon(string path)
        {
            System.Drawing.Bitmap x = ShellEx.GetBitmapFromFilePath(path, ShellEx.IconSizeEnum.ExtraLargeIcon);
            var btSource = Imaging.CreateBitmapSourceFromHBitmap(x.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            return new ImageBrush(btSource);
        }

        /// <summary>
        /// Continuosly tracking Dragging mouse position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragSourceQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //when keystate is non, draop is heppen
            if (e.KeyStates == DragDropKeyStates.None)
            {
                //unsubscribe event
                this.QueryContinueDrag -= queryhandler;
                e.Handled = true;
                //Unhooking on Mouse Up
                InterceptMouse.UnhookWindowsHookEx(InterceptMouse.m_hookID);

                //notifiy user about drop result
                Task.Run
                    (
                        () =>
                        {
                            //Drop hepend outside Instantly app
                            if (InterceptMouse.IsMouseOutsideApp)
                            {
                                if (Tools.isDirectory(DecryptDirectory))
                                {
                                    string[] subfiles = Tools.getAllFileList(DecryptDirectory);
                                    foreach (string subfile in subfiles)
                                    {
                                        if (subfile.Substring(subfile.Length - 4).CompareTo(".enc") == 0)
                                        {
                                            aes.DecryptFile(subfile);
                                            File.Delete(subfile);
                                        }
                                    }
                                }
                                else
                                {
                                    if (DecryptDirectory.Substring(DecryptDirectory.Length - 4).CompareTo(".enc") == 0)
                                    {
                                        aes.DecryptFile(DecryptDirectory);
                                        File.Delete(DecryptDirectory);
                                    }
                                }
                            }
                        }
                    );
            }
        }

        public void SaveDirectory()
        {

            FileStream directory = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + @"\bin\directory.txt");
            String Directory;
            //IEnumerable<Canvas> rectangles = wrapPanel1.Children.OfType<Canvas>();
            String[] rectangles = FileDirectory.ToArray();

            foreach (var rect in rectangles)
            {
                try
                {   
                    if(rect.ToString() != null)
                    {
                        byte[] bt = System.Text.UTF8Encoding.UTF8.GetBytes(rect.ToString() + "\r\n");
                        //Directory = Encoding.Default.GetString(bt);
                        Directory = rect.ToString();
                        if (Directory.EndsWith(".enc"))
                        {
                            bt = System.Text.UTF8Encoding.UTF8.GetBytes(rect.ToString() + "\r\n");
                        }
                        else
                            bt = System.Text.UTF8Encoding.UTF8.GetBytes(rect.ToString() + ".enc" + "\r\n");
                        directory.Write(bt, 0, bt.Length);
                    } 
                }
                catch (NullReferenceException e)
                {
                    return;
                }
            }

            directory.Close();
        }

    }
}

