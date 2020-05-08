using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Spaceship interaction logic
    /// </summary>
    public class Spaceship : InteractableObject
    {
        protected override int Cost => 15;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 1.8F;

        protected override void OnComplete()
        {
            Debug.Log("Spaceship complete");
        }
    }
}
