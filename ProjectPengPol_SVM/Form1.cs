using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
namespace ProjectPengPol_SVM
{
    public partial class Form1 : Form
    {
        public double fungsikeputusan;
        public data[] list;
        public data datauji;
        public double maxBB, maxTB, minBB, minTB;
        public double [,]matrixkernel,matrixdij,matrixkelas;
        int ordo = 5;
        public double maxDij;
        public double lambda = 3;
        public double gama;
        public double constanta=5;
        public double[] ei,delta_ai,alpha_i;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            list = new data[ordo];
            list[0]=(new data(1, 60,  165, "normal",1));
            list[1]=(new data(2, 70,  160, "normal", 1));
            list[2]=(new data(3, 80,  165, "normal", 1));
            list[3]=(new data(4, 100, 155, "tidak", -1));
            list[4]=(new data(5, 40,  175, "tidak", -1));
            tampilkan_TabelSoal();
        }

        public double fungsikernel(data d1, data d2)
        {
            double nilaikernel = Math.Pow(((d1.bb * d2.bb) + (d1.tb * d2.tb) + 1), 2);
            return nilaikernel;
        }

        public void hitung_matrixkernel()
        {
            this.matrixkernel = new double[list.Length, list.Length];
            for (int i = 0; i < ordo; i++)
            {
                for (int j = 0; j < ordo; j++)
                {
                    this.matrixkernel[i, j] = fungsikernel(list[i], list[j]);
                    list[i].fk[j] = matrixkernel[i, j];
                }
            }
        }

        public void hitung_matrixkelas()
        {
            this.matrixkelas = new double[ordo, ordo];
            for (int i = 0; i < ordo; i++)
            {
                for (int j = 0; j < ordo; j++)
                {
                    this.matrixkelas[i, j] = list[i].kelas * list[j].kelas;
                    list[i].kls[j] = matrixkelas[i, j];
                }
            }
        }

        public void hitung_matrixdij()
        {
            this.matrixdij = new double[ordo, ordo];
            for (int i = 0; i < ordo; i++)
            {
                for (int j = 0; j < ordo; j++)
                {
                    this.matrixdij[i, j] = (matrixkernel[i, j] + (Math.Pow(lambda, 2))) * matrixkelas[i, j];
                    list[i].dij[j] = matrixdij[i, j];
                }
            }
            hitung_max_dij();
        }
        public void hitung_max_dij()
        {
            this.maxDij = double.MinValue;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (matrixdij[i, j] > maxDij)
                    {
                        this.maxDij = matrixdij[i, j];
                    }
                }
            }
        }

        public void hitung_alpha()
        {
            lambda = 3;
            constanta = 5;
            gama = (2 / maxDij) / 2;
            ei = new double[5];
            delta_ai = new double[5];
            alpha_i = new double[5];
            for (int i = 0; i < 5; i++)
            {
                ei[i] = 0;
                delta_ai[i] = 0;
                alpha_i[i] = 0;
            }
            double f_lama = 0;
            double threshold = 1e-10;
            int jumlahiterasi = 0;
            double delta = double.MaxValue;
            do
            {                
                for (int i = 0; i < 5; i++)
                {
                    ei[i] = 0;
                    for (int j = 0; j < 5; j++)
                    {
                        ei[i] += alpha_i[j] * matrixdij[j, i];
                    }
                    delta_ai[i] = Math.Min(Math.Max(gama * (1 - ei[i]), (-alpha_i[i])), (constanta - alpha_i[i]));
                    alpha_i[i] += delta_ai[i];
                }
                double f_baru = 0;
                for (int i = 0; i < alpha_i.Length; i++)
                {
                    f_baru += alpha_i[i];
                }
                f_baru = f_baru / 5;
                delta = Math.Abs(f_baru - f_lama);
                jumlahiterasi += 1;
                f_lama = f_baru;                
            } while (delta > threshold);
            textBox_iterasi.Text = jumlahiterasi.ToString();            
        }
        void hitung_fungsikeputusan()
        {
            this.fungsikeputusan = 0;
            for (int  i = 0;  i <5;  i++)
            {
                fungsikeputusan += alpha_i[i] * list[i].kelas * Math.Pow((list[i].bb * datauji.bb + list[i].tb * datauji.tb + 1), 2);
            }
        }
        private void button_next1_Click(object sender, EventArgs e)
        {
            hitung_matrixkernel();
            tampilkan_tabelmatrikkernel();
            hitung_matrixkelas();
            tampilkan_tabelmatrikkelas();
        }
        private void button_next2_Click(object sender, EventArgs e)
        {
            hitung_matrixdij();
            tampilkan_tabelmatrikdij();
            textBox_maxdij.Text = maxDij.ToString();
        }
        private void button_next3_Click(object sender, EventArgs e)
        {

            datauji = new data(6, Double.Parse(textBox_bb.Text), Double.Parse(textBox_tb.Text), "null", 0);
            hitung_fungsikeputusan();
            //test data uji
            textBox_nilaidatauji.Text = fungsikeputusan.ToString();
            textBox_kelasdatauji.Text = (Math.Sign(fungsikeputusan)).ToString();
            if ((Math.Sign(fungsikeputusan)) == 1)
            {
                textBox_statusdatauji.Text = "normal";
            }
            else if ((Math.Sign(fungsikeputusan)) == -1)
            {
                textBox_statusdatauji.Text = "tidak";
            }
        }
        private void button_next2_2_Click(object sender, EventArgs e)
        {
            hitung_alpha();
            textBox_constanta.Text = constanta.ToString();
            textBox_gama.Text = gama.ToString();
            textBox_lambda.Text = lambda.ToString();

            textBox1.Text = alpha_i[0].ToString();
            textBox2.Text = alpha_i[1].ToString();
            textBox3.Text = alpha_i[2].ToString();
            textBox4.Text = alpha_i[3].ToString();
            textBox5.Text = alpha_i[4].ToString();
        }
        public class data
        {
            public int no;
            public double bb, tb;
            public string status;
            public double kelas;
            public double[] fk = new double[5];
            public double[] dij = new double[5];
            public double[] kls = new double[5];
            public data()
            {
            }
            public data(int no,double a, double b, string c, double d)
            {
                this.no = no;
                this.bb = a;
                this.tb = b;
                this.status = c;
                this.kelas = d;
            }
        }
        public void tampilkan_TabelSoal()
        {
            listView1.Items.Clear();
            foreach (data x in list)
            {
                ListViewItem item = new ListViewItem(x.no.ToString());
                item.SubItems.Add(x.bb.ToString());
                item.SubItems.Add(x.tb.ToString());
                item.SubItems.Add(x.status.ToString());
                item.SubItems.Add(x.kelas.ToString());
                listView1.Items.Add(item);
            }
        }
        public void tampilkan_tabelmatrikkernel()
        {
            listView3.Items.Clear();
            foreach (data x in list)
            {
                ListViewItem item = new ListViewItem(x.fk[0].ToString());
                item.SubItems.Add(x.fk[1].ToString());
                item.SubItems.Add(x.fk[2].ToString());
                item.SubItems.Add(x.fk[3].ToString());
                item.SubItems.Add(x.fk[4].ToString());
                listView3.Items.Add(item);
            }
        }
        public void tampilkan_tabelmatrikkelas()
        {
            listView3_2.Items.Clear();
            foreach (data x in list)
            {
                ListViewItem item = new ListViewItem(x.kls[0].ToString());
                item.SubItems.Add(x.kls[1].ToString());
                item.SubItems.Add(x.kls[2].ToString());
                item.SubItems.Add(x.kls[3].ToString());
                item.SubItems.Add(x.kls[4].ToString());
                listView3_2.Items.Add(item);
            }
        }
        public void tampilkan_tabelmatrikdij()
        {
            listView4.Items.Clear();
            foreach (data x in list)
            {
                ListViewItem item = new ListViewItem(x.dij[0].ToString());
                item.SubItems.Add(x.dij[1].ToString());
                item.SubItems.Add(x.dij[2].ToString());
                item.SubItems.Add(x.dij[3].ToString());
                item.SubItems.Add(x.dij[4].ToString());
                listView4.Items.Add(item);
            }
        }
    }
}
