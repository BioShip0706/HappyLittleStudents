namespace HappyLittleStudents
{
    enum TipoOstacolo
    {
        STUDIO,
        SOPRAVVIVENZA,
        SOCIALE,
        ESAME
    }
    internal class CarteOstacolo
    {
        public string nomeCarta;
        public string descCarta;
        public TipoOstacolo tipoOstacolo;

        public CarteOstacolo(string nomeCarta, string descCarta, TipoOstacolo tipoOstacolo)
        {
            this.nomeCarta = nomeCarta;
            this.descCarta = descCarta;
            this.tipoOstacolo = tipoOstacolo;
        }

        public override string ToString()
        {
            return "-----------------\n" + "Nome Carta: " + nomeCarta + "\nDescrizione carta: " + descCarta + "\nTipo Ostacolo: " + tipoOstacolo + "\n-----------------";

        }
    }



}