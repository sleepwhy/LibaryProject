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
    public partial class Refund : Form
    {

        SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=" + Properties.Settings.Default.dbName + ";Integrated Security=True;");
        private string alinanID;
        private MainScreen anaEkran;

        public Refund(string id, MainScreen anaEkran)
        {
            InitializeComponent();
            alinanID = id;
            label10.ForeColor = Color.Red;
            this.anaEkran = anaEkran;
            label11.ForeColor = Color.Red;

            bool siyahTemaAcikMi = anaEkran.siyahTemaAcikMi;
            ThemeHelper.TemaUygula(this, siyahTemaAcikMi);

        }

        private void kitapBilgileriniDoldur()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "SELECT BookName, ISBN, Borrower FROM TableBooks WHERE ID = @p1";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.Add("@p1", SqlDbType.VarChar).Value = alinanID;

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read()) {
                    string kitapAdi = reader["BookName"].ToString();
                    string ISBN = reader["ISBN"].ToString();
                    int OduncAlan = Convert.ToInt32(reader["Borrower"]);

                    textBox1.Text = kitapAdi;
                    textBox2.Text = alinanID;
                    textBox3.Text = ISBN;
                    textBox4.Text = OduncAlan.ToString();
                }


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

        private void Iade_Load(object sender, EventArgs e)
        {
            kitapBilgileriniDoldur();
        }

        private void button2_Click(object sender, EventArgs e)
        {



            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                DateTime bugununTarihi = DateTime.Now;

                string query = "SELECT BorrowingDate FROM TableBooks WHERE ID = @p1";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.Add("@p1", SqlDbType.VarChar).Value = alinanID;

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read()) {
                    DateTime oduncAlmaTarihi = reader.GetDateTime(reader.GetOrdinal("BorrowingDate"));
                    
                    TimeSpan fark = bugununTarihi - oduncAlmaTarihi;
                    
                    int gunSayisi = fark.Days;

                    int gecikenGun = gunSayisi - 10;
                    if(gecikenGun <= 0)
                    {
                        textBox5.Text = "There is no day delay.";
                        textBox5.ForeColor = Color.Green;
                    }
                    else
                    {
                        textBox5.Text = gecikenGun.ToString();
                        textBox6.Text =(gecikenGun * 2).ToString() + "$";
                    }

                }
            }
            catch (Exception ex) {

                MessageBox.Show("Error: " + ex.Message);
            
            }finally { baglanti.Close(); }

        }

        private void cezaOdendi()
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "UPDATE TableBooks SET Borrower = @p1, BorrowingDate = @p2, Situation = @p3 WHERE ID = @p4";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);

                sqlCommand.Parameters.Add("@p1", SqlDbType.Int).Value = DBNull.Value;
                sqlCommand.Parameters.Add("@p2", SqlDbType.Date).Value = DBNull.Value;
                sqlCommand.Parameters.Add("@p3", SqlDbType.Bit).Value = true;
                sqlCommand.Parameters.Add("@p4", SqlDbType.Int).Value = alinanID;

                sqlCommand.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            if(textBox5.Text.Length == 0 && textBox6.Text.Length == 0)
            {
                label11.Visible = true;
                await Task.Delay(2000);
                label11.Visible = false;
            }
            else
            {
                cezaOdendi();
                anaEkran.kitapVerileriniGoster();
                button1.Text = "Penalty Paided";
                button1.ForeColor = Color.Green;
                await Task.Delay(1500);
                button1.Visible = false;
                label10.Visible = true;
            }


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
            Cursor = Cursors.Default;
        }
    }
}
