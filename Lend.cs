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
    public partial class Lend : Form
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=" + Properties.Settings.Default.dbName + ";Integrated Security=True;");

        private string alinanID, alinanISBN;

        private MainScreen anaEkran;
        public Lend(string kitapID, string isbn, MainScreen anaEkran)
        {
            InitializeComponent();
            alinanID = kitapID;
            alinanISBN = isbn;
            textBox4.Text = alinanID;
            textBox5.Text = alinanISBN;
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

        private bool sayiGirisiMi()
        {
            if(int.TryParse(textBox1.Text, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool kitapKutuphanedeMi()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "SELECT Situation FROM TableBooks WHERE ID = @p1";

                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@p1", alinanID);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool durum = Convert.ToBoolean(reader["Situation"]);
                        return durum;

                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return true;
            }
            finally
            {
                baglanti.Close();
            }

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if (sayiGirisiMi() && kitapKutuphanedeMi())
            {

                try
                {
                    if (baglanti.State == ConnectionState.Closed)
                    {
                        baglanti.Open();
                    }

                    string query = "UPDATE TableBooks SET Borrower = @p1, BorrowingDate = @p2, Situation = @p3 WHERE ID = @p4";
                    SqlCommand sqlCommand = new SqlCommand( query, baglanti);
                    sqlCommand.Parameters.AddWithValue("@p1", textBox1.Text);
                    sqlCommand.Parameters.Add("@p2", SqlDbType.Date).Value = dateTimePicker1.Value;
                    sqlCommand.Parameters.Add("@p3", SqlDbType.Bit).Value = false;
                    sqlCommand.Parameters.Add("@p4", SqlDbType.Int).Value = alinanID;

                    sqlCommand.ExecuteNonQuery();

                    button1.Text = "Success!";
                    button1.ForeColor = Color.Green;
                    anaEkran.kitapVerileriniGoster();
                    await Task.Delay(2000);
                    button1.Text = "Lend";
                    button1.ForeColor = default;
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
                finally
                {
                    baglanti.Close();
                }


            }
            else
            {
                if (!sayiGirisiMi()) {
                    textBox1.Text = "Please enter only numbers";
                    textBox1.BackColor = Color.Red;
                }
                else if (!kitapKutuphanedeMi())
                {
                    textBox1.Text = "The selected book is not available in the library";
                    textBox1.BackColor = Color.Red;
                }


            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Please enter only numbers" || textBox1.Text == "The selected book is not available in the library")
            {
                textBox1.Clear();
                textBox1.BackColor = default;
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor= Cursors.Default;
        }
    }
}
