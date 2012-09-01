using System;
using System.Collections.Generic;
using System.Data.Common;

using SlaysherNetworking.Game.World.Objects;

namespace SlaysherServer.Database
{
    public class GameObjectDAO : IDisposable
    {
        private DAO dao;
        private DbConnection Db { get { return dao.DBConnection; } }
        private DbCommand AllGameObjectsCommand { get; set; }
        
        internal GameObjectDAO(DAO dao)
        {
            this.dao = dao;

            AllGameObjectsCommand = Db.CreateCommand();
            AllGameObjectsCommand.CommandText =
                "SELECT id, posx, posy, posz, direction, model"
                + " FROM gameobjects";
        }

        internal List<GameObject> GetAllGameObjects()
        {
            using (var reader = AllGameObjectsCommand.ExecuteReader())
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
            if (AllGameObjectsCommand != null)
            {
                AllGameObjectsCommand.Dispose();
            }
        }
    }
}