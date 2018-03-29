﻿namespace StutiBox.Actors
{
    public interface IPlayerActor
    {
        IConfigurationActor ConfigurationActor
        { get; }
        ILibraryActor LibraryActor { get; }
        PlaybackState PlaybackState { get; }
        int Stream { get; }
        bool Play(int identifier);
		bool Pause();
		bool Resume();
        bool Stop();
		bool Volume(byte volume);
    }
}