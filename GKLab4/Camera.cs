using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace GKLab4
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Target;
        public Vector3 Up;

        public Camera(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;
            Up = up;
        }

        public Matrix4x4 GetViewMatrix()
        {
            return CreateViewMatrix(Position, Target, Up);
        }

        //static Matrix4x4 createViewMatrix(Vector3 P, Vector3 T, Vector3 Uw)
        //{
        //    Vector3 D = Vector3.Normalize(P - T);
        //    Vector3 R = Vector3.Normalize(Vector3.Cross(Uw, D));
        //    Vector3 U = Vector3.Normalize(Vector3.Cross(D, R));

        //    //Matrix4x4 rotM = new Matrix4x4(
        //    //    R.X, R.Y, R.Z, 0,
        //    //    U.X, U.Y, U.Z, 0,
        //    //    D.X, D.Y, D.Z, 0,
        //    //    0, 0, 0, 1
        //    //);
        //    Matrix4x4 rotM = new Matrix4x4(
        //        R.X, U.X, D.X, P.X,
        //        R.Y, U.Y, D.Y, P.Y,
        //        R.Z, U.Z, D.Z, P.Z,
        //        0, 0, 0, 1
        //    );
             
        //    Matrix4x4.Invert(rotM, out Matrix4x4 res);
        //    //Matrix4x4 transM = Matrix4x4.CreateTranslation(-P);
        //    return res;
        //}

        static Matrix4x4 CreateViewMatrix(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 upVector)
        { 
            Vector3 forward = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 right = Vector3.Normalize(Vector3.Cross(upVector, forward));
            Vector3 up = Vector3.Cross(forward, right);

            Matrix4x4 viewMatrix = new Matrix4x4(
                right.X, up.X, -forward.X, 0.0f,
                right.Y, up.Y, -forward.Y, 0.0f,
                right.Z, up.Z, -forward.Z, 0.0f,
                -Vector3.Dot(right, cameraPosition),
                -Vector3.Dot(up, cameraPosition),
                Vector3.Dot(forward, cameraPosition),
                1.0f
            );
            //Matrix4x4 viewMatrix = new Matrix4x4(
            //    right.X, up.X, forward.X, cameraPosition.X,
            //    right.Y, up.Y, forward.Y, cameraPosition.Y,
            //    right.Z, up.Z, forward.Z, cameraPosition.Z,
            //    0,0,0,  1.0f
            //);
            //Matrix4x4.Invert(viewMatrix, out Matrix4x4 res);
            return viewMatrix;
        }
    }
}