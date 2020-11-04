using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio;
using NAudio.Mixer;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using BInrerface;

namespace WindowsFormsApp1
{
    static class Program
    {
        class Core
        {
            protected delegate void dSoundIt(List<List<IXyidiEvent>> xyidiEvents);
            protected event dSoundIt eSoundIt;
            protected delegate void dPrintError(string errorMasege );
            protected event dSoundIt ePrintError;


            private class NoteLogTemper53 : Note
            {

                private int _noteNumber;
                private float _noteFrequency;
                private static float _rootFrequency;
                private static Dictionary<int, float> NotesFrequency;
                private static Dictionary<float, int> FrequencysNote;

                public float RootFrequency { get => _rootFrequency; }

                public override int Number
                {

                    get => _noteNumber;
                    set
                    {
                        if ((value >= 0) && (value <= NotesFrequency.Count))
                        {
                            _noteNumber = value;
                            _noteFrequency = NotesFrequency[value];
                        }
                    }
                }
                public override float Frequency
                {

                    get => _noteFrequency;
                    set
                    {
                        if ((value >= 20) && (value <= 20_000))
                        {
                            _noteFrequency = value;
                            Number = FrequencysNote[value];
                        }
                    }
                }

                public NoteLogTemper53(int num, float root = 7.83f)
                {
                    _rootFrequency = root;
                    if (NotesFrequency == null) { CreateNoteFreqTebles(root); }
                    Number = num;
                }
                public NoteLogTemper53(float freq, float root = 7.83f)
                {
                    _rootFrequency = root;
                    if (NotesFrequency == null) { CreateNoteFreqTebles(root); }
                    Frequency = freq;
                }

                private void CreateNoteFreqTebles(float root)
                {
                    NotesFrequency = new Dictionary<int, float>();
                    FrequencysNote = new Dictionary<float, int>();
                    while (root > 40) { root /= 2; }
                    int id = 0;
                    while (root < 20_000)
                    {
                        for (int i = 0; i < 53; i++)
                        {
                            float CurNoteFreq = ((root - root / 2) * ((i + 1) % 53)) / 53 + root / 2;
                            if ((CurNoteFreq > 20) && (CurNoteFreq < 20_000))
                            {
                                NotesFrequency.Add(id, CurNoteFreq);
                                FrequencysNote.Add(CurNoteFreq, id);
                                id++;
                            }
                        }
                        root *= 2;
                    }
                }
            }
            private class XyidiEvent : IXyidiEvent
            {
                private Note _note;
                private float _startTime;
                private float _duration;
                private int _velocity;


                public Note Note { get => _note; set => _note = value; }
                public float StartTime
                {
                    get => _startTime;
                    set
                    {
                        if (value > 0) { _startTime = value; }
                    }
                }
                public float Duration
                {
                    get => _duration;
                    set
                    {
                        if (value > 0) { _duration = value; }
                    }
                }
                public int Velocity
                {
                    get => _velocity;
                    set
                    {
                        if ((value >= 0) && (value <= 127)) { _velocity = value; }
                    }
                }
                public XyidiEvent(Note n, int vel, float st, float dr)
                {
                    if (n == null) { throw new Exception("Error222"); }
                    Note = n;
                    Velocity = vel;
                    StartTime = st;
                    Duration = dr;
                }
            }
            private class GUIOnWindowsForms : IGUI
            {
                void IGUI.Start()
                {
                    Console.WriteLine(1);
                }
                void IGUI.PrintError(string ErrorMessage)
                {
                    Console.WriteLine("Error");
                }
            }
            private class AudioWithNAudio : IAudio
            {
                
                public void SoundIt(List<List<IXyidiEvent>> xyidiEvents)
                {
                    MixingSampleProvider waveProvider;
                    foreach (List<IXyidiEvent> oneMomentEvent in xyidiEvents)
                    {
                        var lst = new List<ISampleProvider>();
                        foreach (IXyidiEvent xevent in oneMomentEvent)
                        {
                            var sine = new SignalGenerator()
                            {
                                Gain = xevent.Velocity,
                                Frequency = xevent.Note.Frequency,
                                Type = SignalGeneratorType.Sin
                            }.Take(TimeSpan.FromSeconds(xevent.Duration));
                            lst.Add(sine);
                            
                        }
                        waveProvider = new MixingSampleProvider(lst);
                        var wo = new WaveOutEvent();
                        
                            wo.Init( waveProvider);
                            wo.Play();
                            while (wo.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(500);
                            }
                    }
                }
            }

            private IGUI GUI;
            private IAudio Audio;
            private List<List<IXyidiEvent>> XyidiEvents = new List<List<IXyidiEvent>>();

            public Core()
            {
                GUI = new GUIOnWindowsForms();
                Audio = new AudioWithNAudio();


                Note n = new NoteLogTemper53(100);
                var xe = new XyidiEvent(new NoteLogTemper53(205), 100, 10,4);
                var xe2 = new XyidiEvent(new NoteLogTemper53(400), 100, 10, 2);
                List<IXyidiEvent> t = new List<IXyidiEvent>();
                t.Add(xe);
                //t.Add(xe2);
                //t.Add(xe3);
                XyidiEvents.Add(t);

                eSoundIt += Audio.SoundIt;

                eSoundIt(XyidiEvents);
            }
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            Core c = new Core();
        }
    }
}
