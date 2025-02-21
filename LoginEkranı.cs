using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KutuphaneProjesi
{
    public partial class LoginEkranı : Form
    {
        public LoginEkranı()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
            
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbWhyKutuphane;Integrated Security=True;");
       
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(textBox2.UseSystemPasswordChar == true)
            {
                textBox2.UseSystemPasswordChar = false;
                pictureBox1.BackColor = Color.Red;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                pictureBox1.BackColor = Color.Green;
            }
            
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Cursor = Cursors.Default;
            pictureBox1.BorderStyle = BorderStyle.None;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string sifre;
            sifre = textBox2.Text;

            
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "SELECT Sifre FROM TableKutuphaneYoneticileri WHERE KullaniciAdi = @p1";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@p1", textBox1.Text);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read()) {
                    if(textBox2.Text == reader["Sifre"].ToString())
                    {
                        label4.Text = "Sifre Doğru giriş yapılıyor...";
                        
                        using (AnaEkran girisYapildi = new AnaEkran()) {
                            this.Hide();
                            girisYapildi.ShowDialog();
                        }
                    }
                    else
                    {
                        label4.Text = "Hatalı şifre veya kullanıcı adı!";
                        baglanti.Close();
                        await Task.Delay(2000);
                        label4.Text = "";
                    }
                }
                else
                {
                    label4.Text = "Hatalı şifre veya kullanıcı adı!";
                    baglanti.Close();
                    await Task.Delay(2000);
                    label4.Text = "";
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show("Bağlantı Hatası!1" + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
    }
}
