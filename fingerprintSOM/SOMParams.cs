namespace _2DOrganizing
{
    public class SOMParams
    {
        public double LearningRate { get; set; }

        public double LearningRadius { get; set; }

        public double AngleMultiplier { get; set; }

        public double CoordsMultiplier { get; set; }

        public double TypeMultiplier { get; set; }

        public double HashCodeMultiplier { get; set; }

        public int Iterations { get; set; }

        public int CharacteristicsCount { get; set; }

        public SOMParams()
        {
        }

        public SOMParams(double learningRadius, double learningRate, double angleMultiplier, double typeMultiplier, double coordsMultiplier, double hashCodeMultiplier, int iterations, int characteristicsCount)
        {
            LearningRadius = learningRadius;
            LearningRate = learningRate;
            AngleMultiplier = angleMultiplier;
            TypeMultiplier = typeMultiplier;
            CoordsMultiplier = coordsMultiplier;
            HashCodeMultiplier = hashCodeMultiplier;
            Iterations = iterations;
            CharacteristicsCount = characteristicsCount;
        }
    }
}
