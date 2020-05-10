using System.Collections;
using UnityEngine;

namespace Assets.Source
{
    public enum CycleState
    {
        Day,
        Night,
    }

    /// <summary>
    /// Controlls day night cycle
    /// </summary>
    public class DayNightCycleController : MonoBehaviour
    {
        public static CycleState CycleState = CycleState.Day;
        [SerializeField] [Range(1, 300)] private int dayLengthSeconds = 30;
        [SerializeField] [Range(0, 300)] private int nightLengthSeconds = 15;
        [SerializeField] private Color dayColor;
        [SerializeField] private Color nightColor;

        private void Start()
        {
            StartCoroutine(RunCycle());
        }

        private IEnumerator RunCycle()
        {
            CycleState = CycleState.Day;
            Camera.main.backgroundColor = dayColor;
            yield return new WaitForSeconds(dayLengthSeconds);
            CycleState = CycleState.Night;
            Camera.main.backgroundColor = nightColor;
            yield return new WaitForSeconds(nightLengthSeconds);
            StartCoroutine(RunCycle());
        }
    }
}
