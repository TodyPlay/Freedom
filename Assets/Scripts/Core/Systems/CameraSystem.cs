using Authoring;
using Core.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Core.Systems
{
    public partial class CameraSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<MainActor>();
        }

        protected override void OnUpdate()
        {
            var et = SystemAPI.GetSingletonEntity<MainActor>();


            var localToWorld = EntityManager.GetComponentData<LocalToWorld>(et);


            CameraHolder.camera.transform.LookAt(localToWorld.Position);

        }
    }
}