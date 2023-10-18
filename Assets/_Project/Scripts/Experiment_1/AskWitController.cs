using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using _Project.Scripts.Experiment_1.Data;
using DG.Tweening;
using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

namespace _Project.Scripts.Experiment_1
{

    public class AskWitController : MonoBehaviour
    {
        public SpriteRenderer micOnIndicator;
        public GameObject transcriptionParent;
        public TextMeshProUGUI micTranscription;
        public GameObject answerParent;
        public TextMeshProUGUI answerText;
        public Transform gptanswerParent;
        public TextMeshProUGUI gptanswerText;

        public GameObject venus;
        public GameObject mars;
        public GameObject earth;
        public GameObject mercury;


        private InteractableUnityEventWrapper _button;
        private CommunicationData _response;
        private Wit _wit;
        private string _text = "how may people in Texas";
        private string _entityValue;
        private bool IsMovetoTarget;
        public static Action<string> OnMovedPlanet;
        public static Action<string> OnSendingWebRequest;

        // ReSharper disable once UnusedMember.Global
        public void Construct(Wit wit, InteractableUnityEventWrapper button, CommunicationData response)
        {
            _wit = wit;
            _button = button;
            _response = response;
        }

        private void OnEnable()
        {
            SetIndicatorEnable(false);
            _wit.TranscriptionEvents.OnPartialTranscription.AddListener(OnTranscription);
            _wit.AudioEvents.OnMicStartedListening.AddListener(OnMicStartedListening);
            _wit.AudioEvents.OnMicStoppedListening.AddListener(OnMicStoppedListening);
            _wit.VoiceEvents.OnResponse.AddListener(OnResponse);
            HoloScreen.OnOpenScreen += () => { return _text; };
            _button.WhenSelect.AddListener(OnActivatePressed);
            Debug.Log("AskWitController OK");
        }

        private void OnDisable()
        {
            HoloScreen.OnOpenScreen -= () => { return _text; };
            _wit.TranscriptionEvents.OnPartialTranscription.RemoveListener(OnTranscription);
            _wit.AudioEvents.OnMicStartedListening.RemoveListener(OnMicStartedListening);
            _wit.AudioEvents.OnMicStoppedListening.RemoveListener(OnMicStoppedListening);
            _wit.VoiceEvents.OnResponse.RemoveListener(OnResponse);
        }

        //Only for debug

        private void Update()
        {
            //if (IsMovetoTarget)
            //{
            //    float step = speed * Time.deltaTime;
            //    transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            //}

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _wit.Activate();
            }
        }

        private void OnActivatePressed()
        {

            _wit.Activate();
        }

        private void OnMicStoppedListening()
        {

            SetIndicatorEnable(false);
        }

        private void OnMicStartedListening()
        {

            micTranscription.SetText("...");
            answerParent.SetActive(false);
            transcriptionParent.SetActive(true);
            transcriptionParent.transform.DOScaleY(0, 0.3f).From();
            SetIndicatorEnable(true);
        }

        // {
        //     "entities":{
        //         "planets_count:planets_count":[
        //         {
        //             "body":"How many planets in solar system",
        //             "confidence":"1",
        //             "end":"32",
        //             "entities":{
        //        
        //             },
        //             "id":"535134241814316",
        //             "name":"planets_count",
        //             "role":"planets_count",
        //             "start":"0",
        //             "type":"value",
        //             "value":"8"
        //         }
        //         ]
        //     },
        //     "intents":[
        //     {
        //         "confidence":"0.9973112117256707",
        //         "id":"1460115007732233",
        //         "name":"about_solar_system"
        //     }
        //     ],
        //     "is_final":"true",
        //     "speech":{
        //         "confidence":"0.8596",
        //         "tokens":[
        //         {
        //             "end":"900",
        //             "start":"0",
        //             "token":"How"
        //         },
        //         {
        //             "end":"1140",
        //             "start":"900",
        //             "token":"many"
        //         },
        //         {
        //             "end":"1560",
        //             "start":"1140",
        //             "token":"planets"
        //         },
        //         {
        //             "end":"1800",
        //             "start":"1560",
        //             "token":"in"
        //         },
        //         {
        //             "end":"2040",
        //             "start":"1800",
        //             "token":"solar"
        //         },
        //         {
        //             "end":"2340",
        //             "start":"2040",
        //             "token":"system"
        //         }
        //         ]
        //     },
        //     "text":"How many planets in solar system",
        //     "traits":{
        //
        //     }
        // }
        public class Req
        {
            public string sender;
            public string message;
            public string type;
            public string template;
            public string key = "6Gb9tb29HC";
            public Req(string send, string msg, string tp, string temp = null )
            {
                this.sender = send;
                this.message = msg;
                this.type = tp;
                this.template = temp;
            }
        }
        [Serializable]
        public class BotResponse
        {
            public string recipient_id;
            public string text;

            public BotResponse(string send, string msg)
            {
                this.recipient_id = send;
                this.text = msg;

            }
        }
        [ContextMenu("ChangeAllVisibility")]
        public void ChangeAllVisibility(string active)
        {


            if (venus.activeInHierarchy) venus.SetActive(false);
            if (mars.activeInHierarchy) mars.SetActive(false);
            if (earth.activeInHierarchy) earth.SetActive(false);
            if (mercury.activeInHierarchy) mercury.SetActive(false);
            Debug.Log($"switch {active}");
            switch (active)
            {
                case "Venus": venus.SetActive(true); break;
                case "Mars": mars.SetActive(true); break;
                case "Earth": earth.SetActive(true); break;
                case "Mercury": mercury.SetActive(true); break;
            }
        }

        [ContextMenu("WebRequest")]
        private IEnumerator WebRequest(string text,string entityValue)
        {

            Req req = new Req(GameManager.sessionId.ToString(), text, "gpt", "cosmoDesc");
            string uri = "https://europe-central2-devtorium-qna.cloudfunctions.net/aihub";
            var bodyJsonString = JsonUtility.ToJson(req);
            using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                //request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                    _response.Value = request.error;
                }
                else
                {
                    var responses = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary>(request.downloadHandler.text);
                    Debug.Log($"Form upload complete! {request.downloadHandler.text}");
                    answerParent.SetActive(true);
                    answerParent.transform.DOScaleY(0, 0.5f).From();
                    answerText.SetText(entityValue + $"\n{ responses["message"]}");
                    _response.Value = entityValue + $"\n{ responses["message"]}";
                    //gptanswerText.SetText($"\n{responses["message"]}");

                    //gptanswerParent.DOScaleX(1, 0.4f).SetEase(Ease.InOutExpo)
                    //        .OnComplete(() => { gptanswerText.DOText(request.downloadHandler.text, 2f); });

                }

            }
        }
        private IEnumerator GptWebRequest(string text)
        {

            Req req = new Req(GameManager.instance.ToString(), text,"gpt", "cosmoDesc");
            string uri = "https://europe-central2-devtorium-qna.cloudfunctions.net/aihub";
            var bodyJsonString = JsonUtility.ToJson(req);
            using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                //request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"error" + request.error);
                }
                else
                {
                    var responses = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary>(request.downloadHandler.text);
                    Debug.Log($"Form upload complete! {request.downloadHandler.text}");

                    gptanswerText.SetText($"\n{responses["message"]}");

                    gptanswerParent.DOScaleX(1, 0.4f).SetEase(Ease.InOutExpo)
                            .OnComplete(() => { gptanswerText.DOText(request.downloadHandler.text, 2f); });

                }

            }
        }
        //        [
        //	        {
        //		        "recipient_id": "test_user",
        //		          "text": "Mars:\nOrbit: Sun \nNumber in orbit:IV,\nRadius: 227940000km \nPeriod: 686.98 days \nIncl:1.85\nEccen:0.09\nDiscoverer:-\nDate: - \nPseudonym:0\nOn orbit Mars: I-Phobos, II-Deimos,"

        //          }
        //       ]
        private void OnResponse(WitResponseNode node)
        {


            string res = node["entities"]?[0]?[0]?["value"];
            _entityValue = res.Trim(new Char[] { ' ', '*', '"' });
            Debug.Log($"Update {_entityValue}");
            ChangeAllVisibility(_entityValue);
            OnMovedPlanet?.Invoke(_entityValue);
            
            _text = node["text"];
            OnSendingWebRequest?.Invoke(_text);
            IEnumerator coroutine = WebRequest(_text, _entityValue);
            StartCoroutine(coroutine);
            //IEnumerator gptcoroutine = GptWebRequest(_text);
            //StartCoroutine(gptcoroutine);

        }

        private void OnTranscription(string arg0)
        {
            micTranscription.SetText(arg0);
            //voice trascription here
        }
        private void SetIndicatorEnable(bool isOn)
        {
            micOnIndicator.transform.DOPunchScale(Vector3.one * 0.01f, 0.2f, 2, 0.2f).SetRelative();
            micOnIndicator.color = isOn ? Color.red : Color.gray;
        }
    }
}

