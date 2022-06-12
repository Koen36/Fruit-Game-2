using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rigidBody;
    public Collider collider;
    public int shotByPlayer;
    public Vector3 initialVector;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 75f;
    public int charNumber;

    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        rigidBody.AddForce(initialVector);
        StartCoroutine(ExplodeAfterTime());
    }

    public void InitializeProjectile(Vector3 movementDirection, float initialForce, int player)
    {
        initialVector = movementDirection * initialForce;
        shotByPlayer = player;
        charNumber = 1;

        //transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");

        if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag ==  "Player")
        {
            Debug.Log("Ignore");
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), collider);
        }
        else
        {
            Debug.Log("Freeze");
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;

            if (collision.gameObject == gameObject.CompareTag("Player"))
            {
                Debug.Log("Explode");
                Explode();
            }
        }
    }

    private void Explode()
    {
        if (charNumber != 2)
        {
            //ServerSend.ProjectileExploded(this);

            Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider _collider in _colliders)
            {
                if (_collider.CompareTag("Player"))
                {
                    //_collider.GetComponent<Player>().TakeDamage(explosionDamage);
                }
            }
        }

        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(15f);

        Explode();
    }
}