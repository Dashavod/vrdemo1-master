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
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

namespace _Project.Scripts.Experiment_1
{
    public class HoloScreen : MonoBehaviour
    {
        public Transform screenModel;
        public TextMeshProUGUI welcomeText;
        public Image image;
        public InteractableUnityEventWrapper showButton;
        public InteractableUnityEventWrapper hideButton;
        public static Func<string> OnOpenScreen;

        private bool _isActivated;

        private void OnEnable()
        {
            AskWitController.OnSendingWebRequest += Web;
        }

        private void OnDisable()
        {
            AskWitController.OnSendingWebRequest -= Web;
        }
        private void Web(string text)
        {
            IEnumerator coroutine = WebRequest("Solar System, HD quality, realism" + text);
            Debug.Log($"Web gpt");
            StartCoroutine(coroutine);

        }
        private void Start()
        {
            screenModel.DOScaleX(0, 0);
            showButton.WhenSelect.AddListener(() =>
            {
                IEnumerator coroutine = WebRequest(OnOpenScreen?.Invoke());
                StartCoroutine(coroutine);


            });
            hideButton.WhenSelect.AddListener(() =>
            {
                screenModel.DOScaleX(0, 0.4f).SetEase(Ease.InOutExpo);
            });
        }
        private IEnumerator WebRequest(string text)
        {


            Req req = new Req(GameManager.sessionId.ToString(), text,"stable_diffusion");
            string uri = "https://europe-central2-devtorium-qna.cloudfunctions.net/aihub";
            var bodyJsonString = JsonUtility.ToJson(req);
            using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerTexture();
                request.SetRequestHeader("Content-Type", "application/json");
                //request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"error" + request.error);
                }
                else
                {
                    var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                        //DownloadHandlerTexture.GetContent(request);
                    //var responses = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary>(request.downloadHandler.text);
                    Debug.Log($"Form upload complete! {texture}");

                    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                    image.sprite = newSprite;
                    //image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    //((DownloadHandlerTexture)request.downloadHandler).texture;
                    //welcomeText.SetText($"\n{responses["message"]}");

                    screenModel.DOScaleX(1, 0.4f).SetEase(Ease.InOutExpo);

                }

            }
        }
        public class Req
        {
            public string sender;
            public string message;
            public string type;
            public string key = "6Gb9tb29HC";
            public Req(string send, string msg, string tp)
            {
                this.sender = send;
                this.message = msg;
                this.type = tp;
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
        private void ToggleScreen()
        {
            if (_isActivated)
            {
                screenModel.DOScaleX(0, 0.4f).SetEase(Ease.InOutExpo);
            }
            else
            {
                var welcome = welcomeText.text;
                welcomeText.text = "";
                screenModel.DOScaleX(1, 0.4f).SetEase(Ease.InOutExpo)
                    .OnComplete(() => { welcomeText.DOText(welcome, 2f); });
            }

            _isActivated = !_isActivated;
        }
    }
}