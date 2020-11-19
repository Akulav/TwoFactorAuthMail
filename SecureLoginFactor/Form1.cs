using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Ubiety.Dns.Core;

namespace SecureLoginFactor
{
    public partial class Form1 : Form
        
    {
        string mail;
        public byte[] hashes;
        public String salt = RandomString(20);
        public String temporary = RandomString(20);
        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = false;
            pass.Visible = false;

            /*
            var data = Encoding.UTF8.GetBytes(temporary);
            using (SHA512 shaM = new SHA512Managed())
            {
                byte[] hash = shaM.ComputeHash(data);
                hashes = hash;
            }
            */
            mail = textBox1.Text;
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
                //textBox1.Text = String.Empty;
                button1.Visible = false;
                button2.Visible = true;
                

            }
            catch (Exception) { }

            //temporary = String.Empty;
            var md5 = new MD5CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(textBox1.Text.Trim());
            var md5data = md5.ComputeHash(data);
            using (SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-7P80IRU\SQLEXPRESS;initial Catalog=Login;Integrated Security=True;"))
            {

                Random r = new Random();
                sqlCon.Open();

                string query = "INSERT INTO mama (userID, mail, password, confirmed) VALUES (@id, @username, @password, 0)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                var hashedPassword = md5data.ToString();
                sqlCmd.Parameters.AddWithValue("@password", hashedPassword);
                sqlCmd.Parameters.AddWithValue("@id", r.Next(1, 999));
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {
                    
                }
                
            }
            textBox1.Text = String.Empty;
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
                    label2.Text = "Welcome. Account confirmed.";
                using (SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-7P80IRU\SQLEXPRESS;initial Catalog=Login;Integrated Security=True;"))
                {

                    Random r = new Random();
                    sqlCon.Open();

                    string query = "UPDATE mama set confirmed = @id where mail = @username";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", 1);
                    sqlCmd.Parameters.AddWithValue("@username", mail);
                    int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                    if (count == 1)
                    {

                    }

                }
            }

                else
                {
                    label2.Text = "Failed.";
                    MessageBox.Show("Program Will Exit.");
                    Application.Exit();
                        
                }
           // }
            

            
        }

        private void pass_TextChanged(object sender, EventArgs e)
        {

        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "3ruijc823u9ijr794yuo3i29e-r8uwgh4oiujpo3rki9j4ohw47u8oieork0o9";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "3ruijc823u9ijr794yuo3i29e-r8uwgh4oiujpo3rki9j4ohw47u8oieork0o9";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        private void login_Click(object sender, EventArgs e)
        {
            var md5 = new MD5CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(pass.Text.Trim());
            var md5data = md5.ComputeHash(data);
            var hashedPassword = md5data.ToString();

            using (SqlConnection sqlCon = new SqlConnection(@"Data Source=DESKTOP-7P80IRU\SQLEXPRESS;initial Catalog=Login;Integrated Security=True;"))
            {

                
                sqlCon.Open();

                string query = "SELECT COUNT(1) FROM mama WHERE mail=@username AND password=@password";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@password", hashedPassword);
                sqlCmd.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {

                    string quer = "SELECT CONFIRMED FROM mama WHERE mail=@username AND password=@password";
                    SqlCommand sqlCm = new SqlCommand(quer, sqlCon);

                    string conf = sqlCmd.ExecuteScalar().ToString();
                    if (conf == "1")
                    {
                        MessageBox.Show("Logged in. Confirmed mail");
                    }
                    else MessageBox.Show("Logged in. Mail not confirmed");
             
                }

            }
        }
    }
}
