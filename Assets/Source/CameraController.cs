using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        public void Update()
        {
            if (target != null)
            {
                Vector3 v = new Vector3(target.position.x, target.position.y + 1.8F, -10F);
                transform.position = Vector3.Lerp(transform.position, v, Time.deltaTime * 3F);
            }
        }
    }
}
