using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

namespace PieChart.ViitorCloud
{


    public class COVIDManager : MonoBehaviour
    {

        public GameObject prefab;
        public GameObject parentContent;

        public GameObject districtParentContent;
        JSONNode allData;

        public GameObject pieChartGO;
        public PieChart pieChart;

        // Start is called before the first frame update
        void Start()
        {
            // COVID data request
            StartCoroutine(GetData());
        }

        IEnumerator GetData()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://api.covid19india.org/v3/data-2020-07-11.json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Store data
                allData = JSON.Parse(www.downloadHandler.text);

                int totalConfirmed = allData["MH"]["total"]["confirmed"];

                foreach (KeyValuePair<string, JSONNode> data in allData)
                {
                    // Debug.Log(data.Key);
                    // Debug.Log(data.Value["total"]["confirmed"].Value);
                    GameObject instance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                    instance.transform.SetParent(parentContent.GetComponent<Transform>());

                    // Debug.Log(data.Key);
                    // Debug.Log(allData[data.Key]["total"]["confirmed"]);

                    Text stateCode = instance.transform.GetChild(0).gameObject.GetComponent<Text>();
                    stateCode.text = data.Key;

                    Text confirmedNumbers = instance.transform.GetChild(1).gameObject.GetComponent<Text>();
                    confirmedNumbers.text = allData[data.Key]["total"]["confirmed"];

                    instance.GetComponent<Button>().onClick.AddListener(() => districtButtonClicked(data.Key));
                }
            }
        }

        private void districtButtonClicked(string stateCode)
        {
            clearDistrictList();
            // Debug.Log(stateCode);
            foreach (KeyValuePair<string, JSONNode> data in allData[stateCode]["districts"])
            {
                GameObject instance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                instance.transform.SetParent(districtParentContent.GetComponent<Transform>());

                Text districtName = instance.transform.GetChild(0).gameObject.GetComponent<Text>();
                districtName.text = data.Key;

                Text confirmedNumbers = instance.transform.GetChild(1).gameObject.GetComponent<Text>();
                confirmedNumbers.text = data.Value["total"]["confirmed"];

                instance.GetComponent<Button>().onClick.AddListener(() => showPieChart(stateCode, data.Key));
            }
        }

        private void showPieChart(string stateCode, string districtName)
        {
            float confirmed = allData[stateCode]["districts"][districtName]["total"]["confirmed"];
            float recovered = allData[stateCode]["districts"][districtName]["total"]["recovered"];

            pieChart.segments = 2;
            pieChart.Data = new float[] { confirmed, recovered };
            pieChartGO.SetActive(true);
        }

        private void clearDistrictList()
        {
            Transform districtTransform = districtParentContent.GetComponent<Transform>();

            foreach (Transform district in districtTransform)
            {
                Destroy(district.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
