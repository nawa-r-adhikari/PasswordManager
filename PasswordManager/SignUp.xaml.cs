using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 15 FEB 2019
    // PURPOSE     : SignUp Window for iD Password Manager
    //              Lets user to signup for the application with email and master password
    //      
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // 
    //
    //==================================
    public partial class SignUp : Window
        
    {

        public SignUp()
        {
            //initializes components and Starts Window to the center of the screen
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //stores emails from text box to local variables
            string email = txtEmail.Text.ToString();
            string dbEmail = "";
            string pwd1 = txtPwd1.Password.ToString();
            string pwd2 = txtPwd2.Password.ToString();
            //Encrypts password provided by the user
            string encryptedPassword = StringCipher.Encrypt(pwd1, pwd1);

            //if both password fields match
            if(pwd1 == pwd2)
            {
                //Checks the validity of the email address provided
                if (IsEmailvalid(email))
                {
                    //password must be from 8 to 28 character long
                    //should contain at least one number, one Uppercase and one lowercase
                    if (IsPasswordValid(pwd1))
                    {
                        //checks if the database already has that email address
                        dbEmail = CheckExistingAccount(email);
                        //if database email matches the email provided by user
                        //Message box will show the warning
                        if (dbEmail == email)
                        {
                            MessageBox.Show("Email " + email +
                                " already exist in the Database. \nPlease use different email or LOGIN", "Existing Account", MessageBoxButton.OK);
                            btnSave.IsEnabled = false;
                        }
                        else
                        {
                            //if the email address doesn't match in the database
                            //saves email and password to the database
                            string message = SaveAccountToDB(email, encryptedPassword);
                            MessageBox.Show(message, "Account Created", MessageBoxButton.OK);
                            btnSave.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    //if the the email is in invalid format
                    MessageBox.Show("The email Address you provided is not in Valid format\n" +
                        "Please Try again", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //password and verify password doesn't match
                MessageBox.Show("Your password doesn't match", "Warning!", MessageBoxButton.OK);
            }

        }

        //method CheckExistingAccount checks if the email already exist in database
        public string CheckExistingAccount(string email)
        {
            //stored procedure prc_check_existing_account and email parameter is used
            //to check if the email exist in the database
            string commandText = "prc_check_existing_account";
            SqlParameter[] param =
            {
                new SqlParameter("@email", email),
            };

            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);
            //if the ProcessData method of DatabaseHandle class returns empty DataTable
            //it means that email doesn't exist
            if (dt.Rows.Count >= 1)
            {
                string dbEmail = dt.Rows[0][0].ToString();

                return dbEmail;
            }
            else
            {
                return email + " not found in database";
            }

        }

        //method that saves the login account to the database using 
        //ProcessData method of class DatabaseHandle passing parameter
        //prc_add_user stored procedure and encrypted master password.
        public string SaveAccountToDB(string email, string masterPwd)
        {

            string commandText = "prc_add_user";
            SqlParameter[] param =
            {
                new SqlParameter("@email", email),
                new SqlParameter("@master_pwd", masterPwd),
            };
            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);

            return "Account Added to Database Successfully";

        }

        //close button clears the text fields and closes the window
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            txtEmail.Clear();
            txtPwd1.Clear();
            txtPwd2.Clear();
            this.Close();
        }

        //Enables Save button when email changes in the textbox
        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }

        //method that validates the email address
        public bool IsEmailvalid(string email)
        {
            try
            {
                var eAddress = new System.Net.Mail.MailAddress(email);
                return eAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //Checks if the password is valid
        //Valid password should be between 8 to 28 characters length inclusive
        //should contain at least one upper case, one lower case and one digits
        public bool IsPasswordValid(string passWord)
        {
            const int min = 8;
            const int max = 28;
            bool validLength = passWord.Length >= min && passWord.Length <= max;
            bool upper = false;
            bool lower = false;
            bool num = false;

            if (validLength)
            {
                foreach(char c in passWord)
                {
                    if (char.IsUpper(c)) upper = true;
                    else if (char.IsLower(c)) lower = true;
                    else if (char.IsDigit(c)) num = true;
                }

                bool valid = upper && lower && num;
                if (valid)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Password should contain minimum of:\n" +
                        "1 upper case character\n" +
                        "1 lower case character\n" +
                        "1 numeric character", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                //if password length doesn't meet criteria of 8 to 28 characters
                MessageBox.Show("Password must be 8 to 28 Character in length", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
 
        }
    }

}
