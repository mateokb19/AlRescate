using UnityEngine;

public class PetController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float petScale = 0.25f;
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatFrequency = 1.0f;

    private GameObject _instance;
    private Animator _animator;

    void Start()
    {
        Debug.Log($"[PetController] Start. PlayerInventory.Instance={(PlayerInventory.Instance == null ? "NULL" : "OK")}");
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnPetEquipped += SpawnPet;
            Debug.Log("[PetController] Suscrito a OnPetEquipped.");
            if (PlayerInventory.Instance.EquippedPet != null)
                SpawnPet(PlayerInventory.Instance.EquippedPet);
        }
        else Debug.LogWarning("[PetController] PlayerInventory.Instance es NULL en Start.");
    }

    void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnPetEquipped -= SpawnPet;
    }

    void SpawnPet(PetData pet)
    {
        if (_instance != null) Destroy(_instance);
        if (pet == null) { Debug.LogWarning("[PetController] pet es NULL"); return; }
        if (!pet.petPrefab) { Debug.LogWarning($"[PetController] petPrefab de '{pet.displayName}' no asignado en el Inspector."); return; }
        if (spawnPoint == null) { Debug.LogWarning("[PetController] spawnPoint no asignado"); return; }
        _instance = Instantiate(pet.petPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        _instance.transform.localScale = Vector3.one * petScale;
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
