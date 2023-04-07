﻿using System.Globalization;
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
            manager.Payment();
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
        // Lists the available data from the database and returns the id chosen by the user
        public int GetID(string databaseName, string type, string displayColumn = "*", string? where = null)
        {
            Console.WriteLine($"Available {type}:");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"select {displayColumn} from {databaseName}";
            if(where != null)
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
        public void AddScoreBoard(int matchID, int score)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into scoreboard values({matchID}, {score})";
            command.ExecuteNonQuery();
        }
        public void RemoveSports()
        {
            int sportID = GetSportID();
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from sports where id={sportID}";
            command.ExecuteNonQuery();
        }
        public void EditScoreBoard()
        {
            int id = GetID("scoreboard", "scoreboard");
            Console.Write("Enter the new score: ");
            int newScore = Convert.ToInt32(Console.ReadLine()!);
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"update scoreboard set score={newScore} where id={id}";
            command.ExecuteNonQuery();
        }
        public void RemovePlayer()
        {
            int id = GetID("player", "player");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from player where id={id}";
            command.ExecuteNonQuery();
        }
        public void RemoveTournament()
        {
            int id = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"delete from tournament where id={id}";
            command.ExecuteNonQuery();
        }
        public void AddPlayer()
        {
            Console.Write("Enter the name of the player: ");
            string name = Console.ReadLine()!;
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into player values('{name}')";
            command.ExecuteNonQuery();
        }
        public void RegisterIndividual()
        {
            int playerID = GetID("player", "player");
            int tournamentID = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into registeredSport values({playerID}, 0, {tournamentID}, 0)";
            command.ExecuteNonQuery();
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
        }
        public void RegisterGroup()
        {
            int teamID = GetID("team", "team");
            int tournamentID = GetID("tournament", "tournament");
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"insert into registeredSport values(0, {teamID}, {tournamentID}, 0)";
            command.ExecuteNonQuery();
        }
        public void Payment()
        {
            Console.Write("Player or team(P/T)? ");
            string choice = Console.ReadLine()!.ToUpper();
            while(choice != "P" && choice != "T")
            {
                Console.Write("Invalid choice. Player or team(P/T)? ");
                choice = Console.ReadLine()!.ToUpper();
            }
            int id;
            if(choice == "P")
            {
                id = GetID("registeredSport", "player", "id, playerID", "playerID != 0");
            } else
            {
                id = GetID("registeredSport", "team", "id, teamID", "teamID != 0");
            }
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = $"update registeredSport set paid=1 where id={id}";
            command.ExecuteNonQuery();
        }
    }
}