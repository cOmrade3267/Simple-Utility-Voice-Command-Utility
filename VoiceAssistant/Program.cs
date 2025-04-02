using System;
using System.Windows.Forms;

namespace VoiceAssistant
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the hidden main form (voice assistant background)
            MainForm mainForm = new MainForm();
            // Now MainForm.Instance is set in its constructor

            // Run the SettingsForm as the main window for testing the UI.
            Application.Run(new MainForm());
        }
    }
}
