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

namespace Nova_Tools
{
    public partial class Clienti : Form
    {
        SqlConnection conn;
        DataTable clienti;
        DataColumn col;
        DataRow row;
        string connection_string;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 7)
            {
                clienti.Rows[e.RowIndex]["Nume"] = dataGridView1[1, e.RowIndex].Value.ToString();
                clienti.Rows[e.RowIndex]["Adresa"] = dataGridView1[2, e.RowIndex].Value.ToString();
                clienti.Rows[e.RowIndex]["CUI"] = dataGridView1[3, e.RowIndex].ToString();
                clienti.Rows[e.RowIndex]["Numar_de_inregistrare"] = dataGridView1[4, e.RowIndex].Value.ToString();
                clienti.Rows[e.RowIndex]["Banca"] = dataGridView1[5, e.RowIndex].Value == null ? "" : dataGridView1[4, e.RowIndex].Value.ToString();
                clienti.Rows[e.RowIndex]["Cont_bancar"] = dataGridView1[6, e.RowIndex].Value == null ? "" : dataGridView1[5, e.RowIndex].Value.ToString();

                conn.Open();

                SqlCommand upate = new SqlCommand("UPDATE Clienti SET Nume = @nume, Adresa = @adresa, CUI = @cui, Numar_de_inregistrare = @numar_inregistrare, Banca = @banca, Cont_bancar = @cont_bancar WHERE Id = @id", conn);
                upate.Parameters.AddWithValue("@id", Convert.ToInt32(dataGridView1[0, e.RowIndex].Value.ToString()));
                upate.Parameters.AddWithValue("@nume", dataGridView1[1, e.RowIndex].Value.ToString());
                upate.Parameters.AddWithValue("@adresa", dataGridView1[2, e.RowIndex].Value.ToString());
                upate.Parameters.AddWithValue("@cui", dataGridView1[3, e.RowIndex].Value.ToString());
                upate.Parameters.AddWithValue("@numar_inregistrare", dataGridView1[4, e.RowIndex].Value.ToString());
                upate.Parameters.AddWithValue("@banca", dataGridView1[5, e.RowIndex].Value == null ? "" : dataGridView1[4, e.RowIndex].Value.ToString());
                upate.Parameters.AddWithValue("@cont_bancar", dataGridView1[6, e.RowIndex].Value == null ? "" : dataGridView1[5, e.RowIndex].Value.ToString());

                upate.ExecuteNonQuery();

                conn.Close();

                dataGridView1.Rows.Clear();

                sort_clienti();
                fill_datagridview();
            }
        }

        void sort_clienti()
        {
            clienti.DefaultView.Sort = "Nume ASC";
            clienti = clienti.DefaultView.ToTable();
        }

        public Clienti()
        {
            InitializeComponent();

            DataGridViewButtonColumn btncolumn = new DataGridViewButtonColumn();
            btncolumn.HeaderText = "Editeza";
            btncolumn.Name = "Modifica";
            btncolumn.Text = "Modfifica";
            btncolumn.UseColumnTextForButtonValue = true;

            dataGridView1.Columns.Add(btncolumn);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);

            dataGridView1.AllowUserToAddRows = false;

            clienti = new DataTable();
            col = new DataColumn();

            col = new DataColumn("Index");
            col.DataType = System.Type.GetType("System.Int32");
            clienti.Columns.Add(col);

            col = new DataColumn("Nume");
            col.DataType = System.Type.GetType("System.String");
            clienti.Columns.Add(col);

            col = new DataColumn("Adresa");
            col.DataType = System.Type.GetType("System.String");
            clienti.Columns.Add(col);

            col = new DataColumn("CUI");
            col.DataType = System.Type.GetType("System.String");
            clienti.Columns.Add(col);

            col = new DataColumn("Numar_de_inregistrare");
            col.DataType = System.Type.GetType("System.String");
            clienti.Columns.Add(col);

            col = new DataColumn("Banca");
            col.DataType = System.Type.GetType("System.String");
            clienti.Columns.Add(col);

            col = new DataColumn("Cont_bancar");
            col.DataType = System.Type.GetType("System.String");
            clienti.Columns.Add(col);
        }

        private void DataGridView1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void fill_datagridview()
        {
            string[] data = new string[7];
            foreach (DataRow row1 in clienti.Rows)
            {
                data[0] = row1["Index"].ToString();
                data[1] = row1["Nume"].ToString();
                data[2] = row1["Adresa"].ToString();
                data[3] = row1["CUI"].ToString();
                data[4] = row1["Numar_de_inregistrare"].ToString();
                data[5] = row1["Banca"].ToString();
                data[6] = row1["Cont_bancar"].ToString();

                dataGridView1.Rows.Add(data);
            }
        }

        private void Clienti_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"\SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            conn.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM Clienti", conn);
            SqlDataReader dr = query.ExecuteReader();
 
            while(dr.Read())
            {
                row = clienti.NewRow();
                row["Index"] = Convert.ToInt32(dr[0].ToString());
                row["Nume"] = dr[1].ToString();
                row["Adresa"] = dr[2].ToString();
                row["CUI"] = dr[3].ToString();
                row["Numar_de_inregistrare"] = dr[4].ToString();
                row["Banca"] = dr[5].ToString();
                row["Cont_bancar"] = dr[6].ToString();

                clienti.Rows.Add(row);
            }
            dr.Close(); query.Dispose();

            sort_clienti();
            fill_datagridview();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            conn.Close();

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
        }

        private void client_nou_button_Click(object sender, EventArgs e)
        {
            Client_nou nou = new Client_nou(clienti);
            nou.parentForm = this;
            this.Hide();
            nou.Show();
        }

        bool contains(string text, DataRow row1)
        {
            if (row1["Nume"].ToString().ToLower().Contains(text.ToLower())) return true;
            if (row1["Adresa"].ToString().ToLower().Contains(text.ToLower())) return true;
            if (row1["CUI"].ToString().ToLower().Contains(text.ToLower())) return true;
            if (row1["Banca"].ToString().ToLower().Contains(text.ToLower())) return true;
 
            return false;
        }

        void search()
        {
            dataGridView1.Rows.Clear();
            string[] data = new string[7];
            string search_text = search_textbox.Text;
            if(search_textbox.Text == "")
                fill_datagridview();
            else
            {
                foreach(DataRow row1 in clienti.Rows)
                    if(contains(search_text, row1))
                    {
                        data[0] = row1["Index"].ToString();
                        data[1] = row1["Nume"].ToString();
                        data[2] = row1["Adresa"].ToString();
                        data[3] = row1["CUI"].ToString();
                        data[4] = row1["Numar_de_inregistrare"].ToString();
                        data[5] = row1["Banca"].ToString();
                        data[6] = row1["Cont_bancar"].ToString();

                        dataGridView1.Rows.Add(data);
                    }
            }
        }
        private void cauta_button_Click(object sender, EventArgs e)
        {
            search();
        }

        private void search_textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(Keys.Enter))
                search();
        }
    }
}
