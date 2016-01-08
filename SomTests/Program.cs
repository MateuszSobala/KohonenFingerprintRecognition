using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _2DOrganizing;
using System.Drawing;
using System.IO;

namespace SomTests
{
    public class Program
    {
        private static readonly IList<string> FingerprintImages = new List<string>
        {
            "012_3_1.tif",
            "012_3_3.tif",
            "012_3_8.bmp",
            "012_4_1.tif",
            "012_4_3.tif",
            "012_5_1.tif",
            "012_5_3.tif",
            "022_3_1.tif",
            "022_3_3.tif",
            "022_4_1.tif",
            "022_4_3.tif",
            "022_5_1.tif",
            "022_5_3.tif",
            "045_3_1.tif",
            "045_3_3.tif",
            "045_4_1.tif",
            "045_4_3.tif",
            "045_5_1.tif",
            "045_5_3.tif"
        };

        static void Main(string[] args)
        {
            foreach (var fingerprintImage in FingerprintImages)
            {
                var somTester = new SomTester(fingerprintImage);
                somTester.StartTesting();
            }
        }
    }
}
