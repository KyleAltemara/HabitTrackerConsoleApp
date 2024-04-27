using Microsoft.Data.Sqlite;

namespace HabitTrackerConsoleApp;

internal class Program
{
    static void Main()
    {
        // Create or connect to the SQLite database
        using var connection = new SqliteConnection("Data Source=habit_tracker.db");
        connection.Open();

        // Create the habit table if it doesn't exist
        using (var command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Habits (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Quantity INTEGER, Unit TEXT)", connection))
        {
            command.ExecuteNonQuery();
        }

        // Seed data into the database if it's empty
        //using (var command = new SqliteCommand("SELECT COUNT(*) FROM Habits", connection))
        //{
        //    int count = Convert.ToInt32(command.ExecuteScalar());
        //    if (count == 0)
        //    {
        //        SeedData(connection);
        //    }
        //}

        // Show the menu to the user
        int choice;
        do
        {
            Console.WriteLine("Habit Tracker Menu");
            Console.WriteLine("1. Insert a habit");
            Console.WriteLine("2. Delete a habit");
            Console.WriteLine("3. Update a habit");
            Console.WriteLine("4. View all habits");
            Console.WriteLine("5. Generate report");
            Console.WriteLine("0. Exit");

            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.Clear();
                Console.WriteLine($"Invalid choice {choice}. Please try again.");
                continue;
            }
            else
            {
                switch (choice)
                {
                    case 1:
                        InsertHabit(connection);
                        break;
                    case 2:
                        DeleteHabit(connection);
                        break;
                    case 3:
                        UpdateHabit(connection);
                        break;
                    case 4:
                        ViewAllHabits(connection);
                        break;
                    case 5:
                        GenerateReport(connection);
                        break;
                    case 0:
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Invalid choice {choice}. Please try again.");
                        Console.ResetColor();
                        break;
                }
            }

        } while (choice != 0);
    }

    static void InsertHabit(SqliteConnection connection)
    {
        string name = string.Empty;
        while (string.IsNullOrEmpty(name))
        {
            Console.Write("Enter the habit name: ");
            name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Invalid name. Please try again.");
            }
        }

        int quantity = 0;
        while (quantity <= 0)
        {
            Console.Write("Enter the habit quantity: ");
            if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity. Please try again.");
            }
        }

        string unit = string.Empty;
        while (string.IsNullOrEmpty(unit))
        {
            Console.Write("Enter the unit of measurement: ");
            unit = Console.ReadLine();
            if (string.IsNullOrEmpty(unit))
            {
                Console.WriteLine("Invalid unit. Please try again.");
            }
        }

        using (var command = new SqliteCommand("INSERT INTO Habits (Name, Quantity, Unit) VALUES (@Name, @Quantity, @Unit)", connection))
        {
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@Unit", unit);
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Habit inserted successfully.");
    }

    static void DeleteHabit(SqliteConnection connection)
    {
        Console.Write("Enter the habit ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please try again.");
            return;
        }

        using var command = new SqliteCommand("DELETE FROM Habits WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);
        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            Console.WriteLine("Habit deleted successfully.");
        }
        else
        {
            Console.WriteLine("Habit not found.");
        }
    }

    static void UpdateHabit(SqliteConnection connection)
    {
        Console.Write("Enter the habit ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please try again.");
            return;
        }

        Console.Write("Enter the new habit name: ");
        string name = Console.ReadLine();

        Console.Write("Enter the new habit quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity. Please try again.");
            return;
        }

        Console.Write("Enter the new unit of measurement: ");
        string unit = Console.ReadLine();

        using var command = new SqliteCommand("UPDATE Habits SET Name = @Name, Quantity = @Quantity, Unit = @Unit WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Quantity", quantity);
        command.Parameters.AddWithValue("@Unit", unit);
        command.Parameters.AddWithValue("@Id", id);
        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            Console.WriteLine("Habit updated successfully.");
        }
        else
        {
            Console.WriteLine("Habit not found.");
        }
    }

    static void ViewAllHabits(SqliteConnection connection)
    {
        using var command = new SqliteCommand("SELECT * FROM Habits", connection);
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

    static void GenerateReport(SqliteConnection connection)
    {
        Console.WriteLine("Report Menu");
        Console.WriteLine("1. Number of times a habit was performed");
        Console.WriteLine("2. Total quantity of a habit");
        Console.WriteLine("0. Back to main menu");

        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }

        switch (choice)
        {
            case 1:
                ReportNumberOfTimes(connection);
                break;
            case 2:
                ReportTotalQuantity(connection);
                break;
            case 0:
                Console.WriteLine("Returning to main menu...");
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    static void ReportNumberOfTimes(SqliteConnection connection)
    {
        Console.Write("Enter the habit ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please try again.");
            return;
        }

        Console.Write("Enter the year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year. Please try again.");
            return;
        }

        using var command = new SqliteCommand("SELECT COUNT(*) FROM HabitLogs WHERE HabitId = @Id AND strftime('%Y', Date) = @Year", connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Year", year);
        int count = Convert.ToInt32(command.ExecuteScalar());
        Console.WriteLine($"The habit was performed {count} times in {year}.");
    }

    static void ReportTotalQuantity(SqliteConnection connection)
    {
        Console.Write("Enter the habit ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please try again.");
            return;
        }

        Console.Write("Enter the year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year. Please try again.");
            return;
        }

        using var command = new SqliteCommand("SELECT SUM(Quantity) FROM HabitLogs WHERE HabitId = @Id AND strftime('%Y', Date) = @Year", connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Year", year);
        object result = command.ExecuteScalar();
        int totalQuantity = result is DBNull ? 0 : Convert.ToInt32(result);
        Console.WriteLine($"The total quantity of the habit in {year} is {totalQuantity}.");
    }

    static readonly Random Random = new();

    static void SeedData(SqliteConnection connection)
    {
        using var command = new SqliteCommand("INSERT INTO Habits (Name, Quantity, Unit) VALUES (@Name, @Quantity, @Unit)", connection);
        for (int i = 0; i < 100; i++)
        {
            string name = $"Habit {i + 1}";
            int quantity = Random.Next(1, 10);
            string unit = "times";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@Unit", unit);
            command.ExecuteNonQuery();
        }
    }
}
