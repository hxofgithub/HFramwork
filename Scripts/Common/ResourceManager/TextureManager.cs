

public class TextureManager : ResourceManager<UnityEngine.Sprite, TextureManager>
{
    protected override void OnInit(int maxZeroRef)
    {
        base.OnInit(9);
    }

    protected override bool OnProcessUnLoadRes(UnityEngine.Sprite res, string abName, string resName)
    {
        return true;
    }


}
