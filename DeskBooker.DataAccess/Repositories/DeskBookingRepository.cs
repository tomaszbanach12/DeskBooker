using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.DataAccess.Repositories
{
    public class DeskBookingRepository : IDeskBookingRepository
    {
        private readonly DeskBookerContext _deskBookerContext;

        public DeskBookingRepository(DeskBookerContext deskBookerContext)
        {
            _deskBookerContext = deskBookerContext;
        }

        public IEnumerable <DeskBooking> GetAll()
        {
            return _deskBookerContext.DeskBooking.OrderBy(x => x.Date).ToList();
        }

        public DeskBooking Save(DeskBooking newDeskBooking)
        {
            _deskBookerContext.Add(newDeskBooking);
            _deskBookerContext.SaveChanges();
            return newDeskBooking;
        }
    }
}
