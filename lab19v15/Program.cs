using System;

namespace Lab19V15
{
    public class MessageProcessingException : Exception
    {
        public string MessageId { get; } = "";
        public string Details { get; } = "";

        public MessageProcessingException()
        {
        }

        public MessageProcessingException(string message) : base(message)
        {
        }

        public MessageProcessingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MessageProcessingException(string messageId, string details) : base("Помилка обробки повідомлення " + messageId + ": " + details)
        {
            MessageId = messageId;
            Details = details;
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string Error { get; }

        private Result(T? value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, "");
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(default, false, error);
        }

        public static Result<T> Failure(Exception ex)
        {
            return new Result<T>(default, false, ex.Message);
        }
    }

    public class MessageProcessor
    {
        private int _nextId = 1;

        public void ProcessMessage(string message)
        {
            string messageId = "MSG-" + _nextId;
            _nextId++;

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new MessageProcessingException(messageId, "повідомлення порожнє");
            }

            if (!message.Contains(":"))
            {
                throw new MessageProcessingException(messageId, "повідомлення пошкоджене: немає роздільника ':'");
            }

            Console.WriteLine(messageId + " оброблено: " + message);
        }

        public Result<bool> SendMessage(string message)
        {
            int separatorIndex = message.IndexOf(':');

            if (separatorIndex < 0)
            {
                return Result<bool>.Failure("у повідомленні немає отримувача");
            }

            string recipient = message.Substring(0, separatorIndex).Trim();

            if (!recipient.Contains("@"))
            {
                return Result<bool>.Failure("отримувач \"" + recipient + "\" невалідний: немає символу @");
            }

            Console.WriteLine("Повідомлення надіслано: " + recipient);
            return Result<bool>.Success(true);
        }
    }

    class Program
    {
        static void TryProcess(MessageProcessor processor, string message)
        {
            try
            {
                processor.ProcessMessage(message);
            }
            catch (MessageProcessingException ex)
            {
                Console.WriteLine("Перехоплено MessageProcessingException, Id: " + ex.MessageId + ", деталі: " + ex.Details);
            }
        }

        static void TrySend(MessageProcessor processor, string message)
        {
            Result<bool> result = processor.SendMessage(message);

            if (result.IsSuccess)
            {
                Console.WriteLine("Успіх: " + result.Value);
            }
            else
            {
                Console.WriteLine("Помилка: " + result.Error);
            }
        }

        static void Main(string[] args)
        {
            MessageProcessor processor = new MessageProcessor();

            Console.WriteLine("ProcessMessage: власний виняток MessageProcessingException");
            TryProcess(processor, "ivan@mail.com: Привіт");
            TryProcess(processor, "");
            TryProcess(processor, "Привіт без роздільника");

            Console.WriteLine();
            Console.WriteLine("SendMessage: результат Result<bool>");
            TrySend(processor, "ivan@mail.com: Привіт");
            TrySend(processor, "ivan: Привіт");
            TrySend(processor, "Привіт без отримувача");

            Console.WriteLine();
            Console.WriteLine("Result.Failure з винятку");
            try
            {
                processor.ProcessMessage("");
            }
            catch (MessageProcessingException ex)
            {
                Result<bool> fromException = Result<bool>.Failure(ex);
                Console.WriteLine("IsSuccess: " + fromException.IsSuccess);
                Console.WriteLine("Error: " + fromException.Error);
            }
        }
    }
}