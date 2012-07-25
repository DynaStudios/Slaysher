using System.Collections.Generic;
using System.Xml.Linq;

namespace Slaysher.Game.Database
{
    public class Database
    {
        public Dictionary<int, string> ReadAvailablePatternTextures()
        {
            var resultDic = new Dictionary<int, string>();

            XElement textures = XElement.Load(@"Content\Data\patternTextures.xml");
            foreach (XElement texture in textures.Elements())
            {
                resultDic.Add(int.Parse(texture.Element("ID").Value), texture.Element("Path").Value);
            }

            return resultDic;
        }
    }
}