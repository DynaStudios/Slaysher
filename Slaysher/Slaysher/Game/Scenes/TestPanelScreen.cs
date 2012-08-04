using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Models;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.Scenes
{
    public class TestPanelScreen : WindowScreen
    {
        #region Overrides of WindowScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            //Init DropDown
            DropDownItem item1 = new DropDownItem(1, "Test1");
            DropDownItem item2 = new DropDownItem(2, "Test2");
            DropDownItem item3 = new DropDownItem(3, "Test3");
            List<DropDownItem> dropDownItems = new List<DropDownItem> {item1, item2, item3};

            DropDown dropDown = new DropDown(dropDownItems, item2);
            dropDown.SelectionChanged += DropDownItemChanged;

            PanelEntries.Add(new Label("Hallo Welt") { Color = Color.OrangeRed });
            PanelEntries.Add(dropDown);
            PanelEntries.Add(new Button("Test Button 1"));
            PanelEntries.Add(new Button("Test Button 2"));
            PanelEntries.Add(new Button("Test Button 3"));

            Vector2 itemSize = UpdateSubItemPositions();
            PresentationOffset = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2 - itemSize.X/2,
                                             ScreenManager.GraphicsDevice.Viewport.Height/2 - itemSize.Y/2);
        }

        #endregion

        private void DropDownItemChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Item Changed");
        }

    }
}