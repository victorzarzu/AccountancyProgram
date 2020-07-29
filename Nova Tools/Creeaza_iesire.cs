using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

namespace Nova_Tools
{
    public partial class Creeaza_iesire : Form
    {
        public Form parentForm { get; set; }

        SqlConnection conn;
        DataTable stoc;
        DataTable factura;
        DataColumn col;
        DataRow row;
        int product_index;
        int numar_factura = 1;

        public Creeaza_iesire()
        {
            InitializeComponent();

            stoc = new DataTable();
            factura = new DataTable();

            //Stoc

            col = new DataColumn("Id");
            col.DataType = System.Type.GetType("System.Int32");
            stoc.Columns.Add(col);

            col = new DataColumn("Nume");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Brand");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Categorie");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.Double");
            stoc.Columns.Add(col);

            col = new DataColumn("Pret");
            col.DataType = System.Type.GetType("System.Double");
            stoc.Columns.Add(col);

            col = new DataColumn("UM");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Tip_discount");
            col.DataType = System.Type.GetType("System.String");
            stoc.Columns.Add(col);

            col = new DataColumn("Discount");
            col.DataType = System.Type.GetType("System.Double");
            stoc.Columns.Add(col);

            //Factura

            col = new DataColumn("Nume_produs");
            col.DataType = System.Type.GetType("System.String");
            factura.Columns.Add(col);

            col = new DataColumn("Pret_vanzare");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);

            col = new DataColumn("Pret_vanzare_initial");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);

            col = new DataColumn("Unitate_de_masura");
            col.DataType = System.Type.GetType("System.String");
            factura.Columns.Add(col);

            col = new DataColumn("Tip_discount_produs");
            col.DataType = System.Type.GetType("System.String");
            factura.Columns.Add(col);

            col = new DataColumn("Discount_produs");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);
        }

        void load_stoc()
        {
            SqlCommand query = new SqlCommand("SELECT * FROM Stoc WHERE Cantitate > 0 ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            string[] data = new string[7];
            int index = -1;

            while(dr.Read())
            {
                row = stoc.NewRow();

                row["Id"] = ++index; data[0] = index.ToString();
                row["Nume"] = dr[1].ToString(); data[1] = dr[1].ToString();
                row["Categorie"] = dr[2].ToString(); data[4] = dr[3].ToString();
                row["Brand"] = dr[3].ToString(); data[5] = dr[2].ToString();
                row["Cantitate"] = Convert.ToDouble(dr[4].ToString()); data[2] = dr[4].ToString();
                row["Pret"] = Convert.ToDouble(dr[5].ToString()); data[3] = dr[5].ToString();
                row["UM"] = dr[6].ToString(); data[6] = dr[6].ToString();
                row["Tip_discount"] = dr[7].ToString();
                row["Discount"] = Convert.ToDouble(dr[8].ToString());

                stoc.Rows.Add(row);
                dataGridView1.Rows.Add(data);
            }

            dr.Close(); query.Dispose();
        }

        void load_clienti()
        {
            SqlCommand query = new SqlCommand("SELECT Nume FROM Clienti ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            while (dr.Read())
                clienti_combobox.Items.Add(dr[0].ToString());
        }

        void add_to_facturi()
        {
            string produse = "", preturi_de_vanzare = "", discounts = "", ums = "", cantitati = "";
            double valoare_totala = 0;

            foreach (DataRow row1 in factura.Rows)
            {
                produse += row1["Nume_produs"].ToString() + ";";
                preturi_de_vanzare += row1["Pret_vanzare"].ToString() + ";";
                ums += row1["Unitate_de_masura"].ToString() + ";";
                cantitati += row1["Cantitate"].ToString() + ";";

                if (row1["Tip_discount_produs"].ToString() == "Procentual")
                {
                    valoare_totala += (Convert.ToDouble(row1["Pret_vanzare"].ToString()) - Convert.ToDouble(row1["Pret_vanzare"].ToString()) * (Convert.ToDouble(row1["Discount_produs"].ToString())) / 100) * Convert.ToDouble(row1["Cantitate"].ToString());
                    discounts += ((Convert.ToDouble(row1["Discount_produs"].ToString()) / 100) * Convert.ToDouble(row1["Pret_vanzare"].ToString()) * Convert.ToDouble(row1["Cantitate"].ToString())).ToString() + ";";
                }
                else
                {
                    valoare_totala += (Convert.ToDouble(row1["Pret_vanzare"]) - Convert.ToDouble(row1["Discount_produs"].ToString())) * Convert.ToDouble(row1["Cantitate"].ToString());
                    discounts += row1["Discount_produs"].ToString() + ";";
                }
            }

            SqlCommand query = new SqlCommand("SELECT TOP 1 Numar_factura FROM Facturi ORDER BY Numar_factura DESC", conn);
            SqlDataReader dr = query.ExecuteReader();

            if(dr.HasRows)
            {
                dr.Read();
                numar_factura = Convert.ToInt32(dr[0].ToString()) + 1;
            }
            dr.Close(); query.Dispose();

            DateTime data = DateTime.Parse(date_picker.Value.Date.ToString());

            SqlCommand insert = new SqlCommand("INSERT INTO Facturi(Nume_client, Numar_factura, Data, Valoare_totala, Rest_de_plata, Produse, Preturi_de_vanzare, Discounts, UMs, Cantitati, Valuta)  values(@nume_client, @numar_factura, @data, @valoare_totala, @rest_de_plata, @produse, @preturi_de_vanzare, @discounts, @ums, @cantitati, @valuta)", conn);
            insert.Parameters.AddWithValue("@nume_client", clienti_combobox.SelectedItem.ToString()); insert.Parameters.AddWithValue("@numar_factura", numar_factura);
            insert.Parameters.AddWithValue("@data", data); insert.Parameters.AddWithValue("@valoare_totala", valoare_totala); insert.Parameters.AddWithValue("@rest_de_plata", valoare_totala);
            insert.Parameters.AddWithValue("@produse", produse); insert.Parameters.AddWithValue("@preturi_de_vanzare", preturi_de_vanzare); insert.Parameters.AddWithValue("@discounts", discounts);
            insert.Parameters.AddWithValue("@ums", ums); insert.Parameters.AddWithValue("@cantitati", cantitati); insert.Parameters.AddWithValue("@valuta", valuta_combobox.SelectedItem.ToString());

            insert.ExecuteNonQuery();
            insert.Dispose();
        }

        void decrease_from_stoc()
        {
            SqlCommand update = new SqlCommand("UPDATE Stoc SET Cantitate = @cantitate WHERE Nume = @nume and Pret = @pret", conn);
            SqlCommand query = new SqlCommand("SELECT Cantitate FROM Stoc WHERE Nume = @nume_produs and Pret = @pret_produs", conn);
            SqlDataReader dr;
            double cantitate;

            foreach(DataRow row1 in factura.Rows)
            {
                query.Parameters.Clear(); query.Parameters.AddWithValue("@nume_produs", row1["Nume_produs"].ToString()); query.Parameters.AddWithValue("@pret_produs", Convert.ToDouble(row1["Pret_vanzare_initial"].ToString()));
                dr = query.ExecuteReader();
                dr.Read(); cantitate = Convert.ToDouble(dr[0].ToString());
                dr.Close(); query.Dispose();

                update.Parameters.Clear();update.Parameters.AddWithValue("@nume", row1["Nume_produs"].ToString()); 
                update.Parameters.AddWithValue("@pret", Convert.ToDouble(row1["Pret_vanzare_initial"].ToString())); update.Parameters.AddWithValue("@cantitate", cantitate - Convert.ToDouble(row1["Cantitate"].ToString()));
                update.ExecuteNonQuery();

                update.Dispose();
            }
        }

        private void creeaza_Click(object sender, EventArgs e)
        {
            if(clienti_combobox.SelectedIndex == -1)
            {
                MessageBox.Show("Alege un client!");
                return;
            }

            conn.Open();

            add_to_facturi();
            decrease_from_stoc();

            conn.Close();

            Factura factura = new Factura(numar_factura);
            factura.parentForm = this.parentForm;
            factura.Show();

            this.Close();
        }

        private void Creeaza_iesire_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            conn.Open();

            load_stoc();
            load_clienti();

            conn.Close();

            brand_combobox.SelectedIndex = categ_combobox.SelectedIndex = valuta_combobox.SelectedIndex = 0;

            dataGridView1.RowHeadersVisible = dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToResizeRows = dataGridView2.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToAddRows = dataGridView2.AllowUserToAddRows = false;
        }

        bool contains(string search_text, string brand, string categorie, DataRow row1)
        {
            if (!row1["Nume"].ToString().ToLower().Contains(search_text.ToLower())) return false;
            if (!row1["Brand"].ToString().ToLower().Contains(brand.ToLower())) return false;
            if (!row1["Categorie"].ToString().ToLower().Contains(categorie.ToLower())) return false;

            return true;
        }

        void search(string search_text, string brand, string categorie)
        {
            dataGridView1.Rows.Clear();
            string[] data = new string[7];

            foreach(DataRow row1 in stoc.Rows)
                if(contains(search_text, brand, categorie, row1))
                {
                    data[0] = row1["Id"].ToString();
                    data[1] = row1["Nume"].ToString();
                    data[2] = row1["Cantitate"].ToString();
                    data[3] = row1["Pret"].ToString();
                    data[4] = row1["Brand"].ToString();
                    data[5] = row1["Categorie"].ToString();
                    data[6] = row1["UM"].ToString();

                    dataGridView1.Rows.Add(data);
                }
        }

        private void plati_button_Click(object sender, EventArgs e)
        {
            string search_text = "", brand = "", categorie = "";

            if (brand_combobox.SelectedIndex > 0)
                brand = brand_combobox.SelectedItem.ToString();
            if (categ_combobox.SelectedIndex > 0)
                categorie = categ_combobox.SelectedItem.ToString();
            search_text = search_textbox.Text;

            search(search_text, brand, categorie);
        }

        void clear_spaces()
        {
            nume_produs_textbox.Clear();
            pret_vanzare_textbox.Clear();
            cantitate_textbox.Clear();
            tip_discount_combobox.SelectedIndex = -1;
            discount_textbox.Text = "0.00";
        }

        private void adauga_button_Click(object sender, EventArgs e)
        {
            int index;

            if(Convert.ToDouble(discount_textbox.Text) > 100 && tip_discount_combobox.SelectedIndex == 1)
            {
                MessageBox.Show("Discount mai mare decat pretul produsului!");
                return;
            }
            else if(tip_discount_combobox.SelectedIndex == 2 && Convert.ToDouble(discount_textbox.Text) > Convert.ToDouble(pret_vanzare_textbox.Text))
            {
                MessageBox.Show("Discount mai mare decat pretul produsului!");
                return;
            }

            if (cantitate_textbox.Text == "" || nume_produs_textbox.Text == "")
            {
                MessageBox.Show("Date insuficiente!");
                return;
            }

            DataRow row2 = stoc.Rows[Convert.ToInt32(dataGridView1[0, product_index].Value.ToString())];

            if (Convert.ToDouble(cantitate_textbox.Text) > Convert.ToDouble(row2["Cantitate"].ToString()))
            {
                MessageBox.Show("Cantitatea acestui produs este mai mare decat cea din stoc!");
                return;
            }

            foreach (DataRow row1 in factura.Rows)
            {
                if (row1["Nume_produs"].ToString() == nume_produs_textbox.Text && row1["Pret_vanzare"].ToString() == pret_vanzare_textbox.Text && row1["Tip_discount_produs"].ToString() == tip_discount_combobox.SelectedItem.ToString() && row1["Discount_produs"].ToString() == discount_textbox.Text)
                {
                    index = factura.Rows.IndexOf(row1);
                    row1["Cantitate"] = Convert.ToDouble(row1["Cantitate"].ToString()) + Convert.ToDouble(cantitate_textbox.Text);
                    dataGridView2[1, index].Value = Convert.ToDouble(row1["Cantitate"].ToString());
                    return;
                }
            }

            row["Nume_produs"] = nume_produs_textbox.Text;
            row["Pret_vanzare"] = Convert.ToDouble(pret_vanzare_textbox.Text);
            row["Cantitate"] = Math.Round(Convert.ToDouble(cantitate_textbox.Text), 2);
            row["Tip_discount_produs"] = tip_discount_combobox.SelectedItem.ToString();
            row["Discount_produs"] = Math.Round(Convert.ToDouble(discount_textbox.Text), 2);

            factura.Rows.Add(row);

            index = Convert.ToInt32(dataGridView1[0, product_index].Value.ToString());
            row2["Cantitate"] = Math.Round(Convert.ToDouble(row2["Cantitate"].ToString()) - Convert.ToDouble(cantitate_textbox.Text), 2);
            dataGridView1[2, product_index].Value = row2["Cantitate"].ToString();

            if (Convert.ToDouble(row2["Cantitate"].ToString()) == 0)
            {
                stoc.Rows.RemoveAt(index);
                dataGridView1.Rows.RemoveAt(product_index);
            }

            string[] data = new string[4];
            data[0] = nume_produs_textbox.Text;
            data[1] = cantitate_textbox.Text;
            data[2] = pret_vanzare_textbox.Text;
            data[3] = discount_textbox.Text;
            if (tip_discount_combobox.SelectedIndex == 0)
                data[3] += " %";
            dataGridView2.Rows.Add(data);

            clear_spaces();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            product_index = e.RowIndex;
            row = factura.NewRow();
            nume_produs_textbox.Text = dataGridView1[1, e.RowIndex].Value.ToString();

            DataRow row1 = stoc.Rows[Convert.ToInt32(dataGridView1[0, e.RowIndex].Value.ToString())];
            if (row1["Tip_discount"].ToString() == "Procentual")
                tip_discount_combobox.SelectedIndex = 0;
            else
                tip_discount_combobox.SelectedIndex = 1;
            pret_vanzare_textbox.Text = row1["Pret"].ToString();

            row["Pret_vanzare_initial"] = Convert.ToInt32(row1["Pret"].ToString());
            row["Unitate_de_masura"] = dataGridView1[6, e.RowIndex].Value.ToString();

            discount_textbox.Text = Math.Round(Convert.ToDouble(row1["Discount"].ToString()), 2).ToString();
        }

        private void Creeaza_iesire_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }
    }
}
