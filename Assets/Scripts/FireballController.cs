using UnityEngine;

public class FireballController : MonoBehaviour
{
    [HideInInspector] public Character target;
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.fixedDeltaTime);

        if (transform.position == target.transform.position)
        {
            target.TakeDamage(damage, 0);
            Destroy(gameObject);
        }
    }
}
