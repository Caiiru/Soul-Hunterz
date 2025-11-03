using System.Collections.Generic;

/// <summary>
/// Contém os dados para a criação e modificação de uma bala.
/// Os componentes da arma modificam uma instância deste payload antes do disparo.
/// </summary>
public class BulletPayload
{
    // Modificadores de Dano
    public float BonusDamage { get; set; } = 0;

    // Modificadores de Comportamento de Disparo
    public int BulletCount { get; set; } = 1;
    public float SpreadDistance { get; set; } = 0f;

    // Modificadores de Atributos da Bala
    public float SpeedMultiplier { get; set; } = 1.0f;
    public float LifetimeMultiplier { get; set; } = 1.0f;

    public List<ComponentSO> UpdatePayload = new List<ComponentSO>();
}

