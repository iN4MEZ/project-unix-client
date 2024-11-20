using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace NMX
{
    public class QueryGateway : HttpsRequestManager
    {
        public List<GatewayModel> gatewayModelJson;

        public IEnumerator GetServer()
        {
            var req = CreateWebRequest($"{SdkAddress}/query_gateway");

            yield return req.SendWebRequest();

            if ( req.result == UnityEngine.Networking.UnityWebRequest.Result.Success ) {

                var decode = req.downloadHandler.text;

                gatewayModelJson = JsonConvert.DeserializeObject<List<GatewayModel>>(decode);

                foreach (var gatewayModel in gatewayModelJson)
                {
                    LoginUI.Instance.GatewayDropdown.options.Add(new Dropdown.OptionData() { text = gatewayModel.RegionName });
                }

                LoginUI.Instance.UpdateDropdownGateway();
            }
        }

    }
}
