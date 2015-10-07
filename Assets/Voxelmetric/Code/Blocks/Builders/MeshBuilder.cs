﻿using System.Collections;
using UnityEngine;

public class MeshBuilder {

    public static void CrossMeshRenderer(Chunk chunk, BlockPos localPos, BlockPos globalPos, MeshData meshData, TextureCollection texture, Block block)
    {
        float halfBlock = (Config.Env.BlockSize / 2) + Config.Env.BlockFacePadding;
        float colliderOffest = 0.05f * Config.Env.BlockSize;

        //Using the block positions hash is much better for random numbers than saving the offset and height in the block data
        int hash = localPos.GetHashCode();
        if (hash < 0)
            hash *= -1;

        float blockHeight = halfBlock * 2 * (hash % 100) / 100f;

        hash *= 39;
        if (hash < 0)
            hash *= -1;

        float offsetX = (halfBlock * (hash % 100) / 100f) - (halfBlock / 2);

        hash *= 39;
        if (hash < 0)
            hash *= -1;

        float offsetZ = (halfBlock * (hash % 100) / 100f)-(halfBlock / 2);

        //Converting the position to a vector adjusts it based on block size and gives us real world coordinates for x, y and z
        Vector3 vPos = localPos;
        //Vector3 vPos = (pos - chunk.pos);
        Vector3 vPosCollider = localPos;
        vPos += new Vector3(offsetX, 0, offsetZ);

        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock, vPos.z + halfBlock));
        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock + blockHeight, vPos.z + halfBlock));
        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock + blockHeight, vPos.z - halfBlock));
        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock, vPos.z - halfBlock));
        meshData.AddQuadTriangles();
        BlockBuilder.BuildTexture(chunk, localPos, globalPos, meshData, Direction.north, texture);
        meshData.AddColors(1, 1, 1, 1, 1);

        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock, vPos.z - halfBlock));
        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock + blockHeight, vPos.z - halfBlock));
        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock + blockHeight, vPos.z + halfBlock));
        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock, vPos.z + halfBlock));
        meshData.AddQuadTriangles();
        BlockBuilder.BuildTexture(chunk, localPos, globalPos, meshData, Direction.north, texture);
        meshData.AddColors(1, 1, 1, 1, 1);

        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock, vPos.z + halfBlock));
        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock + blockHeight, vPos.z + halfBlock));
        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock + blockHeight, vPos.z - halfBlock));
        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock, vPos.z - halfBlock));
        meshData.AddQuadTriangles();
        BlockBuilder.BuildTexture(chunk, localPos, globalPos, meshData, Direction.north, texture);
        meshData.AddColors(1, 1, 1, 1, 1);

        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock, vPos.z - halfBlock));
        meshData.AddVertex(new Vector3(vPos.x - halfBlock, vPos.y - halfBlock + blockHeight, vPos.z - halfBlock));
        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock + blockHeight, vPos.z + halfBlock));
        meshData.AddVertex(new Vector3(vPos.x + halfBlock, vPos.y - halfBlock, vPos.z + halfBlock));
        meshData.AddQuadTriangles();
        BlockBuilder.BuildTexture(chunk, localPos, globalPos, meshData, Direction.north, texture);
        meshData.AddColors(1, 1, 1, 1, 1);

        meshData.AddVertex(new Vector3(vPosCollider.x - halfBlock, vPosCollider.y - halfBlock + colliderOffest, vPosCollider.z + halfBlock), collisionMesh: true);
        meshData.AddVertex(new Vector3(vPosCollider.x + halfBlock, vPosCollider.y - halfBlock + colliderOffest, vPosCollider.z + halfBlock), collisionMesh: true);
        meshData.AddVertex(new Vector3(vPosCollider.x + halfBlock, vPosCollider.y - halfBlock + colliderOffest, vPosCollider.z - halfBlock), collisionMesh: true);
        meshData.AddVertex(new Vector3(vPosCollider.x - halfBlock, vPosCollider.y - halfBlock + colliderOffest, vPosCollider.z - halfBlock), collisionMesh: true);
        meshData.AddQuadTriangles(collisionMesh:true);
    }
}
