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
            manager.AddTournament();
        }
        public TournamentManager()
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }
        public bool AddSport()
        {
            Console.Write("Enter the name of the sport: ");
            string sport = Console.ReadLine()!;
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from sports where name='{sport}'"; // check whether the sport already exists
            if ((int)command.ExecuteScalar() > 0)
            {
                return false;
            }
            else
            {
                command.CommandText = $"insert into sports values('{sport}')";
                command.ExecuteNonQuery();
                return true;
            }
        }
        public bool AddTournament()
        {
            Console.Write("Enter the name of the tournament: ");
            string name = Console.ReadLine()!;
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from tournament where name='{name}'"; // check whether the tournament already exists
            if ((int)command.ExecuteScalar() > 0)
            {
                return false;
            }
            else
            {
                int sportID = GetSportID();
                command.CommandText = $"insert into tournament values('{name}', {sportID})";
                command.ExecuteNonQuery();
                return true;
            }
        }
        // Lists the available sports and returns the id of the sport chosen by the user
        public int GetSportID()
        {
            Console.WriteLine("Available sports:");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = "select * from sports";
            List<int> sports = new();
            int id;
            using(SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    sports.Add(id);
                    Console.WriteLine($"{sports.Count}. {name}");
                }
            }
            Console.WriteLine("Choose the sport you want: ");
            int k = Convert.ToInt32(Console.ReadLine()!);
            while(k < 1 || k > sports.Count)
            {
                Console.Write("Invalid Choice. Choose the sport you want: ");
                k = Convert.ToInt32((Console.ReadLine()!));
            }
            return sports[k - 1];
        }
        public void AddScoreBoard(int matchID, int score)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into scoreboard values({matchID}, {score})";
            command.ExecuteNonQuery();
        }
        public void RemoveSports(string sport)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from sports where name='{sport}'";
            command.ExecuteNonQuery();
        }
    }
}