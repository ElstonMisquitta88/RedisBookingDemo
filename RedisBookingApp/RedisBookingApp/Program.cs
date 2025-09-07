using Microsoft.Extensions.Configuration;
using RedisBookingApp;
using StackExchange.Redis;

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
        var program = new Program();
        string EndPoint = program.config["ReddisSettings:EndPoints"];
        int Port = Convert.ToInt32(program.config["ReddisSettings:Port"]);
        string User = program.config["ReddisSettings:User"];
        string Password = program.config["ReddisSettings:Password"];

        IDatabase db = CustomReddis.ReddisConnect(EndPoint, Port, User, Password);
        Console.WriteLine("Connected to Redis");
     


    }


}