using System;
using System.Collections.Generic;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace EcconiaCPUServerComponents.Client
{
	public static class KeycapLab
	{
		//Meshes:
		private static readonly Mesh cornerMesh;
		private static readonly Mesh bridgeMesh;
		private static readonly Mesh edgeMesh;
		
		private static readonly Dictionary<int, Mesh> meshCache = new Dictionary<int, Mesh>();
		
		static KeycapLab()
		{
			cornerMesh = new Mesh();
			bridgeMesh = new Mesh();
			edgeMesh = new Mesh();
			createCorner(cornerMesh);
			createEdge(edgeMesh);
			createBridge(bridgeMesh);
		}
		
		private static void createCorner(Mesh mesh)
		{
			List<Vector3> vertexList = new List<Vector3>();
			List<Vector3> vertexNormalList = new List<Vector3>();
			List<int> triangleList = new List<int>();
			{
				int amountOfCornerPoints = 15;
				float angle = 90f / (float) amountOfCornerPoints;
				Quaternion rotation = Quaternion.Euler(0, angle, 0);
				Vector3 normal = new Vector3(-1, 0, 0);
				
				//Top middle vertices:
				vertexList.Add(new Vector3(0, 0, 0)); //0 Middle
				vertexNormalList.Add(Vector3.up);
				
				//First:
				vertexList.Add(new Vector3(-1f, 0, 0)); //1 Top
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(-1f, 0, 0)); //2 Side top
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(-1f, -1, 0)); //3 Side bottom
				vertexNormalList.Add(normal);
				
				//Second:
				vertexList.Add(rotation * vertexList[1]); //4 Top
				vertexNormalList.Add(Vector3.up);
				normal = rotation * normal;
				vertexList.Add(rotation * vertexList[2]); //5 Side top
				vertexNormalList.Add(normal);
				vertexList.Add(rotation * vertexList[3]); //6 Side bottom
				vertexNormalList.Add(normal);
				
				//First top triangle:
				triangleList.Add(0);
				triangleList.Add(1);
				triangleList.Add(4);
				//First bottom triangle:
				triangleList.Add(2);
				triangleList.Add(3);
				triangleList.Add(6);
				//Second bottom triangle:
				triangleList.Add(2);
				triangleList.Add(6);
				triangleList.Add(5);
				
				int vertexListSize = vertexList.Count;
				for(int i = 0; i < amountOfCornerPoints - 1; i++)
				{
					vertexList.Add(rotation * vertexList[vertexListSize - 3]); //Top
					vertexNormalList.Add(Vector3.up);
					normal = rotation * normal;
					vertexList.Add(rotation * vertexList[vertexListSize - 2]); //Side top
					vertexNormalList.Add(normal);
					vertexList.Add(rotation * vertexList[vertexListSize - 1]); //Side bottom
					vertexNormalList.Add(normal);
					
					//Top:
					triangleList.Add(0);
					triangleList.Add(vertexListSize - 3); //Skip 2 new values and take top 
					triangleList.Add(vertexListSize); //Skip new bottom
					//Bottom first:
					triangleList.Add(vertexListSize - 2);
					triangleList.Add(vertexListSize - 1);
					triangleList.Add(vertexListSize + 2);
					//Bottom second:
					triangleList.Add(vertexListSize - 2);
					triangleList.Add(vertexListSize + 2);
					triangleList.Add(vertexListSize + 1);
					
					vertexListSize += 3; //Inc for next top/bottom.
				}
			}
			mesh.vertices = vertexList.ToArray();
			mesh.triangles = triangleList.ToArray();
			mesh.normals = vertexNormalList.ToArray();
		}
		
		private static void createEdge(Mesh mesh)
		{
			List<Vector3> vertexList = new List<Vector3>();
			List<Vector3> vertexNormalList = new List<Vector3>();
			List<int> triangleList = new List<int>();
			{
				//Top:
				vertexList.Add(new Vector3(0, 0, 0)); //0
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(0, 0, 1)); //1
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(1, 0, 1)); //2
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(1, 0, 0)); //3
				vertexNormalList.Add(Vector3.up);
				triangleList.Add(0);
				triangleList.Add(1);
				triangleList.Add(2);
				triangleList.Add(0);
				triangleList.Add(2);
				triangleList.Add(3);
				//Side:
				Vector3 normal = new Vector3(-1, 0, 0);
				vertexList.Add(new Vector3(0, 0, 0)); //4
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(0, -1, 0)); //5
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(0, -1, 1)); //6
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(0, 0, 1)); //7
				vertexNormalList.Add(normal);
				triangleList.Add(4 + 0);
				triangleList.Add(4 + 1);
				triangleList.Add(4 + 2);
				triangleList.Add(4 + 0);
				triangleList.Add(4 + 2);
				triangleList.Add(4 + 3);
			}
			mesh.vertices = vertexList.ToArray();
			mesh.triangles = triangleList.ToArray();
			mesh.normals = vertexNormalList.ToArray();
		}
		
		private static void createBridge(Mesh mesh)
		{
			List<Vector3> vertexList = new List<Vector3>();
			List<Vector3> vertexNormalList = new List<Vector3>();
			List<int> triangleList = new List<int>();
			{
				//Top:
				vertexList.Add(new Vector3(0, 0, 0)); //0
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(0, 0, 1)); //1
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(1, 0, 1)); //2
				vertexNormalList.Add(Vector3.up);
				vertexList.Add(new Vector3(1, 0, 0)); //3
				vertexNormalList.Add(Vector3.up);
				triangleList.Add(0);
				triangleList.Add(1);
				triangleList.Add(2);
				triangleList.Add(0);
				triangleList.Add(2);
				triangleList.Add(3);
				//Left:
				Vector3 normal = new Vector3(-1, 0, 0);
				vertexList.Add(new Vector3(0, 0, 0)); //4
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(0, -1, 0)); //5
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(0, -1, 1)); //6
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(0, 0, 1)); //7
				vertexNormalList.Add(normal);
				triangleList.Add(4 + 0);
				triangleList.Add(4 + 1);
				triangleList.Add(4 + 2);
				triangleList.Add(4 + 0);
				triangleList.Add(4 + 2);
				triangleList.Add(4 + 3);
				//Right:
				normal = new Vector3(1, 0, 0);
				vertexList.Add(new Vector3(1, 0, 0)); //4
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(1, -1, 0)); //5
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(1, -1, 1)); //6
				vertexNormalList.Add(normal);
				vertexList.Add(new Vector3(1, 0, 1)); //7
				vertexNormalList.Add(normal);
				triangleList.Add(8 + 0);
				triangleList.Add(8 + 2);
				triangleList.Add(8 + 1);
				triangleList.Add(8 + 0);
				triangleList.Add(8 + 3);
				triangleList.Add(8 + 2);
			}
			mesh.vertices = vertexList.ToArray();
			mesh.triangles = triangleList.ToArray();
			mesh.normals = vertexNormalList.ToArray();
		}
		
		public static Mesh getKeycapMeshFor(int x, int z)
		{
			if(x > 10 || z > 10 || x < 0 || z < 0)
			{
				throw new Exception("Keycap has illegal size, must be clamped between 0 and 10, got: " + x + ", " + z);
			}
			int key = x * 10 + z;
			Mesh mesh;
			meshCache.TryGetValue(key, out mesh);
			if(mesh == null)
			{
				mesh = createMeshFor(x, z);
				meshCache[key] = mesh;
			}
			return mesh;
		}
		
		private static Mesh createMeshFor(int sideX, int sideZ)
		{
			const float unit = 0.3f; //Constant of one square unit.
			float x = (sideX - 1) * unit;
			float z = (sideZ - 1) * unit;
			
			const float inset = unit / 20f;
			const float radius = unit / 5f;
			const float depth = unit * 0.2f;
			
			const float side = unit - inset;
			const float sideHalf = side / 2f;
			const float outset = sideHalf - radius;
			
			Vector3 cornerScaler = new Vector3(radius, depth, radius);
			
			CombineInstance[] combine = new CombineInstance[]
			{
				//Corners:
				new CombineInstance()
				{
					mesh = cornerMesh,
					transform = transform(
						cornerScaler,
						Quaternion.identity,
						new Vector3(-outset, 0, outset + z)
					),
				},
				new CombineInstance()
				{
					mesh = cornerMesh,
					transform = transform(
						cornerScaler,
						Quaternion.AngleAxis(90, Vector3.up),
						new Vector3(outset + x, 0, outset + z)
					),
				},
				new CombineInstance()
				{
					mesh = cornerMesh,
					transform = transform(
						cornerScaler,
						Quaternion.AngleAxis(90 * 2, Vector3.up),
						new Vector3(outset + x, 0, -outset)
					),
				},
				new CombineInstance()
				{
					mesh = cornerMesh,
					transform = transform(
						cornerScaler,
						Quaternion.AngleAxis(90 * 3, Vector3.up),
						new Vector3(-outset, 0, -outset)
					),
				},
				//Edges:
				new CombineInstance()
				{
					mesh = edgeMesh,
					transform = transform(
						new Vector3(radius, depth, 2 * outset + z),
						Quaternion.identity,
						new Vector3(-sideHalf, 0, -outset)
					),
				},
				new CombineInstance()
				{
					mesh = edgeMesh,
					transform = transform(
						new Vector3(radius, depth, 2 * outset + z),
						Quaternion.AngleAxis(180, Vector3.up),
						new Vector3(sideHalf + x, 0, outset + z)
					),
				},
				//Bridge:
				new CombineInstance()
				{
					mesh = bridgeMesh,
					transform = transform(
						new Vector3(side + z, depth, 2 * outset + x), //Swapped size offset, because 90° rotation.
						Quaternion.AngleAxis(90, Vector3.up),
						new Vector3(-outset, 0, sideHalf + z)
					),
				},
			};
			
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(combine);
			return mesh;
		}
		
		private static Matrix4x4 transform(Vector3 scale, Quaternion rotation, Vector3 position)
		{
			Matrix4x4 pos = Matrix4x4.Translate(position);
			Matrix4x4 sca = Matrix4x4.Scale(scale);
			Matrix4x4 rot = Matrix4x4.Rotate(rotation);
			return pos * rot * sca;
		}
	}
}
