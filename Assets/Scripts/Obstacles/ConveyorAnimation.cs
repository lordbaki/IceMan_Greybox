
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ConveyorAnimation : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0f, 1f);

    private Material beltMaterial;
    private Vector2 offset;

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Material[] materials = meshRenderer.sharedMaterials;

        // Animate only the belt material.
        beltMaterial = new Material(materials[1]);
        materials[1] = beltMaterial;
        meshRenderer.sharedMaterials = materials;

        offset = beltMaterial.GetTextureOffset("_BaseMap");
    }

    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;

        offset.x = Mathf.Repeat(offset.x, 1f);
        offset.y = Mathf.Repeat(offset.y, 1f);

        beltMaterial.SetTextureOffset("_BaseMap", offset);
    }

    void OnDestroy()
    {
        if (beltMaterial != null)
            Destroy(beltMaterial);
    }
}
