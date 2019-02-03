using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class FollowTargetSystem : JobComponentSystem
{
    ComponentGroup group;

    private NativeArray<float3> positions;
    const int max = 32;

    protected override void OnCreateManager()
    {
        group = this.GetComponentGroupForIJobProcessComponentData(typeof(MoveToTargetJob));
        positions = new NativeArray<float3>(max, Allocator.Persistent);
    }

    protected override void OnDestroyManager()
    {
        positions.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps = new CopyPosition
        {
            targets = group.GetComponentDataArray<Target>(),
            targetPositions = positions,
            positionFromEntity = GetComponentDataFromEntity<Position>()
        }.Schedule(group.CalculateLength(), 6, inputDeps);

        inputDeps = new MoveToTargetJob
        {
            deltaTime = Time.deltaTime,
            targetPositions = positions,
        }.Schedule(this, inputDeps);

        return inputDeps;
    }

    [BurstCompile]
    struct CopyPosition : IJobParallelFor
    {
        [WriteOnly] public NativeArray<float3> targetPositions;
        [ReadOnly] public ComponentDataArray<Target> targets;
        [ReadOnly] public ComponentDataFromEntity<Position> positionFromEntity;

        public void Execute(int index)
        {
            var entity = targets[index].Value;
            targetPositions[index] = positionFromEntity[entity].Value;
        }
    }

    [BurstCompile]
    [RequireComponentTag(typeof(Target))]
    struct MoveToTargetJob : IJobProcessComponentDataWithEntity<Position>
    {
        [ReadOnly] public NativeArray<float3> targetPositions;

        public float deltaTime;

        public void Execute(Entity entity, int index, ref Position pos)
        {
            var diff = targetPositions[index] - pos.Value;
            pos.Value += math.normalize(diff) * deltaTime;
        }
    }
}