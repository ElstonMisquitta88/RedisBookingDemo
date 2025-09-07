using Microsoft.Extensions.Configuration;
using RedisBookingApp;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Net.Sockets;

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
            CustomReddis.DeleteAllKeys(db);
            Console.WriteLine("Connected to Redis.");

            string doctorId = "doctor_123";

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1.  Delete all keys");
                Console.WriteLine("2.  Create booking slots for a doctor");
                Console.WriteLine("3.  View all slots for a doctor");
                Console.WriteLine("4.  Lock a slot for a Doctor");
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


                    case "2":
                        // Create Booking Slots for a Doctor
                        string _doctorKey = BookingEngine.SetSlots(db, doctorId);
                        Console.WriteLine($"Slots created for {_doctorKey}.");
                        break;


                    case "3":
                        // View All Slots for a Doctor
                        HashEntry[] allSlots = BookingEngine.GetAllSlots(db, doctorId);
                        foreach (var entry in allSlots)
                        {
                            Console.WriteLine($"{entry.Name} → {entry.Value}");
                        }
                        break;


                    case "4":
                        // Lock a slot for a Doctor
                        Console.Write("Enter Slot ID : ");
                        string? slotId = Console.ReadLine();

                        Console.Write("Enter UserID : ");
                        string? userId = Console.ReadLine();

                        int locked = BookingEngine.LockBookingSlot(db, slotId!, doctorId, userId!);

                        if (locked == 0)
                        {
                            Console.WriteLine($"Slot {slotId} is already locked by another user");
                        }
                        else if (locked == 2)
                        {
                            Console.WriteLine($"Slot {slotId} Already Booked");
                        }
                        else if (locked == -1)
                        {
                            Console.WriteLine($"Slot {slotId} does not Exists");
                        }
                        else if (locked == 1)
                        {
                            Console.WriteLine($"Lock acquired for slot {slotId} by {userId}");
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