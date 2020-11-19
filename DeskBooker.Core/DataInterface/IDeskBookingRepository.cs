using DeskBooker.Core.Domain;
using System.Collections.Generic;

namespace DeskBooker.Core.DataInterface
{
    public interface IDeskBookingRepository
    {
        DeskBooking Save(DeskBooking deskBooking);

        IEnumerable<DeskBooking> GetAll();
    }
}
