using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace VoiceAssistant
{
    public partial class MainForm : Form
    {
        // Static instance for access by other forms.
        public static MainForm Instance { get; private set; }
        // Array to store up to 10 hyperlinks.
        public static string[] StoredLinks { get; set; } = new string[10];

        // Import functions to register/unregister global hotkeys.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Import function for simulating key presses.
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // Constants for simulating key presses.
        private const byte VK_LWIN = 0x5B;      // Left Windows key code
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        // Fields for tray icon, speech recognition, and synthesis.
        private NotifyIcon trayIcon;
        private SpeechRecognitionEngine recognizer;
        private SpeechSynthesizer synthesizer;
        private bool isListening = false;

        // Hotkey constants.
        private const int HOTKEY_ID = 1;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int WM_HOTKEY = 0x0312;

        // Constructor.
        public MainForm()
        {
            InitializeComponent();
            Instance = this;          // Set static instance.
            LoadLinks();              // Load stored links from file "link.txt".
            SetupVoiceRecognition();
            SetupTrayIcon();
        }

        // Public methods for controlling voice recognition from UI.
        public void StartListening()
        {
            if (!isListening)
                ToggleListening();
        }

        public void StopListening()
        {
            if (isListening)
                ToggleListening();
        }

        // Methods for key commands.
        public void MinimizeAllWindow()
        {
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            SendKeys.SendWait("m");
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public void ShowDesktop()
        {
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            SendKeys.SendWait("d");
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public void LockScreen()
        {
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            SendKeys.SendWait("l");
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public void OpenNewWindow()
        {
            SendKeys.SendWait("^(n)"); // Simulate Ctrl+N.
        }

        public void OpenNewTab()
        {
            SendKeys.SendWait("^(t)"); // Simulate Ctrl+T.
        }

        // Global hotkey registration.
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.Z))
            {
                MessageBox.Show("Failed to register hotkey.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            base.OnHandleDestroyed(e);
        }

        // Run hidden.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        // Set up voice recognition.
        private void SetupVoiceRecognition()
        {
            try
            {
                recognizer = new SpeechRecognitionEngine();
                synthesizer = new SpeechSynthesizer();

                Choices commands = new Choices();
                commands.Add(new string[] {
                    "what time is it",
                    "open notepad",
                    "close notepad",
                    "minimize all windows",
                    "show desktop",
                    "lock screen",
                    "open calculator",
                    "open browser",
                    "open task manager",
                    "shutdown computer",
                    "restart computer",
                    "save",
                    "copy",
                    "paste",
                    "open control panel",
                    "open new window",
                    "open new tab",
                    "exit program"
                });
                // Add commands for stored links 1 to 10.
                for (int i = 1; i <= 10; i++)
                {
                    commands.Add("open link " + i);
                }

                GrammarBuilder builder = new GrammarBuilder();
                builder.Append(commands);
                Grammar grammar = new Grammar(builder);

                recognizer.LoadGrammar(grammar);
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting up voice recognition: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Set up the tray icon.
        private void SetupTrayIcon()
        {
            trayIcon = new NotifyIcon();
            try
            {
                // Use a relative path for the icon. Make sure "favicon.ico" is in the same directory as your exe.
                trayIcon.Icon = new Icon("favicon.ico");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading icon: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trayIcon.Icon = SystemIcons.Application;
            }
            trayIcon.Visible = true;
            trayIcon.Text = "Voice Assistant";

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Settings", null, ShowSettings);
            menu.Items.Add("Exit", null, Exit);
            trayIcon.ContextMenuStrip = menu;
            trayIcon.DoubleClick += ShowSettings;

            trayIcon.ShowBalloonTip(3000, "Voice Assistant",
                "Program started. Use Ctrl+Shift+Z to toggle listening.", ToolTipIcon.Info);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                Debug.WriteLine("WM_HOTKEY received.");
                ToggleListening();
            }
            base.WndProc(ref m);
        }

        // Toggle the listening state.
        private void ToggleListening()
        {
            isListening = !isListening;
            if (isListening)
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                trayIcon.ShowBalloonTip(3000, "Voice Assistant", "Listening...", ToolTipIcon.Info);
                synthesizer.SpeakAsync("Listening");
            }
            else
            {
                recognizer.RecognizeAsyncStop();
                trayIcon.ShowBalloonTip(3000, "Voice Assistant", "Stopped listening", ToolTipIcon.Info);
                synthesizer.SpeakAsync("Stopped");
            }
        }

        // Handle recognized voice commands.
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string command = e.Result.Text.ToLower();

            // Process "open link X" commands.
            if (command.StartsWith("open link "))
            {
                string numPart = command.Substring("open link ".Length).Trim();
                if (int.TryParse(numPart, out int index))
                {
                    if (index >= 1 && index <= 10)
                    {
                        if (!string.IsNullOrEmpty(StoredLinks[index - 1]))
                        {
                            Process.Start(StoredLinks[index - 1]);
                            synthesizer.SpeakAsync("Opening stored link " + index);
                        }
                        else
                        {
                            synthesizer.SpeakAsync("No stored link found in slot " + index);
                        }
                    }
                }
                return;
            }

            // Process other voice commands.
            switch (command)
            {
                case "what time is it":
                    string time = DateTime.Now.ToString("h:mm tt");
                    synthesizer.SpeakAsync($"The time is {time}");
                    break;
                case "open notepad":
                    Process.Start("notepad.exe");
                    synthesizer.SpeakAsync("Opening notepad");
                    break;
                case "close notepad":
                    foreach (Process proc in Process.GetProcessesByName("notepad"))
                    {
                        try { proc.Kill(); } catch { }
                    }
                    synthesizer.SpeakAsync("Closing notepad");
                    break;
                case "minimize all windows":
                    MinimizeAllWindow();
                    synthesizer.SpeakAsync("Minimizing all windows");
                    break;
                case "show desktop":
                    ShowDesktop();
                    synthesizer.SpeakAsync("Showing desktop");
                    break;
                case "lock screen":
                    LockScreen();
                    synthesizer.SpeakAsync("Locking screen");
                    break;
                case "open calculator":
                    Process.Start("calc.exe");
                    synthesizer.SpeakAsync("Opening calculator");
                    break;
                case "open browser":
                    Process.Start("https://www.google.com");
                    synthesizer.SpeakAsync("Opening browser");
                    break;
                case "open task manager":
                    Process.Start("taskmgr.exe");
                    synthesizer.SpeakAsync("Opening task manager");
                    break;
                case "shutdown computer":
                    Process.Start("shutdown.exe", "/s /t 0");
                    synthesizer.SpeakAsync("Shutting down computer");
                    break;
                case "restart computer":
                    Process.Start("shutdown.exe", "/r /t 0");
                    synthesizer.SpeakAsync("Restarting computer");
                    break;
                case "save":
                    SendKeys.SendWait("^s");
                    synthesizer.SpeakAsync("Saving");
                    break;
                case "copy":
                    SendKeys.SendWait("^c");
                    synthesizer.SpeakAsync("Copied");
                    break;
                case "paste":
                    SendKeys.SendWait("^v");
                    synthesizer.SpeakAsync("Pasting");
                    break;
                case "open control panel":
                    Process.Start("control.exe");
                    synthesizer.SpeakAsync("Opening control panel");
                    break;
                case "open new window":
                    OpenNewWindow();
                    synthesizer.SpeakAsync("Opening new window");
                    break;
                case "open new tab":
                    OpenNewTab();
                    synthesizer.SpeakAsync("Opening new tab");
                    break;
                case "exit program":
                    Exit(null, null);
                    break;
            }
        }

        private void ShowSettings(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.Show();
            settings.Activate();
        }

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        // Persistence: Load stored links from file "link.txt" in the same directory as the executable.
        public static void LoadLinks()
        {
            string file = Path.Combine(Application.StartupPath, "link.txt");
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);
                for (int i = 0; i < Math.Min(lines.Length, StoredLinks.Length); i++)
                {
                    StoredLinks[i] = lines[i];
                }
            }
            else
            {
                // Optionally, create an empty file if it doesn't exist.
                File.WriteAllText(file, string.Empty);
            }
        }

        // Save stored links to file "link.txt".
        public static void SaveLinks()
        {
            string file = Path.Combine(Application.StartupPath, "link.txt");
            File.WriteAllLines(file, StoredLinks);
        }
    }
}
