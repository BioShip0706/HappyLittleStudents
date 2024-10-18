using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyLittleStudents
{
    public class Personaggio
    {
       public string nomePersonaggio {  get; set; }
       public int[] bonusMalus = new int[4];

        public Personaggio(string nomePersonaggio, int[] bonusMalus)
        {
            this.nomePersonaggio = nomePersonaggio;
            this.bonusMalus = bonusMalus;
        }

        public override string ToString()
        {
            string bonusumalusu = "";
            foreach(int bM in bonusMalus)
            {
                bonusumalusu += bM + " ";
            }
            return "-----------------\n" + "Nome personaggio: " + nomePersonaggio + "\nBonus Malus: " + bonusumalusu + "\n-----------------";
        }
    }


}
