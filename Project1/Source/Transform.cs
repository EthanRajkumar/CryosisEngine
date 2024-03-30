using System;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    /// <summary>
    /// Defines an object's spacial properties such as position, size, and rotation within 2D space.
    /// </summary>
    public class Transform2D
    {
        /// <summary>
        /// Defines the parent to base <see cref="GlobalPosition"/>, <see cref="GlobalRotation"/>, and <see cref="GlobalScale"/> values from.
        /// </summary>
        public Transform2D Parent { get; set; }

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
        public Vector2 GlobalPosition => Parent == null ? Position : ((CryosisMath.Rotate(Position, Parent.GlobalRotation) * Parent.GlobalScale) + Parent.GlobalPosition);

        /// <summary>
        /// Determines the object's global rotation given it's chain of parents. Uses local rotation if no parent exists.
        /// </summary>
        public float GlobalRotation => Parent == null ? Rotation : (Rotation + Parent.GlobalRotation);

        /// <summary>
        /// Determines the object's global scale given it's chain of parents. Uses local scale if no parent exists.
        /// </summary>
        public float GlobalScale => Parent == null ? Scale : (Scale * Parent.GlobalScale);

        /// <summary>
        /// Determines the object's dimensions given global scale factors.
        /// </summary>
        public Vector2 TrueDimensions => Dimensions * GlobalScale;

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
