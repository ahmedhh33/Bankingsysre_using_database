using System.Text.RegularExpressions;
using System.Data.SqlClient;
namespace cSharp_BankSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("      Welcome To    ");
            Console.WriteLine(" +-+-+-+-+-+-+-+-+-+\r\n |Ahmedhh|\r\n +-+-+-+-+-+-+-+-+-+");
            Console.WriteLine("    Banking System      \n");
            Console.ResetColor();

            mainMenu();


            void mainMenu()
            {
                BankSystem bankSystem = new BankSystem();
                User user = new User();
                

                try
                {

                    sqlConnection.Open();

                    while (true)
                    {
                        Console.WriteLine("1. ViewExchangeRates");
                        Console.WriteLine("2. CurrencyConverter");
                        Console.WriteLine("3. Register");
                        Console.WriteLine("4. Login");
                        Console.WriteLine("5. Exit");
                        Console.Write("Select an option: ");
                        string choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                ViewExchangeRates();
                                break;
                            case "2":
                                CurrencyConverter();
                                break;
                            case "3":
                                Console.Write("Enter your name: ");
                                string name = Console.ReadLine();
                                Console.Write("Enter your email: ");
                                string email = Console.ReadLine();
                                Console.WriteLine("Please enter a password that meets the criteria");
                                Console.WriteLine("Password must be at least 8 characters long and contain uppercase letter,lowercase letter,number, and symbol");
                                string password = Console.ReadLine();
                                if (user.IsStrongPassword(password))
                                {
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Password does not meet the criteria. Please try again.");
                                }

                                if (bankSystem.RegisterUser(name, email, password))
                                {
                                    string RegisteringUser = "insert into Users (userName,email,userPassword) values(@name,@email,@password);";
                                    SqlCommand command = new SqlCommand(RegisteringUser, sqlConnection);
                                    command.Parameters.AddWithValue("@name", name);
                                    command.Parameters.AddWithValue("@email", email);
                                    command.Parameters.AddWithValue("@password", password);
                                    command.ExecuteNonQuery();


                                    Console.WriteLine("Registration successful.");
                                }
                                else
                                {
                                    Console.WriteLine("Registration failed. User with this email already exists.");
                                }
                                Console.Clear();
                                break;

                            case "4":
                                Console.Write("Enter your email: ");
                                string loginEmail = Console.ReadLine();
                                Console.Write("Enter your password: ");
                                string loginPassword = Console.ReadLine();

                                if (bankSystem.Login(loginEmail, loginPassword))
                                {
                                    Console.WriteLine("Login successful.");
                                    bankSystem.HandleLoggedInUser(loginEmail);
                                }
                                else
                                {
                                    Console.WriteLine("Login failed. Invalid email or password.");
                                }
                                break;

                            case "5":
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Are you sure you want to exit? (y/n) "); // Check if the user want to exit the application
                                string ExitInput = Console.ReadLine();
                                ExitInput.ToLower();
                                Console.ResetColor();
                                if (ExitInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.Write("Thank You");
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    mainMenu();
                                }
                                break;

                            default:
                                Console.WriteLine("Invalid option. Please try again.");
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    // 12 catch the exception message if any occurs
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    // 13 after all we need to close the connection with database
                    sqlConnection.Close();
                }
            }
        }
        static async Task ViewExchangeRates()
        {
            ExchangeRateService exchangeRateService = new ExchangeRateService();
            ExchangeRateData exchangeRates = await exchangeRateService.GetExchangeRatesAsync();

            if (exchangeRates != null)
            {
                Console.WriteLine($"Base Currency: {exchangeRates.base_code}");
                Console.WriteLine("Exchange Rates:");
                foreach (var conversion_rates in exchangeRates.conversion_rates)
                {
                    Console.WriteLine($"{conversion_rates.Key}: {conversion_rates.Value}");
                }
            }
            return;
        }
        static async Task CurrencyConverter()
        {
            CurrencyConverter currencyConverter = new CurrencyConverter();

            Console.WriteLine("Enter the currency you want to convert from (e.g., USD):");
            string fromCurrency = Console.ReadLine();

            Console.WriteLine("Enter the currency you want to convert to (e.g., EUR):");
            string toCurrency = Console.ReadLine();

            Console.WriteLine("Enter the amount to convert:");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                decimal convertedAmount = await currencyConverter.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);
                if (convertedAmount >= 0)
                {
                    Console.WriteLine($"Converted amount: {convertedAmount} {toCurrency}");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount.");
            }
        }
    }
}