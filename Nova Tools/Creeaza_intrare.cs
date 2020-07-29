using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Nova_Tools
{
    public partial class Creeaza_intrare : Form
    {
        public Form parentForm { set; get; }

        SqlConnection conn;
        DataTable produse;
        DataColumn col;
        DataRow row;
        DataTable nir;
        DataTable niruri;
        int product_index;
        int start_numar_receptie = 0, numar_de_receptie = 0;
        double tva = 1.19f;
        public Creeaza_intrare()
        {
            InitializeComponent();

            produse = new DataTable();
            nir = new DataTable();
            niruri = new DataTable();

            //Produse

            col = new DataColumn("Id");
            col.DataType = System.Type.GetType("System.Int32");
            produse.Columns.Add(col);

            col = new DataColumn("Nume");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            col = new DataColumn("Cod");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            col = new DataColumn("Brand");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            col = new DataColumn("Categorie");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            col = new DataColumn("UM");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            //Nir

            col = new DataColumn("Nume_produs");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            col = new DataColumn("Pret_intrare");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("Valoare_TVA");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("Pret_iesire");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("Unitate_de_masura");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            col = new DataColumn("Brand_nir");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            col = new DataColumn("Categorie_nir");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            //Niruri

            col = new DataColumn("Nume_furnizor");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);

            col = new DataColumn("Numar_factura");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);

            col = new DataColumn("Data");
            col.DataType = System.Type.GetType("System.DateTime");
            niruri.Columns.Add(col);

            col = new DataColumn("Numar_de_receptie");
            col.DataType = System.Type.GetType("System.String");
            niruri.Columns.Add(col);
        }

        void load_niruri()
        {
            SqlCommand query = new SqlCommand("SELECT Nume_furnizor, Numar_factura, Data, Numar_de_receptie FROM Niruri", conn);
            SqlDataReader dr = query.ExecuteReader();

            while(dr.Read())
            {
                row = niruri.NewRow();
                
                row["Nume_furnizor"] = dr[0].ToString();
                row["Numar_factura"] = dr[1].ToString();
                row["Data"] = DateTime.Parse(dr[2].ToString());
                row["Numar_de_receptie"] = dr[3].ToString();

                niruri.Rows.Add(row);
            }

            dr.Close(); query.Dispose();
        }

        void sort_datatable(DataTable dt, string sort)
        {
            dt.DefaultView.Sort = sort;
            dt = dt.DefaultView.ToTable();
        }

        void fill_datagridview()
        {
            string[] data = new string[6];
            foreach(DataRow row1 in produse.Rows)
            {
                data[0] = row1["Id"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Cod"].ToString();
                data[3] = row1["Brand"].ToString();
                data[4] = row1["Categorie"].ToString();
                data[5] = row1["UM"].ToString();

                dataGridView1.Rows.Add(data);
            }
        }

        void fill_comboboxes()
        {
            sort_datatable(produse, "Brand ASC");
            foreach (DataRow row1 in produse.Rows)
                if (!brand_combobox.Items.Contains(row1["Brand"].ToString()))
                    brand_combobox.Items.Add(row1["Brand"].ToString());
            sort_datatable(produse, "Categorie ASC");
            foreach (DataRow row1 in produse.Rows)
                if (!categ_combobox.Items.Contains(row1["Categorie"].ToString()))
                    categ_combobox.Items.Add(row1["Categorie"].ToString());
        }
        
        void load_furnizori()
         {
            SqlCommand query = new SqlCommand("SELECT Nume FROM Furnizori ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            while (dr.Read())
                furnizori_combobox.Items.Add(dr[0].ToString());
            dr.Close(); query.Dispose();
        }

        private void Creeaza_intrare_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            date_picker.Format = DateTimePickerFormat.Custom;
            date_picker.CustomFormat = "dd/MM/yyyy";

            conn.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM Produse", conn);
            SqlDataReader dr = query.ExecuteReader();

            while(dr.Read())
            {
                row = produse.NewRow();

                row["Id"] = Convert.ToInt32(dr[0].ToString());
                row["Nume"] = dr[1].ToString();
                row["Cod"] = dr[2].ToString();
                row["Brand"] = dr[3].ToString();
                row["Categorie"] = dr[4].ToString();
                row["UM"] = dr[5].ToString();

                produse.Rows.Add(row);
            }

            dr.Close(); query.Dispose();

            sort_datatable(produse, "Nume ASC");
            fill_datagridview();
            fill_comboboxes();
            sort_datatable(produse, "Nume ASC");
            load_furnizori();
            load_niruri();

            conn.Close();

            brand_combobox.SelectedIndex = categ_combobox.SelectedIndex = 0;

            dataGridView1.RowHeadersVisible = dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dataGridView1.AllowUserToResizeRows = dataGridView2.AllowUserToResizeRows = false;

        }

        void update_data()
        {
            string[] data = new string[5];
            dataGridView2.Rows.Clear();
            foreach (DataRow row1 in nir.Rows)
            {
                data[0] = row1["Nume_produs"].ToString(); data[1] = row1["Pret_intrare"].ToString();
                data[2] = row1["Valoare_TVA"].ToString(); data[3] = row1["Pret_iesire"].ToString();
                data[4] = row1["Cantitate"].ToString();

                dataGridView2.Rows.Add(data);
            }
        }

        private void adauga_button_Click(object sender, EventArgs e)
        {
            if(nume_produs_textbox.Text == "" || pret_iesire_textbox.Text == "" || pret_intrare_textbox.Text == "" || cantitate_textbox.Text == "" || tva_textbox.Text == "")
            {
                MessageBox.Show("Date insuficiente!");
                return;
            }

            row = nir.NewRow();
            
            bool found = false;

            foreach (DataRow row1 in nir.Rows)
                if (row1["Nume_produs"].ToString() == nume_produs_textbox.Text && pret_intrare_textbox.Text == row1["Pret_intrare"].ToString() && pret_iesire_textbox.Text == row1["Pret_iesire"].ToString())
                {
                    row1["Cantitate"] = Convert.ToDouble(row1["Cantitate"].ToString()) + Convert.ToDouble(cantitate_textbox.Text);
                    found = true;
                }

            row["Nume_produs"] = nume_produs_textbox.Text;
            row["Pret_intrare"] = Convert.ToDouble(pret_intrare_textbox.Text);
            row["Valoare_TVA"] = Convert.ToDouble(tva_textbox.Text);
            row["Pret_iesire"] = Convert.ToDouble(pret_iesire_textbox.Text);
            row["Cantitate"] = Convert.ToDouble(cantitate_textbox.Text);
            row["Unitate_de_masura"] = dataGridView1[5, product_index].Value.ToString();
            row["Brand_nir"] = dataGridView1[3, product_index].Value.ToString();
            row["Categorie_nir"] = dataGridView1[4, product_index].Value.ToString();

            if(!found)
                nir.Rows.Add(row);
            update_data();

            nume_produs_textbox.Clear(); pret_iesire_textbox.Clear(); pret_intrare_textbox.Clear(); cantitate_textbox.Clear(); tva_textbox.Text = "19.00";

        }

        private void creeaza_button_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            search_textbox.Text = "";
            Produs_nou produs_nou = new Produs_nou(produse);
            produs_nou.parentForm = this;
            this.Hide();
            produs_nou.Show();
        }

        bool contains(string search_text, string brand, string categorie, DataRow row1)
        {
            return ((row1["Nume"].ToString().ToLower().Contains(search_text.ToLower()) && row1["Brand"].ToString().ToLower().Contains(brand.ToLower()) && row1["Categorie"].ToString().ToLower().Contains(categorie.ToLower()))
                ||
                (row1["Cod"].ToString().ToLower().Contains(search_text.ToLower()) && row1["Brand"].ToString().ToLower().Contains(brand.ToLower()) && row1["Categorie"].ToString().ToLower().Contains(categorie.ToLower())));
        
        }

        void add_to_datagridview(DataRow row1)
        {
            string[] data = new string[6];

            data[0] = row1["Id"].ToString();
            data[1] = row1["Nume"].ToString();
            data[2] = row1["Cod"].ToString();
            data[3] = row1["Brand"].ToString();
            data[4] = row1["Categorie"].ToString();
            data[5] = row1["UM"].ToString();

            dataGridView1.Rows.Add(data);
        }

        void search_datagridview(string search_text, string brand, string categorie)
        {
            foreach (DataRow row1 in produse.Rows)
                if (contains(search_text, brand, categorie, row1))
                    add_to_datagridview(row1);
        }

        private void plati_button_Click(object sender, EventArgs e)
        {
            string search_text, brand = "", categorie = "";
            if (brand_combobox.SelectedIndex > 0)
                brand = brand_combobox.SelectedItem.ToString();
            if (categ_combobox.SelectedIndex > 0)
                categorie = categ_combobox.SelectedItem.ToString();
            search_text = search_textbox.Text;


            dataGridView1.Rows.Clear();

            search_datagridview(search_text, brand, categorie);
        }

        private void Creeaza_intrare_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            product_index = e.RowIndex;

            nume_produs_textbox.Text = dataGridView1[1, product_index].Value.ToString() + " " + dataGridView1[2, product_index].Value.ToString();
        }

        void add_products_to_stoc()
        {
            foreach (DataRow row1 in nir.Rows)
            {
                SqlCommand query = new SqlCommand("SELECT * FROM Stoc WHERE Nume = @nume and Pret = @pret", conn);
                query.Parameters.Clear();
                query.Parameters.AddWithValue("@nume", row1["Nume_produs"].ToString()); query.Parameters.AddWithValue("@pret", Convert.ToDouble(row1["Pret_iesire"].ToString()));
                SqlDataReader dr = query.ExecuteReader();
                bool exist = dr.HasRows;
                double cantitate = Convert.ToDouble(row1["Cantitate"].ToString());
                if (exist)
                {
                    dr.Read();
                    cantitate += Convert.ToDouble(dr[4].ToString());
                }
                dr.Close(); query.Dispose();

                if (exist)
                {
                    SqlCommand update = new SqlCommand("UPDATE Stoc SET Cantitate = @cantitate WHERE Nume = @nume and Pret = @pret", conn);
                    update.Parameters.Clear();
                    update.Parameters.AddWithValue("@cantitate", cantitate); update.Parameters.AddWithValue("@nume", row1["Nume_produs"].ToString());
                    update.Parameters.AddWithValue("@pret", Convert.ToDouble(row1["Pret_iesire"].ToString()));

                    update.ExecuteNonQuery();
                    update.Dispose();

                    continue;
                }

                SqlCommand insert = new SqlCommand("INSERT INTO Stoc(Nume, Categorie, Brand, Cantitate, Pret, UM, Tip_discount, Discount) values(@nume, @categorie, @brand, @cantitate, @pret, @um, @tip_discount, @discount)", conn);
                insert.Parameters.Clear();
                insert.Parameters.AddWithValue("@nume", row1["Nume_produs"].ToString()); insert.Parameters.AddWithValue("@categorie", row1["Categorie_nir"].ToString());
                insert.Parameters.AddWithValue("@brand", row1["Brand_nir"].ToString());
                insert.Parameters.AddWithValue("@cantitate", cantitate); insert.Parameters.AddWithValue("@pret", Convert.ToDouble(row1["Pret_iesire"].ToString()));
                insert.Parameters.AddWithValue("@um", row1["Unitate_de_masura"].ToString()); insert.Parameters.AddWithValue("@discount", 0);
                insert.Parameters.AddWithValue("@tip_discount", "Procentual");

                insert.ExecuteNonQuery();
                insert.Dispose();
            }
        }

        void add_nir()
        {
            DateTime data = date_picker.Value.Date;
            DateTime minim_date = DateTime.Now, maxim_date = DateTime.Parse("1/1/1900");

            int minim_difference = 999999, diff, maxim_numar_receptie = 0;
            bool found = false;
            double val_total = 0;

            foreach (DataRow row1 in niruri.Rows)
            {
                DateTime dt = DateTime.Parse(row1["Data"].ToString());
                if (DateTime.Compare(dt, data) <= 0)
                {
                    found = true;
                    diff = (data - dt).Days;
                    if (diff < minim_difference)
                    {
                        minim_difference = diff;
                        numar_de_receptie = Convert.ToInt32(row1["Numar_de_receptie"].ToString()) + 1;
                    }
                    else if (diff == minim_difference && Convert.ToInt32(row1["Numar_de_receptie"].ToString()) + 1 > numar_de_receptie)
                        numar_de_receptie = Convert.ToInt32(row1["Numar_de_receptie"].ToString()) + 1;
                }
                if (Convert.ToInt32(row1["Numar_de_receptie"].ToString()) > maxim_numar_receptie)
                    maxim_numar_receptie = Convert.ToInt32(row1["Numar_de_receptie"].ToString());
                if (DateTime.Compare(dt, minim_date) < 0)
                    minim_date = dt;
                if (DateTime.Compare(maxim_date, dt) < 0)
                    maxim_date = dt;
            }
            if (!found && DateTime.Compare(maxim_date, data) <= 0)
                numar_de_receptie = maxim_numar_receptie + 1;
            else if (!found && DateTime.Compare(data, minim_date) <= 0)
                numar_de_receptie = start_numar_receptie + 1;

            foreach (DataRow row1 in niruri.Rows)
            {
                DateTime dt = DateTime.Parse(row1["Data"].ToString());
                if (DateTime.Compare(dt, data) > 0)
                    row1["Numar_de_receptie"] = Convert.ToInt32(row1["Numar_de_receptie"].ToString()) + 1;
            }

            string produse = "", preturi_intrare = "", valori_tva = "", preturi_iesire = "", cantitati = "", ums = "";

            foreach (DataRow row1 in nir.Rows)
            {
                produse += row1["Nume_produs"].ToString() + ";";
                preturi_intrare += row1["Pret_intrare"].ToString() + ";";
                valori_tva += row1["Valoare_TVA"].ToString() + ";";
                preturi_iesire += row1["Pret_iesire"].ToString() + ";";
                cantitati += row1["Cantitate"].ToString() + ";";
                ums += row1["Unitate_de_masura"].ToString() + ";";

                val_total += Convert.ToDouble(row1["Pret_intrare"].ToString()) * tva * Convert.ToDouble(row1["Cantitate"].ToString());
            }

            val_total = Math.Round(val_total, 4);

            conn.Open();

            SqlCommand insert = new SqlCommand("INSERT INTO Niruri(Nume_furnizor, Numar_factura, Data, Valoare_totala, Rest_de_plata, Numar_de_receptie, Produse, Preturi_intrare_fara_TVA, Valori_TVA, Preturi_de_iesire, UMs, Cantitati)" +
                "values(@nume_furnizor, @numar_factura, @data, @val_total, @rest_plata, @numar_receptie, @produse, @preturi_intrare, @valori_tva, @preturi_iesire, @ums, @cantitati)", conn);
            insert.Parameters.AddWithValue("@nume_furnizor", furnizori_combobox.SelectedItem.ToString()); insert.Parameters.AddWithValue("@numar_factura", numar_factura_textbox.Text);
            insert.Parameters.AddWithValue("@data", data); insert.Parameters.AddWithValue("@val_total", val_total); insert.Parameters.AddWithValue("@rest_plata", val_total);
            insert.Parameters.AddWithValue("@numar_receptie", numar_de_receptie); insert.Parameters.AddWithValue("@produse", produse); insert.Parameters.AddWithValue("@preturi_intrare", preturi_intrare);
            insert.Parameters.AddWithValue("@valori_tva", valori_tva); insert.Parameters.AddWithValue("@preturi_iesire", preturi_iesire); insert.Parameters.AddWithValue("@ums", ums);
            insert.Parameters.AddWithValue("@cantitati", cantitati);

            insert.ExecuteNonQuery();
            insert.Dispose();

            SqlCommand update = new SqlCommand("UPDATE Niruri SET Numar_de_receptie = @numar_de_receptie WHERE Nume_furnizor = @nume_furnizor and Numar_factura = @numar_factura", conn);
            foreach (DataRow row1 in niruri.Rows)
            {
                update.Parameters.Clear();
                update.Parameters.AddWithValue("@numar_de_receptie", Convert.ToInt32(row1["Numar_de_receptie"].ToString()));
                update.Parameters.AddWithValue("@nume_furnizor", row1["Nume_furnizor"].ToString());
                update.Parameters.AddWithValue("@numar_factura", Convert.ToInt32(row1["Numar_factura"].ToString()));
                update.ExecuteNonQuery();
            }

            update.Dispose();
        }

        void add_to_intrari()
        {
            double cantitate = 0;
            foreach(DataRow row1 in nir.Rows)
            {
                cantitate = Convert.ToDouble(row1["Cantitate"].ToString());
                SqlCommand query = new SqlCommand("SELECT Cantitate FROM Intrari WHERE Nume_furnizor = @nume_furnizor and Nume_produs = @nume_produs and Pret_intrare = @pret_intrare", conn);
                query.Parameters.Clear();
                query.Parameters.AddWithValue("@nume_furnizor", furnizori_combobox.SelectedItem.ToString()); query.Parameters.AddWithValue("@nume_produs", row1["Nume_produs"].ToString());
                query.Parameters.AddWithValue("@pret_intrare", Math.Round(Convert.ToDouble(row1["Pret_intrare"].ToString()), 2));

                SqlDataReader dr = query.ExecuteReader();
                bool exist = dr.HasRows;
                if (exist)
                {
                    dr.Read();
                    cantitate += Convert.ToDouble(dr[0].ToString());
                }
                dr.Close(); query.Dispose();

                if(exist)
                {
                    SqlCommand update = new SqlCommand("UPDATE Intrari SET Cantitate = @cantitate WHERE Nume_furnizor = @nume_furnizor and Nume_produs = @nume_produs and Pret_intrare = @pret_intrare", conn);
                    update.Parameters.Clear();
                    update.Parameters.AddWithValue("@cantitate", cantitate);  update.Parameters.AddWithValue("@nume_furnizor", furnizori_combobox.SelectedItem.ToString()); 
                    update.Parameters.AddWithValue("@nume_produs", row1["Nume_produs"].ToString()); update.Parameters.AddWithValue("@pret_intrare", Math.Round(Convert.ToDouble(row1["Pret_intrare"].ToString()), 2));

                    update.ExecuteNonQuery();
                    update.Dispose();

                    continue;
                }

                SqlCommand insert = new SqlCommand("INSERT INTO Intrari(Nume_furnizor, Nume_produs, Cantitate, Pret_intrare) values(@nume_furnizor, @nume_produs, @cantitate, @pret_intrare)", conn);
                insert.Parameters.Clear();
                insert.Parameters.AddWithValue("@cantitate", cantitate); insert.Parameters.AddWithValue("@nume_furnizor", furnizori_combobox.SelectedItem.ToString());
                insert.Parameters.AddWithValue("@nume_produs", row1["Nume_produs"].ToString()); insert.Parameters.AddWithValue("@pret_intrare", Math.Round(Convert.ToDouble(row1["Pret_intrare"].ToString()), 2));

                insert.ExecuteNonQuery();
                insert.Dispose();
            }
        }

        private void creeaza_Click(object sender, EventArgs e)
        {
            if(nir.Rows.Count == 0)
            {
                MessageBox.Show("Nu exista produse!");
                return;
            }

            if(furnizori_combobox.SelectedIndex == -1 || numar_factura_textbox.Text == "")
            {
                MessageBox.Show("Date insuficiente");
                return;
            }

            add_nir();
            add_products_to_stoc();
            add_to_intrari();

            conn.Close();

            this.Close();
            parentForm.Hide();

            Nir nir_form = new Nir(numar_de_receptie);
            nir_form.parentForm = this.parentForm;
            nir_form.Show();
        }
    }
}
