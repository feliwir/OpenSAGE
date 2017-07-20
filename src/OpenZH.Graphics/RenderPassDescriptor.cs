﻿namespace OpenZH.Graphics
{
    public sealed partial class RenderPassDescriptor
    {
        public RenderPassDescriptor()
        {
            PlatformConstruct();
        }

        public void SetRenderTargetDescriptor(RenderTarget renderTargetView, LoadAction loadAction, ColorRgba clearColor = default(ColorRgba))
        {
            PlatformSetRenderTargetDescriptor(renderTargetView, loadAction, clearColor);
        }
    }
}
