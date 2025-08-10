using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Edge
{
    public class WordSearchEngine : IDisposable
    {
        private readonly SqliteConnection _connection;

        public WordSearchEngine(string filePath)
        {
            _connection = new SqliteConnection($"Data Source={filePath}");
            _connection.Open();
        }
        
        public IEnumerable<string> SearchWords(string prefix, int limit = 10)
        {
            var results = new List<string>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT Word FROM Words WHERE Word LIKE @prefix LIMIT @limit";
            cmd.Parameters.AddWithValue("@prefix", prefix + "%");
            cmd.Parameters.AddWithValue("@limit", limit);
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }
            return results;
        }
        
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
