using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    private static EnemySpawner s_instance;
    public static EnemySpawner GetSpawner() { return s_instance; }
    [SerializeField] private GameObject m_enemy;
    [SerializeField] private List<GameObject> m_enemyPool = new List<GameObject>();
    [SerializeField] private int m_amountToPool;
    private int m_waveSize = 0;
    [SerializeField] private float m_spawnDelay;
    [SerializeField] private Vector2 m_spawnSpace;
    [SerializeField] private bool m_ShowSpawn = false;
    private bool m_canSpawn = false;

    [SerializeField] private bool _deBugSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        s_instance = this;
        StartCoroutine(SmartPooling());
        StartCoroutine(SpawnEnemy());
    }

    private void Update()
    {
        if (_deBugSpawn)
        {
            _DebugSpawn();
        }
    }
    public void StartNightSpawn()
    {
        //subscribe to the newday event
        GameManager.NewDayEvent += StartSpawning;
    }

    public void SpawnWave(int size)
    {
        m_canSpawn = true;
        m_waveSize = size;
        StartCoroutine(SmartPooling());
        StartCoroutine(SpawnEnemy());


    }

    private void _DebugSpawn()
    {
        _deBugSpawn = false;
        foreach (GameObject enemy in m_enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                bool safe = false;
                float x = 0;
                float y = 0;
                while (!safe)
                {
                    //find a safe space
                    x = Random.Range(transform.position.x - m_spawnSpace.x, transform.position.x + m_spawnSpace.x);
                    y = Random.Range(transform.position.y - m_spawnSpace.y, transform.position.y + m_spawnSpace.y);
                    safe = !Physics2D.BoxCast(new Vector2(x, y), Vector2.one, 0f, Vector2.zero);
                }
                enemy.transform.position = new Vector3(x, y, 0);
                enemy.SetActive(true);
                break;
            }
            else
            {
                //hard break if we cannot find a safe spot to spawn

                continue;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_ShowSpawn)
        {
            Gizmos.color = Color.gray * new Vector4(1, 1, 1, .25f);
            Gizmos.DrawCube(transform.position, m_spawnSpace * 2);
        }

    }

    IEnumerator SmartPooling()
    {
        for (int i = m_enemyPool.Count; i < m_amountToPool; i++)
        {
            //pause between frames.
            yield return null;
            GameObject newEnemy = Instantiate(m_enemy);
            newEnemy.SetActive(false);
            m_enemyPool.Add(newEnemy);
        }
    }

    internal void Restart()
    {
        foreach (GameObject enemy in m_enemyPool)
        {
            enemy.SetActive(false);
        }
        m_canSpawn = true;


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
        foreach (GameObject enemy in m_enemyPool)
        {
            if (enemy.activeInHierarchy)
            {
                numOfActive++;
                activeEnemies = true;
            }
        }
        return activeEnemies;
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(m_spawnDelay);
        int waveSize = m_waveSize - 1;
        while (m_canSpawn)
        {

            m_canSpawn = waveSize > 0;
            foreach (GameObject enemy in m_enemyPool)
            {
                if (!enemy.activeInHierarchy)
                {
                    bool safe = false;
                    float x = 0;
                    float y = 0;
                    while (!safe)
                    {
                        //find a safe space
                        x = Random.Range(transform.position.x - m_spawnSpace.x, transform.position.x + m_spawnSpace.x);
                        y = Random.Range(transform.position.y - m_spawnSpace.y, transform.position.y + m_spawnSpace.y);
                        safe = !Physics2D.BoxCast(new Vector2(x, y), Vector2.one, 0f, Vector2.zero);
                    }
                    enemy.transform.position = new Vector3(x, y, 0);
                    enemy.SetActive(true);
                    waveSize--; // decrease the remaining to spawn;
                    break;
                }
                else
                {
                    //hard break if we cannot find a safe spot to spawn

                    continue;
                }
            }

            yield return new WaitForSeconds(m_spawnDelay);
        }
        m_waveSize = 0;
    }
}
