using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegoDFT_Toolkit 
{ 
    public interface ISteganographycAlgorithm
    {
        public Container EmbedPayload(Container container, byte[] bytes, ColorChannel channel);

        public Byte[] ExtractPayload(Container container, int length, ColorChannel channel);

        public bool CheckAuthenticity1(Container container, int length, ColorChannel channel)
        {
            throw new NotImplementedException();
        }

        public bool CheckAuthenticity2(Container container, int length, ColorChannel channel)
        {
            throw new NotImplementedException();
        }

        public int SpartialDetect(Container container, int length, ColorChannel channel)
        {
            throw new NotImplementedException();
        }

        public int FrequencyDetect(Container container, int length, ColorChannel channel)
        {
            throw new NotImplementedException();
        }

    }
}
