using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class player : MonoBehaviour {
    public LayerMask mask;
    public Transform hitT;
    Vector3 befRight;
    Vector3 befUp;
    Transform cameraT;
    public bool isAbsMove = false;

    // Start is called before the first frame update
    void Start() {
        befRight = transform.right;
        befUp = transform.up;
        cameraT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            isAbsMove = !isAbsMove;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Rayが衝突したかどうか
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] ps = { vertices[triangles[hit.triangleIndex * 3 + 0]]
            ,vertices[triangles[hit.triangleIndex * 3 + 1]]
            ,vertices[triangles[hit.triangleIndex * 3 + 2]]};

            Transform hitTransform = hit.collider.transform;

            for (var i = 0; i < ps.Length; i++) {
                ps[i] = hitTransform.TransformPoint(ps[i]);
            }
            Debug.DrawLine(ps[0],ps[1],Color.red);
            Debug.DrawLine(ps[1],ps[2], Color.blue);
            Debug.DrawLine(ps[0],ps[2], Color.green);

            Vector3[] ds = { ps[0] - ps[1], ps[1] - ps[2], ps[0] - ps[2] };
            var order = ds.OrderByDescending(d => d.sqrMagnitude).ToArray();


            Vector3 o1 = order[2].normalized;
            Vector3 o2 = order[1].normalized;
            Vector3 upV,rightV;

            var or1Dot = Vector3.Dot(o1, befUp) ;
            var or2Dot = Vector3.Dot(o2, befUp);
            if (Mathf.Abs(or1Dot) > Mathf.Abs(or2Dot)) {
                upV = or1Dot < 0 ? -o1 : o1;
                rightV = o2;
            }
            else {
                upV = or2Dot < 0 ? -o2 : o2;
                rightV = o1;
            }

            if (Vector3.Dot(rightV, befRight) < 0) {
                rightV = -rightV;
            }
            befRight = rightV;
            befUp = upV;

            if (isAbsMove) {
                upV = Vector3.up;
                rightV = Vector3.right;
            }
            //hitT.position = hit.point;
            var dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) {
                dir += upV;
            }
            if (Input.GetKey(KeyCode.A)) {
                dir += -rightV;
            }
            if (Input.GetKey(KeyCode.S)) {
                dir += -upV;

            }
            if (Input.GetKey(KeyCode.D)) {
                dir += rightV;

            }

            transform.position = hit.point - transform.forward * 0.1f;

            if (isAbsMove) {
                dir = Vector3.ProjectOnPlane(dir, hit.normal).normalized;
            }
                if (dir != Vector3.zero) {
                Debug.Log(dir);
                transform.position += dir / 50;
                var look = Vector3.Slerp(transform.position + transform.forward, transform.position - hit.normal * 5, 0.01f);

                transform.LookAt(look);
            }

            //transform.position = hit.point - transform.forward * 5;
            //if (dir != Vector3.zero) {
            //    var onPlane = Vector3.ProjectOnPlane(dir, hit.normal);
            //    transform.position += onPlane / 50;
            //    var look = Vector3.Slerp(transform.position + transform.forward, transform.position - hit.normal * 5, 0.01f);
            //    transform.LookAt(look);
            //}
        }
    }
}
