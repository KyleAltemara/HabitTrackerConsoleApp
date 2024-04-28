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
        // Create the Habits table if it does not exist
        // The Id column is the primary key and is auto-incremented
        using var command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Habits (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Quantity INTEGER, Unit TEXT)", _connection);
        command.ExecuteNonQuery();
    }

    public void InsertHabit(string name, int quantity, string unit)
    {
        // Insert a new habit into the Habits table
        // The Id column is not specified because it is auto-incremented
        // The @Name, @Quantity, and @Unit are placeholders for the actual values
        using var command = new SqliteCommand("INSERT INTO Habits (Name, Quantity, Unit) VALUES (@Name, @Quantity, @Unit)", _connection);
        // The AddWithValue method is used to add the actual values to the placeholders
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Quantity", quantity);
        command.Parameters.AddWithValue("@Unit", unit);
        // The ExecuteNonQuery method is used to execute the command
        // It returns the number of rows affected by the command
        command.ExecuteNonQuery();
    }

    public bool DeleteHabit(int id)
    {
        // DELETE will remove the habit with the specified Id
        // if the habit is found, the number of rows affected will be greater than 0
        // otherwise, the habit was not found and the number of rows affected will be 0
        using var command = new SqliteCommand("DELETE FROM Habits WHERE Id = @Id", _connection);
        command.Parameters.AddWithValue("@Id", id);
        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public bool UpdateHabit(int id, string name, int quantity, string unit)
    {
        // UPDATE will modify the habit with the specified Id
        // if the habit is found, the number of rows affected will be greater than 0
        // otherwise, the habit was not found and the number of rows affected will be 0
        using var command = new SqliteCommand("UPDATE Habits SET Name = @Name, Quantity = @Quantity, Unit = @Unit WHERE Id = @Id", _connection);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Quantity", quantity);
        command.Parameters.AddWithValue("@Unit", unit);
        command.Parameters.AddWithValue("@Id", id);
        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public void ViewAllHabits()
    {
        // SELECT * will retrieve all columns from the Habits table
        // If no columns are specified, all columns are retrieved
        using var command = new SqliteCommand("SELECT * FROM Habits", _connection);
        using var reader = command.ExecuteReader();
        Console.WriteLine("ID\tName\tQuantity\tUnit");
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int quantity = reader.GetInt32(2);
            string unit = reader.GetString(3);
            Console.WriteLine($"{id}\t{name}\t{quantity}\t{unit}");
        }
    }

    public int ReportNumberOfTimes(int id)
    {
        // The COUNT(*) function will return the number of rows that match the condition
        // @Id is a placeholder for the actual value
        using var command = new SqliteCommand("SELECT COUNT(*) FROM HabitLogs WHERE HabitId = @Id", _connection);
        command.Parameters.AddWithValue("@Id", id);
        int count = Convert.ToInt32(command.ExecuteScalar());
        return count;
    }

    public int ReportTotalQuantity(int id)
    {
        // The SUM function will return the total of the Quantity column
        // @Id is a placeholder for the actual value
        using var command = new SqliteCommand("SELECT SUM(Quantity) FROM HabitLogs WHERE HabitId = @Id", _connection);
        command.Parameters.AddWithValue("@Id", id);
        object? result = command.ExecuteScalar();
        int totalQuantity = result is DBNull ? 0 : Convert.ToInt32(result);
        return totalQuantity;
    }
}
