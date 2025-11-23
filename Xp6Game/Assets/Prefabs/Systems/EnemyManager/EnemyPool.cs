using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyPool : MonoBehaviour
{
    // Dicionário para armazenar as filas de objetos.
    // A chave é o prefab (GameObject) do inimigo, o valor é a fila de objetos inativos.
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    // --- Configuração ---
    // Você pode usar uma lista para definir o tamanho inicial do pool para cada tipo de inimigo
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialPoolSize = 5; // Quantidade inicial a ser criada
    }

    [Header("Configuração Inicial do Pool")]
    public List<PoolConfig> pools;

    //Events
    EventBinding<OnEnemyDied> m_OnEnemyDiedBinding;




    private void Awake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        BindEvents();
        InitializePools();
    }
    void BindEvents()
    {
        m_OnEnemyDiedBinding = new EventBinding<OnEnemyDied>(OnEnemyDiedHandler);
        EventBus<OnEnemyDied>.Register(m_OnEnemyDiedBinding);
    }



    private void InitializePools()
    {
        // 1. Itera sobre a lista de configurações de pool.
        foreach (PoolConfig pool in pools)
        {
            // Cria uma nova fila para este tipo de prefab.
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // 2. Preenche a fila com objetos inativos.
            for (int i = 0; i < pool.initialPoolSize; i++)
            {
                // Instancia o novo objeto como filho do Pool Manager (para organização)
                GameObject obj = Instantiate(pool.prefab, this.transform);
                obj.SetActive(false); // Desativa o objeto
                objectPool.Enqueue(obj); // Adiciona à fila
            }
            // var _data = pool.prefab.GetComponent<Enemy<EnemySO>>();
            // Debug.Log(pool.prefab.name);
            // Debug.Log(_data.name);
            var _name = pool.prefab.name;
            // Debug.Log(_name);

            // 3. Adiciona o prefab e a fila ao dicionário.
            poolDictionary.Add(_name, objectPool);
        }
    }

    // --- Métodos Públicos de Pooling ---

    // Método para PEGAR (Spawnar) um inimigo do pool
    public GameObject SpawnFromPool(EnemyData enemyData, Vector3 position, Quaternion rotation)
    {
        GameObject prefabToPool = enemyData.m_prefab;
        // string prefabName = prefabToPool.GetComponent<Enemy<EnemySO>>().m_entityData.m_name;
        string prefabName = prefabToPool.name;



        // 1. Verifica se o prefab está registrado no pool.
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning($"Pool para o prefab '{prefabToPool.name}' não encontrado.");

            // Opcional: Instanciar um novo fora do pool se não for encontrado
            return Instantiate(prefabToPool, position, rotation);
        }

        Queue<GameObject> objectPool = poolDictionary[prefabName];

        // 2. Verifica se a fila está vazia (precisa criar mais objetos).
        if (objectPool.Count == 0)
        {
            // Cria e retorna um novo objeto, fora da capacidade inicial.
            GameObject newObj = Instantiate(prefabToPool, this.transform);
            newObj.name = prefabToPool.name; // Para rastreamento
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);

            // Não o adicionamos ao pool (ainda), mas o ativamos.
            return newObj;
        }

        // 3. Pega o objeto do pool e o configura.
        GameObject objectToSpawn = objectPool.Dequeue();
        objectToSpawn.name = prefabToPool.name;

        objectToSpawn.transform.SetParent(this.transform.parent);
        Debug.Log($" {objectToSpawn.name} in position {position}");
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        objectToSpawn.SetActive(true);
        // Retorna o objeto ativado.
        return objectToSpawn;
    }

    // Método para RETORNAR (Destruir/Reciclar) um inimigo para o pool
    public void ReturnToPool(GameObject objectToReturn)
    {
        // Você precisará de uma forma de saber qual prefab original gerou este objeto.
        // A maneira mais robusta é usar uma interface ou componente no objeto poolado.

        // --- Solução Simples (Requer Componente Helper) ---
        // string prefabName = objectToReturn.GetComponent<Enemy<EnemySO>>().m_entityData.m_name;
        string prefabName = objectToReturn.name;

        if (!poolDictionary.ContainsKey(prefabName)) Destroy(objectToReturn);

        // Desativa, reseta o pai (opcional) e coloca na fila
        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(this.transform); // Volta a ser filho do Manager
        poolDictionary[prefabName].Enqueue(objectToReturn);
    }

    private void OnEnemyDiedHandler(OnEnemyDied arg0)
    {
        ReturnToPool(arg0.enemy);

    }

}