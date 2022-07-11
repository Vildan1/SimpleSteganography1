using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace SimpleSteganography
{
    public partial class Form1 : Form
    {
        public Bitmap img;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files (*.png, *.jpg) | *.png; *.jpg";
            openDialog.InitialDirectory = @"C:\Users\metech\Desktop";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openDialog.FileName.ToString();
                pictureBox1.ImageLocation = textBoxFilePath.Text;
            }

        }

        private void buttonEncode_Click(object sender, EventArgs e)
        {
            img = new Bitmap(textBoxFilePath.Text);

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);

                    if (i < 1 && j < textBoxMessage.TextLength)
                    {
                        Console.WriteLine("R = [" + i + "][" + j + "] = " + pixel.R);
                        Console.WriteLine("G = [" + i + "][" + j + "] = " + pixel.G);
                        Console.WriteLine("G = [" + i + "][" + j + "] = " + pixel.B);

                        char letter = Convert.ToChar(textBoxMessage.Text.Substring(j, 1));
                        int value = Convert.ToInt32(letter);
                        Console.WriteLine("letter : " + letter + " value : " + value);

                        img.SetPixel(i, j, Color.FromArgb(pixel.R, pixel.G, value));
                    }

                    if (i == img.Width - 1 && j == img.Height - 1)
                    {
                        img.SetPixel(i, j, Color.FromArgb(pixel.R, pixel.G, textBoxMessage.TextLength));
                    }

                }
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image Files (*.png, *.jpg) | *.png; *.jpg";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = saveFile.FileName.ToString();
                pictureBox1.ImageLocation = textBoxFilePath.Text;

                img.Save(textBoxFilePath.Text);
            }


        }

        private void buttonDecode_Click(object sender, EventArgs e)
        {
            Bitmap img = new Bitmap(textBoxFilePath.Text);


            string message = "";

            Color lastpixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int msgLength = lastpixel.B;

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);

                    if (i < 1 && j < msgLength)
                    {
                        int value = pixel.B;
                        char c = Convert.ToChar(value);
                        string letter = System.Text.Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });

                        message = message + letter;
                    }
                }
            }
            MessageBox.Show(message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(""), 13000);       // GÖNDEREN KİŞİ IP
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);

            using (Socket client = server.Accept())     // karşı taraf ortama bağlandığında
            {
                ImageConverter converter = new ImageConverter();
                Bitmap bt = new Bitmap(textBoxFilePath.Text);
                byte[] buffer = (byte[])converter.ConvertTo(bt, typeof(byte[]));
                client.Send(buffer, buffer.Length, SocketFlags.None);
            }

            server.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(""), 13000); //gönderen kisi ipsine bağlanılır
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    client.Connect(iep);        // eğer birinin locaine bağlanılırsa	

                }
                catch (Exception ex)
                {
                    var excep = ex.InnerException;
                }
                // receive data
                byte[] buffer = new byte[100000000];
                client.Receive(buffer, buffer.Length, SocketFlags.None);    // fotoğraf kabul edilir
                //image.Save(fileName+"2");
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//SifrelenenResim.jpg";
                File.WriteAllBytes(path, buffer);  // alınan foto masaüstüne kaydedilir
            }
        }
    }
}