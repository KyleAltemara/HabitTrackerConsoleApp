# HabitTrackerConsoleApp

This is a console application that allows users to track and manage their habits.The application uses a SQLite database to store and retrieve habit data.Users can insert, delete, update, and view their logged habits.The application also provides reporting functionality to generate specific information about habits.

## Features

- SQLite Database: The application creates a SQLite database if one doesn't exist and creates a table to store habit data.
- Menu Options: The application presents a menu to the user with options to insert a habit, delete a habit, update a habit, view all habits, generate a report, and exit the application.
- Error Handling: The application handles possible errors to ensure it doesn't crash and provides appropriate error messages to the user.
- Termination: The application continues to run until the user inserts 0 to exit.
- Raw SQL: The application interacts with the database using raw SQL commands, without using mappers like Entity Framework.
- Custom Habits: Users can create their own habits to track and choose the unit of measurement for each habit.
- Seed Data: The application automatically seeds data into the database when it's created for the first time, generating a few habits and inserting a hundred records with randomly generated values.
- Reporting: Users can generate reports to view specific information about habits, such as the number of times a habit was performed or the total quantity of a habit.

## Getting Started

To run the application, follow these steps:

1. Make sure you have the necessary dependencies installed, including Microsoft.Data.Sqlite.
2. Clone the repository to your local machine.
3. Open the solution in Visual Studio.
4. Build the solution to restore NuGet packages and compile the code.
5. Run the application.

## Dependencies

- Microsoft.Data.Sqlite: The application uses this package to interact with the SQLite database.

## Usage

1. When the application starts, it will create a SQLite database if one doesn't exist and create a table to store habit data.
2. The application will display a menu with options to insert a habit, delete a habit, update a habit, view all habits, generate a report, or exit the application.
3. Select an option by entering the corresponding number.
4. Follow the prompts to perform the desired action.
5. The application will continue to run until you enter 0 to exit.

## License

This project is licensed under the[MIT License](LICENSE).

## Resources Used
- [The C# Academy](https://www.thecsharpacademy.com/project/12/habit-logger)
- GitHub Copilot to generate code snippets