using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeReservation.Domain.Exceptions
{
    public class InvalidSeatStateException : Exception
    {
        public InvalidSeatStateException(string message)
            : base(message)
        {
        }
    }
}
