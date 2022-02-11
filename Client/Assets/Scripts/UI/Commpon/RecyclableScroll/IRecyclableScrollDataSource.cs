using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public interface IRecyclableScrollDataSource
    {
        void ProvideData(Transform transform, int idx);
    }
}