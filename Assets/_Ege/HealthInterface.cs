public interface IHaveHealth
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    public void TakeDamage(int damageAmount);

    public void Heal(int healAmount);

    public void Dead();
}

public interface IHaveStamina
    {
    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public void DecraseStamina(int decreaseAmount);
    public void IncreaseStamina(int increaseAmount);
}
