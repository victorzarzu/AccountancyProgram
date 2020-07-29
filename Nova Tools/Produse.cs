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

namespace Nova_Tools
{
    public partial class Produse : Form
    {
        public Form parentForm { set; get; }

        SqlConnection conn;
        DataTable produse;
        DataColumn col;
        DataRow row;
        public Produse()
        {
            InitializeComponent();

            produse = new DataTable();

            col = new DataColumn("Nume_furnizor");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            col = new DataColumn("Nume_produs");
            col.DataType = System.Type.GetType("System.String");
            produse.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.Double");
            produse.Columns.Add(col);

            col = new DataColumn("Pret_intrare");
            col.DataType = System.Type.GetType("System.Double");
            produse.Columns.Add(col);

        }

        void load_produse_datagridview_and_furnizori()
        {
            SqlCommand query = new SqlCommand("SELECT * FROM Intrari ORDER BY Nume_produs ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            string[] data = new string[4];
            while (dr.Read())
            {
                row = produse.NewRow();

                row["Nume_produs"] = data[0] = dr[2].ToString();
                row["Nume_furnizor"] = data[1] = dr[1].ToString();
                row["Cantitate"] = Convert.ToDouble(dr[3].ToString()); data[2] = dr[3].ToString();
                row["Pret_intrare"] = Convert.ToDouble(dr[4].ToString()); data[3] = dr[4].ToString();

                produse.Rows.Add(row);
                dataGridView1.Rows.Add(data);
            }

            query.Dispose(); dr.Close();

            dataGridView1.AllowUserToAddRows = dataGridView1.AllowUserToResizeColumns = dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        void load_furnizori()
        {
            SqlCommand query = new SqlCommand("SELECT Nume FROM Furnizori ORDER BY Nume ASC", conn);
            SqlDataReader dr = query.ExecuteReader();

            while (dr.Read())
                furnizori_combobox.Items.Add(dr[0].ToString());

            dr.Close(); query.Dispose();

            furnizori_combobox.SelectedIndex = 0;
        }

        private void Produse_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            conn.Open();

            load_furnizori();
            load_produse_datagridview_and_furnizori();

            conn.Close();
        }

        bool contains(string produs, string furnizor, DataRow row1)
        {
            if (!row1["Nume_furnizor"].ToString().ToLower().Contains(furnizor.ToLower())) return false;
            if (!row1["Nume_produs"].ToString().ToLower().Contains(produs.ToLower())) return false;

            return true;
        }
        void search(string produs, string furnizor)
        {
            dataGridView1.Rows.Clear();

            string[] data = new string[4];
            foreach(DataRow row1 in produse.Rows)
                if(contains(produs, furnizor, row1))
                {
                    data[0] = row1["Nume_produs"].ToString();
                    data[1] = row1["Nume_furnizor"].ToString(); ;
                    data[2] = row1["Cantitate"].ToString();
                    data[3] = row1["Pret_intrare"].ToString();

                    dataGridView1.Rows.Add(data);
                }
        }

        private void creeaza_Click(object sender, EventArgs e)
        {
            string produs = produs_textbox.Text, furnizor = "";
            if (furnizori_combobox.SelectedIndex > 0)
                furnizor = furnizori_combobox.SelectedItem.ToString();

            search(produs, furnizor);
        }

        private void Produse_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }
    }
}
