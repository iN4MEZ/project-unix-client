using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace NMX
{
    public abstract class HttpsRequestManager : MonoBehaviour
    {

        protected string SdkAddress = "http://49.228.131.138:7718";

        protected UnityWebRequest CreateWebRequest(string path,object data)
        {
            string jsonStringData = JsonUtility.ToJson(data);

            var req = new UnityWebRequest(path,"POST");

            if (data != null)
            {
                var bodyRaw = Encoding.UTF8.GetBytes(jsonStringData);

                req.uploadHandler = new UploadHandlerRaw(bodyRaw);

                req.downloadHandler = new DownloadHandlerBuffer();
            }
            req.SetRequestHeader("Content-Type", "application/json");

            return req;
        }

        protected UnityWebRequest CreateWebRequest(string path)
        {

            var req = new UnityWebRequest(path, "GET");

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            return req;
        }

    }
}
