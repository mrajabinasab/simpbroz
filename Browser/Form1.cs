using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            string address = textBox1.Text;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private static int Divider(byte[] main, byte[] divc)
        {
            int count = main.Length - divc.Length + 1;
            for (int i = 0; i < count; ++i)
            {
                bool ok = true;
                for (int j = 0; j < divc.Length; ++j)
                {
                    if (main[i + j] != divc[j])
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    return i;
                }
            }
            return -1;
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            string finalresponse = string.Empty;

            using (var tcpclient = new TcpClient(textBox1.Text, 80))
            using (var stream = tcpclient.GetStream())
            {
                tcpclient.SendTimeout = 500;
                tcpclient.ReceiveTimeout = 1000;
                var httpreqstring = new StringBuilder();
                httpreqstring.AppendLine("GET /?scope=images&nr=1 HTTP/1.1");
                httpreqstring.AppendLine("Host: " + textBox1.Text);
                httpreqstring.AppendLine("User-Agent: SimpBroz v1.0");
                if (checkBox1.Checked)
                httpreqstring.AppendLine("Connection: keep-alive");
                else
                httpreqstring.AppendLine("Connection: close");
                httpreqstring.AppendLine();
                richTextBox2.Text = Convert.ToString(httpreqstring);
                var httpheader = Encoding.ASCII.GetBytes(httpreqstring.ToString());
                await stream.WriteAsync(httpheader, 0, httpheader.Length);

                using (var reciever = new MemoryStream())
                {
                    await stream.CopyToAsync(reciever);
                    reciever.Position = 0;
                    var httpresponse = reciever.ToArray();
                    int httprespcheck = Divider(httpresponse, Encoding.ASCII.GetBytes("\r\n\r\n"));
                    if (httprespcheck == -1)
                    {
                        MessageBox.Show("Http Response is Unhandlabe.","HTTP Response Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    var index = httprespcheck + 4;
                    var head = Encoding.ASCII.GetString(httpresponse, 0, index);
                    richTextBox3.Text = head;
                    reciever.Position = index;

                    if (head.IndexOf("Content-Encoding: gzip") > 0)
                    {
                        using (GZipStream ungzipStream = new GZipStream(reciever, CompressionMode.Decompress))
                        using (var ungzip = new MemoryStream())
                        {
                            ungzipStream.CopyTo(ungzip);
                            ungzip.Position = 0;
                            finalresponse = Encoding.UTF8.GetString(ungzip.ToArray());
                            //richTextBox1.Text = finalresponse;
                            //webBrowser2.DocumentText = finalresponse;
                        }
                    }
                    else
                    {
                        finalresponse = Encoding.UTF8.GetString(httpresponse, index, httpresponse.Length - index);
                        //richTextBox1.Text = finalresponse;
                        //webBrowser2.DocumentText = finalresponse;
                    }
                }

            }
            webBrowser2.Navigate(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser2.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser2.GoBack();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser2.GoForward();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            webBrowser2.DocumentText = richTextBox1.Text;
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = webBrowser2.DocumentText;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
