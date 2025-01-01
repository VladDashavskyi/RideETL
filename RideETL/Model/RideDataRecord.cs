using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideETL.Model
{
    public class RideDataRecord
    {
        public DateTime PickUpDateTime { get; set; }
        public DateTime DropOffDateTime { get; set; }
        public int PassengerCount { get; set; }
        public float TripDistance { get; set; }
        public string StoreAndFwdFlag { get; set; }
        public int PULocationId { get; set; }
        public int DOLocationId { get; set; }
        public decimal FareAmount { get; set; }
        public decimal TipAmount { get; set; }
    }
}
