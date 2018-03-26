using System;
using Un4seen.Bass;

namespace StutiBox.Actors
{
    public class PlayerActor : IPlayerActor
    {
        public IConfigurationActor ConfigurationActor { get; private set; }
        public ILibraryActor LibraryActor { get; private set;}
        public PlaybackState PlaybackState { get; private set; }
        public int Stream { get; private set; }

        public PlayerActor(IConfigurationActor configurationActor, ILibraryActor libraryActor)
        {
            ConfigurationActor = configurationActor;
            LibraryActor = libraryActor;
            PlaybackState = PlaybackState.NotInitialized;
            Stream = 0;
            setupPlayBackEngine();
        }

        public bool Play(int identifier)
        {
            if (PlaybackState == PlaybackState.Playing)
                throw new NotSupportedException("Playback in progress, stop first");
            if (PlaybackState == PlaybackState.Paused)
                throw new NotSupportedException("Playback paused, either stop or use Resume");
            if (PlaybackState == PlaybackState.Faulted || PlaybackState == PlaybackState.NotInitialized)
                throw new NotSupportedException($"Playback engine not available: {PlaybackState}");
            var file = LibraryActor.GetItem(identifier);
            if (file == null)
                throw new ArgumentException("Identifier not valid");
            Stream = Bass.BASS_StreamCreateFile(file.FullPath, 0, 0, BASSFlag.BASS_DEFAULT);
            if (Stream != 0)
            {
                var playBackStartSyncProc = new SYNCPROC(playBackStartCallback);
                var playBackEndSyncProc = new SYNCPROC(playBackEndCallBack);
                var playBackPausedSyncProc = new SYNCPROC(playBackPausedCallBack);
                Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, 0, playBackStartSyncProc, IntPtr.Zero);
                Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, 0, playBackEndSyncProc, IntPtr.Zero);
                Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_STALL | BASSSync.BASS_SYNC_MIXTIME, 0, playBackEndSyncProc, IntPtr.Zero);
                Bass.BASS_ChannelPlay(Stream, false);
            }
            else
                return false;
            return true;
        }

        public bool Stop()
        {
            if (PlaybackState != PlaybackState.Playing || PlaybackState == PlaybackState.Paused)
                throw new NotSupportedException("Cannot stop playback");
            bool result;
            if (Stream != 0)
            {
                result = Bass.BASS_ChannelStop(Stream);
                result = Bass.BASS_StreamFree(Stream);
                PlaybackState = PlaybackState.Stopped;
            }
            else
                result = false;
            Stream = 0;
            return result;
        }

        private void playBackPausedCallBack(int handle, int channel, int data, IntPtr user)
        {
            PlaybackState = data == 0 ? PlaybackState.Paused : PlaybackState.Playing;
        }

        private void playBackStartCallback(int handle, int channel, int data, IntPtr user)
        {
            PlaybackState = PlaybackState.Playing;
        }

        private void playBackEndCallBack(int handle, int channel, int data, IntPtr user)
        {
            PlaybackState = PlaybackState.Stopped;
        }

        private void setupPlayBackEngine()
        {
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                PlaybackState = PlaybackState.Faulted;
                throw new NotSupportedException("Initialization of Playback Engine Failed");
            }
            PlaybackState = PlaybackState.Stopped;
        }
    }

    public enum PlaybackState
    {
        NotInitialized = -1,
        Faulted = 0,
        Stopped = 1,
        Playing = 2,
        Paused = 3
    }
}
