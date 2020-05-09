using UnityEngine;

namespace Assets.Source
{
    /// <summary>
    /// Controls main game processes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance => instance ?? (instance = FindObjectOfType<GameManager>());
    }
}
