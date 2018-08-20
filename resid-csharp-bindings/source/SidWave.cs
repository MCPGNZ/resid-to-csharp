namespace pk
{
    using NAudio.Wave;

    /// <summary>
    /// Showcases Sid emulator
    /// </summary>
    public class SidWave : IWaveProvider
    {
        #region Public Variables
        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(_Hz, 1);
        #endregion Public Variables

        #region Public Methods
        public SidWave(Sid sid, int hz = 20000)
        {
            _Sid = sid;
            _Hz = hz;
        }

        public void Play()
        {
            _WaveOut = new WaveOut();
            _WaveOut.Init(this);
            _WaveOut.Play();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int floatCount = count / 4;
            int floatOffset = offset / 4;

            var floatBuffer = new WaveBuffer(buffer).FloatBuffer;
            for (int n = 0; n < floatCount; ++n)
            {
                /* each sample progresses _Clock/_Hz processor ticks */
                _Sid.Clock(_Clock / _Hz);
                float sample = _Sid.Output() / 65536.0f;

                floatBuffer[floatOffset + n] = sample;
            }

            return count;
        }
        #endregion Public Methods

        #region Private Variables
        private readonly Sid _Sid;
        private WaveOut _WaveOut;

        private const int _Clock = 1000000;
        private readonly int _Hz;
        #endregion Private Variables
    }
}