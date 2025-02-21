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
    public partial class KayitDuzenle : Form
    {
        private AnaEkran anaEkran;
        SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbWhyKutuphane;Integrated Security=True;");
        string alinanId;
        string alinanIsbn;
        public KayitDuzenle(string id, string isbn, AnaEkran anaEkran)
        {
            InitializeComponent();
            alinanId = id;
            alinanIsbn = isbn;
            labelIDgiris.Text = id;
            this.anaEkran = anaEkran;
            label7.ForeColor = Color.Red;

            bool siyahTemaAcikMi = anaEkran.siyahTemaAcikMi;
            ThemeHelper.TemaUygula(this, siyahTemaAcikMi);
        }

        private void KayitDuzenle_Load(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "SELECT * FROM TableKitaplar WHERE ID = @id";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@id", alinanId);

                using(SqlDataReader reader = sqlCommand.ExecuteReader()) 
                {
                    if (reader.Read()) {
                        string kitapAdi = reader["KitapAdi"].ToString();
                        string yazarAdi = reader["YazarAdi"].ToString();
                        string yazarSoyadi = reader["YazarSoyadi"].ToString();
                        string Isbn = reader["ISBN"].ToString();
                        int kitapTurKodu = reader.GetInt32(reader.GetOrdinal("KitapTurKodu"));


                        textBox1.Text = kitapAdi;
                        textBox2.Text = yazarAdi;
                        textBox3.Text = yazarSoyadi;
                        textBox4.Text = Isbn;
                        textBox5.Text = kitapTurKodu.ToString();
                    }


                
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Bağlantı Hatası!" + ex.Message);

            }
            finally {

                baglanti.Close();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            anaEkran.kitapVerileriniGoster();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void ValidateTextBox(TextBox textBox, ref bool hataDurumu, string hataMesaji)
        {
            hataDurumu = false;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = hataMesaji;
                hataDurumu = true;
                textBox.BackColor = Color.Red;
            }
            else
            {
                textBox.BackColor = Color.Green;
            }
            if (textBox.Text == hataMesaji)
            {
                hataDurumu = true;
                textBox.BackColor = Color.Red;
            }

        }

        private bool kitapTuruMevcutMu()
        {
            if (Convert.ToInt32(textBox5.Text) < 0 || Convert.ToInt32(textBox5.Text) > 255)
            {
                textBox5.Text = "Geçersiz Tür Numarası";
                textBox5.BackColor = Color.Red;
                return false;
            }

            string query = "SELECT KitapTurKodu FROM TableKitapTurleri WHERE KitapTurKodu = @p1";
            SqlCommand sqlCommand = new SqlCommand(query, baglanti);
            sqlCommand.Parameters.AddWithValue("@p1", textBox5.Text);

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    textBox5.BackColor = Color.Green;
                    return true;
                }
                else
                {
                    textBox5.Text = "Tür Numarası Mevcut Değil";
                    textBox5.BackColor = Color.Red;
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                return false;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private bool isbnMevcutMu()
        {
            string query = "SELECT ISBN FROM TableKitaplar WHERE ISBN = @p1";


            SqlCommand sqlCommand = new SqlCommand(query, baglanti);
            sqlCommand.Parameters.AddWithValue("@p1", textBox4.Text);

            try
            {

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    if(reader["ISBN"].ToString() == alinanIsbn)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                return false;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void SayiGirisiKontrolu(TextBox textBox, ref bool hataDurumu, string hataMesaji)
        {
            if (!hataDurumu)
            {
                if (!long.TryParse(textBox.Text, out _))
                {
                    hataDurumu = true;
                    textBox.Text = hataMesaji;
                    textBox.BackColor = Color.Red;
                }
                else
                {
                    hataDurumu = false;
                    textBox.BackColor = Color.Green;
                }
            }

        }

        private async void kitabiDegistir()
        {
           
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "UPDATE TableKitaplar SET " +
            "KitapAdi = @kitapAdi, YazarAdi = @yazarAdi, YazarSoyadi = @yazarSoyadi, ISBN = @isbn, KitapTurKodu = @kitapTurKodu " +
            "WHERE ID = @id";

            SqlCommand sqlCommand = new SqlCommand(query, baglanti);

            sqlCommand.Parameters.AddWithValue("@kitapAdi", textBox1.Text);
            sqlCommand.Parameters.AddWithValue("@yazarAdi", textBox2.Text);
            sqlCommand.Parameters.AddWithValue("@yazarSoyadi", textBox3.Text);
            sqlCommand.Parameters.AddWithValue("@isbn", textBox4.Text);
            sqlCommand.Parameters.AddWithValue("@kitapTurKodu", textBox5.Text);
            sqlCommand.Parameters.AddWithValue("@id", alinanId);

            sqlCommand.ExecuteNonQuery();

                button1.ForeColor = Color.Green;
                button1.Text = "Kaydedildi";
                anaEkran.kitapVerileriniGoster();
                await Task.Delay(1500);
                button1.Text = "Kitabı Düzenle";
                textBox1.BackColor = default;
                textBox2.BackColor = default;
                textBox3.BackColor = default;
                textBox4.BackColor = default;
                textBox5.BackColor = default;

                button1.ForeColor = Color.Black;

            }
            catch (Exception ex) {
            MessageBox.Show("Hata Oluştu!" + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }


        }

        private async void kitabiSil()
        {
            try
            {
                baglanti.Open();

                string query = "DELETE FROM TableKitaplar WHERE ISBN = @p1";

                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@p1", alinanIsbn);
                sqlCommand.ExecuteNonQuery();

                MessageBox.Show(alinanIsbn + " ISBN'li kitap başarıyla silindi.");

                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;

                button2.ForeColor = Color.Red;
                button2.Text = "Silindi";
                anaEkran.kitapVerileriniGoster();
                button1.Enabled = false;
                button2.Enabled = false;
                await Task.Delay(2500);
                button2.Visible = false;
                button1.Visible = false;
                label7.Visible = true;
            }
            catch (Exception ex) {

                MessageBox.Show("Sorun Oluştu!" + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            bool kitapAdiHatasi = false;
            bool yazarAdiHatasi = false;
            bool yazarSoyadHatasi = false;
            bool isbnHatasi = false;
            bool kitapTuruHatasi = false;

            ValidateTextBox(textBox1, ref kitapAdiHatasi, "Kitap Adı Boş Olamaz");
            ValidateTextBox(textBox2, ref yazarAdiHatasi, "Yazar Adı Boş Olamaz");
            ValidateTextBox(textBox3, ref yazarSoyadHatasi, "Yazar Soyadı Boş Olamaz");
            ValidateTextBox(textBox4, ref isbnHatasi, "ISBN Numarası Boş Olamaz");
            ValidateTextBox(textBox5, ref kitapTuruHatasi, "Kitap Türü Boş Olamaz");

            if (!isbnHatasi)
            {
                SayiGirisiKontrolu(textBox4, ref isbnHatasi, "ISBN Numarası Harf Olamaz");
            }
            if (!kitapTuruHatasi)
            {
                SayiGirisiKontrolu(textBox5, ref kitapTuruHatasi, "Kitap Türü Harf Olamaz");
            }


            if (!kitapTuruHatasi && !kitapTuruMevcutMu())
            {
                kitapTuruHatasi = true;
            }


            if (!isbnHatasi)
            {
                if (isbnMevcutMu())
                {
                    textBox4.Text = "Bu ISBN Zaten Kayıtlı";
                    isbnHatasi = true;
                    textBox4.BackColor = Color.Red;
                }
                else if (textBox4.Text.Length != 10 && textBox4.Text.Length != 13 && textBox4.Text.Length !=12)
                {
                    textBox4.Text = "Lütfen 10, 12 veya 13 hane giriniz";
                    isbnHatasi = true;
                    textBox4.BackColor = Color.Red;
                }
                else
                {
                    textBox4.BackColor = Color.Green;
                    isbnHatasi = false;
                }
            }

            if (!kitapAdiHatasi && !yazarAdiHatasi && !yazarSoyadHatasi && !isbnHatasi && !kitapTuruHatasi)
            {
                kitabiDegistir();
            }



        }
        private void hataMesajiMi(TextBox textBox, string hataMesaji)
        {
            if (textBox.Text == hataMesaji)
            {
                textBox.Clear();
            }
        }


        private void textBox1_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox1, "Kitap Adı Boş Olamaz");
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox2, "Yazar Adı Boş Olamaz");
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox3, "Yazar Soyadı Boş Olamaz");
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox4, "ISBN Numarası Boş Olamaz");
            hataMesajiMi(textBox4, "ISBN Numarası Harf Olamaz");
            hataMesajiMi(textBox4, "Bu ISBN Zaten Kayıtlı");
            hataMesajiMi(textBox4, "Lütfen 10 veya 13 hane giriniz");
        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox5, "Kitap Türü Boş Olamaz");
            hataMesajiMi(textBox5, "Kitap Türü Harf Olamaz");
            hataMesajiMi(textBox5, "Tür Numarası Mevcut Değil");
            hataMesajiMi(textBox5, "Geçersiz Tür Numarası");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show( alinanIsbn + "ISBN'li Kitabı silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                kitabiSil();
            }
        }
    }
}
