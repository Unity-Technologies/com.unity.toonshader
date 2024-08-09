using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace UnityEngine.Rendering.Toon.Universal.ECS.Samples
{

    public partial struct RotatorSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (rotator, xform) in SystemAPI.Query<RefRO<Rotator>, RefRW<LocalTransform>>())
            {

                var speed = rotator.ValueRO.speed;
                var roatation = quaternion.AxisAngle(math.float3(0.0f, 1.0f, 0.0f), math.PI * -speed/180.0f * Time.time);

                xform.ValueRW.Rotation = roatation;
            }
        }
    }

}