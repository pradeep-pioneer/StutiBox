using System;
namespace StutiBox.Models
{
	public class BassEventArgs:EventArgs
    {
		public BassEvent Event { get; private set; }
		public BassEventArgs(BassEvent bassEvent) => Event = bassEvent;
    }

    public enum BassEvent
	{
		PlaybackRestarting = 0,
        PlaybackFinished = 1
	}
}
