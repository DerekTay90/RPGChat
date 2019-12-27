using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using DataAccessLayer;

namespace LogicLayer
{
    public class CharacterManager : ICharacterManager
    {
        private ICharacterAccessor _characterAccessor;

        public CharacterManager()
        {
            _characterAccessor = new CharacterAccessor();
        }

        public int AddCharacter(Character character, User user)
        {
            int result = 0;

            try
            {
                result = _characterAccessor.InsertCharacter(character, user);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Insert failed", ex);
            }

            return result;
        }

        public bool AddCharacterAbilityScores(Character character)
        {
            bool result = false;

            try
            {
                result = _characterAccessor.InsertCharacterAbilityScore(character) == 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Insert failed", ex);
            }

            return result;
        }

        public bool DeleteCharacter(Character character)
        {
            bool result = false;

            try
            {
                result = _characterAccessor.DeleteCharacter(character);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update failed", ex);
            }

            return result;
        }

        public bool EditCharacter(Character oldCharacter, Character newCharacter)
        {
            bool result = false;

            try
            {
                result = _characterAccessor.UpdateCharacter(oldCharacter, newCharacter) == 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update failed", ex);
            }

            return result;
        }

        public bool EditCharacterAbilityScore(int charID, string abilityScore, int newScore, int oldScore)
        {
            bool result = false;

            try
            {
                result = _characterAccessor.UpdateAbilityScore(charID, abilityScore, newScore, oldScore) == 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update failed", ex);
            }

            return result;
        }

        public List<Character> RetrieveCharacterListByUser(int userid)
        {
            try
            {
                return _characterAccessor.GetAllCharactersByUserID(userid);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Character data not found.", ex);
            }
        }

        public Character RetrieveCharacterStatsByCharacter(Character character)
        {
            try
            {
                return _characterAccessor.GetAbilityScoresByCharacterID(character);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Character data not found.", ex);
            }
        }
    }
}
