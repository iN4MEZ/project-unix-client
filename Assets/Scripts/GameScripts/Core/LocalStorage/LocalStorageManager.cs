
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

        public AvatarData GetAvatarDataById(uint tid)
        {
            try
            {
                string query = $"SELECT * FROM AvatarData WHERE AvatarID = {tid};";
                return ExecuteQuery(dbPath, query, reader =>
                {
                    int id = reader.GetInt32(0);
                    string resourcePath = reader.GetString(1);

                    return new AvatarData
                    {
                        AvatarID = (uint)id,
                        ResourcePath = resourcePath
                    };
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }

        public MonsterData GetMonsterDataById(uint tid)
        {
            try
            {
                string query = $"SELECT * FROM MonsterData WHERE MonsterID = {tid};";
                return ExecuteQuery(dbPath, query, reader =>
                {
                    int id = reader.GetInt32(0);
                    string resourcePath = reader.GetString(1);

                    return new MonsterData
                    {
                        MonsterID = (uint)id,
                        ResourcePath = resourcePath
                    };
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }

        public static T ExecuteQuery<T>(string dbPath, string query, Func<IDataReader, T> handleData)
        {
            // เปิดการเชื่อมต่อ
            using (IDbConnection dbConnection = new SqliteConnection(dbPath))
            {
                dbConnection.Open();

                // สร้างคำสั่ง SQL
                using (IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = query;

                    // อ่านข้อมูล
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // ใช้ delegate handleData ในการจัดการผลลัพธ์
                            return handleData(reader);
                        }
                    }
                }
            }
            return default;
        }

    }
}
