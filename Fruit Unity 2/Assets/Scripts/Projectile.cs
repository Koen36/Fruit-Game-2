using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rigidBody;
    public int shootByPlayer;
    public Vector3 initialForce;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 75f;
    public int charNumber;
    private bool firstTime = true;

    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        rigidBody.AddForce(initialForce);
        StartCoroutine(ExplodeAfterTime());

        //ServerSend.SpawnProjectile(this, shootByPlayer);
    }

    private void FixedUpdate()
    {
        if (firstTime == false)
        {
            transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
        }
        //ServerSend.ProjectilePosition(this);
        firstTime = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void InitializeBananaProjectile(Vector3 _initialMovementDirection, float _initialForceStrength, int _shootByPlayer)
    {
        initialForce = _initialMovementDirection * _initialForceStrength;
        shootByPlayer = _shootByPlayer;
        charNumber = 1;
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
        yield return new WaitForSeconds(10f);

        Explode();
    }
}