using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Wall
    /// </summary>
    public class Wall : InteractableObject
    {
        protected override int Cost => 3;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 2F;

        protected override void OnComplete()
        {
            Debug.Log("Wall complete");
        }
    }
}
