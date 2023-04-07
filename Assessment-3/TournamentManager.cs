using Microsoft.Data.SqlClient;

namespace Assessment_3
{
    internal class TournamentManager
    {
        SqlConnection sqlConnection;
        const string connectionString = "Data Source=DESKTOP-IQRSRP8;Initial Catalog=sport;Integrated Security=True;Encrypt=False;";
        static void Main(string[] args)
        {
            TournamentManager manager = new();
        }
        public TournamentManager()
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }
        public bool AddSport(string name)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from sports where name='{name}'";
            if((int)command.ExecuteScalar() > 0)
            {
                return false;
            } else
            {
                command.CommandText = $"insert into sports values('{name}')";
                command.ExecuteNonQuery();
                return true;
            }
        }
    }
}