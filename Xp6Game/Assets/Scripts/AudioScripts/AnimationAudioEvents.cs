using UnityEngine;
using FMODUnity;

public class AnimationSoundEvents : MonoBehaviour
{
    [System.Serializable]
    public struct AnimationEventSound 
    {
        public string eventName;                 // só pra identificar no inspector
        public EventReference soundEvent;        // referência do evento FMOD
    }

    [SerializeField] private AnimationEventSound[] sounds; // lista de sons disponíveis

    // Função genérica pra ser chamada pelo Animation Event
    public void PlaySound(string eventName)
    {
        foreach (var sound in sounds)
        {
            if (sound.eventName == eventName)
            {
                RuntimeManager.PlayOneShot(sound.soundEvent, transform.position);
                Debug.Log($"{eventName} triggered!");
                return;
            }
        }

       // Debug.LogWarning($"No sound found for event: {eventName}");
    }
}
