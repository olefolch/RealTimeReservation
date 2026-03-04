using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeReservation.Domain.Aggregates.Seat
{
    public enum ReservationStatus
    {
        Active,
        Expired,
        Confirmed
    }
}
