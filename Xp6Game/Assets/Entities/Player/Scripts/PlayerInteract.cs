using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public int interactRadius = 4;
    StarterAssetsInputs _playerInput;

    void Start()
    {
#if ENABLE_INPUT_SYSTEM 
        _playerInput = GetComponent<StarterAssetsInputs>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput.interact)
        {
            CastInteract();
        }

    }

    void CastInteract()
    {
        var hit = Physics.OverlapSphere(transform.position, 4);

        if (hit == null)
            return;

        foreach (var obj in hit)
        {
            if (obj.TryGetComponent<Interactable>(out Interactable comp))
            {
                if (!comp.CanInteract())
                    continue;

                comp.Interact();
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        //Sphere Gizmos to Interact Radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);       
    }


}
