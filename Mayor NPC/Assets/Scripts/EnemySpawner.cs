using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private List<GameObject> enemyPool = new List<GameObject>();
    [SerializeField] private int amountToPool;
    [SerializeField] private float spawnDelay;
    [SerializeField] private Vector2 spawnSpace;
    private bool canSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SmartPooling());
        StartCoroutine(SpawnEnemy());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray * new Vector4(1, 1, 1, .25f);
        Gizmos.DrawCube(transform.position, spawnSpace * 2);
       
    }

    IEnumerator SmartPooling()
    {
        for(int i = enemyPool.Count; i < amountToPool; i++)
        {
            //pause between frames.
            yield return null;
            GameObject newEnemy = Instantiate(enemy);
            newEnemy.SetActive(false);
            enemyPool.Add(newEnemy);
        }
    }

    internal void Restart()
    {
        foreach(GameObject enemy in enemyPool)
        {
            enemy.SetActive(false);
        }
        canSpawn = true;
        //subscribe to the newday event
        GameManager.NewDayEvent += StartSpawning;

    }
    private void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
        //unsubscribe from event
        GameManager.NewDayEvent -= StartSpawning;
    }

    internal bool EnemiesActive(out int numOfActive)
    {
        numOfActive = 0;
        bool activeEnemies = false;
        foreach(GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy)
            {
                numOfActive++;
                activeEnemies =  true;
            }
        }
        return activeEnemies;
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(spawnDelay);
        while (canSpawn)
        {
            foreach(GameObject enemy in enemyPool)
            {
                if (!enemy.activeInHierarchy)
                {
                    bool safe = false;
                    float x = 0;
                    float y = 0;
                    while (!safe)
                    {
                        //find a safe space
                        x = Random.Range(transform.position.x - spawnSpace.x, transform.position.x + spawnSpace.x);
                        y = Random.Range(transform.position.y - spawnSpace.y, transform.position.y + spawnSpace.y);
                        safe = !Physics2D.BoxCast(new Vector2(x, y), Vector2.one, 0f, Vector2.zero);
                    }
                    enemy.transform.position = new Vector3(x, y, 0);
                    enemy.SetActive(true);
                    canSpawn = true;
                    break;
                }
                else
                {
                    canSpawn = false;
                    continue;
                }              
            }
            yield return new WaitForSeconds(spawnDelay);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
