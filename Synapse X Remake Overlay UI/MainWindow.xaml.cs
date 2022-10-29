using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;
using WeAreDevs_API;

namespace Synapse_X_Remake_Overlay_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        public static int ProcessIndex = 1;
        public static string ScriptsPath = "./../../Scripts";

        public MainWindow()
        {
            InitializeComponent();
            LoadWRD();
        }

        private void LoadEditorSyntax()
        {
            StreamReader reader = new StreamReader("./Editor/RblxLua.xml");
            XmlTextReader xmlReader = new XmlTextReader(reader);
            Editor.SyntaxHighlighting = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
        }

        private void LoadWRD()
        {
            if (wrd.IsUpdated() == false)
            {
                var msgbox = MessageBox.Show("WeAreDevs is not yet updated, Are you sure you want continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (msgbox == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    if (wrd.isAPIAttached() == false)
                    {
                        MessageBox.Show("WeAreDevs is not attached, Please try again ...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        SetTimer();
                        LoadScripts();
                        LoadSettings();
                        LoadEditorSyntax();
                    }
                }
            }
            else
            {
                if (wrd.isAPIAttached() == false)
                {
                    MessageBox.Show("WeAreDevs is not attached, Please try again ...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                else
                {
                    SetTimer();
                    LoadScripts();
                    LoadSettings();
                    LoadEditorSyntax();
                }
            }
        }

        private void LoadSettings()
        {
            Editor.Text = Properties.Settings.Default["EditorSave"].ToString();
        }

        private void LoadScripts()
        {
            foreach (var file in Directory.GetFiles(ScriptsPath, "*.txt"))
            {
                ScriptBox.Items.Add(Path.GetFileName(file));
            }

            foreach (var file in Directory.GetFiles(ScriptsPath, "*.lua"))
            {
                ScriptBox.Items.Add(Path.GetFileName(file));
            }
        }

        public void SetTimer()
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
                this.Left = WindowRect.Left + 30;

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

        private void ScriptHubButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ScriptHubWindow scriptHub = new ScriptHubWindow();
                scriptHub.Show();
            }
            catch (Exception error)
            {
                var option = MessageBox.Show($"{error.Message}\n\nDo you still want to continue?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (option == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            wrd.SendLuaScript(Editor.Text);
        }

        public static bool Collapsed = false;
        public static double CurrentHeight;

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Collapsed == false)
            {
                Collapsed = true;
                CollapseButton.Content = "Expand";

                this.ResizeMode = ResizeMode.NoResize;
                CurrentHeight = this.ActualHeight;

                CircleEase ease = new CircleEase();
                ease.EasingMode = EasingMode.EaseInOut;
                DoubleAnimation HeightAnimation = new DoubleAnimation();
                HeightAnimation.From = this.ActualHeight;
                HeightAnimation.To = 80;
                HeightAnimation.EasingFunction = ease;
                HeightAnimation.Duration = TimeSpan.FromMilliseconds(500);
                this.BeginAnimation(HeightProperty, HeightAnimation);

                DoubleAnimation OpacityAnimation = new DoubleAnimation();
                OpacityAnimation.From = 1;
                OpacityAnimation.To = 0;
                OpacityAnimation.EasingFunction = ease;
                OpacityAnimation.Duration = TimeSpan.FromMilliseconds(500);

                Editor.BeginAnimation(OpacityProperty, OpacityAnimation);
                ScriptBox.BeginAnimation(OpacityProperty, OpacityAnimation);
            }
            else
            {
                Collapsed = false;
                CollapseButton.Content = "Collapse";

                this.ResizeMode = ResizeMode.CanResizeWithGrip;

                CircleEase ease = new CircleEase();
                ease.EasingMode = EasingMode.EaseInOut;
                DoubleAnimation HeightAnimation = new DoubleAnimation();
                HeightAnimation.From = 80;
                HeightAnimation.To = CurrentHeight;
                HeightAnimation.EasingFunction = ease;
                HeightAnimation.Duration = TimeSpan.FromMilliseconds(500);
                this.BeginAnimation(HeightProperty, HeightAnimation);

                DoubleAnimation OpacityAnimation = new DoubleAnimation();
                OpacityAnimation.From = 0;
                OpacityAnimation.To = 1;
                OpacityAnimation.EasingFunction = ease;
                OpacityAnimation.Duration = TimeSpan.FromMilliseconds(500);

                Editor.BeginAnimation(OpacityProperty, OpacityAnimation);
                ScriptBox.BeginAnimation(OpacityProperty, OpacityAnimation);
            }
        }

        private void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            ScriptBox.Items.Clear();
            LoadScripts();
        }

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptBox.SelectedIndex != -1)
            {
                string content = File.ReadAllText($"{ScriptsPath}/{ScriptBox.SelectedItem}");
                Editor.Text = content;
            }
        }

        private void ExecuteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptBox.SelectedIndex != -1)
            {
                string content = File.ReadAllText($"{ScriptsPath}/{ScriptBox.SelectedItem}");
                wrd.SendLuaScript(content);
            }
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["EditorSave"] = Editor.Text;
            Properties.Settings.Default.Save();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Editor.Text = "";
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == true)
            {
                string content = File.ReadAllText(openFile.FileName);
                Editor.Text = content;
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Text Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
            saveFile.RestoreDirectory = true;

            if (saveFile.ShowDialog() == true)
            {
                File.WriteAllText(saveFile.FileName, Editor.Text);
            }
        }
    }
}
