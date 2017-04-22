using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Torpedo_HACF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Tábla
        public int THOSSZ, HajoDb;
        public string[,] tabla;
        public string[,] geptabla;
        struct Hajo
        {
            public int balfelsox;
            public int balfelsoy;
            public int hossz;
            public bool irany;
            //igaz - vizszintes
            //hamis fuggolege

        }
        

        public int convertColToNum(char C)
        {
            //65-90 -> -64
            return (C) - 65;
        }

        List<Hajo> hajok = new List<Hajo>();
        List<Hajo> gepihajok = new List<Hajo>();





        private void button1_Click(object sender, EventArgs e)
        {
            int th, tsz, hdb;
            if (int.TryParse(textBox1.Text, out th) && int.TryParse(textBox3.Text, out tsz) && int.TryParse(textBox3.Text, out hdb))
            {
                THOSSZ = th;
                HajoDb = hdb;

                tabla = new string[THOSSZ, THOSSZ];
                geptabla = new string[THOSSZ, THOSSZ];
                for (int i = 0; i < THOSSZ; i++)
                {
                    for (int j = 0; j < THOSSZ; j++)
                    {
                        tabla[i, j] = "-";
                        geptabla[i, j] = "-";
                    }
                }

                while (hajok.Count != HajoDb)
                {
                    Hajo s = new Hajo();
                    Hajo g = new Hajo();
                    string bemenet = Interaction.InputBox("Add meg a hajó adatait, a követkető formátumban: Kordináta,hossz,F/V");

                    string[] seged = bemenet.Split(',');
                    string kord = seged[0];
                    int x = convertColToNum(kord[0]);
                    int y = int.Parse(kord[1].ToString());
                    y--;
                    s.balfelsox = x;
                    s.balfelsoy = y;


                    s.hossz = int.Parse(seged[1]);
                    if (s.hossz > THOSSZ - 1)
                    {
                        MessageBox.Show("Hubás bemenet");
                        continue;
                    }
                    s.irany = seged[2] == "V";
                    if (s.irany)
                    {
                        if (x + s.hossz > THOSSZ)
                        {
                            MessageBox.Show("Hubás bemenet");
                            continue;
                        }

                    }
                    else
                    {
                        if (y + s.hossz > THOSSZ)
                        {
                            MessageBox.Show("Hubás bemenet");
                            continue;
                        }
                    }
             
                    
                    if (!Lehet(tabla,s))
                    {
                        MessageBox.Show("Hubás bemenet");
                        continue;
                    }

                    if (s.irany)
                    {
                        for (int i = x; i < x+s.hossz; i++)
                        {
                            tabla[i, y] = "h";
                        }
                    }
                    else
                    {
                        for (int i = y; i < y + s.hossz; i++)
                        {
                            tabla[x, i] = "h";
                        }
                    }

                    g.hossz = s.hossz;
                    g.irany = s.irany;
                    bool jo = false;
                    Random r = new Random();
                    while (!jo)
                    {
                        g.balfelsox = 0;
                        g.balfelsoy = 0;
                        int xgep = r.Next(0,THOSSZ);
                        int ygep = r.Next(0, THOSSZ);

                        g.balfelsox = xgep;
                        g.balfelsoy = ygep;


                        if (g.irany)
                        {
                            if (xgep + g.hossz > THOSSZ)
                            {
                                continue;
                            }

                        }
                        else
                        {
                            if (ygep + g.hossz > THOSSZ)
                            {
                                continue;
                            }
                        }

                        if (!Lehet(geptabla, g))
                        {
                            continue;
                        }

                        jo = true;
                        if (g.irany)
                        {
                            for (int i = xgep; i < xgep + g.hossz; i++)
                            {
                                geptabla[i, ygep] = "h";
                            }
                        }
                        else
                        {
                            for (int i = ygep; i < ygep + s.hossz; i++)
                            {
                                geptabla[xgep, i] = "h";
                            }
                        }

                    }

                    gepihajok.Add(g);
                    hajok.Add(s);
                }


                dgv.RowCount = THOSSZ;
                dgv.ColumnCount = THOSSZ;
                gepdgv.RowCount = THOSSZ;
                gepdgv.ColumnCount = THOSSZ;

                for (int i = 0; i < THOSSZ; i++)
                {
                    for (int j = 0; j < THOSSZ; j++)
                    {
                        dgv.Rows[i].Cells[j].Value = tabla[i, j];
                    }
                }

                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                gepdgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                gepdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;



            }

         
        }


        //REST API
        private bool Lehet(string[,] tabla, Hajo h)
        {
            if (h.irany)
            {
                for (int i = h.balfelsox; i <h.balfelsox+h.hossz; i++)
                {
                    if (tabla[i,h.balfelsoy] != "-")
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = h.balfelsoy; i < h.balfelsoy + h.hossz; i++)
                {
                    if (tabla[i, h.balfelsox] != "-")
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        string getC(int X, int Y, string[,] TABLA, List<Hajo> hajok)
        {
            if (TABLA[X, Y] == "-")
                return "0";
            if (TABLA[X, Y] == "x")
                return "1";
            if (TABLA[X, Y] == "h")
                return "0";
            if (TABLA[X, Y] == "hx")
            {
                Hajo hajo = hajok.First(x => X <= x.balfelsox + x.hossz && X >= x.balfelsox && Y <= x.balfelsoy + x.hossz && Y >= x.balfelsoy);
                if (hajo.irany) //vizszintes
                {
                    if (hajo.hossz == 1)
                    {
                        if (TABLA[Y, X] == "hx")
                            return "3";
                        else
                            return "2";
                    }
                    else
                    {
                        int hcx = 0;
                        for (int x = hajo.balfelsox; x < hajo.balfelsox + hajo.hossz; x++)
                        {
                            if (TABLA[hajo.balfelsoy, x] == "hx")
                                hcx++;
                        }
                        if (hcx == hajo.hossz)
                            return "3";
                        else
                            return "2";
                    }
                }
                else //fuggoleges
                {
                    if (hajo.hossz == 1)
                    {
                        if (TABLA[Y, X] == "hx")
                            return "3";
                        else
                            return "2";
                    }
                    else
                    {
                        int hcy = 0;
                        for (int y = hajo.balfelsoy; y < hajo.balfelsoy + hajo.hossz; y++)
                        {
                            if (TABLA[y, hajo.balfelsox] == "hx")
                            {
                                hcy++;
                            }
                        }
                        if (hcy == hajo.hossz)
                            return "3";
                        else
                            return "2";
                    }
                }
            }
            return "";
        }

        public string ConvertTableToREST(string[,] rest, int o)
        {
            string op = "";
            for (int i = 0; i < o; i++)
            {
                for (int j = 0; j < o; j++)
                {
                    op += getC(i, j, rest, hajok);
                }
            }
            return op;
        }

        
    }
}
