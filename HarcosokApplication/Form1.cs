using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HarcosokApplication
{
    public partial class Form1 : Form
    {
        MySqlConnection conn = null;
        MySqlCommand sql = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = "localhost";
            sb.UserID = "root";
            sb.Password = "";
            sb.Database = "cs_harcosok";
            sb.CharacterSet = "utf8";

            conn = new MySqlConnection(sb.ToString());

            try
            {
                conn.Open();
                sql = conn.CreateCommand();

                tablaMuveletek();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
                Environment.Exit(0);
                return;
            }

            
            

        }

        private void tablaMuveletek()
        {

            using (sql = new MySqlCommand("CREATE TABLE IF NOT EXISTS harcosok (" +
                "id INT AUTO_INCREMENT," +
                "nev VARCHAR(255) UNIQUE," +
                "letrehozas DATE DEFAULT CURRENT_TIMESTAMP," +
                "PRIMARY KEY (id))", conn))
            { 
                sql.ExecuteNonQuery();
            }

            using (sql = new MySqlCommand("CREATE TABLE IF NOT EXISTS kepessegek (" +
                "id INT AUTO_INCREMENT," +
                "nev VARCHAR(255)," +
                "leiras VARCHAR(255)," +
                "harcos_id INT," +
                "PRIMARY KEY (id))", conn))
            { 
                sql.ExecuteNonQuery();
            }

            using (sql = new MySqlCommand("ALTER TABLE kepessegek ADD FOREIGN KEY (harcos_id) REFERENCES harcosok(id) ON DELETE RESTRICT ON UPDATE RESTRICT", conn))
            { 
                sql.ExecuteNonQuery();
            }

        }

        private void letrehozasButton_Click(object sender, EventArgs e)
        {
            string harcosNev = harcosNeveTextBox.Text.ToString().Trim();

            try
            {
                using (sql = new MySqlCommand("INSERT INTO harcosok (nev) VALUES ('" + harcosNev + "')", conn))
                {
                    MySqlDataReader reader;
                    reader = sql.ExecuteReader();
                    MessageBox.Show("Sikeres név felvétel!");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
                Environment.Exit(0);
                return;
            }

        }
    }
}
