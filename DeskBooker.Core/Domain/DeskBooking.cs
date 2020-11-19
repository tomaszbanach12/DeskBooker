using System;
using System.Collections.Generic;

namespace DeskBooker.Core.Domain
{

    public class DeskBooking : DeskBookingBase
    {
        public Guid Id { get; set; }
        public Guid DeskId { get; set; }   
    }
}