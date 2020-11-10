using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecureLoginFactor
{
    public partial class Form1 : Form
    {
        public byte[] hashes;
        public String temporary = RandomString(20);
        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            /*
            var data = Encoding.UTF8.GetBytes(temporary);
            using (SHA512 shaM = new SHA512Managed())
            {
                byte[] hash = shaM.ComputeHash(data);
                hashes = hash;
            }
            */
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("application.pass99@gmail.com");
                message.To.Add(new MailAddress(textBox1.Text));
                message.Subject = "Password";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = temporary;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("application.pass99@gmail.com", "password.99");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                label2.Text = "Status: PLEASE CHECK MAIL";
                label1.Text = "Password: ";
                textBox1.Text = String.Empty;
                button1.Visible = false;
                button2.Visible = true;
            }
            catch (Exception) { }

            //temporary = String.Empty;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            var data = Encoding.UTF8.GetBytes(textBox1.Text);
            using (SHA512 shaM = new SHA512Managed())
            {
                byte[] hash = shaM.ComputeHash(data);
            */
                if (textBox1.Text.Equals(temporary))
                {
                    label2.Text = "Welcome";
                }

                else
                {
                    label2.Text = "Failed.";
                    MessageBox.Show("Program Will Exit.");
                    Application.Exit();
                        
                }
           // }
            

            
        }
    }
}
