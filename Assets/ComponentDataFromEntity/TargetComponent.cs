using Unity.Entities;

public class TargetComponent : ComponentDataWrapper<Target>{ }

[System.Serializable]
public struct Target : IComponentData{
    public Entity Value;
}