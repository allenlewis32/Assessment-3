using System.Globalization;
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
            bool running = true;
            while (running)
            {
                Console.Write("1. Add sport\n2. Add tournament\n3. Add scoreboard\n4. Remove sport\n5. Edit scoreboard\n6. Remove player\n7. Remove tournament\n8. Add player\n9. Add team\n10. Register individual\n11. Register team\n12. Pay fee\n13. View scoreboard\n0 to exit\nEnter your choice: ");
                int choice = Convert.ToInt32(Console.ReadLine()!);
                switch (choice)
                {
                    case 0:
                        running = false;
                        break;
                    case 1:
                        manager.AddSport();
                        break;
                    case 2:
                        manager.AddTournament();
                        break;
                    case 3:
                        manager.AddScoreBoard();
                        break;
                    case 4:
                        manager.RemoveSports();
                        break;
                    case 5:
                        manager.EditScoreBoard();
                        break;
                    case 6:
                        manager.RemovePlayer();
                        break;
                    case 7:
                        manager.RemoveTournament();
                        break;
                        case 8:
                        manager.AddPlayer();
                        break;
                    case 9:
                        manager.AddTeam();
                        break;
                    case 10:
                        manager.RegisterIndividual();
                        break;
                    case 11:
                        manager.RegisterGroup();
                        break;
                    case 12:
                        manager.Payment();
                        break;
                    case 13:
                        manager.ViewScoreBoard();
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
        }
        public TournamentManager()
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }
        public void AddSport()
        {
            Console.Write("Enter the name of the sport: ");
            string sport = Console.ReadLine()!;
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from sports where name='{sport}'"; // check whether the sport already exists
            if ((int)command.ExecuteScalar() > 0)
            {
                Console.WriteLine("Sport already exists");
            }
            else
            {
                command.CommandText = $"insert into sports values('{sport}')";
                command.ExecuteNonQuery();
                Console.WriteLine("Sport added");
            }
        }
        public void AddTournament()
        {
            Console.Write("Enter the name of the tournament: ");
            string name = Console.ReadLine()!;
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select count(*) from tournament where name='{name}'"; // check whether the tournament already exists
            if ((int)command.ExecuteScalar() > 0)
            {
                Console.WriteLine("Tournament already exists");
            }
            else
            {
                int sportID = GetSportID();
                command.CommandText = $"insert into tournament values('{name}', {sportID})";
                command.ExecuteNonQuery();
                Console.WriteLine("Tournament added");
            }
        }
        // Lists the available data from the database and returns the id chosen by the user
        public int GetID(string databaseName, string type, string displayColumn = "*", string? where = null)
        {
            Console.WriteLine($"Available {type}:");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select {displayColumn} from {databaseName}";
            if (where != null)
            {
                command.CommandText += $" where {where}";
            }
            List<int> ids = new();
            int id;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                    ids.Add(id);
                    Console.WriteLine($"{ids.Count}. {reader[1]}");
                }
            }
            Console.Write($"Choose the {type} you want: ");
            int k = Convert.ToInt32(Console.ReadLine()!);
            while (k < 1 || k > ids.Count)
            {
                Console.Write($"Invalid Choice. Choose the {type} you want: ");
                k = Convert.ToInt32((Console.ReadLine()!));
            }
            return ids[k - 1];
        }
        public int GetSportID()
        {
            return GetID("sports", "sport");
        }
        public void AddScoreBoard()
        {
            int tournamentID = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into scoreboard values({tournamentID}, 0)";
            command.ExecuteNonQuery();
            Console.WriteLine("Scoreboard added");
        }
        public void RemoveSports()
        {
            int sportID = GetSportID();
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from sports where id={sportID}";
            command.ExecuteNonQuery();
            Console.WriteLine("Sport deleted");
        }
        public void EditScoreBoard()
        {
            int id = GetID("scoreboard", "scoreboard");
            Console.Write("Enter the new score: ");
            int newScore = Convert.ToInt32(Console.ReadLine()!);
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"update scoreboard set score={newScore} where id={id}";
            command.ExecuteNonQuery();
            Console.WriteLine("Sport edited");
        }
        public void RemovePlayer()
        {
            int id = GetID("player", "player");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from player where id={id}";
            command.ExecuteNonQuery();
            Console.WriteLine("Player removed");
        }
        public void RemoveTournament()
        {
            int id = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from tournament where id={id}";
            command.ExecuteNonQuery();
            Console.WriteLine("Tournament remove");
        }
        public void AddPlayer()
        {
            Console.Write("Enter the name of the player: ");
            string name = Console.ReadLine()!;
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into player values('{name}')";
            command.ExecuteNonQuery();
            Console.WriteLine("Player added");
        }
        public void RegisterIndividual()
        {
            int playerID = GetID("player", "player");
            int tournamentID = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into registeredSport values({playerID}, 0, {tournamentID}, 0)";
            command.ExecuteNonQuery();
            Console.WriteLine("Registered");
        }
        public void AddTeam()
        {
            Console.Write("Enter the number of players: ");
            int numPlayers = Convert.ToInt32(Console.ReadLine()!);
            int playerID = GetID("player", "player");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into team values({playerID}); select scope_identity()";
            int teamID = Convert.ToInt32(command.ExecuteScalar());
            while (--numPlayers > 0)
            {
                playerID = GetID("player", "player");
                command.CommandText = $"set identity_insert team on;insert into team(id, playerID) values({teamID}, {playerID})";
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Team added");
        }
        public void RegisterGroup()
        {
            int teamID = GetID("team", "team");
            int tournamentID = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into registeredSport values(0, {teamID}, {tournamentID}, 0)";
            command.ExecuteNonQuery();
            Console.WriteLine("Registered");
        }
        public void Payment()
        {
            Console.Write("Player or team(P/T)? ");
            string choice = Console.ReadLine()!.ToUpper();
            while (choice != "P" && choice != "T")
            {
                Console.Write("Invalid choice. Player or team(P/T)? ");
                choice = Console.ReadLine()!.ToUpper();
            }
            int id;
            if (choice == "P")
            {
                id = GetID("registeredSport", "player", "id, playerID", "playerID != 0");
            }
            else
            {
                id = GetID("registeredSport", "team", "id, teamID", "teamID != 0");
            }
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"update registeredSport set paid=1 where id={id}";
            command.ExecuteNonQuery();
            Console.WriteLine("Payment done");
        }
        public void ViewScoreBoard()
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = "select * from scoreboard";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i].ToString() + " ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}