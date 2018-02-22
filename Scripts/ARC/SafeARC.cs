using System;
using System.Collections.Generic;

public class SafeARC : IARC
{
    public int RetainCount
    {
        get
        {
            return _reatinList.Count;
        }
    }

    public bool Release(object owner)
    {
        lock (_lockObj)
        {
            for (int i = 0; i < _reatinList.Count; i++)
            {
                if (_reatinList[i].Target.Equals(owner))
                {
                    _reatinList.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    public bool Retain(object owner)
    {
        lock (_lockObj)
        {
            for (int i = 0; i < _reatinList.Count; i++)
            {
                if (_reatinList[i].Target.Equals(owner))
                {
                    Log.Error("Handle error! Already Retained by:" + owner);
                    return false;
                }                    
            }
            _reatinList.Add(new WeakReference(owner, false));
        }
        return true;
    }

    public void RemoveAllNullRefrence()
    {
        lock (_lockObj)
        {
            for (int i = _reatinList.Count - 1; i >= 0; i--)
            {
                if (_reatinList[i].Target == null)
                    _reatinList.RemoveAt(i);
            }
        }        
    }

    private object _lockObj = new object();
    private List<WeakReference> _reatinList = new List<WeakReference>();
}
