using UnityEngine;

public class PetController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatFrequency = 1.0f;

    private GameObject _instance;
    private Animator _animator;

    void OnEnable()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnPetEquipped += SpawnPet;
            if (PlayerInventory.Instance.EquippedPet != null)
                SpawnPet(PlayerInventory.Instance.EquippedPet);
        }
    }

    void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnPetEquipped -= SpawnPet;
    }

    void SpawnPet(PetData pet)
    {
        Debug.Log($"[PetController] SpawnPet llamado. pet={pet?.displayName ?? "NULL"} prefab={pet?.petPrefab?.name ?? "NULL"} spawnPoint={spawnPoint?.name ?? "NULL"}");
        if (_instance != null) Destroy(_instance);
        if (pet == null) { Debug.LogWarning("[PetController] pet es NULL"); return; }
        if (pet.petPrefab == null) { Debug.LogWarning($"[PetController] petPrefab de '{pet.displayName}' es NULL"); return; }
        if (spawnPoint == null) { Debug.LogWarning("[PetController] spawnPoint no asignado"); return; }
        _instance = Instantiate(pet.petPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        Debug.Log($"[PetController] Instanciado en {spawnPoint.position}. Objeto: {_instance.name}");
        _animator = _instance.GetComponentInChildren<Animator>();
        if (_animator != null && pet.animatorController != null)
            _animator.runtimeAnimatorController = pet.animatorController;
        if (_animator != null) _animator.SetBool("IsEquipped", true);
    }

    void Update()
    {
        if (_instance == null) return;
        float y = Mathf.Sin(Time.time * floatFrequency * Mathf.PI * 2f) * floatAmplitude;
        _instance.transform.localPosition = new Vector3(0f, y, 0f);
    }
}
