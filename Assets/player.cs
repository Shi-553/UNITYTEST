using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class MeshRaycast {
    public class PointDistance {
        public Vector3 d, nomalD, p1, p2;
        public bool isChange ;

        public PointDistance(Vector3 p1, Vector3 p2) {
            Init(p1, p2);
        }
        public void Init(Vector3 p1, Vector3 p2) {
            this.d = p1 - p2;
            nomalD = d.normalized;
            this.p1 = p1;
            this.p2 = p2;
            isChange = false;
        }
        public void DotAdjust(Vector3 v) {
            if (Vector3.Dot(nomalD, v) < 0) {
                d = -d;
                nomalD = -nomalD;
                var t = p1;
                p1 = p2;
                p2 = t;
                isChange = !isChange;
            }
        }
    };

    public MeshRaycast() {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="up"></param>
    /// <param name="right"></param>
    /// <returns>0=底辺 1=高さ 2=斜辺</returns>
    public PointDistance[] GetUpRight(RaycastHit hit, Vector3 up, Vector3 right) {
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (meshCollider == null || meshCollider.sharedMesh == null) {
            return new PointDistance[] { };
        }

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

        PointDistance[] ds = { new PointDistance(ps[0], ps[1]), new PointDistance(ps[1], ps[2]), new PointDistance(ps[0], ps[2]) };
        var order = ds.OrderBy(d => d.d.sqrMagnitude).ToArray();


        order[0].DotAdjust(right);
        order[1].DotAdjust(up);

        Debug.DrawLine(order[0].p1, order[0].p2, Color.red);
        Debug.DrawLine(order[1].p1, order[1].p2, Color.blue);
        Debug.DrawLine(order[2].p1, order[2].p2, Color.green);
        return order;
    }
};

public class player : MonoBehaviour {
    public LayerMask mask;
    public Transform hitT;
    MeshRaycast forwardMeshRaycast, topMeshRaycast, downMeshRaycast;
    Transform cameraT;
    public bool isAbsMove = false;

    public float speed = 1;
    void Start() {
        forwardMeshRaycast = new MeshRaycast();
        topMeshRaycast = new MeshRaycast();
        downMeshRaycast = new MeshRaycast();

        cameraT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            isAbsMove = !isAbsMove;
        }
        //if (frame > 0) {
        //    //transform.position = Vector3.Slerp(sPos,toPos,frame/30.0f);
        //    if (frame > 30) {
        //        frame = 0;
        //    }
        //    return;
        //}

        Ray forwerdRay = new Ray(transform.position, transform.forward);
        Ray bottomRay = new Ray(transform.position, -transform.up);

        if (Physics.Raycast(forwerdRay, out var forwerdHit, Mathf.Infinity, mask)&&
            Physics.Raycast(bottomRay, out var bottomHit, Mathf.Infinity, mask)) {

            var forwardPoints = forwardMeshRaycast.GetUpRight(forwerdHit, transform.up, transform.right);

            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) {
                bool isQ = Input.GetKeyDown(KeyCode.Q);
                transform.position = forwerdHit.point - transform.forward * 0.1f;

                Ray ray = new Ray(transform.position, isQ ? transform.up : -transform.up);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, mask)) {

                    var points = topMeshRaycast.GetUpRight(hit, isQ ? -transform.forward : transform.forward, transform.right);

                    var toP1Sqr = (forwardPoints[1].p1 - transform.position).magnitude;
                    var toP2Sqr = (forwardPoints[1].p2 - transform.position).magnitude;
                    var ratio = toP1Sqr / (toP1Sqr + toP2Sqr);

                    //sPos = transform.position;
                    //toPos = transform.position + points[1].d * ratio * (isQ ? 1 : -1);

                    transform.position += points[1].d * (isQ ? 1 - ratio : -(ratio));
                    //Debug.Log(points[1].d * ratio * (isQ ? -1 : 1));
                   
                    //var angle = Vector3.SignedAngle(forwardPoints[1].nomalD, points[1].nomalD, forwardPoints[0].nomalD);
                    ////angle = isQ ? 90 : -90;
                    //transform.Rotate(forwardPoints[0].nomalD, angle);

                    transform.LookAt(transform.position - hit.normal, points[1].nomalD);


                    //transform.position = sPos;
                    return;
                }
            }



            Vector3 upFV, rightFV;
            if (isAbsMove) {
                upFV = cameraT.up;
                rightFV = cameraT.right;
            }
            else {
                upFV = forwardPoints[1].nomalD;
                rightFV = forwardPoints[0].nomalD;
            }

            //hitT.position = hit.point;
            var dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) {
                dir += upFV;
            }
            if (Input.GetKey(KeyCode.A)) {
                dir += -rightFV;
            }
            if (Input.GetKey(KeyCode.S)) {
                dir += -upFV;

            }
            if (Input.GetKey(KeyCode.D)) {
                dir += rightFV;

            }
            dir.Normalize();

            transform.position =Vector3.Slerp(transform.position, forwerdHit.point - transform.forward * 0.1f,0.01f);

            if (isAbsMove) {
                dir = Vector3.ProjectOnPlane(dir, forwerdHit.normal).normalized;
            }
            if (dir != Vector3.zero) {
                // Debug.Log(dir);
                transform.position += dir* speed / 50;
            }
                var look = Vector3.Slerp(transform.position + transform.forward, transform.position - forwerdHit.normal , 0.01f);

                transform.LookAt(look, bottomHit.normal);

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
