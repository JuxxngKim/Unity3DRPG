using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    private ObjModel _level;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        //Managers.Map.LoadMap(1);

        Screen.SetResolution(640, 480, false);

        _level = new ObjModel("Assets/NavMesh.obj");

        //GameObject player = Managers.Resource.Instantiate("Creature/Player");
        //player.name = "Player";
        //Managers.Object.Add(player);
        

        //Managers.UI.ShowSceneUI<UI_Inven>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        ////Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);
    }

    public override void Clear()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_level == null || _level.IsVaild == false)
            return;
        if (_level.Triangles == null)
            return;

        int index = 0;

        foreach(var triangle in _level.Triangles)
        {
            switch (index)
            {
                case 0: Gizmos.color = Color.red; break;
                case 1: Gizmos.color = Color.green; break;
                case 2: Gizmos.color = Color.yellow; break;
            }
            
            Gizmos.DrawLine(sVectorToVector(triangle.A), sVectorToVector(triangle.B));
            Gizmos.DrawLine(sVectorToVector(triangle.B), sVectorToVector(triangle.C));
            Gizmos.DrawLine(sVectorToVector(triangle.C), sVectorToVector(triangle.A));

            Gizmos.

            if (index++ == 2)
                index = 0;
        }
    }

    private Vector3 sVectorToVector(SharpNav.Geometry.Vector3 form)
    {
        return new Vector3(form.X, form.Y, form.Z);
    }


#endif
}
