using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HappyLittleStudents
{
    internal class Program
    {

        static void Main(string[] args)
        {

            //Popolo tutte le carte CFU che esistono e le metto dentro questa lista, da cui pescherò per creare le mani dei giocatori
            List<CarteCFU> tutteCarteCFU = new List<CarteCFU>();
            PopolaTutteCarteCfu(tutteCarteCFU);
            //foreach(CarteCFU cartaCFU in tutteCarteCFU)
            //{
            //    Console.WriteLine(cartaCFU.ToString()); 
            //}

            //Popolo tutte le carte Ostacolo che esistono e le metto dentro questa lista, da cui pescherò per creare le mani dei giocatori
            List<CarteOstacolo> tutteCarteOstacolo = new List<CarteOstacolo>();
            PopolaTutteCarteOstacolo(tutteCarteOstacolo);
            //foreach (CarteOstacolo cartaOstacolo in tutteCarteOstacolo)
            //{
            //    Console.WriteLine(cartaOstacolo.ToString());
            //}

            List<Giocatore> giocatori = new List<Giocatore>(); //ECCO I 2-4 GIOCATORI
            //int playersCount;
            giocatori =  Fase1(ref tutteCarteCFU, ref tutteCarteOstacolo);
            //foreach(Giocatore player in giocatori)
            //{
            //    Console.WriteLine(player.ToString());
            //}
            

            foreach (Giocatore player in giocatori)
            {
                Console.WriteLine(player.ToString());
            }

            // ORA HO I GIOCATORI, POSSO INIZIARE
            List<CarteCFU> mazzoScarti = new List<CarteCFU>();
            //SalvaPartita(playersCount, giocatori, tutteCarteCFU, mazzoScarti, tutteCarteOstacolo);

            /*
             * All’inizio di ogni turno si dovrà stampare il numero del turno, dopodiché si dovrà estrarre una carta 
               ostacolo e si dovrà stampare il suo contenuto
             */


            //CONTROLLI PER IL SALVATAGGIO
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + @"\Salvataggi\";

            // Verifico se la directory esiste, altrimenti la creo
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string dataDiOggi = DateTime.Today.ToString("dd/MM/yyyy").Replace('/', '_');
            string saveFileName = "";
            while (true)
            {
                try
                {
                    Console.Write("Dare un nome al salvataggio: ");
                    saveFileName = Console.ReadLine() + "_" + dataDiOggi + ".txt";
                    if (File.Exists(directoryPath + saveFileName))
                    {
                        Console.WriteLine("Il file di salvataggio: " + saveFileName + " esiste già. Sovrascrivere?\n" + "1)Si\n2)No");
                        int overwrite = Convert.ToInt32(Console.ReadLine()) - 1;
                        if (overwrite == 0 || overwrite == 1)
                        {
                            if (overwrite == 0) //se non vuoi sovrascrivere, ripeti ciclo, se vuoi sovrascivere, cancella file
                            {
                                File.Delete(directoryPath + saveFileName);
                                break;
                            }
                            
                        }
                        else
                        {
                            throw new Exception();
                        }
                        
                    }
                    else
                    {
                        break;
                    }
                    
                }
                catch(Exception)
                {
                    Console.WriteLine("Inserisci dati corretti!, Riprova");
                }
            }


            //////////
            int numTurno = 1;
            while (giocatori.Count > 1)
            {
                Turno( ref numTurno, giocatori, tutteCarteCFU, tutteCarteOstacolo, mazzoScarti, saveFileName);
            }
            // I TURNI FUNZIONANO COSì
            /*
             *OGNI GIOCATORE HA 5 CARTE  - FATTO (CONTOLLARE SE NON HANNO SOLO CARTE PUNTO)
             *VIENE GIOCATO UN OSTACOLO e messo nel tavolo - FATTO
             *OGNI GIOCATORE GIOCA UNA CARTA 
             *TUTTE LE CARTE VENGONO GIRATE
             *SI CALCOLANO I PUNTEGGI IN BASE A BONUS E MALUS
             *ES CARTA OSTACOLO DINAUSAURO ROSSO STOMPED, CHI HA QUELLO NEL PERSONAGGIO OTTIENE IL BONUS O MALUS
             *SI ESEGUONO GLI EFFETTI DELLE CARTE GIOCATE
             *SE UNO VUOLE PUò GIOCARE UNA CARTA ISTANTANEA
             *SI GUARDANO I PUNTI CFU, CHI NE HA DI PIù VINCE IL TURNO
             *CHI HA FATTO MENO CFU DI TUTTI, COMPRESI TUTTI I BONUS MALUS ETC è IL PERDENTE E LA CARTA OSTACOLO VA NELLA SUA MANO
             *SI RIPESCA DAL MAZZO PER TORNARE A 5 CARTE
             *
             *PER VINCERE 60 CFU O TUTTI RINUNCIATO AGLI STUDI E RIMANI L'ULTIMO
             *PERDE CHI: 3 OSTACOLI STESSO TIPO, UN OSTACOLO DI OGNUNO DEI 4 TIPI
             *QUANDO MUORE LE SUE CARTE VENGONO SCARTATE, LE SUE CARTE OSTACOLO VENGONO RIMESSE IN FONDO AL MAZZO

             *CALCOLO PUNTEGGIO:
             *CARTA CFU
             *CALCOLO BONUS MALUS
             *EFFETTI CARTE
             *CARTE ISTANTANEE
             *ASSEGNA PUNTI
             */

        }

        private static void Turno(ref int numTurno, List<Giocatore> giocatori, List<CarteCFU> tutteCarteCFU, List<CarteOstacolo> tutteCarteOstacolo, List<CarteCFU> mazzoScarti, string saveFileName)
        {
            SalvaPartita(giocatori.Count, giocatori, tutteCarteCFU, mazzoScarti, tutteCarteOstacolo, saveFileName, numTurno); //DA MODIFICARE
            Console.WriteLine("\n----------------------" + "TURNO: " + numTurno + "----------------------\n");
            Random random = new Random();
            int randomNumber = 0;
            
            Tavolo tavolo = new Tavolo();

            CarteOstacolo ostacoloEstratto; //Estraggo un ostacolo e lo metto nel tavolo!
            randomNumber = random.Next(0, tutteCarteOstacolo.Count);  //Estraggo un ostacolo e lo metto nel tavolo!
            ostacoloEstratto = tutteCarteOstacolo[randomNumber]; //Estraggo un ostacolo e lo metto nel tavolo!
            tutteCarteOstacolo.RemoveAt(randomNumber); //Rimuovo l'ostacolo estratto

            tavolo.cartaOstacolo = ostacoloEstratto;

            Console.WriteLine("E' STATO ESTRATTO L'OSTACOLO:");
            Console.WriteLine(tavolo.cartaOstacolo.ToString());

            //Console.WriteLine("\n----------------------" + "SI GIOCA UNA CARTA A TESTA");
            Console.WriteLine("\nGIOCATORI IN PARTITA: ");
            for (int i = 0; i < giocatori.Count; i++)
            {
                Console.WriteLine("ID: " + i + " - " + giocatori[i].nomeUtente); //stampo id e nomi giocatori
            }

            /*
                Durante il turno i giocatori potranno, a turno, eseguire le seguenti azioni:
                [TASTO 1] giocare una carta CFU dalla propria mano 
                [TASTO 2] controllare lo stato degli altri giocatori (CFU e carte ostacolo)
                [TASTO 0] uscire dalla partita
                Di default tutte le informazioni relative al giocatore che deve effettuare la scelta sono visualizzate a 
                schermo (nome, info personaggio, carte etc…)
             */

            //List<CarteCFU> tavolo = new List<CarteCFU>();
            //Tavolo tavoloIntero = new Tavolo();

            for (int i = 0; i < giocatori.Count; i++)
            {
                //Console.Write("Inserire ID giocatore che vuole giocare una carta punto: ");
                //int idGiocatore = Convert.ToInt32(Console.ReadLine());
                //Console.WriteLine("SCEGLI UNA DELLE 3 OPZIONI:" +
                //    "\n0: Gioca una carta CFU" +
                //    "\n1: Controlla stato di altri giocatori" +
                //    "\n2: Esci dalla partita (ABBANDONA)");

                string avanti = "no";
                //int scelta = Convert.ToInt32(Console.ReadLine());
                while (avanti == "no") //chiedo l'opzione e ripeto fino a quando non gioca o esce dalla partita
                {
                    Console.WriteLine("\n" + giocatori[i].nomeUtente + ", SCEGLI UNA DELLE 3 OPZIONI:" +
                    "\n0: Gioca una carta CFU" +
                    "\n1: Controlla stato di altri giocatori" +
                    "\n2: Esci dalla partita (ABBANDONA)");
                    try
                    {
                        Console.Write("ID scelta: ");
                        int scelta = Convert.ToInt32(Console.ReadLine());
                        switch (scelta)
                        {
                            case 0: //Gioca una carta CFU

                                //giocatori[i].CheckCFUPoints(); //controllo se ha almeno una carta punto
                                CfuERipescaggio(giocatori[i], tutteCarteCFU, tutteCarteOstacolo); //metodo che controlla se il giocatore ha almeno una carta cfu, altrimenti svuota e ripesca.
                                Console.WriteLine("\nTutte le carte CFU di " + giocatori[i].nomeUtente + ": ");
                                Console.WriteLine(giocatori[i].IndexedCFUCards());
                                bool idCartaGioco = false;
                                while (idCartaGioco == false)
                                {
                                    try
                                    {
                                        Console.Write("Inserisci ID carta che vuoi giocare: ");
                                        int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;

                                        if (CartaGiocabileCheck(giocatori[i].carteCFU[idCarta].effetto, false)) //se non ha 0 CFU si può giocare
                                        {
                                            tavolo.CarteCfu.Add(giocatori[i].carteCFU[idCarta]); //perchè le carte sono 5, con id 0-4. Le stampo da 1-5 ma devo togliere per scegliere realmente quella
                                                                                                 //tavolo.IDs.Add(i); //aggiungo il suo id alla lista di id //un po inutile
                                            Console.WriteLine("Hai scelto la carta: " + giocatori[i].carteCFU[idCarta].nomeCarta);

                                            tavolo.Cfus.Add(giocatori[i].carteCFU[idCarta].numCFU); //aggiungo i cfu base di quella carta al tavolo

                                            //chiamare metodo per calcolare punteggio


                                            giocatori[i].carteCFU.RemoveAt(idCarta); //tolgo la carta dalla sua mano perchè l'ha giocata
                                            avanti = "si";
                                            idCartaGioco = true;

                                        }
                                        else
                                        {
                                            Console.WriteLine("Questa carta non può  essere giocata ora!");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Inserisci un id valido!,Riprova");
                                    }
                                }
                                break;

                            case 1: //Controlla stato di altri giocatori
                                    //devo prendere i ToString di tutti i giocatori tranne se stesso
                                string output = "";
                                for (int j = 0; j < giocatori.Count; j++)
                                {
                                    if (i != j)
                                    {
                                        output += giocatori[j].ToString();
                                    }
                                }
                                Console.WriteLine("\n" + output);
                                break;

                            case 2: //Esci dalla partita (ABBANDONA)
                                bool abbandonaCiclo = false;
                                while (abbandonaCiclo == false)
                                {
                                    try
                                    {
                                        Console.WriteLine("\nSei sicuro di voler abbandonare la partita?\n0: ANNULLA \n1: CONFERMA");
                                        Console.Write("ID scelta: ");
                                        int abbandona = Convert.ToInt32(Console.ReadLine());
                                        if (abbandona == 1)
                                        {
                                            Console.WriteLine("\n" + giocatori[i].nomeUtente + " ha abbandonato la partita!"); //prima stampo perchè poi non lo trova più visto che lo tolgo dalla lista
                                            mazzoScarti.AddRange(giocatori[i].carteCFU); //metto le carte Cfu del giocatore dentro il mazzo scarti
                                            tutteCarteOstacolo.AddRange(giocatori[i].carteOstacolo); //metto le carte ostacolo sue di nuovo dentro il mazzo ostacoli
                                            giocatori.Remove(giocatori[i]); //cancella quel giocatore dalla lista
                                            avanti = "si";
                                            if (giocatori.Count == 1)
                                            {
                                                i = 0; //Resetto la i perchè se c'è solo un giocatore ha vinto l'unico rimasto: ovvero in posizione 0 
                                                Console.WriteLine("\n" + giocatori[i].nomeUtente + " ha vinto la partita!"); //potevo fare anche giocatori[0].nomeUtente
                                                Environment.Exit(0);
                                                return;
                                            }
                                            abbandonaCiclo = true;
                                            i--; //così l'indice diventa -1 e al prossimo giro diventa 0, ritornando al primo giocatore che c'è 
                                        }
                                    }
                                    catch(Exception)
                                    {
                                        Console.WriteLine("Inserisci un id corretto, Riprova!");
                                    }
                                }
                                break;
                        }
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("\nInserisci un id scelta corretto!, Riprova");
                    }
                }
            }

            //dopo che tutti giocano le carte continuare qua
            Console.WriteLine("\n" + "Le carte giocate vengono ora girate!");
            foreach (CarteCFU cartaCFU in tavolo.CarteCfu)
            {
                Console.WriteLine(cartaCFU.ToString());
            }
            Console.WriteLine("\n");

            /*CALCOLO PUNTEGGIO:
            *CARTA CFU
            * CALCOLO BONUS MALUS
             *EFFETTI CARTE
             * CARTE ISTANTANEE
             * ASSEGNA PUNTI
             */

            CalcoloCFUBonusMalus(giocatori, tavolo); //CALCOLO BONUS MALUS

            bool OrdinaPerCfuTavolo = false;
            OrdinaPerCFu(ref giocatori, tavolo, OrdinaPerCfuTavolo); //ORDINO CARTE,PUNTI,GIOCATORI PER CFU DELLE CARTE GIOCATE

            bool doppioe = false;
            EffettiECalcolo(giocatori,tavolo, tutteCarteCFU, tutteCarteOstacolo, mazzoScarti, ref doppioe); //EFFETTI DELLE CARTE


            //OrdinaPerCFu(ref giocatori,tavolo); //mi sa che è inutile riordinare di nuovo

            CarteIstantanee(giocatori, tavolo,tutteCarteCFU,tutteCarteOstacolo,mazzoScarti, ref doppioe); //GIOCO E EFFETTI CARTE ISTANTANEE

            OrdinaPerCfuTavolo = true;
            OrdinaPerCFu(ref giocatori, tavolo, OrdinaPerCfuTavolo); //ORDINO CARTE,PUNTI,GIOCATORI PER CFU DEL TAVOLO

            ControlloPunteggi(giocatori, tavolo,tutteCarteCFU,tutteCarteOstacolo,mazzoScarti); //CONTROLLO SUI PAREGGI

            ControlloOstacoliGiocatori(giocatori, tavolo, tutteCarteCFU, tutteCarteOstacolo, mazzoScarti);
            //for (int i = 0; i < tavolo.Count; i++)
            //{
            //    giocatori[i].cfuAccumulati += tavolo[i].numCFU; //metto i cfu al giocatore in base alla carta in posizione 0 del tavolo
            //}
            numTurno++;

            int nvincitori = 0;
            foreach(Giocatore player in giocatori)
            {
                if(player.cfuAccumulati >= 60)
                {
                    Console.WriteLine(player.nomeUtente + " hai vinto la partita!. Totalizzando " + player.cfuAccumulati + " CFU");
                    nvincitori++;
                    
                }
            }
 
            Console.WriteLine("\n----------------------------------\nCLASSIFICA GIOCATORI");
            Console.WriteLine(Giocatore.SortPlayersByCfu(giocatori));
            if(nvincitori >= 1)
            {
                Environment.Exit(0);
            }

        }

        private static void ControlloOstacoliGiocatori(List<Giocatore> giocatori, Tavolo tavolo, List<CarteCFU> tutteCarteCFU, List<CarteOstacolo> tutteCarteOstacolo, List<CarteCFU> mazzoScarti)
        {
            //int nOstacoli = 0;
            //int ripetizioni = 0;
            int nStudio = 0;
            int nSopravvivenza = 0;
            int nSociale = 0;
            int nEsame = 0;
            bool treCarteUguali; 
            bool dueCarteUgualiUnEsame;
            bool treCarteDiverse;
            bool dueCarteDiverseEsame;
            //Controllare prima che qualcuno non abbia gli ostacoli per perdere
            for (int i = 0; i < giocatori.Count; i++) //4 giocatori, controllo gli ostacoli
            {
                 nStudio = 0;
                 nSopravvivenza = 0;
                 nSociale = 0;
                 nEsame = 0;
                 treCarteUguali = false;
                 dueCarteUgualiUnEsame = false;
                 treCarteDiverse = false;
                 dueCarteDiverseEsame = false;

                foreach (CarteOstacolo ostacolo in giocatori[i].carteOstacolo)
                {
                    if (ostacolo.tipoOstacolo == TipoOstacolo.STUDIO)
                    {
                        nStudio++;
                    }
                    if (ostacolo.tipoOstacolo == TipoOstacolo.SOPRAVVIVENZA)
                    {
                        nSopravvivenza++;
                    }
                    if (ostacolo.tipoOstacolo == TipoOstacolo.SOCIALE)
                    {
                        nSociale++;
                    }
                    if (ostacolo.tipoOstacolo == TipoOstacolo.ESAME)
                    {
                        nEsame++;
                    }
                }

                treCarteUguali = nStudio >= 3 || nSopravvivenza >= 3 || nSociale >= 3 || nEsame >= 3;
                dueCarteUgualiUnEsame = (nStudio >= 2 && nEsame >= 1) || (nSopravvivenza >= 2 && nEsame >= 1) || (nSociale >= 2 && nEsame >= 1);
                treCarteDiverse = nStudio >= 1 && nSopravvivenza >= 1 && nSociale >= 1;
                dueCarteDiverseEsame = (nStudio >= 1 && nSopravvivenza >= 1 && nEsame >= 1) ||
                                         (nStudio >= 1 && nSociale >= 1 && nEsame >= 1) ||
                                         (nSopravvivenza >= 1 && nSociale >= 1 && nEsame >= 1);

                if (treCarteUguali || dueCarteUgualiUnEsame || treCarteDiverse ||dueCarteDiverseEsame)
                {
                    Console.WriteLine(giocatori[i].nomeUtente + " hai perso la partita a causa degli ostacoli accumulati!, Diventi spettatore!");
                    mazzoScarti.AddRange(giocatori[i].carteCFU);
                    giocatori.Remove(giocatori[i]);
                    i--;
                    //tavolo.Cfus.RemoveAt(i);
                    

                }

                if (giocatori.Count == 1)
                {
                    i = 0; //Resetto la i perchè se c'è solo un giocatore ha vinto l'unico rimasto: ovvero in posizione 0 
                    Console.WriteLine("\n" + giocatori[0].nomeUtente + " ha vinto la partita!"); //potevo fare anche giocatori[0].nomeUtente
                    Environment.Exit(0);
                }
            }

            for (int i = 0; i < giocatori.Count; i++)
            {
                giocatori[i].cfuAccumulati += giocatori[i].carteOstacolo.Count;
                Console.WriteLine("Il giocatore " + giocatori[i].nomeUtente + " avanza di " + giocatori[i].carteOstacolo.Count + " punti per il numero di ostacoli accumulati");
            }
            return;
        }

        private static void CfuERipescaggio(Giocatore giocatore, List<CarteCFU> tutteCarteCfu, List<CarteOstacolo> tutteCarteOstacolo)
        {
            Console.WriteLine("Verifico se hai almeno una carta cfu!");
            bool riprova = false;

            //foreach (CarteCFU carta in giocatore.carteCFU)
            //{
            //    carta.effetto = Effetto.SCAMBIAC;
            //    carta.numCFU = 0;
            //}


            while (riprova == false)
            {
                bool pesca = false;
                bool carteCfuPunto = false;

                if (giocatore.carteCFU.Count < 5) //se il giocatore ha meno di 5 carte pesca per tornare a 5
                {
                    pesca = true;
                }

                for (int i = 0; i < giocatore.carteCFU.Count; i++)
                {
                    if (giocatore.carteCFU[i].numCFU > 0) //questo evita anche che il giocatore non abbia solo carte istantanee
                    {
                        //Console.WriteLine("Hai almeno una carta punto!");
                        carteCfuPunto = true;
                        //return;
                    }
                }

                if (pesca == false && carteCfuPunto == true) //se ho 5 carte e almeno una cfu punto
                {
                    Console.WriteLine("Hai 5 carte e almeno una carta punto!, Proseguo");
                    return;
                }
                else if (pesca == false && carteCfuPunto == false) //se ho 5 carte ma nessuna cfu punto
                {
                    Console.WriteLine("Hai 5 carte ma nessuna cfu punto!, Svuoto la mano e ne prendo altre 5");
                    tutteCarteCfu.AddRange(giocatore.carteCFU); //metto tutte le carte cfu del giocatore nel mazzo e poi ripesco da qua
                    MescolaMazzi(ref tutteCarteCfu, ref tutteCarteOstacolo); // non so se rimescolare
                    giocatore.carteCFU.Clear(); //svuoto la mano perchè se ho 5 carte ma nessuna è cfu punto scarto tutto per prenderne altre 5
                }
                
                Console.WriteLine("Ripesco fino a quando  non ho almeno 5 carte");
                while (giocatore.carteCFU.Count != 5) //ripesca fino a quando  hai 5
                {
                    int indiceCasuale;
                    Random random = new Random();

                    indiceCasuale = random.Next(0, tutteCarteCfu.Count); //prendi un indice casuael
                    giocatore.carteCFU.Add(tutteCarteCfu[indiceCasuale]); //inserisci la carta con quell'indice dal mazzo normale a quello random
                    tutteCarteCfu.RemoveAt(indiceCasuale); //togli la carta dal mazzo normale

                }
               

               
                //giocatore.carteCFU.Clear(); //svuoto la sua mano

                //Ripesco fino a tornare a 5 carte;
                
            }

        }

        private static void ControlloPunteggi(List<Giocatore> giocatori, Tavolo tavolo, List<CarteCFU> tutteCarteCFU, List<CarteOstacolo> tutteCarteOstacolo, List<CarteCFU> mazzoScarti)
        {
            Console.WriteLine("Controllo sui punteggi....");

            //int punteggioMax = tavolo.Cfus[tavolo.Cfus.Count - 1];
            //int punteggioMin = tavolo.Cfus[0];
            int punteggioMax = tavolo.Cfus[0];
            int punteggioMin = tavolo.Cfus[tavolo.Cfus.Count - 1];
            List<Giocatore> ListaVincenti = new List<Giocatore>();
            List<Giocatore> ListaPerdenti = new List<Giocatore>();

            bool tuttiPareggio = false;
            for(int i = 1; i < giocatori.Count; i++)
            {
                if (tavolo.Cfus[0] == tavolo.Cfus[i]) // 12, 15, 7, 4
                {
                    tuttiPareggio = true;
                }
                else
                {
                    tuttiPareggio = false;
                    break;
                }
            }
            if(tuttiPareggio)
            {
                Console.WriteLine("Tutti i giocatori hanno totalizzato lo stesso punteggio Cfu, Nessuno avanza!");
                //Console.WriteLine("L'ostacolo " + tavolo.cartaOstacolo.nomeCarta +  " torna in fondo al mazzo!");
                if (tavolo.cartaOstacolo != null)
                {
                    //ListaPerdenti[0].carteOstacolo.Add(tavolo.cartaOstacolo);
                    tutteCarteOstacolo.Add(tavolo.cartaOstacolo);
                    Console.WriteLine("L'ostacolo " + tavolo.cartaOstacolo.nomeCarta + " torna in fondo al mazzo!");
                    tavolo.cartaOstacolo = null;
                }
                else
                {
                    Console.WriteLine("L'ostacolo non torna nel mazzo a causa delle carte istantanee! / Nessun ostacolo rimasto nel tavolo");
                }
                return;
            }

            for(int i = 0; i < giocatori.Count; i++)
            {
                if (tavolo.Cfus[i] == punteggioMax)
                {
                    ListaVincenti.Add(giocatori[i]);
                }
                if (tavolo.Cfus[i] == punteggioMin)
                {
                    ListaPerdenti.Add(giocatori[i]);
                }
            }


            if(ListaVincenti.Count > 1)
            {
                Console.WriteLine("Risultano più giocatori vincenti!.\nTutti i vincenti ottengono " + punteggioMax + " punti!");
                foreach(Giocatore vincente in ListaVincenti)
                {
                    vincente.cfuAccumulati += punteggioMax;
                    Console.WriteLine(vincente.nomeUtente + " ha ora un punteggio di " + vincente.cfuAccumulati + " !");
                }
            }
            else //Vincitore è uno solo e avanza
            {
                Console.WriteLine(ListaVincenti[0].nomeUtente + " è il vincitore del turno! e ottiene " + punteggioMax + " punti!");
                ListaVincenti[0].cfuAccumulati += punteggioMax;

            }


            if(ListaPerdenti.Count > 1)
            {
                Console.WriteLine("Risultano più perdenti.\nI perdenti si sfideranno ad uno spareggio. Vincerà colui che giocherà la carta con maggior numero di CFU!\n");
                SpareggioPerdenti(giocatori,tavolo,ref ListaPerdenti);
                
            }
            else //Il perdente è uno e si becca l'ostacolo
            {
                Console.WriteLine(ListaPerdenti[0].nomeUtente + " è il perdente di questo turno");
                if (tavolo.cartaOstacolo != null)
                {
                    ListaPerdenti[0].carteOstacolo.Add(tavolo.cartaOstacolo);
                    Console.WriteLine("L'ostacolo " + tavolo.cartaOstacolo.nomeCarta + " finisce nella mano di " + ListaPerdenti[0].nomeUtente);
                   // ListaPerdenti[0].cfuAccumulati += punteggioMin;
                    tavolo.cartaOstacolo = null;
                }
                else
                {
                    Console.WriteLine(ListaPerdenti[0].nomeUtente + " non ottiene l'ostacolo a causa delle carte istantanee! / Nessun ostacolo rimasto nel tavolo");
                    //ListaPerdenti[0].cfuAccumulati += punteggioMin;
                }
            }

        }

        private static void SpareggioPerdenti(List<Giocatore> giocatori, Tavolo tavolo, ref List<Giocatore> listaPerdenti) //nella lista perdenti ci possono essere min 2 e max 3 perdenti.
        {
            //List<CarteCFU> carteDeiPerdenti = new List<CarteCFU>();
            //Tavolo tavoloPerdenti = new Tavolo(new List<CarteCFU>(), tavolo.cartaOstacolo,new List<int>());
            bool continuaa = false;
            while (continuaa == false) //continua fino a quando un perdente non gioca una carta maggiore degli altri perdenti, o qualcuno finisce le carte punto.
            {
                List<CarteCFU> carteDeiPerdenti = new List<CarteCFU>();
                foreach (Giocatore perdente in listaPerdenti) //3 perdenti
                {
                    bool noCfu = true; //non ha carte Cfu punto rimaste? Vero
                    for (int i = 0; i < perdente.carteCFU.Count; i++) //verifico  tutte le carte della sua mano
                    {
                        if (perdente.carteCFU[i].numCFU > 0)
                        {
                            noCfu = false; //ha almeno una carta cfu punto rimasta
                            break;
                        }

                    }
                    if (noCfu == true) //se uno dei perdenti non ha carte cfu punto nel mazzo perde subito
                    {
                        Console.WriteLine(perdente.nomeUtente + " perde il turno!. Nessuna carta cfu punto rimasta nella mano!");
                        if (tavolo.cartaOstacolo != null)
                        {
                            perdente.carteOstacolo.Add(tavolo.cartaOstacolo);
                            Console.WriteLine("L'ostacolo " + tavolo.cartaOstacolo.nomeCarta + " finisce nella mano di " + perdente.nomeUtente);
                            tavolo.cartaOstacolo = null;
                        }
                        else
                        {
                            Console.WriteLine(perdente.nomeUtente + " non ottiene l'ostacolo a causa delle carte istantanee! / Nessun ostacolo rimasto nel tavolo");
                        }
                        return;
                    }

                    Console.WriteLine(perdente.nomeUtente + " gioca una carta cfu punto!");
                    Console.WriteLine(perdente.IndexedCFUCards());
                    bool continua = false;
                    while (continua == false)
                    {
                        try
                        {
                            Console.WriteLine("Inserisci id carta che vuoi giocare, CFU PUNTO!");
                            int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;
                            if (perdente.carteCFU[idCarta].numCFU > 0)
                            {
                                carteDeiPerdenti.Add(perdente.carteCFU[idCarta]);
                                perdente.carteCFU.RemoveAt(idCarta);
                                //tavoloPerdenti.CarteCfu.Add(perdente.carteCFU[idCarta]);
                                continua = true;
                            }
                            else
                            {
                                Console.WriteLine("La carta selezionata deve avere più di 0 cfu!, Riprova");
                            }
                        }
                        catch (Exception)
                        {

                            Console.WriteLine("Inserisci un id valido!, Riprova");
                        }

                    }
                }

                //CONTROLLARE QUALE è L'ID DEL GIOCATORE PERDENTE
                int idPerdente = 0;
                int min = carteDeiPerdenti[0].numCFU; //hardcoded. non è detto che il max sia 9 // Sistemato e impostato sul primo
                bool tuttiUguali = true;
                for (int i = 0; i < carteDeiPerdenti.Count; i++)//facciamo che sono 7,4,9 ////// 8,8,8  /// 7,8,8 //Funziona ora
                {
                    if (carteDeiPerdenti[i].numCFU != min) //per assicurarmi che non siano tutte uguali
                    {
                        tuttiUguali = false;
                    }

                    if (carteDeiPerdenti[i].numCFU < min)
                    {
                        min = carteDeiPerdenti[i].numCFU;
                        idPerdente = i;
                    }
                }

                if (tuttiUguali)
                {
                    Console.WriteLine("Tutti i giocatori hanno giocato una carta con lo stesso punteggio!, riprovare!");
                    carteDeiPerdenti.Clear();
                    

                }
                else
                {

                    Console.WriteLine(listaPerdenti[idPerdente].nomeUtente + " perde il turno!. Nessuna carta cfu punto rimasta nella mano!");
                    if (tavolo.cartaOstacolo != null)
                    {
                        listaPerdenti[idPerdente].carteOstacolo.Add(tavolo.cartaOstacolo);
                        Console.WriteLine("L'ostacolo " + tavolo.cartaOstacolo.nomeCarta + " finisce nella mano di " + listaPerdenti[idPerdente].nomeUtente);
                        tavolo.cartaOstacolo = null;
                    }
                    else
                    {
                        Console.WriteLine(listaPerdenti[idPerdente].nomeUtente + " non ottiene l'ostacolo a causa delle carte istantanee! / Nessun ostacolo rimasto nel tavolo");
                    }
                    return;
                }
            }
        }

        private static void CarteIstantanee(List<Giocatore> giocatori, Tavolo tavolo, List<CarteCFU> tutteCarteCFU, List<CarteOstacolo> tutteCarteOstacolo, List<CarteCFU> mazzoScarti, ref bool doppioe)
        {
            //tavolo.CarteCfu.Clear(); //così rimangono le istantanee
            for (int i = 0; i < giocatori.Count; i++)
            {
                bool continua1 = false;
                Console.WriteLine("Tutte le carte di " + giocatori[i].nomeUtente);
                Console.WriteLine(giocatori[i].IndexedCFUCards());
                while (continua1 == false)
                {
                    try
                    {

                        Console.WriteLine(giocatori[i].nomeUtente + " : Vuoi giocare una carta istantanea? \n0)No\n1)Si");
                        int scelta = Convert.ToInt32(Console.ReadLine());

                        if (scelta == 0 || scelta == 1)
                        {
                            continua1 = true; //scelta giusta

                            if (scelta == 1)
                            {
                                bool continua2 = false;


                                while (continua2 == false)
                                {
                                    //Console.WriteLine(giocatori[i].IndexedCFUCards());
                                    try
                                    {
                                        Console.WriteLine("Inserisci id della carta istantanea che vuoi giocare");
                                        int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;
                                        if (CartaGiocabileCheck(giocatori[i].carteCFU[idCarta].effetto, true))
                                        {
                                            //try
                                            //{

                                                //Gioca carta
                                                //tavolo.CarteCfu.Clear(); //così rimangono le istantanee
                                                //tavolo.CarteCfu.Add(giocatori[i].carteCFU[idCarta]);
                                                Giocatore giocatore = giocatori[i];
                                                //tavolo.Cfus[i] = 1;

                                                if (giocatore.carteCFU[idCarta].effetto == Effetto.SALVA)
                                                {
                                                    int max = 20;
                                                    int indiceMax = 0;
                                                    for (int j = 0; j < giocatori.Count; j++) //cercare il massimo
                                                    {
                                                        if (tavolo.Cfus[j] < max) // 9,5,7,2
                                                        {
                                                            max = tavolo.Cfus[j];
                                                            indiceMax = j;
                                                        }

                                                    }

                                                    if (i != indiceMax) //se non sono il giocatore con il punteggio più basso
                                                    {
                                                        bool avanti = false;
                                                        while (avanti == false)
                                                        {
                                                            try
                                                            {
                                                                Console.WriteLine("Non sei il giocatore con il minor punteggio!\n1)Scegli un altra carta\n2)Vai avanti");
                                                                int sceltaSalva = Convert.ToInt32(Console.ReadLine()) - 1;
                                                                if (sceltaSalva == 0)
                                                                {
                                                                    continue;
                                                                }
                                                                if(sceltaSalva == 1)
                                                                { 
                                                                    continua2 = true;
                                                                    avanti = true;
                                                                    break;
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {
                                                            Console.WriteLine("Inserisci un id corretto, Riprova!");
                                                            }
                                                        }
                                                    }
                                                    else //Se sono il giocatore con il punteggio più basso
                                                    {
                                                        EffettiIstantanei(giocatori, tavolo, tutteCarteCFU, tutteCarteOstacolo, mazzoScarti, ref doppioe, giocatore, giocatore.carteCFU[idCarta]);
                                                        giocatori[i].carteCFU.RemoveAt(idCarta);
                                                        return;
                                                    }
                                                }
                                                else if (giocatore.carteCFU[idCarta].effetto == Effetto.DIROTTA)
                                                {
                                                    int max = 20;
                                                    int indiceMax = 0;
                                                    for (int j = 0; j < giocatori.Count; j++) //cercare il massimo
                                                    {
                                                        if (tavolo.Cfus[j] < max) // 9,5,7,2
                                                        {
                                                            max = tavolo.Cfus[j];
                                                            indiceMax = j;
                                                        }

                                                    }

                                                    if (i != indiceMax) //se non sono il giocatore con il punteggio più basso
                                                    {
                                                        bool avanti = false;
                                                        while (avanti == false)
                                                        {
                                                            try
                                                            {
                                                                Console.WriteLine("Non sei il giocatore con il minor punteggio!\n1)Scegli un altra carta\n2)Vai avanti");
                                                                int sceltaSalva = Convert.ToInt32(Console.ReadLine()) - 1;
                                                                if (sceltaSalva == 0)
                                                                {
                                                                    continue;
                                                                }
                                                                if (sceltaSalva == 1)
                                                                {
                                                                    continua2 = true;
                                                                    avanti = true;
                                                                    break;
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {
                                                                Console.WriteLine("Inserisci un id corretto, Riprova!");
                                                            }
                                                    }
                                                    }
                                                    else //Se sono il giocatore con il punteggio più basso
                                                    {
                                                        EffettiIstantanei(giocatori, tavolo, tutteCarteCFU, tutteCarteOstacolo, mazzoScarti, ref doppioe, giocatore, giocatore.carteCFU[idCarta]);
                                                        giocatori[i].carteCFU.RemoveAt(idCarta);
                                                        return;
                                                    }
                                                }

                                                //Elaborazione effetti istantanei standard che non sono SALVA o DIROTTA
                                                EffettiIstantanei(giocatori, tavolo, tutteCarteCFU, tutteCarteOstacolo, mazzoScarti, ref doppioe, giocatore, giocatore.carteCFU[idCarta]);
                                                giocatori[i].carteCFU.RemoveAt(idCarta);

                                                //Chiedere se vuoi giocarne un altra, e poi mettere continua 
                                                bool continuaAltraCarta = false;
                                                while (continuaAltraCarta == false)
                                                {
                                                    try
                                                    {
                                                        Console.WriteLine(giocatori[i].IndexedCFUCards());
                                                        Console.WriteLine("\nVuoi giocare un altra carta istantanea?  \n0)No\n1)Si");
                                                        int avanti = Convert.ToInt32(Console.ReadLine());
                                                        if (avanti == 0)
                                                        {
                                                            continua2 = true;
                                                            continuaAltraCarta = true;

                                                        }
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Console.WriteLine("Inserisci un id corretto, Riprova!");
                                                    }
                                                }
                                            //}
                                            //catch (Exception)
                                            //{

                                            //    Console.WriteLine("Inserisci un Id valido!, Riprova");
                                            //}


                                        }
                                        else
                                        {
                                            Console.WriteLine("Scegli  una carta con  effetto AUMENTA,DIMINUISCI,INVERTI,SALVA,DIROTTA!, Riprova");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Inserisci un Id corretto!, Riprova");
                                    }
                                }

                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inserisci un numero fra 0 e 1!, Riprova");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Inserisci un id corretto!, Riprova\n");
                    }
                }


            }
        }

        private static void OrdinaPerCFu(ref List<Giocatore> giocatori, Tavolo tavolo, bool OrdinaPerCfuTavolo)
        {
          
            if (OrdinaPerCfuTavolo == false)
            { 
                for (int i = 0; i < tavolo.CarteCfu.Count; i++)
                {
                    for (int j = i + 1; j < tavolo.CarteCfu.Count; j++)
                    {
                        if (tavolo.CarteCfu[i].numCFU < tavolo.CarteCfu[j].numCFU)
                        {
                            CarteCFU cartaSalvata = tavolo.CarteCfu[i];
                            tavolo.CarteCfu[i] = tavolo.CarteCfu[j];
                            tavolo.CarteCfu[j] = cartaSalvata;

                            Giocatore playerSalvato = giocatori[i];
                            giocatori[i] = giocatori[j];
                            giocatori[j] = playerSalvato;

                            int Cfu = tavolo.Cfus[i];
                            tavolo.Cfus[i] = tavolo.Cfus[j];
                            tavolo.Cfus[j] = Cfu;

                        }
                    }
                    Console.WriteLine("GIOCATORE " + giocatori[i].nomeUtente + " CARTA: " + tavolo.CarteCfu[i].nomeCarta + " CFU: " + tavolo.Cfus[i]);
                }
            }
            else
            {

                for (int i = 0; i < tavolo.CarteCfu.Count; i++)
                {
                    for (int j = i + 1; j < tavolo.CarteCfu.Count; j++)
                    {
                        if (tavolo.Cfus[i] < tavolo.Cfus[j]) // 12 15 21 8
                        {
                            CarteCFU cartaSalvata = tavolo.CarteCfu[i];
                            tavolo.CarteCfu[i] = tavolo.CarteCfu[j];
                            tavolo.CarteCfu[j] = cartaSalvata;

                            Giocatore playerSalvato = giocatori[i];
                            giocatori[i] = giocatori[j];
                            giocatori[j] = playerSalvato;

                            int Cfu = tavolo.Cfus[i];
                            tavolo.Cfus[i] = tavolo.Cfus[j];
                            tavolo.Cfus[j] = Cfu;

                        }
                    }
                    Console.WriteLine("GIOCATORE " + giocatori[i].nomeUtente + " CARTA: " + tavolo.CarteCfu[i].nomeCarta + " CFU: " + tavolo.Cfus[i]);
                }
            }

            //Console.WriteLine("GIOCATORE " + giocatori[tavolo.IDs[0]].nomeUtente + " CARTA: " + tavolo.CarteCfu[0].nomeCarta + " ID: " + tavolo.IDs[0] + " CFU: " + tavolo.Cfus[0]);
            //Console.WriteLine("GIOCATORE " + giocatori[tavolo.IDs[1]].nomeUtente + " CARTA: " + tavolo.CarteCfu[1].nomeCarta + " ID: " + tavolo.IDs[1] + " CFU: " + tavolo.Cfus[1]);
            //Console.WriteLine("GIOCATORE " + giocatori[tavolo.IDs[2]].nomeUtente + " CARTA: " + tavolo.CarteCfu[2].nomeCarta + " ID: " + tavolo.IDs[2] + " CFU: " + tavolo.Cfus[2]);
            //Console.WriteLine("GIOCATORE " + giocatori[tavolo.IDs[3]].nomeUtente + " CARTA: " + tavolo.CarteCfu[3].nomeCarta + " ID: " + tavolo.IDs[3] + " CFU: " + tavolo.Cfus[3]);

            //Console.WriteLine("GIOCATORE " + giocatori[0].nomeUtente + " CARTA: " + tavolo.CarteCfu[0].nomeCarta  + " CFU: " + tavolo.Cfus[0]);
            //Console.WriteLine("GIOCATORE " + giocatori[1].nomeUtente + " CARTA: " + tavolo.CarteCfu[1].nomeCarta  + " CFU: " + tavolo.Cfus[1]);
            //Console.WriteLine("GIOCATORE " + giocatori[2].nomeUtente + " CARTA: " + tavolo.CarteCfu[2].nomeCarta  + " CFU: " + tavolo.Cfus[2]);
            //Console.WriteLine("GIOCATORE " + giocatori[3].nomeUtente + " CARTA: " + tavolo.CarteCfu[3].nomeCarta  + " CFU: " + tavolo.Cfus[3]);


        }

       

        private static bool CartaGiocabileCheck(Effetto effetto, bool istantaneo)
        {
            if (istantaneo == false)
            {
                switch (effetto)
                {
                    case Effetto.AUMENTA:
                        return false;
                        break;
                    case Effetto.DIMINUISCI:
                        return false;
                        break;
                    case Effetto.INVERTI:
                        return false;
                        break;

                    case Effetto.SALVA:
                        return false;
                        break;
                    case Effetto.DIROTTA:
                        return false;
                        break;
                    default:
                        return true;
                        break;
                }
            }
            else
            {
                switch (effetto)
                {
                    case Effetto.AUMENTA:
                        return true;
                        break;
                    case Effetto.DIMINUISCI:
                        return true;
                        break;
                    case Effetto.INVERTI:
                        return true;
                        break;

                    case Effetto.SALVA:
                        return true;
                        break;
                    case Effetto.DIROTTA:
                        return true;
                        break;
                    default:
                        return false;
                        break;
                }
            }
        }

        private static void EffettiECalcolo(List<Giocatore> giocatori, Tavolo tavolo, List<CarteCFU> tutteCarteCFU, List<CarteOstacolo> tutteCarteOstacolo, List<CarteCFU> mazzoScarti, ref bool doppioe)
        {
            //CONTINUARE A MODIFICARE EFFETTI Che ora anche i Giocatori sono ordinati
           for(int i = 0; i < giocatori.Count; i++)
           {
                if (tavolo.CarteCfu[i].effetto != Effetto.NESSUNO) //le carte sono ordinate per cfu , quindi è giusto questo
                {
                    Effetto effetto = tavolo.CarteCfu[i].effetto;

                    Giocatore giocatore = giocatori[i]; //Giocatore selezionato in modo corretto dal tavolo ordinato per Cfu in decrescente
                    //int Cfus = tavolo.Cfus[i]; //Cfu del giocatore selezionato in modo corretto dal tavolo ordinato per Cfu decrescenti
                    //CarteCFU cartaCfu = tavolo.CarteCfu[i]; //Carta cfu giocata del giocatore selezionato in modo corretto dal tavolo ordinato per Cfu decrescenti
                    //bool doppioe = false; //variabile  doppioe che raddoppia effetti che aumentano o diminuiscono punteggi
                    switch (effetto)
                    {
                        case Effetto.SCARTAP:
                            Console.WriteLine("\nEFFETTO SCARTAP: " +  giocatore.nomeUtente + ", scarta una carta CFU punto senza effetto e aggiungi il suo punteggio a quello del turno!" );
                            Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                            //Console.Write("Inserisci ID carta che vuoi giocare: ");
                            //int idCarta = Convert.ToInt32(Console.ReadLine());
                            bool continua = false;
                            while (!continua) //finchè continua è falso
                            {
                                try
                                {
                                    Console.Write("Inserisci ID carta che vuoi scartare: ");
                                    int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;
                                    if (giocatore.carteCFU[idCarta].numCFU > 0 && giocatore.carteCFU[idCarta].effetto == Effetto.NESSUNO)
                                    {
                                        Console.WriteLine("Cfu prima dell'effetto: " + tavolo.Cfus[i]); //stampa i cfu del giocatore in posizione 0,1,2,3
                                        tavolo.Cfus[i] += giocatore.carteCFU[idCarta].numCFU;
                                        giocatore.carteCFU.RemoveAt(idCarta); //scartala
                                        continua = true;
                                        Console.WriteLine("Cfu dopo l'effetto: " + tavolo.Cfus[i]);

                                        giocatori[i] = giocatore; //SETTO PER APPLICARE MODIFICHE
                                    }
                                    else
                                    {
                                        Console.WriteLine("Scegli una carta con almeno 1 CFU! e senza effetti, Riprova");
                                    }
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto!, Riprova");
                                }
                            }
                            
                            break;

                        case Effetto.RUBA:
                            Console.WriteLine("\nEFFETTO RUBA: " + giocatore.nomeUtente + ", guarda la mano di un collega a scelta e ruba una sua carta!");
                            //Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                            Console.WriteLine(Giocatore.IndexedPlayers(giocatori));


                            bool continuaa = false;
                            while (!continuaa)
                            {
                                try
                                {
                                    Console.WriteLine("Inserisci l'id del giocatore a cui vuoi visualizzare la mano");
                                    int idGiocatore = Convert.ToInt32(Console.ReadLine()) - 1;

                                    if (idGiocatore != i)
                                    {
                                        Console.WriteLine("Tutte le carte CFU di " + giocatori[idGiocatore].nomeUtente);
                                        Console.WriteLine(giocatori[idGiocatore].IndexedCFUCards());

                                        Console.WriteLine("Inserisci id della carta che vuoi rubare a " + giocatori[idGiocatore].nomeUtente);
                                        int idCarta2 = Convert.ToInt32(Console.ReadLine()) - 1;

                                        giocatore.carteCFU.Add(giocatori[idGiocatore].carteCFU[idCarta2]); //Aggiungo la carta rubata nel mio mazzo
                                        giocatori[idGiocatore].carteCFU.RemoveAt(idCarta2); //E la scarto a chi la ho rubata 

                                        Console.WriteLine("\nLe carte di " + giocatore.nomeUtente + " dopo aver rubato:");
                                        Console.WriteLine(giocatore.AllCfuCards() + "\n");

                                        giocatori[i] = giocatore; //SETTO PER APPLICARE MODIFICHE
                                        continuaa = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Non puoi scegliere te stesso!, Riprova");
                                    }
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto!, Riprova");
                                }
                            }

                            break;

                        case Effetto.SCAMBIADS:
                            Console.WriteLine("\nEFFETTO SCAMBIADS: " + giocatore.nomeUtente + ", scambia la carta giocata con quella di un altro giocatore, purchè senza effetto!");

                            Console.WriteLine("Tutte le carte nel tavolo: ");
                            Console.WriteLine(tavolo.TableIndexedCFUCards());

                            bool continua2 = false;
                            while(!continua2)
                            {
                                try
                                {
                                    Console.Write("Inserisci id della carta che vuoi scambiare con la tua, " + giocatore.nomeUtente);
                                    int idCartaTavolo = Convert.ToInt32(Console.ReadLine()) - 1;

                                    if (idCartaTavolo != i && tavolo.CarteCfu[idCartaTavolo].effetto == Effetto.NESSUNO)
                                    {
                                        CarteCFU cartaGiocatore = tavolo.CarteCfu[i];
                                        //CarteCFU cartaPlayer = cartaCfu; //salvo la mia carta giocata  CarteCFU cartaCfu = tavolo.CarteCfu[tavolo.IDs[i]];
                                        //cartaCfu = tavolo.CarteCfu[idCartaTavolo];
                                        //tavolo.CarteCfu[idCartaTavolo] = cartaPlayer;

                                        tavolo.CarteCfu[i] = tavolo.CarteCfu[idCartaTavolo];
                                        tavolo.CarteCfu[idCartaTavolo] = cartaGiocatore;

                                        continua2 = true;

                                        //tavolo.CarteCfu[i] = cartaCfu;

                                        //scambio Cfus sul tavolo
                                        int CfuMiei = tavolo.Cfus[i];
                                        tavolo.Cfus[i] = tavolo.Cfus[idCartaTavolo];
                                        tavolo.Cfus[idCartaTavolo] = CfuMiei;

                                        Console.WriteLine(tavolo.TableIndexedCFUCards()); //così vedo la modifica
                                    }
                                    else
                                    {
                                        Console.WriteLine("Non puoi selezionare la tua stessa carta nè una carta con Effetto, Riprova");
                                    }
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("Inerisci un id corretto, Riprova!");
                                }
                            }
                            
                            break;

                        case Effetto.SCARTAE:
                            Console.WriteLine("\nEFFETTO SCARTAE: " + giocatore.nomeUtente + ", scarta una carta CFU punto con effetto e aggiungi il suo punteggio a quello del turno!");
                            Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                            //Console.Write("Inserisci ID carta che vuoi giocare: ");
                            //int idCarta = Convert.ToInt32(Console.ReadLine());
                            bool continua3 = false;
                            while (!continua3) //finchè continua è falso
                            {
                                try
                                {
                                    Console.Write("Inserisci ID carta che vuoi scartare: ");
                                    int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;
                                    if (giocatore.carteCFU[idCarta].numCFU > 0)
                                    {
                                        Console.WriteLine("Cfu prima dell'effetto: " + tavolo.Cfus[i]); //stampa i cfu del giocatore in posizione 0,1,2,3
                                        tavolo.Cfus[i] += giocatore.carteCFU[idCarta].numCFU;
                                        giocatore.carteCFU.RemoveAt(idCarta); //scartala
                                        continua3 = true;
                                        Console.WriteLine("Cfu dopo l'effetto: " + tavolo.Cfus[i]);

                                        giocatori[i] = giocatore; //SETTO PER APPLICARE MODIFICHE
                                    }
                                    else
                                    {
                                        Console.WriteLine("Scegli una carta con almeno 1 CFU! e senza effetti, Riprova");
                                    }
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto!, Riprova");
                                }
                            }
                            break;

                        case Effetto.SCARTAC:
                            Console.WriteLine("\nEFFETTO SCARTAC: " + giocatore.nomeUtente + ", scarta da uno a tre carte dalla tua mano");
                            //Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                            int nscartate = 0;
                            
                            while (nscartate < 3) //finchè continua è falso
                            {
                                try
                                {
                                    Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                                    Console.WriteLine("Digita 1 per scartare una carta (MAX 3), \nDigita 2 per fermarti");
                                    int scelta = Convert.ToInt32(Console.ReadLine());
                                    if (scelta == 1)
                                    {
                                        Console.Write("Inserisci ID carta che vuoi scartare: ");
                                        int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;

                                        //Console.WriteLine("Cfu prima dell'effetto: " + tavolo.Cfus[i]); //stampa i cfu del giocatore in posizione 0,1,2,3
                                        //tavolo.Cfus[i] += giocatore.carteCFU[idCarta].numCFU;
                                        giocatore.carteCFU.RemoveAt(idCarta); //scartala

                                        //Console.WriteLine("Cfu dopo l'effetto: " + tavolo.Cfus[i]);
                                        nscartate++;
                                        giocatori[i] = giocatore; //SETTO PER APPLICARE MODIFICHE

                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto!, Riprova");
                                }
                            }
                            break;

                        case Effetto.SCAMBIAP:
                            Console.WriteLine("\nEFFETTO SCAMBIAP: " + giocatore.nomeUtente + ", scambia il punteggio del turno maggiore e minore!");
                            int min = 0;
                            int indiceMin = 0;
                            int max = 20;
                            int indiceMax = 0;
                            for (int j = 0; j < giocatori.Count; j++) //cercare il massimo
                            {
                                if (tavolo.Cfus[j] > min) // 9,5,7,2
                                {
                                    min = tavolo.Cfus[j];
                                    indiceMin = j;
                                }

                                if(tavolo.Cfus[j] < max) // 9,5,7,2
                                {
                                    max = tavolo.Cfus[j];
                                    indiceMax = j;
                                }

                            }
                            Console.WriteLine("Il punteggio di " + giocatori[indiceMin].nomeUtente + ": " + tavolo.Cfus[indiceMin] + " Cfu" +
                                                "\nViene scambiato con quello di " + giocatori[indiceMax].nomeUtente + ": " + tavolo.Cfus[indiceMax] + " Cfu");

                            tavolo.Cfus[indiceMin]  = tavolo.Cfus[indiceMax];
                            tavolo.Cfus[indiceMax] = min;
                            break;

                        case Effetto.DOPPIOE:
                             doppioe = true;
                            Console.WriteLine("\nEFFETTO DOPPIOE: " + giocatore.nomeUtente + ", Per tutto il turno, gli effetti che aumentano o diminuiscono il punteggio sono raddoppiati! (per tutti)");
                            break;

                        case Effetto.SBIRCIA:
                            Console.WriteLine("\nEFFETTO SBIRCIA: " + giocatore.nomeUtente + ", Guarda due carte in cima al mazzo, prendine una e scarta l’altra!");
                            Console.WriteLine("\n" + tutteCarteCFU[0].ToString());
                            Console.WriteLine("\n" + tutteCarteCFU[1].ToString());
                           
                            //Console.Write("Inserisci ID carta che vuoi giocare: ");
                            //int idCarta = Convert.ToInt32(Console.ReadLine());
                            bool continua4 = false;
                            while (!continua4) //finchè continua è falso
                            {
                                try
                                {
                                    Console.Write("Inserisci 1 o 2 in base alla carta che vuoi prendere, l'altra verrà scartata: ");
                                    int idCarta = Convert.ToInt32(Console.ReadLine()) - 1;
                                    if (idCarta == 0 || idCarta == 1)
                                    {
                                        if (idCarta == 0) //prendi 0, scarta 1
                                        {
                                            continua4 = true;
                                            giocatore.carteCFU.Add(tutteCarteCFU[0]);
                                            mazzoScarti.Add(tutteCarteCFU[1]);
                                            tutteCarteCFU.RemoveAt(1);

                                            giocatori[i] = giocatore; //SETTO PER APPLICARE MODIFICHE
                                        }
                                        else
                                        {
                                            //prendi 1, scarta 0 
                                            continua4 = true;
                                            giocatore.carteCFU.Add(tutteCarteCFU[1]);
                                            mazzoScarti.Add(tutteCarteCFU[0]);
                                            tutteCarteCFU.RemoveAt(0);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Inserisci un numero corretto fra 1 e 2, riprova!");
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto!, Riprova");
                                }
                            }
                            break;

                        case Effetto.SCAMBIAC:
                            Console.WriteLine("\nEFFETTO SCAMBIAC: " + giocatore.nomeUtente + ", Scegli due giocatori e scambia le loro carte punto giocate!");
                            Console.WriteLine(Giocatore.IndexedPlayers(giocatori));

                            //Console.WriteLine("Inserisci l'id del primo giocatore");
                            //int idGiocatoreUno = Convert.ToInt32(Console.ReadLine()) - 1;
                            //Console.WriteLine("Inserisci l'id del secondo giocatore");
                            //int idGiocatoreDue = Convert.ToInt32(Console.ReadLine()) - 1;

                            //int indiceCartaUno = 0;
                            //int indiceCartaDue = 0;

                            bool continua5 = false;
                            while (!continua5)
                            {
                                try
                                {
                                    //int idCartaTavolo = Convert.ToInt32(Console.ReadLine()) - 1;
                                    Console.WriteLine("Inserisci l'id del primo giocatore");
                                    int idGiocatoreUno = Convert.ToInt32(Console.ReadLine()) - 1;
                                    Console.WriteLine("Inserisci l'id del secondo giocatore");
                                    int idGiocatoreDue = Convert.ToInt32(Console.ReadLine()) - 1;

                                    if (idGiocatoreUno != idGiocatoreDue)
                                    {
                                        Console.WriteLine("Le carte di " + giocatori[idGiocatoreUno].nomeUtente + " e " + giocatori[idGiocatoreDue].nomeUtente + " vengono ora scambiate!");

                                        //for (int j = 0; j < tavolo.IDs.Count; j++)
                                        //{
                                        //    if (tavolo.IDs[j] == idGiocatoreUno)
                                        //    {
                                        //        indiceCartaUno = j;
                                        //    }
                                        //    if (tavolo.IDs[j] == idGiocatoreDue)
                                        //    {
                                        //        indiceCartaDue = j;
                                        //    }
                                        //}


                                        //CarteCFU cartaGiocatore = tavolo.CarteCfu[indiceCartaUno];
                                        ////CarteCFU cartaPlayer = cartaCfu; //salvo la mia carta giocata  CarteCFU cartaCfu = tavolo.CarteCfu[tavolo.IDs[i]];
                                        ////cartaCfu = tavolo.CarteCfu[idCartaTavolo];
                                        ////tavolo.CarteCfu[idCartaTavolo] = cartaPlayer;

                                        //tavolo.CarteCfu[indiceCartaUno] = tavolo.CarteCfu[indiceCartaDue];
                                        //tavolo.CarteCfu[indiceCartaDue] = cartaGiocatore;

                                        //continua5 = true;

                                        ////tavolo.CarteCfu[i] = cartaCfu;

                                        ////scambio Cfus sul tavolo
                                        //int CfuMiei = tavolo.Cfus[indiceCartaUno];
                                        //tavolo.Cfus[indiceCartaUno] = tavolo.Cfus[indiceCartaDue];
                                        //tavolo.Cfus[indiceCartaDue] = CfuMiei;

                                        //scambio carte
                                        CarteCFU cartaGiocatore = tavolo.CarteCfu[idGiocatoreUno];
                                        tavolo.CarteCfu[idGiocatoreUno] = tavolo.CarteCfu[idGiocatoreDue];
                                        tavolo.CarteCfu[idGiocatoreDue] = cartaGiocatore;

                                        //scambio Cfus sul tavolo
                                        int CfuMiei = tavolo.Cfus[idGiocatoreUno];
                                        tavolo.Cfus[idGiocatoreUno] = tavolo.Cfus[idGiocatoreDue];
                                        tavolo.Cfus[idGiocatoreDue] = CfuMiei;

                                        continua5 = true;

                                    }
                                    else
                                    {
                                        Console.WriteLine("Seleziona due giocatori diversi");
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto!, Riprova");
                                }
                            }
                            break;

                        case Effetto.ANNULLA:
                            Console.WriteLine("\nEFFETTO ANNULLA: " + giocatore.nomeUtente + ", Annulla gli effetti di tutte le carte punto durante il turno!");
                            return;
                            break;

                        
                        
                    }
                }
           }
        }

        private static void EffettiIstantanei(List<Giocatore> giocatori, Tavolo tavolo, List<CarteCFU> tutteCarteCFU, List<CarteOstacolo> tutteCarteOstacolo, List<CarteCFU> mazzoScarti, ref bool doppioe, Giocatore giocatore, CarteCFU CartaIst)
        {
                if (CartaIst.effetto != Effetto.NESSUNO) //le carte sono ordinate per cfu , quindi è giusto questo
                {

                    switch (CartaIst.effetto)
                    {
                        case Effetto.AUMENTA:
                            Console.WriteLine("\nEFFETTO AUMENTA: " + giocatore.nomeUtente + ", Aumenta di 2 CFU (4 CON DOPPIOE) il punteggio del turno di un giocatore a tua scelta!");
                            //Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                            Console.WriteLine(Giocatore.IndexedPlayers(giocatori));

                            bool continua1 = false;
                            while (!continua1)
                            {
                                try
                                {
                                    if (doppioe)
                                    {

                                        Console.WriteLine("Inserisci l'id del giocatore a cui vuoi aumentare il punteggio di 4! (EFFETTO DOPPIOE)");
                                        int idGiocatore = Convert.ToInt32(Console.ReadLine()) - 1;

                                        Console.Write("Hai scelto di aumentare di 4 il punteggio di: " + giocatori[idGiocatore].nomeUtente + " da " + tavolo.Cfus[idGiocatore] + "\n");
                                        tavolo.Cfus[idGiocatore] += 4;
                                        continua1 = true;
                                        Console.Write(" a " + tavolo.Cfus[idGiocatore] + " Cfu!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Inserisci l'id del giocatore a cui vuoi aumentare il punteggio di 2");
                                        int idGiocatore = Convert.ToInt32(Console.ReadLine()) - 1;

                                        Console.Write("Hai scelto di aumentare di 2 il punteggio di: " + giocatori[idGiocatore].nomeUtente + " da " + tavolo.Cfus[idGiocatore] + "\n");
                                        tavolo.Cfus[idGiocatore] += 2;
                                        continua1 = true;
                                        Console.Write(" a " + tavolo.Cfus[idGiocatore] + " Cfu!");

                                    }
                                }
                                catch(Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto, Riprova!");
                                }
                            }
                            //return;
                        break;

                        case Effetto.DIMINUISCI:
                            Console.WriteLine("\nEFFETTO DIMINUISCI: " + giocatore.nomeUtente + ", Diminuisci di 2 CFU (4 CON DOPPIOE) il punteggio del turno di un giocatore a tua scelta!");
                            //Console.WriteLine("\n" + giocatore.IndexedCFUCards());
                            Console.WriteLine(Giocatore.IndexedPlayers(giocatori));

                            bool continua2 = false;
                            while (!continua2)
                            {
                                try
                                {
                                    if (doppioe)
                                    {

                                        Console.WriteLine("Inserisci l'id del giocatore a cui vuoi diminuire il punteggio di 4! (EFFETTO DOPPIOE)");
                                        int idGiocatore = Convert.ToInt32(Console.ReadLine()) - 1;

                                        Console.Write("Hai scelto di diminuire di 4 il punteggio di: " + giocatori[idGiocatore].nomeUtente + " da : " + tavolo.Cfus[idGiocatore]);
                                        tavolo.Cfus[idGiocatore] -= 4;
                                        continua2 = true;
                                        Console.Write(" a " + tavolo.Cfus[idGiocatore] + " Cfu!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Inserisci l'id del giocatore a cui vuoi diminuire il punteggio di 2");
                                        int idGiocatore = Convert.ToInt32(Console.ReadLine()) - 1;

                                        Console.Write("Hai scelto di diminuire di 4 il punteggio di: " + giocatori[idGiocatore].nomeUtente + " da : " + tavolo.Cfus[idGiocatore]);
                                        tavolo.Cfus[idGiocatore] -= 2;
                                        continua2 = true;
                                        Console.Write(" a " + tavolo.Cfus[idGiocatore] + " Cfu!");

                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Inserisci un id corretto, Riprova!");
                                }
                            }
                            //return;
                        break;

                        case Effetto.INVERTI:
                            Console.WriteLine("\nEFFETTO INVERTI: " + giocatore.nomeUtente + ", scambia il punteggio del turno minore e maggiore!");
                            int min = 0;
                            int indiceMin = 0;
                            int max = 20;
                            int indiceMax = 0;
                            for (int j = 0; j < giocatori.Count; j++) //cercare il massimo
                            {
                                if (tavolo.Cfus[j] > min) // 9,5,7,2
                                {
                                    min = tavolo.Cfus[j];
                                    indiceMin = j;
                                }

                                if (tavolo.Cfus[j] < max) // 9,5,7,2
                                {
                                    max = tavolo.Cfus[j];
                                    indiceMax = j;
                                }

                            }
                            Console.WriteLine("Il punteggio di " + giocatori[indiceMin].nomeUtente + ": " + tavolo.Cfus[indiceMin] + " Cfu" +
                                                "\nViene scambiato con quello di " + giocatori[indiceMax].nomeUtente + ": " + tavolo.Cfus[indiceMax] + " Cfu");

                            tavolo.Cfus[indiceMin] = tavolo.Cfus[indiceMax];
                            tavolo.Cfus[indiceMax] = min;
                        break;

                        case Effetto.SALVA: //dopo questo interrompere gli effetti perchè ormai l'ostacolo sparisce
                        Console.WriteLine("\nEFFETTO SALVA: " + giocatore.nomeUtente + ", Metti la carta Ostacolo che stai per prendere in fondo al mazzo!");
                        Console.WriteLine("La carta: " + tavolo.cartaOstacolo.nomeCarta + " viene riposta in fondo al mazzo!");
                        tutteCarteOstacolo.Add(tavolo.cartaOstacolo);
                        tavolo.cartaOstacolo = null;
                        break;

                        case Effetto.DIROTTA: //dopo questo interrompere gli effetti perchè ormai l'ostacolo sparisce
                        Console.WriteLine("\nEFFETTO DIROTTA: " + giocatore.nomeUtente + ", Dai la carta che stai per prendere ad un altro giocatore a tua scelta");
                        Console.WriteLine(Giocatore.IndexedPlayers(giocatori));
                        tavolo.cartaOstacolo = null;


                        bool continuaa = false;
                        while (!continuaa)
                        {
                            try
                            {
                                Console.WriteLine("Inserisci Id giocatore a cui dare la carta ostacolo");
                                int idGiocatore = Convert.ToInt32(Console.ReadLine()) - 1;
                                if (giocatori[idGiocatore] != giocatore)
                                {
                                    giocatori[idGiocatore].carteOstacolo.Add(tavolo.cartaOstacolo);
                                    Console.WriteLine("La carta: " + tavolo.cartaOstacolo.nomeCarta + " viene data a " + giocatori[idGiocatore].nomeUtente + " !");
                                    continuaa = true;
                                }
                                else
                                {
                                    Console.WriteLine("Non puoi selezionare te stesso!, Riprova");
                                }
                            }
                            catch(Exception)
                            {
                                Console.WriteLine("Inserisci un id corretto!,Riprova");
                            }
                        }
                        break;
                    }
                }
        }

        private static void CalcoloCFUBonusMalus(List<Giocatore> giocatori, Tavolo tavolo)
        {
            for (int i = 0; i < giocatori.Count; i++)
            {
                //switch (ostacoloEstratto.tipoOstacolo)
                //{
                //    case TipoOstacolo.STUDIO:
                //      giocatori[i].cfuAccumulati += tavolo.CarteCfu[i].numCFU + giocatori[i].personaggio.bonusMalus[0];
                //        Console.WriteLine(giocatori[i].nomeUtente + ": " + giocatori[i].cfuAccumulati);
                //        break;

                //    case TipoOstacolo.SOPRAVVIVENZA:
                //        giocatori[i].cfuAccumulati += tavolo.CarteCfu[i].numCFU + giocatori[i].personaggio.bonusMalus[1];
                //        Console.WriteLine(giocatori[i].nomeUtente + ": " + giocatori[i].cfuAccumulati);
                //        break;

                //    case TipoOstacolo.SOCIALE:
                //        giocatori[i].cfuAccumulati += tavolo.CarteCfu[i].numCFU + giocatori[i].personaggio.bonusMalus[2];
                //        Console.WriteLine(giocatori[i].nomeUtente + ": " + giocatori[i].cfuAccumulati);
                //        break;

                //    case TipoOstacolo.ESAME:
                //        giocatori[i].cfuAccumulati += tavolo.CarteCfu[i].numCFU + giocatori[i].personaggio.bonusMalus[3];
                //        Console.WriteLine(giocatori[i].nomeUtente + ": " + giocatori[i].cfuAccumulati);
                //        break;
                //}

                switch (tavolo.cartaOstacolo.tipoOstacolo)
                {
                    case TipoOstacolo.STUDIO:
                        tavolo.Cfus[i] = tavolo.Cfus[i] + giocatori[i].personaggio.bonusMalus[0];
                        //Console.WriteLine(giocatori[i].nomeUtente + ": " + tavolo.Cfus[i]);
                        break;

                    case TipoOstacolo.SOPRAVVIVENZA:
                        tavolo.Cfus[i] = tavolo.Cfus[i] + giocatori[i].personaggio.bonusMalus[1];
                        //Console.WriteLine(giocatori[i].nomeUtente + ": " + tavolo.Cfus[i]);
                        break;

                    case TipoOstacolo.SOCIALE:
                        tavolo.Cfus[i] = tavolo.Cfus[i] + giocatori[i].personaggio.bonusMalus[2];
                        //Console.WriteLine(giocatori[i].nomeUtente + ": " + tavolo.Cfus[i]);
                        break;

                    case TipoOstacolo.ESAME:
                        tavolo.Cfus[i] = tavolo.Cfus[i] + giocatori[i].personaggio.bonusMalus[3];
                        //Console.WriteLine(giocatori[i].nomeUtente + ": " + tavolo.Cfus[i]);
                        break;
                }
            }
        }

        public static List<Personaggio> PopolaPersonaggi()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\res\personaggi.txt";
            
            var lines = File.ReadAllLines(path);
            //Console.WriteLine("length è " + lines.Length);

            List<Personaggio> personaggi = new List<Personaggio>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i]; //1 0 - 1 0 Andreosauro
                var parts = line.Split(' ');

                if (parts.Length == 5)
                {
                    //personaggi[i].bonusMalus[0] = Convert.ToInt16(parts[0]);
                    //personaggi[i].bonusMalus[1] = Convert.ToInt16(parts[1]);
                    //personaggi[i].bonusMalus[2] = Convert.ToInt16(parts[2]);
                    //personaggi[i].bonusMalus[3] = Convert.ToInt16(parts[3]);
                    //personaggi[i].nomePersonaggio = parts[4];
                    ////Da rifare con costruttore personaggi
                    
                    int[] bonusMalus = {  //metto tutti i bonus malus dentro questo vettore int
                                         Convert.ToInt16(parts[0]), 
                                         Convert.ToInt16(parts[1]), 
                                         Convert.ToInt16(parts[2]), 
                                         Convert.ToInt16(parts[3]) 
                                       };

                    personaggi.Add(new Personaggio(parts[4],bonusMalus)); //creo un personaggio e lo aggiungo alla lista
                }
            }
            return personaggi;
        }
        
        private static void PopolaTutteCarteCfu(List<CarteCFU> tutteCarteCFU) //3 numeri OCCORRENZE-ENUMERAZIONE-VALORE
        {
            CarteCFU cartaCFU = null;
            int occorrenze = 0;
            int enumerazione = 0;
            int valore = 0;
            string nomeCarta = "";

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\res\carte.txt";
            
            var lines = File.ReadAllLines(path); //string[] quindi posso usare indici
            //Console.WriteLine("length è " + lines.Length);

            for (int i = 0; i < lines.Length ; i++)
            {
                string line = lines[i]; //4 1 0 Monitor  //// OCCORRENZE - ENUMERAZIONE - VALORE - NOME CARTA
                var parts = line.Split(' '); //controllo se è lungo 4 o 5.

                if (parts.Length == 4)
                {
                    //4 1 0 Monitor
                    occorrenze = Convert.ToInt16(parts[0]);
                    enumerazione = Convert.ToInt16(parts[1]);
                    valore = Convert.ToInt16(parts[2]);
                    nomeCarta = parts[3];
                }
                else if (parts.Length == 5)
                {
                    //4 2 0 
                    occorrenze = Convert.ToInt16(parts[0]);
                    enumerazione = Convert.ToInt16(parts[1]);
                    valore = Convert.ToInt16(parts[2]);
                    nomeCarta = parts[3] + " " + parts[4];
                }
                else
                {
                    //4 2 0 
                    occorrenze = Convert.ToInt16(parts[0]);
                    enumerazione = Convert.ToInt16(parts[1]);
                    valore = Convert.ToInt16(parts[2]);
                    nomeCarta = parts[3] + " " + parts[4] + " " + parts[5];
                }

                for (int cicloOccorrenze = 0; cicloOccorrenze < occorrenze; cicloOccorrenze++)
                {
                    
                    cartaCFU = new CarteCFU(nomeCarta,valore, (Effetto)enumerazione); //8 luglio
                    tutteCarteCFU.Add(cartaCFU);

                }
            }
        }

        private static void PopolaTutteCarteOstacolo(List<CarteOstacolo> tutteCarteOstacolo)
        {
            CarteOstacolo cartaOstacolo = null;
            int numCarteTipo;
            string nomeCarta;
            string descrizioneCarta;
            int indiceTipo = 0;

            //leggo il file
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\res\ostacoli.txt";
           
            var lines = File.ReadAllLines(path); //string[] quindi posso usare indici
            //Console.WriteLine("length è " + lines.Length);

            for (int i = 0; i < lines.Length; i++)
            {


                    if (int.TryParse(lines[i], out numCarteTipo))
                    {
                      i++;
                      indiceTipo++;
                    }
                    nomeCarta = lines[i];
                    //Console.WriteLine("NOME: " + nomeCarta);
                    //i++;
                    descrizioneCarta = lines[++i];
                    //Console.WriteLine("DESC: " + descrizioneCarta);
                    
                    switch(indiceTipo)
                    {
                         case 1:
                         cartaOstacolo = new CarteOstacolo(nomeCarta, descrizioneCarta,TipoOstacolo.STUDIO);
                         tutteCarteOstacolo.Add(cartaOstacolo);
                         break;

                         case 2:
                         cartaOstacolo = new CarteOstacolo(nomeCarta, descrizioneCarta, TipoOstacolo.SOPRAVVIVENZA);
                         tutteCarteOstacolo.Add(cartaOstacolo);
                         break;

                         case 3:
                         cartaOstacolo = new CarteOstacolo(nomeCarta, descrizioneCarta, TipoOstacolo.SOCIALE);
                         tutteCarteOstacolo.Add(cartaOstacolo);
                         break;

                         case 4:
                         cartaOstacolo = new CarteOstacolo(nomeCarta, descrizioneCarta, TipoOstacolo.ESAME);
                         tutteCarteOstacolo.Add(cartaOstacolo);
                         break;
                    }

                    //Console.WriteLine("NOME: " + nomeCarta);
                    //Console.WriteLine("DESC: " + descrizioneCarta);
                    //Console.WriteLine("TIPO OSTACOLO: " + cartaOstacolo.tipoOstacolo);
            }


            



        }


        private static List<Giocatore> Fase1(ref List<CarteCFU> tutteCarteCFU, ref List<CarteOstacolo> tutteCarteOstacolo)
        {
            /*  
                Si caricano i personaggi dal file giocatori.txt e i mazzi da carte.txt e ostacoli.txt
                • Preso in input il numero di giocatori ed i nomi di ognuno, si assegna loro un personaggio casuale. 
                • Ad ogni giocatore deve essere assegnato un personaggio diverso.
                • Vengono mescolati i mazzi contenti carte CFU e carte Ostacolo.
                • Ogni giocatore parte con:
                • 0 CFU
                • 0 carte ostacolo
                • 5 carte CFU (che vengono distribuite dopo la mescolatura)
                • A questo punto il gioco può iniziare
             */

            ////Istanzio 4 personaggi


            //Popolo tutti i personaggi


            List<Personaggio> personaggi  = PopolaPersonaggi();




            MescolaMazzi(ref tutteCarteCFU, ref tutteCarteOstacolo);




            while (true)
            {
                try
                {
                    Console.WriteLine("Quanti giocatori devono giocare?\nInserire un numero da 2 a 4");
                    int numgiocatori = Convert.ToInt16(Console.ReadLine());

                    if (!(numgiocatori >= 2 && numgiocatori <= 4))
                    {
                        throw new Exception();
                    }


                    //CREO MANI
                    //riempiamo la mano cfu
                    int indiceCfu;
                    //mazzoCfuMescolato = tutteCarteCFU;
                    Random random = new Random();
                    List<Giocatore> giocatori = new List<Giocatore>();
                    int indicePersonaggio;
                    for (int j = 0; j < numgiocatori; j++) //ripeti da 2-4 giocatori
                    {
                        List<CarteCFU> manoGiocatoreCFU = new List<CarteCFU>();  //CREO MANI //ne creo sempre una nuova così non si accumulano
                        indicePersonaggio = random.Next(0, personaggi.Count);
                        Console.WriteLine("Inserisci nome giocatore " + (j + 1) + ": ");
                        string nomeGiocatore = Console.ReadLine();

                        if(nomeGiocatore == string.Empty)
                        {
                            throw new Exception();
                        }

                        for (int i = 0; i < 5; i++)  //riempiamo la mano cfu //cosi ripete 5 volte per ogni giocatore
                        {
                            indiceCfu = random.Next(0, tutteCarteCFU.Count); //0 - 74 // 0 - 73 e così via
                            manoGiocatoreCFU.Add(tutteCarteCFU[indiceCfu]); //metto nella mano una carta random del mazzo già randomizzato
                            tutteCarteCFU.RemoveAt(indiceCfu); //tolgo quella carta così non la ri-pesco

                        }
                        giocatori.Add(new Giocatore(nomeGiocatore, personaggi[indicePersonaggio], 0, manoGiocatoreCFU, new List<CarteOstacolo>()));
                        personaggi.RemoveAt(indicePersonaggio);
                        //tutteCarteCFU = mazzoCfuMescolato; //resetto tutto a 74 carte
                    }

                    return giocatori;
                }
                catch(Exception)
                {
                    Console.WriteLine("Inserisci dei dati corretti!, Riprova\n");
                }
            }   
            
        }

        private static void MescolaMazzi(ref List<CarteCFU> tutteCarteCFU, ref List<CarteOstacolo> tutteCarteOstacolo)
        {
            int indiceCasuale;
            Random random = new Random();
            //Console.WriteLine("Nel mazzo tutteCarteCFU ci sono " + tutteCarteCFU.Count);
            //MESCOLO MAZZO CFU
            List<CarteCFU> mazzoCfuMescolato = new List<CarteCFU>(); //mazzo che va a contenere le carte randomizzate
            while (tutteCarteCFU.Count != 0) //finchè ci sono carte nel mazzo normale continua
            {
                indiceCasuale = random.Next(0, tutteCarteCFU.Count); //prendi un indice casuael
                mazzoCfuMescolato.Add(tutteCarteCFU[indiceCasuale]); //inserisci la carta con quell'indice dal mazzo normale a quello random
                tutteCarteCFU.RemoveAt(indiceCasuale); //togli la carta dal mazzo normale
            }
            tutteCarteCFU = mazzoCfuMescolato; //risposta tutte le carte del mazzo random nel mazzo normale
            //Console.WriteLine("Nel mazzo tutteCarteCFU mescolato ci sono " + tutteCarteCFU.Count);

            //MESCOLO MAZZO OSTACOLI
            //Console.WriteLine("Nel mazzo tutteCarteOstacolo ci sono " + tutteCarteOstacolo.Count);
            List<CarteOstacolo> mazzoOstacoliMescolato = new List<CarteOstacolo>(); //mazzo che va a contenere le carte randomizzate
            while (tutteCarteOstacolo.Count != 0) //finchè ci sono carte nel mazzo ostacoli continua
            {
                indiceCasuale = random.Next(0, tutteCarteOstacolo.Count); //prendi un indice casuael
                mazzoOstacoliMescolato.Add(tutteCarteOstacolo[indiceCasuale]); //inserisci la carta con quell'indice dal mazzo ostacoli a quello random
                tutteCarteOstacolo.RemoveAt(indiceCasuale); //togli la carta dal mazzo ostacoli
            }
            tutteCarteOstacolo = mazzoOstacoliMescolato; //risposta tutte le carte del mazzo random nel mazzo normale
            //Console.WriteLine("Nel mazzo tutteCarteOstacolo mescolato ci sono " + tutteCarteOstacolo.Count);
        }

        private static void SalvaPartita(int playersCount, List<Giocatore> giocatori, List<CarteCFU> tutteCarteCFU, List<CarteCFU> mazzoScarti, List<CarteOstacolo> tutteCarteOstacolo, string saveFileName, int numTurno)
        {
    
            
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + @"\Salvataggi\";
            
            // Definisci il percorso completo del file log
            string filePath = Path.Combine(directoryPath, saveFileName);
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath,true))
                {
                    sw.WriteLine("TURNO: " + numTurno + "");
                    sw.WriteLine("NUMERO GIOCATORI IN PARTITA: " + playersCount +"\n");
                    foreach (Giocatore giocatore in giocatori)
                    {
                        sw.WriteLine(giocatore.ToString() + "\n"); //tutti i dati del giocatore
                        sw.WriteLine("TUTTE LE CARTE CFU DI " + giocatore.nomeUtente + ":");
                        sw.WriteLine(giocatore.AllCfuCards() + "\n");
                        sw.WriteLine("Carte ostacolo accumulate: " + giocatore.carteOstacolo.Count + "\n");
                        sw.WriteLine("---------------------------------------------------------");
                        sw.WriteLine("TUTTE LE CARTE OSTACOLO DI " + giocatore.nomeUtente + ":");
                        sw.WriteLine(giocatore.AllOstacoloCards());
                        sw.WriteLine("////////////////////////////////////////////////////////////////////////////////////\n////////////////////////////////////////////////////////////////////////////////////\n");
                    }
                    sw.WriteLine("NUMERO CARTE MAZZO CFU: " + tutteCarteCFU.Count +"\n\nCARTE RIMASTE NEL MAZZO CFU: \n");
                    foreach(CarteCFU cartaCfu in tutteCarteCFU)
                    { 
                        sw.WriteLine(cartaCfu.ToString());
                    }
                    sw.WriteLine("\nNUMERO CARTE MAZZO SCARTI: " + mazzoScarti.Count + "\n\nCARTE NEL MAZZO SCARTI: ");
                    foreach (CarteCFU cartaCfu in mazzoScarti)
                    {
                        sw.WriteLine(cartaCfu.ToString());
                    }
                    sw.WriteLine("\nNUMERO CARTE MAZZO OSTACOLI: " + tutteCarteOstacolo.Count + "\n\nCARTE RIMASTE NEL MAZZO OSTACOLI: \n");
                    foreach (CarteOstacolo cartaOstacolo in tutteCarteOstacolo)
                    {
                        sw.WriteLine(cartaOstacolo.ToString());
                    }
                    sw.WriteLine("\n--------------------------------------------------------------------------------------------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }
}
