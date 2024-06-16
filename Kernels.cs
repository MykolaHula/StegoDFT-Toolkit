using Amplifier.OpenCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegoDFT_Toolkit
{
    class Kernels : OpenCLFunctions
    {
        [OpenCLKernel]
        void Embed([Global] float[] inputContainer, [Global] int[] payload, int blocksCount, int width)
        {
            int index = get_global_id(0);

            if (index >= blocksCount)
            {
                return;
            }

            int blockStartIndex1D;

            if (index < (width / 2))
            {
                blockStartIndex1D = index * 2;
            }
            else
            {
                blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
            }

            // Ініціалізація елементів блоку
            float element0 = inputContainer[blockStartIndex1D];
            float element1 = inputContainer[blockStartIndex1D + 1];
            float element2 = inputContainer[blockStartIndex1D + width];
            float element3 = inputContainer[blockStartIndex1D + width + 1];

            // Ініціалізація змінних для зберігання кількості парних і непарних елементів
            int evenCount = 0;
            int oddCount = 0;

            // Перевірка елементів
            if ((int)element0 % 2 == 0) evenCount++; else oddCount++;
            if ((int)element1 % 2 == 0) evenCount++; else oddCount++;
            if ((int)element2 % 2 == 0) evenCount++; else oddCount++;
            if ((int)element3 % 2 == 0) evenCount++; else oddCount++;

            // Коригування кількості парних і непарних елементів за допомогою бітових операцій
            if (evenCount != 0 && oddCount != 0)
            {
                if (evenCount > oddCount)
                {
                    if ((int)element0 % 2 != 0) element0 = (int)element0 & ~1;
                    else if ((int)element1 % 2 != 0) element1 = (int)element1 & ~1;
                    else if ((int)element2 % 2 != 0) element2 = (int)element2 & ~1;
                    else if ((int)element3 % 2 != 0) element3 = (int)element3 & ~1;
                }
                else if (evenCount < oddCount)
                {
                    if ((int)element0 % 2 == 0) element0 = (int)element0 | 1;
                    else if ((int)element1 % 2 == 0) element1 = (int)element1 | 1;
                    else if ((int)element2 % 2 == 0) element2 = (int)element2 | 1;
                    else if ((int)element3 % 2 == 0) element3 = (int)element3 | 1;
                }
            }

            // Запис результатів назад у контейнер
            inputContainer[blockStartIndex1D] = element0;
            inputContainer[blockStartIndex1D + 1] = element1;
            inputContainer[blockStartIndex1D + width] = element2;
            inputContainer[blockStartIndex1D + width + 1] = element3;

            float transformed0;
            float transformed1;
            float transformed2;
            float transformed3;

            float recovered0;
            float recovered1;
            float recovered2;
            float recovered3;

            //bool bit = (payload[byteIndex] & (1 << (7 - bitIndex))) != 0;
            bool bit = payload[index] != 0;

            while (true)
            {

                transformed0 = 0.5f * (element0 + element1 + element2 + element3);
                transformed1 = 0.5f * (element0 - element1 + element2 - element3);
                transformed2 = 0.5f * (element0 + element1 - element2 - element3);
                transformed3 = 0.5f * (element0 - element1 - element2 + element3);

                int mask = bit ? 1 : 0;
                transformed0 = (int)transformed0 | mask;
                transformed1 = (int)transformed1 | mask;
                transformed2 = (int)transformed2 | mask;
                transformed3 = (int)transformed3 | mask;

                recovered0 = 0.5f * (transformed0 + transformed1 + transformed2 + transformed3);
                recovered1 = 0.5f * (transformed0 - transformed1 + transformed2 - transformed3);
                recovered2 = 0.5f * (transformed0 + transformed1 - transformed2 - transformed3);
                recovered3 = 0.5f * (transformed0 - transformed1 - transformed2 + transformed3);


                if (recovered0 >= 255 || recovered1 >= 255 || recovered2 >= 255 || recovered3 >= 255)
                {
                    element0 -= 1;
                    element1 -= 1;
                    element2 -= 1;
                    element3 -= 1;
                }
                else if (recovered0 <= 0 || recovered1 <= 0 || recovered2 <= 0 || recovered3 <= 0)
                {
                    element0 += 1;
                    element1 += 1;
                    element2 += 1;
                    element3 += 1;
                }
                else
                {
                    break;
                }
            }

            inputContainer[blockStartIndex1D] = recovered0;
            inputContainer[blockStartIndex1D + 1] = recovered1;
            inputContainer[blockStartIndex1D + width] = recovered2;
            inputContainer[blockStartIndex1D + width + 1] = recovered3;
        }

        [OpenCLKernel]
        void Extract([Global] float[] inputContainer, [Global] int[] payload, int blocksCount, int width)
        {
            int index = get_global_id(0);

            if (index >= blocksCount)
            {
                return;
            }

            int blockStartIndex1D;

            if (index < (width / 2))
            {
                blockStartIndex1D = index * 2;
            }
            else
            {
                blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
            }

            float transformed0 = 0.5f * (inputContainer[blockStartIndex1D] + inputContainer[blockStartIndex1D + 1] + inputContainer[blockStartIndex1D + width] + inputContainer[blockStartIndex1D + width + 1]);
            float transformed1 = 0.5f * (inputContainer[blockStartIndex1D] - inputContainer[blockStartIndex1D + 1] + inputContainer[blockStartIndex1D + width] - inputContainer[blockStartIndex1D + width + 1]);
            float transformed2 = 0.5f * (inputContainer[blockStartIndex1D] + inputContainer[blockStartIndex1D + 1] - inputContainer[blockStartIndex1D + width] - inputContainer[blockStartIndex1D + width + 1]);
            float transformed3 = 0.5f * (inputContainer[blockStartIndex1D] - inputContainer[blockStartIndex1D + 1] - inputContainer[blockStartIndex1D + width] + inputContainer[blockStartIndex1D + width + 1]);

            if ((((int)transformed0 & 1) == 1) && (((int)transformed1 & 1) == 1) && (((int)transformed2 & 1) == 1) && (((int)transformed3 & 1) == 1))
            {
                payload[index] |= 1;
            }

        }

        [OpenCLKernel]
        void CheckAuthenticity1([Global] float[] inputContainer, [Global] int[] result, int blocksCount, int width)
        {
            int index = get_global_id(0);

            if (index >= blocksCount)
            {
                return;
            }

            int blockStartIndex1D;

            if (index < (width / 2))
            {
                blockStartIndex1D = index * 2;
            }
            else
            {
                blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
            }

            result[index] = 1;

            float transformed0 = 0.5f * (inputContainer[blockStartIndex1D] + inputContainer[blockStartIndex1D + 1] + inputContainer[blockStartIndex1D + width] + inputContainer[blockStartIndex1D + width + 1]);
            if ((transformed0 - floor(transformed0)) != 0)
            {
                result[index] = 0;
                //return;
            }
            float transformed1 = 0.5f * (inputContainer[blockStartIndex1D] - inputContainer[blockStartIndex1D + 1] + inputContainer[blockStartIndex1D + width] - inputContainer[blockStartIndex1D + width + 1]);
            if ((transformed1 - floor(transformed1)) != 0)
            {
                result[index] = 0;
                //return;
            }
            float transformed2 = 0.5f * (inputContainer[blockStartIndex1D] + inputContainer[blockStartIndex1D + 1] - inputContainer[blockStartIndex1D + width] - inputContainer[blockStartIndex1D + width + 1]);
            if ((transformed2 - floor(transformed2)) != 0)
            {
                result[index] = 0;
                //return;
            }
            float transformed3 = 0.5f * (inputContainer[blockStartIndex1D] - inputContainer[blockStartIndex1D + 1] - inputContainer[blockStartIndex1D + width] + inputContainer[blockStartIndex1D + width + 1]);
            if ((transformed3 - floor(transformed3)) != 0)
            {
                result[index] = 0;
                //return;
            }

        }

        [OpenCLKernel]
        void CheckAuthenticity2([Global] float[] inputContainer, [Global] int[] result, int blocksCount, int width)
        {
            int index = get_global_id(0);

            if (index >= blocksCount)
            {
                return;
            }

            int blockStartIndex1D;

            if (index < (width / 2))
            {
                blockStartIndex1D = index * 2;
            }
            else
            {
                blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
            }

            result[index] = 1;

            float transformed0 = 0.5f * (inputContainer[blockStartIndex1D] + inputContainer[blockStartIndex1D + 1] + inputContainer[blockStartIndex1D + width] + inputContainer[blockStartIndex1D + width + 1]);
            float transformed1 = 0.5f * (inputContainer[blockStartIndex1D] - inputContainer[blockStartIndex1D + 1] + inputContainer[blockStartIndex1D + width] - inputContainer[blockStartIndex1D + width + 1]);
            float transformed2 = 0.5f * (inputContainer[blockStartIndex1D] + inputContainer[blockStartIndex1D + 1] - inputContainer[blockStartIndex1D + width] - inputContainer[blockStartIndex1D + width + 1]);
            float transformed3 = 0.5f * (inputContainer[blockStartIndex1D] - inputContainer[blockStartIndex1D + 1] - inputContainer[blockStartIndex1D + width] + inputContainer[blockStartIndex1D + width + 1]);

            bool areEqual = ((int)transformed0 & 1) == ((int)transformed1 & 1) &&
                ((int)transformed0 & 1) == ((int)transformed2 & 1) &&
                ((int)transformed0 & 1) == ((int)transformed3 & 1);

            if (!areEqual)
            {
                result[index] = 0;
            }

        }

        [OpenCLKernel]
        void SpartialDetect([Global] float[] inputContainer, [Global] int[] partialResults, int blocksCount, int width)
        {
            int index = get_global_id(0);

            if (index >= blocksCount)
            {
                return;
            }

            int blockStartIndex1D;

            if (index < (width / 2))
            {
                blockStartIndex1D = index * 2;
            }
            else
            {
                blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
            }

            // Ініціалізація елементів блоку
            float element0 = inputContainer[blockStartIndex1D];
            float element1 = inputContainer[blockStartIndex1D + 1];
            float element2 = inputContainer[blockStartIndex1D + width];
            float element3 = inputContainer[blockStartIndex1D + width + 1];


            int evenCount = 0;
            int oddCount = 0;

            // Перевірка елементів
            if ((int)element0 % 2 == 0) evenCount++; else oddCount++;
            if ((int)element1 % 2 == 0) evenCount++; else oddCount++;
            if ((int)element2 % 2 == 0) evenCount++; else oddCount++;
            if ((int)element3 % 2 == 0) evenCount++; else oddCount++;

            bool blockIsEven = (evenCount == oddCount) || (evenCount == 0) || (oddCount == 0);

            partialResults[index] = 0;
            if (blockIsEven)
            {
                partialResults[index] = 1;
            }

        }

        [OpenCLKernel]
        void FrequencyDetect([Global] float[] inputContainer, [Global] int[] partialResults, int blocksCount, int width)
        {
            int index = get_global_id(0);

            if (index >= blocksCount)
            {
                return;
            }

            int blockStartIndex1D;

            if (index < (width / 2))
            {
                blockStartIndex1D = index * 2;
            }
            else
            {
                blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
            }

            // Ініціалізація елементів блоку
            float element0 = inputContainer[blockStartIndex1D];
            float element1 = inputContainer[blockStartIndex1D + 1];
            float element2 = inputContainer[blockStartIndex1D + width];
            float element3 = inputContainer[blockStartIndex1D + width + 1];

            float transformed0 = 0.5f * (element0 + element1 + element2 + element3);
            float transformed1 = 0.5f * (element0 - element1 + element2 - element3);
            float transformed2 = 0.5f * (element0 + element1 - element2 - element3);
            float transformed3 = 0.5f * (element0 - element1 - element2 + element3);

            // Перевіряємо чи є всі елементи блоку парними
            bool allEven =
                ((((int)transformed0) & 1) == 0) &&
                ((((int)transformed1) & 1) == 0) &&
                ((((int)transformed2) & 1) == 0) &&
                ((((int)transformed3) & 1) == 0);

            bool allOdd =
                ((((int)transformed0) & 1) == 1) &&
                ((((int)transformed1) & 1) == 1) &&
                ((((int)transformed2) & 1) == 1) &&
                ((((int)transformed3) & 1) == 1);


            partialResults[index] = 0;
            if (allEven || allOdd)
            {
                partialResults[index] = 1;
            }

        }

        //[OpenCLKernel]
        //void SumPartialResults([Global] int[] partialResults, [Global] int[] finalResult, int blocksCount)
        //{
        //    int sum = 0;
        //    for (int i = 0; i < blocksCount; i++)
        //    {
        //        sum += partialResults[i];
        //    }
        //    finalResult[0] = sum;
        //}

        //[OpenCLKernel]
        //void PrepareContainer([Global] float[] inputContainer, int blocksCount, int width)
        //{
        //    int index = get_global_id(0);

        //    if (index >= blocksCount)
        //    {
        //        return;
        //    }

        //    int blockStartIndex1D;

        //    if (index < (width / 2))
        //    {
        //        blockStartIndex1D = index * 2;
        //    }
        //    else
        //    {
        //        blockStartIndex1D = (index / (width / 2) * width * 2) + ((index % (width / 2)) * 2);
        //    }

        //    // Ініціалізація елементів блоку
        //    float element0 = inputContainer[blockStartIndex1D];
        //    float element1 = inputContainer[blockStartIndex1D + 1];
        //    float element2 = inputContainer[blockStartIndex1D + width];
        //    float element3 = inputContainer[blockStartIndex1D + width + 1];

        //    // Ініціалізація змінних для зберігання кількості парних і непарних елементів
        //    int evenCount = 0;
        //    int oddCount = 0;

        //    // Перевірка елементів
        //    if ((int)element0 % 2 == 0) evenCount++; else oddCount++;
        //    if ((int)element1 % 2 == 0) evenCount++; else oddCount++;
        //    if ((int)element2 % 2 == 0) evenCount++; else oddCount++;
        //    if ((int)element3 % 2 == 0) evenCount++; else oddCount++;

        //    // Коригування кількості парних і непарних елементів за допомогою бітових операцій
        //    if (evenCount != 0 && oddCount != 0)
        //    {
        //        if (evenCount > oddCount)
        //        {
        //            if ((int)element0 % 2 != 0) element0 = (int)element0 & ~1;
        //            else if ((int)element1 % 2 != 0) element1 = (int)element1 & ~1;
        //            else if ((int)element2 % 2 != 0) element2 = (int)element2 & ~1;
        //            else if ((int)element3 % 2 != 0) element3 = (int)element3 & ~1;
        //        }
        //        else if (evenCount < oddCount)
        //        {
        //            if ((int)element0 % 2 == 0) element0 = (int)element0 | 1;
        //            else if ((int)element1 % 2 == 0) element1 = (int)element1 | 1;
        //            else if ((int)element2 % 2 == 0) element2 = (int)element2 | 1;
        //            else if ((int)element3 % 2 == 0) element3 = (int)element3 | 1;
        //        }
        //    }

        //    // Запис результатів назад у контейнер
        //    inputContainer[blockStartIndex1D] = element0;
        //    inputContainer[blockStartIndex1D + 1] = element1;
        //    inputContainer[blockStartIndex1D + width] = element2;
        //    inputContainer[blockStartIndex1D + width + 1] = element3;
        //}

    }

}