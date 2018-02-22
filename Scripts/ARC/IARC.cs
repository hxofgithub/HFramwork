
/// <summary>
/// Automatic Refrence Counter
/// </summary>
public interface IARC
{
    int RetainCount { get; }
    bool Retain(object owner);
    bool Release(object owner);
}
