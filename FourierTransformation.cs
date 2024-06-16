using Amplifier.OpenCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegoDFT_Toolkit
{
    public class FourierTransformation
    {
        public float[,] Transform(float[,] input)
        {
            float[,] output = new float[2, 2];
            output[0, 0] = 0.5f * (input[0, 0] + input[0, 1] + input[1, 0] + input[1, 1]);
            output[0, 1] = 0.5f * (input[0, 0] - input[0, 1] + input[1, 0] - input[1, 1]);
            output[1, 0] = 0.5f * (input[0, 0] + input[0, 1] - input[1, 0] - input[1, 1]);
            output[1, 1] = 0.5f * (input[0, 0] - input[0, 1] - input[1, 0] + input[1, 1]);
            return output;
        }
    }
}
