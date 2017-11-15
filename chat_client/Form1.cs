using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// added namespaces
using System.Net;
using System.Net.Sockets;


namespace chat_client
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;

        public Form1()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textBox1.Text = GetLocalIP();
            textBox3.Text = GetLocalIP();

        }
        private string GetLocalIP()
        {
            IPHostEntry  host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";

        }
        private void MessageCallBack(IAsyncResult aResult)
        {

            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size>0)
                {
                    byte[] recievedData = new byte[1464];
                    recievedData = (byte[])aResult.AsyncState;

                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string recievedMessage = encoding.GetString(recievedData);
                    listBox1.Items.Add("Friend" + recievedMessage);
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer,0,buffer.Length,SocketFlags.None,ref epRemote,new AsyncCallback(MessageCallBack),buffer );

            }
            catch (Exception exp)
            {

                MessageBox.Show(exp.ToString());
            }
                

        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textBox5.Text);

                sck.Send(msg);
                listBox1.Items.Add("You"+textBox5.Text);
                textBox5.Clear();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
                 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text),Convert.ToInt32(textBox2.Text));
                sck.Bind(epLocal);


                epRemote = new IPEndPoint(IPAddress.Parse(textBox3.Text), Convert.ToInt32(textBox4.Text));
                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
               
                button1.Text = "Connected";   
                button1.Enabled = false;
                button2.Enabled = true;
                textBox5.Focus();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}
