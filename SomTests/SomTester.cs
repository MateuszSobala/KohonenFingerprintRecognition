using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using PatternRecognition.FingerprintRecognition.Core;
using _2DOrganizing;

namespace SomTests
{
    public class SomTester
    {
        private IList<SOMParams> _somParamses;
        private readonly IList<int> _iterations = new List<int> { 100, 500 };
        private readonly IList<double> _learningRates = new List<double> { 0.2, 0.3, 0.4};
        private readonly IList<int> _learningRadiuses = new List<int> { 2, 5, 10 };
        private readonly IList<double> _angleMultipliers = new List<double> { 0, 1, 5};
        private readonly IList<double> _typeMultipliers = new List<double> { 0, 10, 100 };
        private readonly IList<double> _coordsMultipliers = new List<double> { 0, 0.01, 1 };
        private readonly IList<double> _hashCodeMultipliers = new List<double> { 0, 0.00000000001, 0.0000000001 };
        private readonly string _fileName;
        private readonly IProducerConsumerCollection<string> _winnersList = new ConcurrentQueue<string>();
        public static readonly string Folder = "Fingerprints/";
        public static readonly string CsvExtension = ".csv";

        public SomTester(string fileName)
        {
            InitializeParams();
            _fileName = fileName;
        }

        public void StartTesting()
        {
            var inputImage = ImageLoader.LoadImage(AppDomain.CurrentDomain.BaseDirectory + _fileName);
            Parallel.ForEach(_somParamses, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (currentParams) =>
            {
                object clonedImage;
                try
                {
                    clonedImage = inputImage.Clone();
                }
                catch (InvalidOperationException ioe)
                {
                    const string winnerException = "000";
                    var formattedWinnerWithParamsException = string.Format(@"{0};{1};{2};{3};{4};{5};{6};{7}", winnerException,
                        currentParams.LearningRadius, currentParams.LearningRate, currentParams.Iterations,
                        currentParams.AngleMultiplier,
                        currentParams.TypeMultiplier, currentParams.CoordsMultiplier,
                        currentParams.HashCodeMultiplier);

                    _winnersList.TryAdd(formattedWinnerWithParamsException);
                    return;
                }
                
                var queryFeaturesSet = NetworkHelper.GetQueryFeaturesSet((Bitmap)clonedImage, currentParams);

                var resource = new MinutieResource();
                NetworkHelper networkHelper;
                if (MinutieResource.IsAlreadyTaughtAndSaved(currentParams))
                {
                    networkHelper = new NetworkHelper(resource.LoadTaughtPeople(currentParams), currentParams);
                    networkHelper.Teach(currentParams.Iterations, currentParams.Iterations - 1);
                }
                else
                {
                    networkHelper = new NetworkHelper(resource.GetAllFeatures(), currentParams);

                    var iter = 0;
                    while (!(iter >= currentParams.Iterations - 1))
                    {
                        networkHelper.Teach(currentParams.Iterations, iter);
                        iter++;
                    }

                    resource.SaveTaughtPeople(networkHelper._inputs, currentParams);

                    networkHelper.Teach(currentParams.Iterations, iter);
                }

                var winners = new List<int>();
                foreach (var queryFeature in queryFeaturesSet)
                {
                    networkHelper._som.Compute(queryFeature);
                    var w = networkHelper._som.GetWinner();
                    winners.Add(w);
                }

                var winner = resource.GetWinningPerson(winners);
                var formattedWinnerWithParams = string.Format(@"{0};{1};{2};{3};{4};{5};{6};{7}", winner,
                    currentParams.LearningRadius, currentParams.LearningRate, currentParams.Iterations,
                    currentParams.AngleMultiplier,
                    currentParams.TypeMultiplier, currentParams.CoordsMultiplier,
                    currentParams.HashCodeMultiplier);

                _winnersList.TryAdd(formattedWinnerWithParams);
            });

            SaveResults();
        }

        private void SaveResults()
        {
            var resultFileName = String.Format(@"{0}{1}{2}", Folder, _fileName.Substring(0, 7), CsvExtension);
            var fs = new StreamWriter(resultFileName, false);
            fs.WriteLine("Winner;LearningRadius;LearningRate;Iterations;AngleMultiplier;TypeMultiplier;CoordsMultiplier;HashCodeMultiplier");

            foreach (var s in _winnersList.ToArray())
            {
                fs.WriteLine(s);
            }
            fs.Close();
        }

        private void InitializeParams()
        {
            _somParamses = new List<SOMParams>();
            foreach (var iteration in _iterations)
            {
                foreach (var learningRate in _learningRates)
                {
                    foreach (var learningRadius in _learningRadiuses)
                    {
                        foreach (var angleMultiplier in _angleMultipliers)
                        {
                            foreach (var typeMultiplier in _typeMultipliers)
                            {
                                foreach (var coordsMultiplier in _coordsMultipliers)
                                {
                                    foreach (var hashCodeMultiplier in _hashCodeMultipliers)
                                    {
                                        var somParams = new SOMParams(learningRadius, learningRate, angleMultiplier, typeMultiplier, coordsMultiplier, hashCodeMultiplier, iteration, 5);
                                        _somParamses.Add(somParams);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
