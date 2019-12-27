using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface ICharacterManager
    {
        List<Character> RetrieveCharacterListByUser(int userid);
        Character RetrieveCharacterStatsByCharacter(Character character);
        bool EditCharacter(Character oldCharacter, Character newCharacter);
        bool EditCharacterAbilityScore(int charID, string abilityScore, int newScore, int oldScore);
        int AddCharacter(Character character, User user);
        bool AddCharacterAbilityScores(Character character);
        bool DeleteCharacter(Character character);
    }
}
