using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject doors;
    public GameObject sign;

    public AudioSource audioSource;

    public float enemySpawnCooldown = 3;
    public int maxEnemyNumber = 2;
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    public List<GameObject> enemys = new();
    public bool canSpawn = true;

    public Transform[] patrolPoints;

    public bool closed = false;

    void Update()
    {
        if (closed) return;
        if (!canSpawn) return;
        if (enemys.Count == maxEnemyNumber) return;

        canSpawn = false;

        // Spawn
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().OnEnemyDead += OnEnemyDead;
        enemy.GetComponent<EnemyPatrol>().patrolPoints = patrolPoints;
        enemys.Add(enemy);

        if (enemys.Count < maxEnemyNumber) StartCoroutine(SpawnCooldown());
    }

    public void OnEnemyDead(GameObject enemy)
    {
        enemys.Remove(enemy);
        enemy.GetComponent<Enemy>().OnEnemyDead -= OnEnemyDead;
        StartCoroutine(SpawnCooldown());
    }

    public IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(enemySpawnCooldown);

        canSpawn = true;
    }

    public void Close()
    {
        audioSource.Play();
        closed = true;
        doors.SetActive(false);
        sign.GetComponent<Rigidbody2D>().simulated = true;
        sign.GetComponent<Rigidbody2D>().WakeUp();
    }
}
