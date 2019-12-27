using DataObjects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFPresentationLayer
{
    /// <summary>
    /// Interaction logic for frmCharacter.xaml
    /// </summary>
    public partial class frmCharacter : Window
    {
        private User _user = null;
        private Character _character = null;
        private ICharacterManager _characterManager = null;
        private bool _addMode = false;

        public frmCharacter(ICharacterManager characterManager, User user)
        {
            InitializeComponent();
            _characterManager = characterManager;
            _addMode = true;
            _user = user;
        }
        public frmCharacter(Character character, ICharacterManager characterManager, User user)
        {
            _user = user;
            _character = character;
            _characterManager = characterManager;

            try
            {
                _character = _characterManager.RetrieveCharacterStatsByCharacter(character);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_addMode == false)
            {
                DataObjects.AppDetails.AppPath = AppContext.BaseDirectory;
                imgClass.Source = new BitmapImage(new Uri(AppDetails.ImagePath + _character.Race + ".png"));
                txtCharacterName.Text = _character.Name;
                txtRace.Text = _character.Race;
                txtStrength.Text = _character.Strength.ToString();
                txtDexterity.Text = _character.Dexterity.ToString();
                txtConstitution.Text = _character.Constitution.ToString();
                txtIntelligence.Text = _character.Intelligence.ToString();
                txtWisdom.Text = _character.Wisdom.ToString();
                txtCharisma.Text = _character.Charisma.ToString();
                txtHPMax.Text = _character.HitPointMax.ToString();
                txtAC.Text = _character.ArmorClass.ToString();
                txtProf.Text = "+3";
            }
            else
            {
                SetEditMode();
                btnStrengthCheck.IsEnabled = false;
                btnDexterityCheck.IsEnabled = false;
                btnConstitutionCheck.IsEnabled = false;
                btnIntelligenceCheck.IsEnabled = false;
                btnWisdomCheck.IsEnabled = false;
                btnCharismaCheck.IsEnabled = false;
            }
        }

        private void SetEditMode()
        {
            txtCharacterName.IsReadOnly = false;
            txtRace.IsReadOnly = false;
            txtStrength.IsReadOnly = false;
            txtDexterity.IsReadOnly = false;
            txtConstitution.IsReadOnly = false;
            txtIntelligence.IsReadOnly = false;
            txtWisdom.IsReadOnly = false;
            txtCharisma.IsReadOnly = false;
            txtHPMax.IsReadOnly = false;
            txtAC.IsReadOnly = false;

            btnEdit.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            txtCharacterName.Focus();
        }
        private void BtnStrengthCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearChecks();
            txtStrengthResult.Text = (DieRoller.RollDie(Dice.Twenty) + (Int32.Parse(txtStrength.Text) - 10) / 2).ToString();
        }

        private void BtnDexterityCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearChecks();
            txtDexterityResult.Text = (DieRoller.RollDie(Dice.Twenty) + (Int32.Parse(txtDexterity.Text) - 10) / 2).ToString();
        }

        private void BtnConstitutionCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearChecks();
            txtConstitutionResult.Text = (DieRoller.RollDie(Dice.Twenty) + (Int32.Parse(txtConstitution.Text) - 10) / 2).ToString();
        }

        private void BtnIntelligenceCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearChecks();
            txtIntelligenceResult.Text = (DieRoller.RollDie(Dice.Twenty) + (Int32.Parse(txtIntelligence.Text) - 10) / 2).ToString();
        }

        private void BtnWisdomCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearChecks();
            txtWisdomResult.Text = (DieRoller.RollDie(Dice.Twenty) + (Int32.Parse(txtWisdom.Text) - 10) / 2).ToString();
        }

        private void BtnCharismaCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearChecks();
            txtCharismaResult.Text = (DieRoller.RollDie(Dice.Twenty) + (Int32.Parse(txtCharisma.Text) - 10) / 2).ToString();
        }

        private void ClearChecks()
        {
            txtStrengthResult.Text = "";
            txtDexterityResult.Text = "";
            txtConstitutionResult.Text = "";
            txtIntelligenceResult.Text = "";
            txtWisdomResult.Text = "";
            txtCharismaResult.Text = "";
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // error checks
            if (txtCharacterName.Text == "")
            {
                MessageBox.Show("You must enter a first name.");
                txtCharacterName.Focus();
                return;
            }
            if(!(txtRace.Text == "Human" || txtRace.Text == "Elf" || txtRace.Text == "Tiefling"
                || txtRace.Text == "Dwarf" || txtRace.Text == "Halfling" || txtRace.Text == "Gnome"
                || txtRace.Text == "Dragonborn" || txtRace.Text == "Half-Elf") || txtRace.Text == "Half-orc")
            {
                MessageBox.Show("You must enter a valid character Race.");
                txtRace.Focus();
                return;
            }
            if(!(Int32.TryParse(txtHPMax.Text, out int HP)))
            {
                MessageBox.Show("You must enter a valid Hit Point Maximum.");
                txtHPMax.Focus();
                return;
            }
            if(!(Int32.TryParse(txtAC.Text, out int AC)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtAC.Focus();
                return;
            }
            if(!(Int32.TryParse(txtStrength.Text, out int Strength)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtStrength.Focus();
                return;
            }
            if(!(Int32.TryParse(txtDexterity.Text, out int Dexterity)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtDexterity.Focus();
                return;
            }
            if(!(Int32.TryParse(txtConstitution.Text, out int Constitution)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtConstitution.Focus();
                return;
            }
            if(!(Int32.TryParse(txtIntelligence.Text, out int Intelligence)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtIntelligence.Focus();
                return;
            }
            if(!(Int32.TryParse(txtWisdom.Text, out int Wisdom)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtWisdom.Focus();
                return;
            }
            if(!(Int32.TryParse(txtCharisma.Text, out int Charisma)))
            {
                MessageBox.Show("You must enter a valid Armor Class.");
                txtCharisma.Focus();
                return;
            }

            Character newCharacter = new Character()
            {
                Name = txtCharacterName.Text.ToString(),
                Race = txtRace.Text.ToString(),
                HitPointMax = HP,
                ArmorClass = AC,
                Description = "New Character",
                Strength = Strength,
                Dexterity = Dexterity,
                Constitution = Constitution,
                Intelligence = Intelligence,
                Wisdom = Wisdom,
                Charisma = Charisma
            };
            if (_addMode)
            {
                try
                {
                    newCharacter.CharacterID = _characterManager.AddCharacter(newCharacter, _user);
                    if (_characterManager.AddCharacterAbilityScores(newCharacter))
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n"
                        + ex.InnerException.Message);
                }
            }
            else
            {
                try
                {
                    if (_characterManager.EditCharacter(_character, newCharacter) 
                        && _characterManager.EditCharacterAbilityScore(_character.CharacterID, "Strength", _character.Strength, newCharacter.Strength)
                        && _characterManager.EditCharacterAbilityScore(_character.CharacterID, "Dexterity", _character.Dexterity, newCharacter.Dexterity)
                        && _characterManager.EditCharacterAbilityScore(_character.CharacterID, "Constitution", _character.Constitution, newCharacter.Constitution)
                        && _characterManager.EditCharacterAbilityScore(_character.CharacterID, "Intelligence", _character.Intelligence, newCharacter.Intelligence)
                        && _characterManager.EditCharacterAbilityScore(_character.CharacterID, "Wisdom", _character.Wisdom, newCharacter.Wisdom)
                        && _characterManager.EditCharacterAbilityScore(_character.CharacterID, "Charisma", _character.Charisma, newCharacter.Charisma))
                    {
                        // success
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        throw new ApplicationException("Data not Saved.",
                            new ApplicationException("User may no longer exist.")); ;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n"
                        + ex.InnerException.Message);
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
