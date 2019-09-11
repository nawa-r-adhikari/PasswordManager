using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for PasswordGenerator.xaml
    /// </summary>
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 15 FEB 2019
    // PURPOSE     : Password Generator window for iD Password Manager
    //               Can be used to Generate password with different complexity
    //               simple password without numbers and symbols, with alphabets and number or all three
    //               Pin numbers with only numbers
    //              
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // 
    //
    //==================================
    public partial class RandomPasswordGenerator : Window
    {
        //Variables for different password complexity
        private static string lCase = "abcdefgijkmnopqrstwxyz";
        private static string uCase = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string num = "23456789";
        private static string spec = "*$-+?_&=!%{}/";
        //All three check box are checked by default
        private bool letterChkd = true, digitChkd = true, symbolsChkd = true;

        public RandomPasswordGenerator()
        {
            InitializeComponent();
            //Starts Window to the center of the screen
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            //Refreshes password on startup
            RefreshPassword();
            
        }

        private void PwdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Refreshes password when the slider value changes
            RefreshPassword();

        }

        //generates new set of password every time RefreshPassword method is called
        public void RefreshPassword()
        {
            //Gets the password length from the slider
            int pwdLen = (int)this.pwdSlider.Value;
            //initialize password to null
            string pwd = "";

            //Checks various condition where different check boxes are checked
            if (letterChkd && digitChkd && symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, lCase, uCase, num, spec);
            else if (!letterChkd && digitChkd && symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, num, spec, num, spec);
            else if (!letterChkd && !digitChkd && symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, spec, spec, spec, spec);

            else if (letterChkd && !digitChkd && symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, lCase, uCase, spec, spec);

            else if (letterChkd && digitChkd && !symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, lCase, uCase, num, num);

            else if (letterChkd && !digitChkd && !symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, lCase, uCase, lCase, uCase);
            else if (!letterChkd && digitChkd && !symbolsChkd)
                pwd = RandPasswordGenerator.Generate(pwdLen, num, num, num, num);
            else
                pwd = ""; //if none of the check box are checked password will be null
            //displays the password in password box
            this.pwdTextBox.Text = pwd;
                    
        }

        //event when refresh password button is pressed
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPassword();
        }

        //toggle switch for check box checked or unchecked event
        private void ChkLetters_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chkLetters.IsChecked)
                this.letterChkd = true;
            else
                this.letterChkd = false;


            RefreshPassword();
        }

        //toggle switch for check box checked or unchecked event
        private void ChkDigits_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chkDigits.IsChecked)
                this.digitChkd = true;
            else
                this.digitChkd = false;

            RefreshPassword();
        }

        //toggle switch for check box checked or unchecked event
        private void ChkSymbols_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chkSymbols.IsChecked)
                this.symbolsChkd = true;
            else
                this.symbolsChkd = false;

            RefreshPassword();
        }

        //Selects all the password in textbox when double clicked
        private void PwdTextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).SelectAll();
        }

        //Copies password to the Clipboard
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.pwdTextBox.Text.ToString());
            MessageBox.Show("Password Copied to the ClipBoard", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        //Refreshes password when chkLetters checkbox is checked
        private void ChkLetters_Checked(object sender, RoutedEventArgs e)
        {
            RefreshPassword();
        }
    }
}
