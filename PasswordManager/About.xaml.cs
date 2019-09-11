
using System.Windows;


namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 15 FEB 2019
    // PURPOSE     : About Window for iD Password Manager, user can find information about the application
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    //
    //==================================

    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
