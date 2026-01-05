using Microsoft.Data.Sqlite;
using Notebook.Data.Models;

namespace Notebook.Data.Repositories
{
    public class NotebookRepository
    {
        private readonly string _connectionString;

        public NotebookRepository(string connectionString)
        {
            _connectionString = $"Data Source={connectionString};Mode=ReadWriteCreate";
            InitDatabase();
        }

        private void InitDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Pages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PageNumber INTEGER NOT NULL UNIQUE,
                    Text TEXT NOT NULL
                );
                """;

            cmd.ExecuteNonQuery();
        }

        public void SavePage(Page page)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var pageCmd = connection.CreateCommand();
            pageCmd.CommandText = """
                INSERT INTO Pages (PageNumber, Text)
                VALUES ($num, $txt)
                ON CONFLICT(PageNumber) DO UPDATE SET Text = $txt;
                """;

            pageCmd.Parameters.AddWithValue("$num", page.PageNumber);
            pageCmd.Parameters.AddWithValue("$txt", page.Content);
            pageCmd.ExecuteNonQuery();

            transaction.Commit();
        }

        public Page LoadPage(int pageNumber)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var pageCmd = connection.CreateCommand();
            pageCmd.CommandText = "SELECT Text FROM Pages WHERE PageNumber = $num";
            pageCmd.Parameters.AddWithValue("$num", pageNumber);

            var text = (string)pageCmd.ExecuteScalar();
            return new Page() { PageNumber = 2, Content = text }; 
        }
    }
}
