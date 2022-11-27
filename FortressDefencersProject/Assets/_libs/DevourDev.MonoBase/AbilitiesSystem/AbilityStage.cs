namespace DevourDev.MonoBase.AbilitiesSystem
{
    public enum AbilityStage : byte
    {
        None = 0,
        PrecastStart = 1,
        Precasting = 2,
        CastStart = 3,
        Casting = 4,
        PostcastStart = 5,
        Postcasting = 6,
        PostcastEnd = 7,
    }
}