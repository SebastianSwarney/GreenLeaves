using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class FindNearestPointOnMesh : MonoBehaviour
{
	public GameObject target;
	public Vector3 pt;

	public Transform intersection;

	private void Start()
	{

	}


	void OnDrawGizmos()
	{
		//Gizmos.color = Color.white;
		//Gizmos.DrawSphere(pt, .1f);

		MeshFilter mf = target.GetComponent<MeshFilter>();

		/*
		Mesh mesh = mf.sharedMesh;
		VertTriList vt = new VertTriList(mesh);
		Vector3 objSpacePt = target.transform.InverseTransformPoint(pt);
		Vector3[] verts = mesh.vertices;
		KDTree kd = KDTree.MakeFromPoints(verts);
		Vector3 meshPt = NearestPointOnMesh(objSpacePt, verts, kd, mesh.triangles, vt);
		Vector3 worldPt = target.transform.TransformPoint(meshPt);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(worldPt, .1f);
		*/


		FindIntersection(mf);
	}

	private void FindIntersection(MeshFilter mf)
	{
		Mesh mesh = mf.sharedMesh;

		Vector3 avePos = Vector3.zero;
		int aveCount = 0;

		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			Vector3 p1 = mesh.vertices[mesh.triangles[i + 0]];
			Vector3 p2 = mesh.vertices[mesh.triangles[i + 1]];
			Vector3 p3 = mesh.vertices[mesh.triangles[i + 2]];



			Triangle newTri = new Triangle(p1, p2, p3);

			IntersectionResult result = new IntersectionResult();

			EzySlice.Plane cuttingPlane = new EzySlice.Plane();

			Vector3 refUp = transform.InverseTransformDirection(intersection.up);
			Vector3 refPt = transform.InverseTransformPoint(intersection.position);

			cuttingPlane.Compute(refPt, refUp);

			Intersector.Intersect(cuttingPlane, newTri, result);

			if (result.isValid)
			{
				//DebugExtension.DebugWireSphere(transform.position + p1, Color.blue, 0.1f);
				//DebugExtension.DebugWireSphere(transform.position + p2, Color.blue, 0.1f);
				//DebugExtension.DebugWireSphere(transform.position + p3, Color.blue, 0.1f);

				for (int i2 = 0; i2 < result.intersectionPoints.Length; i2++)
				{
					avePos += transform.position + result.intersectionPoints[i2];
					aveCount++;
				}
			}
		}

		avePos /= aveCount;

		DebugExtension.DebugWireSphere(avePos, Color.blue, 0.1f);
	}

	public Vector3 NearestPointOnTri(Vector3 pt, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 edge1 = b - a;
		Vector3 edge2 = c - a;
		Vector3 edge3 = c - b;
		float edge1Len = edge1.magnitude;
		float edge2Len = edge2.magnitude;
		float edge3Len = edge3.magnitude;

		Vector3 ptLineA = pt - a;
		Vector3 ptLineB = pt - b;
		Vector3 ptLineC = pt - c;
		Vector3 xAxis = edge1 / edge1Len;
		Vector3 zAxis = Vector3.Cross(edge1, edge2).normalized;
		Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

		Vector3 edge1Cross = Vector3.Cross(edge1, ptLineA);
		Vector3 edge2Cross = Vector3.Cross(edge2, -ptLineC);
		Vector3 edge3Cross = Vector3.Cross(edge3, ptLineB);
		bool edge1On = Vector3.Dot(edge1Cross, zAxis) > 0f;
		bool edge2On = Vector3.Dot(edge2Cross, zAxis) > 0f;
		bool edge3On = Vector3.Dot(edge3Cross, zAxis) > 0f;

		//	If the point is inside the triangle then return its coordinate.
		if (edge1On && edge2On && edge3On)
		{
			float xExtent = Vector3.Dot(ptLineA, xAxis);
			float yExtent = Vector3.Dot(ptLineA, yAxis);
			return a + xAxis * xExtent + yAxis * yExtent;
		}

		//	Otherwise, the nearest point is somewhere along one of the edges.
		Vector3 edge1Norm = xAxis;
		Vector3 edge2Norm = edge2.normalized;
		Vector3 edge3Norm = edge3.normalized;

		float edge1Ext = Mathf.Clamp(Vector3.Dot(edge1Norm, ptLineA), 0f, edge1Len);
		float edge2Ext = Mathf.Clamp(Vector3.Dot(edge2Norm, ptLineA), 0f, edge2Len);
		float edge3Ext = Mathf.Clamp(Vector3.Dot(edge3Norm, ptLineB), 0f, edge3Len);

		Vector3 edge1Pt = a + edge1Ext * edge1Norm;
		Vector3 edge2Pt = a + edge2Ext * edge2Norm;
		Vector3 edge3Pt = b + edge3Ext * edge3Norm;

		float sqDist1 = (pt - edge1Pt).sqrMagnitude;
		float sqDist2 = (pt - edge2Pt).sqrMagnitude;
		float sqDist3 = (pt - edge3Pt).sqrMagnitude;

		if (sqDist1 < sqDist2)
		{
			if (sqDist1 < sqDist3)
			{
				return edge1Pt;
			}
			else
			{
				return edge3Pt;
			}
		}
		else if (sqDist2 < sqDist3)
		{
			return edge2Pt;
		}
		else
		{
			return edge3Pt;
		}
	}
}
