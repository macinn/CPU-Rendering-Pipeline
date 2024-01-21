using System.Numerics;
using Timer = System.Windows.Forms.Timer;


namespace GKLab4
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

            //Cube c2 = new Cube(1, new Vector4(0, 1, 0, 1));
            //c2.ModelMatrix = Matrix4x4.CreateTranslation(20, 0, 0);
            //renderer.addMesh(c2);

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
    }







}
