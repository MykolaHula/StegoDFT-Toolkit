using Accord.Math;
using Amplifier;
using Amplifier.OpenCL;
using StegoDFT_Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace StegoDFT_Toolkit
{
    public class FourierTransformSteganographyOpenCL : ISteganographycAlgorithm, IDisposable
    {
        //dynamic EXEC;
        private OpenCLCompiler Compiler;
        public string DeviceName;
        private string KernelFileName = "amp_cl{0}.bin";
        private bool disposed = false;

        public FourierTransformSteganographyOpenCL()
        {
            Compiler = new OpenCLCompiler();
            Compiler.UseDevice(0);
            Compiler.CompileKernel(typeof(Kernels));
            //EXEC = Compiler.GetExec();
        }

        public FourierTransformSteganographyOpenCL(int deviceID)
        {
            Compiler = new OpenCLCompiler();
            Compiler.UseDevice(deviceID);
            Compiler.CompileKernel(typeof(Kernels));
            //EXEC = Compiler.GetExec();
        }

        public FourierTransformSteganographyOpenCL(OpenCLCompiler compiler, int deviceID)
        {
            this.Compiler = compiler;
            KernelFileName = string.Format(KernelFileName, deviceID);
            if (compiler == null)
            {
                throw new ArgumentNullException(nameof(compiler), "Compiler instance cannot be null.");
            }
            if (File.Exists(KernelFileName))
            {
                LoadKernel(deviceID);
            }
            else
            {
                CompileAndSaveKernel(deviceID);
            }
        }

        private void LoadKernel(int deviceID)
        {
            try
            {
                Compiler.Load(KernelFileName, deviceID);
                DeviceName = Compiler.Devices[deviceID].Name;
                //dynamic exec = Compiler.GetExec();
                //EXEC = exec;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load kernel: " + ex.Message, ex);
            }
        }

        private void CompileAndSaveKernel(int deviceID)
        {
            try
            {
                Compiler.UseDevice(deviceID);
                Compiler.CompileKernel(typeof(Kernels));
                DeviceName = Compiler.Devices[deviceID].Name;
                Compiler.Save(KernelFileName);
                //dynamic exec = Compiler.GetExec();
                //EXEC = exec;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to compile and save kernel: " + ex.Message, ex);
            }
        }

        public Container EmbedPayload(Container container, byte[] bytes, ColorChannel channel)
        {
            Container outputContainer = (Container)container.Clone();
            //for(int i = 0; i < outputContainer.BlockCount; i++)
            //{
            //    float[,] blockfloat = ConvertColorBlockTofloat(outputContainer[i]);
            //    PrepareBlock(blockfloat);
            //    InjectPayloadIntoBlock(blockfloat, outputContainer, i);
            //}

            float[,] Input2D = GetColorValueArray(outputContainer.SourceImage, channel);
            int x = Input2D.GetLength(0);
            int y = Input2D.GetLength(1);


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Array payload = ByteArrayToIntArray(bytes);
            stopwatch.Stop();
            Console.WriteLine($" Час витрачений на переведення байтів у масив int: {stopwatch.ElapsedMilliseconds} мс");

            float[] inputContainerfloat = FlattenArray(Input2D);

            //PrepareContainer(inputContainerfloat, payload.Length, Input2D.GetLength(1));
            var EXEC = Compiler.GetExec();
            EXEC.Embed(inputContainerfloat, payload, payload.Length, Input2D.GetLength(1));
            float[,] expanded = ExpandArray(inputContainerfloat, x, y);
            return InjectPayload(expanded, outputContainer, channel);
        }

        public byte[] ExtractPayload(Container container, int length, ColorChannel channel)
        {
            float[,] Input2D = GetColorValueArray(container.SourceImage, channel);
            float[] inputContainerfloat = FlattenArray(Input2D);
            int[] outputPayload = new int[length];
            //byte[] bytes = new byte[length / 8];
            var EXEC = Compiler.GetExec();
            EXEC.Extract(inputContainerfloat, outputPayload, length, Input2D.GetLength(1));

            byte[] bytes;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bytes = IntArrayToByteArray(outputPayload);
            stopwatch.Stop();
            Console.WriteLine($" Час витрачений на відновлення байтів: {stopwatch.ElapsedMilliseconds} мс");

            return bytes;
        }

        //private bool BlockIsOdd(float[,] block)
        //{
        //    int rows = block.GetLength(0);
        //    int cols = block.GetLength(1);

        //    bool odd = true;

        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            if ((((int)block[i, j]) & 1) == 0)
        //            {
        //                odd = false;
        //                break;
        //            }
        //        }
        //    }

        //    return odd;
        //}

        public bool CheckAuthenticity1(Container container, int length, ColorChannel channel)
        {
            float[,] Input2D = GetColorValueArray(container.SourceImage, channel);
            float[] inputContainerfloat = FlattenArray(Input2D);
            //byte[] bytes = new byte[length / 8];
            int[] partialResults = new int[length];
            var EXEC = Compiler.GetExec();
            EXEC.CheckAuthenticity1(inputContainerfloat, partialResults, length, Input2D.GetLength(1));
            int[] result = [0];
            SumPartialResults(partialResults, result, length);

            bool result_ = result[0] == length;
            return result_;
        }

        public bool CheckAuthenticity2(Container container, int length, ColorChannel channel)
        {
            float[,] Input2D = GetColorValueArray(container.SourceImage, channel);
            float[] inputContainerfloat = FlattenArray(Input2D);
            //byte[] bytes = new byte[length / 8];
            int[] resultInt = new int[] { 1 };
            var EXEC = Compiler.GetExec();
            int[] partialResults = new int[length];
            EXEC.CheckAuthenticity2(inputContainerfloat, partialResults, length, Input2D.GetLength(1));

            int[] result = [0];
            SumPartialResults(partialResults, result, length);

            bool result_ = result[0] == length;
            return result_;
        }

        public int SpartialDetect(Container container, int length, ColorChannel channel)
        {
            float[,] Input2D = GetColorValueArray(container.SourceImage, channel);
            float[] inputContainerfloat = FlattenArray(Input2D);

            int[] partialResults = new int[length];
            int[] result = new int[1];

            Compiler.Execute("SpartialDetect", inputContainerfloat, partialResults, length, Input2D.GetLength(1));
            SumPartialResults(partialResults, result, length);

            Array.Clear(Input2D, 0, Input2D.Length);
            Array.Clear(inputContainerfloat, 0, inputContainerfloat.Length);
            Array.Clear(partialResults, 0, partialResults.Length);
            return result[0];
        }

        public int FrequencyDetect(Container container, int length, ColorChannel channel)
        {
            float[,] Input2D = GetColorValueArray(container.SourceImage, channel);
            float[] inputContainerfloat = FlattenArray(Input2D);

            int[] partialResults = new int[length];
            var EXEC = Compiler.GetExec();
            EXEC.FrequencyDetect(inputContainerfloat, partialResults, length, Input2D.GetLength(1));

            int[] result = [0];
            //EXEC.SumPartialResults(partialResults, result, length);
            SumPartialResults(partialResults, result, length);

            return result[0];
        }

        void SumPartialResults(int[] partialResults, int[] finalResult, int blocksCount)
        {
            int sum = 0;
            for (int i = 0; i < blocksCount; i++)
            {
                sum += partialResults[i];
            }
            finalResult[0] = sum;
        }

        static Container InjectPayload(float[,] floatBlock, Container container, ColorChannel channel)
        {
            // Отримання розмірів зображення
            int width = container.SourceImage.Width;
            int height = container.SourceImage.Height;

            // Прохід по кожному пікселю зображення
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    // Отримання поточного значення синього кольору з масиву
                    float Value = floatBlock[y, x];

                    // Зміна значення кольору пікселя
                    int newValue = (int)Math.Round(Value);
                    newValue = Math.Max(0, Math.Min(255, newValue));

                    // Отримання поточного кольору пікселя
                    Color oldColor = container.SourceImage.GetPixel(y, x);

                    Color newColor;
                    switch (channel)
                    {
                        case ColorChannel.Red:
                            newColor = Color.FromArgb(newValue, oldColor.G, oldColor.B);
                            break;
                        case ColorChannel.Green:
                            newColor = Color.FromArgb(oldColor.R, newValue, oldColor.B);
                            break;
                        case ColorChannel.Blue:
                            newColor = Color.FromArgb(oldColor.R, oldColor.G, newValue);
                            break;
                        default:
                            throw new ArgumentException("Invalid color channel specified.");
                    }

                    // Оновлення кольору пікселя у зображенні
                    container.SourceImage.SetPixel(y, x, newColor);
                }
            }
            return container;
        }

        //static void InjectPayloadIntoBlock(float[,] floatBlock, Container container, int index)
        //{
        //    int rows = floatBlock.GetLength(0);
        //    int cols = floatBlock.GetLength(1);

        //    Color[,] block = container[index];

        //    //Parallel.For(0, rows, i =>
        //    //{
        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            int intValue = (int)Math.Round(floatBlock[i, j]);
        //            intValue = Math.Max(0, Math.Min(255, intValue));
        //            block[i, j] = Color.FromArgb(block[i, j].R, block[i, j].G, intValue);
        //        }
        //    }
        //    //});
        //    container[index] = block;
        //}

        // Функція для перетворення масиву байтів у масив цілих чисел
        static int[] ByteArrayToIntArray(byte[] byteArray)
        {
            int[] intArray = new int[byteArray.Length * 8]; // Створення масиву int

            for (int i = 0; i < byteArray.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    intArray[i * 8 + j] = (byteArray[i] >> (7 - j)) & 1; // Отримання кожного біту та запис у масив int
                }
            }

            return intArray;
        }

        static byte[] IntArrayToByteArray(int[] intArray)
        {
            int byteArrayLength = intArray.Length / 8;
            byte[] byteArray = new byte[byteArrayLength]; // Створення байтового масиву

            for (int i = 0; i < byteArrayLength; i++)
            {
                byte currentByte = 0;
                for (int j = 0; j < 8; j++)
                {
                    currentByte |= (byte)(intArray[i * 8 + j] << (7 - j)); // Запис кожного байту в байтовий масив
                }
                byteArray[i] = currentByte;
            }

            return byteArray;
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
                    }
                }
            }
            //});

            return floatBlock;
        }

        static float[,] GetColorValueArray(Bitmap image, ColorChannel channel)
        {
            // Отримання розмірів зображення
            int width = image.Width;
            int height = image.Height;

            // Створення масиву для зберігання значень яскравості синього кольору пікселів
            float[,] brightnessArray = new float[height, width];

            // Прохід по кожному пікселю зображення та запис значення яскравості синього кольору у масив
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Отримання кольору пікселя
                    Color pixelColor = image.GetPixel(x, y);

                    float brightness = 0;

                    switch (channel)
                    {
                        case ColorChannel.Red:
                            brightness = pixelColor.R;
                            break;
                        case ColorChannel.Green:
                            brightness = pixelColor.G;
                            break;
                        case ColorChannel.Blue:
                            brightness = pixelColor.B;
                            break;
                    }

                    // Запис значення яскравості синього кольору у масив
                    brightnessArray[y, x] = brightness;
                }
            }

            // Повернення масиву значень яскравості синіх пікселів
            return brightnessArray;
        }


        //private void PrepareBlock(float[,] block)
        //{
        //    List<Tuple<int, int>> evenIndices = new List<Tuple<int, int>>();
        //    List<Tuple<int, int>> oddIndices = new List<Tuple<int, int>>();

        //    // Рахуємо парні та неппарні числа 
        //    for (int i = 0; i < block.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < block.GetLength(1); j++)
        //        {
        //            if (block[i, j] % 2 == 0)
        //            {
        //                evenIndices.Add(new Tuple<int, int>(i, j));
        //            }
        //            else
        //            {
        //                oddIndices.Add(new Tuple<int, int>(i, j));
        //            }
        //        }
        //    }

        //    // Вирівнюємо кількість парних і непарних елементів у разі якщо їх
        //    // кількість не рівна або всі вони не є парними або непарними
        //    if (evenIndices.Count() != 0 && oddIndices.Count() != 0)
        //    {
        //        if (evenIndices.Count() > oddIndices.Count())
        //        {
        //            var (i, j) = oddIndices[0];
        //            //block[i, j] = (int)block[i, j] | 1;
        //            block[i, j] += 1; // Робимо непарний елемент парним
        //        }

        //        if (evenIndices.Count() < oddIndices.Count())
        //        {
        //            var (i, j) = evenIndices[0];
        //            //block[i, j] = (int)block[i, j] & ~1;
        //            block[i, j] -= 1; // Робимо парний елемент непарним
        //        }
        //    }
        //}

        //private void PrepareContainer(float[] inputContainerfloat, int length, int blockLength)
        //{
        //    EXEC.PrepareContainer(inputContainerfloat, length, blockLength);
        //}

        static T[] FlattenArray<T>(T[,] twoDArray)
        {
            int rows = twoDArray.GetLength(0); // Кількість рядків
            int cols = twoDArray.GetLength(1); // Кількість стовпців

            T[] oneDArray = new T[rows * cols]; // Створення одновимірного масиву

            // Заповнення одновимірного масиву значеннями з двовимірного масиву
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    oneDArray[index] = twoDArray[i, j];
                    index++;
                }
            }

            return oneDArray;
        }

        static float[,] ExpandArray(float[] oneDArray, int rows, int cols)
        {
            if (oneDArray.Length != rows * cols)
            {
                throw new ArgumentException("Довжина одновимірного масиву не відповідає розміру двовимірного масиву");
            }

            float[,] twoDArray = new float[cols, rows]; // Створення двовимірного масиву

            // Заповнення двовимірного масиву значеннями з одновимірного масиву
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    twoDArray[j, i] = oneDArray[index];
                    index++;
                }
            }

            return twoDArray;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Звільнення керованих ресурсів
                    if (Compiler != null)
                    {
                        Compiler.Dispose();
                        Compiler = null;
                    }
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Фіналізатор
        ~FourierTransformSteganographyOpenCL()
        {
            Dispose(false);
        }

    }
}
