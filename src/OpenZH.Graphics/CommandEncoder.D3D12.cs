using SharpDX.Direct3D12;
using SharpDX.Mathematics.Interop;
using OpenZH.Graphics.Platforms.Direct3D12;
using System;

namespace OpenZH.Graphics
{
    partial class CommandEncoder
    {
        private readonly GraphicsCommandList _commandList;
        private readonly RenderPassDescriptor _renderPassDescriptor;

        private GraphicsPipelineState _currentPipelineState;

        internal CommandEncoder(GraphicsDevice graphicsDevice, GraphicsCommandList commandList, RenderPassDescriptor renderPassDescriptor)
            : base(graphicsDevice)
        {
            _commandList = commandList;
            _renderPassDescriptor = renderPassDescriptor;

            _renderPassDescriptor.OnOpenedCommandList(_commandList);
        }

        private void PlatformClose()
        {
            _renderPassDescriptor.OnClosingCommandList(_commandList);

            // Don't close _commandList. We'll close it in D3D12CommandBuffer.Commit.
        }

        private void PlatformDrawIndexed(
            PrimitiveType primitiveType,
            uint indexCount,
            IndexType indexType,
            Buffer indexBuffer,
            uint indexBufferOffset)
        {
            _commandList.PrimitiveTopology = primitiveType.ToPrimitiveTopology();

            _commandList.SetIndexBuffer(new IndexBufferView
            {
                BufferLocation = indexBuffer.DeviceBuffer.GPUVirtualAddress,
                Format = indexType.ToDxgiFormat(),
                SizeInBytes = (int) indexBuffer.DeviceBuffer.Description.Width
            });

            _commandList.DrawIndexedInstanced(
                (int) indexCount,
                1,
                (int) indexBufferOffset,
                0,
                0);
        }

        private void PlatformSetPipelineState(GraphicsPipelineState pipelineState)
        {
            _commandList.PipelineState = pipelineState.DevicePipelineState;
            _currentPipelineState = pipelineState;
        }

        private void PlatformSetRootSignature(RootSignature rootSignature)
        {
            _commandList.SetGraphicsRootSignature(rootSignature.DeviceRootSignature);
        }

        private void PlatformSetVertexBuffer(int slot, Buffer vertexBuffer)
        {
            if (_currentPipelineState == null)
            {
                throw new InvalidOperationException("Must call SetPipelineState before SetVertexBuffer");
            }

            _commandList.SetVertexBuffer(slot, new VertexBufferView
            {
                BufferLocation = vertexBuffer.DeviceBuffer.GPUVirtualAddress,
                SizeInBytes = (int) vertexBuffer.DeviceBuffer.Description.Width,
                StrideInBytes = _currentPipelineState.Descriptor.VertexDescriptor.GetStride(slot)
            });
        }

        private void PlatformSetViewport(Viewport viewport)
        {
            _commandList.SetViewport(viewport.ToViewportF());
            _commandList.SetScissorRectangles(new RawRectangle(0, 0, viewport.Width, viewport.Height));
        }
    }
}
