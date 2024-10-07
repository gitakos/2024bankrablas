using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace bankrablas
{
    class Varos : VarosElem
    {
        public VarosElem[,] varoselemek;

        Dictionary<string, ConsoleColor> szinbizbasz = new Dictionary<string, ConsoleColor>();

        public int barikadokSzama = 70;
        public int whiskeySzama = 3;
        public int banditakSzama = 5;
        public int palyaMeret = 25;
        public int aranyrogokSzama = 5;

        public bool VEGE = false;

        public Dictionary<Type, List<VarosElem>> elemekLista = new Dictionary<Type, List<VarosElem>>();

        Random rand = new Random();

        List<List<int>> szabadMezok = new List<List<int>>();

        public List<Bandita>tamadoBanditak = new List<Bandita>();

        public Varos()
        {
            szinbizbasz.Add("S", ConsoleColor.Blue);
            szinbizbasz.Add("W", ConsoleColor.DarkYellow);
            szinbizbasz.Add(".", ConsoleColor.DarkGray);
            szinbizbasz.Add("V", ConsoleColor.Magenta);
            szinbizbasz.Add("X", ConsoleColor.Green);
            szinbizbasz.Add("B", ConsoleColor.Red);
            szinbizbasz.Add("A", ConsoleColor.Yellow);

            for (int i = 0; i < palyaMeret; i++) {
                szabadMezok.Add(new List<int>());
                for (int j = 0; j < palyaMeret; j++)
                {
                    szabadMezok[i].Add(j);
                }
            }

            varoselemek = new VarosElem[palyaMeret, palyaMeret];
            setup();
        }

        public void setup()
        {
            elemekLista.Add(typeof(Barikad),elemGen(typeof(Barikad), barikadokSzama));
            elemekLista.Add(typeof(Varoshaza), elemGen(typeof(Varoshaza), 1));
            bejarhatoEllen();
            elemekLista.Add(typeof(Bandita), elemGen(typeof(Bandita), banditakSzama));
            elemekLista.Add(typeof(Whiskey), elemGen(typeof(Whiskey), whiskeySzama));
            elemekLista.Add(typeof(Aranyrog), elemGen(typeof(Aranyrog), aranyrogokSzama));
            elemekLista.Add(typeof(Seriff), elemGen(typeof(Seriff), 1));
            groundTolt();
        }

        public void groundTolt()
        {
            for (int i = 0; i < szabadMezok.Count; i++)
            {
                for (int j = 0; j < szabadMezok[i].Count; j++)
                {
                    varoselemek[i, szabadMezok[i][j]] = new Ground();
                }
            }
        }

        public List<VarosElem> elemGen(Type fajta,int mennyit)
        {

            List<VarosElem> elemek = new List<VarosElem>();
            List<int> nemUresX = new List<int>();
            for (int i = 0; i < mennyit; i++)
            {
                VarosElem elem = (VarosElem)Activator.CreateInstance(fajta);
                elemek.Add(elem);
                /*int x = rand.Next(0, palyaMeret);
                int y = rand.Next(0, palyaMeret);
                while (varoselemek[x, y].GetType() != typeof(Ground))
                {
                    x = rand.Next(0, palyaMeret);
                    y = rand.Next(0, palyaMeret);
                }*/
                nemUresX = new List<int>();
                for(int j = 0; j < szabadMezok.Count; j++)
                {
                    if (szabadMezok[j].Count != 0)
                    {
                        nemUresX.Add(j);
                    }
                }
                int x = nemUresX[rand.Next(0, nemUresX.Count)];
                int y = szabadMezok[x][rand.Next(0, szabadMezok[x].Count)];

                szabadMezok[x].Remove(y);

                elem.elemX = x;
                elem.elemY = y;
                varoselemek[x, y] = elem;
            }
            return elemek;
        }

        int[,] ellenorzottek;
        int ellenorizettekSzama = 1;

        public void bejarhatoEllen()
        {
            ellenorzottek = new int[palyaMeret, palyaMeret];
            ellenorizettekSzama = 1;
            ellenorzottek[0, 0] = 1;
            szomszedEllen(0, 0);
            if (ellenorizettekSzama != palyaMeret * palyaMeret)
            {
                Console.WriteLine("Elbaszódott " + ellenorizettekSzama);
                setup();
            }
        }

        public bool ragasztosE(VarosElem elem)
        {
            if (elem != null)
            {
                return elem.GetType() == typeof(Barikad) || elem.GetType() == typeof(Varoshaza);
            }
            return false;
        }

        public void szomszedEllen(int k, int v)
        {
            //0: nem voltunk rajta, 1: voltunk rajta, -1: kő
            for(int i = k-1; i < k+2; i++)
            {
                for (int j = v - 1; j < v + 2; j++)
                {
                    try
                    {
                        //Console.WriteLine((i,j));
                        //Console.WriteLine(ellenorzottek[i,j]);
                        if (i == k && j == v) continue;
                        if (ellenorzottek[i, j] == -1 || ellenorzottek[i, j] == 1)
                        {
                            continue;
                        }
                        if (ragasztosE(varoselemek[i, j]) == true)
                        {
                            ellenorzottek[i, j] = -1;
                            ellenorizettekSzama += 1;
                            continue;
                        }
                        if (ellenorzottek[i, j] == 0)
                        {
                            ellenorzottek[i, j] = 1;
                            ellenorizettekSzama += 1;
                            //Console.WriteLine("iufeIZFEWuziefwefw");
                            szomszedEllen(i,j);
                        }
                    }
                    catch
                    {

                    }

                }
            }
        }
        public override string ToString()
        {
            Console.ForegroundColor = ConsoleColor.White;
            //Console.Clear();
            vonalRajz();
            Console.WriteLine();
            for (int i = 0; i < varoselemek.GetLength(0); i++)
            {
                for (int j = 0; j < varoselemek.GetLength(1); j++)
                {
                    if (varoselemek[i,j] != null)
                    {
                        if (varoselemek[i, j].latott == true)
                        {
                            Console.ForegroundColor = szinbizbasz[varoselemek[i, j].ToString()];
                            /*if(varoselemek[i,j].banditaMellet == true)
                            {
                                //Console.WriteLine("ITTTT");
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                            }*/
                            Console.Write(varoselemek[i, j].ToString() + " ");
                            //Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            /*if (varoselemek[i, j].banditaMellet == true)
                            {
                                //Console.WriteLine("ITTTT");
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                            }*/
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(varoselemek[i, j].ToString() + " ");
                            //Console.BackgroundColor = ConsoleColor.Black;
                        }
                        
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("F ");
                    }
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
            vonalRajz();
            Seriff seriffElem = elemekLista[typeof(Seriff)][0] as Seriff;
            Console.WriteLine("Sheriff adatai:");
            Console.WriteLine("Életerő: " + seriffElem.elet+"             ");//Console.Cursor a kedvenc functionom
            Console.WriteLine("Aranyrögök: " + seriffElem.aranyRogok + "             ");
            Console.WriteLine("Ölések: " + seriffElem.olesek + "             ");
            vonalRajz();
            Console.WriteLine("Támadt: ");
            if (seriffElem.tamadt != null)
            {
                Console.WriteLine("Pozíció: "+(seriffElem.tamadt.elemX, seriffElem.tamadt.elemY) + " Élet: " + seriffElem.tamadt.elet + " Sebzés ellene: -"+seriffElem.sebzes+"               ");
                Thread.Sleep(1000);
            }
            vonalRajz();
            Console.WriteLine("Támadók: ");
            if (tamadoBanditak.Count > 0)
            {
                for (int i = 0; i < tamadoBanditak.Count; i++)
                {
                    Console.WriteLine("Pozíció: " + (tamadoBanditak[i].elemX, tamadoBanditak[i].elemY)+" Élet: "+ tamadoBanditak[i].elet+" Kapott sebzes: -" + tamadoBanditak[i].elozoSebzes+"                 ");
                }
                Thread.Sleep(1000);
            }
            vonalRajz();
            return null;
        }

        public void vonalRajz()
        {
            Console.WriteLine();
            for (int i = 0; i < palyaMeret; i++)
            {
                Console.Write("- ");
            }
            Console.WriteLine();
        }
    }
}
