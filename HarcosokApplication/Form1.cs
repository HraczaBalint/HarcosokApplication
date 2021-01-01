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
        MySqlDataReader dr = null;

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
                kepessegHasznalo();

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

            if (harcosNev.Length == 0)
            {
                MessageBox.Show("Üres nevet adtál meg!");
                harcosNeveTextBox.Clear();
                harcosNeveTextBox.Focus();
                return;
            }

            try
            {
                using (sql = new MySqlCommand("INSERT INTO harcosok (nev) VALUES ('" + harcosNev + "')", conn))
                {
                    dr = sql.ExecuteReader();
                    MessageBox.Show("Sikeres név felvétel!");
                    dr.Close();
                    harcosNeveTextBox.Clear();
                    harcosNeveTextBox.Focus();

                    kepessegHasznalo();
                }
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                harcosNeveTextBox.Clear();
                harcosNeveTextBox.Focus();
                return;
            }

        }

        private void kepessegHasznalo()
        {
            hasznaloComboBox.Items.Clear();

            sql = new MySqlCommand("SELECT nev FROM harcosok ORDER BY id", conn);
            dr = sql.ExecuteReader();

            while (dr.Read())
            {
                hasznaloComboBox.Items.Add(dr[0].ToString());
            }

            dr.Close();
        }

        private void hozzaadasButton_Click(object sender, EventArgs e)
        {
            string kepessegNev = kepessegNeveTextBox.Text.ToString().Trim();
            string kepessegLeirasa = leirasTextBox.Text.ToString().Trim();

            int harcos_id = 0;

            using (sql = new MySqlCommand("SELECT id FROM harcosok WHERE nev = '" + hasznaloComboBox.SelectedItem.ToString() + "'", conn))
            {
                dr = sql.ExecuteReader();

                if (dr.Read())
                {
                    harcos_id = (int)dr[0];
                }

                dr.Close();
            }

            if (kepessegNev.Length == 0)
            {
                MessageBox.Show("Üres képességnevet adtál meg!");
                kepessegNeveTextBox.Clear();
                kepessegNeveTextBox.Focus();
                return;
            }

            if (kepessegLeirasa.Length == 0)
            {
                MessageBox.Show("Üres képességleírást adtál meg!");
                leirasTextBox.Clear();
                leirasTextBox.Focus();
                return;
            }

            try
            {
                using (sql = new MySqlCommand("INSERT INTO kepessegek (nev, leiras, harcos_id) VALUES ('" + kepessegNev + "','" + kepessegLeirasa + "'," + harcos_id +")", conn))
                {
                    dr = sql.ExecuteReader();
                    MessageBox.Show("Sikeres képesség felvétel!");
                    dr.Close();
                    kepessegNeveTextBox.Clear();
                    leirasTextBox.Clear();
                    kepessegNeveTextBox.Focus();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                kepessegNeveTextBox.Clear();
                leirasTextBox.Clear();
                return;
            }
        }
    }
}
