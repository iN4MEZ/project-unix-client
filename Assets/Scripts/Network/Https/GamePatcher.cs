using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace NMX
{
    public class GamePatcher : HttpsRequestManager
    {
        //private string serverUrl = $"{SdkAddress}/Patch/"; // URL ของ resource บน server
        private string localFilePath;
        private string localVersionFilePath;
        private string folderName;


        private void Awake()
        {
        }

        private void Start()
        {

        }

        public void UpdateGame()
        {

        }

        public IEnumerator Analysis()
        {
            folderName = "/ResourceData";

            string fullPath = Application.dataPath + folderName;

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            localFilePath = Path.Combine(fullPath, "data.db");
            localVersionFilePath = Path.Combine(fullPath, "resource_version.txt");

            // Debug! 
            LoginUI.Instance.ActiveLoginPanel(false);

            Debug.Log("Analysis Game Data ...");

            // ตรวจสอบเวอร์ชันของ resource จาก server
            UnityWebRequest versionRequest = UnityWebRequest.Get($"{SdkAddress}/Patch/" + "datav");
            yield return versionRequest.SendWebRequest();

            if (versionRequest.result == UnityWebRequest.Result.ConnectionError || versionRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(versionRequest.error);
                yield break;
            }

            string serverVersion = versionRequest.downloadHandler.text;

            // ตรวจสอบเวอร์ชันใน local
            string localVersion = "";
            if (File.Exists(localVersionFilePath))
            {
                localVersion = File.ReadAllText(localVersionFilePath);
            }

            // ถ้าเวอร์ชันไม่ตรงกัน ให้ดาวน์โหลด resource ใหม่
            if (serverVersion != localVersion)
            {
                Debug.Log("Loading new update ...");
                UnityWebRequest resourceRequest = UnityWebRequest.Get($"{SdkAddress}/Patch/" + serverVersion);
                yield return resourceRequest.SendWebRequest();

                if (resourceRequest.result == UnityWebRequest.Result.ConnectionError || resourceRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(resourceRequest.error);
                    yield break;
                }

                // บันทึก resource ใหม่ลงในไฟล์
                File.WriteAllBytes(localFilePath, resourceRequest.downloadHandler.data);
                File.WriteAllText(localVersionFilePath, serverVersion);

                Debug.Log("Resource updated to version: " + serverVersion);
            }
            else
            {
                Debug.Log("Resource is up to date");
            }

            LoginUI.Instance.ActiveLoginPanel(true);
            LoginUI.Instance.GameVersionTxt.text = "Version " + File.ReadAllText(localVersionFilePath) ;

            LocalStorageManager.Instance.Initialize();


            // โหลด resource เพื่อใช้ในเกม
            //LoadResource();
        }
    }
}
