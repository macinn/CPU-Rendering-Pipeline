using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CPU_Rendering
{
    // Perform backaface check in Camera cooridnates
    public class BackfaceCulling : IShader<Traiangle>
    {
        private static readonly Vector3 frontVector = Vector3.UnitZ;
        public static Traiangle? Process(Traiangle state)
        {
            Vector3 traiangleNormal = 
                1/3 * (state.Vertices[0].Normal 
                + state.Vertices[1].Normal 
                + state.Vertices[2].Normal);
            bool isBackface = Vector3.Dot(traiangleNormal, frontVector) > 0;
            return isBackface ? state : null;
        }
    }
}
