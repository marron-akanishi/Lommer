using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Management;

namespace Lommer {
    public partial class Form1 : Form {
        string ProgramName = "";
        Process Target;
        long PhysicalMax = 0;
        long VirtualMax = 0;
        Timer Counter = new Timer();
        PerformanceCounter CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter FreeRamCounter = new PerformanceCounter("Memory", "Available KBytes");

        public Form1() {
            InitializeComponent();
            label6.Text = label7.Text = label8.Text = label9.Text = label11.Text = label13.Text = "0";
            Counter.Interval = 1000;
            Counter.Tick += new EventHandler(Process_Checker);
            Counter.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executable File(*.exe)|*.exe";
            ofd.Title = "Please select target program.";
            ofd.RestoreDirectory = true;
            if(ofd.ShowDialog() == DialogResult.OK) {
                ProgramName = ofd.FileName;
                textBox1.Text = Path.GetFileName(ProgramName);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if(ProgramName == "") {
                MessageBox.Show("Please set target program first!");
                return;
            }
            button2.Enabled = false;
            Target = Process.Start(ProgramName);
            Counter.Enabled = true;
        }

        private void Process_Checker(object Sender, EventArgs e) {
            long PhyNow, VirNow;
            try {
                Target.Refresh();
                PhyNow = Target.WorkingSet64 / 1024 / 1024;
                VirNow = Target.VirtualMemorySize64 / 1024 / 1024;
            }
            catch {
                Counter.Enabled = false;
                button2.Enabled = true;
                return;
            }
            int cpu = (int)CpuCounter.NextValue();
            int ram = (int)FreeRamCounter.NextValue() / 1024;
            if (PhyNow > PhysicalMax) PhysicalMax = PhyNow;
            if (VirNow > VirtualMax) VirtualMax = VirNow;
            label6.Text = PhyNow.ToString();
            label7.Text = VirNow.ToString();
            label8.Text = PhysicalMax.ToString();
            label9.Text = VirtualMax.ToString();
            label11.Text = cpu.ToString() + "%";
            label13.Text = ram.ToString();
        }
    }
}
