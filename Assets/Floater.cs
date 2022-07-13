using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {
    public MeshFilter meshFilter;
    Mesh mesh;
    public GameObject gameObject;

    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    Vector3[] vertexArray;  
    Vector3[] vertexArrayStart;

    public Vector4 waveA; 
    public Vector4 waveB; 
    public Vector4 waveC; 
    public float myTime;

    void Start() {
        vertexArray = new Vector3[100*100];
        mesh = meshFilter.mesh;
        vertexArrayStart = mesh.vertices;
        vertexArray = mesh.vertices;
        // mesh.vertices = vertexArray;
    }

    private void FixedUpdate() {
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        myTime = Time.timeSinceLevelLoad;
        Shader.SetGlobalFloat("_CustomTime", myTime);
        Shader.SetGlobalVector("_WaveA", waveA);
        Shader.SetGlobalVector("_WaveB", waveB);
        Shader.SetGlobalVector("_WaveC", waveC);
        // print(vertexArray[7]);
        GetHeight(vertexArray[241]);
        Wave(241);
        rigidBody.MovePosition(new Vector3(vertexArray[241].x, vertexArray[241].y, vertexArray[241].z));
        // gameObject.transform.position = vertexArray[0]- new Vector3(-100f, 0, -100f);
        // if (transform.position.y < vertexArray[0][1]) {
        //     float displacementMultiplier = Mathf.Clamp01(-transform.position.y / depthBeforeSubmerged) * displacementAmount;
        //     rigidBody.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), ForceMode.Acceleration);
        // }
    }

    private Vector3 GerstnerWave(Vector4 wave, Vector3 p) {
        float steepness = wave[2];
        float wavelength = wave[3];
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(9.8f / k);
        Vector2 d = new Vector2(wave[0], wave[1]);
        d.Normalize();
        Vector2 pxz = new Vector2(p.x, p.z);
        float f = k * (Vector2.Dot(d, pxz) - c * myTime);
        float a = steepness / k;
        return new Vector3(
            // 0,
            d[0] * (a * Mathf.Cos(f)),
            a * Mathf.Sin(f),
            d[1] * (a * Mathf.Cos(f))
            // 0
        );

    }

    private void Wave(int i) {
        Vector3 gridPoint = vertexArray[i];
        Vector3 p = gridPoint;
        Vector3 newP = GerstnerWave(waveA, gridPoint); 
        p = new Vector3(vertexArrayStart[i].x + newP.x, newP.y, vertexArrayStart[i].z + newP.z); 
        // p += GerstnerWave(waveB, gridPoint);
        // p += GerstnerWave(waveC, gridPoint);
        // p = new Vector3(vertexArrayStart[i].x + newP.x, p.y + newP.y, vertexArrayStart[i].z + newP.z); 
        // p = newP;
        // p += Gers0tnerWave(waveC, gridPoint);
        vertexArray[i] = p;
    }

    private void GetHeight(Vector3 position) {
        // position.x = position.x - vertexArrayStart[8].x;
        // // print(position.x);
        // float steepness = waveA[2];
        // float wavelength = waveA[3];
        // float k = 2 * Mathf.PI / wavelength;
        // float c = Mathf.Sqrt(9.8f / k);
        // Vector2 d = new Vector2(waveA[0], waveA[1]);
        // d.Normalize();
        // float a = steepness / k;
        // float f = Mathf.Acos((position.x / d[0]) / a);
        // float y = Mathf.Sin(f); 

        // // print(f);
        // print(y);
        // return y;
        // // position = new Vector3((position.x / d[0]) / a, position.y, (position.z / d[1]) / a)
        float i;
        print(new Vector2(position.x + 50, position.z + 50));
        if (position.z % 2 == 0) {
            i = 2*(position.x + 50) + 101*(position.z + 50);
        } else {
            i = 2*(position.x + 50) + 101*((position.z + 50) - 1) + 1;
        }

        // float i = (position.x + 50) * (position.z + 50 + 1) + (position.z + 50);
        print(i);

    }
}
