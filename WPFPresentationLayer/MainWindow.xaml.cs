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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogicLayer;
using DataObjects;
using WpfPresentationLayer;

namespace WPFPresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IUserManager _userManager;
        private ICharacterManager _characterManager;
        private ICampaignManager _campaignManager;
        private User _user = null;

        public MainWindow()
        {
            InitializeComponent();
            _userManager = new UserManager();
            _characterManager = new CharacterManager();
            _campaignManager = new CampaignManager();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text;
            var password = pwdPassword.Password;
            if(btnLogin.Content.ToString() == "Logout")
            {
                logoutUser();
                return;
            }
            if(email.Length < 7 || password.Length < 7)
            {
                MessageBox.Show("Invalid Email or Password", "Invalid Login", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtEmail.Text = "";
                pwdPassword.Password = "";
                txtEmail.Focus();
                return;
            }
            try
            {
                _user = _userManager.AuthenticateUser(email, password);
                // code to set up the interface, come back here
                string roles = "";
                for (int i = 0; i < _user.Roles.Count; i++)
                {
                    roles += _user.Roles[i];
                    if(i<_user.Roles.Count - 1)
                    {
                        roles += ", ";
                    }
                }

                if (pwdPassword.Password.ToString() == "newuser")
                {
                    //force password reset
                    var resetPassword = new frmUpdatePassword(_user, _userManager);
                    if (resetPassword.ShowDialog() == false)
                    {
                        logoutUser();
                        MessageBox.Show("You must change your password to continue.");
                        return;
                    }
                }
                txtEmail.Text = "";
                pwdPassword.Password = "";
                txtEmail.IsEnabled = false;
                txtEmail.Visibility = Visibility.Hidden;
                pwdPassword.IsEnabled = false;
                pwdPassword.Visibility = Visibility.Hidden;
                btnLogin.Content = "Logout";
                lblPassword.Visibility = Visibility.Hidden;
                lblEmail.Visibility = Visibility.Hidden;
                lblUserLoggedIn.Visibility = Visibility.Visible;
                lblWelcomeMessage.Visibility = Visibility.Visible;
                lblWelcomeMessage.Content = "Welcome back!";
                lblUserLoggedIn.Content = _user.FirstName + " " + _user.LastName;
                showUserTabs();
                

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void showUserTabs()
        {
            foreach (var r in _user.Roles)
            {
                switch (r)
                {
                    case "Characters":
                        tabCharacters.Visibility = Visibility.Visible;
                        tabCharacters.IsSelected = true;
                        break;
                    case "Campaigns":
                        tabCampaigns.Visibility = Visibility.Visible;
                        tabCampaigns.IsSelected = true;
                        break;
                    case "UserManagement":
                        tabUserManagement.Visibility = Visibility.Visible;
                        tabUserManagement.IsSelected = true;
                        break;
                    default:
                        break;
                }
            }
            tabsetMain.Visibility = Visibility.Visible;
        }

        private void logoutUser()
        {
            _user = null;

            // reset the login
            btnLogin.Content = "Login";
            txtEmail.Text = "";
            pwdPassword.Password = "";
            txtEmail.IsEnabled = true;
            pwdPassword.IsEnabled = true;
            txtEmail.Visibility = Visibility.Visible;
            pwdPassword.Visibility = Visibility.Visible;
            lblWelcomeMessage.Visibility = Visibility.Hidden;
            lblUserLoggedIn.Visibility = Visibility.Hidden;
            lblEmail.Visibility = Visibility.Visible;
            lblPassword.Visibility = Visibility.Visible;
            hideAllUserTabs();
            return;
        }

        private void hideAllUserTabs()
        {
            foreach (TabItem item in tabsetMain.Items)
            {
                item.Visibility = Visibility.Collapsed;
            }
            tabsetMain.Visibility = Visibility.Collapsed;
        }

        private void TabCharacters_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if(icCharacterList.ItemsSource == null)
                {
                    icCharacterList.ItemsSource = _characterManager.RetrieveCharacterListByUser(_user.UserID);
                }
                Visibility visibility = Visibility.Hidden;
                Application.Current.Resources["HiddenDeleteButton"] = visibility;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message, "Character Retrieval Failed",  MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TabCampaigns_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (icCampaignList.ItemsSource == null)
                {
                    icCampaignList.ItemsSource = _campaignManager.RetrieveCampaignListByUser(_user.UserID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message, "Campaign Retrieval Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TabUserManagement_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgUserList.ItemsSource == null)
                {
                    dgUserList.ItemsSource = _userManager.RetrieveUserListByActive();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void DgUserList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            User selectedUser = (User)dgUserList.SelectedItem;

            MessageBox.Show("You selected " + selectedUser.FirstName);
            var userWindow = new frmUser(selectedUser, _userManager);
            if (userWindow.ShowDialog() == true)
            {
                refreshUserList();
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var userWindow = new frmUser(_userManager);
            if (userWindow.ShowDialog() == true)
            {
                refreshUserList();
            }
        }
        private void ChkActive_Click(object sender, RoutedEventArgs e)
        {
            refreshUserList();
        }
        private void refreshCharacterList()
        {
            icCharacterList.ItemsSource = _characterManager.RetrieveCharacterListByUser(_user.UserID);
        }
        private void refreshUserList()
        {
            dgUserList.ItemsSource =
                _userManager.RetrieveUserListByActive((bool)chkActive.IsChecked);
            //dgUserList.Columns.RemoveAt(4);
        }
        private void DgUserList_AutoGeneratedColumns(object sender, EventArgs e)
        {
            //dgUserList.Columns.Remove(dgUserList.Columns[4]);

            dgUserList.Columns[0].Header = "User ID";
            dgUserList.Columns[1].Header = "Given Name";
            dgUserList.Columns[2].Header = "Family Name";
            dgUserList.Columns[3].Header = "Email Address";
        }

        private void CharName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            Character selectedCharacter = (Character)button.DataContext;
            var characterWindow = new frmCharacter(selectedCharacter, _characterManager, _user);
            if (characterWindow.ShowDialog() == true)
            {
                refreshCharacterList();
            }
        }

        private void BtnAddCharacter_Click(object sender, RoutedEventArgs e)
        {
            var characterWindow = new frmCharacter(_characterManager, _user);
            if(characterWindow.ShowDialog() == true)
            {
                refreshCharacterList();
            }
        }

        private void ChkDelete_Checked(object sender, RoutedEventArgs e)
        {
            //icCharacterList.ClearValue(Button.VisibilityProperty);
            //refreshCharacterList();
            Visibility visibility = Visibility.Visible;
            Application.Current.Resources["DeleteButton"] = new Visibility();
        }

        private void BtnDeleteCharacter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            Character selectedCharacter = (Character)button.DataContext;
            var response = MessageBox.Show("Confirm Delete", "Delete" + selectedCharacter.Name, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (response == MessageBoxResult.Yes)
            {
                var result = _characterManager.DeleteCharacter(selectedCharacter);
            }
            refreshCharacterList();
        }
    }
}
