﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CPU_Rendering;

internal class Fog : IShader<Pixel>
{
    public static float fogFactor = 0.1f;
    public static Pixel? Process(Pixel state)
    {
        return state;
    }
}
