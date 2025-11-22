using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public List<DropTable> m_MinionTables;
    public List<DropTable> m_CasterTables;


    private Dictionary<string, DropTable> m_EnemyTables;
    //Component Prefab 




    //Event
    EventBinding<OnEnemyDied> m_OnEnemyDied;



    void Start()
    {
        BindObjects();
        BindEvents();
    }
    void BindObjects()
    {
        //Make all tables a dictionary for fast read 
        m_EnemyTables = new Dictionary<string, DropTable>();
        foreach (var table in m_MinionTables)
        {
            m_EnemyTables.Add(table.m_enemy.m_name, table);
        }
        foreach (var table in m_CasterTables)
        {
            m_EnemyTables.Add(table.m_enemy.m_name, table);
        }


    }

    void BindEvents()
    {
        m_OnEnemyDied = new EventBinding<OnEnemyDied>(HandleEnemyDied);
        EventBus<OnEnemyDied>.Register(m_OnEnemyDied);
    }

    void UnbindEvents()
    {
        EventBus<OnEnemyDied>.Unregister(m_OnEnemyDied);
    }

    private void HandleEnemyDied(OnEnemyDied arg0)
    {
        DropTable _enemyTable = DecodeEnemy(arg0.enemyID);
        if (_enemyTable == null) return;

        if (!CanDrop(_enemyTable))
            return;

        ComponentSO _componentToDrop = GetComponentToDrop(_enemyTable);
        if (_componentToDrop == null) return;

        // Debug.Log($"Dropping {_componentToDrop.ComponentName} for {arg0.enemyID} on {arg0.deathPosition}");
        EventBus<OnDropComponent>.Raise(new OnDropComponent
        {
            isFromPlayer = false,
            data = _componentToDrop,
            position = arg0.deathPosition
        });




    }

    DropTable DecodeEnemy(string enemyName)
    {

        m_EnemyTables.TryGetValue(enemyName, out var table);
        return table;

    }
    bool CanDrop(DropTable dropTable)
    {
        float _baseChance = dropTable.m_BaseDropChance;
        // roll if can drop anything

        float _chance = Random.Range(0, 100);

        return _chance < _baseChance;
    }

    ComponentSO GetComponentToDrop(DropTable dropTable)
    {
        float _chance = Random.Range(0, 100);
        foreach (var entry in dropTable.m_PossibleDrops)
        {
            if (_chance < entry.m_DropChance)
            {
                return entry.m_ComponentToDrop;
            }
        }
        return null;
    }



    void OnDestroy()
    {
        UnbindEvents();
    }
}
