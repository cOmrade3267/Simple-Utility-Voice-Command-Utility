using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace VoiceAssistant
{
    public partial class SettingsForm : Form
    {
        private Stopwatch stopwatch = new Stopwatch();

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm.Instance.StartListening();
                stopwatch.Restart();
                lblStatus.Text = "Started";
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting voice recognition: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm.Instance.StopListening();
                stopwatch.Stop();
                lblStatus.Text = "Stopped";
                timer1.Stop();
                lblElapsedTime.Text = "00:00:00";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping voice recognition: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText = "Voice Assistant Commands:\n\n" +
                              "what time is it - Announces current time\n" +
                              "open notepad - Launches Notepad\n" +
                              "close notepad - Closes Notepad\n" +
                              "minimize all windows - Minimizes all windows\n" +
                              "show desktop - Displays desktop\n" +
                              "lock screen - Locks screen\n" +
                              "open calculator - Opens Calculator\n" +
                              "open browser - Opens default browser\n" +
                              "open task manager - Opens Task Manager\n" +
                              "shutdown computer - Shuts down computer\n" +
                              "restart computer - Restarts computer\n" +
                              "save - Simulates Ctrl+S\n" +
                              "copy - Simulates Ctrl+C\n" +
                              "paste - Simulates Ctrl+V\n" +
                              "open control panel - Opens Control Panel\n" +
                              "open new window - Opens a new window\n" +
                              "open new tab - Opens a new tab\n" +
                              "open link x - Opens stored hyperlink in slot x (x=1..10)\n" +
                              "exit program - Exits application\n\n" +
                              "Use the numeric control to select a slot (1-10) for saving or removing a link.";
            MessageBox.Show(helpText, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveLink_Click(object sender, EventArgs e)
        {
            try
            {
                int slot = (int)numSlot.Value;  // Slots numbered 1 to 10
                MainForm.StoredLinks[slot - 1] = txtLink.Text;
                MainForm.SaveLinks();
                MessageBox.Show("Link saved in slot " + slot + ": " + MainForm.StoredLinks[slot - 1],
                    "Link Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving link: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveLink_Click(object sender, EventArgs e)
        {
            try
            {
                int slot = (int)numSlot.Value;
                MainForm.StoredLinks[slot - 1] = "";
                MainForm.SaveLinks();
                MessageBox.Show("Link removed from slot " + slot, "Link Removed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing link: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblElapsedTime.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
