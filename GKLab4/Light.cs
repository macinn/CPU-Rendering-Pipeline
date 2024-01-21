using System.DirectoryServices;
using System.Numerics;

namespace GKLab4
{
    public abstract class Light
    {
        protected Vector4 color;
        protected Vector4 lightPos4;
        protected Vector3 lightPos3;
        public bool enabled = true;

        public Light(Vector4 color, Vector4 position)
        {
            this.color = color;
            lightPos4 = position;
            lightPos3 = new Vector3(position.X, position.Y, position.Z);
        }

        public void setView(Matrix4x4 viewM)
        {
            lightPos4 = Vector4.Transform(lightPos4, viewM);
            lightPos3 = new Vector3(lightPos4.X, lightPos4.Y, lightPos4.Z);
        }
        public abstract Vector4 getIntensity(Vertex vertex, Vector3 camPos);
    }

    public class PointLight : Light
    {
        public PointLight(Vector4 color, Vector4 position) : base(color, position)
        {
        }

        public override Vector4 getIntensity(Vertex vertex, Vector3 camPos)
        {
            const float kd = 0.5f;
            const float ks = 0.5f;
            const float m = 30f;
            

            Vector3 objectPosition = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
            Vector3 N = new Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);

            Vector3 L = Vector3.Normalize(lightPos3 - objectPosition);
            Vector3 R = (2 * Vector3.Dot(N, L) * N) - L;

            // TODO: V
            Vector3 V = new Vector3(0,0,1);

            float cosNL = Math.Clamp(Vector3.Dot(N, L),0,1);
            float cosVRM = (float)Math.Pow(Math.Clamp(Vector3.Dot(V, R), 0, 1), m);
            float loR = color.X;
            float loG = color.Y;
            float loB = color.Z;

            float red = (kd * loR * cosNL) + (ks * loR * cosVRM);
            float green = (kd * loG * cosNL) + (ks * loG * cosVRM);
            float blue = (kd * loB * cosNL) + (ks * loB * cosVRM);

            return new Vector4(red, green, blue, 1);
        }
    }

    public class AmbientLight : Light
    {
        public AmbientLight(Vector4 color) : base(color, Vector4.Zero)
        {
        }

        public override Vector4 getIntensity(Vertex vertex, Vector3 camPos)
        {
            return color;
        }
    }

    public class SpotLight : Light
    {
        Vector3 spotDirection, D;
        float spotCosineCutoff = 0.3f;
        float spotExponent = 10;
        public SpotLight(Vector4 color, Vector4 position, Vector3 spotDirection) : base(color, position)
        {
            this.spotDirection = spotDirection;
            this.D = -Vector3.Normalize(spotDirection);
        }

        public override Vector4 getIntensity(Vertex vertex, Vector3 camPos)
        {
            Vector3 position = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
            Vector3 L = Vector3.Normalize(lightPos3 - position);
            float spotCosine = Vector3.Dot(D, L);

            if(spotCosine < spotCosineCutoff)
            {
                return Vector4.Zero;
            } 

            float spotFactor = (float)Math.Pow(spotCosine, spotExponent);
            const float kd = 0.5f;
            const float ks = 0.5f;
            const float m = 30f;

            Vector3 normal = new Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);

            Vector3 R = (2 * Vector3.Dot(normal, position) * normal) - lightPos3;
            Vector3 V = Vector3.Normalize(camPos - position);

            float cosNL = Math.Clamp(Vector3.Dot(normal, lightPos3), 0, 1);
            float cosVRM = (float)Math.Pow(Math.Clamp(Vector3.Dot(V, R), 0, 1), m);
            float loR = color.X * vertex.Color.X;
            float loG = color.Y * vertex.Color.Y;
            float loB = color.Z * vertex.Color.Z;

            float red = (kd * loR * cosNL) + (ks * loR * cosVRM) * spotFactor;
            float green = (kd * loG * cosNL) + (ks * loG * cosVRM) * spotFactor;
            float blue = (kd * loB * cosNL) + (ks * loB * cosVRM) * spotFactor;


            return new Vector4(red, green, blue, 1);
        }
    }
}
