using TMPro;
using UnityEngine;

public class ComponentDescription : MonoBehaviour
{
    // Instância estática para acesso fácil
    public static ComponentDescription Instance;

    [Header("UI Elements")]
    public GameObject popupPanel; // O GameObject pai do popup
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Transform statsContainer; // Pai dos Textos de Stats
    public TextMeshProUGUI statsPrefab; // O Prefab do texto que irá exibir cada stat

    public Vector2 offset = new Vector3(0, -50);

    private bool m_isDragging;

    // Variável para armazenar o último objeto de stat criado
    private TextMeshProUGUI currentStatsText;

    private void Awake()
    {
        // Garante que só há uma instância
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Começa escondido
        HidePopup();
    }

    /// <summary>
    /// Exibe o popup com os dados fornecidos e o posiciona.
    /// </summary>
    /// <param name="data">O ComponentSO contendo os dados.</param>
    /// <param name="position">A posição da tela onde o popup deve aparecer.</param>
    public void ShowPopup(ComponentSO data, Vector3 position)
    {
        if (m_isDragging) return;
        // 1. Atualizar o conteúdo
        nameText.text = data.ComponentName;
        descriptionText.text = data.Description;

        // 2. Limpar estatísticas anteriores
        ClearStats();

        // 3. Gerar novas estatísticas
        foreach (WeaponStats stat in data.weaponStats)
        {
            // Instancia o prefab de texto para cada stat
            TextMeshProUGUI statEntry = Instantiate(statsPrefab, statsContainer);

            // Cria a string de descrição: Ex: "+10% Velocidade de Ataque"
            string statValueString = FormatStatValue(stat.m_WeaponAttribute, stat.m_Value);
            string statDescription = stat.m_Description;

            statEntry.text = $"{statValueString} - {statDescription}";
        }

        // 4. Posicionar e exibir o painel
        popupPanel.transform.position = new Vector3(position.x + offset.x, position.y + offset.y, 0);
        popupPanel.SetActive(true);
    }

    /// <summary>
    /// Esconde o popup.
    /// </summary>
    public void HidePopup()
    {
        popupPanel.SetActive(false);
        // Limpar os textos de stats quando o popup for escondido (liberar memória)
        ClearStats();
    }

    /// <summary>
    /// Remove todos os textos de estatísticas antigos do container.
    /// </summary>
    private void ClearStats()
    {
        for (int i = statsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(statsContainer.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Formata o valor da estatística baseado no seu atributo (%, flat, etc.).
    /// </summary>
    private string FormatStatValue(WeaponAttribute attribute, float value)
    {
        string sign = value > 0 ? "+" : "";

        switch (attribute)
        {
            // Atributos de Multiplicador (porcentagem)
            case WeaponAttribute.SpeedMultiplier:
            case WeaponAttribute.LifetimeMultiplier:
                return $"{sign}{(value * 100f):F0}%"; // Ex: +10%

            // Atributos de Taxa (inverso, geralmente Tempo)
            case WeaponAttribute.RechargeTime:
            case WeaponAttribute.FireDelay:
                if (value < 0) return $"-{Mathf.Abs(value):F2}s"; // Redução de tempo
                else return $"+{value:F2}s"; // Aumento de tempo

            // Atributos Flat (valor bruto)
            case WeaponAttribute.Damage:
            case WeaponAttribute.SpeedFlat:
            case WeaponAttribute.FlatLifeTime:
            case WeaponAttribute.MaxAmmo:
            default:
                return $"{sign}{value:F1}"; // Ex: +5.0
        }
    }

    public void SetDragging(bool isDragging)
    {
        m_isDragging = isDragging;

    }
}