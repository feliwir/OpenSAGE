﻿using System.Numerics;
using ShaderGen;
using static OpenSage.Graphics.Shaders.CommonShaderHelpers;

namespace OpenSage.Graphics.Shaders
{
    public static class MeshShaderHelpers
    {
        public struct VertexInput
        {
            [PositionSemantic] public Vector3 Position;

            [NormalSemantic] public Vector3 Normal;
            [TextureCoordinateSemantic] public Vector3 Tangent;
            [TextureCoordinateSemantic] public Vector3 Binormal;

            [TextureCoordinateSemantic] public uint BoneIndex;
            [TextureCoordinateSemantic] public uint BoneIndex2;
            [TextureCoordinateSemantic] public float BoneWeight;
            [TextureCoordinateSemantic] public float BoneWeight2;

            [TextureCoordinateSemantic] public Vector2 UV0;
            [TextureCoordinateSemantic] public Vector2 UV1;
        }

        public struct MeshConstants
        {
            public /* bool */ uint SkinningEnabled;
            public /* bool */ uint WeightsEnabled;
            public uint NumBones;
        }

        public struct RenderItemConstantsVS
        {
            public Matrix4x4 World;
        }

        public static void GetSkinnedVertexData(
            ref VertexInput input,
            Matrix4x4 skinning)
        {
            input.Position = Vector3.Transform(input.Position, skinning);
            input.Normal = TransformNormal(input.Normal, skinning);
        }

        public static void GetSkinnedVertexDataWithWeights(
            ref VertexInput input,
            Matrix4x4 skinning1,
            Matrix4x4 skinning2)
        {
            float weight1 = input.BoneWeight;
            float weight2 = input.BoneWeight2;

            Matrix4x4 combined = (skinning1 * weight1) + (skinning2 * weight2);

            input.Position = Vector3.Transform(input.Position, combined);
            input.Normal = TransformNormal(input.Normal, combined);
        }

        public static void VSSkinnedInstancedPositionOnly(
            VertexInput input,
            out Vector4 position,
            out Vector3 worldPosition,
            Matrix4x4 world,
            Matrix4x4 viewProjection)
        {
            var worldPositionHomogeneous = Vector4.Transform(input.Position, world);

            position = Vector4.Transform(worldPositionHomogeneous, viewProjection);

            worldPosition = worldPositionHomogeneous.XYZ();
        }

        public static void VSSkinnedInstanced(
            VertexInput input,
            out Vector4 position,
            out Vector3 worldPosition,
            out Vector3 worldNormal,
            out Vector2 cloudUV,
            Matrix4x4 world,
            Matrix4x4 viewProjection,
            Matrix4x4 cloudShadowMatrix,
            float timeInSeconds)
        {
            VSSkinnedInstancedPositionOnly(
                input,
                out position,
                out worldPosition,
                world,
                viewProjection);

            worldNormal = TransformNormal(input.Normal, world);

            cloudUV = CloudHelpers.GetCloudUV(
                worldPosition,
                cloudShadowMatrix,
                timeInSeconds);
        }
    }
}
