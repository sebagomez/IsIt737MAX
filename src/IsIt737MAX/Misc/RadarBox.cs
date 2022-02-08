using System.Collections.Generic;
using Newtonsoft.Json;

namespace IsIt737MAX.Misc
{
    public class Current
    {
        public string csalna { get; set; }
        public string acd { get; set; }
    }

    public class List2
    {
        public List<Current> list { get; set; }
    }

    public class RadarBox
    {
        public Current current { get; set; }
        [JsonProperty("list")]
        public List2 list { get; set; }
    }
}
