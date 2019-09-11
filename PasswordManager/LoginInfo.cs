
namespace PasswordManager
{

    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 8 APRIL 2019
    // PURPOSE     : LoginInfo class for iD Password Manager
    //               Stores UserId, email and masterPwd during application runtime
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    //
    //
    //==================================
    public static class LoginInfo
    {
        //Paratemeter for userID, email and masterPwd
        private static int userId = 0;
        private static string email = "";
        private static string masterPwd = "";

        //Getter and Setter method MasterPwd
        public static string MasterPwd { get => masterPwd; set => masterPwd = value; }
        //Getter and Setter method for Email
        public static string Email { get => email; set => email = value; }
        //Getter and Setter method for UserId
        public static int UserId { get => userId; set => userId = value; }


    }
}
