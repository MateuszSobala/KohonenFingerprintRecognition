using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternRecognition.FingerprintRecognition.Core;

namespace _2DOrganizing
{
    public class Fingerprint
    {
        public int Id;
        public List<Scan> Scans;

        public Fingerprint(int id)
        {
            Id = id;
            Scans = new List<Scan>();
        }

        public Fingerprint(int id, List<Scan> scans)
        {
            Id = id;
            Scans = scans;
        }
    }
}
