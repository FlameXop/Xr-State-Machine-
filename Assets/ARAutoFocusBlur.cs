using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ARAutoFocusBlur : MonoBehaviour
{
    public Transform arCamera;
    private DepthOfField dof;
    private Transform targetCharacter;

    void Start()
    {
        Volume volume = GetComponent<Volume>();
        if (volume != null && volume.profile.TryGet(out dof))
        {
            // Keep blur off while scanning the floor so user can see clearly
            dof.active = false;
        }
    }

    void Update()
    {
        if (targetCharacter == null || dof == null || arCamera == null) return;

        // Calculate live distance from phone camera to the character
        float distToCharacter = Vector3.Distance(arCamera.position, targetCharacter.position);

        // Start blurring right behind the character!
        dof.gaussianStart.value = distToCharacter + 0.3f;
        dof.gaussianEnd.value = distToCharacter + 1.5f;
    }

    // Call this from SpawnCharacter() in CharacterStateManager
    public void EnableFocusOnCharacter(Transform spawnedChar)
    {
        targetCharacter = spawnedChar;
        if (dof != null)
        {
            dof.active = true;
        }
    }
}