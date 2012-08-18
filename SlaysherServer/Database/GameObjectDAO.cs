using System.Collections.Generic;
using Npgsql;
using SlaysherNetworking.Game.World.Objects;

namespace SlaysherServer.Database
{
    public class GameObjectDAO
    {
        private readonly NpgsqlConnection _db;
		private NpgsqlCommand _allGameObjectsCommand;

        internal GameObjectDAO(NpgsqlConnection db)
        {
            _db = db;
        }

        public List<GameObject> GetAllGameObjects()
        {
            if (_allGameObjectsCommand == null)
            {
                _allGameObjectsCommand = new NpgsqlCommand(
                    "SELECT id, posx, posy, posz, direction, model"
                    + " FROM gameobjects",
                    _db);
            }

            var reader = _allGameObjectsCommand.ExecuteReader();
            List<GameObject> gameObjects = new List<GameObject>();

            while (reader.Read())
            {
                GameObject obj = new GameObject
                    {
                        Id = (int) reader["id"],
                        PosX = (float) reader["posx"],
                        PosY = (float) reader["posy"],
                        PosZ = (float) reader["posz"],
                        Model = (string) reader["model"]
                    };
                gameObjects.Add(obj);
            }

            return gameObjects;
        }
    }
}