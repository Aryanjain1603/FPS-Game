using System;
using Unity.Mathematics;
using UnityEngine;


    public class GunAimer : MonoBehaviour
    {
        public Camera camera;
        public Transform gunModel;
        public float aimDistance = 10;
        public float rotationSpeed = 20f;
        public Transform target;
               

        void Update()
        {
            Vector3 targetPosition = camera.transform.position + camera.transform.forward * aimDistance;
            target.transform.position = targetPosition;

            Vector3 direction = (targetPosition - gunModel.position).normalized;
            quaternion lookRotation = Quaternion.LookRotation(direction);

            gunModel.rotation = Quaternion.Slerp(gunModel.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }