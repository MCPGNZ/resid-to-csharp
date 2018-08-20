namespace resid_csharp_bindings
{
    using System.Windows;
    using pk;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /* create sid emulator */
            _Sid = new Sid();

            /* setup parameters */
            _Sid.Parameters.Volume = 1.0f;

            var voice = _Sid.Parameters.Voices[0];
            voice.Sustain = 1.0f;
            voice.Gate = true;
            voice.Frequency = 0.05f;
            voice.Waveform = Sidlib.Waveform.Sawtooth;

            /* play with NAudio */
            var player = new SidWave(_Sid);
            player.Play();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _Sid.Parameters.Voices[0].Frequency = (float)e.NewValue;
        }

        private readonly Sid _Sid;
    }
}