using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisBookingApp;

public class SlotDetails
{
    public string status { get; set; }
    public string time { get; set; }
    public string bookedBy { get; set; }
}
public class BookingEngine
{
    public static string SetSlots(IDatabase db, string doctorId)
    {
        var date = DateTime.Now.ToString("yyyyMMdd");
        var doctorKey = $"available_slots:{doctorId}:{date}";

        // Define slots
        var slot1 = new { status = "available", time = "10:00AM-10:30AM", bookedBy = "" };
        var slot2 = new { status = "available", time = "10:30AM-11:00AM", bookedBy = "" };
        var slot3 = new { status = "available", time = "11:00AM-11:30AM", bookedBy = "" };

        // Set TTL for the key to expire in 24 hours
        db.KeyExpire(doctorKey, TimeSpan.FromHours(24));

        // Save as Hash fields (serialize as JSON)
        db.HashSet(doctorKey, new HashEntry[]
        {
            new HashEntry(doctorId+"01", JsonSerializer.Serialize(slot1)),
            new HashEntry(doctorId+"02".ToString(), JsonSerializer.Serialize(slot2)),
            new HashEntry(doctorId+"03".ToString(), JsonSerializer.Serialize(slot3)),
        });

        return doctorKey;
    }
}
