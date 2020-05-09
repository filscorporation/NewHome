using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// UI showing that player can spend energy here
    /// </summary>
    public class EnergyCostUI : MonoBehaviour
    {
        private Transform targetToFollow;
        private float height = 0;
        private float maxHeight = 0;
        private float targetAlpha = 1F;
        private Vector2 lastPosition;
        private bool deactivating = false;

        [SerializeField] private GameObject energySlotPrefab;
        [SerializeField] private GameObject energyCellPrefab;
        [SerializeField] private RectTransform arrows;
        private List<GameObject> slots;
        private float slotHeight = 12.5F;
        private int filledCells = 0;

        private void Update()
        {
            if (!deactivating && targetToFollow == null)
            {
                deactivating = true;
                Destroy(this, 0.5F);
                Destroy(gameObject, 0.5F);
                return;
            }

            FollowTarget();
        }

        private void FollowTarget()
        {
            height = Mathf.Lerp(height, maxHeight, Time.deltaTime * 5F);
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                image.color = LerpColor(image.color);
            }

            Vector3 pos;
            if (targetToFollow == null)
            {
                pos = lastPosition;
            }
            else
            {
                pos = targetToFollow.position;
                lastPosition = targetToFollow.position;
            }
            transform.position = Camera.main.WorldToScreenPoint(pos + new Vector3(0, height));
        }

        private Color LerpColor(Color c)
        {
            return new Color(c.r, c.g, c.b, Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 2F));
        }

        /// <summary>
        /// Hides UI when energy bar was filled
        /// </summary>
        /// <returns></returns>
        public IEnumerator HideOnComplete()
        {
            yield return new WaitForSeconds(0.5F);
            maxHeight = 0;
            targetAlpha = 0F;
            yield return new WaitForSeconds(1F);
            Destroy(gameObject);
        }

        /// <summary>
        /// Hides UI
        /// </summary>
        /// <returns></returns>
        public IEnumerator Hide()
        {
            yield return new WaitForSeconds(0.5F);
            maxHeight = 0;
            targetAlpha = 0F;
            foreach (GameObject slot in slots)
            {
                slot.GetComponentInChildren<EnergyCell>()?.Back();
            }
            yield return new WaitForSeconds(1F);
            Destroy(gameObject);
        }

        /// <summary>
        /// Initialize energy cost UI with elements size
        /// </summary>
        /// <param name="target"></param>
        /// <param name="size"></param>
        /// <param name="h"></param>
        public void Initialize(Transform target, int size, float h)
        {
            slots = new List<GameObject>();
            for (int i = 0; i < size; i++)
            {
                slots.Add(Instantiate(energySlotPrefab, Vector3.zero, Quaternion.identity, transform));
                slots[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, slotHeight * i);
            }
            arrows.anchoredPosition = new Vector2(0, slotHeight * size);

            targetToFollow = target;
            maxHeight = h;
            height = 0;
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }
        }

        /// <summary>
        /// Fill one enegry slot
        /// </summary>
        public void AddEnergy()
        {
            if (filledCells == slots.Count)
                return;

            GameObject c = Instantiate(energyCellPrefab, Vector3.zero, Quaternion.identity, slots[filledCells].transform);
            c.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            filledCells++;

            if (filledCells == slots.Count)
                arrows.gameObject.SetActive(false);
        }
    }
}
