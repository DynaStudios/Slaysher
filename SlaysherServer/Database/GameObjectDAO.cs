using System;
using System.Collections.Generic;

using Npgsql;

using SlaysherNetworking.Game.World.Objects;

namespace SlaysherServer.Database
{
    public class GameObjectDAO : IDisposable
    {
        private DAO dao;
        private NpgsqlConnection Db { get { return dao.DBConnection; } }
        private NpgsqlCommand _allGameObjectsCommand;
        private NpgsqlCommand AllGameObjectsCommand
        {
            get
            {
                if (_allGameObjectsCommand == null)
                {
                    _allGameObjectsCommand = new NpgsqlCommand(
                        "SELECT id, posx, posy, posz, direction, model"
                        + " FROM gameobjects",
                        Db);
                }
                return _allGameObjectsCommand;
            }
        }
        
        internal GameObjectDAO(DAO dao)
        {
            this.dao = dao;
        }

        internal List<GameObject> GetAllGameObjects()
        {
            using (var reader = _allGameObjectsCommand.ExecuteReader())
            {

                List<GameObject> gameObjects = new List<GameObject>();

                while (reader.Read())
                {
                    GameObject obj = new GameObject
                    {
                        Id = (int)reader["id"],
                        PosX = (float)reader["posx"],
                        PosY = (float)reader["posy"],
                        PosZ = (float)reader["posz"],
                        Model = (string)reader["model"]
                    };
                    gameObjects.Add(obj);

                }
                return gameObjects;
            }
        }

        public void Dispose()
        {
            if (_allGameObjectsCommand != null)
            {
                _allGameObjectsCommand.Dispose();
            }
        }
    }
}