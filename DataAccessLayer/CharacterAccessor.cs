using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class CharacterAccessor : ICharacterAccessor
    {
        public bool DeleteCharacter(Character character)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_delete_character", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CharacterID", character.CharacterID);

            try
            {
                conn.Open();
                result = (Convert.ToInt32(cmd.ExecuteScalar()) == 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public Character GetAbilityScoresByCharacterID(Character character)
        {
            List<int> scores = new List<int>();

            // connection
            var conn = DBConnection.GetConnection();

            // command objects
            var cmd = new SqlCommand("[sp_select_ability_scores_by_characterid]");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CharacterID", character.CharacterID);
            try
            {
                // open connection
                conn.Open();

                // execute the first command

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //int score = reader.GetInt32(0);
                    scores.Add(reader.GetInt32(0));
                }
                character.Charisma = scores[0];
                character.Constitution = scores[1];
                character.Dexterity = scores[2];
                character.Intelligence = scores[3];
                character.Strength = scores[4];
                character.Wisdom = scores[5];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return character;
        }

        public List<Character> GetAllCharactersByUserID(int UserID)
        {
            List<Character> characters = new List<Character>();

            var conn = DBConnection.GetConnection();

            var cmd1 = new SqlCommand("sp_select_characters_by_userid");
            var cmd2 = new SqlCommand("sp_select_class_by_characterid");

            cmd1.Connection = conn;
            cmd2.Connection = conn;

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd2.CommandType = CommandType.StoredProcedure;

            cmd1.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                conn.Open();
                var reader1 = cmd1.ExecuteReader();

                if (reader1.HasRows)
                {
                    while (reader1.Read())
                    {
                        var character = new Character();
                        character.CharacterID = reader1.GetInt32(0);
                        character.Name = reader1.GetString(1);
                        character.Race = reader1.GetString(2);
                        character.HitPointMax = reader1.GetInt32(3);
                        character.ArmorClass = reader1.GetInt32(4);
                        character.Description = reader1.GetString(5);
                        characters.Add(character);
                        DataObjects.AppDetails.AppPath = AppContext.BaseDirectory;
                        character.ImageSource = AppDetails.ImagePath + character.Race + ".png";
                    }
                    reader1.Close();
                }
                cmd2.Parameters.AddWithValue("@CharacterID", "test");
                foreach (var character in characters)
                {
                    cmd2.Parameters["@CharacterID"].Value = character.CharacterID;
                    var reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            character.CharacterClasses.Add(reader2.GetString(0));
                        }
                    }
                    reader2.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return characters;
        }

        public List<CharacterVM> GetAllCharactersVMByUserID(int UserID)
        {
            List<Character> charactersList = new List<Character>();
            List<CharacterVM> characters = new List<CharacterVM>();

            var conn = DBConnection.GetConnection();

            var cmd1 = new SqlCommand("sp_select_characters_by_userid");
            var cmd2 = new SqlCommand("sp_select_class_by_characterid");

            cmd1.Connection = conn;
            cmd2.Connection = conn;

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd2.CommandType = CommandType.StoredProcedure;

            cmd1.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                conn.Open();
                var reader1 = cmd1.ExecuteReader();

                if (reader1.HasRows)
                {
                    while (reader1.Read())
                    {
                        var character = new Character();
                        character.CharacterID = reader1.GetInt32(0);
                        character.Name = reader1.GetString(1);
                        character.Race = reader1.GetString(2);
                        character.HitPointMax = reader1.GetInt32(3);
                        character.ArmorClass = reader1.GetInt32(4);
                        character.Description = reader1.GetString(5);
                        character.CharacterClasses = new List<string>();
                        charactersList.Add(character);
                        var characterVM = new CharacterVM();
                        characterVM.Name = character.Name;
                    }
                    reader1.Close();
                }
                cmd2.Parameters.AddWithValue("@CharacterID", "test");
                foreach (var character in charactersList)
                {
                    cmd2.Parameters["@CharacterID"].Value = character.CharacterID;
                    var reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            character.CharacterClasses.Add(reader2.GetString(0));
                        }
                    }
                    reader2.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return characters;
        }

        public int InsertCharacter(Character character, User user)
        {
            int charID = -1;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_character", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@Name", character.Name);
            cmd.Parameters.AddWithValue("@RaceID", character.Race);
            cmd.Parameters.AddWithValue("@Hitpoints", character.HitPointMax);
            cmd.Parameters.AddWithValue("@ArmorClass", character.ArmorClass);
            cmd.Parameters.AddWithValue("@Description", character.Description);

            try
            {
                conn.Open();
                charID = Convert.ToInt32(cmd.ExecuteScalar());
                if(charID == 0)
                {
                    throw new ApplicationException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return charID;
        }

        public int InsertCharacterAbilityScore(Character character)
        {
            int rows = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_ability_scores", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CharacterID", character.CharacterID);
            cmd.Parameters.AddWithValue("@Strength", character.Strength);
            cmd.Parameters.AddWithValue("@Dexterity", character.Dexterity);
            cmd.Parameters.AddWithValue("@Constitution", character.Constitution);
            cmd.Parameters.AddWithValue("@Intelligence", character.Intelligence);
            cmd.Parameters.AddWithValue("@Wisdom", character.Wisdom);
            cmd.Parameters.AddWithValue("@Charisma", character.Charisma);

            try
            {
                conn.Open();
                rows = Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public int UpdateAbilityScore(int charID, string abilityScore, int newScore, int oldScore)
        {
            int rows = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_character_Ability_score", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CharacterID", charID);

            cmd.Parameters.AddWithValue("@AbilityScoreID", abilityScore);
            cmd.Parameters.AddWithValue("@NewScore", newScore);
            cmd.Parameters.AddWithValue("@OldScore", oldScore);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public int UpdateCharacter(Character oldCharacter, Character newCharacter)
        {
            int rows = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_character", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CharacterID", oldCharacter.CharacterID);

            cmd.Parameters.AddWithValue("@NewName", newCharacter.Name);
            cmd.Parameters.AddWithValue("@NewRaceID", newCharacter.Race);
            cmd.Parameters.AddWithValue("@NewHitPoints", newCharacter.HitPointMax);
            cmd.Parameters.AddWithValue("@NewArmorClass", newCharacter.ArmorClass);
            cmd.Parameters.AddWithValue("@NewDescription", newCharacter.Description);

            cmd.Parameters.AddWithValue("@OldName", oldCharacter.Name);
            cmd.Parameters.AddWithValue("@OldRaceID", oldCharacter.Race);
            cmd.Parameters.AddWithValue("@OldHitPoints", oldCharacter.HitPointMax);
            cmd.Parameters.AddWithValue("@OldArmorClass", oldCharacter.ArmorClass);
            cmd.Parameters.AddWithValue("@OldDescription", oldCharacter.Description);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }
    }
}
