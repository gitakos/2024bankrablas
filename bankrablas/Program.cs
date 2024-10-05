using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bankrablas
{
    internal class Program
    {
        public static Varos vaross;
        public static Seriff seriffElem;
        public static int frameIdo = 500;
        static void Main(string[] args)
        {
            vaross = new Varos();
            seriffElem = (Seriff)vaross.elemekLista[typeof(Seriff)][0];
            seriffElem.szomszedFelfed(ref vaross);
            vaross.ToString();
            Thread.Sleep(frameIdo);
            Console.Clear();
            while (true) {
                szimulal();
                if(vaross.VEGE == true)
                {
                    break;
                }
            }
            Console.WriteLine("JÁTÉK VÉGE!");
            Console.ReadLine();
        }
        static float frameCounter = 1;
        static float banditaFrame = 1;
        static void szimulal()
        {
            vaross.tamadoBanditak = new List<Bandita>();
            if (frameCounter /1.5>banditaFrame)
            {
                banditaFrame++;
                //Console.WriteLine("FRAME");
                for (int i = 0; i < vaross.elemekLista[typeof(Bandita)].Count; i++)
                {
                    Bandita CSharpOlyanJo = vaross.elemekLista[typeof(Bandita)][i] as Bandita;
                    //Console.WriteLine((CSharpOlyanJo.elemX,CSharpOlyanJo.elemY));
                    CSharpOlyanJo.mozog(ref vaross);
                    //Console.WriteLine(CSharpOlyanJo.elet);
                }
                for (int i = 0; i < vaross.elemekLista[typeof(Bandita)].Count; i++)
                {
                    Bandita CSharpOlyanJo = vaross.elemekLista[typeof(Bandita)][i] as Bandita;
                    if(CSharpOlyanJo.latott == true)
                    {
                        CSharpOlyanJo.banditaMellettJelol(true, ref vaross);
                    }
                }
            }
            seriffElem.mozog(ref vaross);
            seriffElem.szomszedFelfed(ref vaross);
            vaross.ToString();
            Console.WriteLine("KÖR: "+frameCounter);
            Console.WriteLine("KÖRBANDITA: " + banditaFrame);
            Thread.Sleep(frameIdo);
            if (vaross.VEGE == false)
            {
                Console.Clear();
            }
            frameCounter++;
        }
    }
}
