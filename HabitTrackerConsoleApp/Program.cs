using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;

namespace HabitTrackerConsoleApp;

internal class Program
{
    static void Main()
    {
        using var habitDatabase = new HabitDatabase("habit_tracker.db");
        var menu = new SelectionPrompt<string>()
        .Title("[bold]Habit Tracker Menu[/]")
        .AddChoices([
            "Exit",
            "Add New Habit",
            "Log Habit",
            "Delete Habit",
            "Delete Logged Habit",
            "Update Habit",
            "View All Habits",
            "View Habit",
            "Report Number of Times",
            "Report Total Quantity",
        ]);

        while (true)
        {
            string choice = AnsiConsole.Prompt(menu);
            switch (choice)
            {
                case "Exit":
                    return;
                case "Add New Habit":
                    AddNewHabit(habitDatabase);
                    break;
                case "Log Habit":
                    LogHabit(habitDatabase);
                    break;
                case "Delete Habit":
                    DeleteHabit(habitDatabase);
                    break;
                case "Delete Logged Habit":
                    DeleteLoggedHabit(habitDatabase);
                    break;
                case "Update Habit":
                    UpdateHabit(habitDatabase);
                    break;
                case "View All Habits":
                    ViewAllHabits(habitDatabase);
                    break;
                case "View Habit":
                    ViewHabit(habitDatabase);
                    break;
                case "Report Number of Times":
                    ReportNumberOfTimes(habitDatabase);
                    break;
                case "Report Total Quantity":
                    ReportTotalQuantity(habitDatabase);
                    break;
            }
        }
    }

    private static void AddNewHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        var builder = new StringBuilder();
        builder.AppendLine("Existing habits:");
        builder.AppendLine("Habit - Unit");
        foreach (var habit in habits)
        {
            builder.AppendLine($"{habit.habitName} - {habit.unit}");
        }

        builder.AppendLine("Enter the name of the new habit (or enter to return to main menu):");
        var habitName = AnsiConsole.Prompt(new TextPrompt<string>(builder.ToString()).AllowEmpty());
        if (string.IsNullOrEmpty(habitName))
        {
            return;
        }

        var habitUnit = AnsiConsole.Prompt(new TextPrompt<string>("Enter the units of the new habit (or enter to return to main menu):").AllowEmpty());
        if (string.IsNullOrEmpty(habitUnit))
        {
            return;
        }

        if (habitDatabase.AddNewHabit(habitName, habitUnit))
        {
            AnsiConsole.MarkupLine("[bold]New habit added successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Failed to add new habit![/]");
        }
    }

    private static void LogHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        if (habits.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to log![/]");
            return;
        }

        var habitNames = habits.Select(habit => habit.habitName).ToList();
        habitNames.Add("Return to main menu");
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == "Return to main menu")
        {
            return;
        }

        var habitQuantity = AnsiConsole.Ask<int>($"Enter the {habits.Single(h => h.habitName == habitName).unit} of {habitName} to log:");
        if (habitQuantity == 0)
        {
            AnsiConsole.MarkupLine("[bold red]Failed to log habit![/]");
            return;
        }

        if (habitDatabase.LogHabit(habitName, habitQuantity))
        {
            AnsiConsole.MarkupLine("[bold]Habit logged successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Failed to log habit![/]");
        }
    }

    private static void DeleteHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        if (habits.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to delete![/]");
            return;
        }

        var habitNames = habits.Select(habit => habit.habitName).ToList();
        habitNames.Add("Return to main menu");
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == "Return to main menu")
        {
            return;
        }

        if (habitDatabase.DeleteHabitTable(habitName))
        {
            AnsiConsole.MarkupLine("[bold]Habit deleted successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Failed to delete habit![/]");
        }
    }

    private static void DeleteLoggedHabit(HabitDatabase habitDatabase)
    {
        var habitsAndLogs = habitDatabase.GetAllHabitsAndLogs();
        if (habitsAndLogs.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No logged habits to delete![/]");
            return;
        }

        var habitNames = habitsAndLogs.Select(kvp => kvp.Key.habitName).ToList();
        habitNames.Add("Return to main menu");
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == "Return to main menu")
        {
            return;
        }

        var habitLogs = habitsAndLogs.Single(kvp => kvp.Key.habitName == habitName).Value;
        if (habitLogs.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No logged habits to delete![/]");
            return;
        }

        var habitLogStrings = habitLogs.Select(log => $"ID: {log.id}, Quantity: {log.quantity}, Timestamp: {log.timeStamp}").ToList();
        menu = new SelectionPrompt<string>().AddChoices(habitLogStrings);
        var habitLogString = AnsiConsole.Prompt(menu);
        var habitId = habitLogs.Single(log => $"ID: {log.id}, Quantity: {log.quantity}, Timestamp: {log.timeStamp}" == habitLogString).id;
        if (habitDatabase.DeleteLoggedHabit(habitName, habitId))
        {
            AnsiConsole.MarkupLine("[bold]Logged habit deleted successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Failed to delete logged habit![/]");
        }
    }

    private static void UpdateHabit(HabitDatabase habitDatabase)
    {
        // Implement the logic for updating a habit here
    }

    private static void ViewAllHabits(HabitDatabase habitDatabase)
    {
        // Implement the logic for viewing all habits here
    }

    private static void ViewHabit(HabitDatabase habitDatabase)
    {
        // Implement the logic for viewing a specific habit here
    }

    private static void ReportNumberOfTimes(HabitDatabase habitDatabase)
    {
        // Implement the logic for reporting the number of times here
    }

    private static void ReportTotalQuantity(HabitDatabase habitDatabase)
    {
        // Implement the logic for reporting the total quantity here
    }
}
