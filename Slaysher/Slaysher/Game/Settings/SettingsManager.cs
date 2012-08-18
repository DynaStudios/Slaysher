using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Slaysher.Game.Settings.Options;

namespace Slaysher.Game.Settings
{
    public class SettingsManager
    {
        private string _applicationPath;

        public SettingsManager()
        {
            InitSettingsManager();
        }

        public bool IsFirstGameStart { get; set; }
        public GameSettings GameSettings { get; set; }

        private void InitSettingsManager()
        {
            _applicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                            "Slaysher");

            //Check if Folder already exists
            if (Directory.Exists(_applicationPath))
                LoadSettings();
            else
                CreateUserSettings();
        }

        private void LoadSettings()
        {
            Debug.WriteLine("Settings Folder were found. Load Settings");

            Stream stream = File.OpenRead(Path.Combine(_applicationPath, "game.xml"));
            var serializer = new XmlSerializer(typeof (GameSettings));
            GameSettings = (GameSettings) serializer.Deserialize(stream);
            stream.Close();

            Debug.WriteLine("Loaded all Settings");
        }

        private void CreateUserSettings()
        {
            Debug.WriteLine("Settings Folder were NOT found. Create blank settings at {0}", _applicationPath);
            IsFirstGameStart = true;

            //Create Settings Folder
            Directory.CreateDirectory(_applicationPath);

            //Create Game Settings
            GameSettings = new GameSettings();

            ForceSave();
        }

        /// <summary>
        /// Instantly saves ALL Settings Files
        /// </summary>
        public void ForceSave()
        {
            Stream stream = File.OpenWrite(Path.Combine(_applicationPath, "game.xml"));

            var serializer = new XmlSerializer(typeof (GameSettings));
            serializer.Serialize(stream, GameSettings);

            stream.Close();
        }

        public void Reload(string settingsName)
        {
            throw new NotImplementedException();
        }
    }
}