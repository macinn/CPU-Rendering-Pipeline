﻿using Utils;

namespace CPU_Rendering;



internal partial class Pipeline
{
    class Clipping : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            // TODO: Implement clipping
            return state;
        }
    }
}
