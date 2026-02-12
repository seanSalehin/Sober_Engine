namespace Sober.ECS.Events
{
    public readonly struct CollisionEnterEvent
    {
        //collision start (when two entities start touching)
        public readonly int A;
        public readonly int B;
        public CollisionEnterEvent(int a, int b) 
        {
            A = a;
            B = b;
        }
    }

    public readonly struct CollisionExitEvent
    {
        //collision end 
        public readonly int A;
        public readonly int B;
        public CollisionExitEvent(int a, int b) 
        {
            A = a; 
            B = b;
        }
    }
}
