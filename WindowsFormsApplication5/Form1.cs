using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AddClipboardFormatListener(this.Handle);    // Add our window to the clipboard's format listener list.

        }
        
        /// <summary>
         /// Places the given window in the system-maintained clipboard format listener list.
         /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                IDataObject iData = Clipboard.GetDataObject();      // Clipboard's data.

                /* Depending on the clipboard's current data format we can process the data differently. 
                 * Feel free to add more checks if you want to process more formats. */
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    var playSound = false;
                
                    string text = (string)iData.GetData(DataFormats.Text);

                    //split each line of the map into string arrays
                    string[] SplitItem = Regex.Split(text, "\r\n|\r|\n");
                    //split each search into string arrays
                    string[] myStrings = textBox2.Text.Split('|');

                    // Only check maps from path fo exile to avoid misshaps
                    if (text.Contains("Map Tier: "))
                    {
                        for (var i = 0; i < SplitItem.Length; i++)
                        {
                            for (var j = 0; j < myStrings.Length; j++)
                            {
                                if (SplitItem[i].ToLower().Contains(myStrings[j].ToLower()))
                                {
                                    richTextBox1.Text += SplitItem[i] + "\r";
                                    playSound = true;
                                }
                            }
                        } 
                    }
                    
                    // if found string then play sound ONLY once, actually tested against multi search and boy was that sound i heard retarded :D
                    if (playSound)
                    {
                        // play notify.wav (can replace with what ever sound file you want after building i guess)
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Content\notify.wav");
                        player.Play();
                        playSound = false;
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save shit
            Properties.Settings.Default.WindowsLocation = this.Location;
            Properties.Settings.Default.ModLine = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // load shit
            this.Location = Properties.Settings.Default.WindowsLocation;
            textBox2.Text = Properties.Settings.Default.ModLine;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Save text on every input because ive had things crash in the past and just like to be exactly where ive left off
            Properties.Settings.Default.ModLine = textBox2.Text;
            Properties.Settings.Default.Save();
        }
    }
}
