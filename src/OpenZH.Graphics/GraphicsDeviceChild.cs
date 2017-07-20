﻿namespace OpenZH.Graphics
{
    public abstract class GraphicsDeviceChild : GraphicsObject
    {
        public GraphicsDevice GraphicsDevice { get; }

        protected GraphicsDeviceChild(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
