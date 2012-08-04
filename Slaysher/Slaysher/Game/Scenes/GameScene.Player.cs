using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Slaysher.Game.Entities;
using SlaysherNetworking.Game.World;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SlaysherNetworking.Game.Entities;

namespace Slaysher.Game.Scenes
{
    public partial class GameScene
    {
        private Model _playerModel;

        private void TickPlayer(GameTime time)
        {
            Player.ExecuteMovement(time.ElapsedGameTime);
            //TODO: Update stuff like position etc. here
        }
    }
}