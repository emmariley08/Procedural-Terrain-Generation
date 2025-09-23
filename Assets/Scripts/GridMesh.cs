using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Unity.VisualScripting;
using UnityEngine;

// QUESTIONS
// 1. general format of unity. prefabs, etc
// 2. I want my thing to keep its color and be able to drag the numbesr and it change
// 3. should i have one temporary thing that shows up and then the generator object always? 
// 4. what are u and v used for? 




public class GridMesh : MonoBehaviour
{
    HashSet<Vector2> chunks = new HashSet<Vector2>();
    private Vector3[] verts;  // the vertices of the mesh
    private Vector2[] uvs;
    private GameObject meshObj;
    private Mesh mesh;
    private int[] tris;       // the triangles of the mesh (triplets of integer references to vertices)
    private int ntris = 0;    // the number of triangles that have been created so far
    public static int xCap = 85;
    public static int zCap = 85;
    public float perlin1 = 3f;
    public float perlin2 = .1f;
    public Material blue;
    public int texture_width = xCap + 1;
    public int texture_height = zCap + 1;
    public float scale = 10;
    public float[] ys;
    int max_plane_updown = -1;
    int max_plane_leftright = -1;
    float plane_size = 42.5f;
    public Vector3 newPosition;
    public GameObject flagPre;
    public List<Vector3> flagCoords;

    // Start is called before the first frame update
    void Start()
    {

        Camera.main.transform.Translate(45, 30, 0);
        //Vector2 begin = new Vector2(0, 0);
       //chunks.Add(begin);
        //create_new_mesh(0, 0);
    }


    void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        float dz = Input.GetAxis("Vertical");
        Vector3 cam_pos = Camera.main.transform.position;
        //print(testPos);
        int cx = Mathf.FloorToInt(cam_pos.x / 85);
        int cz = Mathf.FloorToInt(cam_pos.z / 85);
        Vector2 cxcz = new Vector2(cx, cz);
        if (chunks.Contains(cxcz))
        {
            return;
        }
        else
        {
            chunks.Add(cxcz);
            //chunks.Add(new Vector2(cx + 1, cz + 1));
            create_new_mesh(cx, cz);
        }
        print("CX: " + cx + "    CZ: " + cz);


        // if (cam_pos.z > (max_plane_updown + .5) * plane_size * 2)
        // {
        //     flagCoords.Clear();
        //     create_new_mesh(1);
        // }
        // if (cam_pos.x > (max_plane_leftright + .5) * plane_size * 2)
        // {
        //     flagCoords.Clear();
        //     create_new_mesh(2);
        // }
    }

    // 0 first
    // 1 upback 
    // 2 leftright
    void create_new_mesh(int cx, int cz)
    {
        int indexforward = max_plane_updown + 1;
        int indexright = max_plane_leftright + 1;
        // Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
        // if (dir == 1)
        // {
        //     origin = new Vector3(0.0f, 0.0f, 2 * plane_size * (indexforward + 0.0f));
        //     max_plane_updown++;
        // }
        // else if (dir == 2)
        // {
        //     origin = new Vector3(2 * plane_size * (indexright + 0.0f), 0.0f, 0.0f);
        //     max_plane_leftright++;
        // }
        // //newPosition = meshObj.transform.position;
        Vector3 origin = new Vector3(cx * 85.0f, 0, cz * 85.0f);
        newPosition = origin;
        generateMeshData();
        //generateFlags();

        // instantiate Game object and give it meshfilter, and meshrenderer
        Mesh mesh = new Mesh();
        mesh.name = "please show up";
        GameObject meshObj = new GameObject("terrain", typeof(MeshRenderer), typeof(MeshFilter));
        meshObj.GetComponent<MeshFilter>().mesh = mesh;
        //meshObj.GetComponent<MeshRenderer>().material = blue;
        //meshObj.transform.localScale = new Vector3();
        ///print(meshObj.transform.position);
        meshObj.transform.position = origin;
        //print(newPosition);
        print(origin);

        // change color of the object
        // Renderer rend = meshObj.GetComponent<Renderer>();
        // rend.material.color = new Color (5.0f, 26.0f, 1.0f, 1.0f);

        Texture2D texture = make_a_texture(ys);

        // attach the texture to the mesh
        Renderer renderer = meshObj.GetComponent<Renderer>();
        renderer.material.mainTexture = texture;

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        //meshObj.transform.position = Vector3.zero;
        //mountains.Apply();
        mesh.RecalculateNormals();
    }

    Texture2D make_a_texture(float[] yArr)
    {
        Texture2D texture = new Texture2D(texture_width, texture_height);
        Color[] colors = new Color[texture_width * texture_height];

        // create the Perlin noise pattern in "colors"
        for (int i = 0; i < texture_width; i++)
            for (int j = 0; j < texture_height; j++)
            {
                // float x = scale * i / (float) texture_width;
                // float y = scale * j / (float) texture_height;
                float r, g, b;
                if (ys[j * texture_width + i] <= 8) // water
                {
                    r = 26f / 255;
                    g = 117f / 255;
                    b = 159f / 255;
                }                   // Perlin noise!
                else if (ys[j * texture_width + i] <= 8.5)  // sand
                {
                    r = 212f / 255;
                    g = 162f / 255;
                    b = 118f / 255;
                }
                else if (ys[j * texture_width + i] <= 10) // green
                {
                    // r = 24f / 255;
                    // g = 50f / 255;
                    // b = 39f / 255;
                    r = 88f / 255;
                    g = 129f / 255;
                    b = 87f / 255;
                }
                else if (ys[j * texture_width + i] <= 14) // grey
                {
                    r = 53f / 255;
                    g = 73f / 255;
                    b = 82f / 255;
                }
                else // white
                {
                    r = 200f / 255;
                    g = 209f / 255;
                    b = 218f / 255;

                }
                colors[j * texture_width + i] = new Color(r, g, b, 1.0f);  // gray scale values (r = g = b)
            }

        // copy the colors into the texture
        texture.SetPixels(colors);

        // do texture specific stuff, probably including making the mipmap levels
        texture.Apply();

        return (texture);
    }

    private void generateMeshData()
    {

        // list of vertices of a cube
        int num_verts = (xCap + 1) * (zCap + 1);
        verts = new Vector3[num_verts];
        ntris = 0;
        List<Vector3> flagCoords = new List<Vector3>();

        int num_tris = xCap * zCap * 2;  // we need two triangles per face
        tris = new int[num_tris * 3];  // need three vertices per triangle
        uvs = new Vector2[verts.Length];
        ys = new float[num_verts];

        // creating the list of vertices
        for (int vertCount = 0, z = 0; z <= zCap; z++)
        {
            for (int x = 0; x <= xCap; x++)
            {

                float y = perlinFunc(x, z);
                ys[vertCount] = y;
                verts[vertCount] = new Vector3(x, y, z);
                if (flagCoords.Count <= 7 && y > 16)
                {
                    getFlagCoords(verts[vertCount]);
                }
                vertCount++;
            }

        }

        for (int i = 0; i < verts.Length; i++)
        {
            uvs[i] = new Vector2(verts[i].x / xCap, verts[i].z / zCap);
        }
        //mesh.vertices = verts;

        int vertsPerRow = xCap + 1;
        for (int r = 0; r < zCap; r++)
        {
            for (int c = 0; c < xCap; c++)
            {
                int BL = r * vertsPerRow + c;
                int BR = r * vertsPerRow + c + 1;
                int TL = (vertsPerRow * (r + 1)) + c;
                int TR = (vertsPerRow * (r + 1)) + c + 1;

                // MakeTri(BL, BR, TL);
                // MakeTri(BR, TR, TL);
                MakeTri(BL, TL, TR);
                MakeTri(BL, TR, BR);

            }
        }
        //mesh.triangles = tris;
    }
    public void getFlagCoords(Vector3 test)
    {
        Boolean fits = true;
        if (flagCoords.Count == 0)
        {
            flagCoords.Add(test);
        }
        else
        {
            for (int i = 0; i < flagCoords.Count; i++)
            {
                if (Math.Sqrt((Math.Pow(test.x - flagCoords[i].x, 2) + Math.Pow(test.z - flagCoords[i].z, 2))) < 10)
                {
                    fits = false;
                }
            }
            if (fits)
            {
                flagCoords.Add(test);
            }

        }
        //return Vector3.zero;
    }
    void MakeTri(int i1, int i2, int i3)
    {
        int index = ntris * 3;  // figure out the base index for storing triangle indices
        ntris++;

        tris[index] = i1;
        tris[index + 1] = i2;
        tris[index + 2] = i3;
    }

    float perlinFunc(float x, float z)
    {

        //int offsetX = UnityEngineRandom.Rand(0, 10000);
        x = (x + newPosition.x + 4598) * .8f;
        z = (z + newPosition.z + 7888) * .5f;
        float band1 = Mathf.PerlinNoise(x, z) * 2f - 1f;
        float band2 = perlin1 * Mathf.PerlinNoise(perlin2 * x, perlin2 * z);
        float band3 = perlin1 * perlin1 * Mathf.PerlinNoise(perlin2 * perlin2 * x, perlin2 * perlin2 * z);
        if ((band1 + band2 + band3) < 8)
        {
            //return .01f * (Mathf.PerlinNoise(x * 5f, z* 5f) *2f -1f) + 7;
            return 8f;
        }
        return band1 + band2 + band3;
    }

    void generateFlags()
    {
        for (int i = 0; i < flagCoords.Count; i++)
        {
            Vector3 position = flagCoords[i];
            position.x += newPosition.x;
            position.y += 1;
            position.z += newPosition.z;
            Quaternion rot = Quaternion.identity;
            Instantiate(flagPre, position, rot);
        }
        //flagCoords.Clear;
    }
}
