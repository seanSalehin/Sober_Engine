//Store Positions, Rotation, Scale of an entity in 3D space + cache world matrix
using OpenTK.Mathematics;


namespace Sober.ECS.Components
{
    public struct TransformComponent
    {
        public Vector2 LocalPosition;
        public float LocalRotation;
        public Vector2 LocalScale;
        public int ParentEntityId;

        //cached world matrix
        public bool Dirty;
        public Matrix4 WorldMatrix;
        public Matrix4 LocalMatrix;

        public static TransformComponent Default()
        {
            return new TransformComponent()
            {
                LocalPosition = Vector2.Zero,
                LocalRotation = 0f,
                LocalScale = Vector2.One,
                ParentEntityId = 0,
                Dirty = true,
                WorldMatrix = Matrix4.Identity,
                LocalMatrix = Matrix4.Identity
            };
        }
    }
}
