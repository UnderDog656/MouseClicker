using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Hotkeys;

namespace MouseClicker
{
    public partial class Form1 : Form
    {
        private Hotkeys.GlobalHotkey hk;
        private Hotkeys.GlobalHotkey hk1;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public string Saved = "";
        public int cord;
        private int cord1;
        public int test123;
        private int test1234;
        public int act_position1;
        public int act_position2;
        private int clicks = 0;
        private BackgroundWorker _worker = null;
        private BackgroundWorker _worker1;
        bool edited;
        public int pos;
        private bool check = true;
        private bool check1 = true;
        private bool check2 = true;
        public DialogResult Msg_Box { get; private set; }

        public Form1()
        {
            InitializeComponent();
            hk = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.X, this);
            hk1 = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.Z, this);
        }
        private Keys GetKey(IntPtr LParam)
        {
            return (Keys)((LParam.ToInt32()) >> 16);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                switch (GetKey(m.LParam))
                {
                    case Keys.Z:
                        if (textBox3.Text.Length == 0 || comboBox1.Text == "" || comboBox1.Text == "Select Frequency" || comboBox1.Text == "0")
                        {
                            if (check == true)
                            {
                                check = false;
                                button4.PerformClick();
                            }
                        }
                        else if (textBox3.Text.StartsWith("{") && Convert.ToInt32(comboBox1.Text) >= 1 && Convert.ToInt32(comboBox1.Text) <= 100000)
                        {
                            if (check1 == true)
                            {
                                if (_worker1 == null)
                                {
                                        button4.PerformClick();
                                        check1 = false;
                                }
                                else
                                {
                                    if (check2 == true)
                                    {
                                        if (_worker1.IsBusy)
                                        {
                                            check2 = false;
                                        }
                                        else
                                        {
                                            _worker1.CancelAsync();
                                            check2 = false;
                                            button4.PerformClick();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Keys.X:
                        check1 = true;
                        check2 = true;
                        if (_worker1 == null)
                        {

                        }
                        else
                        {
                            _worker1.CancelAsync();
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
            label2.ForeColor = Color.Red;
            if (hk.Register())
            {
            }
            if (hk1.Register())
            {
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string num = e.KeyChar.ToString();
            if (num == "a" || num == "A")
            {
                if (label9.Text == "")
                {
                    label9.Text = "Current Coordinates";
                }
                textBox3.Text = "{X=" + test123.ToString() + "," + "Y=" + test1234.ToString() + "}";
                test123 = cord;
                test1234 = cord1;
                Saved = cord.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            _worker.CancelAsync();
        }
        private void UpdateTextbox(string _Text)

        {
            textBox1.Text = _Text;
            textBox1.Text = "{X=" + cord.ToString() + "," + "Y=" + cord1.ToString() + "}";
        }
        

        private void stop(object sender, EventArgs e)
        {
            _worker.CancelAsync();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox1.Text == "Select Frequency" || comboBox1.Text == "0" || test123.Equals(0)||test123.Equals(0))
            {
                Msg_Box = MessageBox.Show(" Make sure that you have chosen a frequency and correct cordinates ", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Msg_Box == DialogResult.OK)
                {
                    check = true;
                }
            }
            else
            {
                pos = Convert.ToInt32(comboBox1.Text);
                _worker1 = new BackgroundWorker();
                _worker1.WorkerSupportsCancellation = true;

                _worker1.DoWork += new DoWorkEventHandler((state, args) =>
                {
                    do
                    {
                        if (_worker1.CancellationPending)
                            break;
                        else
                        {
                            Control control = new Control();
                            System.Windows.Forms.Cursor.Position = control.PointToScreen(new Point(test123-8, test1234-31));
                            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                            System.Threading.Thread.Sleep(pos);
                        }
                    } while (true);
                });
                _worker1.RunWorkerAsync();
            }
        }
        private void button5_Click_2(object sender, EventArgs e)
        {
            label2.Text = "Press A to save cordinates";
            Button button = sender as Button;
            if (button == null) return;

            if (button.Text != "Start")
            {
                label2.Text = "";
                label5.Text = "";
                button.Text = "Start";
                button.BackColor = Color.Green;
                if (_worker == null)
                {

                }
                else
                    _worker.CancelAsync();
            }
            else if (button.Text == "Start")
            {
                label5.Text = "Current Position";
                button.Text = "Stop";
                button.BackColor = Color.Red;
                _worker = new BackgroundWorker();
                _worker.WorkerSupportsCancellation = true;

                _worker.DoWork += new DoWorkEventHandler((state, args) =>
                {
                    do
                    {
                        if (_worker.CancellationPending)
                            break;
                        else
                        {
                            cord = MousePosition.X;
                            cord1 = MousePosition.Y;
                            System.Threading.Thread.Sleep(1);
                            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(Form1_KeyPress);
                            this.Invoke(new Action<string>(UpdateTextbox), new object[] { "123" });
                        }
                    } while (true);
                });
                _worker.RunWorkerAsync();
            }
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            edited = !char.IsControl(e.KeyChar);
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clicks++;
            button1.Text = Convert.ToString(clicks);
        }
    }
}
