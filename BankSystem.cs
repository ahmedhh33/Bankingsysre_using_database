using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;

namespace cSharp_BankSystem
{

    internal class BankSystem
    {
        private List<User> users;
        private List<Account> accounts;
        private User loggedInUser;
        private string userDataDirectory = "Bank System";
        private static string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true"; // Replace with your SQL Server connection string
        private static SqlConnection connection = new SqlConnection(connectionString);
        public BankSystem()
        {
            users = new List<User>();
            accounts = new List<Account>();
        }
        
        public bool RegisterUser(string name, string email, string password)
        {
            // Check if a user with the given email already exists.
            if (users.Any(u => u.Email == email))
            {
                return false; // User with this email already exists.
            }

            User newUser = new User(name, email, password);
            return true; // Registration successful.
        }
        



        public bool Login(string email, string password)
        {
            try
            {
                using (SqlCommand command = new SqlCommand("select * from Users WHERE email = @Email AND userPassword = @Password", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        return true;

                        reader.Close();
                    }
                    else
                    {
                        Console.WriteLine("Invalid email or password. Please try again.");
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
                connection.Close();
            }
            return false; // Login failed.
        }

        public void HandleLoggedInUser(string email)
        {



            //if (loggedInUser == null)
            //{
            //    Console.WriteLine("You must log in first.");
            //    return;
            //}

            while (true)
            {
                UserAccounts(email);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("1. Create Bank Account");
                Console.WriteLine("2. Delete Account");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n$ $$ Operations $$ $\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("3. Deposit");
                Console.WriteLine("4. Withdraw");
                Console.WriteLine("5. Transfer Money");
                Console.WriteLine("6. Account history");
                Console.ResetColor();
                Console.WriteLine("7. Logout");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        CreateAccount();
                        break;
                    case "2":
                        Console.Clear();
                        DeleteAccount();
                        break;
                    case "3":
                        Console.Clear();
                        Deposit();
                        break;
                    case "4":
                        Console.Clear();
                        Withdraw();
                        break;
                    case "5":
                        Console.Clear();
                        Transfer();
                        break;
                    case "6":
                        Console.Clear();
                        GetAccountHistory();
                        break;
                    case "7":
                        loggedInUser = null; // Logout the user.
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
       
        public void CreateAccount()
        {
           // string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
            //SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                //if (loggedInUser == null)
                //{
                //    Console.WriteLine("You must log in first.");
                //    return;
                //}

                Console.Write("Enter initial balance: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
                {
                    
                    Console.Write("Enter accountHolderID: ");
                    int accountHolderID = int.Parse(Console.ReadLine());
                    string Createraccount = "insert into accounts (balance,userID) values(@initialBalance,@accountHolderID);";
                    SqlCommand command = new SqlCommand(Createraccount, connection);
                    command.Parameters.AddWithValue("@initialBalance", initialBalance);
                    command.Parameters.AddWithValue("@accountHolderID", accountHolderID);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Bank account created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error creating bank account.");
                    }

                    return;
                }
                else
                {
                    Console.WriteLine("Invalid initial balance.");
                    return;
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
                connection.Close();
            }

        }
        public void DeleteAccount()
        {
            Console.WriteLine("Please account number you want to delete");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }
            int account = GetAccountByNumber(accountNumber);

            if (account <= 0)
            {
                Console.WriteLine("Account not found.");
                return;
            }
            try
            {
                connection.Open();
                
                using (SqlCommand command = new SqlCommand("delete from accounts where accountsNumber = @accountNumber", connection))
                {
                    
                    command.Parameters.AddWithValue("@accountNumber", account);
                    int trowaffected = command.ExecuteNonQuery();
                    if (trowaffected > 0)
                    {
                        Console.WriteLine($"The account {account} deleted successfuly");
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
                connection.Close();
            }
            return;
        }
        public void UserAccounts(string Email)
        {
            try
            {
                connection.Open();
                
                using (SqlCommand command = new SqlCommand("select * from accounts a join Users u on a.userID = u.userID WHERE email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);

                    
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int accountNumber = reader.GetInt32(reader.GetOrdinal("accountsNumber"));
                        int accountHolderID = reader.GetInt32(reader.GetOrdinal("userID"));
                        decimal currentBalance = reader.GetDecimal(reader.GetOrdinal("balance"));

                        Console.WriteLine($"UserID :{accountHolderID} Account Number: {accountNumber}, Balance: {currentBalance} ");
                    }

                    reader.Close();
                    
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
                connection.Close();
            }
        }

        public void Deposit()
        {
            //if (loggedInUser == null)
            //{
            //    Console.WriteLine("You must log in first.");
            //    return;
            //}
            Console.Write("Enter the account number to deposit into: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }
            int account = GetAccountByNumber(accountNumber);

            if (account <=0)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter the amount to deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid deposit amount.");
                return;
            }
            try
            {
                connection.Open();
                int tType = (int)TransactionType.Deposit;
                using (SqlCommand command = new SqlCommand("insert into transactions (amount,Ttype,accountsNumber) values (@amount,@tType,@accountNumber)", connection))
                {
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@tType", tType);
                    command.Parameters.AddWithValue("@accountNumber", accountNumber);

                    int rowaffected =command.ExecuteNonQuery();
                    if(rowaffected > 0)
                    {
                        Console.WriteLine("Depositing successful");
                        string Updatebalance = "Update accounts set balance = balance + @amount where accountsNumber = @accountNumber";
                        SqlCommand Command = new SqlCommand(Updatebalance, connection);
                        Command.Parameters.AddWithValue("@amount", amount);
                        Command.Parameters.AddWithValue("@accountNumber", accountNumber);

                        int returns = Command.ExecuteNonQuery();
                        Console.WriteLine("balace abdated");
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
                connection.Close();
            }
            
            return;
        }
        public void Withdraw()
        {
            
            Console.Write("Enter the account number to withdraw from: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }
            int account = GetAccountByNumber(accountNumber);

            if (account <=0)
            {
                Console.WriteLine("Account not found.");
                return;
            }
            Console.Write("Enter the amount to withdraw: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid  amount.");
                return;
            }

            
            try
            {
                connection.Open();

                
                string Updatebalance = "Update accounts set balance = balance - @amount where balance>@amount and accountsNumber = @accountNumber";
                SqlCommand Command = new SqlCommand(Updatebalance, connection);
                Command.Parameters.AddWithValue("@amount", amount);
                Command.Parameters.AddWithValue("@accountNumber", accountNumber);

                int rowaffected = Command.ExecuteNonQuery();
                if (rowaffected > 0)
                {
                    Console.WriteLine("Withdrawing is successful");

                }
                else
                {
                    Console.WriteLine("Invalid withdrawal amount or insufficient funds.");
                    return;
                }



                if (rowaffected > 0)
                {
                    int tType = (int)TransactionType.Withdrawal;
                    using (SqlCommand command = new SqlCommand("insert into transactions (amount,Ttype,accountsNumber) values (@amount,@tType,@accountNumber)", connection))
                    {
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@tType", tType);
                        command.Parameters.AddWithValue("@accountNumber", accountNumber);
                        int trowaffected = command.ExecuteNonQuery();
                        if(trowaffected > 0)
                        {
                            Console.WriteLine("Transactionprocess inserted");
                        }
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
                connection.Close();
            }

            return;
            
        }

        public void Transfer()
        {
            

            Console.Write("Enter the account number want to transfer from: ");
            if (!int.TryParse(Console.ReadLine(), out int sourceAccountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            int sourceAccount = GetAccountByNumber(sourceAccountNumber);

            if (sourceAccount <= 0)
            {
                Console.WriteLine("Source account not found.");
                return;
            }

            Console.Write("Enter the account number want to transfer to: ");
            if (!int.TryParse(Console.ReadLine(), out int targetAccountNumber))
            {
                Console.WriteLine("Invalid target account number.");
                return;
            }

            int targetAccount = GetAccountByNumber(targetAccountNumber);

            if (targetAccount <= 0)
            {
                Console.WriteLine("Target account not found.");
                return;
            }
            //Console.WriteLine("Account holder name: {0}\nAccount number: {1}", targetAccount.AccountHolderName, targetAccount.AccountNumber);
            Console.Write("Enter the amount to transfer: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid transfer amount.");
                return;
            }
            try
            {
                connection.Open();


                string Updatebalance = "Update accounts set balance = balance - @amount where balance>@amount and accountsNumber = @sourceAccountNumber";
                SqlCommand Command = new SqlCommand(Updatebalance, connection);
                Command.Parameters.AddWithValue("@amount", amount);
                Command.Parameters.AddWithValue("@sourceAccountNumber", sourceAccountNumber);

                int rowaffected = Command.ExecuteNonQuery();
                Console.WriteLine("Withdrawing transfer is successful");

                if (rowaffected > 0)
                {
                    int Type = (int)TransactionType.Withdrawal;
                    using (SqlCommand command = new SqlCommand("insert into transactions (amount,Ttype,accountsNumber) values (@amount,@Type,@sourceAccountNumber)", connection))
                    {
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@Type", Type);
                        command.Parameters.AddWithValue("@sourceAccountNumber", sourceAccountNumber);
                        int trowaffected = command.ExecuteNonQuery();
                        if (trowaffected > 0)
                        {
                            Console.WriteLine("Transactionprocess inserted");
                        }
                    }
                }

                string Updatebalances = "Update accounts set balance = balance + @amount where accountsNumber = @targetAccount";
                SqlCommand Commands = new SqlCommand(Updatebalances, connection);
                Commands.Parameters.AddWithValue("@amount", amount);
                Commands.Parameters.AddWithValue("@targetAccount", targetAccount);
                int rowsaffected = Commands.ExecuteNonQuery();
                if (rowsaffected > 0)
                {

                    Console.WriteLine("Depositing trans successful");
                    int tType = (int)TransactionType.Deposit;

                    using (SqlCommand command = new SqlCommand("insert into transactions (amount,Ttype,accountsNumber) values (@amount,@tType,@targetAccount)", connection))
                    {
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@tType", tType);
                        command.Parameters.AddWithValue("@targetAccount", targetAccount);
                        int returns = Command.ExecuteNonQuery();
                        if (returns > 0)
                        {
                            Console.WriteLine("Transection inserted");
                        }
                    }
                }
                
               
                int Trype = (int)TransactionType.Transfer;
                using (SqlCommand commmand = new SqlCommand("insert into transactions (amount,SrcAccNO,Ttype,TargetAccNO,accountsNumber) values (@amount,@sourceAccountNumber,@Trype,@targetAccount,@sourceAccountNumber)", connection))
                {
                    commmand.Parameters.AddWithValue("@amount", amount);
                    commmand.Parameters.AddWithValue("@Trype", Trype);
                    commmand.Parameters.AddWithValue("@targetAccount", targetAccount);
                    commmand.Parameters.AddWithValue("@sourceAccountNumber", sourceAccountNumber);

                    int rowaffecteds = commmand.ExecuteNonQuery();
                    if (rowaffecteds > 0)
                    {
                        Console.WriteLine($"Transferred {amount} OMR from account {sourceAccountNumber} to account {targetAccountNumber}.");

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
                connection.Close();

            }
            //SaveUserData(); // Save the updated user data to the JSON file

            //Console.WriteLine($"Source account balance: {sourceAccount.Balance} OMR");
            

            
            return;
        }
        public void GetAccountHistory()
        {
            
            Console.Write("Enter the account number to show the history: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Invalid account number.");
            }
            int account = GetAccountByNumber(accountNumber);

            if (account <= 0)
            {
                Console.WriteLine("Account not found.");
                return;
            }
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("select * from transactions WHERE accountsNumber = @accountNumber", connection))
                {
                    command.Parameters.AddWithValue("@accountNumber", accountNumber);


                    SqlDataReader reader = command.ExecuteReader();
                    //reader.Read();
                    while (reader.Read())
                    {
                        int TransactionID = reader.IsDBNull(reader.GetOrdinal("TransactionID")) ? 0 : reader.GetInt32(reader.GetOrdinal("TransactionID"));
                        int Ttype = reader.IsDBNull(reader.GetOrdinal("Ttype")) ? 0 : reader.GetInt32(reader.GetOrdinal("Ttype"));
                        decimal amount = reader.GetDecimal(reader.GetOrdinal("amount"));
                        int SrcAccNO = reader.IsDBNull(reader.GetOrdinal("SrcAccNO")) ? 0 : reader.GetInt32(reader.GetOrdinal("SrcAccNO"));
                        int TargetAccNO = reader.IsDBNull(reader.GetOrdinal("TargetAccNO")) ? 0 : reader.GetInt32(reader.GetOrdinal("TargetAccNO"));
                        int accountsNumber = reader.IsDBNull(reader.GetOrdinal("accountsNumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("accountsNumber"));
                        DateTime Ttimestamp = reader.GetDateTime(reader.GetOrdinal("Ttimestamp"));

                        Console.WriteLine($"TransactionID: {TransactionID}");
                        Console.WriteLine($"Type: {Ttype}");
                        Console.WriteLine($"Amount: {amount}");
                        Console.WriteLine($"Source Account Number: {SrcAccNO}");
                        Console.WriteLine($"Target Account Number: {TargetAccNO}");
                        Console.WriteLine($"Account Number: {accountsNumber}");
                        Console.WriteLine($"Timestamp: {Ttimestamp}");

                        Console.WriteLine(new string('-', 40));
                    }

                    reader.Close();

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
                connection.Close();
            }
            Console.WriteLine($"Transaction History for Account {accountNumber}:");
            
        }
        private int GetAccountByNumber(int accountNumber)
        {
           
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("select accountsNumber from accounts WHERE accountsNumber = @accountNumber", connection))
                {
                    command.Parameters.AddWithValue("@accountNumber", accountNumber);


                    SqlDataReader reader = command.ExecuteReader();
                    //reader.Read();
                    while (reader.Read())
                    {
                        int accounttNumber = reader.GetInt32(reader.GetOrdinal("accountsNumber"));
                        return accounttNumber;
                    }

                    reader.Close();

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
                connection.Close();
            }
            return -1;
        }
        

        
    }

}
