using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Energy bar UI logic
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] private GameObject energyCellPrefab;
        [SerializeField] private RectTransform topCellEffect;
        private List<GameObject> cells = new List<GameObject>();
        private float cellHeight = 6.25F;
        private float yOffset = 15.625F;
        private int filledCells = 0;

        private bool energyBarActive = false;
        private Animator animator;
        private const string animatorActiveParam = "Active";

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            if (!energyBarActive)
                return;
        }

        /// <summary>
        /// Sets energy value stored in the bar
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        public void SetValue(int value, int max)
        {
            if (value == filledCells)
                return;

            if (value > filledCells)
            {
                for (int i = filledCells; i < value; i++)
                {
                    cells.Add(Instantiate(energyCellPrefab, Vector3.zero, Quaternion.identity, transform));
                    cells[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, cellHeight * i + yOffset);
                }
                topCellEffect.anchoredPosition = new Vector2(0, cellHeight * value + yOffset);
                filledCells = value;
            }

            if (value < filledCells)
            {
                for (int i = value; i < filledCells; i++)
                {
                    Destroy(cells[i]);
                    cells.RemoveAt(i);
                }
                topCellEffect.anchoredPosition = new Vector2(0, cellHeight * value + yOffset);
                filledCells = value;
            }
        }

        /// <summary>
        /// Show energy bar
        /// </summary>
        public void Activate()
        {
            if (energyBarActive)
                return;

            energyBarActive = true;
            animator.SetBool(animatorActiveParam, true);
        }

        /// <summary>
        /// Hide energy bar
        /// </summary>
        public void Deactivate()
        {
            if (!energyBarActive)
                return;

            energyBarActive = false;
            animator.SetBool(animatorActiveParam, false);
        }
    }
}
