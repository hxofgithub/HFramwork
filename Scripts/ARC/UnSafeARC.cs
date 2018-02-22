
public class UnSafeARC : IARC
{
    public int RetainCount
    {
        get
        {
            return _retainCount;
        }
    }

    public bool Release(object owner)
    {
        _retainCount--;
        return true;
    }

    public bool Retain(object owner)
    {
        _retainCount++;
        return true;
    }

    private int _retainCount = 0;
}
