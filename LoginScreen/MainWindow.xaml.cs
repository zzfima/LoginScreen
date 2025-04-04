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

            var connectionString = "Data Source=..\\..\\..\\DB\\LoginDB.db;Version=3;";
            var readCommand = $"SELECT * FROM Users WHERE Username = '{txtUsername.Text}' AND Password = '{txtPassword.Password}'";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(readCommand, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    bool isExist = reader.HasRows;
                    if (isExist)
                    {
                        lblSuccess.Text = "Login successful!";
                    }
                    else
                    {
                        lblError.Text = "Invalid username or password.";
                    }
                }
            }
        }
    }
}