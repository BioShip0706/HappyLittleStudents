namespace HappyLittleStudents
{
    enum Effetto
    {
        NESSUNO,
        SCARTAP,
        RUBA,
        SCAMBIADS,
        SCARTAE,
        SCARTAC,
        SCAMBIAP,
        DOPPIOE,
        SBIRCIA,
        SCAMBIAC,
        ANNULLA,
        AUMENTA,
        DIMINUISCI,
        INVERTI,
        SALVA,
        DIROTTA
    }

    internal class CarteCFU
    {
        public string nomeCarta;
        public int numCFU;
        public Effetto effetto;

        public CarteCFU(string nomeCarta, int numCFU, Effetto effetto)
        {
            this.nomeCarta = nomeCarta;
            this.numCFU = numCFU;
            this.effetto = effetto;
        }

        static string RitornaEffetto(int enumerazione)
        {
            Effetto effetto = (Effetto)enumerazione;

            return effetto.ToString();
        }

        public override string ToString()
        {
            return "-----------------\n" + "Nome Carta: " + nomeCarta + "\nNum CFU: " + numCFU + "\nEffetto carta: " + effetto + "\n-----------------";
        }


    }


}