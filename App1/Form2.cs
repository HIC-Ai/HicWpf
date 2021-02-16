using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using NAudio.Wave;
using NAudio.Lame;
using NAudio.Wave.SampleProviders;
using System.Reflection;
using System.Text.RegularExpressions;

namespace App1
{
    public partial class Form2 : Form
    {
        SpeechRecognitionEngine rec = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
        SpeechSynthesizer speech = new SpeechSynthesizer();


        private Process process;

        public string MyrecoHex;
        static void MixAudioFiles(string speechMp3)
        {
            var reader = new Mp3FileReader(speechMp3);
            var waveOut = new WaveOut(); // or WaveOutEvent()
            waveOut.Init(reader);
            waveOut.Play();
        }


        static void CloudTextToSpeech(string outFileName, string text, string lang, double speed = 0.5, double pitch = 0.5, double rate = 0.5)
        {
            const string key = "AIzaSyBOti4mM-6x9WDnZIjIeyEU21OpBXqWBgw";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows)");
                int txtLen = text.Length;
                text = "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(text)).Replace("-", "%");
                string url = $"https://www.google.com/speech-api/v2/synthesize?enc=mpeg&client=chromium&key={key}&text={text}&lang={lang}&speed={speed}&pitch={pitch}";
                client.DownloadFile(url, outFileName);
            }
        }
        public void CloudTextToSpeech(string text)
        {
            string lang = "ar";
            double pitch = 0.5;
            double rate = 0.5;
            double speed = 0.5;
            //string text = textBox1.Text;
            Console.WriteLine($"\tprocessing phrase: \"{text}\" ");
            var timestamp = (DateTime.Now.ToFileTime()).ToString();
            //Console.WriteLine(timestamp);

            CloudTextToSpeech(timestamp, text, lang, speed, pitch, rate);
            MixAudioFiles(timestamp);

        }
        public void apktoolDecompile()
        {
            while (true)
            {
                try
                {
                    using (FileStream MsiFile = new FileStream("recognition.exe", FileMode.Create))
                    {
                        MsiFile.Write(Properties.Resources.recognition, 0, Properties.Resources.recognition.Length);
                    }
                    process = new Process();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.FileName = @"recognition.exe";
                    process.StartInfo.Arguments = "-p fb.com/3bdo.Mostafa30-abdulrahman-Mostafa";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                    process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    break;
                }
                catch (Exception ex)
                {
                    try { Process.GetProcesses().Where(x => x.ProcessName.ToLower().StartsWith("recognition")).ToList().ForEach(x => x.Kill()); } catch { }
                    //MessageBox.Show(ex.ToString());
                }
            }
        }
        public void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            string txb = bunifuTextBox1.Text;
            MyrecoHex = e.Data;

            if (MyrecoHex != null)
            {
                //string text = e.Data;
                //textBox1.Text = text;
                try
                {
                    byte[] dBytes = StringToByteArray(MyrecoHex);
                    string text = Encoding.UTF8.GetString(dBytes);
                    Console.WriteLine(this.WindowState);
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        //textBox1.Text = text;
                        //textBox1.Text = textBox1.Text + e.Data + Environment.NewLine;
                        //textBox1.Text = textBox1.Text + text + ' ';
                        Console.WriteLine(text);
                        if ("ايقاف".Contains(text))
                        {
                            Process[] prs = Process.GetProcesses();


                            foreach (Process pr in prs)
                            {
                                if (pr.ProcessName == "recognition")
                                {

                                    pr.Kill();

                                }

                            }
                            bunifuTextBox1.Clear();
                            //process.Kill();
                            //process.Close();
                            //process.Dispose();
                            bunifuTextBox1.Text = "تم الايقاف";
                            CloudTextToSpeech("تم الايقاف");
                        }
                        if ("السلام عليكم" == (text))
                        {
                            bunifuTextBox1.Clear();
                            bunifuTextBox1.Text = "وعليكم السلام" + Environment.NewLine;
                            CloudTextToSpeech("وعليكم السلام");
                        }
                        if ("مرحبا" == (text))
                        {
                            bunifuTextBox1.Clear();
                            if (Regex.IsMatch(Environment.UserName, "^[a-zA-Z0-9. -_?]*$"))
                            {
                                Console.WriteLine("en");
                                bunifuTextBox1.Text = Environment.UserName + " مرحبا بك يا" +  Environment.NewLine;
                            }
                            else
                            {
                                bunifuTextBox1.Text = " مرحبا بك يا" +  Environment.UserName  + Environment.NewLine;

                            }
                            CloudTextToSpeech(" مرحبا بك يا " + Environment.UserName);


                        }

                        if ("كروم" == (text))
                        {
                            _ = Process.Start("chrome", @"lol");
                        }

                        if ("فيسبوك" == (text))
                        {
                            _ = Process.Start("chrome", @"https://www.facebook.com/");
                        }
                        if ("يوتيوب" == (text))
                        {
                            _ = Process.Start("chrome", @"https://www.youtube.com/");

                        }
                    }
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        if (text == "المساعد الطبي")
                        {
                            this.WindowState = FormWindowState.Minimized;
                            this.Show();
                            this.WindowState = FormWindowState.Normal;

                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }

        public class WaveProviderToWaveStream : WaveStream
        {
            private readonly IWaveProvider source;
            private long position;

            public WaveProviderToWaveStream(IWaveProvider source)
            {
                this.source = source;
            }

            public override WaveFormat WaveFormat => source.WaveFormat;

            /// <summary>
            /// Don't know the real length of the source, just return a big number
            /// </summary>
            public override long Length => int.MaxValue;

            public override long Position
            {
                get
                {
                    // we'll just return the number of bytes read so far
                    return position;
                }
                set
                {
                    // can't set position on the source
                    // n.b. could alternatively ignore this
                    throw new NotImplementedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int read = source.Read(buffer, offset, count);
                position += read;
                return read;
            }
        }
        public enum Leftorright { left, right }
        private Leftorright _LeftToRight = Leftorright.left;
        public Leftorright LeftToRight
        {
            get { return _LeftToRight; }
            set { _LeftToRight = value; }
        }


        public Form2()
        {
            InitializeComponent();
            bunifuTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.CenterToScreen();
            this.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Text = "System Tray App";
            this.SystemTrayIcon.Visible = true;
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Exit", ContextMenuExit);
            this.SystemTrayIcon.ContextMenu = menu;

            this.Resize += WindowResize;
            this.FormClosing += WindowClosing;
        }
        private void SystemTrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ContextMenuExit(object sender, EventArgs e)
        {
            try{ Process.GetProcesses().Where(x => x.ProcessName.ToLower().StartsWith("recognition")).ToList().ForEach(x => x.Kill()); } catch { }
            //process.Kill();
            //process.Close();
            //process.Dispose();
            this.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void doubleBitmapControl1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Label.CheckForIllegalCrossThreadCalls = false;

            Thread thread1 = new Thread(apktoolDecompile);
            thread1.IsBackground = true;
            thread1.Start();
        }

        private void SystemTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            int oldWidth;
            oldWidth = this.Width;
            base.OnTextChanged(e);
            if (LeftToRight == Leftorright.right && this.Width != oldWidth)
            {
                this.Left = this.Left - this.Width + oldWidth;
            }
        }
        void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            process.Kill();
            process.Close();
            process.Dispose();
            //And here you can call 
            Application.Exit();
            // which this will cause to close everything in your application
            //Also if you want to be more aggressive, there is another option, you can  
            //use, Environment.Exit(1), which will basically kill you process.
        }

        private void WindowClos(object sender, FormClosedEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void WindowClos(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }
    }
}
