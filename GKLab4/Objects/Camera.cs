using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Objects
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Target;
        public Vector3 Up;
        public Action<long> updateCallback = (dt) => { };

        public Camera(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;
            Up = up;
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAtLeftHanded(Position, Target, Up);
        }
    }
}