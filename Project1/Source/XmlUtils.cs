using System.Collections.Generic;
using System.Windows.Markup;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    /// <summary>
    /// Provides methods to load common data-types or objects required for game development.
    /// </summary>
    public static class XmlUtils
    {
        public delegate GameComponent ComponentSerializer(XElement element);

        public static Dictionary<string, ComponentSerializer> ComponentSerializers { get; set; } = new Dictionary<string, ComponentSerializer>()
        {
            { "GameSprite", GameSprite.FromXml },
            { "SpriteAnimator", SpriteAnimator.FromXml },
            { "ScrollingSprite", ScrollingSprite.FromXml },
            { "TextComponent", TextComponent.FromXml },
            { "Camera", Camera.FromXml }
        };

        /// <summary>
        /// Deserializes an integer value from a specified <see cref="XAttribute"/> of an <see cref="XElement"/>.
        /// </summary>
        public static int IntFromAttribute(XElement element, string attributeName)
        {
            if (int.TryParse(element.Attribute(attributeName).Value, out int value))
                return value;
            else
                return 0;
        }

        /// <summary>
        /// Deserializes an integer value from a specified <see cref="XAttribute"/> of an <see cref="XElement"/>.
        /// </summary>
        public static uint UIntFromAttribute(XElement element, string attributeName)
        {
            if (uint.TryParse(element.Attribute(attributeName).Value, out uint value))
                return value;
            else
                return 0;
        }

        /// <summary>
        /// Deserializes an integer value from a specified <see cref="XAttribute"/> of an <see cref="XElement"/>.
        /// </summary>
        public static float FloatFromAttribute(XElement element, string attributeName)
        {
            if (float.TryParse(element.Attribute(attributeName).Value, out float value))
                return value;
            else
                return 0;
        }

        /// <summary>
        /// Returns the raw string value in a specified <see cref="XAttribute"/> of an <see cref="XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xAttribute"></param>
        /// <returns></returns>
        public static string AttributeValue(XElement element, string xAttribute)
            => element.Attribute(xAttribute).Value;

        /// <summary>
        /// Deserializes a <see cref="Point"/> struct from an XML element. Defaults to attributes name "X" and "Y".
        /// </summary>
        public static Point PointFromXml(XElement element)
            => new Point(IntFromAttribute(element, "X"), IntFromAttribute(element, "Y"));

        /// <summary>
        /// Deserializes a <see cref="Point"/> struct from an XML element, and two attribute names.
        /// </summary>
        public static Point PointFromXml(XElement element, string xAttribute, string yAttribute)
            => new Point(IntFromAttribute(element, xAttribute), IntFromAttribute(element, yAttribute));

        /// <summary>
        /// Deserializes a <see cref="Point"/> struct from an XML element.
        /// </summary>
        public static Vector2 Vec2FromXml(XElement element)
            => new Vector2(FloatFromAttribute(element, "X"), FloatFromAttribute(element, "Y"));

        /// <summary>
        /// Deserializes a <see cref="Point"/> struct from an XML element, and two attribute names.
        /// </summary>
        public static Vector2 Vec2FromXml(XElement element, string xAttribute, string yAttribute)
            => new Vector2(FloatFromAttribute(element, xAttribute), FloatFromAttribute(element, yAttribute));

        /// <summary>
        /// Deserializes a <see cref="Color"/> struct from an XML element.
        /// </summary>
        public static Color ColorFromXml(XElement element)
            => new Color(UIntFromAttribute(element, "Color"));

        /// <summary>
        /// Deserializes a <see cref="Rectangle"/> struct from an XML element.
        /// </summary>
        public static Rectangle RectFromXml(XElement element)
            => new Rectangle(PointFromXml(element), PointFromXml(element, "W", "H"));

        /// <summary>
        /// Deserializes a <see cref="Transform2D"/> object from an XML element.
        /// </summary>
        public static Transform2D TransformFromXml(XElement e)
            => new Transform2D(Vec2FromXml(e, "X", "Y"), Vec2FromXml(e, "W", "H"), Vec2FromXml(e, "oX", "oY"), 
                               FloatFromAttribute(e, "Rotation"), FloatFromAttribute(e, "Scale"));

        public static GameObject GameObjectFromXml(XElement element)
        {
            string name = element.Attribute("Name").Value;
            bool.TryParse(element.Attribute("IsActive").Value, out bool isActive);
            bool.TryParse(element.Attribute("IsVisible").Value, out bool isVisible);
            float.TryParse(element.Attribute("Alpha").Value, out float alpha);

            Transform2D transform = TransformFromXml(element.Element("Transform"));

            GameObject obj = new GameObject(name, transform) { IsActive = isActive, IsVisible = isVisible, Alpha = alpha };

            foreach(XElement elem in element.Element("Components").Elements())
                obj.AddComponent(GameComponentFromXml(elem));

            foreach (XElement elem in element.Element("Children").Elements())
                obj.AddObject(GameObjectFromXml(elem));

            return obj;
        }

        public static GameComponent GameComponentFromXml(XElement element)
        {
            string componentType = element.Attribute("Type").Value;

            return ComponentSerializers[componentType](element);
        }

        public static void LoadObjectsFromXml(XElement element, GameObjectCollection collection)
        {
            foreach (XElement elem in element.Elements())
                collection.AddGameObject(GameObjectFromXml(elem));
        }
    }
}