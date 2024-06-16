using Accord.IO;
using Accord.Math;
using Amplifier;
using Humanizer.Bytes;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;

namespace StegoDFT_Toolkit
{

    public partial class Form1 : Form
    {

        private Container? InputContainer;
        private Container? OutputContainer;
        private byte[]? Payload;
        private byte[]? ExtractedBytes;
        OpenCLCompiler CompilerToSelect;
        private BackgroundWorker backgroundWorker;
        public delegate void WorkFunction(BackgroundWorker worker, DoWorkEventArgs e);

        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            CompilerToSelect = new OpenCLCompiler();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            //backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            //backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        //private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    WorkFunction workFunction = e.Argument as WorkFunction;
        //    if (workFunction != null)
        //    {
        //        workFunction(sender as BackgroundWorker, e);
        //    }
        //}

        //private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    progressBar1.Style = ProgressBarStyle.Blocks;
        //}

        private void Form1_Load(object sender, EventArgs e)
        {

            string[] computingModes = { "Software", "OpenCl" };
            computingModeCombo.Items.AddRange(computingModes);
            computingModeCombo.SelectedIndex = 0;

            var devices = CompilerToSelect.Devices;
            foreach (var device in devices)
            {
                acceleratorCombo.Items.Add(device.Name);
            }
            acceleratorCombo.SelectedIndex = 0;

            string[] DIModes = { "Псевдовипадкові значення", "Всі одиниці", "Всі нулі" };
            DIModeBox.Items.AddRange(DIModes);
            DIModeBox.SelectedIndex = 0;

            string[] imageItems = { "Вхідне зображення", "Стеганоповідомлення" };
            analizeItemCombo.Items.AddRange(imageItems);
            analizeItemCombo.SelectedIndex = 0;

            string[] analizeMethods = { "Підтвердження автентичності 1", "Підтвердження автентичності 2", "Дослідження просторової області", "Дослідження частотної області" };
            analizeMethodCombo.Items.AddRange(analizeMethods);
            analizeMethodCombo.SelectedIndex = 0;

        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string selectedFilePath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Отримання шляху до вибраного файлу
                    selectedFilePath = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            pathTextBox.Text = selectedFilePath;
            Bitmap inputBitmap = new Bitmap(selectedFilePath);
            InputContainer = new Container(inputBitmap, 2, 2);

            FileInfo fileInfo = new FileInfo(selectedFilePath);

            // Отримання назви файлу
            string fileName = fileInfo.Name;
            nameTextBox.Text = fileName;

            // Отримання розміру файлу в байтах
            long fileSizeBytes = fileInfo.Length;

            // Конвертація розміру файлу в мегабайти
            double fileSizeMB = fileSizeBytes / (1024.0 * 1024.0);

            sizeTextBox.Text = Math.Round(fileSizeMB, 3).ToString() + " MB";

            blocksTextBox.Text = InputContainer.BlockCount.ToString();

            inputImageBox.Image = inputBitmap;
        }


        private async Task EmbedDI()
        {
            if (InputContainer == null)
            {
                MessageBox.Show(String.Format($"Спочатку завантажте зображення!"), "Помилка!");
                return;
            }

            byte[] payload;

            switch (DIModeBox.SelectedIndex)
            {
                case 0:
                    payload = GenerateRandomBytes(InputContainer.BlockCount / 8 * (int)fillUpDown.Value/100);
                    break;
                case 1:
                    payload = Enumerable.Repeat<byte>(0xFF, InputContainer.BlockCount / 8 * (int)fillUpDown.Value / 100).ToArray();
                    break;
                case 2:
                    payload = Enumerable.Repeat<byte>(0x00, InputContainer.BlockCount / 8 * (int)fillUpDown.Value / 100).ToArray();
                    break;
                default:
                    payload = GenerateRandomBytes(InputContainer.BlockCount / 8 * (int)fillUpDown.Value / 100);
                    break;
            }

            ISteganographycAlgorithm algorithm;

            //payload = Enumerable.Repeat<byte>(0x00, InputContainer.BlockCount / 8).ToArray();

            switch (computingModeCombo.SelectedIndex)
            {
                case 0:
                    algorithm = new FourierTransformSteganography();
                    break;
                case 1:
                    //int acceleratorIndex = acceleratorCombo.SelectedIndex;
                    //algorithm = new FourierTransformSteganographyOpenCL(acceleratorIndex);
                    algorithm = new FourierTransformSteganography();
                    break;
                default:
                    algorithm = new FourierTransformSteganography();
                    break;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            OutputContainer = algorithm.EmbedPayload(InputContainer.DeepClone(), payload, ColorChannel.Blue).DeepClone();
            stopwatch.Stop();
            steganomessageBox.Image = OutputContainer.SourceImage;
            Payload = payload;

            //MessageBox.Show(String.Format($"Повідомлення вбудовано успішно!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms."), "Успіх!");
            MessageBox.Show(String.Format($"Повідомлення вбудовано успішно!"), "Успіх!");
        }
        private async void EmbedButton_Click(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            Task.Delay(10);
            await EmbedDI();
            progressBar1.Style = ProgressBarStyle.Blocks;
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            if (OutputContainer == null)
            {
                MessageBox.Show(String.Format($"Спочатку створіть стеганоповідомлення!"), "Помилка!");
                return;
            }
            ISteganographycAlgorithm algorithm;

            switch (computingModeCombo.SelectedIndex)
            {
                case 0:
                    algorithm = new FourierTransformSteganography();
                    break;
                case 1:
                    //int acceleratorIndex = acceleratorCombo.SelectedIndex;
                    //algorithm = new FourierTransformSteganographyOpenCL(acceleratorIndex);
                    algorithm = new FourierTransformSteganography();
                    break;
                default:
                    algorithm = new FourierTransformSteganography();
                    break;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ExtractedBytes = algorithm.ExtractPayload(OutputContainer.DeepClone(), Payload.Length * 8, ColorChannel.Blue);
            stopwatch.Stop();

            int valid = 0;
            for (int i = 0; i < ExtractedBytes.Length; i++)
            {
                if (ExtractedBytes[i] == Payload[i])
                {
                    valid = valid + 1;
                }
            }

            //if (valid == Payload.Length)
            //{
            //    MessageBox.Show($"Повідомлення витягнуто успішно!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Успіх!");
            //}
            //else
            //{
            //    MessageBox.Show($"Повідомлення не відповідає еталонному!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Помилка!");
            //}

            if (valid == Payload.Length)
            {
                MessageBox.Show($"Повідомлення витягнуто успішно!", "Успіх!");
            }
            else
            {
                MessageBox.Show($"Повідомлення не відповідає еталонному!", "Помилка!");
            }
        }

        private void computingModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (computingModeCombo.SelectedIndex == 0)
            {
                acceleratorCombo.Enabled = false;
            }
            else
            {
                acceleratorCombo.Enabled = true;
            }

        }

        private byte[] GenerateRandomBytes(int size)
        {
            byte[] randomBytes = new byte[size];
            Random random = new Random();

            random.NextBytes(randomBytes);

            return randomBytes;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputContainer == null)
            {
                MessageBox.Show(String.Format($"Спочатку створіть стеганоповідомлення!"), "Помилка!");
                return;
            }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG files (*.png)|*.png";
                saveFileDialog.DefaultExt = "png";
                saveFileDialog.AddExtension = true;
                saveFileDialog.Title = "Збереження зображення";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    OutputContainer.SourceImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private void analizeButton_Click(object sender, EventArgs e)
        {
            Container item;
            switch (analizeItemCombo.SelectedIndex)
            {
                case 0:
                    if (InputContainer == null)
                    {
                        MessageBox.Show(String.Format($"Спочатку завантажте зображення!"), "Помилка!");
                        return;
                    }
                    item = InputContainer;
                    break;
                case 1:
                    if (OutputContainer == null)
                    {
                        MessageBox.Show(String.Format($"Спочатку створіть стеганоповідомлення!"), "Помилка!");
                        return;
                    }
                    item = OutputContainer;
                    break;
                default:
                    if (InputContainer == null)
                    {
                        MessageBox.Show(String.Format($"Спочатку завантажте зображення!"), "Помилка!");
                        return;
                    }
                    item = InputContainer;
                    break;
            }

            ISteganographycAlgorithm algorithm;

            switch (computingModeCombo.SelectedIndex)
            {
                case 0:
                    algorithm = new FourierTransformSteganography();
                    break;
                case 1:
                    int acceleratorIndex = acceleratorCombo.SelectedIndex;
                    algorithm = new FourierTransformSteganographyOpenCL(CompilerToSelect, acceleratorIndex);
                    break;
                default:
                    algorithm = new FourierTransformSteganography();
                    break;
            }

            bool result;
            int resultCount;
            Stopwatch stopwatch = new Stopwatch();

            switch (analizeMethodCombo.SelectedIndex)
            {
                case 0:
                    {
                        stopwatch.Start();
                        result = algorithm.CheckAuthenticity1(item, item.BlockCount, ColorChannel.Blue);
                        stopwatch.Stop();
                    }
                    if (result)
                    {
                        MessageBox.Show($"Автентичність стеганоповідомлення підтверджено!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Успіх!");
                    }
                    else
                    {
                        MessageBox.Show($"Стеганоповідомлення пошкоджено, або відсутня ДІ!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.");
                    }
                    break;
                case 1:

                    stopwatch.Restart();
                    result = algorithm.CheckAuthenticity2(item, item.BlockCount, ColorChannel.Blue);
                    stopwatch.Stop();
                    if (result)
                    {
                        MessageBox.Show($"Автентичність стеганоповідомлення підтверджено!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Успіх!");
                    }
                    else
                    {
                        MessageBox.Show($"Стеганоповідомлення пошкоджено, або відсутня ДІ!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.");
                    }
                    break;
                case 2:
                    stopwatch.Restart();
                    resultCount = algorithm.SpartialDetect(item, item.BlockCount, ColorChannel.Blue);
                    stopwatch.Stop();
                    int evenPart = (int)((float)resultCount / (float)item.BlockCount * 100);
                    bool isSteganomessage1 = false;
                    if (evenPart > 75)
                    {
                        isSteganomessage1 = true;
                    }
                    if (isSteganomessage1)
                    {
                        MessageBox.Show($"ДІ виявлено!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Успіх!");
                    }
                    else
                    {
                        MessageBox.Show($"Кількість повністю парних, непрних або з однаковою кількістю парних/непарних елементів блоків у просторовій області: {resultCount}\n" +
                            $"Загальна кількість блоків у контейнері: {item.BlockCount}\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Обробку не виявлено!");
                    }
                    break;
                case 3:
                    stopwatch.Restart();
                    resultCount = algorithm.FrequencyDetect(item, item.BlockCount, ColorChannel.Blue);
                    stopwatch.Stop();
                    int evenPart2 = (int)((float)resultCount / (float)item.BlockCount * 100);
                    bool isSteganomessage2 = false;
                    if (evenPart2 > 55)
                    {
                        isSteganomessage2 = true;
                    }
                    if (isSteganomessage2)
                    {
                        MessageBox.Show($"ДІ виявлено!\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "Успіх!");
                    }
                    else
                    {
                        MessageBox.Show($"Кількість повністю парних або непарних блоків у частотній області: {resultCount}\n" +
                            $"Загальна кількість блоків у контейнері: {item.BlockCount}\nЧас виконання: {stopwatch.ElapsedMilliseconds} ms.", "ДІ не виявлено!");
                    }
                    break;
                    //default:
                    //    item = InputContainer;
                    //    break;
            }

        }
    }
}
