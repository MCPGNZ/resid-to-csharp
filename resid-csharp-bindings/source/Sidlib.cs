namespace pk
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Low-level class for accessing resid-0.16.dll methods
    /// Sidlib is a .dll wrapper for slightly modified Resid implementation
    /// </summary>
    public class Sidlib
    {
        #region Public Types
        public enum Waveform
        {
            Noise = 128,
            Pulse = 64,
            Sawtooth = 32,
            Triangle = 16
        }
        public enum ChipModel
        {
            Mos6581,
            Mos8580
        }
        public enum SamplingMethod
        {
            SampleFast,
            SampleInterpolate,
            SampleResampleInterpolate,
            SampleResampleFast
        }
        #endregion Public Types

        #region Lifetime
        /// <summary>
        /// Returns a pointer to SID instance
        /// </summary>
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "create_sid")]
        public static extern IntPtr CreateSID();

        /// <summary>
        /// Releases SID instance
        /// </summary>
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "release_sid")]
        public static extern void ReleaseSID(IntPtr ptr);
        #endregion Lifetime

        #region Features
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "set_chip_model")]
        public static extern void SetChipModel(IntPtr ptr, ChipModel model);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "enable_filter")]
        public static extern void EnableFilter(IntPtr ptr, bool enable);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "enable_external_filter")]
        public static extern void EnableExternalFilter(IntPtr ptr, bool enable);
        #endregion Features

        #region Sampling
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "set_sampling_parameters")]
        public static extern bool SetSamplingParameters(IntPtr ptr, double clockFreq, SamplingMethod method, double sampleFreq, double passFreq = -1, double filterScale = 0.97);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "adjust_sampling_frequency")]
        public static extern void AdjustSamplingFrequency(IntPtr ptr, bool enable);
        #endregion Sampling

        #region Simulation
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "clock")]
        public static extern void Clock(IntPtr ptr);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "clock_t")]
        public static extern void Clock(IntPtr ptr, int deltaT);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "reset")]
        public static extern void Reset(IntPtr ptr);
        #endregion Simulation

        #region State
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "read")]
        public static extern int Read(IntPtr ptr, int address);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "write")]
        public static extern void Write(IntPtr ptr, int address, int value);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "input")]
        public static extern void Input(IntPtr ptr, int sample);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "output")]
        public static extern int Output(IntPtr ptr);

        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "output_n")]
        public static extern int Output(IntPtr ptr, int bits);
        #endregion State

        #region Internal
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "read_state")]
        public static extern void ReadState(IntPtr ptr, byte[] registers);
        [DllImport(_LibPath, CallingConvention = _Convention, EntryPoint = "write_state")]
        public static extern void WriteState(IntPtr ptr, byte[] registers);
        #endregion Internal

        #region Private Variables
        private const string _LibPath = "resid-0.16.dll";
        private const CallingConvention _Convention = CallingConvention.StdCall;
        #endregion Private Variables
    }

    /// <summary>
    /// RAII higher-level Sidlib wrapper
    /// </summary>
    public class Sid
    {
        #region Lifetime
        public Sid()
        {
            _Sid = Sidlib.CreateSID();
        }
        ~Sid()
        {
            Sidlib.ReleaseSID(_Sid);
        }
        #endregion Lifetime

        #region Public Variables
        public SidParameters Parameters => _Parameters ?? (_Parameters = new SidParameters(this));
        #endregion Public Variables

        #region Public Methods
        #region Features
        public void SetChipModel(Sidlib.ChipModel model)
        {
            Sidlib.SetChipModel(_Sid, model);
        }
        public void EnableFilter(bool enable)
        {
            Sidlib.EnableFilter(_Sid, enable);
        }

        public void EnableExternalFilter(bool enable)
        {
            Sidlib.EnableExternalFilter(_Sid, enable);
        }
        #endregion Features

        #region Sampling
        public bool SetSamplingParameters(double clockFreq, Sidlib.SamplingMethod method, double sampleFreq,
            double passFreq = -1, double filterScale = 0.97)
        {
            return Sidlib.SetSamplingParameters(_Sid, clockFreq, method, sampleFreq, passFreq, filterScale);
        }

        public void AdjustSamplingFrequency(bool enable)
        {
            Sidlib.AdjustSamplingFrequency(_Sid, enable);
        }
        #endregion Sampling

        #region Simulation
        /// <summary>
        /// Simulates one cycle of SID
        /// </summary>
        public void Clock()
        {
            Sidlib.Clock(_Sid);
        }

        /// <summary>
        /// Simulates deltaT cycles of SID
        /// </summary>
        /// <param name="deltaT">Number of cycles to simulate</param>
        public void Clock(int deltaT)
        {
            Sidlib.Clock(_Sid, deltaT);
        }

        public void Reset()
        {
            Sidlib.Reset(_Sid);
        }
        #endregion Simulation

        #region State
        /// <summary>
        /// Reads SID register
        /// </summary>
        /// <returns>Value of the register</returns>
        public byte Read(byte address)
        {
            if (address < 25) { throw new InvalidOperationException("Sid.Read(...) : operation is only available for addresses above 24"); }
            return (byte)Sidlib.Read(_Sid, address);
        }

        /// <summary>
        /// Writes SID register
        /// </summary>
        public void Write(byte address, byte value)
        {
            if (address >= 25) { throw new InvalidOperationException("Sid.Write(...) : operation is only available for addresses under 25"); }
            Sidlib.Write(_Sid, address, value);
        }

        public void Input(byte sample)
        {
            Sidlib.Input(_Sid, sample);
        }
        /// <summary>
        /// Reads current SID audio output value in 16 bits
        /// </summary>
        public int Output()
        {
            return Sidlib.Output(_Sid);
        }
        /// <summary>
        /// Reads current SID audio output value in N  bits
        /// </summary>
        public int Output(int bits)
        {
            return Sidlib.Output(_Sid, bits);
        }
        #endregion State

        #region Internal
        /// <summary>
        /// Reads internal emulator register state, that is inaccessible in real device
        /// </summary>
        public byte[] ReadState()
        {
            var result = new byte[32];
            Sidlib.ReadState(_Sid, result);
            return result;
        }
        /// <summary>
        // Reads single internal emulator register value, that is inaccessible in real device
        /// </summary>
        public byte ReadState(byte address)
        {
            var state = ReadState();
            return state[address];
        }
        /// <summary>
        // Writes internal register state to the emulator, this is impossible on real device
        /// </summary>
        public void WriteState(byte[] registers)
        {
            Sidlib.WriteState(_Sid, registers);
        }
        #endregion Internal
        #endregion Public Methods

        #region Private Variables
        /// <summary>
        /// Pointer to the C++ SID class instance
        /// </summary>
        private readonly IntPtr _Sid;

        /// <summary>
        /// API for easy-modification of SID parameters
        /// </summary>
        private SidParameters _Parameters;
        #endregion Private Variables
    }

    /// <summary>
    /// Sid register utility
    /// </summary>
    public class SidParameters
    {
        #region Public Types
        public class Voice
        {
            #region Public Variables
            public float Frequency
            {
                set
                {
                    int converted = (int)(value * ((1 << 16) - 1));

                    byte low = (byte)(converted >> 0);
                    byte high = (byte)(converted >> 8);

                    Write(0, low);
                    Write(1, high);
                }
            }
            public float PulseDutyWaveCycle
            {
                set
                {
                    int converted = (int)(value * ((1 << 12) - 1));

                    byte low = (byte)(converted >> 0);
                    byte high = (byte)(converted >> 8);

                    Write(2, low);
                    Write(3, high);
                }
            }

            public Sidlib.Waveform Waveform
            {
                set
                {
                    byte register = Read(4);
                    register &= 0x0F;
                    register |= (byte)value;

                    Write(4, register);
                }
            }

            public bool Test
            {
                set
                {
                    byte register = Read(4);
                    register &= 0xF7;
                    register |= (byte)((value ? 1 : 0) << 3);
                    Write(4, register);
                }
            }
            public bool RingModulation
            {
                set
                {
                    byte register = Read(4);
                    register &= 0xFB;
                    register |= (byte)((value ? 1 : 0) << 2);
                    Write(4, register);
                }
            }
            public bool Synchronize
            {
                set
                {
                    byte register = Read(4);
                    register &= 0xFD;
                    register |= (byte)((value ? 1 : 0) << 1);
                    Write(4, register);
                }
            }
            public bool Gate
            {
                set
                {
                    byte register = Read(4);
                    register &= 0xFE;
                    register |= (byte)(value ? 1 : 0);
                    Write(4, register);
                }
            }

            public float Attack
            {
                set
                {
                    int converted = (int)(value * ((1 << 4) - 1));

                    byte register = Read(5);
                    register &= 0x0F;
                    register |= (byte)(converted << 4);
                    Write(5, register);
                }
            }
            public float Decay
            {
                set
                {
                    int converted = (int)(value * ((1 << 4) - 1));

                    byte register = Read(5);
                    register &= 0xF0;
                    register |= (byte)(converted << 0);
                    Write(5, register);
                }
            }
            public float Sustain
            {
                set
                {
                    int converted = (int)(value * ((1 << 4) - 1));

                    byte register = Read(6);
                    register &= 0x0F;
                    register |= (byte)(converted << 4);
                    Write(6, register);
                }
            }
            public float Release
            {
                set
                {
                    int converted = (int)(value * ((1 << 4) - 1));

                    byte register = Read(6);
                    register &= 0xF0;
                    register |= (byte)(converted << 0);
                    Write(6, register);
                }
            }
            public bool FilterRouting
            {
                set
                {
                    byte register = _Sid.ReadState(23);
                    register &= (byte)~(1 << (_VoiceNumber - 1));
                    register |= (byte)((value ? 1 : 0) << (_VoiceNumber - 1));
                    _Sid.Write(23, register);
                }
            }
            #endregion Public Variables

            #region Public Methods
            public Voice(Sid sid, int voiceNumber)
            {
                _Sid = sid;
                _VoiceNumber = voiceNumber;
            }
            #endregion Public Methods

            #region Private Variables
            private readonly Sid _Sid;
            private readonly int _VoiceNumber;

            private byte _Address => (byte)((_VoiceNumber - 1) * 7);
            #endregion Private Variables

            #region Private Variables
            private void Write(byte address, byte value)
            {
                _Sid.Write((byte)(_Address + address), value);
            }
            private byte Read(byte address)
            {
                return _Sid.ReadState(address);
            }
            #endregion Private Variables
        }
        #endregion Public Types

        #region Public Variables
        public Voice[] Voices;
        public float Volume
        {
            set
            {
                var converted = (byte)(value * ((1 << 4) - 1));

                var register = _Sid.ReadState(24);
                register &= 0xF0;
                register |= (byte)(converted << 0);
                _Sid.Write(24, register);
            }
        }

        public float FilterCutoff
        {
            set
            {
                int converted = (int)(value * ((1 << 12) - 1));

                byte lowRegister = _Sid.ReadState(21);
                lowRegister &= 0xF0;
                lowRegister |= (byte)(converted & 0x0F);
                _Sid.Write(21, lowRegister);

                var highRegister = (byte)((converted >> 4) & 0xFF);
                _Sid.Write(22, highRegister);
            }
        }

        public float FilterResonance
        {
            set
            {
                int converted = (int)(value * ((1 << 4) - 1));

                byte register = _Sid.ReadState(23);
                register &= 0xF0;
                register |= (byte)(converted << 4);
                _Sid.Write(23, register);
            }
        }
        public bool ExternalInput
        {
            set
            {
                byte register = _Sid.ReadState(23);
                register &= 0xF7;
                register |= (byte)(value ? 1 : 0);
                _Sid.Write(23, register);
            }
        }
        public bool MuteVoice3
        {
            set
            {
                var register = _Sid.ReadState(24);
                register &= 0x7F;
                register |= (byte)((value ? 1 : 0) << 7);
                _Sid.Write(24, register);
            }
        }
        public bool HighPass
        {
            set
            {
                var register = _Sid.ReadState(24);
                register &= 0xBF;
                register |= (byte)((value ? 1 : 0) << 6);
                _Sid.Write(24, register);
            }
        }
        public bool BandPass
        {
            set
            {
                var register = _Sid.ReadState(24);
                register &= 0xDF;
                register |= (byte)((value ? 1 : 0) << 5);
                _Sid.Write(24, register);
            }
        }
        public bool LowPass
        {
            set
            {
                var register = _Sid.ReadState(24);
                register &= 0xEF;
                register |= (byte)((value ? 1 : 0) << 4);
                _Sid.Write(24, register);
            }
        }
        #endregion Public Variables

        #region Public Methods
        public SidParameters(Sid sid)
        {
            _Sid = sid;

            /* create voices API */
            var first = new Voice(sid, 1);
            var second = new Voice(sid, 2);
            var third = new Voice(sid, 3);

            Voices = new[] { first, second, third };
        }
        #endregion Public Methods

        #region Private Variables
        private readonly Sid _Sid;
        #endregion Private Variables

        #region Helper Methods
        private static byte ChangeBit(byte source, byte number, bool state)
        {
            byte value = (byte)(1 << number);

            source &= (byte)~value;
            source |= (byte)(state ? 1 : 0);

            return source;
        }
        private static byte ChangeNibble(byte source, bool high, byte state)
        {
            if (high)
            {
                source &= 0x0F;
                source |= (byte)(state << 4);
                return source;
            }
            /* if(low) */

            source &= 0xF0;
            source |= (byte)(state & 0x0F);
            return source;
        }
        #endregion Helper Methods
    }
}