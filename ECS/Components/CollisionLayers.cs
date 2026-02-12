namespace Sober.ECS.Components
{
    public static class CollisionLayers
    {
        public const int None = 0;
        public const int Player = 1 << 0; 
        public const int World = 1 << 1; 
        public const int Enemy = 1 << 2; 
        public const int Bullet = 1 << 3;  
        public const int Pickup = 1 << 4;  // items
        public const int Trigger = 1 << 5;  //  checkpoints, doors
        public const int OneWayPlatform = 1 << 6;  // jump through platforms (land above)
        public const int Water = 1 << 7; 
        public const int Ladder = 1 << 8;  
        public const int CameraBlocker = 1 << 9; 
        public const int UI = 1 << 10; 
        public const int All = ~0; //enable all layers
    }
}
