﻿using StutiBox.Models;

namespace StutiBox.Actors
{
    public interface IPlayerActor
    {
        ILibraryActor LibraryActor { get; }
		IBassActor BassActor { get; }
        PlaybackState PlaybackState { get; }
		LibraryItem CurrentLibraryItem { get; }
        bool Play(int identifier);
		bool Pause();
		bool Resume();
        bool Stop();
		bool Volume(byte volume);
		bool ToggleRepeat();
		bool Seek(double seconds);
		bool ConversationStarted();
		bool ConversationFinished();
    }
}