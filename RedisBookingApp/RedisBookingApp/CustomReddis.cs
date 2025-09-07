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
    public static void ReddisClose(IDatabase db)
    {
        var muxer = db.Multiplexer;
        if (muxer != null && muxer.IsConnected)
        {
            muxer.Close();
        }
    }
    public static void DeleteAllKeys(IDatabase db)
    {
        var muxer = db.Multiplexer;
        var endpoints = muxer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = muxer.GetServer(endpoint);
            var keys = server.Keys();
            foreach (var key in keys)
            {
                db.KeyDelete(key);
            }
        }
    }


}
