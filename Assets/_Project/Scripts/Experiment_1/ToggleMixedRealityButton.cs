using System;
using Oculus.Interaction;
using UnityEngine;

namespace _Project.Scripts.Experiment_1
{
    public class ToggleMixedRealityButton : MonoBehaviour
    {
        private InteractableUnityEventWrapper _button;
        private bool _isActivatedMixed = true;
        private OVRManager _ovrManager;
        private VirtualEnvironmentTag _vrEnvironment;

        public void Construct(InteractableUnityEventWrapper button,
            OVRManager ovrManager,
            VirtualEnvironmentTag vrEnvironment)
        {
            _button = button;
            _ovrManager = ovrManager;
            _vrEnvironment = vrEnvironment;
        }

        private void Start()
        {
            _button.WhenSelect.AddListener(ToggleReality);
        }

        private void ToggleReality()
        {
            _isActivatedMixed = !_isActivatedMixed;
            _ovrManager.isInsightPassthroughEnabled = _isActivatedMixed;
            _vrEnvironment.gameObject.SetActive(!_isActivatedMixed);
        }
    }
}