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
using System.IO;
using System.Runtime.CompilerServices;

namespace Nova_Tools
{
    public partial class Facturi : Form
    {
        public Form parentForm { get; set; }

        SqlConnection conn;
        DataTable facturi;
        DataColumn col;
        DataRow row;
        int index = -1;
        string client = "";
        string nume, cui, numar_de_inregistrare, adresa, punct_de_lucru, capital_social, banca, cont_bancar, trezoreria, cont_trezorerie, telefon_fix, telefon_mobil, email, site;
        string adresa_client, cui_client, nr_inreg;
        double valoare_plata, valoare_plata_copy;
        string numar_facturi = "contravaloare facturi ";
        string numar = "";
        Dictionary<string, string> suma_text = new Dictionary<string, string>();
        public Facturi()
        {
            InitializeComponent();

            facturi = new DataTable();

            col = new DataColumn("Index");
            col.DataType = System.Type.GetType("System.Int32");
            facturi.Columns.Add(col);

            col = new DataColumn("Numar_factura");
            col.DataType = System.Type.GetType("System.Int32");
            facturi.Columns.Add(col);

            col = new DataColumn("Nume_client");
            col.DataType = System.Type.GetType("System.String");
            facturi.Columns.Add(col);

            col = new DataColumn("Data");
            col.DataType = System.Type.GetType("System.DateTime");
            facturi.Columns.Add(col);

            col = new DataColumn("Valoare_totala");
            col.DataType = System.Type.GetType("System.Double");
            facturi.Columns.Add(col);

            col = new DataColumn("Rest_de_plata");
            col.DataType = System.Type.GetType("System.Double");
            facturi.Columns.Add(col);

            col = new DataColumn("Mod_plata");
            col.DataType = System.Type.GetType("System.String");
            facturi.Columns.Add(col);

            col = new DataColumn("Data_platii");
            col.DataType = System.Type.GetType("System.DateTime");
            facturi.Columns.Add(col);

            datetime_set();

            start_date_picker.Format = end_date_picker.Format = DateTimePickerFormat.Custom;
            start_date_picker.CustomFormat = end_date_picker.CustomFormat = "dd/MM/yyyy";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.RowHeadersVisible = dataGridView1.AllowUserToAddRows = dataGridView1.AllowUserToResizeColumns = dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.RowHeadersVisible = false;
            client_combobox.SelectedIndex = 0;

            numerar_radio.Checked = true;
        }
        void datetime_set()
        {
            int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            DateTime start = DateTime.Parse(month.ToString() + "/1" + "/" + year.ToString());
            DateTime finish = DateTime.Parse(month.ToString() + "/1" + "/" + year.ToString());
            start_date_picker.Value = start;
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
                finish = DateTime.Parse(month.ToString() + "/31" + "/" + year.ToString());
            else if (month == 2)
            {
                if (year % 4 == 0 || year % 400 == 0)
                    finish = DateTime.Parse(month.ToString() + "/29" + "/" + year.ToString());
                else
                    DateTime.Parse(month.ToString() + "/28" + "/" + year.ToString());
            }
            else
                finish = DateTime.Parse(month.ToString() + "/30" + "/" + year.ToString());
            end_date_picker.Value = finish;
        }

        private void Facturi_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }
        void load_clienti()
        {
            SqlCommand query = new SqlCommand("SELECT Nume FROM Clienti ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            while (dr.Read())
                client_combobox.Items.Add(dr[0].ToString());

            dr.Close(); query.Dispose();
        }

        void load_facturi()
        {
            SqlCommand query = new SqlCommand("SELECT Numar_factura, Nume_client, Data, Valoare_totala, Rest_de_plata, Mod_plata, Data_platii FROM Facturi ORDER BY Numar_factura ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            string[] data = new string[6];

            while(dr.Read())
            {
                ++index;
                row = facturi.NewRow();
                row["Index"] = index; row["Numar_factura"] = dr[0].ToString(); row["Nume_client"] = dr[1].ToString(); row["Data"] = DateTime.Parse(DateTime.Parse(dr[2].ToString()).ToShortDateString());
                row["Valoare_totala"] = Convert.ToDouble(dr[3].ToString()); row["Rest_de_plata"] = Convert.ToDouble(dr[4].ToString()); row["Mod_plata"] = dr[5].ToString();
                if(Convert.ToDouble(row["Rest_de_plata"]) == 0)
                    row["Data_platii"] = DateTime.Parse(DateTime.Parse(dr[6].ToString()).ToShortDateString());

                facturi.Rows.Add(row);

                if (Convert.ToDouble(row["Rest_de_plata"]) == 0)
                    continue;

                DateTime dat = DateTime.Parse(row["Data"].ToString());
                string days = dat.Day.ToString(), month = dat.Month.ToString(), year = dat.Year.ToString();
                if (days.Length == 1)
                    days = "0" + days;
                if (month.Length == 1)
                    month = "0" + month;
                string date = days + "." + month + "." + year;

                data[0] = index.ToString(); data[1] = dr[0].ToString(); data[2] = dr[1].ToString();
                data[3] = date; data[4] = dr[3].ToString().Replace('.', ','); data[5] = dr[4].ToString().Replace('.', ',');

                if(DateTime.Compare(start_date_picker.Value, dat) <= 0 && DateTime.Compare(end_date_picker.Value, dat) >= 0)
                    dataGridView1.Rows.Add(data);
            }
            dr.Close(); query.Dispose();
        }

        void create_dictionary()
        {
            suma_text.Add("1", "unu");
            suma_text.Add("2", "doi");
            suma_text.Add("3", "trei");
            suma_text.Add("4", "patru");
            suma_text.Add("5", "cinci");
            suma_text.Add("6", "sase");
            suma_text.Add("7", "sapte");
            suma_text.Add("8", "opt");
            suma_text.Add("9", "noua");
            suma_text.Add("10", "zece");
            suma_text.Add("11", "unusprezece");
            suma_text.Add("12", "doisprezece");
            suma_text.Add("13", "treisprezece");
            suma_text.Add("14", "patrusprezece");
            suma_text.Add("15", "cincisprezece");
            suma_text.Add("16", "sasesprezece");
            suma_text.Add("17", "saptesprezece");
            suma_text.Add("18", "optsprezece");
            suma_text.Add("19", "nouasprezece");
            suma_text.Add("20", "douazeci");
            suma_text.Add("30", "treizeci");
            suma_text.Add("40", "patruzeci");
            suma_text.Add("50", "cincizeci");
            suma_text.Add("60", "sasezeci");
            suma_text.Add("70", "saptezeci");
            suma_text.Add("80", "optzeci");
            suma_text.Add("90", "nouazeci");
            suma_text.Add("100", "osuta");
            suma_text.Add("200", "douasute");
            suma_text.Add("300", "treisute");
            suma_text.Add("400", "patrusute");
            suma_text.Add("500", "cincisute");
            suma_text.Add("600", "sasesute");
            suma_text.Add("700", "saptesute");
            suma_text.Add("800", "optsute");
            suma_text.Add("900", "nouasute");
            suma_text.Add("1000", "omie");
            suma_text.Add("2000", "douamii");
            suma_text.Add("3000", "treimii");
            suma_text.Add("4000", "patrumii");
            suma_text.Add("5000", "cincimii");
            suma_text.Add("6000", "sasemii");
            suma_text.Add("7000", "saptemii");
            suma_text.Add("8000", "optmii");
            suma_text.Add("9000", "nouamii");

        }

        private void Facturi_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"\SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            conn.Open();

            load_clienti();
            load_facturi();
            load_data();
            create_dictionary();

            dataGridView1.AllowUserToAddRows = dataGridView1.AllowUserToResizeRows = dataGridView1.RowHeadersVisible = false;

            conn.Close();

            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("PaperA4", 826, 1169);
        }

        string metoda_de_plata()
        {
            string metoda = "Numerar";

            if (ordin_radio.Checked)
                metoda = "Ording de plata";
            else if (bilet_radio.Checked)
                metoda = "Bilet la ordin";
            else if (cec_radio.Checked)
                metoda = "CEC";
            else if (bon_radio.Checked)
                metoda = "Bon fiscal";

            return metoda;
        }

        void clear_plata()
        {
            numerar_radio.Checked = true;
            ordin_radio.Checked = bilet_radio.Checked = cec_radio.Checked = bon_radio.Checked = false;
            valoare_plata_textbox.Clear();

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
                row1.Cells[6].Value = false;
        }

        void plateste_sql()
        {
            conn.Open();

            SqlCommand update = new SqlCommand("UPDATE Facturi SET Rest_de_plata = @rest_de_plata, Mod_plata = @mod_plata, Data_platii = @data_platii WHERE Numar_factura = @numar_factura", conn);

            foreach (DataRow row1 in facturi.Rows)
            {
                if (row1["Data_platii"].ToString() == "" || DateTime.Parse(row1["Data_platii"].ToString()).ToShortDateString() != DateTime.Parse(DateTime.Now.ToShortDateString()).ToShortDateString())
                    continue;
                
                update.Parameters.Clear();
                update.Parameters.AddWithValue("@numar_factura", Convert.ToInt32(row1["Numar_factura"].ToString()));
                update.Parameters.AddWithValue("@rest_de_plata", Convert.ToDouble(row1["Rest_de_plata"]));
                update.Parameters.AddWithValue("@mod_plata", row1["Mod_plata"].ToString());
                update.Parameters.AddWithValue("@data_platii", DateTime.Parse(row1["Data_platii"].ToString()));

                update.ExecuteNonQuery();
            }

            update.Dispose();

            conn.Close();
        }

        void load_client()
        {
            conn.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM Clienti WHERE Nume = @nume", conn);
            query.Parameters.AddWithValue("@nume", client);
            SqlDataReader dr = query.ExecuteReader();
            dr.Read();
            adresa_client = dr[2].ToString(); cui_client = dr[3].ToString(); nr_inreg = dr[4].ToString();

            dr.Close(); query.Dispose();

            conn.Close();
        }

        void load_chitanta()
        {
            using (StreamReader sr = new StreamReader("numar_chitanta.txt"))
            {
                numar = sr.ReadLine();
                numar = (Convert.ToInt32(numar) + 1).ToString();

                sr.Close();
            }

            File.Delete("numar_chitanta.txt");
            
            using(FileStream fs = File.Create("numar_chitanta.txt"))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(numar);
                fs.Write(text, 0, text.Length);
            }

        }

        private void plateste_button_Click(object sender, EventArgs e)
        {
            string modalitate_de_plata = metoda_de_plata();

            if (valoare_plata_textbox.Text == "")
            {
                MessageBox.Show("Introdu o valoare pentru plata!");
                return;
            }

            bool found = false;

            valoare_plata = valoare_plata_copy = Convert.ToDouble(valoare_plata_textbox.Text);
            if(valoare_plata <= 0)
            {
                MessageBox.Show("Introdu o valoare mai mare decat 0!");
                return;
            }

            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row1.Cells[6].Value) == true)
                {
                    found = true;
                    client = row1.Cells[2].Value.ToString();
                    numar_facturi += row1.Cells[1].Value.ToString() + " ,";
                    double cat = Math.Min(Convert.ToDouble(facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Rest_de_plata"]), valoare_plata);
                    facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Rest_de_plata"] = Convert.ToDouble(facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Rest_de_plata"]) - cat;
                    valoare_plata -= cat;
                    valoare_plata = Math.Round(valoare_plata, 2);
                    row1.Cells[5].Value = Convert.ToDouble(facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Rest_de_plata"]);

                    if(Convert.ToDouble(facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Rest_de_plata"].ToString()) == 0)
                        facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Mod_plata"] = modalitate_de_plata;

                    facturi.Rows[Convert.ToInt32(row1.Cells[0].Value.ToString())]["Data_platii"] = DateTime.Now;
                }
            }
            foreach (DataGridViewRow row1 in dataGridView1.Rows)
                if (Convert.ToDouble(row1.Cells[5].Value) == 0)
                    dataGridView1.Rows.Remove(row1);

            numar_facturi = numar_facturi.Remove(numar_facturi.Length - 2);

            if(!found)
            {
                MessageBox.Show("Trebuie sa selectati cel putin o factura!");
                return;
            }

            clear_plata();
            plateste_sql();
            load_client();
            load_chitanta();

            printPreviewDialog1.ShowDialog();
        }
        bool is_ok(string furnizor, string numar_factura, DateTime start, DateTime end, bool paid, DataRow row1)
        {
            if (!row1["Nume_client"].ToString().ToLower().Contains(furnizor.ToString().ToLower())) return false;
            if (!row1["Numar_factura"].ToString().ToLower().Contains(numar_factura_textbox.Text.ToLower())) return false;
            if (!paid)
            {
                if (Convert.ToDouble(row1["Rest_de_plata"].ToString()) == 0) return false;
                if (!(DateTime.Compare(start, DateTime.Parse(row1["Data"].ToString())) <= 0 && DateTime.Compare(end, DateTime.Parse(row1["Data"].ToString())) >= 0)) return false;
            }
            else
            {
                if (Convert.ToDouble(row1["Rest_de_plata"].ToString()) != 0) return false;
                if (!(DateTime.Compare(start, DateTime.Parse(row1["Data_platii"].ToString())) <= 0 && DateTime.Compare(end, DateTime.Parse(row1["Data_platii"].ToString())) >= 0)) return false;
            }
            return true;
        }


        void search(string furnizor, string numar_factura, DateTime start, DateTime end, bool paid)
        {
            string[] data = new string[6];
            foreach (DataRow row1 in facturi.Rows)
            {
                if (is_ok(furnizor, numar_factura, start, end, paid, row1))
                {
                    data[2] = row1["Nume_client"].ToString();
                    data[1] = row1["Numar_factura"].ToString();

                    DateTime dat = DateTime.Parse(row1["Data"].ToString());
                    string days = dat.Day.ToString(), month = dat.Month.ToString(), year = dat.Year.ToString();
                    if (days.Length == 1)
                        days = "0" + days;
                    if (month.Length == 1)
                        month = "0" + month;
                    string date = days + "." + month + "." + year;
                    data[3] = date;
                    data[4] = row1["Valoare_totala"].ToString();
                    data[5] = row1["Rest_de_plata"].ToString();

                    if (paid)
                    {
                        data[3] = DateTime.Parse(row1["Data_platii"].ToString()).ToShortDateString();
                        data[5] = "";
                    }

                    if (DateTime.Compare(start_date_picker.Value, dat) <= 0 && DateTime.Compare(end_date_picker.Value, dat) >= 0)
                        dataGridView1.Rows.Add(data);
                }
            }
        }
        private void search_button_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            if (platit_checkbox.Checked)
            {
                dataGridView1.Columns[5].Visible = dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[3].HeaderText = "Data platii";
            }
            else
            {
                dataGridView1.Columns[5].Visible = dataGridView1.Columns[6].Visible = true;
                dataGridView1.Columns[3].HeaderText = "Data";
            }
            string furnizor = "";

            if (client_combobox.SelectedIndex > 0)
                furnizor = client_combobox.SelectedItem.ToString();

            search(furnizor, numar_factura_textbox.Text, start_date_picker.Value, end_date_picker.Value, platit_checkbox.Checked);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            Factura fact = new Factura(Convert.ToInt32(dataGridView1[1, e.RowIndex].Value.ToString()));
            fact.parentForm = this;
            fact.Show();
            this.Hide();
        }

        void load_data()
        {
            SqlCommand query = new SqlCommand("SELECT * FROM Firma WHERE Id = @id", conn);
            query.Parameters.AddWithValue("@id", 1);
            SqlDataReader dr = query.ExecuteReader();
            dr.Read();

            nume = dr[1].ToString(); cui = dr[2].ToString(); numar_de_inregistrare = dr[3].ToString(); adresa = dr[4].ToString(); punct_de_lucru = dr[5].ToString(); capital_social = dr[6].ToString();
            banca = dr[7].ToString(); cont_bancar = dr[8].ToString(); trezoreria = dr[9].ToString(); cont_trezorerie = dr[10].ToString(); telefon_fix = dr[11].ToString(); telefon_mobil = dr[12].ToString();
            email = dr[13].ToString(); site = dr[14].ToString();
        }

        void print_basis(System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = 600, height = 420, distance = 520;
            string font_family = "Arial";

            Font font_giant_bold = new Font(font_family, 20, FontStyle.Bold);
            Font font_big_bold = new Font(font_family, 14);
            Font font_medium = new Font(font_family, 12);
            Font font_normal = new Font(font_family, 11);
            int chit_startx = e.PageBounds.Width / 2 - width / 2, chit_starty = 75;
            string[] adr = adresa.Split(' ');
            string[] cap = capital_social.Split(' ');

            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 -  width / 2, 75), new Point(e.PageBounds.Width / 2 + width / 2, 75));
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 - width / 2, 75), new Point(e.PageBounds.Width / 2 - width / 2, 75 + height));
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 + width / 2, 75), new Point(e.PageBounds.Width / 2 + width / 2, 75 + height));
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 - width / 2, 75 + height), new Point(e.PageBounds.Width / 2 + width / 2, 75 + height));

            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 - width / 2, 75 + distance), new Point(e.PageBounds.Width / 2 + width / 2, 75 + distance));
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 - width / 2, 75 + distance), new Point(e.PageBounds.Width / 2 - width / 2, 75 + height + distance));
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 + width / 2, 75 + distance), new Point(e.PageBounds.Width / 2 + width / 2, 75 + height + distance));
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(e.PageBounds.Width / 2 - width / 2, 75 + height + distance), new Point(e.PageBounds.Width / 2 + width / 2, 75 + height + distance));

            g.DrawString("SC / PF / AF _______________________________", font_big_bold, Brushes.Black, new Point(chit_startx + 7, chit_starty += 9));
            g.DrawString(nume, font_normal, Brushes.Black, new Point(chit_startx + 120, chit_starty));
            g.DrawString("Adresa: ____________________________________,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(adresa, font_normal, Brushes.Black, new Point(chit_startx + 72, chit_starty));
            g.DrawString("Județ: ______________", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(adr[adr.Length - 1], font_normal, Brushes.Black, new Point(chit_startx + 65, chit_starty));            
            g.DrawString("Atribut fiscal: RO CUI: ________________", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(cui, font_normal, Brushes.Black, new Point(chit_startx + 180, chit_starty));
            g.DrawString("Nr. înreg. Reg. Com.: ________________", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(numar_de_inregistrare, font_normal, Brushes.Black, new Point(chit_startx + 180, chit_starty));
            g.DrawString("Capital social: ________ RON", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(cap[0], font_normal, Brushes.Black, new Point(chit_startx + 120, chit_starty));

            int chit_starty_copy = chit_starty;
            chit_starty = 75 + distance;

            g.DrawString("SC / PF / AF _______________________________", font_big_bold, Brushes.Black, new Point(chit_startx + 7, chit_starty += 9));
            g.DrawString(nume, font_normal, Brushes.Black, new Point(chit_startx + 120, chit_starty));
            g.DrawString("Adresa: ____________________________________,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(adresa, font_normal, Brushes.Black, new Point(chit_startx + 72, chit_starty));
            g.DrawString("Județ: ______________", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(adr[adr.Length - 1], font_medium, Brushes.Black, new Point(chit_startx + 65, chit_starty));
            g.DrawString("Atribut fiscal: RO CUI: ________________", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(cui, font_normal, Brushes.Black, new Point(chit_startx + 180, chit_starty));
            g.DrawString("Nr. înreg. Reg. Com.: ________________", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(numar_de_inregistrare, font_normal, Brushes.Black, new Point(chit_startx + 180, chit_starty));
            g.DrawString("Capital social: ________ RON", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 20));
            g.DrawString(cap[0], font_normal, Brushes.Black, new Point(chit_startx + 120, chit_starty));

            chit_starty = chit_starty_copy;

            g.DrawString("CHITANȚĂ", font_giant_bold, Brushes.Black, new Point(e.PageBounds.Width / 2 - 77, chit_starty + 25));
            g.DrawString("CHITANȚĂ", font_giant_bold, Brushes.Black, new Point(e.PageBounds.Width / 2 - 77, chit_starty + 25 + distance));

            g.DrawString("Seria .......... Nr ......................", font_medium, Brushes.Black, new Point(e.PageBounds.Width / 2 - 116, chit_starty += 60));
            g.DrawString("NTP        " + numar, font_medium, Brushes.Black, new Point(e.PageBounds.Width / 2 - 65, chit_starty - 3));
            g.DrawString("Seria .......... Nr ......................", font_medium, Brushes.Black, new Point(e.PageBounds.Width / 2 - 116, chit_starty + distance));
            g.DrawString("NTP        " + numar, font_medium, Brushes.Black, new Point(e.PageBounds.Width / 2 - 65, chit_starty - 3 + distance));

            DateTime dat = DateTime.Now;
            string days = dat.Day.ToString(), month = dat.Month.ToString(), year = dat.Year.ToString();
            if (days.Length == 1)
                days = "0" + days;
            if (month.Length == 1)
                month = "0" + month;
            string date = days + "." + month + "." + year;
            string valoare_string = valoare_plata_copy.ToString();
            string plata = "";
            int cifre = 0;
            if (valoare_plata_copy - Math.Truncate(valoare_plata_copy) == 0)
                valoare_string += ",00";
            for (int i = valoare_string.Length - 1; i > valoare_string.Length - 4; --i)
                plata = valoare_string[i] + plata;
            for(int i = valoare_string.Length - 4;i >= 0;--i)
            {
                plata = valoare_string[i] + plata;
                ++cifre;
                if(cifre == 3)
                {
                    plata = " " + plata;
                    cifre = 0;
                }
            }

            string plata_litere = "";
            int nr = 0, prelucrare = 0, zeci = 0, ordin = 0;
            string plata_litere_part = "";
            plata = " " + plata;
            bool exist_under_mie = false;
            for(int i = plata.Length - 1;i >= 0;--i)
            {
                if(plata[i] == ' ')
                {
                    ++ordin;
                    if(nr / 100 != 0)
                    {
                        prelucrare = nr / 100;
                        prelucrare *= 100;
                        plata_litere_part = suma_text[prelucrare.ToString()];
                    }
                    if((nr % 100) / 10 != 0)
                    {
                        prelucrare = (nr % 100) / 10;
                        prelucrare *= 10;
                        plata_litere_part += suma_text[prelucrare.ToString()];

                        if (nr % 10 != 0)
                            plata_litere_part += "si";
                    }
                    if(nr % 10 != 0)
                    {
                        prelucrare = nr % 10;
                        plata_litere_part += suma_text[prelucrare.ToString()];
                    }

                    if (ordin == 2 && nr != 0)
                        exist_under_mie = true;

                    if (ordin == 3 && nr <= 9 && exist_under_mie == false)
                        plata_litere_part = suma_text[(nr * 1000).ToString()];

                    else if (ordin == 3 && nr > 9)
                    {
                        if (nr < 9)
                            plata_litere_part += "mii";
                        else
                            plata_litere_part += "demii";
                    }

                    plata_litere = plata_litere_part + plata_litere;

                    plata_litere_part = "";
                    nr = 0;
                    zeci = 0;
                    continue;
                }

                if(plata[i] == '.' || plata[i] == ',')
                {
                    ++ordin;
                    if (nr / 100 != 0)
                    {
                        prelucrare = nr / 100;
                        prelucrare *= 100;
                        plata_litere_part = suma_text[prelucrare.ToString()];
                    }
                    if ((nr % 100) / 10 != 0)
                    {
                        prelucrare = (nr % 100) / 10;
                        prelucrare *= 10;
                        plata_litere_part += suma_text[prelucrare.ToString()];

                        if (nr % 10 != 0)
                            plata_litere_part += "si";
                    }

                    if (nr % 10 != 0)
                    {
                        prelucrare = nr % 10;
                        plata_litere_part += suma_text[prelucrare.ToString()];
                    }

                    plata_litere = "LEI" + plata_litere_part;

                    if (nr != 0)
                        plata_litere += "BANI";

                    plata_litere_part = "";
                    nr = 0;
                    zeci = 0;
                    continue;
                }

                ++zeci;
                if (zeci == 2)
                    nr = 10 * Convert.ToInt32(plata[i].ToString()) + nr;
                else if (zeci == 3)
                    nr = 100 * Convert.ToInt32(plata[i].ToString()) + nr;
                else
                    nr = Convert.ToInt32(plata[i].ToString());

            }

            string plata_litere_part1 = "", plata_litere_part2 = "";
            int litere = 0;
            for(int i = 0;i < plata_litere.Length;++i)
            {
                ++litere;
                if (litere > 33)
                    plata_litere_part2 += plata_litere[i];
                else
                    plata_litere_part1 += plata_litere[i];
            }

            g.DrawString("Data .......................................", font_medium, Brushes.Black, new Point(e.PageBounds.Width / 2 - 116, chit_starty += 24));
            g.DrawString(date, font_normal, Brushes.Black, new Point(e.PageBounds.Width / 2 - 73, chit_starty - 3));
            g.DrawString("Data .......................................", font_medium, Brushes.Black, new Point(e.PageBounds.Width / 2 - 116, chit_starty + distance));
            g.DrawString(date, font_normal, Brushes.Black, new Point(e.PageBounds.Width / 2 - 73, chit_starty - 3 + distance));

            g.DrawString("Am primit de la ...............................................................................................,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 35));
            g.DrawString(client, font_normal, Brushes.Black, new Point(chit_startx + 130, chit_starty - 3));
            g.DrawString("Am primit de la ...............................................................................................,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty + distance));
            g.DrawString(client, font_normal, Brushes.Black, new Point(chit_startx + 130, chit_starty - 3 + distance));

            g.DrawString("adresa ............................................................................................................,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(adresa_client, font_normal, Brushes.Black, new Point(chit_startx + 70, chit_starty - 3));
            g.DrawString("adresa ............................................................................................................,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty + distance));
            g.DrawString(adresa_client, font_normal, Brushes.Black, new Point(chit_startx + 70, chit_starty - 3 + distance));

            g.DrawString("CUI .............................................. Nr. ORC ...................................................,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(cui + "                                            " + nr_inreg, font_normal, Brushes.Black, new Point(chit_startx + 50, chit_starty - 3));
            g.DrawString("CUI .............................................. Nr. ORC ...................................................,", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty + distance));
            g.DrawString(cui + "                                            " + nr_inreg, font_normal, Brushes.Black, new Point(chit_startx + 50, chit_starty - 3 + distance));

            g.DrawString("suma de ................................... RON adica ....................................................", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(plata.Replace('.', ','), font_normal, Brushes.Black, new Point(chit_startx + 85, chit_starty - 3));
            g.DrawString("suma de ................................... RON adica ....................................................", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty + distance));
            g.DrawString(plata.Replace('.', ','), font_normal, Brushes.Black, new Point(chit_startx + 85, chit_starty - 3 + distance));
            g.DrawString(plata_litere_part1, font_normal, Brushes.Black, new Point(chit_startx + 340, chit_starty - 3));
            g.DrawString(plata_litere_part1, font_normal, Brushes.Black, new Point(chit_startx + 340, chit_starty - 3 + distance));

            g.DrawString(".........................................................................................................................", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(plata_litere_part2, font_normal, Brushes.Black, new Point(chit_startx + 10, chit_starty - 3));
            g.DrawString(".........................................................................................................................", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty + distance));
            g.DrawString(plata_litere_part2, font_normal, Brushes.Black, new Point(chit_startx + 10, chit_starty - 3 + distance));

            g.DrawString("reprezentând ...................................................................................................", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty += 24));
            g.DrawString(numar_facturi, font_medium, Brushes.Black, new Point(chit_startx + 120, chit_starty - 4));
            g.DrawString("reprezentând ...................................................................................................", font_medium, Brushes.Black, new Point(chit_startx + 7, chit_starty + distance));
            g.DrawString(numar_facturi, font_medium, Brushes.Black, new Point(chit_startx + 120, chit_starty - 4 + distance));

            g.DrawString("Casier, ", font_medium, Brushes.Black, new Point(chit_startx + 400, chit_starty += 32));
            g.DrawString("Casier, ", font_medium, Brushes.Black, new Point(chit_startx + 400, chit_starty + distance));

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            print_basis(e);

        }
    }
}
