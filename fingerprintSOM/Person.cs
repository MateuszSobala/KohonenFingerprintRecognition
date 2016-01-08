using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DOrganizing
{
    public class Person
    {
        public string Id;
        public List<Fingerprint> Fingerprints;

        public Person(string id)
        {
            Id = id;
            Fingerprints = new List<Fingerprint>();
        }

        public Person(string id, List<Fingerprint> fingerprints)
        {
            Id = id;
            Fingerprints = fingerprints;
        }
    }
}
