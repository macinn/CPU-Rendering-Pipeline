using Microsoft.VisualBasic.Logging;
using Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO: ruchomy reflektor

namespace Deprecated
{

    public class Renderer
    {
        // Uniforms
        public Matrix4x4 ViewMatrix { get; set; }
        public Matrix4x4 ProjectionMatrix { get; set; }

        private List<IRenderable> meshes;
        private List<Vector4> lightPositions;
        private Color backgroundColor = Color.LightBlue;



        // Canvas



        // Dt
        private long dt;
        Stopwatch stopwatch;

        // Camers


        private void updateUniforms()
        {

        }
        private void updateDt()
        {
            stopwatch.Stop();
            dt = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
        }
        public Renderer(PictureBox canvas)
        {
            RenderStream.setUp(canvas);
            meshes = new List<IRenderable>();

            RenderStream.AddCamera(new Camera(
                new Vector3(0, 0, 3),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)));
            RenderStream.AddCamera(new Camera(
                new Vector3(10, 10, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)));
            RenderStream.AddCamera(new Camera(
                new Vector3(3, 3, 20),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)));

            Camera followingCam = new Camera(
                new Vector3(3, 3, 30),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));
            followingCam.updateCallback = (dt) =>
            {
                followingCam.Target = meshes[4].ModelMatrix.Translation;
            };
            RenderStream.AddCamera(followingCam);

            Camera movingCam = new Camera(
                new Vector3(3, 3, 30),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));
            movingCam.updateCallback = (dt) =>
            {
                movingCam.Position = meshes[4].ModelMatrix.Translation * 2;
            };
            RenderStream.AddCamera(movingCam);

            setProjectionMatrix(MathF.PI / 4, canvas.Width / canvas.Height, 0.1f, 10000f);


            RenderStream.addLight(new AmbientLight(new Vector4(0.2f, 0.2f, 0.2f, 1)));

            Light pointLight = new PointLight(
                new Vector4(0.4f, 0.4f, 0.4f, 1),
                position: new Vector4(0, 0, 0, 1));
            RenderStream.addLight(pointLight);

            SpotLight refletor2 = new SpotLight(
                new Vector4(0.7f, 0.1f, 0.1f, 1),
                 new Vector4(0, 0, -10, 1),
                 new Vector3(0, 0, 1));
            RenderStream.addLight(refletor2);

            SpotLight reflector = new SpotLight(
                new Vector4(0.0f, 0.7f, 0.0f, 1),
                 new Vector4(0, 0, 0, 1),
                new Vector3(0, 0, 1));

            reflector.updateCallback = (dt) =>
            {
                reflector.setPosition(meshes[4].ModelMatrix.Translation * 1.5f);
                Vector3 direction = Vector3.Normalize(meshes[4].ModelMatrix.Translation - Vector3.Zero);
                reflector.setSpotDirection(direction);
            };
            RenderStream.addLight(reflector);


            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        public void setProjectionMatrix(float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);
        }

        public void addMesh(IRenderable mesh)
        {
            meshes.Add(mesh);
        }
        public void render()
        {
            updateDt();
            updateUniforms();
            RenderStream.startDraw();
            //meshes[0].ModelMatrix = Matrix4x4.CreateRotationX((float)(Math.PI / 8));
            //meshes[0].ModelMatrix = Matrix4x4.CreateRotationY((float)(Math.PI / 8));
            meshes[0].ModelMatrix = meshes[0].ModelMatrix * Matrix4x4.CreateRotationY(0.001f * dt);
            meshes[0].ModelMatrix = meshes[0].ModelMatrix * Matrix4x4.CreateRotationX(0.001f * dt);

            ViewMatrix = RenderStream.currentCam.GetViewMatrix();
            RenderStream.currentCam.updateCallback(dt);
            foreach (Light light in RenderStream.lights)
            {
                //if(light is PointLight)
                light.setView(ViewMatrix);
                light.updateCallback(dt);
            }

            foreach (var mesh in meshes)
            {
                mesh.Render(ViewMatrix, ProjectionMatrix, dt);
            }

            RenderStream.flush();
        }
    }
}

