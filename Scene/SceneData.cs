using System.Text.Json.Serialization;

namespace Sober.Scene
{
    //defines C# models and makes them ready to be translated to or from JSON.
    public sealed class SceneData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Default";
        [JsonPropertyName("assets")]
        public SceneAssets Assets { get; set; } = new();
        [JsonPropertyName("entities")]
        public List<EntityData> Entities { get; set; } = new();
    }


    public sealed class SceneAssets
    {
        [JsonPropertyName("textures")]
        public Dictionary<string, string> Textures { get; set; } = new();

        [JsonPropertyName("shaders")]
        public Dictionary<string, ShaderData> Shaders { get; set; } = new();
    }


    public sealed class ShaderData
    {
        [JsonPropertyName("vert")]
        public string Vert { get; set; } = "";

        [JsonPropertyName("frag")]
        public string Frag { get; set; } = "";
    }


    public sealed class EntityData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Entity";

        [JsonPropertyName("prefab")]
        public string? Prefab { get; set; }

        [JsonPropertyName("transform")]
        public TransformData? Transform { get; set; }

        [JsonPropertyName("sprite")]
        public SpriteData? Sprite { get; set; }

        [JsonPropertyName("mesh")]
        public MeshRendererData? Mesh { get; set; }

        [JsonPropertyName("velocity")]
        public VelocityData? Velocity { get; set; }

        [JsonPropertyName("player")]
        public bool Player { get; set; }

        [JsonPropertyName("camera")]
        public CameraData? Camera { get; set; }

        [JsonPropertyName("follow")]
        public CameraFollowData? Follow { get; set; }
    }


    public sealed class TransformData
    {
        public float[] Position { get; set; } = new float[] { 0, 0 };
        public float Rotation { get; set; } = 0f;
        public float[] Scale { get; set; } = new float[] { 1, 1 };
    }


    public sealed class SpriteData
    {
       public string TextureKey { get; set; } = "";
    }


    public sealed class MeshRendererData
    {
        public string Mesh { get; set; } = "Quad";     
        public string ShaderKey { get; set; } = "";
        public bool Pulse { get; set; } = false;
    }


    public sealed class VelocityData
    {
        public float Speed { get; set; } = 1.0f;
    }


    public sealed class CameraData
    {
        public float Size { get; set; } = 1.2f;
        public float Zoom { get; set; } = 1.0f;
    }


    public sealed class CameraFollowData
    {
        public string TargetName { get; set; } = "Player";
        public float Smoothing { get; set; } = 8f;
        public float[] DeadZone { get; set; } = new float[] { 0.1f, 0.1f };
    }
}