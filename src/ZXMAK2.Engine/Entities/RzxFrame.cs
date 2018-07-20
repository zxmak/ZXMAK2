

namespace ZXMAK2.Engine.Entities
{
    public class RzxFrame
    {
        public int FetchCount { get; private set; }
        public byte[] InputData { get; private set; }

        public RzxFrame(int fetchCount, byte[] inputData)
        {
            FetchCount = fetchCount;
            InputData = inputData;
        }
    }
}
