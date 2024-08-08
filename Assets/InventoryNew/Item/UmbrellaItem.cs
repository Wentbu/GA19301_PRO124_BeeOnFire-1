using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Umbrella")]
public class UmbrellaItem : Item
{
    public GameObject umbrellaPrefab;
    private PrefabUmbrella prefabUmbrella;

    public override void Use(PlayerControl playerController)
    {
        if (prefabUmbrella == null)
        {
            prefabUmbrella = FindObjectOfType<PrefabUmbrella>();
        }

        if (prefabUmbrella != null)
        {
            prefabUmbrella.AttachUmbrellaToPlayer(playerController.gameObject);
        }
    }
}
