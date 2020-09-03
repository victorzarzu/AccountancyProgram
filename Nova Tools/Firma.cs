using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.IO;

namespace Nova_Tools
{
    public partial class Firma : Form
    {
        string nume, cui, numar_inregistrare, adresa, punct_de_lucru, capital_social, banca, cont_bancar, trezorerie, cont_trezorerie, telefon_fix, telefon_mobil, email, site;
        SqlConnection conn;
        private void save_button_Click(object sender, EventArgs e)
        {
            conn.Open();

            bool just_update = false;
            if (not_empty())
                just_update = true;
            nume = nume_textbox.Text; cui = cui_textbox.Text; numar_inregistrare = numar_inreg_textbox.Text; adresa = adresa_textbox.Text; punct_de_lucru = punct_lucru_textbox.Text;
            capital_social = capital_textbox.Text; banca = banca_textbox.Text; cont_bancar = cont_bancar_textbox.Text; trezorerie = trezorerie_textbox.Text; cont_trezorerie = cont_trezorerie_textbox.Text;
            telefon_fix = telefon_fix_textbox.Text; telefon_mobil = telefon_mobil_textbox.Text; email = email_textbox.Text; site = site_textbox.Text;
            if (!just_update)
            {
                SqlCommand insert = new SqlCommand("INSERT INTO Firma(Id, Nume, CUI, Numar_de_inregistrare, Adresa, Punct_de_lucru, Capital_social, Banca, Cont_bancar, Trezorerie, Cont_trezorerie, Telefon_fix, Telefon_mobil, Email, Site) values(@id, @nume, @cui, @numar_inregistrare, @adresa, @punct_de_lucru, @capital_social, @banca, @cont_bancar, @trezorerie, @cont_trezorerie, @telefon_fix, @telefon_mobil, @email, @site)", conn);
                insert.Parameters.AddWithValue("@id", 1); insert.Parameters.AddWithValue("@nume", nume); insert.Parameters.AddWithValue("@cui", cui); insert.Parameters.AddWithValue("@numar_inregistrare", numar_inregistrare);
                insert.Parameters.AddWithValue("@adresa", adresa); insert.Parameters.AddWithValue("@punct_de_lucru", punct_de_lucru); insert.Parameters.AddWithValue("@capital_social", capital_social);
                insert.Parameters.AddWithValue("@banca", banca); insert.Parameters.AddWithValue("@cont_bancar", cont_bancar); insert.Parameters.AddWithValue("@trezorerie", trezorerie);
                insert.Parameters.AddWithValue("@cont_trezorerie", cont_trezorerie); insert.Parameters.AddWithValue("@telefon_fix", telefon_fix); insert.Parameters.AddWithValue("@telefon_mobil", telefon_mobil);
                insert.Parameters.AddWithValue("@email", email); insert.Parameters.AddWithValue("@site", site);

                insert.ExecuteNonQuery();
                insert.Dispose();
                conn.Close();

                return;
            }

            SqlCommand update = new SqlCommand("UPDATE Firma SET Nume = @nume, CUI = @cui, Numar_de_inregistrare = @numar_inregistrare, Adresa = @adresa, Punct_de_lucru = @punct_de_lucru, Capital_social = @capital_social, Banca = @banca, Cont_bancar = @cont_bancar, Trezorerie = @trezorerie, Cont_trezorerie = @cont_trezorerie, Telefon_fix = @telefon_fix, Telefon_mobil = @telefon_mobil, Email = @email, Site = @site WHERE Id = @id", conn);
            update.Parameters.AddWithValue("@id", 1); update.Parameters.AddWithValue("@nume", nume); update.Parameters.AddWithValue("@cui", cui); update.Parameters.AddWithValue("@numar_inregistrare", numar_inregistrare);
            update.Parameters.AddWithValue("@adresa", adresa); update.Parameters.AddWithValue("@punct_de_lucru", punct_de_lucru); update.Parameters.AddWithValue("@capital_social", capital_social);
            update.Parameters.AddWithValue("@banca", banca); update.Parameters.AddWithValue("@cont_bancar", cont_bancar); update.Parameters.AddWithValue("@trezorerie", trezorerie);
            update.Parameters.AddWithValue("@cont_trezorerie", cont_trezorerie); update.Parameters.AddWithValue("@telefon_fix", telefon_fix); update.Parameters.AddWithValue("@telefon_mobil", telefon_mobil);
            update.Parameters.AddWithValue("@email", email); update.Parameters.AddWithValue("@site", site);

            update.ExecuteNonQuery();
            update.Dispose();

            conn.Close();
        }
        public Firma()
        {
            InitializeComponent();
        }

        private void Firma_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"\SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            this.ActiveControl = save_button;

            conn.Open();

            SqlCommand read = new SqlCommand("SELECT * FROM Firma", conn);
            SqlDataReader dr = read.ExecuteReader();

            dr.Read();
            nume_textbox.Text = dr[1].ToString(); cui_textbox.Text = dr[2].ToString(); numar_inreg_textbox.Text = dr[3].ToString(); adresa_textbox.Text = dr[4].ToString(); punct_lucru_textbox.Text = dr[5].ToString();
            capital_textbox.Text = dr[6].ToString(); banca_textbox.Text = dr[7].ToString(); cont_bancar_textbox.Text = dr[8].ToString(); trezorerie_textbox.Text = dr[9].ToString(); cont_trezorerie_textbox.Text = dr[10].ToString();
            telefon_fix_textbox.Text = dr[11].ToString(); telefon_mobil_textbox.Text = dr[12].ToString(); email_textbox.Text = dr[13].ToString(); site_textbox.Text = dr[14].ToString();

            conn.Close();
        }

        bool not_empty()
        {
            string command = "SELECT * FROM Firma WHERE Id = @val";
            SqlCommand query = new SqlCommand(command, conn);

            query.Parameters.AddWithValue("@val", 1);
            SqlDataReader dr = query.ExecuteReader();
            bool exist = dr.HasRows;
            query.Dispose(); dr.Close();

            if (exist)
                return true;
            return false;
        }
    }
}
