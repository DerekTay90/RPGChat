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

namespace WpfPresentationLayer
{
    /// <summary>
    /// Interaction logic for frmUser.xaml
    /// </summary>
    public partial class frmUser : Window
    {
        private User _user = null;
        private IUserManager _userManager = null;
        private bool _addMode = false;

        public frmUser(IUserManager userManager)
        {
            InitializeComponent();
            _userManager = userManager;
            _addMode = true;
        }
        public frmUser(User user, IUserManager userManager)
        {
            InitializeComponent();

            _user = user;
            _userManager = userManager;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_addMode == false)
            {
                txtEmployeeID.Text = _user.UserID.ToString();
                txtFirstName.Text = _user.FirstName;
                txtLastName.Text = _user.LastName;
                txtEmailAddress.Text = _user.Email;
                chkActive.IsChecked = _user.Active;

                // populate the employee roles
                populateRoles();

            }
            else
            {
                SetEditMode();
                chkActive.IsChecked = true;
                chkActive.IsEnabled = false;
                lstUnassignedRoles.IsEnabled = false;
                lstAssignedRoles.IsEnabled = false;
            }
        }

        private void populateRoles()
        {
            try
            {
                var eRoles = _userManager.RetrieveUserRoles(_user.UserID);
                lstAssignedRoles.ItemsSource = eRoles;

                var roles = _userManager.RetrieveUserRoles();
                foreach (string r in eRoles)
                {
                    roles.Remove(r);
                }
                lstUnassignedRoles.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode();
        }

        private void SetEditMode()
        {
            txtFirstName.IsReadOnly = false;
            txtLastName.IsReadOnly = false;
            txtEmailAddress.IsReadOnly = false;
            chkActive.IsEnabled = true;

            lstAssignedRoles.IsEnabled = true;
            lstUnassignedRoles.IsEnabled = true;

            btnEdit.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            txtFirstName.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // error checks
            if (txtFirstName.Text == "")
            {
                MessageBox.Show("You must enter a first name.");
                txtFirstName.Focus();
                return;
            }
            if (txtLastName.Text == "")
            {
                MessageBox.Show("You must enter a last name.");
                txtLastName.Focus();
                return;
            }
            if (!(txtEmailAddress.Text.ToString().Length > 6
                  && txtEmailAddress.Text.ToString().Contains("@")
                  && txtEmailAddress.Text.ToString().Contains(".")))
            {
                MessageBox.Show("You must enter a valid email address.");
                txtEmailAddress.Focus();
                return;
            }

            User newUser = new User()
            {
                FirstName = txtFirstName.Text.ToString(),
                LastName = txtLastName.Text.ToString(),
                Email = txtEmailAddress.Text.ToString(),
                Active = (bool)chkActive.IsChecked
            };

            if (_addMode)
            {
                try
                {
                    if (_userManager.AddUser(newUser))
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
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
                    if (_userManager.EditUser(_user, newUser))
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ChkActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string caption = (bool)chkActive.IsChecked ? "Reactivate User" :
                    "Deactivate User";
                if (MessageBox.Show("Are you sure?", caption,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.No)
                {
                    chkActive.IsChecked = !(bool)chkActive.IsChecked;
                    return;
                }

                _userManager.SetUserActiveState((bool)chkActive.IsChecked, _user.UserID);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n"
                    + ex.InnerException.Message);
            }
        }

        private void LstUnassignedRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_addMode || lstUnassignedRoles.SelectedItems.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("Are you sure?", "Change Role Assignment",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                if (_userManager.AddUserRole(_user.UserID,
                    (string)lstUnassignedRoles.SelectedItem))
                {
                    populateRoles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }

        private void LstAssignedRoles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_addMode || lstAssignedRoles.SelectedItems.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("Are you sure?", "Change Role Assignment",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.No)
            {
                return;
            }
            try
            {
                if (_userManager.DeleteUserRole(_user.UserID,
                    (string)lstAssignedRoles.SelectedItem))
                {
                    populateRoles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message);
            }
        }
    }
}
