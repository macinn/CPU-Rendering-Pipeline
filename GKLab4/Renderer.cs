using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO: 3 kamery - easy

// TODO: gładki obiekt
// TODO: mgła

// TODO: backface fix


namespace GKLab4
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
                new Vector3(0, 0, 5), 
                new Vector3(0, 0, 0), 
                new Vector3(0, 1, 0)));
            RenderStream.AddCamera(new Camera(
                new Vector3(10, 10, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)));


            setProjectionMatrix(MathF.PI / 4, canvas.Width / canvas.Height, 0.1f, 1000f);

            RenderStream.addLight(new AmbientLight(new Vector4(0.2f, 0.2f, 0.2f, 1)));

            Light pointLight = new PointLight(
                new Vector4(0.5f, 0.5f, 0.5f, 1),
                position: new Vector4(10, 0, 10, 1));
            pointLight.setView(ViewMatrix);
            RenderStream.addLight(pointLight);

            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        public void setProjectionMatrix(float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            this.ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);
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
            this.ViewMatrix = RenderStream.currentCam.GetViewMatrix();
            foreach (var mesh in meshes)
                {
                    mesh.ModelMatrix = mesh.ModelMatrix* Matrix4x4.CreateRotationY(0.001f * dt);
                    mesh.ModelMatrix = mesh.ModelMatrix* Matrix4x4.CreateRotationX(0.001f * dt);
                    mesh.Render(ViewMatrix, ProjectionMatrix);
                }
            
            RenderStream.flush();
        }
    }
}

