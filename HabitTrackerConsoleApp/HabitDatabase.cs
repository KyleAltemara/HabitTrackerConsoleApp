using Microsoft.Data.Sqlite;

namespace HabitTrackerConsoleApp;

internal class HabitDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public HabitDatabase(string databasePath)
    {
        // If the file does not exist, it will be created
        _connection = new SqliteConnection($"Data Source={databasePath}");
        _connection.Open();
        CreateHabitsTable();
    }

    public void Dispose()
    {
        // The using statement will call this method automatically
        // when the object goes out of scope
        ((IDisposable)_connection).Dispose();
    }

    private void CreateHabitsTable()
    {
        string createTableQuery = @"CREATE TABLE IF NOT EXISTS habits (name TEXT PRIMARY KEY,unit TEXT);";
        using var command = new SqliteCommand(createTableQuery, _connection);
        command.ExecuteNonQuery();
    }

    public bool AddNewHabit(string habitName, string unit)
    {
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        if (habitCount == 0)
        {
            // Create the habit table using the name
            string createHabitTableQuery = $"CREATE TABLE IF NOT EXISTS {habitName} (id INTEGER PRIMARY KEY AUTOINCREMENT, quantity INTEGER, timestamp TEXT);";
            using var createHabitTableCommand = new SqliteCommand(createHabitTableQuery, _connection);
            createHabitTableCommand.ExecuteNonQuery();

            // Insert the habit into the habits table
            string insertHabitQuery = $"INSERT INTO habits (name, unit) VALUES ('{habitName}', '{unit}');";
            using var insertHabitCommand = new SqliteCommand(insertHabitQuery, _connection);
            insertHabitCommand.ExecuteNonQuery();
        }

        return habitCount == 0;
    }

    public bool LogHabit(string habitName, int quantity)
    {
        var timestamp = DateTime.Now;
        // Check if the habit already exists in the habits table
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        if (habitCount == 0)
        {
            return false;
        }

        // Add the habit to the new table
        string addHabitQuery = $"INSERT INTO {habitName} (quantity, timestamp) VALUES ({quantity}, '{timestamp:yyyy-MM-dd HH:mm:ss}');";
        using var addHabitCommand = new SqliteCommand(addHabitQuery, _connection);
        addHabitCommand.ExecuteNonQuery();
        return true;
    }

    public bool DeleteHabitTable(string habitName)
    {

        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        if (habitCount == 0)
        {
            return false;
        }

        // Delete the habit from the habits table
        string deleteHabitQuery = $"DELETE FROM habits WHERE name = '{habitName}';";
        using var deleteHabitCommand = new SqliteCommand(deleteHabitQuery, _connection);
        deleteHabitCommand.ExecuteNonQuery();
        // Delete the habit table
        string deleteHabitTableQuery = $"DROP TABLE {habitName};";
        using var deleteHabitTableCommand = new SqliteCommand(deleteHabitTableQuery, _connection);
        deleteHabitTableCommand.ExecuteNonQuery();
        return true;
    }

    public bool DeleteLoggedHabit(string habitName, int id)
    {
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        if (habitCount == 0)
        {
            return false;
        }

        // Delete the log entry from the habit table
        string deleteHabitQuery = $"DELETE FROM {habitName} WHERE id = {id};";
        using var deleteHabitCommand = new SqliteCommand(deleteHabitQuery, _connection);
        return deleteHabitCommand.ExecuteNonQuery() > 0;
    }

    public bool UpdateHabit(string oldHabitName, string oldUnit, string newHabitName = "", string newUnit = "")
    {
        if (newHabitName == "" && newUnit == "")
        {
            throw new ArgumentException("At least one of the new habit name or unit must be provided.");
        }

        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{oldHabitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());
        if (habitCount == 0)
        {
            return false;
        }

        if (newHabitName == "")
        {
            newHabitName = oldHabitName;
        }
        else if (newUnit == "")
        {
            newUnit = oldUnit;
        }

        string updateHabitQuery = $"UPDATE habits SET name = '{newHabitName}', unit = '{newUnit}' WHERE name = '{oldHabitName}';";
        using var updateHabitCommand = new SqliteCommand(updateHabitQuery, _connection);
        updateHabitCommand.ExecuteNonQuery();
        return true;
    }

    public List<(string habitName, string unit)> GetAllHabits()
    {
        var ret = new List<(string habitName, string unit)>();
        string selectQuery = "SELECT * FROM habits;";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            ret.Add((reader.GetString(0), reader.GetString(1)));
        }

        return ret;
    }

    public Dictionary<(string habitName, string unit), List<(int id, int quantity, string timeStamp)>> GetAllHabitsAndLogs()
    {
        var ret = new Dictionary<(string habitName, string unit), List<(int id, int quantity, string timeStamp)>>();
        string selectQuery = "SELECT * FROM habits;";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            string habitName = reader.GetString(0);
            string unit = reader.GetString(1);
            string selectHabitQuery = $"SELECT id, quantity, timestamp FROM {habitName};";
            using var selectHabitCommand = new SqliteCommand(selectHabitQuery, _connection);
            using var habitReader = selectHabitCommand.ExecuteReader();
            var habitList = new List<(int id, int quantity, string timeStamp)>();
            while (habitReader.Read())
            {
                habitList.Add((habitReader.GetInt32(0), habitReader.GetInt32(1), habitReader.GetString(2)));
            }

            ret.Add((habitName, unit), habitList);
        }

        return ret;
    }

    public List<(int id, int quantity, string timeStamp)>? GetHabit(string habitName, out string unit)
    {
        string selectHabitQuery = $"SELECT unit FROM habits WHERE name = '{habitName}';";
        using var selectHabitCommand = new SqliteCommand(selectHabitQuery, _connection);
        unit = selectHabitCommand.ExecuteScalar()?.ToString() ?? "";
        if (unit == "")
        {
            return null;
        }

        selectHabitQuery = $"SELECT id, quantity, timestamp FROM {habitName};";
        using var selectHabitCommand2 = new SqliteCommand(selectHabitQuery, _connection);
        using var reader = selectHabitCommand2.ExecuteReader();
        var habitList = new List<(int id, int quantity, string timeStamp)>();
        while (reader.Read())
        {
            habitList.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
        }

        return habitList;
    }

    public int ReportNumberOfTimes(int id)
    {
        return 0;
    }

    public int ReportTotalQuantity(int id)
    {
        return 0;
    }
}
