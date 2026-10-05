using System;

namespace Sw3V15
{
    public class VideoRecorder : IDisposable
    {
        private bool _disposed = false;
        private string _outputFile;
        private bool _isRecording;

        public string OutputFile
        {
            get { return _outputFile; }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
        }

        public VideoRecorder(string outputFile)
        {
            _outputFile = outputFile;
            _isRecording = false;
            Console.WriteLine($"Відеозаписувач створено, файл: {_outputFile}");
        }

        public void StartRecording()
        {
            if (_disposed)
            {
                Console.WriteLine("Неможливо почати запис: ресурс вже звільнено.");
                return;
            }

            _isRecording = true;
            Console.WriteLine($"Запис розпочато у файл \"{_outputFile}\".");
        }

        public void StopRecording()
        {
            if (_isRecording)
            {
                _isRecording = false;
                Console.WriteLine($"Запис у файл \"{_outputFile}\" зупинено.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine("Звільнення керованих ресурсів VideoRecorder.");
                }

                if (_isRecording)
                {
                    StopRecording();
                }

                Console.WriteLine($"Ресурси відеозаписувача \"{_outputFile}\" звільнено.");
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VideoRecorder()
        {
            Dispose(false);
        }
    }

    class Program
    {
        static void UsingScenario()
        {
            Console.WriteLine("Сценарій 1: using");
            using (var recorder = new VideoRecorder("video1.mp4"))
            {
                recorder.StartRecording();
                recorder.StopRecording();
            }
            Console.WriteLine();
        }

        static void ExplicitDisposeScenario()
        {
            Console.WriteLine("Сценарій 2: явний виклик Dispose()");
            var recorder = new VideoRecorder("video2.mp4");
            recorder.StartRecording();
            recorder.StopRecording();
            recorder.Dispose();
            Console.WriteLine();
        }

        static void NoDisposeScenario()
        {
            Console.WriteLine("Сценарій 3: без Dispose(), через GC");
            var recorder = new VideoRecorder("video3.mp4");
            recorder.StartRecording();
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            UsingScenario();
            ExplicitDisposeScenario();
            NoDisposeScenario();

            Console.WriteLine("Завершення Main, примусовий виклик GC");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}