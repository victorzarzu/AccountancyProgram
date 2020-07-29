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
using System.Text.RegularExpressions;

namespace Nova_Tools
{
    public partial class Nir : Form
    {
        public Form parentForm { get; set; }

        int numar_de_receptie;
        SqlConnection conn;
        string nume_furnizor, numar_factura, data, produse, preturi_intrare_fara_tva, valori_tva, preturi_de_iesire, ums, cantitati;
        DataTable nir;
        string firma, cui, numar_inregistrare;

        private void cauta_button_Click(object sender, EventArgs e)
        {
            printPreviewDialog1 = new PrintPreviewDialog();
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.Show();
        }


        DataColumn col;
        DataRow row;

        private void Nir_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Show();
        }

        double valoare_totala;
        
        public Nir(int numar)
        {
            InitializeComponent();

            numar_de_receptie = numar;
        }

        void load()
        {
            conn.Open();

            SqlCommand query = new SqlCommand("SELECT * FROM Niruri WHERE Numar_de_receptie = @numar_de_receptie", conn);
            query.Parameters.AddWithValue("@numar_de_receptie", numar_de_receptie);
            SqlDataReader dr = query.ExecuteReader();
            dr.Read();
            nume_furnizor = dr[0].ToString();
            numar_factura = dr[1].ToString();
            data = DateTime.Parse(dr[2].ToString()).ToShortDateString();
            valoare_totala = Convert.ToDouble(dr[3].ToString());
            produse = dr[6].ToString();
            preturi_intrare_fara_tva = dr[7].ToString();
            valori_tva = dr[8].ToString();
            preturi_de_iesire = dr[9].ToString();
            ums = dr[10].ToString();
            cantitati = dr[11].ToString();

            furnizor_label.Text = nume_furnizor;
            numar_factura_label.Text = numar_factura;
            DateTime dat = DateTime.Parse(data);
            string days = dat.Day.ToString(), month = dat.Month.ToString(), year = dat.Year.ToString();
            if (days.Length == 1)
                days = "0" + days;
            if (month.Length == 1)
                month = "0" + month;
            string date = days + "." + month + "." + year;
            data_label.Text = date;

            furnizor_label.ForeColor = numar_factura_label.ForeColor = data_label.ForeColor = Color.LightSlateGray;

            dr.Close(); query.Dispose();

            query = new SqlCommand("SELECT Nume, CUI, Numar_de_inregistrare FROM Firma", conn);
            dr = query.ExecuteReader();
            dr.Read();
            firma = dr[0].ToString(); cui = dr[1].ToString(); numar_inregistrare = dr[2].ToString();

            conn.Close();
        }

        void add_to_datagridview()
        {
            string[] data = new string[5];
            string[] produse_split = produse.Split(';');
            string[] preturi_intrare_fara_tva_split = preturi_intrare_fara_tva.Split(';');
            string[] valori_tva_split = valori_tva.Split(';');
            string[] preturi_de_iesire_split = preturi_de_iesire.Split(';');
            string[] ums_split = ums.Split(';');
            string[] cantitati_split = cantitati.Split(';');

            for(int i = 0;i < produse_split.Length - 1;++i)
            {
                row = nir.NewRow();

                row["Nume_produs"] = produse_split[i];
                row["Pret_intrare_fara_TVA"] = Convert.ToDouble(preturi_intrare_fara_tva_split[i]);
                row["Valori_TVA"] = Convert.ToDouble(valori_tva_split[i]);
                row["Pret_iesire"] = Convert.ToDouble(preturi_de_iesire_split[i]);
                row["UM"] = ums_split[i];
                row["Cantitate"] = Convert.ToDouble(cantitati_split[i]);

                nir.Rows.Add(row);

                data[0] = produse_split[i];
                data[2] = preturi_intrare_fara_tva_split[i];
                data[3] = valori_tva_split[i];
                data[4] = preturi_de_iesire_split[i];
                data[1] = cantitati_split[i];

                dataGridView2.Rows.Add(data);
            }
        }

        void draw_basic(System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            string font_family = "Arial";
            Font font_giant_bold = new Font(font_family, 18, FontStyle.Bold);
            Font font_medium = new Font(font_family, 12);
            Font font_normal = new Font(font_family, 8);
            Font font_normal7 = new Font(font_family, 7);
            Font font_normal_bold = new Font(font_family, 8, FontStyle.Bold);
            Font font_normal9 = new Font(font_family, 9);
            int cadran_bottom = 150, cadran_up = 65;

            g.DrawString(firma, font_medium, Brushes.Black, new Point(10, 10));
            g.DrawString("Cod fiscal: " +cui, font_medium, Brushes.Black, new Point(10, 30));
            g.DrawString("Reg. com: " + numar_inregistrare, font_medium, Brushes.Black, new Point(10, 50));
            g.DrawString("Notă de recepție și constatare de diferențe", font_giant_bold, Brushes.Black, new Point(508, 10));
            g.DrawLine(new Pen(Brushes.Black), new Point(400, cadran_up), new Point(e.PageBounds.Width - 40, cadran_up));
            g.DrawLine(new Pen(Brushes.Black), new Point(400, 65), new Point(400, cadran_bottom));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - 40, cadran_up), new Point(e.PageBounds.Width - 40, cadran_bottom));
            g.DrawLine(new Pen(Brushes.Black), new Point(400, cadran_bottom), new Point(e.PageBounds.Width - 40, cadran_bottom));
            g.DrawLine(new Pen(Brushes.Black), new Point(400, cadran_bottom - 27), new Point(e.PageBounds.Width - 40, cadran_bottom - 27));

            int xstart_cadran = 400;
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 36, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawString("OP", font_normal, Brushes.Black, new Point(xstart_cadran - 27, cadran_up + 18));
            g.DrawString("PAD", font_normal, Brushes.Black, new Point(xstart_cadran - 31, cadran_up + 33));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 36, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawString("COP", font_normal, Brushes.Black, new Point(xstart_cadran - 31, cadran_up + 18));
            g.DrawString("GST.", font_normal, Brushes.Black, new Point(xstart_cadran - 32, cadran_up + 33));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 70, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawString("COD", font_normal, Brushes.Black, new Point(xstart_cadran - 50, cadran_up + 18));
            g.DrawString("FURNIZOR", font_normal, Brushes.Black, new Point(xstart_cadran - 65, cadran_up + 33));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 80, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawString("NR. FACTURA", font_normal, Brushes.Black, new Point(xstart_cadran - 79, cadran_up + 10));
            g.DrawString("AVIZ", font_normal, Brushes.Black, new Point(xstart_cadran - 55, cadran_up + 25));
            g.DrawString("EXPEDITIE", font_normal, Brushes.Black, new Point(xstart_cadran - 71, cadran_up + 40));
            g.DrawString(numar_factura, font_normal7, Brushes.Black, new Point(xstart_cadran - 76, cadran_up + 67));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 70, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawString("DATA", font_normal, Brushes.Black, new Point(xstart_cadran - 54, cadran_up + 18));
            g.DrawString("EXPED.", font_normal, Brushes.Black, new Point(xstart_cadran - 58, cadran_up + 33));

            DateTime dat = DateTime.Parse(data);
            string days = dat.Day.ToString(), month = dat.Month.ToString(), year = dat.Year.ToString();
            if (days.Length == 1)
                days = "0" + days;
            if (month.Length == 1)
                month = "0" + month;
            string date = days + "." + month + "." + year;
            g.DrawString(date, font_normal, Brushes.Black, new Point(xstart_cadran - 66, cadran_up + 65));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 155, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran - 155, cadran_up + 20), new Point(xstart_cadran, cadran_up + 20));
            g.DrawString("NOTĂ DE RECEPȚIE", font_normal, Brushes.Black, new Point(xstart_cadran - 134, cadran_up + 5));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran - 65, cadran_up + 20), new Point(xstart_cadran - 65, cadran_bottom));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran - 155, cadran_up + 38), new Point(xstart_cadran - 65, cadran_up + 38));
            g.DrawString("DATA", font_normal, Brushes.Black, new Point(xstart_cadran - 128, cadran_up + 23));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran - 125, cadran_up + 38), new Point(xstart_cadran - 125, cadran_bottom - 27));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran - 95, cadran_up + 38), new Point(xstart_cadran - 95, cadran_bottom - 27));
            g.DrawString("Z", font_normal, Brushes.Black, new Point(xstart_cadran - 144, cadran_up + 41));
            g.DrawString("L", font_normal, Brushes.Black, new Point(xstart_cadran - 114, cadran_up + 41));
            g.DrawString("A", font_normal, Brushes.Black, new Point(xstart_cadran - 85, cadran_up + 41));
            g.DrawString(date, font_normal, Brushes.Black, new Point(xstart_cadran - 139, cadran_up + 65));
            g.DrawString("NUMAR", font_normal, Brushes.Black, new Point(xstart_cadran - 54, cadran_up + 33));
            g.DrawString(numar_de_receptie.ToString(), font_normal7, Brushes.Black, new Point(xstart_cadran - 60, cadran_up + 67));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_cadran += 155, cadran_up), new Point(xstart_cadran, cadran_bottom));
            g.DrawString("NR.   ------------------------", font_normal, Brushes.Black, new Point(xstart_cadran - 135, cadran_up + 25));
            g.DrawString("CONTRACT", font_normal, Brushes.Black, new Point(xstart_cadran - 94, cadran_up + 20));
            g.DrawString("COMANDĂ", font_normal, Brushes.Black, new Point(xstart_cadran - 93, cadran_up + 34));
            g.DrawString("CONT", font_normal, Brushes.Black, new Point(xstart_cadran + 45, cadran_up + 18));
            g.DrawString("CREDITOR", font_normal, Brushes.Black, new Point(xstart_cadran + 31, cadran_up + 33));

            g.DrawString("Subsemnații, membri ai comisiei de recepție am procedat la recepționarea valorilor materiale furnizate de .................................................................................................... din .................................", font_normal9, Brushes.Black, new Point(40, 175));
            g.DrawString(nume_furnizor, font_normal_bold, Brushes.Black, new Point(656, 173));
            g.DrawString("cu vagonul / auto nr .................................................. documente însoțitoare .................................................................................................. constatăndu-se următoarele ...........................................", font_normal9, Brushes.Black, new Point(40, 190));
            g.DrawString(numar_factura, font_normal_bold, Brushes.Black, new Point(482, 189));

            g.DrawLine(new Pen(Brushes.Black), new Point(40, 230), new Point(e.PageBounds.Width - 40, 230));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, 230), new Point(40, 310));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - 40, 230), new Point(e.PageBounds.Width - 40, 310));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, 310), new Point(e.PageBounds.Width - 40, 310));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, 290), new Point(e.PageBounds.Width - 40, 290));

            int xstart_tabel = 40, ystart_tabel = 230;

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 35, 230), new Point(xstart_tabel, 310));
            g.DrawString("Nr.", font_normal, Brushes.Black, new Point(xstart_tabel - 27, ystart_tabel + 18));
            g.DrawString("Crt.", font_normal, Brushes.Black, new Point(xstart_tabel - 29, ystart_tabel + 28));
            g.DrawString("0", font_normal, Brushes.Black, new Point(xstart_tabel - 23, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 165, 230), new Point(xstart_tabel, 310));
            g.DrawString("Specificație", font_normal, Brushes.Black, new Point(xstart_tabel - 118, ystart_tabel + 15));
            g.DrawString("mărfuri și ambalaje", font_normal, Brushes.Black, new Point(xstart_tabel - 136, ystart_tabel + 29));
            g.DrawString("1", font_normal, Brushes.Black, new Point(xstart_tabel - 86, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 34, 230), new Point(xstart_tabel, 310));
            g.DrawString("Cod", font_normal, Brushes.Black, new Point(xstart_tabel - 29, ystart_tabel + 24));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 80, 230), new Point(xstart_tabel, 310));
            g.DrawString("Adaos", font_normal, Brushes.Black, new Point(xstart_tabel - 58, ystart_tabel + 10));
            g.DrawString("comercial", font_normal, Brushes.Black, new Point(xstart_tabel - 65, ystart_tabel + 24));
            g.DrawString("pe UM", font_normal, Brushes.Black, new Point(xstart_tabel - 58, ystart_tabel + 38));
            g.DrawString("2 (7 - 6)", font_normal, Brushes.Black, new Point(xstart_tabel - 61, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 34, 230), new Point(xstart_tabel, 310));
            g.DrawString("UM", font_normal, Brushes.Black, new Point(xstart_tabel - 27, ystart_tabel + 24));
            g.DrawString("3", font_normal, Brushes.Black, new Point(xstart_tabel - 22, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 100, 230), new Point(xstart_tabel, 310));
            g.DrawString("Cantitate", font_normal, Brushes.Black, new Point(xstart_tabel - 75, ystart_tabel + 2));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel - 100, ystart_tabel + 15), new Point(xstart_tabel, ystart_tabel + 15));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel - 50, ystart_tabel + 15), new Point(xstart_tabel - 50, 310));
            g.DrawString("Livr.", font_normal, Brushes.Black, new Point(xstart_tabel - 87, ystart_tabel + 29));
            g.DrawString("Prim.", font_normal, Brushes.Black, new Point(xstart_tabel - 38, ystart_tabel + 29));
            g.DrawString("4", font_normal, Brushes.Black, new Point(xstart_tabel - 80, 294));
            g.DrawString("5", font_normal, Brushes.Black, new Point(xstart_tabel - 30, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 80, 230), new Point(xstart_tabel, 310));
            g.DrawString("Preț", font_normal, Brushes.Black, new Point(xstart_tabel - 52, ystart_tabel + 10));
            g.DrawString("furnizor", font_normal, Brushes.Black, new Point(xstart_tabel - 61, ystart_tabel + 22));
            g.DrawString("fără TVA", font_normal, Brushes.Black, new Point(xstart_tabel - 64, ystart_tabel + 36));
            g.DrawString("6", font_normal, Brushes.Black, new Point(xstart_tabel - 46, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 80, 230), new Point(xstart_tabel, 310));
            g.DrawString("Preț", font_normal, Brushes.Black, new Point(xstart_tabel - 52, ystart_tabel + 10));
            g.DrawString("amănunt", font_normal, Brushes.Black, new Point(xstart_tabel - 62, ystart_tabel + 22));
            g.DrawString("fără TVA", font_normal, Brushes.Black, new Point(xstart_tabel - 63, ystart_tabel + 36));
            g.DrawString("7 (6 x 12)", font_normal, Brushes.Black, new Point(xstart_tabel - 65, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 46, 230), new Point(xstart_tabel, 310));
            g.DrawString("T.V.A", font_normal, Brushes.Black, new Point(xstart_tabel - 38, ystart_tabel + 18));
            g.DrawString("%", font_normal, Brushes.Black, new Point(xstart_tabel - 30, ystart_tabel + 31));
            g.DrawString("8", font_normal, Brushes.Black, new Point(xstart_tabel - 28, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 56, 230), new Point(xstart_tabel, 310));
            g.DrawString("T.V.A", font_normal, Brushes.Black, new Point(xstart_tabel - 43, ystart_tabel + 24));
            g.DrawString("9 (7 x 8)", font_normal, Brushes.Black, new Point(xstart_tabel - 49, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 66, 230), new Point(xstart_tabel, 310));
            g.DrawString("Valoare", font_normal, Brushes.Black, new Point(xstart_tabel - 53, ystart_tabel + 18));
            g.DrawString("T.V.A", font_normal, Brushes.Black, new Point(xstart_tabel - 49, ystart_tabel + 31));
            g.DrawString("10 (9 x 5)", font_normal, Brushes.Black, new Point(xstart_tabel - 58, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 70, 230), new Point(xstart_tabel, 310));
            g.DrawString("Preț", font_normal, Brushes.Black, new Point(xstart_tabel - 49, ystart_tabel + 10));
            g.DrawString("amănunt", font_normal, Brushes.Black, new Point(xstart_tabel - 59, ystart_tabel + 22));
            g.DrawString("cu TVA", font_normal, Brushes.Black, new Point(xstart_tabel - 57, ystart_tabel + 36));
            g.DrawString("11 (7 + 9)", font_normal, Brushes.Black, new Point(xstart_tabel - 62, 294));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 120, 230), new Point(xstart_tabel, 310));
            g.DrawString("Adaos com.", font_normal, Brushes.Black, new Point(xstart_tabel - 92, ystart_tabel + 2));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel - 120, ystart_tabel + 15), new Point(xstart_tabel, ystart_tabel + 15));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel - 90, ystart_tabel + 15), new Point(xstart_tabel - 90, 310));
            g.DrawString("%", font_normal, Brushes.Black, new Point(xstart_tabel - 112, ystart_tabel + 29));
            g.DrawString("Valoare", font_normal, Brushes.Black, new Point(xstart_tabel - 67, ystart_tabel + 29));
            g.DrawString("12", font_normal, Brushes.Black, new Point(xstart_tabel - 113, 294));
            g.DrawString("13 (2 x 5)", font_normal, Brushes.Black, new Point(xstart_tabel - 72, 294));


            g.DrawString("Valoare", font_normal, Brushes.Black, new Point(xstart_tabel + 41, ystart_tabel + 10));
            g.DrawString("amănunt", font_normal, Brushes.Black, new Point(xstart_tabel + 39, ystart_tabel + 22));
            g.DrawString("cu TVA", font_normal, Brushes.Black, new Point(xstart_tabel + 41, ystart_tabel + 36));
            g.DrawString("14 (11 x 5)", font_normal, Brushes.Black, new Point(xstart_tabel + 33, 294));

            int ystart = e.PageBounds.Height - 100;

            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart), new Point(e.PageBounds.Width - 40, ystart));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart), new Point(40, ystart + 95));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - 40, ystart), new Point(e.PageBounds.Width - 40, ystart + 95));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart + 95), new Point(e.PageBounds.Width - 40, ystart + 95));

            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart + 15), new Point(e.PageBounds.Width - 40, ystart + 15));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart + 35), new Point(e.PageBounds.Width - 40, ystart + 35));
            g.DrawString("COMISIA DE RECEPȚIE", font_normal, Brushes.Black, new Point(45, ystart + 1));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - 40 - 280, ystart), new Point(e.PageBounds.Width - 40 - 280, ystart + 95));
            g.DrawString("DATA", font_normal, Brushes.Black, new Point(e.PageBounds.Width - 40 - 227, ystart + 20));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - 40 - 140, ystart + 15), new Point(e.PageBounds.Width - 40 - 140, ystart + 95));
            g.DrawString("SEMNĂTURA", font_normal, Brushes.Black, new Point(e.PageBounds.Width - 40 - 105, ystart + 20));
            g.DrawString("PRIMIT ÎN GESTIUNE", font_normal, Brushes.Black, new Point(e.PageBounds.Width - 40 - 275, ystart + 1));


            int width_receptie = e.PageBounds.Width - 80 - 280;
            int xstart = 40;
            int width_nume = width_receptie / 3, width_semnatura = width_receptie / 6;
            g.DrawString("NUME ȘI PRENUME", font_normal, Brushes.Black, new Point(45 + 75, ystart + 20));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart += width_nume, ystart + 15), new Point(xstart, ystart + 95));
            g.DrawString("SEMNĂTURA", font_normal, Brushes.Black, new Point(xstart + 5 + 27, ystart + 20));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart += width_semnatura, ystart + 15), new Point(xstart, ystart + 95));
            g.DrawString("NUME ȘI PRENUME", font_normal, Brushes.Black, new Point(xstart + 80, ystart + 20));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart += width_nume, ystart + 15), new Point(xstart, ystart + 95));
            g.DrawString("SEMNĂTURA", font_normal, Brushes.Black, new Point(xstart + 5 + 27, ystart + 20));

            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart + 65), new Point(e.PageBounds.Width - 40 - 280, ystart + 65));
        }

        void draw_product(System.Drawing.Printing.PrintPageEventArgs e, int ystart_tabel, DataRow row1, int index, int rows)
        {
            string font_family = "Arial";
            int xstart_tabel = 40;
            Font font_normal7 = new Font(font_family, 7);
            Font font_normal = new Font(font_family, 8);
            Graphics g = e.Graphics;

            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart_tabel), new Point(40, ystart_tabel - 20 * rows));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - 40, ystart_tabel - 20 * rows), new Point(e.PageBounds.Width - 40, ystart_tabel));
            g.DrawLine(new Pen(Brushes.Black), new Point(40, ystart_tabel), new Point(e.PageBounds.Width - 40, ystart_tabel));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 35, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString(index.ToString(), font_normal, Brushes.Black, new Point(xstart_tabel - 23, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 165, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));

            int randuri = 0;
            string rand = "";
            string[] cuvinte = row1["Nume_produs"].ToString().Split(' ');
            string rand_plus_curent = "";
            SizeF stringSize = new SizeF();

            for (int i = 0; i < cuvinte.Length; ++i)
            {
                rand_plus_curent = rand + cuvinte[i] + " ";
                stringSize = g.MeasureString(rand_plus_curent, font_normal);
                if (stringSize.Width <= 185)
                    rand += cuvinte[i] + " ";
                else
                {
                    g.DrawString(rand, font_normal7, Brushes.Black, new Point(xstart_tabel - 163, ystart_tabel - 16 * (rows - randuri)));
                    rand = cuvinte[i];
                    ++randuri;
                }
            }

            g.DrawString(rand, font_normal7, Brushes.Black, new Point(xstart_tabel - 163, ystart_tabel - 16 * (rows - randuri)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 34, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 80, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));

            double pret_iesire_fara_tva_double = Math.Round(Convert.ToDouble(row1["Pret_iesire"].ToString()) / 1.19, 2);
            double pret_intrare_double = Math.Round(Convert.ToDouble(row1["Pret_intrare_fara_TVA"].ToString()), 2);
            double tva_val = Math.Round((pret_iesire_fara_tva_double * 0.19), 2);
            string tva_val_string = tva_val.ToString().Replace('.', ',');
            if (tva_val - Math.Truncate(tva_val) == 0)
                tva_val_string += ",00";

            double adaos = Math.Round(pret_iesire_fara_tva_double - pret_intrare_double, 2);
            string adaos_string = adaos.ToString().Replace('.', ',');
            if (adaos - Math.Truncate(adaos) == 0)
                adaos_string += ",00";

            g.DrawString(adaos_string, font_normal7, Brushes.Black, new Point(xstart_tabel - 75, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 34, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString(row1["UM"].ToString(), font_normal7, Brushes.Black, new Point(xstart_tabel - 27, ystart_tabel - 16 - 9 * (rows - 1)));

            double cantitate = Math.Round(Convert.ToDouble(row1["Cantitate"].ToString()), 2);
            string cant = cantitate.ToString().Replace('.', ',');
            if (cantitate - Math.Truncate(cantitate) == 0)
                cant += ",00";

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 100, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString(cant, font_normal7, Brushes.Black, new Point(xstart_tabel - 95, ystart_tabel - 16 - 9 * (rows - 1)));
            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel - 50, ystart_tabel - 20 * rows), new Point(xstart_tabel - 50, ystart_tabel));
            g.DrawString(cant, font_normal7, Brushes.Black, new Point(xstart_tabel - 45, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 80, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));

            string pret_intrare = row1["Pret_intrare_fara_TVA"].ToString();
            if (Convert.ToDouble(row1["Pret_intrare_fara_TVA"].ToString()) - Math.Truncate(Convert.ToDouble(row1["Pret_intrare_fara_TVA"].ToString())) == 0)
                pret_intrare += ",00";

            g.DrawString(pret_intrare.Replace('.', ','), font_normal7, Brushes.Black, new Point(xstart_tabel - 75, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 80, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));

            string pret_iesire_fara_tva = Math.Round(Convert.ToDouble(row1["Pret_iesire"].ToString()) / 1.19, 2).ToString();
            if (Convert.ToDouble(pret_iesire_fara_tva) - Math.Truncate(Convert.ToDouble(pret_iesire_fara_tva)) == 0)
                pret_iesire_fara_tva += ",00";
            g.DrawString(pret_iesire_fara_tva.Replace('.', ','), font_normal7, Brushes.Black, new Point(xstart_tabel - 75, ystart_tabel - 16 - 9 * (rows - 1)));


            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 46, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString("19,00", font_normal7, Brushes.Black, new Point(xstart_tabel - 37, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 56, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString(tva_val_string, font_normal7, Brushes.Black, new Point(xstart_tabel - 51, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 66, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString(cant, font_normal7, Brushes.Black, new Point(xstart_tabel - 45, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 70, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));

            string pret_iesire = row1["Pret_iesire"].ToString();
            if (Convert.ToDouble(pret_iesire) - Math.Truncate(Convert.ToDouble(pret_iesire)) == 0)
                pret_iesire += ",00";

            g.DrawString(pret_iesire.Replace('.', ','), font_normal7, Brushes.Black, new Point(xstart_tabel - 65, ystart_tabel - 16 - 9 * (rows - 1)));

            double adaos_la_suta = Math.Round((adaos * 100) / pret_intrare_double, 2);
            string adaos_suta_string = adaos_la_suta.ToString().Replace('.', ',');

            if (adaos_la_suta - Math.Truncate(adaos_la_suta) == 0)
                adaos_suta_string += ",00";
            double valoare_adaos = Convert.ToDouble(row1["Cantitate"].ToString()) * adaos;
            string valoare_adaos_string = valoare_adaos.ToString().Replace('.', '.');
            if (valoare_adaos - Math.Truncate(valoare_adaos) == 0)
                valoare_adaos_string += ",00";

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel += 120, ystart_tabel - 20 * rows), new Point(xstart_tabel, ystart_tabel));
            g.DrawString(adaos_suta_string, font_normal7, Brushes.Black, new Point(xstart_tabel - 119, ystart_tabel - 16 - 9 * (rows - 1)));
            g.DrawString(valoare_adaos_string, font_normal7, Brushes.Black, new Point(xstart_tabel - 85, ystart_tabel - 16 - 9 * (rows - 1)));

            g.DrawLine(new Pen(Brushes.Black), new Point(xstart_tabel - 90, ystart_tabel - 20 * rows), new Point(xstart_tabel - 90, ystart_tabel));

            double val_total = Math.Round(Convert.ToDouble(row1["Pret_iesire"].ToString()) * Convert.ToDouble(row1["Cantitate"].ToString()), 2);
            string val_total_string = val_total.ToString().Replace('.', ',');
            if (val_total - Math.Truncate(val_total) == 0)
                val_total_string += ",00";

            g.DrawString(val_total_string, font_normal7, Brushes.Black, new Point(xstart_tabel + 5, ystart_tabel - 16 - 9 * (rows - 1)));
            
        }

        void draw_data(System.Drawing.Printing.PrintPageEventArgs e)
        {
            int ystart_label = 310;
            Graphics g = e.Graphics;
            int index = 0;
            int rows;
            double total_tva = 0, total_adaos_comercial = 0, total_nir = 0, total_vanzare = 0;
            string font_family = "Arial";
            Font font_big_bold = new Font(font_family, 10, FontStyle.Bold);
            Font font_normal = new Font(font_family, 8);
            SizeF stringSize = new SizeF();

            foreach (DataRow row1 in nir.Rows)
            {
                total_tva += (Convert.ToDouble(row1["Pret_iesire"].ToString()) / 1.19) * 0.19;
                total_adaos_comercial += (Convert.ToDouble(row1["Pret_iesire"].ToString()) / 1.19 - Convert.ToDouble(row1["Pret_intrare_fara_TVA"].ToString()));
                total_nir += Convert.ToDouble(row1["Pret_intrare_fara_TVA"].ToString()) * Convert.ToDouble(row1["Cantitate"].ToString());
                total_vanzare += Convert.ToDouble(row1["Pret_iesire"].ToString()) * Convert.ToDouble(row1["Cantitate"].ToString());
                stringSize = g.MeasureString(row["Nume_produs"].ToString(), font_normal);
                if (stringSize.Width % 160 != 0)
                    rows = Convert.ToInt32(stringSize.Width) / 160 + 1;
                else
                    rows = Convert.ToInt32(stringSize.Width) / 160 + 1;
                draw_product(e, ystart_label += 20 * rows, row1, ++index, rows);
            }

            int ystart = e.PageBounds.Height - 125;
            g.DrawString("TOTAL", font_big_bold, Brushes.Black, new Point(40, ystart));
            string total_tva_string = Math.Round(total_tva, 2).ToString().Replace('.', ',');
            if (total_tva - Math.Truncate(total_tva) == 0)
                total_tva_string += ",00";
            g.DrawString(total_tva_string, font_big_bold, Brushes.Black, new Point(712, ystart));

            string total_adaos_string = Math.Round(total_adaos_comercial, 2).ToString().Replace('.', ',');
            if (total_adaos_comercial - Math.Truncate(total_adaos_comercial) == 0)
                total_adaos_string += ",00";

            g.DrawString(total_adaos_string, font_big_bold, Brushes.Black, new Point(938, ystart));

            string total_nir_string = Math.Round(total_nir, 2).ToString().Replace('.', ',');
            if (total_nir - Math.Round(total_nir) == 0)
                total_nir_string += ",00";
            g.DrawString(total_nir_string, font_big_bold, Brushes.Black, new Point(500, ystart));

            string total_vanzare_string = Math.Round(total_vanzare, 2).ToString().Replace('.', ',');
            if (total_vanzare - Math.Round(total_vanzare) == 0)
                total_vanzare_string += ",00";
            g.DrawString(total_vanzare_string, font_big_bold, Brushes.Black, new Point(1025, ystart));
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            draw_basic(e);
            draw_data(e);
        }

        private void Nir_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).ToString();
            path = path.Remove(path.Length - 9);
            path = path.Remove(0, 6);
            string connection_string = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =" + path + @"SharpBill.mdf; Integrated Security = True; Connect Timeout = 30";
            conn = new SqlConnection(connection_string);

            nir = new DataTable();

            col = new DataColumn("Nume_produs");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            col = new DataColumn("Pret_intrare_fara_TVA");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("Valori_TVA");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("Pret_iesire");
            col.DataType = System.Type.GetType("System.Double");
            nir.Columns.Add(col);

            col = new DataColumn("UM");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.String");
            nir.Columns.Add(col);

            load();
            add_to_datagridview();

            dataGridView2.AllowUserToAddRows = dataGridView2.AllowUserToResizeRows = dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("PaperA4", 826, 1169);
            printDocument1.DefaultPageSettings.Landscape = true;
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
        }
    }
}
