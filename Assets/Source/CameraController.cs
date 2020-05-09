using System;
using UnityEngine;

namespace Assets.Source
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Start()
        {
            basePosition = transform.position;
        }

        private void Update()
        {
            if (target != null)
            {
                Vector3 v = new Vector3(target.position.x, target.position.y + 1.8F, -10F);
                basePosition = Vector3.Lerp(basePosition, v, Time.deltaTime * 3F);
            }

            ShakeUpdate();
        }

        [Range(0, 1F)] [SerializeField] private float maxShakeOffset = 0.5F;
        [Range(0, 1F)] [SerializeField] private float shakeDeduction = 0.6F;
        private Vector3 basePosition;

        private bool isShaking = false;
        private float shakeForce;
        private float seed;
        private DateTime shakeStart;

        private void ShakeUpdate()
        {
            if (!isShaking)
            {
                transform.position = basePosition;
                return;
            }

            float offsetX = maxShakeOffset * shakeForce * Mathf.PerlinNoise(seed + 0.1F, (float)(DateTime.Now - shakeStart).TotalMilliseconds);
            float offsetY = maxShakeOffset * shakeForce * Mathf.PerlinNoise(seed + 0.2F, (float)(DateTime.Now - shakeStart).TotalMilliseconds);

            transform.position = new Vector3(basePosition.x + offsetX, basePosition.y + offsetY, basePosition.z);
            shakeForce -= Time.deltaTime * shakeDeduction;
            if (shakeForce <= 0)
                isShaking = false;
        }

        /// <summary>
        /// Shakes camera
        /// </summary>
        /// <param name="force"></param>
        public void Shake(float force)
        {
            isShaking = true;
            shakeForce = force;
            seed = UnityEngine.Random.Range(0, 1);
            shakeStart = DateTime.Now;
        }
    }
}
