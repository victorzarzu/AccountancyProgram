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
using System.Runtime.InteropServices.WindowsRuntime;

namespace Nova_Tools
{
    public partial class Plati : Form
    {
        public Form parentForm { get; set; }
        SqlConnection conn;
        DataTable niruri;
        DataColumn col;
        DataRow row;

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

        public Plati()
        {
            InitializeComponent();

            niruri = new DataTable();

            col = new DataColumn("Numar_de_receptie");
            col.DataType = System.Type.GetType("System.Int32");
            niruri.Columns.Add(col);

            col = new DataColumn("Nume_furnizor");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);

            col = new DataColumn("Numar_factura");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);

            col = new DataColumn("Data");
            col.DataType = System.Type.GetType("System.DateTime");
            niruri.Columns.Add(col);

            col = new DataColumn("Valoare_totala");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);

            col = new DataColumn("Rest_de_plata");
            col.DataType = System.Type.GetType("System.Double");
            niruri.Columns.Add(col);

            col = new DataColumn("Mod_plata");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);

            col = new DataColumn("Data_platii");
            col.DataType = System.Type.GetType("System.DateTime");
            niruri.Columns.Add(col);

            datetime_set();
        }

        void load_furnizori()
        {
            SqlCommand query = new SqlCommand("SELECT Nume FROM Furnizori ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            while (dr.Read())
                furnizor_combobox.Items.Add(dr[0].ToString());
            dr.Close(); query.Dispose();
        }

        private void Plati_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"\SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            start_date_picker.Format = end_date_picker.Format = DateTimePickerFormat.Custom;
            start_date_picker.CustomFormat = end_date_picker.CustomFormat = "dd/MM/yyyy";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.RowHeadersVisible = dataGridView1.AllowUserToAddRows = dataGridView1.AllowUserToResizeColumns = dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.RowHeadersVisible = false;
            furnizor_combobox.SelectedIndex = 0;
            numerar_radio.Checked = true;

            conn.Open();

            SqlCommand query = new SqlCommand("SELECT Numar_de_receptie, Nume_furnizor, Numar_factura, Data, Valoare_totala, Rest_de_plata, Data_platii from Niruri ORDER BY Numar_de_receptie ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            string[] data = new string[8];
            int index = -1;

            while (dr.Read())
            {
                row = niruri.NewRow();
                ++index;

                DateTime start, finish;
                start = start_date_picker.Value; finish = end_date_picker.Value;

                row["Numar_de_receptie"] = Convert.ToInt32(dr[0].ToString());  data[0] = dr[0].ToString();
                row["Nume_furnizor"] = dr[1].ToString(); data[1] = dr[1].ToString();
                row["Numar_factura"] = dr[2].ToString(); data[2] = dr[2].ToString();
                row["Data"] = DateTime.Parse(dr[3].ToString());

                DateTime dat = DateTime.Parse(row["Data"].ToString());
                string days = dat.Day.ToString(), month = dat.Month.ToString(), year = dat.Year.ToString();
                if (days.Length == 1)
                    days = "0" + days;
                if (month.Length == 1)
                    month = "0" + month;
                string date = days + "." + month + "." + year;
                data[3] = date;

                row["Valoare_totala"] = Convert.ToDouble(dr[4].ToString()); data[4] = dr[4].ToString();
                row["Rest_de_plata"] = Convert.ToDouble(dr[5].ToString()); data[5] = dr[5].ToString();
                if(Convert.ToDouble(row["Rest_de_plata"]) == 0)
                    row["Data_platii"] = DateTime.Parse(dr[6].ToString()).ToShortDateString();
                data[7] = index.ToString();

                niruri.Rows.Add(row);

                if(DateTime.Compare(start, DateTime.Parse(dr[3].ToString())) <= 0 && DateTime.Compare(finish, DateTime.Parse(dr[3].ToString())) >= 0 && Convert.ToDouble(dr[5].ToString()) != 0)
                    dataGridView1.Rows.Add(data);

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dataGridView1.AllowUserToAddRows = false;
            }

            dr.Close();  query.Dispose();

            load_furnizori();

            conn.Close();

            dataGridView1.AllowUserToResizeRows = false;
        }

        private void Plati_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }

        bool is_ok(string furnizor, string numar_factura, DateTime start, DateTime end, bool paid, DataRow row1)
        {
            if (!row1["Nume_furnizor"].ToString().ToLower().Contains(furnizor.ToString().ToLower())) return false;
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
            foreach(DataRow row1 in niruri.Rows)
            {
                if (is_ok(furnizor, numar_factura, start, end, paid, row1))
                {
                    data[0] = row1["Numar_de_receptie"].ToString();
                    data[1] = row1["Nume_furnizor"].ToString();
                    data[2] = row1["Numar_factura"].ToString();

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

                        dat = DateTime.Parse(row1["Data_platii"].ToString());
                        days = dat.Day.ToString(); month = dat.Month.ToString(); year = dat.Year.ToString();
                        if (days.Length == 1)
                            days = "0" + days;
                        if (month.Length == 1)
                            month = "0" + month;
                        date = days + "." + month + "." + year;
                        data[3] = date;
                        data[5] = "";
                    }

                    dataGridView1.Rows.Add(data);
                }
            }
        }
        private void search_button_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            if(platit_checkbox.Checked)
            {
                dataGridView1.Columns[5].Visible = dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[3].HeaderText = "Data platii";
            }
            else
            {
                dataGridView1.Columns[5].Visible = dataGridView1.Columns[6].Visible =true;
                dataGridView1.Columns[3].HeaderText = "Data";
            }
            string furnizor = "";
            if (furnizor_combobox.SelectedIndex > 0)
                furnizor = furnizor_combobox.SelectedItem.ToString();

            search(furnizor, numar_factura_textbox.Text, start_date_picker.Value, end_date_picker.Value, platit_checkbox.Checked);
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
        }

        private void plateste_button_Click(object sender, EventArgs e)
        {
            string modalitate_de_plata = metoda_de_plata();

            if(valoare_plata_textbox.Text == "")
            {
                MessageBox.Show("Introdu o valoare pentru plata!");
                return;
            }

            double valoare_plata = Convert.ToDouble(valoare_plata_textbox.Text);

            foreach(DataGridViewRow row1 in dataGridView1.Rows)
            {
                if(Convert.ToBoolean(row1.Cells[6].Value) == true)
                {
                    double cat = Math.Min(Convert.ToDouble(niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Rest_de_plata"]), valoare_plata);
                    niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Rest_de_plata"] = Convert.ToDouble(niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Rest_de_plata"]) - cat;
                    valoare_plata -= cat;
                    valoare_plata = Math.Round(valoare_plata, 2);
                    row1.Cells[5].Value = Convert.ToDouble(niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Rest_de_plata"]);

                    if (Convert.ToDouble(niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Rest_de_plata"]) == 0)
                        niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Mod_plata"] = modalitate_de_plata;

                    niruri.Rows[Convert.ToInt32(row1.Cells[7].Value.ToString())]["Data_platii"] = DateTime.Now;
                }
            }
            foreach (DataGridViewRow row1 in dataGridView1.Rows)
                if (Convert.ToDouble(row1.Cells[5].Value) == 0)
                    dataGridView1.Rows.Remove(row1);

            conn.Open();

            SqlCommand update = new SqlCommand("UPDATE Niruri SET Rest_de_plata = @rest_de_plata, Mod_plata = @mod_plata, Data_platii = @data_platii WHERE Numar_de_receptie = @numar_de_receptie", conn);

            foreach(DataRow row1 in niruri.Rows)
            {
                if (row1["Data_platii"].ToString() == "" || DateTime.Parse(row1["Data_platii"].ToString()).ToShortDateString() != DateTime.Parse(DateTime.Now.ToShortDateString()).ToShortDateString())
                    continue;

                update.Parameters.Clear();
                update.Parameters.AddWithValue("@rest_de_plata", Convert.ToDouble(row1["Rest_de_plata"]));
                update.Parameters.AddWithValue("@mod_plata", row1["Mod_plata"].ToString());
                update.Parameters.AddWithValue("@data_platii", DateTime.Parse(row1["Data_platii"].ToString()));
                update.Parameters.AddWithValue("@numar_de_receptie", Convert.ToInt32(row1["Numar_de_receptie"].ToString()));

                update.ExecuteNonQuery();
            }

            conn.Close();

            clear_plata();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            int numar_de_receptie = Convert.ToInt32(dataGridView1[0, e.RowIndex].Value);
            Nir nir = new Nir(numar_de_receptie);
            nir.parentForm = this;
            nir.Show();
            this.Hide();
        }
    }
}
