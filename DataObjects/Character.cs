using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Character
    {
        public int CharacterID { get; set; }
        public string Name { get; set; }
        public string Race { get; set; }
        public List<string> CharacterClasses { get; set; }
        public int HitPointMax { get; set; }
        public int ArmorClass { get; set; }
        public string Description { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string ImageSource { get; set; }
        public Character()
        {
            CharacterClasses = new List<string>();

        }
    }
}
