using System.IO;

using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    public class TextureLoader : ResourceLoader<Texture2D>
    {
        public GraphicsDevice GraphicsDevice { get; set; }

        public TextureLoader(string rootDirectory, GraphicsDevice graphicsDevice) : base(rootDirectory)
        {
            GraphicsDevice = graphicsDevice;
        }

        protected override Texture2D LoadItem(string contentPath)
        {
            string filePath = $"{RootDirectory}/{contentPath}";

            if (!File.Exists(filePath))
                return null;

            return Texture2D.FromFile(GraphicsDevice, filePath);
        }

        protected override void UnloadItem(Texture2D value)
            => value?.Dispose();
    }
}