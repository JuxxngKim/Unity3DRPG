using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayer : MonoBehaviour
{
    [SerializeField] float _speed;

    private Vector3 _targetDir;
    private Vector3 _targetPosition;

    private Triangle _currentTri;

    private void Start()
    {
        _targetDir = Vector3.zero;
        _targetPosition = this.transform.position;
    }

    public void InitMap(ObjModel level)
    {
        Vector3 myPosition = this.transform.position;
        myPosition.y = 0.0f;

        Debug.LogError($"myPosition : {myPosition}");

        var triangles = level.Triangles;
        for(int i = 0; i < triangles.Count; ++i)
        {
            Triangle triangle = triangles[i];
            bool inSide = CalcOnTriangle(triangle.Vertices, myPosition);
            if (inSide)
            {
                Debug.LogError($"==========================");
                Debug.LogError($"nav : {i}");
                Debug.LogError($"A : {triangle.Vertices[0]}");
                Debug.LogError($"B : {triangle.Vertices[1]}");
                Debug.LogError($"C : {triangle.Vertices[2]}");
                _currentTri = triangle;
                //break;
            }
        }

        if(_currentTri == null)
        {
            Debug.LogError("Outside Navmesh!");
        }
    }

    private bool CalcOnTriangle(Vector3[] vertices, Vector3 point)
    {
        Vector3 a = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);
        Vector3 b = Vector3.Cross(vertices[1] - point, vertices[2] - point);
        Vector3 c = Vector3.Cross(vertices[2] - point, vertices[0] - point);
        Vector3 d = Vector3.Cross(vertices[0] - point, vertices[1] - point);

        float temp = b.magnitude + c.magnitude + d.magnitude;
        int r = (int)temp;
        float t = (int)a.magnitude;
        if (r == t)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var result = GetClickPosition();
            if (!result.isValid)
                return;

            _targetDir = result.position - this.transform.position;
            _targetDir.y = 0.0f;
            _targetPosition = result.position;
            _targetPosition.y = 0.0f;
        }

        UpdateMove();
    }

    private void UpdateMove()
    {
        if (_targetPosition == this.transform.position)
        {
            _targetDir = Vector3.zero;
            return;
        }

        this.transform.position = Vector3.MoveTowards(this.transform.position, _targetPosition, Time.deltaTime * _speed);
    }

    private (bool isValid, Vector3 position) GetClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
        {
            return (true, hit.point);
        }

        return (true, Vector3.zero);
    }
}
