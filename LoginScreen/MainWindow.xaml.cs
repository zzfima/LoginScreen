using System.Data.SQLite;
using System.Windows;

namespace LoginScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            lblSuccess.Text = string.Empty;
            lblError.Text = string.Empty;

            var isExist = ValidateUser(txtUsername.Text, txtPassword.Password);
            if (isExist)
            {
                lblSuccess.Text = "Login successful!";
            }
            else
            {
                lblError.Text = "Invalid username or password.";
            }
        }


        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterUserSafe(txtUsername.Text, txtPassword.Password);
        }

        //bad practice
        /*
            1. SQL Injection: The query is vulnerable to SQL injection because user input is directly concatenated into the SQL query.
                ' OR 1=1 --
                '; DROP TABLE moro; --
            2. Plaintext Passwords: Storing passwords as plaintext is a huge security risk.
         */
        public bool ValidateUser(string username, string password)
        {
            var connectionString = "Data Source=..\\..\\..\\DB\\LoginDB.db;Version=3;";
            var readCommand = $"SELECT * FROM Users WHERE username = '{username}' AND hashedPassword = '{password}'";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(readCommand, connection);
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
                catch (Exception)
                {
                    return false; 
                }
            }
        }

        //good practice
        /*
            1. Use Parameterized Queries: This prevents SQL injection attacks.
            2. Hash Passwords: Store hashed passwords instead of plaintext.
         */
        public bool ValidateUserSafe(string username, string password)
        {
            var connectionString = "Data Source=..\\..\\..\\DB\\LoginDB.db;Version=3;";
            var readCommand = "SELECT hashedPassword FROM Users WHERE username = @username";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(readCommand, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    var hash = command.ExecuteScalar() as string;
                    if (hash == null)
                        return false;

                    return BCrypt.Net.BCrypt.Verify(password, hash);
                }
            }
        }

        public void RegisterUserSafe(string username, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var connectionString = "Data Source=..\\..\\..\\DB\\LoginDB.db;Version=3;";
            var insertCommand = "INSERT INTO Users (username, hashedPassword) VALUES (@username, @hashedPassword)";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(insertCommand, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                command.ExecuteScalar();
            }
        }
    }
}