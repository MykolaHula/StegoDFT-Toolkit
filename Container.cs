using System.Drawing;
using Accord.Math.Decompositions;


namespace StegoDFT_Toolkit
{
    [Serializable]
    public class Container : ICloneable
    {
        public Bitmap SourceImage { get; }
        public int BlockSizeM { get; } // Розмір блоку по горизонталі
        public int BlockSizeN { get; } // Розмір блоку по вертикалі
        public int BlockCount { get; }
        private int[] FiltredBlocks { get; }

        public Color[,] this[int index]
        {
            get { return ReadBlock(FiltredBlocks[index]); }
            set { WriteBlock(FiltredBlocks[index], value); }
        }

        public Container(Bitmap sourceImage, int blockSizeN, int blockSizeM)
        {
            SourceImage = sourceImage;
            BlockSizeM = blockSizeN;
            BlockSizeN = blockSizeM;

            FiltredBlocks = Filter();
            BlockCount = FiltredBlocks.Length;
        }

        public object Clone()
        {
            return new Container(SourceImage, BlockSizeN, BlockSizeM);
        }

        private int[] Filter()
        {
            List<int> filtred = new List<int>();
            int blockCount = GetBlockCount();
            for(int index = 0; index < blockCount; index++)
            {
                //double[,] blockDouble = ConvertColorBlockToDouble(ReadBlock(index));

                //SingularValueDecomposition blockSVD = new(blockDouble);
                //double[,] diagonalMatrix = blockSVD.DiagonalMatrix;
                //double singularValue1 = diagonalMatrix[0, 0];

                //if (roundSing(singularValue1) >= 200)
                    filtred.Add(index);


                //if (roundSing(singularValue1) >= 100 && (singularValue2 * 0.75) < 100 && (singularValue2 * 0.75) > 20
                //    && (singularValue2 * 0.25) > 10 && (singularValue2 * 0.25) < 100)
                //    filtred.Add(index);
            }


            return filtred.ToArray();
        }

        double roundSing(double x)
        {
            int n;
            if (x >= 100)
                n = 2;  // Округлення до сотень
            else
                n = 1;  // Округлення до десятків
            double multiplier = Math.Pow(10, n);
            double rounded = Math.Floor(x / multiplier) * multiplier;
            return rounded;
        }

        static double[,] ConvertColorBlockToDouble(Color[,] block)
        {
            int rows = block.GetLength(0);
            int cols = block.GetLength(1);

            double[,] doubleBlock = new double[rows, cols];

            //Parallel.For(0, rows, i =>
            //{
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        doubleBlock[i, j] = block[i, j].B;
                    }
                }
            //});

            return doubleBlock;
        }

        private int GetBlockCount()
        {
            if (SourceImage == null)
            {
                Console.WriteLine("Помилка: Зображення не ініціалізоване.");
                return 0;
            }

            if (BlockSizeM <= 0 || BlockSizeN <= 0)
            {
                Console.WriteLine("Помилка: Розміри блоку повинні бути більшими за 0.");
                return 0;
            }

            if (SourceImage.Height <= 0 || SourceImage.Width <= 0)
            {
                Console.WriteLine("Помилка: Розміри зображення повинні бути більшими за 0.");
                return 0;
            }

            int horizontalBlocks = SourceImage.Height / BlockSizeM;
            int verticalBlocks = SourceImage.Width / BlockSizeN;

            int blocksCount = horizontalBlocks * verticalBlocks;
            return blocksCount;
        }

        private Color[,] ReadBlock(int blockIndex)
        {
            int blockCount = GetBlockCount();
            if (blockIndex < 0 || blockIndex >= blockCount)
            {
                Console.WriteLine("Помилка: Некоректний номер блоку.");
                return null;
            }

            int blocksPerRow = SourceImage.Width / BlockSizeN;
            int blockRow = blockIndex / blocksPerRow;
            int blockCol = blockIndex % blocksPerRow;

            int startX = blockCol * BlockSizeN;
            int startY = blockRow * BlockSizeM;

            Color[,] blockPixels = new Color[BlockSizeM, BlockSizeN];

            for (int i = 0; i < BlockSizeM; i++)
            {
                for (int j = 0; j < BlockSizeN; j++)
                {
                    blockPixels[i, j] = SourceImage.GetPixel(startX + j, startY + i);
                }
            }
            return blockPixels;
        }

        private void WriteBlock(int blockIndex, Color[,] block)
        {
            int blockCount = GetBlockCount();
            if (blockIndex < 0 || blockIndex >= blockCount)
            {
                Console.WriteLine("Помилка: Некоректний номер блоку.");
                return;
            }

            int blocksPerRow = SourceImage.Width / BlockSizeN;
            int blockRow = blockIndex / blocksPerRow;
            int blockCol = blockIndex % blocksPerRow;

            int startX = blockCol * BlockSizeN;
            int startY = blockRow * BlockSizeM;

            for (int i = 0; i < BlockSizeM; i++)
            {
                for (int j = 0; j < BlockSizeN; j++)
                {
                    SourceImage.SetPixel(startX + j, startY + i, block[i, j]);
                }
            }
        }



    }
}
