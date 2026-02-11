namespace Sober.Scene
{
    public class GameObject
    {
        public string Name { get; set; }
        public Transform Transform { get; } = new Transform();
        public GameObject(string name)
        {
            Name = name;
        }
    }
}
