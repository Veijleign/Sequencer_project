using System;
namespace BshInterface
{
    p
    protected interface IXyidiEvent
    {
        public Note Note { get; set; }
        public int Velocity { get; set; }
        public float StartTime { get; set; }
        public float Duration { get; set; }

    }
    protected interface IWorkWithXyidi
    {
        public void AddXyidiEvent(float startTime, float playTime, int noteNumber);
        public void RemoveXyidiEvent(float startTime, float playTime, int noteNumber);
        //public 
    }
    protected interface IGUI
    {
        public void Start();
        public void PrintError(string ErrorMessage);
    }
    protected interface IAudio
    {
        public void SoundIt(List<List<IXyidiEvent>> xyidiEvents);
    }
}