using System;
using System.Data;
using System.Data.SqlClient;



namespace PasswordManager
{
    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 10 MARCH 2019
    // PURPOSE     : DatabaseHandle class for iD Password Manager, 
    //               any database call will use this class.
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // 
    //
    //==================================
    public class DatabaseHandle
    {
        //Parameters used in sucessful database connection
        private readonly string connectionstring;
        private SqlConnection connection;
        private SqlDataAdapter adapter;
        private SqlCommand command;

        //constructor that initializes the parameters used in database connection
        public DatabaseHandle()
        {
            command = new SqlCommand();
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            connection = new SqlConnection(connectionstring);
        }

        //method ProcessData takes command text and sql parameter as an argument
        //returns the DataTable after execution of the Stored Procedure execution
        public DataTable ProcessData(string cmdTxt, SqlParameter[] param)
        {
            try {
                connection.Open();
                command.Connection = connection;
                command.CommandText = cmdTxt;
                command.Parameters.AddRange(param);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adp = new SqlDataAdapter(command);
                adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                return dt;

            }
            catch (Exception)
            {
                //if any error in database returns empty DataTable
                return new DataTable();
            }
        }



    }
}
