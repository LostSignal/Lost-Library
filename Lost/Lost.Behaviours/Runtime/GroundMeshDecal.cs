//-----------------------------------------------------------------------
// <copyright file="GroundMeshDecal.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{    using UnityEngine;    [ExecuteInEditMode]    [RequireComponent(typeof(MeshFilter))]    [RequireComponent(typeof(MeshRenderer))]    public class GroundMeshDecal : MonoBehaviour    {
        #pragma warning disable 0649        [Range(1, 5)]        [SerializeField] private int quadCount = 2;        [SerializeField] private float groundOffset = 0.02f;        [SerializeField] private MeshFilter meshFilter;        [SerializeField] private int layerMask;
        #pragma warning restore 0649

        // Variables used to determine if we need to update the mesh
        private Matrix4x4 oldMatrix;        private int oldQuadCount;        private float oldGroundOffset;        public void UpdateMeshVerticies()        {            Vector3[] verts = this.meshFilter.sharedMesh.vertices;            for (int i = 0; i < verts.Length; i++)            {                Vector3 worldSpaceVert = this.transform.localToWorldMatrix.MultiplyPoint(verts[i]);

                // Resetting the y to the position so we can re-project
                worldSpaceVert = worldSpaceVert.SetY(this.transform.position.y);                Ray ray = new Ray(worldSpaceVert, Vector3.down);                RaycastHit hit;                if (Physics.Raycast(ray, out hit, float.MaxValue, this.layerMask))                {                    worldSpaceVert = hit.point.AddToY(this.groundOffset);                }                verts[i] = this.transform.worldToLocalMatrix.MultiplyPoint(worldSpaceVert);            }            this.meshFilter.sharedMesh.vertices = verts;        }        private void OnValidate()        {            this.AssertGetComponent<MeshFilter>(ref this.meshFilter);        }        private void OnEnable()        {            this.OnValidate();            this.UpdateMesh();        }        private void UpdateMesh()        {
            // Making sure the mesh is created and set to the sharedMesh
            if (this.meshFilter.sharedMesh == null)            {                this.meshFilter.sharedMesh = new Mesh();

                // This makes sure they're never saved out and don't dirty up the scene every time the scene loads
                this.meshFilter.sharedMesh.hideFlags = HideFlags.HideAndDontSave;            }            Mesh mesh = this.meshFilter.sharedMesh;            int vertCount = (this.quadCount + 1) * (this.quadCount + 1);            int triCount = (this.quadCount * this.quadCount) * 2;            if (mesh.vertexCount == vertCount)            {
                // No need to update the mesh
                return;            }

            // If we got here then we need to recalculate all our meshes verts/tris/uvs/normals
            mesh.Clear();

            // 3 ------- 2
            //   |   / |
            //   |  /  |
            //   | /   |
            //   |/    |
            // 0 ------- 1

            Vector3[] vertices = new Vector3[vertCount];            Vector2[] uvs = new Vector2[vertCount];            Vector3[] normals = new Vector3[vertCount];            int[] tris = new int[triCount * 3];

            // Initializing the verts, uvs and normals
            for (int i = 0; i < vertCount; i++)            {                int xIndex = i % (this.quadCount + 1);                int yIndex = i / (this.quadCount + 1);                float x = (float)xIndex / (float)this.quadCount;                float y = (float)yIndex / (float)this.quadCount;                vertices[i] = new Vector3(x, 0, y);                uvs[i] = new Vector2(x, y);                normals[i] = Vector3.up;            }

            // Making sure the mesh is centered about the origin
            for (int i = 0; i < vertCount; i++)            {                vertices[i] = vertices[i] + new Vector3(-0.5f, 0, -0.5f);            }

            // Setting up the triangles
            int triIndex = 0;            for (int i = 0; i < this.quadCount; i++)            {                for (int j = 0; j < this.quadCount; j++)                {                    int startIndex = i + ((this.quadCount + 1) * j);                    tris[triIndex++] = startIndex;                    tris[triIndex++] = startIndex + (this.quadCount + 2);                    tris[triIndex++] = startIndex + 1;                    tris[triIndex++] = startIndex;                    tris[triIndex++] = startIndex + (this.quadCount + 1);                    tris[triIndex++] = startIndex + (this.quadCount + 2);                }            }            mesh.vertices = vertices;            mesh.uv = uvs;            mesh.normals = normals;            mesh.triangles = tris;            this.UpdateMeshVerticies();        }        private void Update()        {
            // If it moved then update the verticies
            if (this.transform.localToWorldMatrix != oldMatrix || this.oldGroundOffset != this.groundOffset)            {                this.UpdateMeshVerticies();                this.oldMatrix = this.transform.localToWorldMatrix;                this.oldGroundOffset = this.groundOffset;            }

            // Checking to see if the quad count changed and we must update the mesh
            if (this.quadCount != this.oldQuadCount)            {                this.UpdateMesh();                this.oldQuadCount = this.quadCount;            }        }    }}