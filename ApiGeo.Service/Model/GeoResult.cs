using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGeo.Service.Model
{
    public class GeoResult
    {
        [ForeignKey("GeoRequest")]
        public int GeoResultId { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string State { get; set; }

        public virtual GeoRequest GeoRequest { get; set; }
    }
}
