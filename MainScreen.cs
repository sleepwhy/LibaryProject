using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KutuphaneProjesi
{
    public partial class MainScreen : Form
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=" + Properties.Settings.Default.dbName + ";Integrated Security=True;");

        public bool siyahTemaAcikMi = false;

        private bool aramaMenusuKapali = true;
        public MainScreen()
        {
            InitializeComponent();
            label2.ForeColor = Color.Red;
            ThemeHelper.TemaUygula(this, siyahTemaAcikMi);
            label14.Top = 275;

        }



        private void button1_Click(object sender, EventArgs e)
        {
            using (NewRecord yeniKayit = new NewRecord(this))
            {
                yeniKayit.ShowDialog();
            }

        }

        public void kitapVerileriniGoster()
        {
            try
            {
                SqlDataAdapter dataAdapter;
                DataTable dt = new DataTable();


                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                if (aramaMenusuKapali)
                {

                    string query = "SELECT * FROM TableBooks";
                    dataAdapter = new SqlDataAdapter(query, baglanti);
                }
                else if(textBox1.Text.Length == 0 && textBox2.Text.Length == 0 && textBox3.Text.Length == 0 && textBox4.Text.Length == 0 && textBox5.Text.Length == 0) {
                    string query = "SELECT * FROM TableBooks";
                    dataAdapter = new SqlDataAdapter(query, baglanti);
                }
                else
                {

                    string query = "SELECT * FROM TableBooks WHERE " +
                                   "ID LIKE @p1 OR BookName LIKE @p2 OR " +
                                   "AuthorName LIKE @p3 OR AuthorSurname LIKE @p4 OR ISBN LIKE @p5";


                    string p1, p2, p3, p4, p5;


                    p1 = textBox1.Text.Length > 0 ? "%" + textBox1.Text + "%" : "**";
                    p2 = textBox2.Text.Length > 0 ? "%" + textBox2.Text + "%" : "**";
                    p3 = textBox3.Text.Length > 0 ? "%" + textBox3.Text + "%" : "**";
                    p4 = textBox4.Text.Length > 0 ? "%" + textBox4.Text + "%" : "**";
                    p5 = textBox5.Text.Length > 0 ? "%" + textBox5.Text + "%" : "**";

                    SqlCommand sqlCommand = new SqlCommand(query, baglanti);

                    sqlCommand.Parameters.AddWithValue("@p1", p1);
                    sqlCommand.Parameters.AddWithValue("@p2", p2);
                    sqlCommand.Parameters.AddWithValue("@p3", p3);
                    sqlCommand.Parameters.AddWithValue("@p4", p4);
                    sqlCommand.Parameters.AddWithValue("@p5", p5);

                    dataAdapter = new SqlDataAdapter(sqlCommand);
                }


                dataAdapter.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;
                    label15.Visible = false;
                }
                else
                {
                    label15.Visible = true;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapat
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }

            // DataGridView ayarları
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView1.AllowUserToAddRows = false;
        }


        private void AnaEkran_Load(object sender, EventArgs e)
        {
            kitapVerileriniGoster();
            dataGridView1.ClearSelection();
        }

        private void AnaEkran_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private string secilenKontrol()
        {
            int secilenSatir = dataGridView1.SelectedCells[0].RowIndex;
            string secilenID = dataGridView1.Rows[secilenSatir].Cells[0].Value?.ToString();
            label2.Text = "Selected ID:" + secilenID;
            label2.ForeColor = Color.Green;
            return secilenID;
        }

        private string secilenIsbn()
        {
            int secilenSatir = dataGridView1.SelectedCells[0].RowIndex;
            string secilenIsbn = dataGridView1.Rows[secilenSatir].Cells[4].Value?.ToString();
            return secilenIsbn;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            secilenKontrol();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            secilenKontrol();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                label1.Text = secilenKontrol();

                using (EditRecord kayitDuzenle = new EditRecord(secilenKontrol(), secilenIsbn(), this))
                {
                    kayitDuzenle.ShowDialog();
                }
                kitapVerileriniGoster();
                secilenKontrol();

            }
            catch (Exception) {
                label1.Visible = true;
                await Task.Delay(2000);
                label1.Visible = false;

            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                label1.Text = secilenKontrol();

                using (Lend oduncVer = new Lend(secilenKontrol(), secilenIsbn(), this))
                {
                    oduncVer.ShowDialog();
                }
                kitapVerileriniGoster();
                secilenKontrol();
            }
            catch (Exception)
            {
                label1.Visible = true;
                await Task.Delay(2000);
                label1.Visible = false;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                label1.Text = secilenKontrol();
                label1.Text = "";

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "SELECT Situation FROM TableBooks WHERE ID = @p1";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.Add("@p1", SqlDbType.VarChar).Value = secilenKontrol();

                SqlDataReader reader = sqlCommand.ExecuteReader();
                bool durum;
                if (reader.Read())
                {
                    durum = Convert.ToBoolean(reader["Situation"]);
                }
                else
                {
                    durum = false;
                }
                if (!durum) {
                    using (Refund iade = new Refund(secilenKontrol(), this))
                    {
                        reader.Close();
                        iade.ShowDialog();
                    }
                    kitapVerileriniGoster();
                }
                else
                {
                    label1.Visible = true;
                    label1.Text = "This book has not been borrowed!";
                    await Task.Delay(2000);
                    label1.Visible = false;
                }



            }
            catch (Exception)
            {
                label1.Visible = true;
                await Task.Delay(2000);
                label1.Visible = false;
            }
            finally
            {
                baglanti.Close();

                secilenKontrol();
            }
        }

        private void istatistikGuncelle()
        {

                try
                {
                    if (baglanti.State == ConnectionState.Closed)
                    {
                        baglanti.Open();
                    }

                    string query = "SELECT COUNT(*) FROM TableBooks";
                    SqlCommand sqlCommand = new SqlCommand(query, baglanti);

                    int toplamSatirSayisi = (int)sqlCommand.ExecuteScalar();

                    label4.Text = toplamSatirSayisi.ToString();


                        query = "SELECT COUNT(*) FROM TableBooks WHERE Situation = 0";
                        sqlCommand.CommandText = query;

                        int durumSayisi = (int)sqlCommand.ExecuteScalar();

                        label7.Text = durumSayisi.ToString();
                    



                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error" + ex.Message);


                }
                finally { baglanti.Close(); }
            


        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            if(progressBar1.Value + 1 >= progressBar1.Maximum) {
            progressBar1.Value = 0;
            }

            if(progressBar1.Value == 0)
            {
                istatistikGuncelle();
                progressBar1.Value += 1;
            }
            else
            {
                progressBar1.Value += 1;
            }

        }


        private void button5_Click(object sender, EventArgs e)
        {
            aramaMenusuKapali = !aramaMenusuKapali;


            for (int i = 9; i <= 13; i++)
            {
                Control label = this.Controls["label" + i];
                if (label != null)
                {
                    label.Visible = !aramaMenusuKapali;
                    label.Top = 350;
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                Control textBox = this.Controls["textBox" + i];
                if (textBox != null)
                {
                    textBox.Visible = !aramaMenusuKapali;
                    textBox.Top = 350;
                }
            }


            groupBox1.Top = aramaMenusuKapali ? 256 : 400;
            label14.Visible = aramaMenusuKapali ? false : true;
            label15.Top += aramaMenusuKapali ? -100 : 100;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            kitapVerileriniGoster();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            kitapVerileriniGoster();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            kitapVerileriniGoster();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            kitapVerileriniGoster();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            kitapVerileriniGoster();
        }

        private void AnaEkran_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            siyahTemaAcikMi = !siyahTemaAcikMi;
            if (!siyahTemaAcikMi)
            {
                ThemeHelper.TemaUygula(this, siyahTemaAcikMi);
                label2.BackColor = SystemColors.Window;

            }
            else
            {
                BackColor = Color.Black;
                ThemeHelper.TemaUygula(this, siyahTemaAcikMi);
                label2.BackColor = Color.Black;
            }
        }
    }
}