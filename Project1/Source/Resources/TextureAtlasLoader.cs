using System;

using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CryosisEngine
{
    public class TextureAtlasLoader : ResourceLoader<TextureAtlas>
    {
        public TextureLoader TextureLoader { get; set; }

        public TextureAtlasLoader(string rootDirectory, TextureLoader textureLoader) : base(rootDirectory)
        {
            TextureLoader = textureLoader;
        }

        /// <inheritdoc/>
        protected override TextureAtlas LoadItem(string contentPath)
        {
            string metaFilePath = $"{RootDirectory}/{WorkingDirectory}/{WorkingDirectory}_meta.xml", filePath, imagePath;

            if (File.Exists(metaFilePath))
            {
                string[] args = contentPath.Split('/');
                contentPath = args[args.Length - 1];

                XDocument xml = XDocument.Load(metaFilePath);
                XElement metaElement = xml.Root.Element(contentPath);

                if (metaElement != null)
                    filePath = metaElement.Attribute("AtlasPath").Value;
                else
                    filePath = $"{contentPath}.xml";
            }
            else
                filePath = $"{contentPath}.xml";

            if (!File.Exists($"{RootDirectory}/{WorkingDirectory}/{filePath}"))
                filePath = $"DefaultAtlas.xml";

            XDocument atlasDocument = XDocument.Load($"{RootDirectory}/{WorkingDirectory}/{filePath}");
            imagePath = atlasDocument.Root.Attribute("TexturePath").Value;

            return TextureAtlas.FromXml(atlasDocument.Root, TextureLoader.LoadContent(imagePath));
        }

        protected override void UnloadItem(TextureAtlas atlas)
        {
            atlas.BaseTexture.Dispose();
        }
    }
}
