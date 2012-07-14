using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Slaysher.Game.Database
{
    public class Database
    {
        public Database()
        {
        }

        public Dictionary<int, string> ReadAvailablePatternTextures()
        {
            var resultDic = new Dictionary<int, string>();

            var textures = XElement.Load(@"Content\Data\patternTextures.xml");
            foreach (var texture in textures.Elements())
            {
                resultDic.Add(int.Parse(texture.Element("ID").Value), texture.Element("Path").Value);
            }

            return resultDic;
        }
    }
}