# HabitTrackerConsoleApp

This is a console application that allows users to track and manage their habits. The application uses a SQLite database to store and retrieve habit data. Users can insert, delete, update, and view their logged habits. The application also provides reporting functionality to generate specific information about habits.

## Features

- SQLite Database: The application creates a SQLite database if one doesn't exist and creates a table to store habit data. This is handled by the `HabitDatabase` class.
- Menu Options: The application presents a menu to the user with options to insert a habit, delete a habit, update a habit, view all habits, generate a report, and exit the application. This is implemented using the `SelectionPrompt` class from the `Spectre.Console` library.
- Error Handling: The application handles possible errors to ensure it doesn't crash and provides appropriate error messages to the user.
- Termination: The application continues to run until the user chooses the "Exit" option.
- Raw SQL: The application interacts with the database using raw SQL commands, without using mappers like Entity Framework.
- Custom Habits: Users can create their own habits to track and choose the unit of measurement for each habit.
- Update Habits: Users can update the name, unit, or and individual logged habit's quantity.
- Reporting: Users can view a report of all habits or an individial habit.

## Getting Started

To run the application, follow these steps:

1. Make sure you have the necessary dependencies installed, including Microsoft.Data.Sqlite and Spectre.Console.
2. Clone the repository to your local machine.
3. Open the solution in Visual Studio.
4. Build the solution to restore NuGet packages and compile the code.
5. Run the application.

## Dependencies

- Microsoft.Data.Sqlite: The application uses this package to interact with the SQLite database.
- Spectre.Console: The application uses this package to create a user-friendly console interface.

## Usage

1. When the application starts, it will create a SQLite database if one doesn't exist and create a table to store habit data.
2. The application will display a menu with options to insert a habit, delete a habit, update a habit, view all habits, generate a report, or exit the application.
3. Select an option by entering the corresponding number.
4. Follow the prompts to perform the desired action.
5. The application will continue to run until you choose the "Exit" option.

## License

This project is licensed under the [MIT License](LICENSE).

## Resources Used
- [The C# Academy](https://www.thecsharpacademy.com/project/12/habit-logger)
- GitHub Copilot to generate code snippets