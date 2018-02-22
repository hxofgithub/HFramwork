
using UnityEngine.UI;

public class EmptyForRaycast : MaskableGraphic
{
    protected EmptyForRaycast()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}
