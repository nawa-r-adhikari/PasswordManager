using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;


namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 15 FEB 2019
    // PURPOSE     : Home Window for iD Password Manager, user can add, delete, edit, generate passwords
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // Added password generator
    // Added search functionality
    // Added enabling and disabling of the buttons
    //
    //==================================
    public partial class HomeWindow : Window
    {

        public HomeWindow()
        {
            //Initialize components and loads window to the center of screen
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }


        //When user logs out from main app, brings to login window
        private void AppLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mw = new MainWindow();
            mw.InitializeComponent();
            mw.Show();


        }

        //When user click on signup button brings to sign up page
        private void Mainwindow_login(object sender, EventArgs args)
        {
            this.Show();
        }

        //Closes app when exit menu pressed
        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }

        //Brings password generator window, where user can generate complex password 
        // of length 1 to 28
        private void PwdGenerator_Click(object sender, RoutedEventArgs e)
        {
            RandomPasswordGenerator pwg = new RandomPasswordGenerator();
            pwg.InitializeComponent();
            pwg.Show();
        }

        //Brings up Add password window when add password menu pressed
        private void AddPwd_Click(object sender, RoutedEventArgs e)
        {
            AddNewPwd anp = new AddNewPwd(this);
            anp.InitializeComponent();
            anp.Show();

        }

        //Brings up Change Password window when Change Master password pressed
        private void ChangePwd_Click(object sender, RoutedEventArgs e)
        {
            ChangeMasterPassword cmp = new ChangeMasterPassword(this);
            cmp.Show();
        }

        //Loads the Datagrid in home window after user sucessfully logs in
        public void LoadDataDataGrid()
        {
            //passes parameter of stored procedure and user_ID to class DatabaseHandle
            //executes the SQL command in stored proc
            //If stored proc is to only write on database empty datatable is returned
            int userID = LoginInfo.UserId;
            string commandText = "prc_get_Account";
            SqlParameter[] param =
            {
                new SqlParameter("user_ID", userID)
            };
            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);
            dtaGridAccount.ItemsSource = dt.DefaultView;

        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            //encrypted password in stored in password box
            //when user clicks on view password, the encrypted password will be decrypted 
            //using master password and displayed in MaskBox textbox
            //ShowPassowrd button is toggled from View to Hide based on user intereaction

            string ePassword = txtPassword.Text.ToString();
            string maskPassword = txtMaskBox.Text.ToString();
            string uname = txtUserName.Text.ToString();

            if (btnShow.Content.ToString() == "View")
            {
                btnShow.Content = "Hide";
                txtMaskBox.Text = StringCipher.Decrypt(ePassword, LoginInfo.MasterPwd);
            }
            else if(btnShow.Content.ToString() == "Hide")
            {
                btnShow.Content = "View";
                txtMaskBox.Text = "********************";
            }
        }

        //Encrypted password on passwordbox is decrypted and when Copy Button is pressed
        //and copies the password to the ClipBoard 
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            string tempPwd = this.txtPassword.Text.ToString();
            Clipboard.SetText(StringCipher.Decrypt(tempPwd, LoginInfo.MasterPwd));
            MessageBoxResult result = MessageBox.Show(this, "Password Copied to Clipboard", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        //Edit button decrypts the password in passwordbox and enables the textbox maskbox and enables save button
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

            EnableEdit();
            string ePassword = txtPassword.Text.ToString();
            txtMaskBox.Text = StringCipher.Decrypt(ePassword, LoginInfo.MasterPwd);
            btnSave.IsEnabled = true;
            btnEdit.IsEnabled = false;
            EnableEdit();
        }

        //Enables Edit on textboxes maskbox, username and notes
        public void EnableEdit()
        {

            txtMaskBox.IsReadOnly = false;
            txtUserName.IsReadOnly = false;
            txtNotes.IsReadOnly = false;
        }

        //Makes textboxes readonly
        public void DisableEdit()
        {
            txtMaskBox.IsReadOnly = true;
            txtUserName.IsReadOnly = true;
            txtNotes.IsReadOnly = true;
        }

        //Deletes the selected cell data from the database
        //Calls DeletePassword method passing account_ID that is being deleted
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dtaGridAccount.SelectedItem;
            int account_ID = (int)row["account_ID"];
            DeletePassword(account_ID);
            MessageBox.Show("Password Deleted Sucessfully", "Password Delete", MessageBoxButton.OK);
            LoadDataDataGrid();
        }

        //Saves the Edit made to the passwords
        //Changes made to the password is Encrypted before saving it to the datbase
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string ePassword = StringCipher.Encrypt(txtMaskBox.Text.ToString(), LoginInfo.MasterPwd);
            DataRowView row = (DataRowView)dtaGridAccount.SelectedItem;
            int account_ID = (int)row["account_ID"];
            EditPassword(account_ID,  ePassword);
            btnEdit.IsEnabled = false;
            btnSave.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnShow.IsEnabled = false;
            txtMaskBox.Text = "";
            DisableEdit();
            MessageBox.Show("Password changed in Database Sucessfully", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDataDataGrid();
        }

        //passes the accountID that is being deleted to the DatabaseHandle class
        //Deletes the password from the database
        //Stored Procedure to delete password is passed to the DatabaseHandle class
        public void DeletePassword(int accountID)
        {
            string commandText = "prc_del_accountpwd";
            SqlParameter[] param =
            {
                new SqlParameter("account_ID", accountID)
            };
            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);

        }


        //passes the accountID that is being Edited to the DatabaseHandle class
        //Updates the password to the database
        //Stored Procedure to Edit password is passed to the DatabaseHandle class
        public void EditPassword(int accountID, string passCode)
        {
            string commandText = "prc_edit_accountpwd";
            SqlParameter[] param =
            {
                new SqlParameter("account_ID", accountID),
                new SqlParameter("password", passCode)
            };

            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);

        }

        //Enables and disables buttons and textbox when the datagrid currentcell changed
        private void DtaGridAccount_CurrentCellChanged(object sender, EventArgs e)
        {
            string ePassword = txtPassword.Text.ToString();
            btnSave.IsEnabled = false;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnShow.IsEnabled = true;
            DisableEdit();
            txtMaskBox.Text = "********************";
            btnShow.Content = "View";
        }

        //Calls method TextSearch when search button is pressed
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            TextSearch();
        }

        //Method TextSearch filters the data using SQL command through stored procedure
        //the filtered result is passed to the datagrid display defaultview
        //method ProcessData in DatabaseHandle class filters the result using SQL command using stored proc
        public void TextSearch()
        {
            int userID = LoginInfo.UserId;
            
            string commandText = "prc_get_account_search";
            SqlParameter[] param =
            {
                new SqlParameter("user_ID", userID),
                new SqlParameter("search_text", txtSearchBox.Text.ToString())
            };
            DatabaseHandle dbh = new DatabaseHandle();
            DataTable dt = new DataTable();
            dt = dbh.ProcessData(commandText, param);
            string dataTableAccounts = dt.Rows[0][0].ToString();
            dtaGridAccount.ItemsSource = dt.DefaultView;

        }

        //calls TextSearch method every time text in searchbox changes
        private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextSearch();
        }

        // Displays About page when about menu is pressed
        private void About_Click(object sender, RoutedEventArgs e)
        {
            About ab = new About();
            ab.Show();
        }


    }
}
