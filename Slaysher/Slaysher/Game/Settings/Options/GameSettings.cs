using System;

namespace Slaysher.Game.Settings.Options
{
    [Serializable]
    public class GameSettings
    {
        public string ServerAdress = "slaysher.dyna-studios.com";

        public bool Fullscreen = false;
        public int ScreenWidth = 1024;
        public int ScreenHeight = 768;

        public bool SoundEnabled = true;
        public int MasterVolume = 100;
        public int MusicVolume = 100;
        public int SfxVolume = 100;
    }
}