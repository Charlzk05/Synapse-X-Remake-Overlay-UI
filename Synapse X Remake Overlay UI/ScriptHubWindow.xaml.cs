using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;
using WeAreDevs_API;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Synapse_X_Remake_Overlay_UI
{
    /// <summary>
    /// Interaction logic for ScriptHubWindow.xaml
    /// </summary>
    public partial class ScriptHubWindow : Window
    {
        ExploitAPI wrd = new ExploitAPI();

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect lprect);

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hwnd);

        public static string RobloxWindow = "RobloxPlayerBeta";
        public static int ProcessIndex = 0;
        static string json = File.ReadAllText("./../ScriptHub.json");
        JObject value = JObject.Parse(json);

        public ScriptHubWindow()
        {
            InitializeComponent();
            SetTimer();
            LoadScripts();
        }

        private void LoadScripts()
        {
            foreach (var item in value["Scripts"])
            {
                ScriptBox.Items.Add(item["Title"]);
            }
        }

        private void SetTimer()
        {
            this.Opacity = 0.2;
            DispatcherTimer UIOverlayer = new DispatcherTimer();
            UIOverlayer.Interval = TimeSpan.Zero;
            UIOverlayer.Tick += UIOverlayer_Tick;
            UIOverlayer.Start();
        }

        private void UIOverlayer_Tick(object sender, EventArgs e)
        {
            try
            {
                Process process = Process.GetProcessesByName(RobloxWindow)[ProcessIndex];
                IntPtr hwnd = process.MainWindowHandle;
                Rect WindowRect = new Rect();
                GetWindowRect(hwnd, ref WindowRect);
                this.Top = WindowRect.Top + 50;
                this.Left = WindowRect.Right - 412;

                if (IsIconic(hwnd) == true)
                {
                    this.WindowState = WindowState.Minimized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            }
            catch (Exception)
            {
                Application.Current.Shutdown();
            }
        }

        private void ScriptBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScriptBox.SelectedIndex != -1)
            {
                ImageBox.Visibility = Visibility.Hidden;
                StatusBox.Text = "";
                StatusBox.Visibility = Visibility.Visible;
                BitmapImage image = new BitmapImage();
                image.DownloadProgress += Image_DownloadProgress;
                image.DownloadCompleted += Image_DownloadCompleted;
                image.BeginInit();
                image.UriSource = new Uri(value["Scripts"][ScriptBox.SelectedIndex]["Img"].ToString(), UriKind.Absolute);
                image.EndInit();

                ImageBox.Source = image;
            }
        }

        private void Image_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            StatusBox.Text = $"Loading ... {e.Progress}%";
        }

        private void Image_DownloadCompleted(object sender, EventArgs e)
        {
            ImageBox.Visibility = Visibility.Visible;
            StatusBox.Visibility = Visibility.Hidden;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Hide();
            main.Show();
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptBox.SelectedIndex != -1)
            {
                string content = value["Scripts"][ScriptBox.SelectedIndex]["Content"].ToString();
                MessageBox.Show(content);
            }
        }
    }
}
