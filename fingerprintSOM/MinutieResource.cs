using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;

namespace _2DOrganizing
{
    public class MinutieResource
    {
        private const string FeaturesDatabase = "Features.csv";
        private const string TaughtFeaturesDatabasePrefix = "Features";
        private const string TaughtFeaturesDatabaseSuffix = ".csv";
        public static readonly string Folder = "Fingerprints/";

        private readonly List<string> existingPeople = new List<string>
        {
            "012",
            "013",
            "017",
            "022",
            "027",
            "045",
            "047",
            "057",
            "076"
        };

        private readonly List<string> wantedFingers = new List<string> {"3", "4", "5"};
        private readonly List<string> wantedScans = new List<string> {"1", "3"};
        public List<Person> People;

        public MinutieResource()
        {
            People = new List<Person>();

            if (File.Exists(string.Format(@"{0}{1}", Folder, FeaturesDatabase)))
            {
                LoadPeople();
            }
            else
            {
                ExtractFeatures();
                SavePeople();
            }
        }

        public List<Minutia> GetAllFeatures()
        {
            var features = new List<Minutia>();

            foreach (var person in People)
            {
                foreach (var fingerprint in person.Fingerprints)
                {
                    foreach (var scan in fingerprint.Scans)
                    {
                        features.AddRange(scan.Features);
                    }
                }
            }

            return features;
        }

        public string GetWinningPerson(List<int> winners)
        {
            var winnerIterator = 0;
            var peopleWinners = new Dictionary<string, int>();

            foreach (var person in People)
            {
                var winnerCounts = 0;
                foreach (var fingerprint in person.Fingerprints)
                {
                    foreach (var scan in fingerprint.Scans)
                    {
                        foreach (var feature in scan.Features)
                        {
                            winnerCounts += winners.Count(x => x == winnerIterator);
                            winnerIterator++;
                        }
                    }
                }
                peopleWinners.Add(person.Id, winnerCounts);
            }

            return peopleWinners.First(p => p.Value == peopleWinners.Max(x => x.Value)).Key;
        }

        private void ExtractFeatures()
        {
            var featExtractor = new Ratha1995MinutiaeExtractor();

            foreach (var person in existingPeople)
            {
                var fingerprintOwner = new Person(person);

                foreach (var finger in wantedFingers)
                {
                    var fingerprintFinger = new Fingerprint(int.Parse(finger));

                    foreach (var scan in wantedScans)
                    {
                        var fingerprintImg =
                            ImageLoader.LoadImage(string.Format(@"{0}{1}_{2}_{3}.tif", Folder, person, finger, scan));
                        var features = featExtractor.ExtractFeatures(fingerprintImg);

                        var fingerprintScan = new Scan(int.Parse(scan), features);
                        fingerprintFinger.Scans.Add(fingerprintScan);
                    }

                    fingerprintOwner.Fingerprints.Add(fingerprintFinger);
                }

                People.Add(fingerprintOwner);
            }
        }

        private void SavePeople()
        {
            var fileName = string.Format(@"{0}{1}", Folder, FeaturesDatabase);
            var fs = new StreamWriter(fileName, false);
            fs.WriteLine("Person;Finger;Scan;FeatureX;FeatureY;FeatureAngle;FeatureType");

            foreach (var person in People)
            {
                foreach (var fingerprint in person.Fingerprints)
                {
                    foreach (var scan in fingerprint.Scans)
                    {
                        foreach (var feature in scan.Features)
                        {
                            fs.WriteLine("{0};{1};{2};{3};{4};{5:f4};{6}", person.Id, fingerprint.Id, scan.Id, feature.X,
                                feature.Y, feature.Angle, (int) feature.MinutiaType);
                        }
                    }
                }
            }
            fs.Close();
        }

        private void LoadPeople()
        {
            var fileName = string.Format(@"{0}{1}", Folder, FeaturesDatabase);
            using (var sr = new StreamReader(fileName))
            {
                var line = sr.ReadLine();
                var splitStringArray = new string[1] {";"};

                line = sr.ReadLine();
                var members = line.Split(splitStringArray, StringSplitOptions.None);
                var person = new Person(members[0]);
                var finger = new Fingerprint(int.Parse(members[1]));
                var scan = new Scan(int.Parse(members[2]));
                var feature = new Minutia(short.Parse(members[3]), short.Parse(members[4]), double.Parse(members[5]))
                {
                    MinutiaType = (MinutiaType) int.Parse(members[6])
                };
                scan.Features.Add(feature);

                // Read lines from the file until the end of the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    members = line.Split(splitStringArray, StringSplitOptions.None);

                    feature = new Minutia(short.Parse(members[3]), short.Parse(members[4]), double.Parse(members[5]))
                    {
                        MinutiaType = (MinutiaType) int.Parse(members[6])
                    };
                    scan.Features.Add(feature);

                    var scanId = int.Parse(members[2]);
                    if (scan.Id != scanId)
                    {
                        finger.Scans.Add(scan);
                        scan = new Scan(scanId);
                    }

                    var fingerId = int.Parse(members[1]);
                    if (finger.Id != fingerId)
                    {
                        person.Fingerprints.Add(finger);
                        finger = new Fingerprint(fingerId);
                    }

                    if (person.Id != members[0])
                    {
                        People.Add(person);
                        person = new Person(members[0]);
                    }
                }

                finger.Scans.Add(scan);
                person.Fingerprints.Add(finger);
                People.Add(person);

                sr.Close();
            }
        }

        private static string GetTaughtFeaturesFilenameByParams(SOMParams somParams)
        {
            return string.Format(@"{0}{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}{9}", Folder, TaughtFeaturesDatabasePrefix,
                somParams.LearningRadius, somParams.LearningRate * 100000, somParams.Iterations, somParams.AngleMultiplier * 100000,
                somParams.TypeMultiplier * 100000, somParams.CoordsMultiplier * 100000, somParams.HashCodeMultiplier * 100000,
                TaughtFeaturesDatabaseSuffix);
        }

        public static bool IsAlreadyTaughtAndSaved(SOMParams somParams)
        {
            return File.Exists(GetTaughtFeaturesFilenameByParams(somParams));
        }

        public void SaveTaughtPeople(double[][] features, SOMParams somParams)
        {
            var fileName = GetTaughtFeaturesFilenameByParams(somParams);
            var fs = new StreamWriter(fileName, false);
            fs.WriteLine("Person;Finger;Scan;FeatureX;FeatureY;FeatureAngle;FeatureType");
            var count = 0;

            foreach (var person in People)
            {
                foreach (var fingerprint in person.Fingerprints)
                {
                    foreach (var scan in fingerprint.Scans)
                    {
                        var scanFeaturesCount = scan.Features.Count;
                        var scanFeaturesIter = 0;
                        while (scanFeaturesCount > scanFeaturesIter)
                        {
                            fs.WriteLine("{0};{1};{2};{3:f4};{4:f4};{5:f4};{6:f4};{7:f4}", person.Id, fingerprint.Id,
                                scan.Id, features[count][2], features[count][3], features[count][0], features[count][1],
                                features[count][4]);
                            count++;
                            scanFeaturesIter++;
                        }
                    }
                }
            }
            fs.Close();
        }

        public double[][] LoadTaughtPeople(SOMParams somParams)
        {
            double[][] features;
            var fileName = GetTaughtFeaturesFilenameByParams(somParams);

            People = new List<Person>();
            using (var sr = new StreamReader(fileName))
            {
                var lineCount = File.ReadAllLines(fileName).Count();
                features = new double[lineCount - 1][];
                var count = 0;
                var line = sr.ReadLine();
                var splitStringArray = new string[1] {";"};

                line = sr.ReadLine();
                var members = line.Split(splitStringArray, StringSplitOptions.None);
                var person = new Person(members[0]);
                var finger = new Fingerprint(int.Parse(members[1]));
                var scan = new Scan(int.Parse(members[2]));
                var feature = new Minutia((short)double.Parse(members[3]), (short)double.Parse(members[4]), double.Parse(members[5]))
                {
                    MinutiaType = (MinutiaType)double.Parse(members[6])
                };
                scan.Features.Add(feature);

                features[count] = new double[5];
                features[count][2] = double.Parse(members[3]);
                features[count][3] = double.Parse(members[4]);
                features[count][0] = double.Parse(members[5]);
                features[count][1] = double.Parse(members[6]);
                features[count][4] = double.Parse(members[7]);
                count++;

                // Read lines from the file until the end of the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    members = line.Split(splitStringArray, StringSplitOptions.None);

                    features[count] = new double[5];
                    features[count][2] = double.Parse(members[3]);
                    features[count][3] = double.Parse(members[4]);
                    features[count][0] = double.Parse(members[5]);
                    features[count][1] = double.Parse(members[6]);
                    features[count][4] = double.Parse(members[7]);
                    count++;

                    feature = new Minutia((short)double.Parse(members[3]), (short)double.Parse(members[4]), double.Parse(members[5]))
                    {
                        MinutiaType = (MinutiaType)double.Parse(members[6])
                    };
                    scan.Features.Add(feature);

                    var scanId = int.Parse(members[2]);
                    if (scan.Id != scanId)
                    {
                        finger.Scans.Add(scan);
                        scan = new Scan(scanId);
                    }

                    var fingerId = int.Parse(members[1]);
                    if (finger.Id != fingerId)
                    {
                        person.Fingerprints.Add(finger);
                        finger = new Fingerprint(fingerId);
                    }

                    if (person.Id != members[0])
                    {
                        People.Add(person);
                        person = new Person(members[0]);
                    }
                }

                finger.Scans.Add(scan);
                person.Fingerprints.Add(finger);
                People.Add(person);

                sr.Close();
            }

            return features;
        }
    }
}