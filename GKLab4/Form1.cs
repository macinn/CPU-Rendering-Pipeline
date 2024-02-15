using System.Numerics;
using Timer = System.Windows.Forms.Timer;


namespace CPU_Rendering
{
    public partial class Form1 : Form
    {
        Renderer renderer;
        public Form1()
        {
            InitializeComponent();

            shadingBox.DataSource = Enum.GetValues(typeof(ShadingType));
            shadingBox.SelectedItem = RenderStream.shadingType;
            fogInput.Value = (decimal)RenderStream.fogFactor;

            renderer = new Renderer(Canvas);
            renderer.addMesh(new Cube(1, new Vector4(1, 0, 0, 1)));

            Cube c2 = new Cube(2, new Vector4(0.2f, 0.6f, 0.1f, 0f));
            c2.ModelMatrix = Matrix4x4.CreateTranslation(3, 3, 0);
            renderer.addMesh(c2);

            Cube c3 = new Cube(1, new Vector4(0.8f, 0.1f, 0.4f, 0f));
            c3.ModelMatrix = Matrix4x4.CreateTranslation(-5, -1, 1);
            renderer.addMesh(c3);

            Sphere s1 = new Sphere(3f, 10, 6, new Vector4(0.67f, 0.85f, 0.9f, 0f));
            s1.ModelMatrix = Matrix4x4.CreateTranslation(0, -6, 0);
            renderer.addMesh(s1);

            Sphere s2 = new Sphere(1f, 10, 6, new Vector4(1f, 0f, 0f, 0f));
            s2.ModelMatrix = Matrix4x4.CreateTranslation(0, 0, 10);
            s2.updateModelFun = (dt) =>
            {
                //s2.ModelMatrix = Matrix4x4.CreateTranslation(0, 0, 10);
                s2.ModelMatrix *= Matrix4x4.CreateRotationY((float)(dt * Math.PI / 10000));
                s2.ModelMatrix *= Matrix4x4.CreateRotationX((float)(dt * Math.PI / 10000));
            };
            renderer.addMesh(s2);


            Timer timer = new Timer();
            timer.Interval = 1000 / 80;
            timer.Start();
            timer.Tick += new EventHandler(RenderFrame);

        }
        void RenderFrame(object Sender, EventArgs e)
        {
            renderer.render();
        }

        private void shadingBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderStream.shadingType = (ShadingType)shadingBox.SelectedItem;
        }

        private void fogInput_ValueChanged(object sender, EventArgs e)
        {
            RenderStream.fogFactor = (float)fogInput.Value;
        }

        private void camerButton_Click(object sender, EventArgs e)
        {
            RenderStream.changeCamera();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            IRenderable._drawLines = linesBox.Checked;
        }

        private void backFaceCullingButton_CheckedChanged(object sender, EventArgs e)
        {
            RenderStream.backFaceCulling = backFaceCullingButton.Checked;
        }
    }







}
