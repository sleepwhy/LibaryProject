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
    public partial class LoginScreen : Form
    {
        public LoginScreen()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
            
        }

       
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

            Properties.Settings.Default.dbName = textBox3.Text;
            try
            {
                SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog="+ Properties.Settings.Default.dbName + ";Integrated Security=True;");

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                string query = "SELECT Password FROM TableLibraryAdmins WHERE Username = @p1";
                SqlCommand sqlCommand = new SqlCommand(query, baglanti);
                sqlCommand.Parameters.AddWithValue("@p1", textBox1.Text);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read()) {
                    if(textBox2.Text == reader["Password"].ToString())
                    {
                        label4.Text = "Password is correct, logging in...";
                        
                        using (MainScreen girisYapildi = new MainScreen()) {
                            this.Hide();
                            girisYapildi.ShowDialog();
                        }
                    }
                    else
                    {
                        label4.Text = "Incorrect password or username!";
                        baglanti.Close();
                        await Task.Delay(2000);
                        label4.Text = "";
                    }
                }
                else
                {
                    label4.Text = "Incorrect password or username!";
                    baglanti.Close();
                    await Task.Delay(2000);
                    label4.Text = "";
                }
                baglanti.Close();

            }
            catch (Exception ex) 
            {
                MessageBox.Show("Connection Error!" + ex.Message);
            }
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            textBox3.Text = Properties.Settings.Default.dbName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show("Click yes for copy the DB script", "Need a DB?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string script = @"
                        
                        USE [master]
                        GO
                        /****** Object:  Database [DBLibrarySystem]    Script Date: 28.04.2025 11:30:47 ******/
                        CREATE DATABASE [DBLibrarySystem]
                         CONTAINMENT = NONE
                         ON  PRIMARY 
                        ( NAME = N'DbLibrarySystem_main', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\DbLibrarySystem_main.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
                         LOG ON 
                        ( NAME = N'DbLibrarySystem_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\DbLibrarySystem_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
                         WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET COMPATIBILITY_LEVEL = 160
                        GO
                        IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
                        begin
                        EXEC [DBLibrarySystem].[dbo].[sp_fulltext_database] @action = 'enable'
                        end
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ANSI_NULL_DEFAULT OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ANSI_NULLS OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ANSI_PADDING OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ANSI_WARNINGS OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ARITHABORT OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET AUTO_CLOSE OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET AUTO_SHRINK OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET AUTO_UPDATE_STATISTICS ON 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET CURSOR_CLOSE_ON_COMMIT OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET CURSOR_DEFAULT  GLOBAL 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET CONCAT_NULL_YIELDS_NULL OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET NUMERIC_ROUNDABORT OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET QUOTED_IDENTIFIER OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET RECURSIVE_TRIGGERS OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET  DISABLE_BROKER 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET DATE_CORRELATION_OPTIMIZATION OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET TRUSTWORTHY OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ALLOW_SNAPSHOT_ISOLATION OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET PARAMETERIZATION SIMPLE 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET READ_COMMITTED_SNAPSHOT OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET HONOR_BROKER_PRIORITY OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET RECOVERY SIMPLE 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET  MULTI_USER 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET PAGE_VERIFY CHECKSUM  
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET DB_CHAINING OFF 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET TARGET_RECOVERY_TIME = 60 SECONDS 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET DELAYED_DURABILITY = DISABLED 
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET ACCELERATED_DATABASE_RECOVERY = OFF  
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET QUERY_STORE = ON
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
                        GO
                        USE [DBLibrarySystem]
                        GO
                        /****** Object:  Table [dbo].[TableBooks]    Script Date: 28.04.2025 11:30:47 ******/
                        SET ANSI_NULLS ON
                        GO
                        SET QUOTED_IDENTIFIER ON
                        GO
                        CREATE TABLE [dbo].[TableBooks](
	                        [ID] [int] IDENTITY(1,1) NOT NULL,
	                        [BookName] [nvarchar](50) NULL,
	                        [AuthorName] [nvarchar](50) NULL,
	                        [AuthorSurname] [nvarchar](50) NULL,
	                        [ISBN] [nvarchar](50) NULL,
	                        [BookTypeNo] [int] NULL,
	                        [Situation] [bit] NULL,
	                        [Borrower] [varchar](50) NULL,
	                        [BorrowingDate] [date] NULL
                        ) ON [PRIMARY]
                        GO
                        /****** Object:  Table [dbo].[TableBookTypes]    Script Date: 28.04.2025 11:30:47 ******/
                        SET ANSI_NULLS ON
                        GO
                        SET QUOTED_IDENTIFIER ON
                        GO
                        CREATE TABLE [dbo].[TableBookTypes](
	                        [TypeName] [nvarchar](50) NULL,
	                        [TypeID] [int] NOT NULL PRIMARY KEY,
                         CONSTRAINT [UQ_TableBookTypes] UNIQUE NONCLUSTERED 
                        (
	                        [TypeID] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY]
                        GO
                        /****** Object:  Table [dbo].[TableLibraryAdmins]    Script Date: 28.04.2025 11:30:47 ******/
                        SET ANSI_NULLS ON
                        GO
                        SET QUOTED_IDENTIFIER ON
                        GO
                        CREATE TABLE [dbo].[TableLibraryAdmins](
	                        [Username] [nvarchar](50) NULL,
	                        [Password] [nvarchar](50) NULL
                        ) ON [PRIMARY]
                        GO
                        USE [master]
                        GO
                        ALTER DATABASE [DBLibrarySystem] SET  READ_WRITE 
                        GO

						USE [DBLibrarySystem]
						GO

						INSERT INTO [dbo].[TableBookTypes] (TypeName, TypeID)
						VALUES 
						('Novel', 101),
						('Science Fiction', 102),
						('Biography', 103),
						('Philosophy', 104);

						INSERT INTO [dbo].[TableLibraryAdmins] (Username, Password)
						VALUES 
						('admin', 'admin');

						-- Books 
						INSERT INTO [dbo].[TableBooks] (BookName, AuthorName, AuthorSurname, ISBN, BookTypeNo, Situation, Borrower, BorrowingDate)
						VALUES
						('The Great Gatsby', 'F. Scott', 'Fitzgerald', '9780743273565', 101, 1, NULL, NULL), -- Novel
						('To Kill a Mockingbird', 'Harper', 'Lee', '9780061120084', 101, 1, NULL, NULL), -- Novel
						('Dune', 'Frank', 'Herbert', '9780441013593', 102, 1, NULL, NULL), -- Science Fiction
						('Neuromancer', 'William', 'Gibson', '9780441569595', 102, 1, NULL, NULL), -- Science Fiction
						('Steve Jobs', 'Walter', 'Isaacson', '9781451648539', 103, 1, NULL, NULL), -- Biography
						('Long Walk to Freedom', 'Nelson', 'Mandela', '9780316548182', 103, 1, NULL, NULL), -- Biography
						('Meditations', 'Marcus', 'Aurelius', '9780140449334', 104, 1, NULL, NULL), -- Philosophy
						('The Republic', 'Plato', '', '9780140455113', 104, 1, NULL, NULL), -- Philosophy
						('1984', 'George', 'Orwell', '9780451524935', 101, 1, NULL, NULL), -- Novel
						('Foundation', 'Isaac', 'Asimov', '9780553293357', 102, 1, NULL, NULL), -- Science Fiction
						('Becoming', 'Michelle', 'Obama', '9781524763138', 103, 1, NULL, NULL), -- Biography
						('Beyond Good and Evil', 'Friedrich', 'Nietzsche', '9780140449235', 104, 1, NULL, NULL); -- Philosophy
                        ";

            if (answer == DialogResult.Yes)
            {
                Clipboard.SetText(script);
                MessageBox.Show("Copied!", "Success");
            } 
        }
    }
}
