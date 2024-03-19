using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Advanced_Combat_Tracker;

namespace AutoSharpActPlugin
{
    public partial class AutoSharpGUI : UserControl
    {
        public AutoSharpGUI(AutoSharpActEntry autoSharpActEntry)
        {
            InitializeComponent();
            this.autoSharpActEntry = autoSharpActEntry;
            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;
        }

        private readonly AutoSharpActEntry autoSharpActEntry;

        // 12 is the magic number of items to show, we plus 4 for user experience
        private int preloadItems = 16;

        private void Button_LocateModulesFolder_Click(object sender, EventArgs e)
        {
            Process.Start(autoSharpActEntry.ModulesFolder);
        }

        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            // Save the top index, because when we remove the item, the top index will be reset to 0 if there is no enough items
            var topIndex = ListBox_Log.TopIndex;
            ListBox_Log.Items.Add(logInfo.logLine);
            if (ListBox_Log.Items.Count > 1000)
            {
                ListBox_Log.Items.RemoveAt(0);
            }
            ListBox_Log.TopIndex = topIndex;
            // Scroll to bottom
            var autoScroll = ListBox_Log.TopIndex >= ListBox_Log.Items.Count - preloadItems;
            if (autoScroll)
            {
                ScrollToBottom();
            }
        }

        private void Button_Reload_Click(object sender, EventArgs e)
        {
            autoSharpActEntry.Reload();
        }

        private void ScrollToBottom()
        {
            if (ListBox_Log.Items.Count > 0)
            {
                ActGlobals.oFormActMain.Invoke(new Action(() =>
                {
                    ListBox_Log.TopIndex = ListBox_Log.Items.Count - 1;
                }));
            }
        }

        private void AutoSharpGUI_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < preloadItems; i++)
            {
                ListBox_Log.Items.Add(string.Empty);
            }
            ScrollToBottom();
        }

        private void Button_CopyLog_Click(object sender, EventArgs e)
        {
            var logText = ListBox_Log.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(logText))
            {
                return;
            }
            Clipboard.SetText(logText);
        }
    }
}
