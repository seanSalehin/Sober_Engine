using OpenTK.Mathematics;

namespace Sober.ECS.Components
{
    public struct TriggerZoneComponent
    {
        public Vector2 HalfSize;
        public string ScriptId;
        public bool IsTriggered;

        public TriggerZoneComponent(Vector2 halfSize, string scriptId)
        {
            HalfSize = halfSize;
            ScriptId = scriptId;
            IsTriggered = false;
        }
    }
}