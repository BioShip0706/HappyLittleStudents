using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyLittleStudents
{
    internal class Giocatore
    {
        public string nomeUtente;
        public Personaggio personaggio;
        public int cfuAccumulati;

        public List<CarteCFU> carteCFU = new List<CarteCFU>();
        public List<CarteOstacolo> carteOstacolo = new List<CarteOstacolo>();

        public Giocatore(string nomeUtente, Personaggio personaggio, int cfuAccumulati, List<CarteCFU> carteCFU, List<CarteOstacolo> carteOstacolo)
        {
            this.nomeUtente = nomeUtente;
            this.personaggio = personaggio;
            this.cfuAccumulati = cfuAccumulati;
            this.carteCFU = carteCFU;
            this.carteOstacolo = carteOstacolo;
        }

        public override string ToString()
        {
            string tutteCarteCFU = "";
            string tutteCarteOstacolo = "";
            string bonusumalusu = "";
            foreach (int bM in personaggio.bonusMalus)
            {
                bonusumalusu += bM + " ";
            }
            foreach (CarteCFU carteCFU in carteCFU)
            {
                tutteCarteCFU += carteCFU.nomeCarta + ", ";
            }
            foreach (CarteOstacolo carteOstacolo in carteOstacolo)
            {
                tutteCarteOstacolo += carteOstacolo.nomeCarta + ": " + carteOstacolo.tipoOstacolo + "| ";
            }

            return  "-----------------\n" 
                    + "Nome utente: " + nomeUtente 
                    + "\nPersonaggio scelto: " + personaggio.nomePersonaggio
                    + "\nCfu accumulati: " + cfuAccumulati
                    + "\nCarte CFU: " + tutteCarteCFU
                    + "\nCarte Ostacolo: " + tutteCarteOstacolo
                    + "\nBonus Malus: " + bonusumalusu 
                    + "\n-----------------";
        }

        public string AllCfuCards()
        {
            string tutteCarteCFU = "";
            foreach (CarteCFU cartaCFU in carteCFU)
            {
                tutteCarteCFU += cartaCFU.ToString();
            }
            return tutteCarteCFU;
        }

        public static string SortPlayersByCfu(List<Giocatore> giocatori)
        {
            string risultato = "";
            //int max = 100;
            for (int i = 0; i < giocatori.Count; i++)
            {
                for (int j = i + 1; j < giocatori.Count; j++)
                {
                    if (giocatori[i].cfuAccumulati < giocatori[j].cfuAccumulati) // 45,63,18,35
                    {
                        Giocatore playerSalvato = giocatori[i];
                        giocatori[i] = giocatori[j];
                        giocatori[j] = playerSalvato;
                    }
                }
            }

            for (int i = 0; i < giocatori.Count; i++)
            {
                string tipiOstacoli = "|";
                for(int j = 0; j < giocatori[i].carteOstacolo.Count; j++)
                {
                    tipiOstacoli += giocatori[i].carteOstacolo[j].tipoOstacolo + "|";
                }
               risultato += (i + 1) + ") " + giocatori[i].nomeUtente + ": " + giocatori[i].cfuAccumulati + " cfu! " + " Ostacoli: " + tipiOstacoli + "\n";

            }


            return risultato;
        }

        public string IndexedCFUCards()
        {
            string output = "";
            //do un indice alle carte in base al mazzo
            for (int i = 0; i < carteCFU.Count; i++)
            {
               output += "ID: " + (i+1) +"\n" +carteCFU[i].ToString() +"\n";
            }

            return output;
        }

        public string AllOstacoloCards()
        {
            string tutteCarteOstacolo = "";
            foreach (CarteOstacolo cartaOstacolo in carteOstacolo)
            {
                tutteCarteOstacolo += cartaOstacolo.ToString();
            }
            return tutteCarteOstacolo;
        }

        public bool CheckCFUPoints()  //controllo se ho almeno una carta cfu punto
        {
            //CarteCFU carta1 = new CarteCFU("carta1",0,Effetto.NESSUNO);
            //CarteCFU carta2 = new CarteCFU("carta2",0,Effetto.NESSUNO);
            //CarteCFU carta3 = new CarteCFU("carta3",0,Effetto.NESSUNO);
            //CarteCFU carta4 = new CarteCFU("carta4",8,Effetto.NESSUNO);
            //CarteCFU carta5 = new CarteCFU("carta5",0,Effetto.NESSUNO);
            //List<CarteCFU> cartee = new List<CarteCFU>();
            //cartee.Add(carta1);
            //cartee.Add(carta2);
            //cartee.Add(carta3);
            //cartee.Add(carta4);
            //cartee.Add(carta5);

            //bool cfuPointCard = false;
            //for (int i = 0; i < cartee.Count; i++)
            //{
            //    if (cartee[i].numCFU > 0)
            //    {
            //        cfuPointCard = true;
            //    }
            //}
            //Console.WriteLine(cfuPointCard);
            //return cfuPointCard;


            //bool cfuPointCard = false;
            for (int i = 0; i < carteCFU.Count; i++) // 5 carte con corrispettivi punti: 0 , 5 , 0 , 0 , 8
            {
                if (carteCFU[i].numCFU > 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal static string IndexedPlayers(List<Giocatore> giocatori)
        {
            string result = "";
            for (int i = 0; i < giocatori.Count; i++)
            {
                result += (i + 1) + ") " + giocatori[i].nomeUtente + "\n";
            }

            return result;
        }
    }
}
