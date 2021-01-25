﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geolocator.Service.RemoteModel
{
    public class GeoResultRemote
    {
        public int GeoResultId { get; set; }
        public string Lat { get; set; }

        public string Lon { get; set; }
        public string State { get; set; }
    }
}
