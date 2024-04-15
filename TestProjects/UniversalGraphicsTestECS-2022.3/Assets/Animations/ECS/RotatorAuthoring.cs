
using Unity.Entities;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Toon.Universal.ECS.Samples
{
    public struct Rotator : IComponentData
    {
        public float speed;
        public float3 pivot;
    }

    public class RotatorAuthoring : MonoBehaviour
    {
        public float _speed = 20.0f;
        public float3 _pivot = Vector3.zero;
        class Baker : Baker<RotatorAuthoring>
        {
            public override void Bake(RotatorAuthoring src)
            {
                var data = new Rotator() { speed = src._speed, pivot = src._pivot };
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
            }
        }
    }

}