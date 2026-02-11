namespace Sober.Scene
{
    public sealed class Library
    {

        //stores prefab entities and combines them with overrides to make final entities.

        private readonly Dictionary<string, EntityData> _prefabs = new();
        public void Register(string PrefabName, EntityData data)
        {
            _prefabs[PrefabName] = data;
        }


        public bool TryGet(string PrefabName, out EntityData data)
        {
            return _prefabs.TryGetValue(PrefabName, out data);
        }

        public static EntityData Merge(EntityData prefab, EntityData overrides)
        {
            return new EntityData
            {
                Name = overrides.Name != "Entity" ? overrides.Name : prefab.Name,
                Prefab = overrides.Prefab,
                Transform = overrides.Transform ?? prefab.Transform,
                Sprite = overrides.Sprite ?? prefab.Sprite,
                Mesh = overrides.Mesh ?? prefab.Mesh,
                Velocity = overrides.Velocity ?? prefab.Velocity,
                Player = overrides.Player || prefab.Player,
                Camera = overrides.Camera ?? prefab.Camera,
                Follow = overrides.Follow ?? prefab.Follow
            };
        }
    }
}
