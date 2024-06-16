using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Channels;
using System.Xml.Linq;


namespace StegoDFT_Toolkit
{
    public class FourierTransformSteganography : ISteganographycAlgorithm
    {

        public Container EmbedPayload(Container container, byte[] bytes, ColorChannel channel)
        {
            Container outputContainer = (Container)container.Clone();
            int currentBlockIndex = 0;
            FourierTransformation fourierTransfotmation = new FourierTransformation();

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool bit = (bytes[i] & (1 << (7 - j))) != 0;
                    float[,] blockfloat = ConvertColorBlockTofloat(outputContainer[currentBlockIndex], channel);
                    PrepareBlock(blockfloat);

                    while (true)
                    {
                        float[,] blockTransformed = fourierTransfotmation.Transform(blockfloat);

                        SetBit(blockTransformed, bit);

                        float[,] blockfloatTMP = fourierTransfotmation.Transform(blockTransformed);

                        bool exceedsMaxValue = false;
                        bool exceedsMinValue = false;

                        int exceed_row = 0, exceed_col = 0;

                        for (int rowIndex = 0; rowIndex < blockfloat.GetLength(0); rowIndex++)
                        {
                            for (int colIndex = 0; colIndex < blockfloat.GetLength(1); colIndex++)
                            {
                                float value = blockfloatTMP[rowIndex, colIndex];
                                //blockfloat[rowIndex, colIndex] = Math.Clamp(value, 0, 255);

                                if (value > 255)
                                {
                                    exceed_row = rowIndex;
                                    exceed_col = colIndex;
                                    exceedsMinValue = false;
                                    exceedsMaxValue = true;
                                }
                                else if (value < 0)
                                {
                                    exceed_row = rowIndex;
                                    exceed_col = colIndex;
                                    exceedsMinValue = true;
                                    exceedsMaxValue = false;
                                }
                            }
                        }

                        if (exceedsMaxValue)
                        {
                            //for (int rowIndex = 0; rowIndex < blockfloat.GetLength(0); rowIndex++)
                            //{
                            //    for (int colIndex = 0; colIndex < blockfloat.GetLength(1); colIndex++)
                            //    {
                            //        blockfloat[rowIndex, colIndex] -= 1;
                            //    }
                            //}
                            blockfloat[exceed_row, exceed_col] -= 1;

                        }
                        else if (exceedsMinValue)
                        {
                            //for (int rowIndex = 0; rowIndex < blockfloat.GetLength(0); rowIndex++)
                            //{
                            //    for (int colIndex = 0; colIndex < blockfloat.GetLength(1); colIndex++)
                            //    {
                            //        blockfloat[rowIndex, colIndex] += 1;
                            //    }
                            //}
                            blockfloat[exceed_row, exceed_col] += 1;
                        }
                        else
                        {
                            blockfloat = blockfloatTMP;
                            break;
                        }
                    }

                    InjectPayloadIntoBlock(blockfloat, outputContainer, currentBlockIndex, channel);
                    currentBlockIndex++;
                }
            }

            return outputContainer;
        }


        private void SetBit(float[,] blockTransformed, bool bit)
        {
            for (int rowIndex = 0; rowIndex < blockTransformed.GetLength(0); rowIndex++)
            {
                for (int colIndex = 0; colIndex < blockTransformed.GetLength(1); colIndex++)
                {
                    if (bit)
                    {
                        // Якщо bit == true, установка найменшого значущого біта на 1
                        blockTransformed[rowIndex, colIndex] = (int)blockTransformed[rowIndex, colIndex] | 1;
                    }
                    else
                    {
                        // Якщо bit == false, скидання найменшого значущого біта на 0
                        blockTransformed[rowIndex, colIndex] = (int)blockTransformed[rowIndex, colIndex] & ~1;
                    }
                }
            }
        }

        public byte[] ExtractPayload(Container container, int length, ColorChannel channel)
        {
            int byteCount = length / 8;
            byte[] bytes = new byte[byteCount];
            FourierTransformation fourierTransfotmation = new FourierTransformation();
            for (int i = 0; i < byteCount; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int bitIndex = i * 8 + j;
                    if (bitIndex < length)
                    {
                        float[,] blockfloat = ConvertColorBlockTofloat(container[bitIndex], channel);
                        float[,] blockTransformed = fourierTransfotmation.Transform(blockfloat);
                        if (BlockIsOdd(blockTransformed))
                        {
                            bytes[i] |= (byte)(1 << (7 - j));
                        }
                    }
                }
            }
            return bytes;
        }




        private bool BlockIsOdd(float[,] block)
        {
            int rows = block.GetLength(0);
            int cols = block.GetLength(1);

            bool odd = true;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((((int)block[i, j]) & 1) == 0)
                    {
                        odd = false;
                        break;
                    }
                }
            }

            return odd;
        }

        private bool BlockIsEven(float[,] block)
        {
            int rows = block.GetLength(0);
            int cols = block.GetLength(1);

            bool even = true;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((((int)block[i, j]) & 1) == 1)
                    {
                        even = false;
                        break;
                    }
                }
            }

            return even;
        }

        public bool CheckAuthenticity1(Container container, int length, ColorChannel channel)
        {
            FourierTransformation fourierTransformation = new FourierTransformation();

            for (int i = 0; i < length; i++)
            {
                float[,] blockfloat = ConvertColorBlockTofloat(container[i], channel);
                float[,] blockTransformed = fourierTransformation.Transform(blockfloat);

                if (!BlockIsSetOfIntegers(blockTransformed))
                {
                    // Якщо хоча б один блок не є набором цілих чисел,
                    // встановити прапорець isAuthentic у false
                    return false;
                }
            }

            return true;
        }

        private bool BlockIsSetOfIntegers(float[,] block)
        {
            int rows = block.GetLength(0);
            int cols = block.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((block[i, j] - Math.Floor(block[i, j])) != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckAuthenticity2(Container container, int length, ColorChannel channel)
        {
            FourierTransformation fourierTransformation = new();
            for (int i = 0; i < length; i++)
            {
                float[,] blockfloat = ConvertColorBlockTofloat(container[i], channel);
                float[,] blockTransformed = fourierTransformation.Transform(blockfloat);
                if (!BlockAreEvenOrOdd(blockTransformed))
                {
                    return false;
                }
            }
            return true;
        }

        public int SpartialDetect(Container container, int length, ColorChannel channel)
        {
            int evenBlocksCount = 0;
            for (int i = 0; i < length; i++)
            {
                float[,] blockfloat = ConvertColorBlockTofloat(container[i], channel);

                int evenCount = 0;
                int oddCount = 0;

                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        // Перевірка парності або непарності елементу
                        if ((int)blockfloat[y, x] % 2 == 0)
                        {
                            evenCount++;
                        }
                        else
                        {
                            oddCount++;
                        }
                    }
                }

                bool blockIsEven = (evenCount == oddCount) || (evenCount == 0) || (oddCount == 0);
                if (blockIsEven)
                {
                    evenBlocksCount++;
                }
            }
            return evenBlocksCount;
        }

        public int FrequencyDetect(Container container, int length, ColorChannel channel)
        {
            int evenOrOddBlocksCount = 0;
            for (int i = 0; i < length; i++)
            {
                float[,] blockfloat = ConvertColorBlockTofloat(container[i], channel);
                FourierTransformation fourierTransformation = new();
                float[,] blockTransformed = fourierTransformation.Transform(blockfloat);
                if (BlockIsOdd(blockTransformed) || BlockIsEven(blockTransformed))
                {
                    evenOrOddBlocksCount++;
                }
            }
            return evenOrOddBlocksCount;
        }

        private bool BlockAreEvenOrOdd(float[,] block) 
        {
            bool areEqual = (((int)block[0, 0]) & 1) == (((int)block[0, 1]) & 1) &&
                            (((int)block[0, 0]) & 1) == (((int)block[1, 0]) & 1) &&
                            (((int)block[0, 0]) & 1) == (((int)block[1, 1]) & 1); 
            return areEqual;
        }



        static void InjectPayloadIntoBlock(float[,] floatBlock, Container container, int index, ColorChannel channel)
        {
            int rows = floatBlock.GetLength(0);
            int cols = floatBlock.GetLength(1);

            Color[,] block = container[index];

            //Parallel.For(0, rows, i =>
            //{
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int intValue = (int)Math.Round(floatBlock[i, j]);
                    intValue = Math.Max(0, Math.Min(255, intValue));

                    Color oldColor = block[i, j];
                    Color newColor;

                    switch (channel)
                    {
                        case ColorChannel.Red:
                            newColor = Color.FromArgb(intValue, oldColor.G, oldColor.B);
                            break;
                        case ColorChannel.Green:
                            newColor = Color.FromArgb(oldColor.R, intValue, oldColor.B);
                            break;
                        case ColorChannel.Blue:
                            newColor = Color.FromArgb(oldColor.R, oldColor.G, intValue);
                            break;
                        default:
                            throw new ArgumentException("Invalid color channel specified.");
                    }

                    block[i, j] = newColor;
                }
            }
            //});
            container[index] = block;
        }

        static float[,] ConvertColorBlockTofloat(Color[,] block, ColorChannel channel)
        {
            int rows = block.GetLength(0);
            int cols = block.GetLength(1);

            float[,] floatBlock = new float[rows, cols];

            //Parallel.For(0, rows, i =>
            //{
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch (channel)
                    {
                        case ColorChannel.Red:
                            floatBlock[i, j] = block[i, j].R;
                            break;
                        case ColorChannel.Green:
                            floatBlock[i, j] = block[i, j].G;
                            break;
                        case ColorChannel.Blue:
                            floatBlock[i, j] = block[i, j].B;
                            break;
                        default:
                            throw new ArgumentException("Invalid color channel specified.");
                    }
                }
            }
            //});

            return floatBlock;
        }

        private void PrepareBlock(float[,] block)
        {
            List<Tuple<int, int>> evenIndices = new List<Tuple<int, int>>();
            List<Tuple<int, int>> oddIndices = new List<Tuple<int, int>>();

            // Рахуємо парні та неппарні числа 
            for (int i = 0; i < block.GetLength(0); i++)
            {
                for (int j = 0; j < block.GetLength(1); j++)
                {
                    if (block[i, j] % 2 == 0)
                    {
                        evenIndices.Add(new Tuple<int, int>(i, j));
                    }
                    else
                    {
                        oddIndices.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            // Вирівнюємо кількість парних і непарних елементів у разі якщо їх
            // кількість не рівна або всі вони не є парними або непарними
            if (evenIndices.Count() != 0 && oddIndices.Count() != 0)
            {
                if (evenIndices.Count() > oddIndices.Count())
                {
                    var (i, j) = oddIndices[0];
                    //block[i, j] = (int)block[i, j] | 1;
                    block[i, j] += 1; // Робимо непарний елемент парним
                }

                if (evenIndices.Count() < oddIndices.Count())
                {
                    var (i, j) = evenIndices[0];
                    //block[i, j] = (int)block[i, j] & ~1;
                    block[i, j] -= 1; // Робимо парний елемент непарним
                }
            }
        }
    }
}
