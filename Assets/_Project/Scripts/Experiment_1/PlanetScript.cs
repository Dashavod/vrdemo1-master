using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace _Project.Scripts.Experiment_1
{

    public class PlanetScript : MonoBehaviour
    {
        private bool IsMovetoTarget;
        public Vector3 target;
        public Vector3 generalPosition;
        public float speed;
        private void OnEnable()
        {
            AskWitController.OnMovedPlanet += MovePlanetToPlayer;
        }

        private void OnDisable()
        {
            AskWitController.OnMovedPlanet -= MovePlanetToPlayer;
        }
        private void MovePlanetToPlayer(string objectname)
        {

            //float step = speed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, target, step);
            
            IsMovetoTarget = gameObject.name== objectname;
            Debug.Log($"{gameObject.name}move planet to player");
        }
        private void MovePlanetFromPlayer(string objectname)
        {
            Debug.Log($"move planet from player");
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (IsMovetoTarget)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, step);
            }
        }
    }
}