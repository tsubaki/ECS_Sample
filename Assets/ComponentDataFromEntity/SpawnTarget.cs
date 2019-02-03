using UnityEngine;
using Unity.Entities;

public class SpawnTarget : MonoBehaviour
{
    [SerializeField] GameObjectEntity goal;
    [SerializeField] GameObject prefab;

    public void Start()
    {
        var entityManager = World.Active.GetExistingManager<EntityManager>();
        var entity = entityManager.Instantiate(prefab);

        entityManager.SetComponentData(entity, new Target() { Value = goal.Entity });
    }
}