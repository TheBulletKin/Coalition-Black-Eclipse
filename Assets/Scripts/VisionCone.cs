using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCone : MonoBehaviour
{
	public float range = 10f;
	public float angle = 45f;
	public int resolution = 6;  // Higher value = smoother edge

	private void Start()
	{
		//GenerateMesh();
	}

	private void GenerateMesh()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();

		// Number of vertices based on resolution
		int vertexCount = resolution + 2;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		// Center point
		vertices[0] = Vector3.zero;

		float angleStep = angle / (resolution - 1);

		for (int i = 0; i <= resolution; i++)
		{
			float currentAngle = -angle / 2f + angleStep * i;
			float radian = currentAngle * Mathf.Deg2Rad;
			vertices[i + 1] = new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian)) * range;
		}

		// Define triangles
		for (int i = 0; i < resolution; i++)
		{
			triangles[i * 3] = 0;
			triangles[i * 3 + 1] = i + 1;
			triangles[i * 3 + 2] = i + 2;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}

	public void UpdateMesh(float newRange, float newAngle)
	{
		range = newRange;
		angle = newAngle;
		GenerateMesh();
	}
}
