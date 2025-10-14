using UnityEngine;

[CreateAssetMenu(fileName = "New Multiple Bullet", menuName = "Components/MultipleBullet")]
public class MultipleBulletComponentSO : ComponentSO
{
    [SerializeField] SpawnBulletBehaviour behaviour;
    [SerializeField] int bulletCount = 2;
    [SerializeField] float distanceOffset = 1f;

    public override void Execute(GameObject target, Transform firePoint, int slotindex)
{ 
    // 1. Calcular a largura total do espalhamento.
    // Ex: 3 balas, offset de 1. Largura = (3-1) * 1 = 2.
    float totalWidth = (bulletCount - 1) * distanceOffset;

    // 2. Calcular o deslocamento inicial para centralizar.
    // O ponto de partida para o primeiro item (i=0) deve ser movido para trás pela metade da largura total.
    float initialOffset = -totalWidth / 2f;

    // O vetor de direção para o espalhamento (geralmente firePoint.right para spread horizontal).
    // Se quiser espalhar em outra direção (ex: para frente/trás), use firePoint.forward.
    Vector3 spreadDirection = firePoint.right;

    // O objeto de bala que será usado (referência para o Instantiate ou o próprio target).
    GameObject bulletToUse;
    
    // Looping por todas as 'bulletCount' balas.
    for (int i = 0; i < bulletCount; i++)
    {
        // 3. Calcular o deslocamento total para a bala 'i' (é um valor float).
        // Deslocamento = Deslocamento Inicial + (i * distância entre as balas)
        float currentOffset = initialOffset + (i * distanceOffset);

        // 4. Calcular a posição final usando o vetor de direção do espalhamento.
        // centerPosition + (Direção de Espalhamento * Magnitude do Deslocamento)
        Vector3 finalPosition = firePoint.position + (spreadDirection * currentOffset);

        // 5. Decide se usa o target original ou instancia um novo.
        if (i == bulletCount - 1)
        {
            // Reutiliza o objeto 'target' original para economizar uma instância.
            bulletToUse = target;
        }
        else
        {
            // Instancia uma nova bala.
            bulletToUse = Instantiate(target, firePoint.position, firePoint.rotation);
        }

        // 6. Aplica a posição e a rotação.
        bulletToUse.transform.position = finalPosition;
        bulletToUse.transform.forward = firePoint.forward; // Mantém a direção de tiro do firePoint.
        
        // Opcional: Chama a inicialização da bala, se houver um script 'Bullet'.
        if (bulletToUse.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.Initialize();
        }
    }
}

}
enum SpawnBulletBehaviour
{
    Parallel,
    WithAngle,

}


