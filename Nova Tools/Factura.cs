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
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Drawing.Printing;

namespace Nova_Tools
{
    public partial class Factura : Form
    {
        public Form parentForm { get; set; }

        SqlConnection conn;
        DataTable factura;
        DataColumn col;
        DataRow row;
        int numar_factura;
        string client = "", produse = "", cantitati = "", preturi_vanzare = "", discounts = "", ums = "";
        DateTime data;
        string nume, cui, numar_de_inregistrare, adresa, punct_de_lucru, capital_social, banca, cont_bancar, trezoreria, cont_trezorerie, telefon_fix, telefon_mobil, email, site;
        double valoare, valoare_tva, total_de_plata;
        string date;

        private void cauta_button_Click(object sender, EventArgs e)
        {
            printPreviewDialog1 = new PrintPreviewDialog();
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.Show();
        }

        private GraphicsPath MakeRoundedRect(RectangleF rect, float xradius, float yradius, bool round_ul, bool round_ur, bool round_lr, bool round_ll)
        {
            // Make a GraphicsPath to draw the rectangle.
            PointF point1, point2;
            GraphicsPath path = new GraphicsPath();

            // Upper left corner.
            if (round_ul)
            {
                RectangleF corner = new RectangleF(
                    rect.X, rect.Y,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 180, 90);
                point1 = new PointF(rect.X + xradius, rect.Y);
            }
            else point1 = new PointF(rect.X, rect.Y);

            // Top side.
            if (round_ur)
                point2 = new PointF(rect.Right - xradius, rect.Y);
            else
                point2 = new PointF(rect.Right, rect.Y);
            path.AddLine(point1, point2);

            // Upper right corner.
            if (round_ur)
            {
                RectangleF corner = new RectangleF(
                    rect.Right - 2 * xradius, rect.Y,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 270, 90);
                point1 = new PointF(rect.Right, rect.Y + yradius);
            }
            else point1 = new PointF(rect.Right, rect.Y);

            // Right side.
            if (round_lr)
                point2 = new PointF(rect.Right, rect.Bottom - yradius);
            else
                point2 = new PointF(rect.Right, rect.Bottom);
            path.AddLine(point1, point2);

            // Lower right corner.
            if (round_lr)
            {
                RectangleF corner = new RectangleF(
                    rect.Right - 2 * xradius,
                    rect.Bottom - 2 * yradius,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 0, 90);
                point1 = new PointF(rect.Right - xradius, rect.Bottom);
            }
            else point1 = new PointF(rect.Right, rect.Bottom);

            // Bottom side.
            if (round_ll)
                point2 = new PointF(rect.X + xradius, rect.Bottom);
            else
                point2 = new PointF(rect.X, rect.Bottom);
            path.AddLine(point1, point2);

            // Lower left corner.
            if (round_ll)
            {
                RectangleF corner = new RectangleF(
                    rect.X, rect.Bottom - 2 * yradius,
                    2 * xradius, 2 * yradius);
                path.AddArc(corner, 90, 90);
                point1 = new PointF(rect.X, rect.Bottom - yradius);
            }
            else point1 = new PointF(rect.X, rect.Bottom);

            // Left side.
            if (round_ul)
                point2 = new PointF(rect.X, rect.Y + yradius);
            else
                point2 = new PointF(rect.X, rect.Y);
            path.AddLine(point1, point2);

            // Join with the start point.
            path.CloseFigure();

            return path;
        }

        void draw_basic(System.Drawing.Printing.PrintPageEventArgs e)
        {
            Image logo = Image.FromFile("logo.png");

            string font_family = "Arial";

            Font font_giant_bold = new Font(font_family, 20, FontStyle.Bold);
            Font font_big_bold = new Font(font_family, 14, FontStyle.Bold);
            Font font_medium = new Font(font_family, 11);
            Font font_medium_bold = new Font(font_family, 11, FontStyle.Bold);
            Font font_medium1 = new Font(font_family, 11);
            Font font_medium1_bold = new Font(font_family, 11, FontStyle.Bold);
            Font font_normal = new Font(font_family, 10);
            Font font_normal_bold = new Font(font_family, 10, FontStyle.Bold);
            Font font_small = new Font(font_family, 9);
            float ystart_left = 30, xstart_left = 20;
            float ystep = 20;
            float imagewidth = 444.44f, imageheight = 333.33f, imagex = e.PageBounds.Width / 2 - imagewidth / 2, imagey = 40;

            Graphics g = e.Graphics;

            g.DrawImage(logo, imagex, imagey, imagewidth, imageheight);

            g.DrawString(nume.ToUpper(), font_big_bold, Brushes.Black, xstart_left, ystart_left);
            g.DrawString(cui + ",  " + numar_de_inregistrare, font_normal_bold, Brushes.Black, xstart_left + 1, ystart_left += 35);
            g.DrawString(adresa, font_normal_bold, Brushes.Black, xstart_left, ystart_left += ystep);
            g.DrawString("Punct de lucru: " + punct_de_lucru, font_normal_bold, Brushes.Black, xstart_left, ystart_left += ystep);
            g.DrawString("Capital social: " + capital_social, font_normal_bold, Brushes.Black, xstart_left, ystart_left += ystep);
            g.DrawString("Date bancare: " + cont_bancar, font_normal_bold, Brushes.Black, xstart_left, ystart_left += ystep);
            g.DrawString(banca, font_normal_bold, Brushes.Black, xstart_left + 96, ystart_left += ystep);
            g.DrawString(cont_trezorerie, font_normal_bold, Brushes.Black, xstart_left + 96, ystart_left += ystep);
            g.DrawString(trezoreria, font_normal_bold, Brushes.Black, xstart_left + 96, ystart_left += ystep);
            g.DrawString("E-mail: " + email, font_normal_bold, Brushes.Black, xstart_left, ystart_left += ystep);
            g.DrawString("Site: " + site, font_normal_bold, Brushes.Black, xstart_left, ystart_left += ystep);

            g.DrawString("Cota TVA: ................... %", font_medium, Brushes.Black, xstart_left + 10, ystart_left += 2 * ystep);
            g.DrawString("19,00", font_medium_bold, Brushes.Black, xstart_left + 125, ystart_left - 2);

            float ystart_right = 20, xstart_right = e.PageBounds.Width / 2 + 70;

            RectangleF rect = new RectangleF(xstart_right, ystart_right, 335, 110);
            using (Pen pen = new Pen(Brushes.Black))
            {
                GraphicsPath path = MakeRoundedRect(rect, 20, 20, true, true, true, true);
                e.Graphics.FillPath(Brushes.LightGray, path);
                e.Graphics.DrawPath(pen, path);
            }

            g.DrawString("Seria:  NTP", font_medium, Brushes.Black, xstart_right + 10, ystart_right += 10);
            g.DrawString("Numar:  " + numar_factura.ToString(), font_medium, Brushes.Black, xstart_right + 175, ystart_right);
            g.DrawString("Data (ziua, luna, anul):     " + date, font_medium, Brushes.Black, xstart_right + 10, ystart_right += ystep + 9);
            g.DrawString("Nr. avizului de însoțire a mărfii:", font_medium, Brushes.Black, xstart_right + 10, ystart_right += ystep + 9);
            g.DrawString("(daca este cazul)", font_medium, Brushes.Black, xstart_right + 90, ystart_right += ystep);

            g.DrawString("Cumpărător: ", font_medium, Brushes.Black, xstart_right, ystart_right += ystep + 9);

            ystep += 10;

            string[] client_split = client.Split(' ');
            client = "";
            int nr = 0;

            for (int i = 0; i < client_split.Length; ++i) 
            {
                client += client_split[i] + " ";
                ++nr;
                if ((nr == 4 && i == 3) || (i == (client_split.Length - 1) && client_split.Length <= 4))
                {
                    g.DrawString(client, font_medium_bold, Brushes.Black, xstart_right + 100, ystart_right);
                    client = "";
                }
                else if (nr == 4)
                {
                    g.DrawString(client, font_medium_bold, Brushes.Black, xstart_right + 100, ystart_right += ystep);
                    client = "";
                }
            }
            if(client != "")
                g.DrawString(client, font_medium_bold, Brushes.Black, xstart_right + 100, ystart_right += ystep);

            g.DrawString("CIF/CUI/CNP: ", font_medium, Brushes.Black, xstart_right, ystart_right += ystep);
            g.DrawString(cui + ", " + numar_de_inregistrare, font_medium_bold, Brushes.Black, xstart_right + 115, ystart_right);


            g.DrawString("Adresa: ", font_medium, Brushes.Black, xstart_right, ystart_right + ystep);

            string[] adresa_split = adresa.Split(' ');
            nr = 0;
            adresa = "";

            for (int i = 0; i < adresa_split.Length; ++i)
            {
                adresa += adresa_split[i] + " ";
                ++nr;
                if (nr == 4)
                {
                    g.DrawString(adresa, font_medium_bold, Brushes.Black, xstart_right + 65, ystart_right += ystep);
                    adresa = "";
                }
            }
            if(adresa != "")
                g.DrawString(adresa, font_medium_bold, Brushes.Black, xstart_right + 65, ystart_right += ystep);

            g.DrawString("Cod IBAN: ", font_medium, Brushes.Black, xstart_right, ystart_right += ystep);
            g.DrawString(cont_bancar, font_medium_bold, Brushes.Black, xstart_right + 87, ystart_right);
            g.DrawString("Banca: ", font_medium, Brushes.Black, xstart_right, ystart_right + ystep);


            string[] banca_split = banca.Split(' ');
            banca = "";
            nr = 0;

            for (int i = 0; i < banca_split.Length; ++i)
            {
                banca += banca_split[i] + " ";
                ++nr;
                if(nr == 4)
                {
                    g.DrawString(banca, font_medium_bold, Brushes.Black, xstart_right + 60, ystart_right += ystep);
                    banca = "";
                }
            }
            if(banca != "")
                g.DrawString(banca, font_medium_bold, Brushes.Black, xstart_right + 60, ystart_right += ystep);

            g.DrawString("FACTURĂ", font_giant_bold, Brushes.Black, e.PageBounds.Width / 2 - 100, 245);
            g.DrawString("FISCALĂ", font_giant_bold, Brushes.Black, e.PageBounds.Width / 2 + 35 - 100, 275);

            Point cadran_start = new Point(Convert.ToInt32(xstart_left), 325);

            //Cadran

            int cadran_height = 325;

            g.DrawLine(new Pen(Brushes.Black), cadran_start, new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), cadran_height));
            g.DrawLine(new Pen(Brushes.Black), cadran_start, new Point(Convert.ToInt32(xstart_left), e.PageBounds.Height - 30));
            g.DrawLine(new Pen(Brushes.Black), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), cadran_height), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), e.PageBounds.Height - 30));
            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left), e.PageBounds.Height - 30), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), e.PageBounds.Height - 30));

            //Header

            int header_height_end = 385, header_width = cadran_start.X + 15;
            int height_bottom = e.PageBounds.Height - 185;

            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left),header_height_end), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), header_height_end));
            g.DrawString("Nr.", font_normal, Brushes.Black, new Point(header_width, cadran_start.Y + 10));
            g.DrawString("crt.", font_normal, Brushes.Black, new Point(header_width, cadran_start.Y + 25));
            g.DrawLine(new Pen(Brushes.Black), new Point(header_width += 40, cadran_height), new Point(header_width, height_bottom));

            g.DrawString("Denumirea produselor", font_normal, Brushes.Black, new Point(header_width += 50, cadran_start.Y + 13));
            g.DrawString("sau a serviciilor", font_normal, Brushes.Black, new Point(header_width += 30, cadran_start.Y + 28));
            g.DrawLine(new Pen(Brushes.Black), new Point(header_width += 180, cadran_height), new Point(header_width, height_bottom));

            header_width -= 6;

            g.DrawString("U.M.", font_normal, Brushes.Black, new Point(header_width += 14, cadran_start.Y + 20));
            header_width -= 4;
            g.DrawLine(new Pen(Brushes.Black), new Point(header_width += 45, cadran_height), new Point(header_width, height_bottom));

            g.DrawString("Cantitatea", font_normal, Brushes.Black, new Point(header_width += 13, cadran_start.Y + 20));
            g.DrawLine(new Pen(Brushes.Black), new Point(header_width += 85, cadran_height), new Point(header_width, height_bottom));

            g.DrawString("Preț unitar", font_normal, Brushes.Black, new Point(header_width += 21, cadran_start.Y + 6));
            g.DrawString("(fără TVA)", font_normal, Brushes.Black, new Point(header_width, cadran_start.Y + 23));
            g.DrawString("-LEI-", font_normal, Brushes.Black, new Point(header_width += 18, cadran_start.Y + 37));
            g.DrawLine(new Pen(Brushes.Black), new Point(header_width += 75, cadran_height), new Point(header_width, e.PageBounds.Height - 30));
            int pret_unitar_width = header_width;

            g.DrawString("Valoarea", font_normal, Brushes.Black, new Point(header_width += 28, cadran_start.Y + 13));
            g.DrawString("-LEI-", font_normal, Brushes.Black, new Point(header_width += 15, cadran_start.Y + 28));
            g.DrawLine(new Pen(Brushes.Black), new Point(header_width += 75, cadran_height), new Point(header_width, height_bottom + 55));

            g.DrawString("Valoare TVA", font_normal, Brushes.Black, new Point(header_width += 3, cadran_start.Y + 13));
            g.DrawString("-LEI-", font_normal, Brushes.Black, new Point(header_width += 23, cadran_start.Y + 28));

            header_width = cadran_start.X + 21;

            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left), header_height_end += 20), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), header_height_end));
            g.DrawString("0", font_normal, Brushes.Black, new Point(header_width, header_height_end -= 19));
            g.DrawString("1", font_normal, Brushes.Black, new Point(header_width += 155, header_height_end));
            g.DrawString("2", font_normal, Brushes.Black, new Point(header_width += 157, header_height_end));
            g.DrawString("3", font_normal, Brushes.Black, new Point(header_width += 73, header_height_end));
            g.DrawString("4", font_normal, Brushes.Black, new Point(header_width += 108, header_height_end));
            g.DrawString("5(3x4)", font_normal, Brushes.Black, new Point(header_width += 98, header_height_end));
            g.DrawString("6", font_normal, Brushes.Black, new Point(header_width += 122, header_height_end));

            //Bottom

            g.DrawString("Factura circulă fără ștampilă și semnătură conform Legii 227/2015 art. 319, alin. 29 din Codul Fiscal.", font_normal_bold, Brushes.Black, new Point(75, height_bottom - 20));
            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left), height_bottom), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), height_bottom));
            
            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left), height_bottom + 25), new Point(pret_unitar_width, height_bottom + 25));
            g.DrawString("Întocmit: ZARZU MARLENA; CNP: 2740601044424; CI :XC 685898", font_normal_bold, Brushes.Black, new Point(25, height_bottom + 5));

            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left + 125), height_bottom + 25), new Point(Convert.ToInt32(xstart_left + 125), e.PageBounds.Height - 30));
            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left + 430), height_bottom + 25), new Point(Convert.ToInt32(xstart_left + 430), e.PageBounds.Height - 30));
            g.DrawLine(new Pen(Brushes.Black), new Point(Convert.ToInt32(xstart_left + 430), height_bottom + 55), new Point(e.PageBounds.Width - Convert.ToInt32(xstart_left), height_bottom + 55));

            g.DrawString("Semnătura și", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left += 20), height_bottom + 30));
            g.DrawString("ștampila", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left += 15), height_bottom + 42));
            g.DrawString("furnizorului", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left - 7), height_bottom + 53));

            int height_bottom_copy = height_bottom;
            g.DrawString("Date priving expediția:", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left += 95), height_bottom += 30));
            g.DrawString("Numele delegatului .................................................", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left), height_bottom += 17));
            g.DrawString("Act de identitate  Seria ..... Nr. ......... eliberat de", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left), height_bottom += 17));
            g.DrawString("Mijloc transport ........................................................", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left), height_bottom += 17));
            g.DrawString("Expedierea s-a făcut în prezența noastră", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left), height_bottom += 17));
            g.DrawString("la data " + date, font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left), height_bottom += 17));
            g.DrawString("SEMNĂTURI ...........................................................", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left), height_bottom += 17));

            height_bottom = height_bottom_copy;
            g.DrawString("TOTAL", font_normal, Brushes.Black, new Point(Convert.ToInt32(xstart_left += 345), height_bottom + 31));
            g.DrawString("Semnătură de primire", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left -= 40), height_bottom + 60));
            g.DrawString("Total de plată", font_small, Brushes.Black, new Point(Convert.ToInt32(xstart_left += 145), height_bottom + 60));
            
        }

        void draw_data(System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            string font_family = "Arial";
            Font font_normal = new Font(font_family, 10);
            Font font_big_bold = new Font(font_family, 14, FontStyle.Bold);

            int index = 0, header_width = 41, header_height_end = 406, header_width_copy = 41;
            int last_height = 41, last_height_copy;
            int max_chars = 33;
            int x_pret_unitate = 0, x_valoare_tva = 0;
            int height_bottom = e.PageBounds.Height - 185 + 31;

            foreach (DataRow row1 in factura.Rows)
            {
                ++index;
                g.DrawString(index.ToString(), font_normal, Brushes.Black, new Point(header_width, header_height_end));

                string[] nume_produs = row1["Nume_produs"].ToString().Split(' ');
                string nume_rand = "";

                last_height = header_height_end;

                header_width += 36;

                foreach(string cuvant in nume_produs)
                {
                    if (nume_rand.Length + cuvant.Length <= max_chars)
                        nume_rand += cuvant + " ";
                    else
                    {
                        g.DrawString(nume_rand, font_normal, Brushes.Black, new Point(header_width, header_height_end));
                        nume_rand = cuvant;
                        header_height_end += 20;
                    }
                }

                g.DrawString(nume_rand, font_normal, Brushes.Black, new Point(header_width, header_height_end));
                last_height_copy = header_height_end;
                header_height_end = last_height;

                g.DrawString(row1["UM"].ToString(), font_normal, Brushes.Black, new Point(header_width += 268, header_height_end));

                string cantitate = Math.Round(Convert.ToDouble(row["Cantitate"].ToString()), 2).ToString().Replace('.', ',');
                double cant = Math.Round(Convert.ToDouble(row["Cantitate"].ToString()), 2);
                if (cant - Math.Truncate(cant) == 0)
                    cantitate += ",00";
                g.DrawString(cantitate, font_normal, Brushes.Black, new Point(header_width += 48, header_height_end));
                string pret = Math.Round(Convert.ToDouble(row1["Pret_vanzare"].ToString()) / 1.19, 2).ToString().Replace('.', ',');
                double prt = Math.Round(Convert.ToDouble(row1["Pret_vanzare"].ToString()) / 1.19, 2);
                valoare += prt;
                g.DrawString(pret, font_normal, Brushes.Black, new Point(header_width += 100, header_height_end));
                string pret_total = Math.Round(prt * cant, 2).ToString().Replace('.', ',');
                double prt_total = Math.Round(prt * cant, 2);
                g.DrawString(pret_total, font_normal, Brushes.Black, new Point(header_width += 113, header_height_end));
                x_pret_unitate = header_width;
                g.DrawString((Math.Round(prt_total * 0.19, 2)).ToString().Replace('.', ','), font_normal, Brushes.Black, new Point(header_width += 118, header_height_end));
                x_valoare_tva = header_width;
                valoare_tva += Math.Round(prt_total * 0.19, 2);
                header_height_end = last_height_copy + 20;
                header_width = header_width_copy;
            }
            total_de_plata = valoare_tva + valoare;
            string plata = total_de_plata.ToString();
            if (total_de_plata - Math.Truncate(total_de_plata) == 0)
                plata += ",00";
            string total_plata = "";
            int cifre = 0;
            for (int i = plata.Length - 1; i > plata.Length - 4; --i)
                total_plata = plata[i] + total_plata;

            for(int i = plata.Length - 4;i >= 0;--i)
            {
                ++cifre;
                total_plata = plata[i] + total_plata;
                if (cifre == 3)
                {
                    total_plata = " " + total_plata;
                    cifre = 0;
                }
            }

            g.DrawString(valoare.ToString().Replace('.', ','), font_normal, Brushes.Black, new Point(x_pret_unitate, height_bottom));
            g.DrawString(valoare_tva.ToString().Replace('.', ','), font_normal, Brushes.Black, new Point(x_valoare_tva, height_bottom));
            g.DrawString(total_plata.Replace('.', ',') + " LEI", font_big_bold, Brushes.Black, new Point(e.PageBounds.Width - 215, e.PageBounds.Height - 65));
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            draw_basic(e);
            draw_data(e);
        }

        private void Factura_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(parentForm != null)
                parentForm.Show();
        }

        public Factura(int numar)
        {
            InitializeComponent();

            numar_factura = numar;
            factura = new DataTable();

            col = new DataColumn("Nume_produs");
            col.DataType = System.Type.GetType("System.String");
            factura.Columns.Add(col);

            col = new DataColumn("Cantitate");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);

            col = new DataColumn("UM");
            col.DataType = System.Type.GetType("System.String");
            factura.Columns.Add(col);

            col = new DataColumn("Pret_vanzare");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);

            col = new DataColumn("Discount");
            col.DataType = System.Type.GetType("System.Double");
            factura.Columns.Add(col);
        }
        
        void load()
        {
            SqlCommand query = new SqlCommand("SELECT * FROM Facturi WHERE Numar_factura = @numar_factura", conn);
            query.Parameters.AddWithValue("@numar_factura", numar_factura);
            SqlDataReader dr = query.ExecuteReader();
            dr.Read();

            client = furnizor_label.Text = dr[0].ToString();
            data = DateTime.Parse(dr[2].ToString());
            string days = data.Day.ToString(), month = data.Month.ToString(), year = data.Year.ToString();
            if (days.Length == 1)
                days = "0" + days;
            if (month.Length == 1)
                month = "0" + month; 
            date = days + "." + month + "." + year;
            data_label.Text = date;
            numar_factura_label.Text = numar_factura.ToString();
            produse = dr[5].ToString();
            preturi_vanzare = dr[6].ToString();
            discounts = dr[7].ToString();
            ums = dr[8].ToString();
            cantitati = dr[9].ToString();

            dr.Close(); dr.Dispose();

            query = new SqlCommand("SELECT * FROM Firma WHERE Id = @id", conn);
            query.Parameters.AddWithValue("@id", 1);
            dr = query.ExecuteReader();
            dr.Read();

            nume = dr[1].ToString(); cui = dr[2].ToString(); numar_de_inregistrare = dr[3].ToString(); adresa = dr[4].ToString(); punct_de_lucru = dr[5].ToString(); capital_social = dr[6].ToString();
            banca = dr[7].ToString(); cont_bancar = dr[8].ToString(); trezoreria = dr[9].ToString(); cont_trezorerie = dr[10].ToString(); telefon_fix = dr[11].ToString(); telefon_mobil = dr[12].ToString();
            email = dr[13].ToString(); site = dr[14].ToString();
        }

        void load_tables()
        {
            string[] produse_split = produse.Split(';');
            string[] preturi_vanzare_split = preturi_vanzare.Split(';');
            string[] discounts_split = discounts.Split(';');
            string[] ums_split = ums.Split(';');
            string[] cantitati_split = cantitati.Split(';');

            string[] data = new string[4];

            for(int i = 0;i < produse_split.Length - 1;++i)
            {
                row = factura.NewRow();

                row["Nume_produs"] = data[0] = produse_split[i];
                row["Cantitate"] = Convert.ToDouble(cantitati_split[i]); data[1] = cantitati_split[i];
                row["UM"] = data[2] = ums_split[i];
                row["Pret_vanzare"] = Convert.ToDouble(preturi_vanzare_split[i]); data[2] = preturi_vanzare_split[i];
                row["Discount"] = Convert.ToDouble(discounts_split[i]); data[3] = discounts_split[i];

                factura.Rows.Add(row);
                dataGridView2.Rows.Add(data);
            }
        }

        private void Factura_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader("connection_string.txt"))
            {
                string connection_string = sr.ReadLine();
                conn = new SqlConnection(connection_string);
            }

            conn.Open();

            load();
            load_tables();

            conn.Close();
            

            dataGridView2.AllowUserToAddRows = dataGridView2.AllowUserToResizeRows = dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("PaperA4", 826, 1169);
        }
    }
}
