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

            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Bookmarks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PageNumber INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    FOREIGN KEY (PageNumber) REFERENCES Pages(PageNumber)
                );
                """;
            cmd.ExecuteNonQuery();

            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Settings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Theme TEXT NOT NULL,
                    BaseTheme TEXT NOT NULL,
                    LineDrawing INTEGER NOT NULL
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

        public void SaveSettings(Settings settings)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var Cmd = connection.CreateCommand();
            Cmd.CommandText = """
                UPDATE Settings SET Theme=$theme, BaseTheme=$baseTheme, LineDrawing=$ld WHERE Id = 1;
                """;

            Cmd.Parameters.AddWithValue("theme", settings.Theme);
            Cmd.Parameters.AddWithValue("$ld", settings.DarawingLines);
            Cmd.Parameters.AddWithValue("$baseTheme", settings.BaseTheme);
            Cmd.ExecuteNonQuery();

            transaction.Commit();
        }

        public Settings LoadSettings()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var Cmd = connection.CreateCommand();
            Cmd.CommandText = "SELECT * FROM Settings WHERE Id = 1;";

            var obj = Cmd.ExecuteReader();

            string theme;
            string baseTheme;
            int dl;
            Settings settings;

            if (obj.HasRows)
            {
                obj.Read();
                theme = obj.GetString(1);
                baseTheme = obj.GetString(2);
                dl = obj.GetInt32(3);
                settings = new Settings { Theme = theme, BaseTheme = baseTheme, DarawingLines = dl };
            }
            else
            {
                theme = "Dark";
                baseTheme = "Dark";
                dl = 1;
                settings = new Settings { Theme = theme, BaseTheme = baseTheme, DarawingLines = dl };

                obj.Close();
                Cmd.CommandText = "INSERT INTO Settings(Theme, BaseTheme, LineDrawing) VALUES($theme, $baseTheme, $ld)";
                Cmd.Parameters.AddWithValue("theme", settings.Theme);
                Cmd.Parameters.AddWithValue("$ld", settings.DarawingLines);
                Cmd.Parameters.AddWithValue("$baseTheme", settings.BaseTheme);
                Cmd.ExecuteNonQuery();
            }

            return settings;
        }
    }
}
