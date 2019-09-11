using System.Windows;
using System.Data;
using System.Data.SqlClient;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for ChangeMasterPassword.xaml
    /// </summary>
    /// 
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 5 MARCH 2019
    // PURPOSE     : Change Master Password window for iD Password Manager, 
    //               user can Change the master password in database
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // Added Password validation
    //
    //==================================
    public partial class ChangeMasterPassword : Window
    {
        private HomeWindow hw; //object for HomeWindow

        //Initializes components and 
        //Starts window to the center of the screen
        public ChangeMasterPassword()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }

        //initializes components
        //Starts window to the center of the screen
        //this constructor is called when the object is passed
        public ChangeMasterPassword(HomeWindow homeWindow)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            hw = homeWindow;
        }

        //closes Change Master Password window when cancel button pressed
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //Verifies if the current password matches the password in database
        //verifies if the new password matches the verify password textboxes.
        //if old password matches the master password will be changed
        //All the account password in the database will be updated based on new master password
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string newPwd = txtNewPwd.Password.ToString();
            string verifyPwd = txtVerifyPwd.Password.ToString();
            string oldPwd = txtOldPwd.Password.ToString();
            string oldDbPwd = LoginInfo.MasterPwd;
            if (newPwd == verifyPwd)
            {
                if(oldPwd == oldDbPwd)
                {
                    //Changes master password from old to new
                    //Decrypts old accounts password and Encrypts with new accounts password
                    ChangeMp(oldDbPwd, newPwd);
                    MessageBox.Show("Your master password was changed successfully!", "Success!", MessageBoxButton.OK);
                    //Refresh Grid
                    hw.TextSearch();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Incorrect Current Password!", "Warning!", MessageBoxButton.OK);

                }
            }
            else if(txtNewPwd != txtVerifyPwd)
            {
                MessageBox.Show("Your new password and verify password doesn't match", "Warning!", MessageBoxButton.OK);
            }
        }


        //method ChangeMp, takes old password and changes into the database
        //once master password is updated
        //all the account password encrypted by old password will be decrypted 
        //and encrypted again with the new masterpassword
        public void ChangeMp(string oPwd, string nPwd)
        {
                //gets all the accounts from the database
                string commandText = "prc_get_account";
                SqlParameter[] param =
                {
                    new SqlParameter("user_ID", LoginInfo.UserId)
                };

                DatabaseHandle dbh = new DatabaseHandle();
                DataTable dt = new DataTable();
                dt = dbh.ProcessData(commandText, param);

                //update master password
                int uid = LoginInfo.UserId;
                string encMPwd = StringCipher.Encrypt(nPwd, nPwd);
                string commandText1 = "prc_update_master_pwd";
                SqlParameter[] param1 =
                {
                        new SqlParameter("user_ID", uid),
                        new SqlParameter("master_pwd", encMPwd)
                };
                DatabaseHandle dbh1 = new DatabaseHandle();
                DataTable dt1 = new DataTable();
                dt1 = dbh1.ProcessData(commandText1, param1);
                //update Login Info master password
                LoginInfo.MasterPwd = nPwd;

                //Update all the Account passwords
                int rows = dt.Rows.Count;
                int[] testArray = new int[rows];
                string commandText2 = "prc_update_acccount_pwd";
                foreach (DataRow dr in dt.Rows)
                {

                    int id = int.Parse(dr["account_ID"].ToString());
                    string tempPassword = (dr["password"].ToString());
                    string plainPwd = StringCipher.Decrypt(tempPassword, oPwd);
                    string encPwd = StringCipher.Encrypt(plainPwd, nPwd);

                    SqlParameter[] param2 =
                    {
                        new SqlParameter("account_ID", id),
                        new SqlParameter("password", encPwd)
                    };
                    DatabaseHandle dbh2 = new DatabaseHandle();
                    DataTable dt2 = new DataTable();
                    dt2 = dbh2.ProcessData(commandText2, param2);
                }

        }
    }
}
