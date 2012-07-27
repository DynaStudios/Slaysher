using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Slaysher.Game.IO;
using Slaysher.Game.Scenes;

namespace Slaysher.Game.GUI
{
    public class GUIManager : IScene
    {
        public string Name
        {
            get { return "GUIManager"; }
        }

        public Engine Engine { get; set; }

        public GUIManager(Engine engine)
        {
            Engine = engine;
        }

        public void LoadScene()
        {
            //Load Mouse Cursor and Menu Graphics
            Cursor myCursor = LoadCustomCursor(Application.StartupPath + @"\Content\Images\Game\Arrow.cur");
            Form winForm = (Form) Control.FromHandle(Engine.Window.Handle);
            winForm.Cursor = myCursor;

            //Make Mouse visible
            Engine.IsMouseVisible = true;

            //Engine.Keyboard.KeyUp += keyboard_KeyboardKeyUp;
        }

        public void Render(Microsoft.Xna.Framework.GameTime time)
        {
            if (Engine.GameState == GameState.Menu)
            {
                //Draw Menu
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
            //Calculate Cursor Stuff here
        }

        public void UnloadScene()
        {
            //Not necassery since GUIManager will always run
        }

        public void KeyboardKeyboardKeyUp(object sender, EventArgs eventArgs)
        {
            KeyboardEventArgs eventA = (KeyboardEventArgs) eventArgs;
        }

        //WinAPI Calls here
        public static Cursor LoadCustomCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            //if (hCurs == IntPtr.Zero) throw new Win32Exception();
            var curs = new Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof (Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null) fi.SetValue(curs, true);
            return curs;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);
    }
}