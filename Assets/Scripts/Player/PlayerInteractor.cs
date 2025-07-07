using System;
using UnityEngine;


public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;
    public float hitDistance = 3f;
    public LayerMask interactableMask;
    private PlayerInputHandler input;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        TryInteraction();
    }
    private void TryInteraction()
    {
        // Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,hitDistance,interactableMask))
        {
            Debug.Log(hit.collider.name);
            I_Interactable interactable = hit.collider.GetComponent<I_Interactable>();
            if (interactable != null)
            {
                Debug.Log(interactable.GetInteractionText());
                if (input.InteractTriggered)
                {
                    interactable.Interact();
                }
            }
            // I_Damageable damageable = hit.collider.GetComponent<I_Damageable>();
            // if (damageable != null)
            // {
            //     Debug.DrawRay(cam.transform.position, cam.transform.forward * hitDistance, Color.red);
            //   
            //     if (Input.GetMouseButtonDown(0))
            //     {
            //         damageable.Damage(50);
            //     }
            // }
        }
    }
}
