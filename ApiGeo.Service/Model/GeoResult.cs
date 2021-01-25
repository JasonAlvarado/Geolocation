using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGeo.Service.Model
{
    public class GeoResult
    {
        public int Id { get; set; }
        public int GeoResultId { get; set; }
        public string Lat { get; set; }

        public string Long { get; set; }
        public string State { get; set; }
    }
}
