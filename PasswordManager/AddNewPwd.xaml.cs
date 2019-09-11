using System;
using System.Windows;
using System.Data;
using System.Data.SqlClient;


namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for AddNewPwd.xaml
    /// </summary>
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 25 FEB 2019
    // PURPOSE     : Add new password Window for iD Password Manager, user can Add password to their Login Account
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // Added password validation
    //
    //==================================
    public partial class AddNewPwd : Window
    {

        HomeWindow hw; //Object for HomeWindow

        //Initializes components and starts window to the center of the screen
        public AddNewPwd()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }

        //Constructor that takes HomeWindow Object as parameter
        public AddNewPwd(HomeWindow homeWindow)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            hw = homeWindow;
        }

        //Validates username and password field
        //Saves username and password to the database after valid username and password entered
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            string mp = LoginInfo.MasterPwd;
            int id = LoginInfo.UserId;
            if (txtAccountName.Text.ToString() == "" || txtPwd.Password.ToString() == "")
            {
                MessageBox.Show("Username or Password Field cannot be Empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                //calls AddPasswordDb() method to add username and password after validation
                string message = "Password Sucessfully Added";
                AddPasswordDb();
                MessageBox.Show(message, "Password Add", MessageBoxButton.OK);
                this.Close();
            }
            //Refreshes the DataGrid in HomeWindow
            hw.TextSearch();

        }

        //button cancel closes Add New Password window
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //Saves username and password to the database
        //calls ProcessData method of class DatabaseHandle
        //Procedure prc_add_pwd and sql parameter is passed to ProcessData of class DatabaseHandle
        //password provided by the user is encrypted before saving to database
        //using Encrypt method of StringCipher class
        public void AddPasswordDb()
        {
            string accountName = txtAccountName.Text.ToString();
            string userName = txtUsrName.Text.ToString();
            string password = txtPwd.Password.ToString();
            string encryptedPassword = StringCipher.Encrypt(password, LoginInfo.MasterPwd);
            string notes = txtNote.Text.ToString();
            var date = DateTime.Now.ToString("yyyy/MM/dd");

            string commandText = "prc_add_pwd";
            SqlParameter[] param =
            {
                new SqlParameter("@account_name", accountName),
                new SqlParameter("@user_ID", LoginInfo.UserId),
                new SqlParameter("@username", userName),
                new SqlParameter("@password", encryptedPassword),
                new SqlParameter("@notes", notes),
            };
            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);


        }
    }
}
