using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGeo.Service.Model
{
    public class GeoRequest
    {
        public int Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }

        [Required]
        public string Country { get; set; }
    }
}
