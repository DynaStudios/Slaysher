using System.Collections.Generic;

using MySql.Data.MySqlClient;

using SlaysherNetworking.Game.World.Objects;


namespace SlaysherServer.Database
{
    public class GameObjectDAO
    {
        private MySqlConnection _db;
        private MySqlCommand allGameObjectsCommand = null;

        internal GameObjectDAO(MySqlConnection db)
        {
            _db = db;
        }

        public List<GameObject> getAllGameObjects()
        {
            if (allGameObjectsCommand == null)
            {
                allGameObjectsCommand = new MySqlCommand(
                        "SELECT id, posx, posy, posz, direction, model"
                        + " FROM gameobjects",
                        _db);
            }

            MySqlDataReader reader = allGameObjectsCommand.ExecuteReader();
            List<GameObject> gameObjects = new List<GameObject>();

            while (reader.Read())
            {
                GameObject obj = new GameObject();
                obj.Id = (int)reader["id"];
                obj.PosX = (float)reader["posx"];
                obj.PosY = (float)reader["posy"];
                obj.PosZ = (float)reader["posz"];
                obj.Model = (string)reader["model"];
                gameObjects.Add(obj);
            }

            return gameObjects;
        }
    }
}
