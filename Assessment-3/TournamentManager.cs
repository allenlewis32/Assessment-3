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
            manager.AddScoreBoard(1, 50);
        }
        public TournamentManager()
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }
        public bool AddSport(string name)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from sports where name='{name}'"; // check whether the sport already exists
            if ((int)command.ExecuteScalar() > 0)
            {
                return false;
            }
            else
            {
                command.CommandText = $"insert into sports values('{name}')";
                command.ExecuteNonQuery();
                return true;
            }
        }
        public bool AddTournament(string name, string sport)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from tournament where name='{name}'"; // check whether the tournament already exists
            if ((int)command.ExecuteScalar() > 0)
            {
                return false;
            }
            else
            {
                command.CommandText = $"select id from sports where name='{name}'";
                int sportID = (int)command.ExecuteScalar();
                command.CommandText = $"insert into tournament values('{name}', {sportID})";
                command.ExecuteNonQuery();
                return true;
            }
        }
        public void AddScoreBoard(int matchID, int score)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into scoreboard values({matchID}, {score})";
            command.ExecuteNonQuery();
        }
    }
}