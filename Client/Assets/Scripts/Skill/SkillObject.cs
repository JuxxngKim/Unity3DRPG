using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeongJ.Inagme;

public class SkillObject : MonoBehaviour
{
    BaseActor _owner;

    public void Init (BaseActor owner)
    {
        _owner = owner;
    }
}
