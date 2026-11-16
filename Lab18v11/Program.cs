using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lab18v11
{
    [Serializable]
    public class PaymentGatewayException : Exception
    {
        public string GatewayName { get; } = string.Empty;
        public string ErrorCode { get; } = string.Empty;

        public PaymentGatewayException() { }

        public PaymentGatewayException(string message) : base(message) { }

        public PaymentGatewayException(string message, Exception innerException)
            : base(message, innerException) { }

        public PaymentGatewayException(string message, string gatewayName, string errorCode)
            : base(message)
        {
            GatewayName = gatewayName;
            ErrorCode = errorCode;
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

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);

        public static Result<T> Failure(string error) => new Result<T>(default, false, error);

        public static Result<T> Failure(Exception ex) => new Result<T>(default, false, ex.Message);
    }

    public class PaymentService
    {
        private readonly HashSet<string> _completedTransactions = new HashSet<string>
        {
            "TXN-1001",
            "TXN-1002",
            "TXN-1003"
        };

        public void ProcessPayment(decimal amount)
        {
            if (amount <= 0)
            {
                throw new PaymentGatewayException(
                    "Сума платежу повинна бути більшою за нуль.",
                    "LiqPayGateway",
                    "ERR_INVALID_AMOUNT"
                );
            }

            if (amount > 50000)
            {
                throw new PaymentGatewayException(
                    $"Перевищено разовий ліміт шлюзу для суми {amount} грн.",
                    "LiqPayGateway",
                    "ERR_LIMIT_EXCEEDED"
                );
            }

            Console.WriteLine($"  [PaymentService] Платіж на суму {amount} грн успішно проведено через LiqPayGateway.");
        }

        public Result<bool> RefundPayment(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return Result<bool>.Failure("Ідентифікатор транзакції не може бути порожнім.");
            }

            if (!_completedTransactions.Contains(transactionId))
            {
                return Result<bool>.Failure($"Транзакцію '{transactionId}' не знайдено в історії платежів.");
            }

            _completedTransactions.Remove(transactionId);
            return Result<bool>.Success(true);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var paymentService = new PaymentService();

            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №18 (Варіант 11): Власні винятки та патерн Result ===\n");

            Console.WriteLine("--- 1. Проведення платежів (обробка PaymentGatewayException) ---");
            decimal[] amountsToPay = { 1200m, 75000m, -50m };

            foreach (var amount in amountsToPay)
            {
                try
                {
                    Console.WriteLine($"Спроба оплати: {amount} грн...");
                    paymentService.ProcessPayment(amount);
                }
                catch (PaymentGatewayException ex)
                {
                    Console.WriteLine($"  [Помилка шлюзу {ex.GatewayName} | Код: {ex.ErrorCode}]: {ex.Message}");
                }
            }

            Console.WriteLine("\n--- 2. Повернення коштів (обробка Result<bool>) ---");
            string[] transactionsToRefund = { "TXN-1002", "TXN-9999", "" };

            foreach (var txnId in transactionsToRefund)
            {
                Console.WriteLine($"Запит на повернення для транзакції '{txnId}':");
                Result<bool> refundResult = paymentService.RefundPayment(txnId);

                if (refundResult.IsSuccess)
                {
                    Console.WriteLine($"  [Успіх] Кошти за транзакцією '{txnId}' успішно повернуто (Результат: {refundResult.Value}).");
                }
                else
                {
                    Console.WriteLine($"  [Відмова] {refundResult.Error}");
                }
            }
        }
    }
}