using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBookingApp;

public class CustomReddis
{
    public static IDatabase ReddisConnect(string EndPoint,int Port, string User, string Password)
    {
        var muxer = ConnectionMultiplexer.Connect(
                    new ConfigurationOptions
                    {
                        EndPoints = { { EndPoint, Port } },
                        User = User,
                        Password = Password
                    }
                );
        var db = muxer.GetDatabase();
        return db;
    }
}
