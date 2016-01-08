using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition.FingerprintRecognition.Core;

namespace _2DOrganizing
{
    public class Scan
    {
        public int Id;
        public List<Minutia> Features;
        public int CorrelationScore = 0;

        public Scan(int id)
        {
            Id = id;
            Features = new List<Minutia>();
        }

        public Scan(int id, List<Minutia> features)
        {
            Id = id;
            Features = features;
        }
    }
}
