[System.Serializable]
public struct CharacterStats
{
    private int maxLifePoints;
    public int baseLifePoints;
    private int maxAttackDamage;
    public int baseAttackDamage;

    public void Reset()
    {
        baseLifePoints = maxLifePoints;
        baseAttackDamage = maxAttackDamage;
    }

    public void Initialize()
    {
        maxAttackDamage = baseLifePoints;
        maxLifePoints = baseLifePoints;
    }
}
