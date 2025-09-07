using Microsoft.Extensions.Configuration;
using RedisBookingApp;
using StackExchange.Redis;
using System.Linq.Expressions;

class Program
{
    private readonly IConfiguration config;
    public Program()
    {
        config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
    }

    static void Main()
    {
        try
        {
            var program = new Program();
            string EndPoint = program.config["ReddisSettings:EndPoints"];
            int Port = Convert.ToInt32(program.config["ReddisSettings:Port"]);
            string User = program.config["ReddisSettings:User"];
            string Password = program.config["ReddisSettings:Password"];


            IDatabase db = CustomReddis.ReddisConnect(EndPoint, Port, User, Password);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1.  Delete all keys");
                Console.WriteLine("2.  Create booking slots for a doctor");
                Console.WriteLine("3.  View all slots for a doctor");
                Console.WriteLine("99. Exit");
                Console.Write("Enter choice: ");
                var input = Console.ReadLine();

                //Switch case to delete keys or create slots or book slots or view slots or exit

                switch (input)
                {
                    case "1":
                        CustomReddis.DeleteAllKeys(db);
                        Console.WriteLine("All keys deleted.");
                        break;

                    //  return back to menu after each operation


                    case "2":
                        // Create Booking Slots for a Doctor
                        string doctorId = "doctor_123";
                        string _doctorKey = BookingEngine.SetSlots(db, doctorId);
                        Console.WriteLine($"Slots created for {_doctorKey}.");
                        break;


                    case "3":
                        // View All Slots for a Doctor

                        //Console.WriteLine("Enter Doctor ID");
                        //string doctorId_View = Console.ReadLine();

                        string doctorId_View = "doctor_123";
                        

                        var date = DateTime.Now.ToString("yyyyMMdd");
                        var doctorKey = $"available_slots:{doctorId_View}:{date}";

                        var allSlots = db.HashGetAll(doctorKey);
                        Console.WriteLine($"Slots for {doctorKey}:");
                        foreach (var entry in allSlots)
                        {
                            Console.WriteLine($"{entry.Name} → {entry.Value}");
                        }


                        break;
                    case "99":
                        // Exit Application
                        exit = true;
                        CustomReddis.ReddisClose(db);
                        Console.WriteLine("Connection Closed");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }


}