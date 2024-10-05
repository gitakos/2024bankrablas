using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas
{
    internal class Bandita : VarosElem
    {
        public int aranyRogok = 0;
        public int elet = 100;
        public Bandita() {
        }
        Random rand = new Random();
        public (int, int) hovaLepjen(ref Varos varosElem)
        {
            (int, int) koviKoord = (elemX,elemY);
            List<(int, int)> validKoordok = new List<(int, int)>();
            for (int i = elemX - 1; i < elemX + 2; i++)
            {
                for (int j = elemY - 1; j < elemY + 2; j++)
                {
                    if ((i, j) == (elemX, elemY))
                    {
                        continue;
                    }
                    try
                    {
                       if(varosElem.varoselemek[i, j].GetType()==typeof(Aranyrog))
                        {
                            aranyRogok++;
                            koviKoord = (i, j);
                            break;
                        }
                        if (varosElem.varoselemek[i, j].GetType() == typeof(Seriff))
                        {
                            parbaj(varosElem.varoselemek[i, j] as Seriff,ref varosElem);
                            return (elemX,elemY);
                        }
                        if (varosElem.varoselemek[i, j].GetType() != typeof(Ground))
                        {
                            continue;
                        }
                        validKoordok.Add((i, j));
                    }
                    catch
                    {

                    }
                }
            }
            if (koviKoord == (elemX, elemY))
            {
                return validKoordok[rand.Next(validKoordok.Count)];
            }
            return koviKoord;

        }
        public void mozog(ref Varos varosElem)
        {
            (int, int) poz = hovaLepjen(ref varosElem);
            //Console.WriteLine("FUTOK");
            //Console.WriteLine(poz);
            //Console.WriteLine(poz);
            Ground ujFold = new Ground();
            ujFold.latott = this.latott;
            this.latott = varosElem.varoselemek[poz.Item1, poz.Item2].latott;
            varosElem.varoselemek[elemX, elemY] = ujFold;
            banditaMellettJelol(false, ref varosElem);
            elemX = poz.Item1;
            elemY = poz.Item2;
            varosElem.varoselemek[elemX, elemY] = this;
            //if(latott == true)
            //{
                //banditaMellettJelol(true, ref varosElem);
            //}
        }
        public void banditaMellettJelol(bool ertek,ref Varos varosElem)
        {
            for (int i = elemX - 1; i < elemX + 2; i++)
            {
                for (int j = elemY - 1; j < elemY + 2; j++)
                {
                    try
                    {
                        varosElem.varoselemek[i, j].banditaMellet = ertek;
                            //Console.WriteLine(varosElem.varoselemek[i, j].banditaMellet);
                    }
                    catch
                    {

                    }
                }
                //Console.WriteLine();
            }
        }
        public override string ToString()
        {
            return "B";
        }
        public void parbaj(Seriff ellenfel,ref Varos varosElem)
        {
            varosElem.tamadoBanditak.Add(this);
            ellenfel.elet -= rand.Next(5,15);
        }
    }
}
