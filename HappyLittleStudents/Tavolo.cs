using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyLittleStudents
{
    internal class Tavolo
    {
        public List<CarteCFU> CarteCfu;
        //public List<CarteOstacolo> CarteOstacolo;
        public CarteOstacolo cartaOstacolo;

        //public List<int> IDs; //per segnare l'ordine dei giocatori che hanno giocato

        public List<int> Cfus;

        public Tavolo()
        {
            CarteCfu = new List<CarteCFU>();
            //IDs = new List<int>();
            Cfus = new List<int>();
        }

        public Tavolo(List<CarteCFU> carteCfu, CarteOstacolo cartaOstacolo)
        {
            CarteCfu = carteCfu;
            this.cartaOstacolo = cartaOstacolo;
        }

        public string TableIndexedCFUCards()
        {
            string output = "";
            //do un indice alle carte in base al mazzo
            for (int i = 0; i < CarteCfu.Count; i++)
            {
                output += "ID: " + (i + 1) + "\n" + CarteCfu[i].ToString() + "\n";
            }

            return output;
        }
    }
}
