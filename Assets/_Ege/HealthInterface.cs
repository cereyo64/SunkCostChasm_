public interface IHaveHealth
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    public void TakeDamage(int damageAmount);

    public void Heal(int healAmount);

    public void Dead();
}
