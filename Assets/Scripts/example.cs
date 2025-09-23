using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class example : MonoBehaviour {

	private Vector3[] verts;  // the vertices of the mesh
	private int[] tris;       // the triangles of the mesh (triplets of integer references to vertices)
	private int ntris = 0;    // the number of triangles that have been created so far

	// Create the mesh and put several copies of it into the scene.

	void Start() {

		// call the routine that makes a cube (the mesh) from scratch
		Mesh my_mesh = CreateMyMesh();

		// make several copies of this mesh and place these copies in the scene

		float k = 10.0f;
		int num_objects = 20;

		for (int i = 0; i < num_objects; i++) {

			// create a new GameObject and give it a MeshFilter and a MeshRenderer
			GameObject s = new GameObject(i.ToString("Object 0"));
			s.AddComponent<MeshFilter>();
			s.AddComponent<MeshRenderer>();

			float t = i / (float) num_objects;    // t in range [0,1]
			float x = k * (2 * t - 1);            // x translate in range of [-k,k]
			float y = 1.0f;
			float z = 0.0f;
			s.transform.position = new Vector3 (x, y, z);  // move this object to a new location
			s.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);  // shrink the object

			// associate the mesh with this object
			s.GetComponent<MeshFilter>().mesh = my_mesh;

			// change the color of the object
			Renderer rend = s.GetComponent<Renderer>();
			rend.material.color = new Color (0.4f, 0.8f, 0.4f, 1.0f);  // light green color
		}
	
	}

	// Create a cube that is centered at the origin (0, 0, 0) with sides of length = 2.
	//
	// Although the faces of a cube share corners, we cannot share these vertices
	// because that would mess up the surface normals at the vertices.

	Mesh CreateMyMesh() {
		
		// create a mesh object
		Mesh mesh = new Mesh();

		// list of vertices of a cube
		int num_verts = 24;
		verts = new Vector3[num_verts];

		// vertices for faces of the cube
		// (notice multiple copies of the same vertex to make the cube have sharp edges)

		verts[0] = new Vector3 ( 1, -1, -1);
		verts[1] = new Vector3 ( 1, -1,  1);
		verts[2] = new Vector3 (-1, -1,  1);
		verts[3] = new Vector3 (-1, -1, -1);

		verts[4] = new Vector3 (-1,  1, -1);
		verts[5] = new Vector3 (-1,  1,  1);
		verts[6] = new Vector3 ( 1,  1,  1);
		verts[7] = new Vector3 ( 1,  1, -1);

		verts[8] = new Vector3 (-1,  1,  1);
		verts[9] = new Vector3 (-1,  1, -1);
		verts[10] = new Vector3 (-1, -1, -1);
		verts[11] = new Vector3 (-1, -1,  1);

		verts[12] = new Vector3 ( 1,  1,  1);
		verts[13] = new Vector3 (-1,  1,  1);
		verts[14] = new Vector3 (-1, -1,  1);
		verts[15] = new Vector3 ( 1, -1,  1);

		verts[16] = new Vector3 ( 1,  1, -1);
		verts[17] = new Vector3 ( 1,  1,  1);
		verts[18] = new Vector3 ( 1, -1,  1);
		verts[19] = new Vector3 ( 1, -1, -1);

		verts[20] = new Vector3 (-1,  1, -1);
		verts[21] = new Vector3 ( 1,  1, -1);
		verts[22] = new Vector3 ( 1, -1, -1);
		verts[23] = new Vector3 (-1, -1, -1);

		// the squares that make up the cube faces

		int num_tris = 12;  // we need two triangles per face
		tris = new int[num_tris * 3];  // need three vertices per triangle

		MakeQuad (0, 1, 2, 3);
		MakeQuad (4, 5, 6, 7);
		MakeQuad (8, 9, 10, 11);
		MakeQuad (12, 13, 14, 15);
		MakeQuad (16, 17, 18, 19);
		MakeQuad (20, 21, 22, 23);

		// save the vertices and the triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateNormals();  // automatically calculate the vertex normals

		return (mesh);
	}

	// make a triangle from three vertex indices (clockwise order)
	void MakeTri(int i1, int i2, int i3) {
		int index = ntris * 3;  // figure out the base index for storing triangle indices
		ntris++;

		tris[index]     = i1;
		tris[index + 1] = i2;
		tris[index + 2] = i3;
	}

	// make a quadrilateral from four vertex indices (clockwise order)
	void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri (i1, i2, i3);
		MakeTri (i1, i3, i4);
	}

	// Update is called once per frame (in this case we don't need to do anything)
	void Update () {
	}
}
