using System;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    /// <summary>
    /// Defines an object's spacial properties such as position, size, and rotation within 2D space.
    /// </summary>
    public class Transform2D
    {
        Transform2D _parent;

        /// <summary>
        /// Defines the parent to base <see cref="GlobalPosition"/>, <see cref="GlobalRotation"/>, and <see cref="GlobalScale"/> values from.
        /// </summary>
        public Transform2D Parent
        {
            get => _parent;

            set
            {
                Vector2 position = GlobalPosition;
                Vector2 dimensions = GlobalDimensions;
                Vector2 origin = GlobalOrigin;
                float rotation = GlobalRotation;
                float scale = GlobalScale;

                _parent = value;

                GlobalPosition = position;
                GlobalDimensions = dimensions;
                GlobalOrigin = origin;
                GlobalRotation = rotation;
                GlobalScale = scale;
            }
        }

        /// <summary>
        /// Defines local position of the object in 2D space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Defines base dimensions of the object in 2D space.
        /// </summary>
        public Vector2 Dimensions { get; set; }

        /// <summary>
        /// Defines the central origin of the object in 2D space.
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Defines the local rotation of the object in 2D space.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Defines the local scale of the object in 2D space.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Determines the object's global position given it's chain of parents. Uses local position if no parent exists.
        /// </summary>
        public Vector2 GlobalPosition
        {
            get => Parent == null ? Position : ((CryosisMath.Rotate(Position, Parent.GlobalRotation) * Parent.GlobalScale) + Parent.GlobalPosition);

            set => Position += CryosisMath.Rotate(value - GlobalPosition, -GlobalRotation);
        }

        /// <summary>
        /// Determines the object's global rotation given it's chain of parents. Uses local rotation if no parent exists.
        /// </summary>
        public float GlobalRotation
        {
            get => Parent == null ? Rotation : (Rotation + Parent.GlobalRotation);

            set => Rotation += (value - GlobalRotation);
        }

        /// <summary>
        /// Determines the object's global scale given it's chain of parents. Uses local scale if no parent exists.
        /// </summary>
        public float GlobalScale
        {
            get => Parent == null ? Scale : (Scale * Parent.GlobalScale);

            set => Scale = (value * Scale) / GlobalScale;
        }

        /// <summary>
        /// Determines the object's dimensions given global scale factors.
        /// </summary>
        public Vector2 GlobalDimensions
        {
            get => Dimensions * GlobalScale;

            set => Dimensions = value / GlobalScale;
        }

        public Vector2 GlobalOrigin
        {
            get => Origin * GlobalScale;

            set => Origin = value / GlobalScale;
        }

        public Transform2D(Vector2 position, Vector2 dimensions, Vector2 origin, float rotation, float scale)
        {
            Position = position;
            Dimensions = dimensions;
            Origin = origin;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Determines the top-left of this <see cref="Transform2D"/> after applying origin, scale, and rotation.
        /// </summary>
        public Vector2 TopLeft => GlobalPosition - (CryosisMath.Rotate(Origin, GlobalRotation) * GlobalScale);
    }
}
