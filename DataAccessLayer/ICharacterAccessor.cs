using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface ICharacterAccessor
    {
        List<Character> GetAllCharactersByUserID(int UserID);
        List<CharacterVM> GetAllCharactersVMByUserID(int UserID);
        Character GetAbilityScoresByCharacterID(Character character);
        int UpdateCharacter(Character oldCharacter, Character newCharacter);
        int UpdateAbilityScore(int charID, string abilityScore, int newScore, int oldScore);
        int InsertCharacter(Character character, User user);
        int InsertCharacterAbilityScore(Character character);
        bool DeleteCharacter(Character character);
    }
}
