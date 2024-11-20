
using System.Data; // สำหรับฐานข้อมูล
using Mono.Data.Sqlite; // ไลบรารี SQLite
using UnityEngine;

namespace NMX
{
    public class LocalStorageManager : MonoBehaviour
    {
        private string dbPath;

        void Start()
        {
            // กำหนดเส้นทางไปยังไฟล์ .db ใน StreamingAssets
            dbPath = $"URI=file:{Application.streamingAssetsPath}/data.db";

            LoadData();
        }

        void LoadData()
        {
            // เปิดการเชื่อมต่อ
            using (IDbConnection dbConnection = new SqliteConnection(dbPath))
            {
                dbConnection.Open();

                // สร้างคำสั่ง SQL
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Resources";

                // อ่านข้อมูล
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string resourcePath = reader.GetString(1);
                        float damageAmount = reader.GetFloat(2);

                        Debug.Log($"ID: {id}, Path: {resourcePath}, Damage: {damageAmount}");
                    }
                }
            }
        }
    }
}
