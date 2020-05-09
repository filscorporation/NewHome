using System.Collections;
using UnityEngine;

namespace Assets.Source.Objects.Interactable
{
    /// <summary>
    /// Wall
    /// </summary>
    public class Wall : InteractableObject
    {
        protected override int Cost => 3;
        protected override float InteractRadius => 0.5F;
        protected override float Height => 2F;

        protected override IEnumerator OnComplete()
        {
            IsActive = false;
            Debug.Log("Wall complete");
            yield break;
        }
    }
}
