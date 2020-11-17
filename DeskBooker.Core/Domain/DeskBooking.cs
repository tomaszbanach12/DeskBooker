using System;
using System.Collections.Generic;

namespace DeskBooker.Core.Domain
{

    public class DeskBooking : DeskBookingBase
    {
        public Guid DeskId { get; set; }
    }
}