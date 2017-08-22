using System;
using System.Diagnostics;
using System.Linq;

namespace GitlabCmd.Console.Utilities
{
    public sealed class ResultCommonLogic
    {
        public bool IsFailure { get; }
        public bool IsSuccess => !IsFailure;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _error;

        public string Error
        {
            [DebuggerStepThrough]
            get
            {
                if (IsSuccess)
                    throw new InvalidOperationException("There is no error message for success.");

                return _error;
            }
        }

        [DebuggerStepThrough]
        public ResultCommonLogic(bool isFailure, string error)
        {
            if (isFailure)
            {
                if (string.IsNullOrEmpty(error))
                    throw new ArgumentNullException(nameof(error), "There must be error message for failure.");
            }
            else
            {
                if (error != null)
                    throw new ArgumentException("There should be no error message for success.", nameof(error));
            }

            IsFailure = isFailure;
            _error = error;
        }
    }

    public struct Result
    {
        private static readonly Result _okResult = new Result(false, null);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic _logic;

        public bool IsFailure => _logic.IsFailure;
        public bool IsSuccess => _logic.IsSuccess;
        public string Error => _logic.Error;

        [DebuggerStepThrough]
        private Result(bool isFailure, string error) => _logic = new ResultCommonLogic(isFailure, error);

        [DebuggerStepThrough]
        public static Result Ok() => _okResult;

        [DebuggerStepThrough]
        public static Result Fail(string error) => new Result(true, error);

        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value) => new Result<T>(false, value, null);

        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string error) => new Result<T>(true, default, error);

        [DebuggerStepThrough]
        public static Result<T> Fail<T>(Result result) => new Result<T>(true, default, result.Error);

        /// <summary>
        /// Returns first failure in the list of <paramref name="results"/>. If there is no failure returns success.
        /// </summary>
        /// <param name="results">List of results.</param>
        [DebuggerStepThrough]
        public static Result FirstFailureOrSuccess(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                    return Fail(result.Error);
            }

            return Ok();
        }

        /// <summary>
        /// Returns failure which combined from all failures in the <paramref name="results"/> list. Error messages are separated by <paramref name="errorMessagesSeparator"/>. 
        /// If there is no failure returns success.
        /// </summary>
        /// <param name="errorMessagesSeparator">Separator for error messages.</param>
        /// <param name="results">List of results.</param>
        [DebuggerStepThrough]
        public static Result Combine(string errorMessagesSeparator, params Result[] results)
        {
            var failedResults = results.Where(x => x.IsFailure).ToList();

            if (!failedResults.Any())
                return Ok();

            string errorMessage = string.Join(errorMessagesSeparator, failedResults.Select(x => x.Error).ToArray());
            return Fail(errorMessage);
        }

        [DebuggerStepThrough]
        public static Result Combine(params Result[] results) => Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(params Result<T>[] results) => Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(string errorMessagesSeparator, params Result<T>[] results)
        {
            Result[] untyped = results.Select(result => (Result)result).ToArray();
            return Combine(errorMessagesSeparator, untyped);
        }
    }

    public struct Result<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic _logic;

        public bool IsFailure => _logic.IsFailure;
        public bool IsSuccess => _logic.IsSuccess;
        public string Error => _logic.Error;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T _value;

        public T Value
        {
            [DebuggerStepThrough]
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("There is no value for failure.");

                return _value;
            }
        }

        [DebuggerStepThrough]
        internal Result(bool isFailure, T value, string error)
        {
            if (!isFailure && value == null)
                throw new ArgumentNullException(nameof(value));

            _logic = new ResultCommonLogic(isFailure, error);
            _value = value;
        }

        public static implicit operator Result(Result<T> result) =>
            result.IsSuccess ? Result.Ok() : Result.Fail(result.Error);
    }
}