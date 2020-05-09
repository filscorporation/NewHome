using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Animates y coordinate by sin function
    /// </summary>
    public class SinusAnimator : MonoBehaviour
    {
        public bool Animate = true;
        
        [SerializeField] private float flyAmplitude = 0.5F;
        private float flyOffset;
        private float baseY;

        private void Start()
        {
            flyOffset = Random.Range(0, 1F);
            baseY = transform.position.y;
        }

        private void Update()
        {
            if (Animate)
                Fly();
        }

        private void Fly()
        {
            transform.position = new Vector2(
                transform.position.x,
                baseY + Mathf.Sin((DateTime.Now.Millisecond / 1000F + flyOffset) * 2 * Mathf.PI) * flyAmplitude);
        }
    }
}
