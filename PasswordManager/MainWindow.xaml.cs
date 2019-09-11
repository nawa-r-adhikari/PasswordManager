using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 15 FEB 2019
    // PURPOSE     : Main window for iD Password Manager, user can SignUp and Login to HomeWindow
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // Added Password validation
    //
    //==================================
    public partial class MainWindow : Window
    {
        // Creates instance of HomeWindow
        HomeWindow hw = new HomeWindow();


        public MainWindow()
        {
            //Initializes Components and Starts the Window in the center of the screen
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            //Uncomment for testing
            //txtEmail.Text = "nadhikari2017@fit.edu";
            //txtMasterPwd.Password = "Fit5898Rocks!";


        }


        //When login button pressed, entered email and password is validated with
        //the database. If username and password is valid, it will let you login and opens HomeWindow
        //Once password is retrived from database it is saved in MasterPwd parameter of LoginInfo class
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
 
            string temp_masterPwd = txtMasterPwd.Password.ToString();
            string email = txtEmail.Text.ToString();
            GetDbPwd(email);
            //verfies if database password matches the userinput password
            if (temp_masterPwd == LoginInfo.MasterPwd)
            {

                hw.LoadDataDataGrid();
                hw.Show();
                this.Close();
            }
            else
            {
                lblLoginError.Visibility = System.Windows.Visibility.Visible;
            }

        }

        //for new user/account displays SignUp
        private void SignUp(object sender, RoutedEventArgs e)
        {
            SignUp su = new SignUp();
            su.Show();
        }

        //GetDbPwd method gets the password stored in the database
        //prc_get_accountpwd and sql parameter is passed to ProcessData method of class DatabaseHandle
        //If database query returns the password, it is validated against user typed password
        //If the database return empty datable meaning no password found for provied email
        private string GetDbPwd(string email)
        {
                string commandText = "prc_get_accountpwd";
                SqlParameter[] param =
                {
                    new SqlParameter("email", email)
                };
  
                DatabaseHandle dbh = new DatabaseHandle();
                DataTable dt = new DataTable();
                dt = dbh.ProcessData(commandText, param);
                
                //If query returns at least one row
                if (dt.Rows.Count >= 1)
                {
                    try
                    {
                        string txt_mp = txtMasterPwd.Password.ToString();
                        string encrypted_mp = dt.Rows[0][1].ToString(); //encrypted master password

                        //stores masterpassword, userId and email to the parameters in LoginInfo class
                        LoginInfo.MasterPwd = StringCipher.Decrypt(encrypted_mp, txt_mp);
                        LoginInfo.UserId = int.Parse(dt.Rows[0][0].ToString());
                        LoginInfo.Email = txtEmail.Text.ToString();

                        return LoginInfo.MasterPwd;
                    }
                    catch
                    {
                        return "Incorrect Password";
                    }
                }
                //if empty datatable is returned the provided email didn't have any password
                else
                {
                    return "No password found for provided email";
                }

        }

        //Hides LoginError when passwordbox text changes
        private void TxtMasterPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblLoginError.Visibility = System.Windows.Visibility.Hidden;
        }


    }


}


