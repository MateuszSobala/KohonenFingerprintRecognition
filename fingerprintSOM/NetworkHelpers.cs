using AForge.Neuro;
using AForge.Neuro.Learning;
using PatternRecognition.FingerprintRecognition.Core;
using System.Collections.Generic;
using System.Drawing;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;

namespace _2DOrganizing
{
    public class NetworkHelper
    {
        public DistanceNetwork _som;
        public SOMLearning _teacher;
        public int[] _networkSize;

        public readonly double[][] _inputs;
        private readonly double _learningRadius;
        private readonly double _fixedLearningRate;
        private readonly double _driftingLearningRate;

        public NetworkHelper(IList<Minutia> features, SOMParams somParams)
        {
            var trainingFeaturesCount = features.Count;
            var inputs = new double[trainingFeaturesCount][];
            var featureCharacteristicsCount = somParams.CharacteristicsCount;

            for (var i = 0; i < trainingFeaturesCount; i++)
            {
                var feature = features[i];

                inputs[i] = new double[featureCharacteristicsCount];
                inputs[i][0] = feature.Angle * somParams.AngleMultiplier;
                inputs[i][1] = (double)feature.MinutiaType * somParams.TypeMultiplier;
                inputs[i][2] = feature.X * somParams.CoordsMultiplier;
                inputs[i][3] = feature.Y * somParams.CoordsMultiplier;
                inputs[i][4] = feature.GetHashCode()*somParams.HashCodeMultiplier;
            }

            _networkSize = CreateRectangleFromFeatures(trainingFeaturesCount);

            _som = new DistanceNetwork(featureCharacteristicsCount, trainingFeaturesCount);
            _teacher = new SOMLearning(_som, _networkSize[0], _networkSize[1]);
            _inputs = inputs;
            _learningRadius = somParams.LearningRadius;
            _fixedLearningRate = somParams.LearningRate / 10;
            _driftingLearningRate = _fixedLearningRate * 9;
        }

        public NetworkHelper(double[][] features, SOMParams somParams)
        {
            var trainingFeaturesCount = features.GetLength(0);
            var featureCharacteristicsCount = somParams.CharacteristicsCount;

            _networkSize = CreateRectangleFromFeatures(trainingFeaturesCount);

            _som = new DistanceNetwork(featureCharacteristicsCount, trainingFeaturesCount);
            _teacher = new SOMLearning(_som, _networkSize[0], _networkSize[1]);
            _inputs = features;
            _learningRadius = somParams.LearningRadius;
            _fixedLearningRate = somParams.LearningRate / 10;
            _driftingLearningRate = _fixedLearningRate * 9;
        }

        public void Teach(int iterations, int currentIteration)
        {
            _teacher.LearningRate = _driftingLearningRate * (iterations - currentIteration) / iterations + _fixedLearningRate;
            _teacher.LearningRadius = _learningRadius * (iterations - currentIteration) / iterations;

            _teacher.RunEpoch(_inputs);
        }

        public static double[][] GetQueryFeaturesSet(Bitmap image, SOMParams somParams)
        {
            var featExtractor = new Ratha1995MinutiaeExtractor();
            var features = featExtractor.ExtractFeatures(image);

            var queryFeaturesSet = new double[features.Count][];

            for (var i = 0; i < features.Count; i++)
            {
                // create new sample
                var feature = features[i];

                queryFeaturesSet[i] = new double[somParams.CharacteristicsCount];
                queryFeaturesSet[i][0] = feature.Angle * somParams.AngleMultiplier;
                queryFeaturesSet[i][1] = (double)feature.MinutiaType * somParams.TypeMultiplier;
                queryFeaturesSet[i][2] = feature.X * somParams.CoordsMultiplier;
                queryFeaturesSet[i][3] = feature.Y * somParams.CoordsMultiplier;
                queryFeaturesSet[i][4] = feature.GetHashCode() * somParams.HashCodeMultiplier;
            }

            return queryFeaturesSet;
        }

        private static int[] CreateRectangleFromFeatures(int featureCount)
        {
            int[] result = { 1, 1 };

            int[] modulos = { 2, 3, 5, 7, 11, 13, 17, 19};
            var tempModulo = 1;

            do
            {
                result[0] *= tempModulo;
                featureCount /= tempModulo;
                tempModulo = 0;

                foreach (var modulo in modulos)
                {
                    if (featureCount % modulo == 0)
                    {
                        tempModulo = modulo;
                        break;
                    }
                }
            } while (tempModulo != 0);

            result[1] = featureCount;

            return result;
        }
    }
}
