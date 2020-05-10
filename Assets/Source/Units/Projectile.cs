using UnityEngine;

namespace Assets.Source.Units
{
    /// <summary>
    /// Projectile shoot by golems
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public const float Gravity = 10F;
        public const float GroundLevel = -4.01F;
        private float dx;
        private float dy;
        private int damage;
        private int direction = 0;
        [SerializeField] private GameObject onHitEffect;
        private Vector2 lastPos;

        public void Initialize(float speedX, float speedY, int dir, int dmg)
        {
            dx = speedX;
            dy = speedY;
            direction = dir;
            damage = dmg;
            lastPos = transform.position;
        }

        private void FixedUpdate()
        {
            transform.position += new Vector3(direction * dx, dy) * Time.fixedDeltaTime;
            dy -= Gravity * Time.fixedDeltaTime;
            if (transform.position.y < GroundLevel)
            {
                Destroy(Instantiate(onHitEffect, transform.position, Quaternion.identity), 2F);
                Destroy(gameObject);
            }
            
            //float range = Mathf.Abs(transform.position.x - lastPos.x);
            //GameObject hit = direction == 1
            //    ? MapManager.Instance.Enemies.FirstToTheRight(transform.position.x, range)?.gameObject
            //    : MapManager.Instance.Enemies.FirstToTheLeft(transform.position.x, range)?.gameObject;
            //if (hit != null)
            //{
                var hitCollider = Physics2D.Linecast(lastPos, transform.position);
                if (hitCollider.collider == null)
                    return;
                Alien alien = hitCollider.collider.gameObject.GetComponent<Alien>();
                if (alien == null)
                    return;

                alien.TakeDamage(damage);
                Destroy(Instantiate(onHitEffect, transform.position, Quaternion.identity), 2F);
                Destroy(gameObject);
            //}
        }

        private void LateUpdate()
        {
            lastPos = transform.position;
        }
    }
}
