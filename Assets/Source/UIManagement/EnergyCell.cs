using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Energy cell in UI
    /// </summary>
    public class EnergyCell : MonoBehaviour
    {
        private RectTransform rect;
        private bool back = false;
        private Vector2 targetPos;

        private const string backAnimatorParam = "Back";

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            transform.position = Camera.main.WorldToScreenPoint(Player.Instance.transform.position);
        }

        private void Update()
        {
            if (back)
            {
                transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * 8F);
            }
            else
            {
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, Vector2.zero, Time.deltaTime * 8F);
            }
        }

        /// <summary>
        /// Makes cell to move back to player
        /// </summary>
        public void Back()
        {
            targetPos = Camera.main.WorldToScreenPoint(Player.Instance.transform.position);
            GetComponent<Animator>().SetTrigger(backAnimatorParam);
            back = true;
        }
    }
}
