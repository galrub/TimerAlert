using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SystemTrayApp
{
    public partial class AppWindow : Form
    {

        private int currentInterval = 5;
        private Dictionary<int, IntevalSet> intervalSets = new Dictionary<int, IntevalSet>();

        public AppWindow()
        {
            InitializeComponent();
            this.CenterToScreen();

            // To provide your own custom icon image, go to:
            //   1. Project > Properties... > Resources
            //   2. Change the resource filter to icons
            //   3. Remove the Default resource and add your own
            //   4. Modify the next line to Properties.Resources.<YourResource>
            this.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Icon = Properties.Resources.Yohproject_Cute_Clock;

            // Change the Text property to the name of your application
            this.SystemTrayIcon.Text = "Clicker Timer";
            this.SystemTrayIcon.Visible = true;

            // Modify the right-click menu of your system tray icon here
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Exit", ContextMenuExit);
            this.SystemTrayIcon.ContextMenu = menu;

            this.Resize += WindowResize;
            this.FormClosing += WindowClosing;

            var inters = new Dictionary<int, string>();
            inters.Add(5, "5 minutes");
            inters.Add(15, "15 minutes");
            inters.Add(30, "30 minutes");
            intervals.DataSource = new BindingSource(inters, null);
            intervals.DisplayMember = "Value";
            intervals.ValueMember = "Key";
            intervals.SelectedValueChanged += Intervals_SelectedValueChanged;
            clickTimer.Tick += ClickTimer_Tick;
            InitializeIntevalSets();
            clickTimer.Enabled = true;
        }

        private void InitializeIntevalSets()
        {
            this.intervalSets.Add(5, new IntevalSet(5, "5 Minute"));
            this.intervalSets.Add(15, new IntevalSet(15, "15 Minute"));
            this.intervalSets.Add(30, new IntevalSet(30, "30 Minute"));
        }

        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now;
            IntevalSet set;
            if(intervalSets.TryGetValue(currentInterval, out set)) {
                if(set.Minutes.Contains(currentTime.Minute) && currentTime.Second == 45)
                {
                    var player = new SoundPlayer(Properties.Resources.mixkit_modern_technology_select_3124);
                    player.Play();
                } 
            }
        }

        private void Intervals_SelectedValueChanged(object sender, EventArgs e)
        {
            currentInterval = ((KeyValuePair<int, string>)intervals.SelectedItem).Key;
        }

        private void SystemTrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ContextMenuExit(object sender, EventArgs e)
        {
            this.SystemTrayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public class IntevalSet
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public List<int> Minutes {  get; set; }

        public override string ToString()
        {
            return Text;
        }

        public IntevalSet(int inteval, string text)
        {
            this.Text = text;
            this.Value = inteval;
            this.Minutes = new List<int>();
            var cnt = 60 / inteval;
            for (int i = 1; i <= cnt; i++)
            {
                this.Minutes.Add((i * inteval) - 1);
            }
        }
    }
}
