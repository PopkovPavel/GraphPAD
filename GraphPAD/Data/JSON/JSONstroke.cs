using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphPAD.Data.JSON
{
    public class JSONstroke
    {
     //   [JsonProperty("StrokeArray")]
        public StylusPointCollection StrokeArray { get; set; }

      //  [JsonProperty("Color")]
        public Color Color { get; set; }
      //  [JsonProperty("Width")]
        public double Width { get; set; }


    }
}
