using System.Text;
using Spectre.Console;

namespace HabitTrackerConsoleApp;

internal class Program
{
    private const string exitOption = "Return to main menu";

    static void Main()
    {
        using var habitDatabase = new HabitDatabase("habit_tracker.db");
        var menuOptions = new Dictionary<string, Action>
        {
            { "Add New Habit", () => AddNewHabit(habitDatabase) },
            { "Log Habit", () => LogHabit(habitDatabase) },
            { "Delete Habit", () => DeleteHabit(habitDatabase) },
            { "Delete Logged Habit", () => DeleteLoggedHabit(habitDatabase) },
            { "Update Habit", () => UpdateHabit(habitDatabase) },
            { "View All Habits", () => ViewAllHabits(habitDatabase) },
            { "View Habit", () => ViewHabit(habitDatabase) },
            { "Exit", () => Environment.Exit(0) },
        };

        var menu = new SelectionPrompt<string>()
            .Title("[bold]Habit Tracker Menu[/]")
            .AddChoices(menuOptions.Keys);

        while (true)
        {
            string choice = AnsiConsole.Prompt(menu);
            menuOptions[choice]();
        }
    }

    /// <summary>
    /// Adds a new habit to the database.
    /// </summary>
    /// <param name="habitDatabase">The database to add the habit to.</param>
    private static void AddNewHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        var builder = new StringBuilder();
        if (habits.Count > 0)
        {
            builder.AppendLine("Existing habits:");
            builder.AppendLine("Habit - Unit");
            foreach (var habit in habits)
            {
                builder.AppendLine($"{habit.habitName} - {habit.unit}");
            }
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

    /// <summary>
    /// Logs a habit in the database.
    /// </summary>
    /// <param name="habitDatabase">The database to log the habit in.</param>
    private static void LogHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        if (habits.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to log![/]");
            return;
        }

        var habitNames = habits.Select(habit => habit.habitName).ToList();
        habitNames.Add(exitOption);
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == exitOption)
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

    /// <summary>
    /// Deletes a habit from the database.
    /// </summary>
    /// <param name="habitDatabase">The database to delete the habit from.</param>
    private static void DeleteHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        if (habits.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to delete![/]");
            return;
        }

        var habitNames = habits.Select(habit => habit.habitName).ToList();
        habitNames.Add(exitOption);
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == exitOption)
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

    /// <summary>
    /// Deletes a logged habit from the database.
    /// </summary>
    /// <param name="habitDatabase">The database to delete the logged habit from.</param>
    private static void DeleteLoggedHabit(HabitDatabase habitDatabase)
    {
        var habitsAndLogs = habitDatabase.GetAllHabitsAndLogs();
        if (habitsAndLogs.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No logged habits to delete![/]");
            return;
        }

        var habitNames = habitsAndLogs.Select(kvp => kvp.Key.habitName).ToList();
        habitNames.Add(exitOption);
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == exitOption)
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

    /// <summary>
    /// Updates a habit in the database.
    /// </summary>
    /// <param name="habitDatabase">The database to update the habit in.</param>
    private static void UpdateHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        if (habits.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to update![/]");
            return;
        }

        var habitNames = habits.Select(habit => habit.habitName).ToList();
        habitNames.Add(exitOption);
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        AnsiConsole.MarkupLine("[bold]Select the habit to update:[/]");
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == exitOption)
        {
            return;
        }

        var habit = habits.Single(h => h.habitName == habitName);

        const string option1 = "Update habit name and/or unit";
        const string option2 = "Update habit log";
        menu = new SelectionPrompt<string>().AddChoices(option1, option2, exitOption);
        var choice = AnsiConsole.Prompt(menu);
        switch (choice)
        {
            case option1:
                var newHabitName = AnsiConsole.Prompt(new TextPrompt<string>("Enter the new name of the habit (or enter to keep the same):").AllowEmpty());
                string newUnit;
                if (string.IsNullOrEmpty(newHabitName))
                {
                    newUnit = AnsiConsole.Prompt(new TextPrompt<string>("Enter the new unit of the habit (or enter to return to main menu):").AllowEmpty());
                    if (string.IsNullOrEmpty(newUnit))
                    {
                        return;
                    }
                }
                else
                {
                    newUnit = AnsiConsole.Prompt(new TextPrompt<string>("Enter the new unit of the habit (or enter to keep the same):").AllowEmpty());
                }

                if (habitDatabase.UpdateHabit(habit.habitName, habit.unit, newHabitName, newUnit))
                {
                    AnsiConsole.MarkupLine("[bold]Habit updated successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]Failed to update habit![/]");
                }

                break;
            case option2:
                var habitLogs = habitDatabase.GetHabitLogs(habitName, out string unit);
                if (habitLogs is null || habitLogs.Count == 0)
                {
                    AnsiConsole.MarkupLine("[bold red]No logs to update![/]");
                    return;
                }

                var habitLogStrings = habitLogs.Select(log => $"ID: {log.id}, Quantity: {log.quantity}, Timestamp: {log.timeStamp}").ToList();
                habitLogStrings.Add(exitOption);
                menu = new SelectionPrompt<string>().AddChoices(habitLogStrings);
                var habitLogString = AnsiConsole.Prompt(menu);
                if (habitLogString == exitOption)
                {
                    return;
                }

                var (id, quantity, timeStamp) = habitLogs.Single(log => $"ID: {log.id}, Quantity: {log.quantity}, Timestamp: {log.timeStamp}" == habitLogString);
                var newQuantity = AnsiConsole.Ask<int>($"Enter the new {unit} of {habitName} to log:");
                if (newQuantity == 0)
                {
                    AnsiConsole.MarkupLine("[bold red]Failed to update log![/]");
                    return;
                }

                if (habitDatabase.UpdateLoggedHabit(habitName, id, newQuantity))
                {
                    AnsiConsole.MarkupLine("[bold]Log updated successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]Failed to update log![/]");
                }

                break;
            case exitOption:
                return;
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Views all habits in the database.
    /// </summary>
    /// <param name="habitDatabase">The database to view the habits from.</param>
    private static void ViewAllHabits(HabitDatabase habitDatabase)
    {
        var habitsAndLogs = habitDatabase.GetAllHabitsAndLogs();
        if (habitsAndLogs.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to view![/]");
            return;
        }

        foreach (var kvp in habitsAndLogs)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold]{kvp.Key.habitName}[/]");
            var table = new Table();
            table.AddColumn("id");
            table.AddColumn(kvp.Key.unit);
            table.AddColumn("timestamp");
            foreach (var (id, quantity, timeStamp) in kvp.Value)
            {
                table.AddRow(id.ToString(), quantity.ToString(), timeStamp);
            }

            AnsiConsole.Write(table);
        }
    }

    /// <summary>
    /// Views a specific habit in the database.
    /// </summary>
    /// <param name="habitDatabase">The database to view the habit from.</param>
    private static void ViewHabit(HabitDatabase habitDatabase)
    {
        var habits = habitDatabase.GetAllHabits();
        if (habits.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No habits to view![/]");
            return;
        }

        var habitNames = habits.Select(habit => habit.habitName).ToList();
        habitNames.Add(exitOption);
        var menu = new SelectionPrompt<string>().AddChoices(habitNames);
        var habitName = AnsiConsole.Prompt(menu);
        if (habitName == exitOption)
        {
            return;
        }

        AnsiConsole.MarkupLine($"[bold]{habitName}[/]");
        var habitLogs = habitDatabase.GetHabitLogs(habitName, out string unit);
        if (habitLogs is null || habitLogs.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No logs to view![/]");
            return;
        }

        var table = new Table();
        table.AddColumn("id");
        table.AddColumn(unit);
        table.AddColumn("timestamp");
        foreach (var (id, quantity, timeStamp) in habitLogs)
        {
            table.AddRow(id.ToString(), quantity.ToString(), timeStamp);
        }

        AnsiConsole.Write(table);
    }
}
