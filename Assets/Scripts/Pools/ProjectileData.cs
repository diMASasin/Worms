namespace Pools
{
    public class ProjectileData
    {
        public Wind.Wind Wind { get; set; }

        public ProjectileData(Wind.Wind wind)
        {
            Wind = wind;
        }
    }
}