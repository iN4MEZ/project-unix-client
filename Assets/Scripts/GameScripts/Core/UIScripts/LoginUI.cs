using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace NMX
{
    public class LoginUI : MonoBehaviour
    {
        public string UsernameField => usernameInputField.text;
        public string PasswordField => passwordInputField.text;

        public InputField usernameInputField;
        public InputField passwordInputField;

        public Text GameVersionTxt;

        public Dropdown GatewayDropdown;
        public Dropdown DevAccountDropdown;

        public Button LoginBtn;

        public Canvas LoginPanelCanvas;

        public LoginWebRequest LoginWebRequest { get; private set; }

        private GameObject uiLoadingGate;

        private Canvas cvLoad;

        private GamePatcher patcher;

        private QueryGateway gateway;

        private IPEndPoint targetAddress;

        private List<AccountModel> accountsForDev;

        public static LoginUI Instance {  get; private set; }


        private class AccountModel
        {
            public string username;
            public string password;
        }


        private void Awake()
        {
            Instance = this;

            patcher = new GamePatcher();
            LoginWebRequest = new LoginWebRequest();
            gateway = new QueryGateway();

            accountsForDev = new List<AccountModel>();

            accountsForDev.Add(new AccountModel { username = "test", password = "test" });
            accountsForDev.Add(new AccountModel { username = "test2", password = "test2" });

            GatewayDropdown.ClearOptions();
            DevAccountDropdown.ClearOptions();
            LoginBtn.onClick.AddListener(OnClickEnterGame);
        }

        private void Start()
        {
            uiLoadingGate = GameObject.FindGameObjectWithTag("LoadingUI");

            cvLoad = uiLoadingGate.GetComponentInChildren<Canvas>();
            cvLoad.enabled = false;

            DontDestroyOnLoad(LoginPanelCanvas.gameObject);

            StartCoroutine(patcher.Analysis());
            StartCoroutine(gateway.GetServer());

            LoginUI.Instance.DevAccountDropdown.options.Add(new Dropdown.OptionData() { text = "None" });

            foreach (var account in accountsForDev)
            {

                LoginUI.Instance.DevAccountDropdown.options.Add(new Dropdown.OptionData() { text = account.username });
            }

            UpdateDropdownDevAccount();
        }

        public void UpdateDropdownGateway()
        {
            GatewayDropdown.captionText.text = GatewayDropdown.options[1].text;

            targetAddress = new IPEndPoint(IPAddress.Parse(gateway.gatewayModelJson[1].Address), gateway.gatewayModelJson[1].Port);

            GatewayDropdown.onValueChanged.AddListener(delegate { GatewayDropDownEvent(GatewayDropdown); });
        }

        public void UpdateDropdownDevAccount()
        {
            DevAccountDropdown.captionText.text = DevAccountDropdown.options[0].text;

            DevAccountDropdown.onValueChanged.AddListener(delegate { DevAccountDropDownEvent(DevAccountDropdown); });
        }


        public void OnClickEnterGame()
        {
            StartCoroutine(LoginWebRequest.Login(UsernameField,PasswordField,targetAddress));

        }

        public void ActiveLoginPanel(bool ac)
        {
            LoginPanelCanvas.enabled = ac;
        }

        private void GatewayDropDownEvent(Dropdown dropdown)
        {
            int index = dropdown.value;
            targetAddress = new IPEndPoint(IPAddress.Parse(gateway.gatewayModelJson[index].Address), gateway.gatewayModelJson[index].Port);
        }

        private void DevAccountDropDownEvent(Dropdown dropdown)
        {
            int index = dropdown.value-1;

            Debug.Log(accountsForDev[index].username);

            StartCoroutine(LoginWebRequest.Login(accountsForDev[index].username, accountsForDev[index].password, targetAddress));
        }

    }
}
