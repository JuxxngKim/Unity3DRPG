using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;
    public PositionInfo2 PosInfo;
    public StatInfo Stat;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var result = GetClickPosition();
            Debug.LogError($"result : {result.Item2}");
        }
    }

    private (bool, Vector3) GetClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
        {
            return (true, hit.point);
        }

        return (true, Vector3.zero);
    }

    public void SyncPos()
    {
        //Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
    }
}
