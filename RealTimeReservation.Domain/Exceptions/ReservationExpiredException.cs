using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeReservation.Domain.Exceptions
{
    public class ReservationExpiredException : Exception
    {
        public ReservationExpiredException()
            : base("Reservation has expired.")
        {
        }
    }
}
