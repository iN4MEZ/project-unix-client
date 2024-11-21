
using System;
using System.Data; // สำหรับฐานข้อมูล
using Mono.Data.Sqlite; // ไลบรารี SQLite
using UnityEngine;

namespace NMX
{
    public class LocalStorageManager : MonoBehaviour
    {
        private string dbPath;

        public static LocalStorageManager Instance { get; private set; }

        public void Start()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void Initialize()
        {
            // กำหนดเส้นทางไปยังไฟล์ .db ใน StreamingAssets
            dbPath = $"URI=file:{Application.dataPath}/ResourceData/data.db";
        }
        void LoadData()
        {
            // เปิดการเชื่อมต่อ
            using (IDbConnection dbConnection = new SqliteConnection(dbPath))
            {
                dbConnection.Open();

                // สร้างคำสั่ง SQL
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM AvatarData";

                // อ่านข้อมูล
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string resourcePath = reader.GetString(1);

                        Debug.Log($"ID: {id}, Path: {resourcePath}");
                    }
                }
            }
        }

        public AvatarData GetAvatarData()
        {
            try
            {
                // เปิดการเชื่อมต่อ
                using (IDbConnection dbConnection = new SqliteConnection(dbPath))
                {
                    dbConnection.Open();

                    // สร้างคำสั่ง SQL
                    IDbCommand dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = "SELECT * FROM AvatarData";

                    // อ่านข้อมูล
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string resourcePath = reader.GetString(1);

                            return new AvatarData { AvatarID = (uint)id, ResourcePath = resourcePath };
                        }
                    }
                }
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }

        public AvatarData GetAvatarDataById(uint tid)
        {
            try
            {
                // เปิดการเชื่อมต่อ
                using (IDbConnection dbConnection = new SqliteConnection(dbPath))
                {
                    dbConnection.Open();

                    // สร้างคำสั่ง SQL
                    IDbCommand dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = $"SELECT * FROM AvatarData WHERE AvatarID = {(int)tid};";

                    // อ่านข้อมูล
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string resourcePath = reader.GetString(1);

                            return new AvatarData { AvatarID = (uint)id, ResourcePath = resourcePath };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }
    }
}
