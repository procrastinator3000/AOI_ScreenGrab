using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace AOI_ScreenGrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly List<Tuple<string, Screen>> _screens = new List<Tuple<string, Screen>>();

        AppData _data;
        string _lastScreenshotPath = String.Empty;
        string _rumunFilepath;

        public MainWindow()
        {
            InitializeComponent();
            AppSettings.Init();
            _data = AppData.Load();
            _rumunFilepath = AppSettings.Current.LogsDir + $"\\rumunskieRozwiazanie.txt";
            if (File.Exists(_rumunFilepath))
            {
                try
                {
                    _lastScreenshotPath = File.ReadAllText(_rumunFilepath);
                }
                catch (Exception)
                {

                }
            }
            Init();
        }

        void Init()
        {
            //_screens.Add(new Tuple<string, Screen>("Full", null));
            _screens.AddRange(Screen.AllScreens.Select(s => new Tuple<string, Screen>(s.DeviceName, s)));
            tbWO.Text = _data.WO;
            tbProduct.Text = _data.PROD;
            lblCounter.Content = "" + _data.NR;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var c = AppSettings.Current;

            if( _data == null || _data.WO != tbWO.Text)
            {
                _data = new AppData()
                {
                    WO = tbWO.Text,
                    PROD = tbProduct.Text,
                    NR = 0,
                };
            }

            _data.NR++;
            DateTime now = DateTime.Now;
            var filepath = c.Filepath
                            .Replace("{WO}", tbWO.Text)
                            .Replace("{LINE}", c.LineId)
                            .Replace("{PROD}", tbProduct.Text)
                            .Replace("{DATE}", now.ToString("ddMMyyyy-hhmmss"))
                            .Replace("{NR}", ""+_data.NR)
                            + ".jpg";
            var dirpath = Path.GetDirectoryName(filepath);
            Directory.CreateDirectory(dirpath);
            //var screen = _screens[cbScreenSelect.SelectedIndex].Item2;
            int screenid = AppSettings.Current.ScreenIndex;
            var screen = screenid<0 || screenid>= _screens.Count ? null : _screens[screenid].Item2;
            var rect = screen is null
                ? new System.Drawing.Rectangle((int)SystemParameters.VirtualScreenLeft, (int)SystemParameters.VirtualScreenTop, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight)
                : screen.WorkingArea;

            GrabScreen(rect.Left, rect.Top, rect.Width, rect.Height, filepath);
            _data.Save();
            LogToFile($"{now.ToString("yyyy.MM.dd hh:mm:ss")}|{tbWO.Text}|{tbProduct.Text}|{_data.NR}");
            _lastScreenshotPath = filepath;
            File.WriteAllText(_rumunFilepath, _lastScreenshotPath);
            lblCounter.Content = "" + _data.NR;
            if(c.ShowMessage)
                System.Windows.Forms.MessageBox.Show($"Zapisano zdjecie z numerem [{_data.NR}]");
            if(c.ShowPhoto)
                Poka_Fote();
        }

        void GrabScreen(int left, int top, int w, int h, string filepath)
        {
            using (Bitmap bmp = new Bitmap(w, h))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Opacity = .0;
                    g.CopyFromScreen(left, top, 0, 0, bmp.Size);
                    bmp.Save(filepath);
                    Opacity = 1;
                }
                
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(gridFields.Visibility == Visibility.Collapsed)
            {
                gridFields.Visibility = Visibility.Visible;
                btnToggle.Content = "v";
                this.Height = 170;
            }
            else
            {
                gridFields.Visibility = Visibility.Collapsed;
                btnToggle.Content = "^";
                this.Height = 115;
            }
        }

        void LogToFile(string message)
        {
            var filepath = AppSettings.Current.LogsDir+ $"\\{AppSettings.Current.LineId}_{DateTime.Now.ToString("yyMM")}_logs.log";
            try
            {
                File.AppendAllText(filepath, message+"\n");
            }catch(Exception ex)
            {

            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Poka_Fote();
        }

        private void Poka_Fote()
        {
            if (!string.IsNullOrWhiteSpace(_lastScreenshotPath))
                Process.Start(_lastScreenshotPath);
        }
    }
}
