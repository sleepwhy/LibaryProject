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
    public partial class YeniKayit : Form
    {

        private AnaEkran anaEkran;
        SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbWhyKutuphane;Integrated Security=True;");
        public YeniKayit(AnaEkran anaEkran)
        {
            InitializeComponent();
            this.anaEkran = anaEkran;


            bool siyahTemaAcikMi = anaEkran.siyahTemaAcikMi;
            ThemeHelper.TemaUygula(this, siyahTemaAcikMi);


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor= Cursors.Default; 
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
                textBox.BackColor= Color.Green;
            }
            if(textBox.Text == hataMesaji)
            {
                hataDurumu = true;
                textBox.BackColor = Color.Red;
            }

        }

        private bool kitapTuruMevcutMu()
        {
            if (Convert.ToInt32(textBox5.Text) < 0 || Convert.ToInt32(textBox5.Text) > 255)
            {
                textBox5.Text = "Invalid Type Number";
                textBox5.BackColor = Color.Red;
                return false;
            }

            string query = "SELECT TypeID FROM TableBookTypes WHERE TypeID = @p1";
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
                    textBox5.Text = "Type Number Not Available";
                    textBox5.BackColor = Color.Red;
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
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
            string query = "SELECT ISBN FROM TableBooks WHERE ISBN = @p1";


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
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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
        
        private async void kitabiListeyeEkle(string kitapAdi, string yazarAdi, string yazarSoyadi, string isbn, int kitapTuru)
        {
            string query = "INSERT INTO TableBooks (BookName, AuthorName, AuthorSurname, ISBN, Situation, BookTypeNo) " +
                "VALUES (@p1, @p2, @p3, @p4, 1, @p5);";
            SqlCommand sqlCommand = new SqlCommand(query, baglanti);
            sqlCommand.Parameters.AddWithValue("@p1", kitapAdi);
            sqlCommand.Parameters.AddWithValue("@p2", yazarAdi);
            sqlCommand.Parameters.AddWithValue("@p3", yazarSoyadi);
            sqlCommand.Parameters.AddWithValue("@p4", isbn);
            sqlCommand.Parameters.AddWithValue("@p5", kitapTuru);

            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                sqlCommand.ExecuteNonQuery();

                button1.Text = "Book was Added!";
                button1.ForeColor = Color.Green;
                anaEkran.kitapVerileriniGoster();
                await Task.Delay(2000);
                button1.Text = "Save Book";
                button1.ForeColor = default;



            }
            catch (Exception ex)
            {
                button1.Text = "Failed to add the book.";
                button1.ForeColor = Color.Red;
                MessageBox.Show("Error" + ex.Message);
                await Task.Delay(2000);
                button1.Text = "Save Book";
                button1.ForeColor = default;
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

            ValidateTextBox(textBox1, ref kitapAdiHatasi, "Book Name cannot be empty");
            ValidateTextBox(textBox2, ref yazarAdiHatasi, "Author Name cannot be empty");
            ValidateTextBox(textBox3, ref yazarSoyadHatasi, "Author Surname cannot be empty");
            ValidateTextBox(textBox4, ref isbnHatasi, "ISBN Number cannot be empty");
            ValidateTextBox(textBox5, ref kitapTuruHatasi, "Book Genre cannot be empty");


            if (!isbnHatasi)
            {
                SayiGirisiKontrolu(textBox4, ref isbnHatasi, "ISBN Number cannot contain letters");
            }
            if (!kitapTuruHatasi)
            {
                SayiGirisiKontrolu(textBox5, ref kitapTuruHatasi, "Book Genre cannot contain letters");
            }



            if (!kitapTuruHatasi && !kitapTuruMevcutMu())
            {
                kitapTuruHatasi = true;
            }


            if (!isbnHatasi)
            {
                if (isbnMevcutMu())
                {
                    textBox4.Text = "This ISBN is already registered";
                    isbnHatasi = true;
                    textBox4.BackColor = Color.Red;
                }
                else if (textBox4.Text.Length != 10 && textBox4.Text.Length != 13 && textBox4.Text.Length != 12)
                {
                    textBox4.Text = "Please enter 10, 12, or 13 digits";
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
                kitabiListeyeEkle(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, Convert.ToInt32(textBox5.Text));
            }
        }


        private void hataMesajiMi(TextBox textBox, string hataMesaji)
        {
            if(textBox.Text == hataMesaji)
            {
                textBox.Clear();
            }
        }


        private void textBox1_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox1, "Book Name cannot be empty");
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox2, "Author Name cannot be empty");
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox3, "Author Surname cannot be empty");
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox4, "ISBN Number cannot be empty");
            hataMesajiMi(textBox4, "ISBN Number cannot contain letters");
            hataMesajiMi(textBox4, "This ISBN is already registered");
            hataMesajiMi(textBox4, "Please enter 10 or 13 digits");
        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox5, "Book Genre cannot be empty");
            hataMesajiMi(textBox5, "Book Genre cannot contain letters");
            hataMesajiMi(textBox5, "Genre Number does not exist");
            hataMesajiMi(textBox5, "Invalid Genre Number");
        }

    }
}
