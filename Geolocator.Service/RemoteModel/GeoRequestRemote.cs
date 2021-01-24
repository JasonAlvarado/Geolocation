using System;
using System.Collections.Generic;
using System.Text;

namespace Geolocator.Service.RemoteModel
{
    public class GeoRequestRemote
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
    }
}
