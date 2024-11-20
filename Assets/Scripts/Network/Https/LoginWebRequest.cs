using NMX.GameCore.Network.Client;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace NMX
{
    public class LoginWebRequest : HttpsRequestManager
    {

        public LoginData CsLoginData { get; private set; }

        public class LoginData
        {
            public string msg;
        }

        public class LoginModel
        {
            public string username;
            public string password;
        }

        public string MyToken { get; private set; } = string.Empty;


        public LoginWebRequest() {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("No internet connection.");
            }
        }

        public IEnumerator Login(string ua,string pw,IPEndPoint address)
        {

            var loginPayload = new LoginModel
            {
                username = ua,
                password = pw
            };

            var req = CreateWebRequest($"{SdkAddress}/login", loginPayload);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var decode = req.downloadHandler.text;

                Dictionary<string, string> headers = req.GetResponseHeaders();

                var jsonDocument = JsonUtility.FromJson<LoginData>(decode);

                try
                {
                    Debug.Log(jsonDocument.msg);

                    if (headers["Token"] != null)
                    {
                        Client.instance.ClientToken = headers["Token"];
                        Client.instance.Connect(address);
                    }
                } catch
                {

                }

            } else
            {
                Debug.LogError("Error Login Server unavailable");
            }

        }
    }
}
