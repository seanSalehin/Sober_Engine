//Filters entities by component combinations (two components TA and TB)
namespace Sober.ECS
{
    public static class Query
    {

        public static IEnumerable<int> with<TA, TB>(World world)
            where TA : struct
            where TB : struct
        {
            var a = world.GetStore<TA>();
            var b = world.GetStore<TB>();

            int aCount = 0;
            int bCount = 0;
            foreach (var _ in a.All()) aCount++;
            foreach (var _ in b.All()) bCount++;

            if (aCount <= bCount)
            {
                foreach (var pair in a.All())
                {
                    if (b.Has(pair.Key))
                    {
                        yield return pair.Key;
                    }
                }
            }
            else
            {
                foreach (var pair in b.All())
                {
                    if (a.Has(pair.Key))
                    {
                        yield return pair.Key;
                    }
                }
            }
        }
    }
}