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
                harcosokListaja();
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
                    harcosokListaja();
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

            string kivalszottHarcos = null;

            if (hasznaloComboBox.SelectedItem == null)
            {
                MessageBox.Show("Nem válaszottál ki harcost!");
                return;
            }
            else
            {
                kivalszottHarcos = hasznaloComboBox.SelectedItem.ToString();
            }

            using (sql = new MySqlCommand("SELECT id FROM harcosok WHERE nev = '" + kivalszottHarcos + "'", conn))
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

            harcosokKepessegei();
        }

        private void harcosokListaja()
        {
            harcosokListBox.Items.Clear();

            sql = new MySqlCommand("SELECT nev, letrehozas FROM harcosok ORDER BY id", conn);
            dr = sql.ExecuteReader();

            while (dr.Read())
            {
                DateTime dateTime = DateTime.Now;
                dateTime.ToString("yyyy/MM/dd");

                harcosokListBox.Items.Add(dr[0].ToString() + "\t" + dr[1].ToString().Substring(0,12));
            }

            dr.Close();
        }

        private void harcosokKepessegei()
        {
            kepessegekListBox.Items.Clear();
            leirasaTextBox.Clear();

            string kivalaszottHarcos = null;

            if (harcosokListBox.SelectedItem == null)
            {
                return;
            }
            else
            {
                kivalaszottHarcos = harcosokListBox.SelectedItem.ToString().Split('\t')[0];
            }

            int kivalszottharcosIDje = 0;

            try
            {
                using (sql = new MySqlCommand("SELECT id FROM harcosok WHERE nev='" + kivalaszottHarcos + "'", conn))
                {
                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        kivalszottharcosIDje = (int)dr[0];
                    }

                    dr.Close();
                }

                using (sql = new MySqlCommand("SELECT nev FROM kepessegek WHERE harcos_id =" + kivalszottharcosIDje, conn))
                {
                    dr = sql.ExecuteReader();

                    while (dr.Read())
                    {
                        kepessegekListBox.Items.Add(dr[0].ToString());
                    }

                    dr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void harcosokListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            harcosokKepessegei();
        }

        private void kepessegekListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            leirasaTextBox.Clear();

            string kivalasztottKepesseg = null;

            if (kepessegekListBox.SelectedItem == null)
            {
                leirasaTextBox.Clear();
            }
            else
            {
                kivalasztottKepesseg = kepessegekListBox.SelectedItem.ToString();
            }

            int kivalasztottKepessegIDje = 0;

            try
            {
                using (sql = new MySqlCommand("SELECT id FROM kepessegek WHERE nev='" + kivalasztottKepesseg + "'", conn))
                {
                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        kivalasztottKepessegIDje = (int)dr[0];
                    }

                    dr.Close();
                }

                using (sql = new MySqlCommand("SELECT leiras FROM kepessegek WHERE id=" + kivalasztottKepessegIDje, conn))
                {
                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        leirasaTextBox.Text = dr[0].ToString();
                    }

                    dr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void torlesButton_Click(object sender, EventArgs e)
        {
            string torlendoKepesseg = null;

            if (kepessegekListBox.SelectedItem == null)
            {
                MessageBox.Show("Nincs kiválaszott képesség!");
            }
            else
            {
                torlendoKepesseg = kepessegekListBox.SelectedItem.ToString();
            }

            int torlendKepessegIDje = 0;

            try
            {
                using (sql = new MySqlCommand("SELECT id FROM kepessegek WHERE nev ='" + torlendoKepesseg + "'", conn))
                {
                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        torlendKepessegIDje = (int)dr[0];
                    }

                    dr.Close();
                }

                using (sql = new MySqlCommand("DELETE FROM kepessegek WHERE id=" + torlendKepessegIDje, conn))
                {
                    dr = sql.ExecuteReader();

                    MessageBox.Show("Képesség törölve!");

                    dr.Close();

                    harcosokKepessegei();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void modositasButton_Click(object sender, EventArgs e)
        {
            string modositandoKepesseg = null;

            if (kepessegekListBox.SelectedItem == null)
            {
                MessageBox.Show("Nincs kiválaszott képesség!");
            }
            else
            {
                modositandoKepesseg = kepessegekListBox.SelectedItem.ToString();
            }

            int modositandoKepessegIDje = 0;
            string modositandoLeiras = null;

            if (leirasaTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show("Üres képességleirást adtál meg!");
                return;
            }
            else
            {
                modositandoLeiras = leirasaTextBox.Text.ToString();
            }

            try
            {
                using (sql = new MySqlCommand("SELECT id FROM kepessegek WHERE nev ='" + modositandoKepesseg + "'", conn))
                {
                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        modositandoKepessegIDje = (int)dr[0];
                    }

                    dr.Close();
                }

                using (sql = new MySqlCommand("UPDATE kepessegek SET leiras = '" + modositandoLeiras + "' WHERE id =" + modositandoKepessegIDje, conn))
                {
                    dr = sql.ExecuteReader();

                    MessageBox.Show("Képességleírás frissítve!");

                    dr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
