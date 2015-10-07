﻿using UnityEngine;
using System.Collections.Generic;

public class CustomMesh : BlockController {

    public Vector3[] verts = new Vector3[0];
    public int[] trisUp = new int[0];
    public int[] trisDown = new int[0];
    public int[] trisNorth = new int[0];
    public int[] trisEast = new int[0];
    public int[] trisSouth = new int[0];
    public int[] trisWest = new int[0];
    public int[] trisOther = new int[0];
    public Vector2[] uvs = new Vector2[0];

    public bool isSolid;

    public TextureCollection collection;

    public string blockName;

    public override void SetUpController(BlockConfig config, World world)
    {
        blockName = config.name;
        collection = world.textureIndex.GetTextureCollection(config.textures[0]);
        isSolid = config.isSolid;
        SetUpMeshControllerMesh( world.config.meshFolder + "/" + config.meshFileName, this, new Vector3(config.meshXOffset, config.meshYOffset, config.meshZOffset));
    }

    public override string Name()
    {
        return blockName;
    }

    public override bool IsSolid(Direction direction) {
        return isSolid;
    }

    public override void AddBlockData(Chunk chunk, BlockPos localPos, BlockPos globalPos, MeshData meshData, Block block)
    {
        int initialVertCount = meshData.vertices.Count;
        int colInitialVertCount = meshData.colVertices.Count;

        foreach (var vert in verts)
        {
            meshData.AddVertex(vert + (Vector3)localPos);
            meshData.colVertices.Add(vert + (Vector3)localPos);

            if (uvs.Length == 0)
                meshData.uv.Add(new Vector2(0, 0));

            //Coloring of blocks is not yet implemented so just pass in full brightness
            meshData.colors.Add(new Color(1, 1, 1, 1));
        }

        if (uvs.Length != 0)
        {
            Rect texture;
            if (collection != null)
                texture = collection.GetTexture(chunk, localPos, Direction.down);
            else
                texture = new Rect();


            foreach (var uv in uvs)
            {
                meshData.uv.Add(new Vector2((uv.x * texture.width) + texture.x, (uv.y * texture.height) + texture.y));
            }
        }

        if (!chunk.LocalGetBlock(localPos.Add(0, 1, 0)).controller.IsSolid(Direction.down))
            foreach (var tri in trisUp)
            {
                meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
            }

        if (!chunk.LocalGetBlock(localPos.Add(0, -1, 0)).controller.IsSolid(Direction.up))
            foreach (var tri in trisDown)
            {
                meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
            }

        if (!chunk.LocalGetBlock(localPos.Add(0, 0, 1)).controller.IsSolid(Direction.south))
            foreach (var tri in trisNorth)
            {
                meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
            }

        if (!chunk.LocalGetBlock(localPos.Add(0, 0, -1)).controller.IsSolid(Direction.north))
            foreach (var tri in trisEast)
            {
                meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
            }

        if (!chunk.LocalGetBlock(localPos.Add(1, 0, 0)).controller.IsSolid(Direction.west))
            foreach (var tri in trisSouth)
            {
                meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
            }

        if (!chunk.LocalGetBlock(localPos.Add(-1, 0, 0)).controller.IsSolid(Direction.east))
            foreach (var tri in trisEast)
            {
                meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
            }

        foreach (var tri in trisOther)
        {
            meshData.AddTriangle(tri + initialVertCount);
                meshData.colTriangles.Add(tri + colInitialVertCount);
        }
    }

    public override bool IsTransparent() { return true; }


    static bool AlignedWith(Vector3 vertex, int xyOrz, float value)
    {
        float vertexValue = 0;
        if (xyOrz == 0)
        {
            vertexValue = vertex.x;
        }
        else if (xyOrz == 1)
        {
            vertexValue = vertex.y;
        }
        else if (xyOrz == 2)
        {
            vertexValue = vertex.z;
        }
        else
        {
            Debug.LogError(xyOrz + " is not the index of x, y or z");
        }

        if (vertexValue > value - 0.005f && vertexValue < value + 0.005f)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets the mesh file with the given name and adds the mesh's data to the controller
    /// </summary>
    public static void SetUpMeshControllerMesh(string meshLocation, CustomMesh controller, Vector3 positionOffset)
    {
        GameObject meshGO = (GameObject)Resources.Load(meshLocation);//(from m in meshGOs where m.name == meshName select m).First();

        for (int GOIndex = 0; GOIndex < meshGO.transform.childCount; GOIndex++)
        {
            Mesh mesh = meshGO.transform.GetChild(GOIndex).GetComponent<MeshFilter>().sharedMesh;

            List<Vector3> verts = new List<Vector3>();

            foreach (var vertex in mesh.vertices)
            {
                verts.Add(vertex + positionOffset);
            }

            List<int> trisUp = new List<int>();
            List<int> trisDown = new List<int>();
            List<int> trisNorth = new List<int>();
            List<int> trisEast = new List<int>();
            List<int> trisSouth = new List<int>();
            List<int> trisWest = new List<int>();
            List<int> trisOther = new List<int>();

            // Split the triangles into 7 groups, one for each side they
            // could be aligned with and one for unaligned triangles
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                //Triangles aligned with top side of mesh
                if (AlignedWith(verts[mesh.triangles[i]], 1, 0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 1]], 1, 0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 2]], 1, 0.5f))
                {
                    trisUp.Add(mesh.triangles[i]);
                    trisUp.Add(mesh.triangles[i + 1]);
                    trisUp.Add(mesh.triangles[i + 2]);
                }
                //Triangles aligned with top side of mesh
                else if (AlignedWith(verts[mesh.triangles[i]], 1, -0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 1]], 1, -0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 2]], 1, -0.5f))
                {
                    trisDown.Add(mesh.triangles[i]);
                    trisDown.Add(mesh.triangles[i + 1]);
                    trisDown.Add(mesh.triangles[i + 2]);
                }
                //Triangles aligned with top side of mesh
                else if (AlignedWith(verts[mesh.triangles[i]], 2, 0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 1]], 2, 0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 2]], 2, 0.5f))
                {
                    trisNorth.Add(mesh.triangles[i]);
                    trisNorth.Add(mesh.triangles[i + 1]);
                    trisNorth.Add(mesh.triangles[i + 2]);
                }
                //Triangles aligned with top side of mesh
                else if (AlignedWith(verts[mesh.triangles[i]], 0, 0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 1]], 0, 0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 2]], 0, 0.5f))
                {
                    trisEast.Add(mesh.triangles[i]);
                    trisEast.Add(mesh.triangles[i + 1]);
                    trisEast.Add(mesh.triangles[i + 2]);
                }
                //Triangles aligned with top side of mesh
                else if (AlignedWith(verts[mesh.triangles[i]], 2, -0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 1]], 2, -0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 2]], 2, -0.5f))
                {
                    trisSouth.Add(mesh.triangles[i]);
                    trisSouth.Add(mesh.triangles[i + 1]);
                    trisSouth.Add(mesh.triangles[i + 2]);
                }
                //Triangles aligned with top side of mesh
                else if (AlignedWith(verts[mesh.triangles[i]], 0, -0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 1]], 0, -0.5f)
                    && AlignedWith(verts[mesh.triangles[i + 2]], 0, -0.5f))
                {
                    trisWest.Add(mesh.triangles[i]);
                    trisWest.Add(mesh.triangles[i + 1]);
                    trisWest.Add(mesh.triangles[i + 2]);
                }
                else
                {
                    trisOther.Add(mesh.triangles[i]);
                    trisOther.Add(mesh.triangles[i + 1]);
                    trisOther.Add(mesh.triangles[i + 2]);
                }
            }

            trisUp.AddRange(controller.trisUp);
            controller.trisUp = trisUp.ToArray();
            trisDown.AddRange(controller.trisDown);
            controller.trisDown = trisDown.ToArray();
            trisNorth.AddRange(controller.trisNorth);
            controller.trisNorth = trisNorth.ToArray();
            trisEast.AddRange(controller.trisEast);
            controller.trisEast = trisEast.ToArray();
            trisSouth.AddRange(controller.trisSouth);
            controller.trisSouth = trisSouth.ToArray();
            trisWest.AddRange(controller.trisWest);
            controller.trisWest = trisWest.ToArray();
            trisOther.AddRange(controller.trisOther);
            controller.trisOther = trisOther.ToArray();

            controller.verts = mesh.vertices;
            controller.uvs = mesh.uv;

        }
    }
}
