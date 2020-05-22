using System.ComponentModel;

namespace Assets.Source.Objects
{
    public enum BuildableType
    {
        BuildingBase,
        Building,
    }

    /// <summary>
    /// Objects that can be built or repaired
    /// </summary>
    public interface IBuildable
    {
        BuildableType Type();

        float GetX();

        bool Build(float progress);
    }
}
