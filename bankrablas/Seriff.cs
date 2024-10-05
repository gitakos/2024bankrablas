using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas
{
    internal class Seriff : VarosElem
    {
        Dictionary<Type, int> prior = new Dictionary<Type, int>() {};
        List<(int,int)> kozeliNemlatottak = new List<(int, int)>();
        int elozoX = -1;
        int elozoY = -1;

        public int aranyRogok = 0;
        public int elet = 100;
        public int olesek = 0;
        public int sebzes = 0;
        public Bandita tamadt;
        public override string ToString()
        {
            return "S";
        }
        public Seriff()
        {
            prior.Add(typeof(Bandita), -300);
            prior.Add(typeof(Varoshaza), 1);
            prior.Add(typeof(Barikad), 999);
            prior.Add(typeof(Whiskey), 1);
            prior.Add(typeof(Aranyrog), -400);
            prior.Add(typeof(Ground), 0);
            prior.Add(typeof(Seriff), 999);
            prior.Add(typeof(Varos),-200);//Új hely
            sebzes = random.Next(21, 35);
        }

        public void szomszedFelfed(ref Varos varosElem)
        {
            //0: nem voltunk rajta, 1: voltunk rajta, -1: kő
            //Console.WriteLine();
            for (int i = elemX - 1; i < elemX + 2; i++)
            {
                for (int j = elemY - 1; j < elemY + 2; j++)
                {
                    try
                    {
                        varosElem.varoselemek[i, j].latott = true;
                    }
                    catch
                    {

                    }
                }
                //Console.WriteLine();
            }
           // Console.WriteLine((elemX,elemY));
            //Console.WriteLine(  );
            for (int i = elemX - 3; i < elemX + 4; i++)
            {
                for (int j = elemY - 3; j < elemY + 4; j++)
                {
                    //Console.Write((i,j));
                    try
                    {
                        if(varosElem.varoselemek[i, j].latott == false&& !kozeliNemlatottak.Contains((i,j)))
                        {
                            kozeliNemlatottak.Add((i,j));
                        }
                        else if(varosElem.varoselemek[i, j].latott == true)
                        {
                            kozeliNemlatottak.Remove((i, j));
                        }
                    }
                    catch
                    {
                    }
                }
                //Console.WriteLine();
            }
        }
        public (int, int) iranyDont(ref Varos varosElem)
        {
            (int, (int, int)) legkisebb = (9999999, (-1,-1));
            foreach(KeyValuePair<Type,List<VarosElem>> kep in  varosElem.elemekLista)
            {
                if(kep.Key == typeof(Barikad))
                {
                    continue;
                }
                //Console.WriteLine(kep.Key+"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                for (int i = 0; i < kep.Value.Count; i++)
                {
                    //Console.WriteLine("LÁTOTT VIZSGÁLT: " + latottak[i]);
                    if (kep.Value[i].latott == false)
                    {
                        continue;
                    }
                    (int, int) poz = (kep.Value[i].elemX, kep.Value[i].elemY);
                    VarosElem elem = varosElem.varoselemek[poz.Item1, poz.Item2];
                    (int, (int, int)) cucc = palyaSulyoz(elem.elemX, elem.elemY, elemX, elemY,elem.GetType(),prior, ref varosElem);
                    if (cucc.Item1 <= legkisebb.Item1)
                    {
                        legkisebb = cucc;
                    }
                }
            }

            //Console.WriteLine(legkisebb);
            (int, (int, int)) nemlat = legkisebbNemlathato();
            (int, (int, int)) cucc2 = palyaSulyoz(nemlat.Item2.Item1, nemlat.Item2.Item2, elemX, elemY, typeof(Varos), prior, ref varosElem);
            if (cucc2.Item1 <= legkisebb.Item1)
            {
                legkisebb = cucc2;
            }
            //Console.WriteLine(legkisebb);
            return legkisebb.Item2;
        }

        public (int, (int, int)) legkisebbNemlathato()
        {
            (int, (int, int)) legkisebb = (9999999, (-1, -1));
            for (int i = 0;i<kozeliNemlatottak.Count;i++)
            {
                int tav = ketPontTavolsaga(kozeliNemlatottak[i].Item1, kozeliNemlatottak[i].Item2, elemX, elemY);
                if (tav < legkisebb.Item1)
                {
                    legkisebb = (tav, (kozeliNemlatottak[i].Item1, kozeliNemlatottak[i].Item2));
                }
            }
            return legkisebb;
        }

        public void mozog(ref Varos varosElem)
        {
            (int, int) poz = iranyDont(ref varosElem);
            bool elorement = tevesDont(varosElem.varoselemek[poz.Item1, poz.Item2],ref varosElem);

            if (elorement)
            {
                Ground ujFold = new Ground();
                ujFold.latott = true;
                ujFold.banditaMellet = varosElem.varoselemek[elemX, elemY].banditaMellet;
                //Console.WriteLine(varosElem.elemekLista[typeof(Aranyrog)].Count);
                varosElem.varoselemek[elemX, elemY] = ujFold;
                elozoX = elemX;
                elozoY = elemY;
                elemX = poz.Item1;
                elemY = poz.Item2;
                banditaMellet = varosElem.varoselemek[elemX, elemY].banditaMellet;
                varosElem.varoselemek[elemX, elemY] = this;
            }
        }
        public bool tevesDont(VarosElem elem,ref Varos varosElem)
        {
            //Console.WriteLine(elem.GetType());
            if (elet < 0)
            {
                varosElem.VEGE = true;
                return false;
            }
            if (elem is Whiskey)
            {
                randomWhiskeyGen(ref varosElem);
                if (elet > 50)
                {
                    elet = 100;
                }
                else
                {
                    elet += 50;
                }
                prior[typeof(Whiskey)] = 1;
                prior[typeof(Bandita)] = -300;
                return true;
            }
            if(elem is Bandita)
            {
                tamadt = elem as Bandita;
                parbaj(elem as Bandita, varosElem);
                if (elet < 30)
                {
                    prior[typeof(Bandita)] = 998;
                    prior[typeof(Whiskey)] = -500;
                }
                return false;
            }
            else
            {
                tamadt = null;
            }
            if(elem is Varoshaza)
            {
                varosElem.VEGE = true;
                return false;
            }
            if(elem is Barikad)
            {
                return false;
            }
            if (elem is Aranyrog)
            {
                aranyRogok += 1;
                return true;
            }
            if (elet < 50)
            {
                prior[typeof(Whiskey)] = -500;
            }
            if (aranyRogok == varosElem.aranyrogokSzama)
            {
                prior[typeof(Varoshaza)] = -999;
                prior[typeof(Bandita)] = 998;
            }
            return true;
        }
        public void randomWhiskeyGen(ref Varos varosElem)
        {
            List<(int, int)> szabadMezok = new List<(int, int)>();
            for(int i= 0; i < varosElem.varoselemek.GetLength(0); i++)
            {
                for (int j = 0; j < varosElem.varoselemek.GetLength(1); j++)
                {
                    if (varosElem.varoselemek[i,j].GetType() == typeof(Ground))
                    {
                        szabadMezok.Add((i, j));
                    }
                }
            }
            (int,int) poz = szabadMezok[random.Next(0,szabadMezok.Count)];
            Whiskey ujWhiskey = new Whiskey();
            ujWhiskey.elemX = poz.Item1;
            ujWhiskey.elemY = poz.Item2;
            ujWhiskey.latott = varosElem.varoselemek[poz.Item1,poz.Item2].latott;
            ujWhiskey.banditaMellet = varosElem.varoselemek[poz.Item1, poz.Item2].banditaMellet;
            varosElem.varoselemek[poz.Item1, poz.Item2] = ujWhiskey;
        }

        /*public void varosElemTorol(VarosElem elem,Varos varosElem)
        {
            if (elem is Whiskey)
            {
                int index = elemIndexKeres(varosElem.whiskeyk, elem);
                varosElem.whiskeyk.Remove()
            }
            if (elem is Bandita)
            {
                Console.WriteLine("Harcol");
                return false;
            }
            if (elem is Varoshaza)
            {
                Console.WriteLine("Elmenekült! (Városháza)");
                return true;
            }
            if (elem is Barikad)
            {
                Console.WriteLine("HIBA! (Barikádra lépés)");
                return false;
            }
            if (elem is Aranyrog)
            {
                aranyRogok += 1;
                return true;
            }
            if (elet < 50)
            {
                prior[typeof(Whiskey)] = -400;
            }
        }

        public int elemIndexKeres(List<VarosElem> lista, VarosElem elem)
        {
            for(int i = 0; i < lista.Count; i++)
            {
                if ((lista[i].elemX, lista[i].elemY) == (elem.elemX, elem.elemY))
                {
                    return i;
                }
            }
            return -1;
        }*/
        Random random = new Random();
        public (int,(int,int)) palyaSulyoz(int celpontX, int celpontY,int bazisX,int bazisY, Type celpontType,Dictionary<Type, int>prior,ref Varos varosElem)
        {
            int[,] sulyokCelpont = new int[3, 3];
            //Console.WriteLine();
            int legkisebb = 9999999;
            (int,int) legkisebbInd = (-1,-1);
            for (int i = bazisX - 1; i < bazisX + 2; i++)
            {
                for (int j = bazisY - 1; j < bazisY + 2; j++)
                {
                    //
                    //Console.WriteLine("");
                    try
                    {
                        Type elemType = varosElem.varoselemek[i, j].GetType();
                        //Console.Write((i - bazisX+1, j - bazisY + 1)+":");
                        int typeFaktor = varosElem.varoselemek[i, j].latott ? prior[elemType] : prior[typeof(Varos)];
                        int celpontFaktor = prior[celpontType];
                        if (varosElem.varoselemek[i, j].banditaMellet == true)
                        {
                            typeFaktor = prior[typeof(Bandita)];
                        }
                        int tav = ketPontTavolsaga(i, j, celpontX, celpontY);
                        sulyokCelpont[i - bazisX + 1, j - bazisY + 1] = tav +typeFaktor+ celpontFaktor;//Math.Sqrt(Math.Pow(i - celpontX, 2) + Math.Pow(j - celpontY, 2))
                        if (sulyokCelpont[i - bazisX + 1, j - bazisY + 1] <= legkisebb && (i, j) != (elozoX, elozoY))
                        {
                            legkisebbInd = (i, j);
                            legkisebb = sulyokCelpont[i - bazisX + 1, j - bazisY + 1];
                        }
                        //Console.Write(sulyokCelpont[i - bazisX + 1, j - bazisY + 1] + " = " +tav+" + "+typeFaktor+" + "+celpontFaktor);
                    }
                    catch
                    {
                        //Console.WriteLine((i - bazisX + 1, j - bazisY+1));
                        //Console.WriteLine((i,j));
                        //Console.WriteLine(sulyokCelpont[i - bazisX + 1, j - bazisY + 1]);
                        //Console.WriteLine("BAJ VAN ");
                    }
                }
                //Console.WriteLine();
            }

            return (legkisebb, legkisebbInd);
        }
        public int ketPontTavolsaga(int x1,int y1, int x2, int y2)
        {
            return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        public void parbaj(Bandita ellenfel,Varos varosElem)
        {
            ellenfel.elet -= sebzes;
            //Console.WriteLine("Ellenfél élete: "+ellenfel.elet);
            if(ellenfel.elet <= 0)
            {
                aranyRogok += ellenfel.aranyRogok;
                olesek += 1;

                Ground fos = new Ground();
                fos.latott = true;
                ellenfel.banditaMellettJelol(false,ref varosElem);
                varosElem.varoselemek[ellenfel.elemX, ellenfel.elemY] = fos;
                varosElem.elemekLista[typeof(Bandita)].RemoveAt(varosElem.elemekLista[typeof(Bandita)].FindIndex((c)=>c.elemX == ellenfel.elemX&& c.elemY == ellenfel.elemY));
                if (olesek == varosElem.banditakSzama)
                {
                    varosElem.VEGE = true;
                }
            }
        }
    }
}
