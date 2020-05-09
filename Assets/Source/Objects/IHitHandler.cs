namespace Assets.Source.Objects
{
    /// <summary>
    /// Objects that can take hit from aliens
    /// </summary>
    public interface IHitHandler
    {
        /// <summary>
        /// Takes a hit
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="buildingDamage"></param>
        /// <returns>True if need to bounce</returns>
        bool TakeHit(int damage, int buildingDamage);
    }
}
