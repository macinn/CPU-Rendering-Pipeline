﻿using Utils;

namespace CPU_Rendering;



internal partial class Pipeline
{
    class Lighting : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            return state;
        }
    }
}
