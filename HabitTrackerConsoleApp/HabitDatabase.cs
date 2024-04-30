using Microsoft.Data.Sqlite;

namespace HabitTrackerConsoleApp;

/// <summary>
/// Represents a habit database.
/// </summary>
internal class HabitDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="HabitDatabase"/> class.
    /// </summary>
    /// <param name="databasePath">The path to the database file.</param>
    public HabitDatabase(string databasePath)
    {
        // If the file does not exist, it will be created
        _connection = new SqliteConnection($"Data Source={databasePath}");
        _connection.Open();
        CreateHabitsTable();
    }

    /// <summary>
    /// Disposes the habit database.
    /// </summary>
    public void Dispose()
    {
        // The using statement will call this method automatically
        // when the object goes out of scope
        ((IDisposable)_connection).Dispose();
    }

    /// <summary>
    /// Creates the habits table if it does not exist.
    /// </summary>
    private void CreateHabitsTable()
    {
        // Create the habits table if it does not exist
        // The table will contain the name of the habit and its unit
        // The name of the habit will be the primary key
        // The unit will be the unit of the habit
        string createTableQuery = @"CREATE TABLE IF NOT EXISTS habits (name TEXT PRIMARY KEY,unit TEXT);";
        using var command = new SqliteCommand(createTableQuery, _connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Adds a new habit to the database.
    /// </summary>
    /// <param name="habitName">The name of the habit.</param>
    /// <param name="unit">The unit of the habit.</param>
    /// <returns>True if the habit is added successfully; otherwise, false.</returns>
    public bool AddNewHabit(string habitName, string unit)
    {
        //SELECT takes the table name and the column name(s) to select
        //COUNT(*) counts the number of rows that match the condition
        //WHERE name = '{habitName}' is the condition to match the habit name
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        // Check if the habit already exists in the habits table
        if (habitCount == 0)
        {
            // Create the habit table using the name
            // The table will contain the ID, quantity, and timestamp of the habit
            // The ID will be the primary key, and it will autoincrement
            //CREATE TABLE takes the table name and the column name(s) to create
            //IF NOT EXISTS ensures that the table is created only if it does not exist
            string createHabitTableQuery = $"CREATE TABLE IF NOT EXISTS {habitName} (id INTEGER PRIMARY KEY AUTOINCREMENT, quantity INTEGER, timestamp TEXT);";
            using var createHabitTableCommand = new SqliteCommand(createHabitTableQuery, _connection);
            createHabitTableCommand.ExecuteNonQuery();

            // Insert the habit into the habits table
            // The habits table keeps track of the name and unit of the habit
            // and the habit table name is the habit name
            //INSERT INTO takes the table name and the column name(s) to insert
            //VALUES takes the values to insert
            string insertHabitQuery = $"INSERT INTO habits (name, unit) VALUES ('{habitName}', '{unit}');";
            using var insertHabitCommand = new SqliteCommand(insertHabitQuery, _connection);
            insertHabitCommand.ExecuteNonQuery();
        }

        return habitCount == 0;
    }

    /// <summary>
    /// Logs a habit entry in the database.
    /// </summary>
    /// <param name="habitName">The name of the habit.</param>
    /// <param name="quantity">The quantity of the habit.</param>
    /// <returns>True if the habit is logged successfully; otherwise, false.</returns>
    public bool LogHabit(string habitName, int quantity)
    {
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        if (habitCount == 0)
        {
            return false;
        }

        // Add the habit to the new table
        string addHabitQuery = $"INSERT INTO {habitName} (quantity, timestamp) VALUES ({quantity}, '{DateTime.Now:yyyy-MM-dd HH:mm:ss}');";
        using var addHabitCommand = new SqliteCommand(addHabitQuery, _connection);
        addHabitCommand.ExecuteNonQuery();
        return true;
    }

    /// <summary>
    /// Deletes a habit and its associated table from the database.
    /// </summary>
    /// <param name="habitName">The name of the habit.</param>
    /// <returns>True if the habit is deleted successfully; otherwise, false.</returns>
    public bool DeleteHabitTable(string habitName)
    {

        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        // Check if the habit exists in the habits table
        if (habitCount == 0)
        {
            return false;
        }

        // Delete the habit from the habits table
        // DELETE takes the table name and the condition to match the habit name
        string deleteHabitQuery = $"DELETE FROM habits WHERE name = '{habitName}';";
        using var deleteHabitCommand = new SqliteCommand(deleteHabitQuery, _connection);
        deleteHabitCommand.ExecuteNonQuery();
        // Delete the habit table
        // DROP TABLE takes the table name to delete
        string deleteHabitTableQuery = $"DROP TABLE {habitName};";
        using var deleteHabitTableCommand = new SqliteCommand(deleteHabitTableQuery, _connection);
        deleteHabitTableCommand.ExecuteNonQuery();
        return true;
    }

    /// <summary>
    /// Deletes a logged habit entry from the database.
    /// </summary>
    /// <param name="habitName">The name of the habit.</param>
    /// <param name="id">The ID of the logged habit entry.</param>
    /// <returns>True if the logged habit entry is deleted successfully; otherwise, false.</returns>
    public bool DeleteLoggedHabit(string habitName, int id)
    {
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());

        // Check if the habit exists in the habits table
        if (habitCount == 0)
        {
            return false;
        }

        // Delete the log entry from the habit table
        string deleteHabitQuery = $"DELETE FROM {habitName} WHERE id = {id};";
        using var deleteHabitCommand = new SqliteCommand(deleteHabitQuery, _connection);
        //true if the logged habit entry is deleted successfully; otherwise, false.
        return deleteHabitCommand.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Updates a habit in the database.
    /// </summary>
    /// <param name="oldHabitName">The old name of the habit.</param>
    /// <param name="oldUnit">The old unit of the habit.</param>
    /// <param name="newHabitName">The new name of the habit.</param>
    /// <param name="newUnit">The new unit of the habit.</param>
    /// <returns>True if the habit is updated successfully; otherwise, false.</returns>
    public bool UpdateHabit(string oldHabitName, string oldUnit, string newHabitName = "", string newUnit = "")
    {
        if (newHabitName == "" && newUnit == "")
        {
            throw new ArgumentException("At least one of the new habit name or unit must be provided.");
        }

        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{oldHabitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());
        // Check if the habit exists in the habits table
        if (habitCount == 0)
        {
            return false;
        }

        //set the new habit name  to the old habit name if the new habit name is empty
        //and vice versa for the unit names
        if (newHabitName == "")
        {
            newHabitName = oldHabitName;
        }
        else if (newUnit == "")
        {
            newUnit = oldUnit;
        }

        // Update the habit in the habits table
        // UPDATE takes the table name, the column name(s), and the new value(s)
        string updateHabitQuery = $"UPDATE habits SET name = '{newHabitName}', unit = '{newUnit}' WHERE name = '{oldHabitName}';";
        using var updateHabitCommand = new SqliteCommand(updateHabitQuery, _connection);
        updateHabitCommand.ExecuteNonQuery();
        if (oldHabitName == newHabitName)
        {
            return true;
        }

        // Alter the habit table name
        // ALTER TABLE takes the old table name and the new table name
        string updateHabitTableQuery = $"ALTER TABLE {oldHabitName} RENAME TO {newHabitName};";
        using var updateHabitTableCommand = new SqliteCommand(updateHabitTableQuery, _connection);
        updateHabitTableCommand.ExecuteNonQuery();
        return true;
    }

    /// <summary>
    /// Updates a logged habit entry in the database.
    /// </summary>
    /// <param name="habitName">The name of the habit.</param>
    /// <param name="id">The ID of the logged habit entry.</param>
    /// <param name="newQuantity">The new quantity of the habit.</param>
    /// <returns>True if the logged habit entry is updated successfully; otherwise, false.</returns>
    public bool UpdateLoggedHabit(string habitName, int id, int newQuantity)
    {
        string selectQuery = $"SELECT COUNT(*) FROM habits WHERE name = '{habitName}';";
        using var selectCommand = new SqliteCommand(selectQuery, _connection);
        int habitCount = Convert.ToInt32(selectCommand.ExecuteScalar());
        if (habitCount == 0)
        {
            return false;
        }

        string updateHabitQuery = $"UPDATE {habitName} SET quantity = {newQuantity} WHERE id = {id};";
        using var updateHabitCommand = new SqliteCommand(updateHabitQuery, _connection);
        return updateHabitCommand.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// Gets all habits from the database.
    /// </summary>
    /// <returns>A list of habit names and units.</returns>
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

    /// <summary>
    /// Gets all habits and their logged entries from the database.
    /// </summary>
    /// <returns>A dictionary of habit names and units with their corresponding logged entries.</returns>
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

    /// <summary>
    /// Gets the logged entries of a habit from the database.
    /// </summary>
    /// <param name="habitName">The name of the habit.</param>
    /// <param name="unit">The unit of the habit.</param>
    /// <returns>A list of logged habit entries.</returns>
    public List<(int id, int quantity, string timeStamp)>? GetHabitLogs(string habitName, out string unit)
    {
        string selectHabitQuery = $"SELECT unit FROM habits WHERE name = '{habitName}';"; // Get the unit of the habit
        using var selectHabitCommand = new SqliteCommand(selectHabitQuery, _connection);
        unit = selectHabitCommand.ExecuteScalar()?.ToString() ?? "";
        if (unit == "")
        {
            return null;
        }

        selectHabitQuery = $"SELECT id, quantity, timestamp FROM {habitName};"; // Get the logged habit entries
        using var selectHabitCommand2 = new SqliteCommand(selectHabitQuery, _connection);
        using var reader = selectHabitCommand2.ExecuteReader();
        var habitList = new List<(int id, int quantity, string timeStamp)>();
        while (reader.Read())
        {
            habitList.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
        }

        return habitList;
    }
}
