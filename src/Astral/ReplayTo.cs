namespace Astral
{
    public class ReplayToInfo
    {
        public ReplayToInfo(string replayTo, string replayOn)
        {
            ReplayTo = replayTo;
            ReplayOn = replayOn;
        }

        /// <summary>
        /// replay to hint for transport
        /// </summary>
        public string ReplayTo { get; }
        
        /// <summary>
        /// response correlation id 
        /// </summary>
        public string ReplayOn { get; }
    }
}