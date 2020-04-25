using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Source
{
    /// <summary>
    /// Animates bees in main menu
    /// </summary>
    public class SinusAnimator : MonoBehaviour
    {
        public bool Animate = true;

        private Vector2 basePosition;
        [SerializeField] private float flyAmplitude = 0.5F;
        private float flyOffset;

        public void Start()
        {
            basePosition = transform.position;
            flyOffset = Random.Range(0, 1F);
        }

        public void Update()
        {
            if (Animate)
                Fly();
        }

        private void Fly()
        {
            transform.position = new Vector2(
                basePosition.x,
                basePosition.y + Mathf.Sin((DateTime.Now.Millisecond / 1000F + flyOffset) * 2 * Mathf.PI) * flyAmplitude);
        }
    }
}
