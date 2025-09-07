using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RedisBookingApp;

public class SlotDetails
{
    public string status { get; set; }
    public string time { get; set; }
    public string bookedBy { get; set; }
}

public class BookingEngine
{

    private static string Doctor_KeyPrefix = "available_slots";
    private static string Lock_KeyPrefix = "lock";


    // Create Slots for a Doctor - Pass doctorId as parameters
    public static string SetSlots(IDatabase db, string doctorId)
    {
        var date = DateTime.Now.ToString("yyyyMMdd");
        var doctorKey = $"{Doctor_KeyPrefix}:{doctorId}:{date}";


        // Define slots
        var slot1 = new { status = "available", time = "10:00AM-10:30AM", bookedBy = "" };
        var slot2 = new { status = "booked", time = "10:30AM-11:00AM", bookedBy = "" };
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

    // Fetch all slots for a Doctor - Pass doctorId as parameters
    public static HashEntry[] GetAllSlots(IDatabase db, string doctorId)
    {
        var date = DateTime.Now.ToString("yyyyMMdd");
        var doctorKey = $"{Doctor_KeyPrefix}:{doctorId}:{date}";
        var allSlots = db.HashGetAll(doctorKey);
        return allSlots;
    }


    public static int LockBookingSlot(IDatabase db, string slotId, string doctorId, string userId)
    {
        int locked = 0;
        var date = DateTime.Now.ToString("yyyyMMdd");
        var doctorKey = $"{Doctor_KeyPrefix}:{doctorId}:{date}";
        var slotJson = db.HashGet(doctorKey, slotId);
        if (slotJson.IsNullOrEmpty)
        {
            locked = -1; // Slot does not exist
        }
        else
        {
            // Slot Exists , check if already booked
            var slot = JsonSerializer.Deserialize<SlotDetails>(slotJson!);
            if (slot!.status == "booked")
            {
                locked=2; // Slot already booked
            }
            else
            {
                // Acquire Lock
                var lockKey = $"{Lock_KeyPrefix}:{doctorId}:{slotId}";
                bool _res = db.StringSet(lockKey, userId, TimeSpan.FromMinutes(5), when: When.NotExists);
                if (_res) { locked = 1; } 
            }
        }

        return locked;
    }
}
