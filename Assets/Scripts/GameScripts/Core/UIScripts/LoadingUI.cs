using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NMX
{
    public class LoadingUI : MonoBehaviour
    {

        public Slider LoadingProcessSlider {  get; private set; }

        public static LoadingUI Instance { get; private set; }

        private void Start()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }


        public void InitLoadingUIData()
        {
            Canvas canvasLoadingUi = gameObject.GetComponentInChildren<Canvas>();

            foreach (Transform t in canvasLoadingUi.gameObject.transform)
            {
                if (t.gameObject.GetComponent<Slider>() != null)
                {
                    LoadingProcessSlider = t.gameObject.GetComponent<Slider>();
                }
            }
        }
    }
}
