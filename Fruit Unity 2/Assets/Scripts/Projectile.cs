using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileObjectController localProjectileController;

    public int projectileID;
    public Rigidbody rigidBody;
    public Collider collider;
    public int shotByPlayer;
    public Vector3 initialVector;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 75f;
    public int charNumber;

    private void Start()
    {
        rigidBody.AddForce(initialVector);
        StartCoroutine(ExplodeAfterTime());

        if (transform.rotation.x != 0f)
        {
            transform.rotation = Quaternion.AngleAxis(30, Vector3.right);
        }
    }

    public void InitializeProjectile(Vector3 movementDirection, float initialForce, int player)
    {
        initialVector = movementDirection * initialForce;
        shotByPlayer = player;
        charNumber = 1;
    }

    public void FixedUpdate()
    {
        if (rigidBody.velocity.y != 0)
        {
            transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");

        if (other.gameObject.tag != "Projectile")
        {
            Debug.Log("Freeze");
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;

            if (other.gameObject == gameObject.CompareTag("Player"))
            {
                Debug.Log("Explode");
                Explode();
            }
        }
    }
    /*
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
    */
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
        localProjectileController.DestroyNetwork();
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(15f);

        Explode();
    }
}