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

                string query = "SELECT * FROM TableBooks WHERE ID = @id";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@id", alinanId);

                using(SqlDataReader reader = sqlCommand.ExecuteReader()) 
                {
                    if (reader.Read()) {
                        string kitapAdi = reader["BookName"].ToString();
                        string yazarAdi = reader["AuthorName"].ToString();
                        string yazarSoyadi = reader["AuthorSurname"].ToString();
                        string Isbn = reader["ISBN"].ToString();
                        int kitapTurKodu = reader.GetInt32(reader.GetOrdinal("BookTypeNo"));


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

                MessageBox.Show("Connection Error!" + ex.Message);

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

        private async void kitabiDegistir()
        {
           
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "UPDATE TableBooks SET " +
            "BookName = @kitapAdi, AuthorName = @yazarAdi, AuthorSurname = @yazarSoyadi, ISBN = @isbn, BookTypeNo = @kitapTurKodu " +
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
                button1.Text = "Saved";
                anaEkran.kitapVerileriniGoster();
                await Task.Delay(1500);
                button1.Text = "Edit Book";
                textBox1.BackColor = default;
                textBox2.BackColor = default;
                textBox3.BackColor = default;
                textBox4.BackColor = default;
                textBox5.BackColor = default;

                button1.ForeColor = Color.Black;

            }
            catch (Exception ex) {
            MessageBox.Show("Error!" + ex.Message);
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

                string query = "DELETE FROM TableBooks WHERE ISBN = @p1";

                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@p1", alinanIsbn);
                sqlCommand.ExecuteNonQuery();

                MessageBox.Show("Book(ISBN: " + alinanIsbn + " ) was deleted successfully.");

                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;

                button2.ForeColor = Color.Red;
                button2.Text = "Deleted!";
                anaEkran.kitapVerileriniGoster();
                button1.Enabled = false;
                button2.Enabled = false;
                await Task.Delay(2500);
                button2.Visible = false;
                button1.Visible = false;
                label7.Visible = true;
            }
            catch (Exception ex) {

                MessageBox.Show("Error!" + ex.Message);
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

            ValidateTextBox(textBox1, ref kitapAdiHatasi, "Book Title Cannot Be Empty");
            ValidateTextBox(textBox2, ref yazarAdiHatasi, "Author Name Cannot Be Empty");
            ValidateTextBox(textBox3, ref yazarSoyadHatasi, "Author Surname Cannot Be Empty");
            ValidateTextBox(textBox4, ref isbnHatasi, "ISBN Number Cannot Be Empty");
            ValidateTextBox(textBox5, ref kitapTuruHatasi, "Book Type Cannot Be Empty");

            if (!isbnHatasi)
            {
                SayiGirisiKontrolu(textBox4, ref isbnHatasi, "ISBN Number Cannot Be Letters");
            }
            if (!kitapTuruHatasi)
            {
                SayiGirisiKontrolu(textBox5, ref kitapTuruHatasi, "Book Type Cannot Be Letter");
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
                else if (textBox4.Text.Length != 10 && textBox4.Text.Length != 13 && textBox4.Text.Length !=12)
                {
                    textBox4.Text = "Please enter 10, 12 or 13 digits";
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
            hataMesajiMi(textBox1, "Book Title Cannot Be Empty");
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox2, "Author Name Cannot Be Empty");
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox3, "Author Surname Cannot Be Empty");
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox4, "ISBN Number Cannot Be Empty");
            hataMesajiMi(textBox4, "ISBN Number Cannot Be Letters");
            hataMesajiMi(textBox4, "This ISBN is already registered");
            hataMesajiMi(textBox4, "Please enter 10 or 13 digits");
        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            hataMesajiMi(textBox5, "Book Type Cannot Be Empty");
            hataMesajiMi(textBox5, "Book Type Cannot Be Letter");
            hataMesajiMi(textBox5, "Type Number Not Available");
            hataMesajiMi(textBox5, "Invalid Type Number");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete the book with ISBN"+ alinanIsbn, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                kitabiSil();
            }
        }
    }
}
