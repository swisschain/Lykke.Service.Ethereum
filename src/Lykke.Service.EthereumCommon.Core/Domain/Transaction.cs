﻿using System;
using System.Numerics;

namespace Lykke.Service.EthereumCommon.Core.Domain
{
    public class Transaction
    {
        private Transaction(
            BigInteger amount,
            DateTime builtOn,
            string data,
            string from,
            TransactionState state,
            string to,
            Guid transactionId)
        {
            Amount = amount;
            BuiltOn = builtOn;
            Data = data;
            From = from;
            State = state;
            To = to;
            TransactionId = transactionId;
        }

        internal Transaction(
            BigInteger amount,
            BigInteger? blockNumber,
            DateTime? broadcastedOn,
            DateTime builtOn,
            DateTime? completedOn,
            string data,
            DateTime? deletedOn,
            string error,
            string from,
            string hash,
            string signedData,
            TransactionState state,
            string to,
            Guid transactionId)
        {
            Amount = amount;
            BlockNumber = blockNumber;
            BroadcastedOn = broadcastedOn;
            BuiltOn = builtOn;
            CompletedOn = completedOn;
            Data = data;
            DeletedOn = deletedOn;
            Error = error;
            From = from;
            Hash = hash;
            SignedData = signedData;
            State = state;
            To = to;
            TransactionId = transactionId;
        }
        
        public static Transaction Build(
            Guid transactionId,
            string from,
            string to,
            BigInteger amount,
            string data)
        {
            return new Transaction
            (
                amount: amount,
                builtOn: DateTime.UtcNow,
                data: data,
                from: from,
                transactionId: transactionId,
                state: TransactionState.Built,
                to: to
            );
        }
        
        
        
        public BigInteger Amount { get; }

        public BigInteger? BlockNumber { get; private set; }
        
        public DateTime? BroadcastedOn { get; private set; }
        
        public DateTime BuiltOn { get; }
        
        public DateTime? CompletedOn { get; private set; }

        public string Data { get; }
        
        public DateTime? DeletedOn { get; private set; }
        
        public string Error { get; private set; }
        
        public string From { get; }

        public string Hash { get; private set; }

        public string SignedData { get; private set; }

        public TransactionState State { get; private set; }

        public string To { get; }
        
        public Guid TransactionId { get; }


        public void OnBroadcasted(
            string signedData,
            string hash)
        {
            if (State == TransactionState.Built)
            {
                BroadcastedOn = DateTime.UtcNow;
                Hash = hash;
                SignedData = signedData;
                State = TransactionState.InProgress;
            }
            else
            {
                throw new InvalidOperationException
                (
                    $"Transaction can not be broadcasted from current [{State.ToString()}] state."
                );
            }
        }

        public void OnSucceded(
            BigInteger blockNumber)
        {
            if (State == TransactionState.InProgress)
            {
                BlockNumber = blockNumber;
                CompletedOn = DateTime.UtcNow;
                State = TransactionState.Completed;
            }
            else
            {
                throw new InvalidOperationException
                (
                    $"Transaction can not succeed from current [{State.ToString()}] state."
                );
            }
        }

        public void OnFailed(
            BigInteger blockNumber,
            string error)
        {
            if (State == TransactionState.InProgress)
            {
                BlockNumber = blockNumber;
                CompletedOn = DateTime.UtcNow;
                Error = error;
                State = TransactionState.Failed;
            }
            else
            {
                throw new InvalidOperationException
                (
                    $"Transaction can not fail from current [{State.ToString()}] state."
                );
            }
        }
        
        public void OnDeleted()
        {
            if (State != TransactionState.Deleted)
            {
                DeletedOn = DateTime.UtcNow;
                State = TransactionState.Deleted;
            }
        }
    }
}
