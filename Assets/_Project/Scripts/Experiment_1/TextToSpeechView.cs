using _Project.Scripts.Experiment_1.Data;
using Meta.WitAi.TTS.Utilities;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Assertions;

namespace _Project.Scripts.Experiment_1
{
    public class TextToSpeechView : MonoBehaviour
    {
        private TTSSpeaker _speaker;
        private InteractableUnityEventWrapper _button;
        private CommunicationData _response;

        public void Construct(TTSSpeaker speaker, InteractableUnityEventWrapper button, CommunicationData response)
        {
            _speaker = speaker;
            _button = button;
            _response = response;
        }

        private void Start()
        {
            _speaker.Speak("Hello");
            Assert.IsNotNull(_button);
            _button.WhenSelect.AddListener(() => { _speaker.Speak(_response.Value); });
        }
    }
}