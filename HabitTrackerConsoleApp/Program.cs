namespace HabitTrackerConsoleApp;

internal class Program
{
    static void Main()
    {
        using var habitDatabase = new HabitDatabase("habit_tracker.db");
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
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid choice {choice}. Please try again.");
                Console.ResetColor();
                continue;
            }
            else
            {
                switch (choice)
                {
                    case 1:
                        InsertHabit(habitDatabase);
                        break;
                    case 2:
                        DeleteHabit(habitDatabase);
                        break;
                    case 3:
                        UpdateHabit(habitDatabase);
                        break;
                    case 4:
                        habitDatabase.ViewAllHabits();
                        break;
                    case 5:
                        GenerateReport(habitDatabase);
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

    static void InsertHabit(HabitDatabase habitDatabase)
    {
        string name = string.Empty;
        while (string.IsNullOrEmpty(name))
        {
            Console.Write("Enter the habit name: ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid name. Please try again.");
                Console.ResetColor();
            }
            else
            {
                name = line;
            }
        }

        int quantity = -1;
        while (quantity < 0)
        {
            Console.Write("Enter the habit quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int line) || line < 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid quantity. Please try again.");
                Console.ResetColor();
            }
            else
            {
                quantity = line;
            }
        }

        string unit = string.Empty;
        while (string.IsNullOrEmpty(unit))
        {
            Console.Write("Enter the unit of measurement: ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid unit. Please try again.");
                Console.ResetColor();
            }
            else
            {
                unit = line;
            }
        }

        habitDatabase.InsertHabit(name, quantity, unit);
        Console.WriteLine("Habit inserted successfully.");
    }

    static void DeleteHabit(HabitDatabase habitDatabase)
    {
        int id = -1;
        while (id < 0)
        {
            Console.Write("Enter the habit ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int line) || line < 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid ID. Please try again.");
                Console.ResetColor();
            }
            else
            {
                id = line;
            }
        }

        if (habitDatabase.DeleteHabit(id))
        {
            Console.WriteLine("Habit deleted successfully.");
        }
        else
        {
            Console.WriteLine("Habit not found.");
        }
    }

    static void UpdateHabit(HabitDatabase habitDatabase)
    {
        int id = -1;
        while (id < 0)
        {
            Console.Write("Enter the habit ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int line) || line < 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid ID. Please try again.");
                Console.ResetColor();
            }
            else
            {
                id = line;
            }
        }

        string name = string.Empty;
        while (string.IsNullOrEmpty(name))
        {
            Console.Write("Enter the habit name: ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid name. Please try again.");
                Console.ResetColor();
            }
            else
            {
                name = line;
            }
        }

        int quantity = -1;
        while (quantity < 0)
        {
            Console.Write("Enter the habit quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int line) || line < 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid quantity. Please try again.");
                Console.ResetColor();
            }
            else
            {
                quantity = line;
            }
        }

        string unit = string.Empty;
        while (string.IsNullOrEmpty(unit))
        {
            Console.Write("Enter the unit of measurement: ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid unit. Please try again.");
                Console.ResetColor();
            }
            else
            {
                unit = line;
            }
        }

        if (habitDatabase.UpdateHabit(id, name, quantity, unit))
        {
            Console.WriteLine("Habit updated successfully.");
        }
        else
        {
            Console.WriteLine("Habit not found.");
        }
    }

    static void GenerateReport(HabitDatabase habitDatabase)
    {
        int choice;
        Console.Clear();
        do
        {
            Console.WriteLine("Report Menu");
            Console.WriteLine("1. Number of times a habit was performed");
            Console.WriteLine("2. Total quantity of a habit");
            Console.WriteLine("0. Back to main menu");

            Console.Write("Enter your choice: ");
            while (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Please try again.");
                Console.ResetColor();
                continue;
            }

            int id;
            switch (choice)
            {
                case 1:
                    id = -1;
                    while (id < 0)
                    {
                        Console.Write("Enter the habit ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int line) || line < 0)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid ID. Please try again.");
                            Console.ResetColor();
                        }
                        else
                        {
                            id = line;
                        }
                    }

                    int count = habitDatabase.ReportNumberOfTimes(id);
                    Console.WriteLine($"The habit was performed {count} times.");
                    break;
                case 2:
                    id = -1;
                    while (id < 0)
                    {
                        Console.Write("Enter the habit ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int line) || line < 0)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid ID. Please try again.");
                            Console.ResetColor();
                        }
                        else
                        {
                            id = line;
                        }
                    }

                    int totalQuantity = habitDatabase.ReportTotalQuantity(id);
                    Console.WriteLine($"The total quantity of the habit is {totalQuantity}.");
                    break;
                case 0:
                    break;
                default:
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ResetColor();
                    break;
            }
        } while (choice != 0);
        Console.Clear();
    }
}
