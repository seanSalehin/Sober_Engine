namespace Sober.ECS.Components
{
    public struct AudioSourceComponent
    {
        public int SourceId;
        public string ClipKey;
        public bool Loop;
        public float Volume;
        public bool PlayRequested;
    }
}
