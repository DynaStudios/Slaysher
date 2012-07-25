using System.Collections.Generic;

using MySql.Data.MySqlClient;
using SlaysherNetworking.Game.World.Objects;

namespace SlaysherServer.Database
{
    public class GameObjectDAO
    {
        private readonly MySqlConnection _db;
        private MySqlCommand _allGameObjectsCommand;

        internal GameObjectDAO(MySqlConnection db)
        {
            _db = db;
        }

        public List<GameObject> GetAllGameObjects()
        {
            if (_allGameObjectsCommand == null)
            {
                _allGameObjectsCommand = new MySqlCommand(
                        "SELECT id, posx, posy, posz, direction, model"
                        + " FROM gameobjects",
                        _db);
            }

            MySqlDataReader reader = _allGameObjectsCommand.ExecuteReader();
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