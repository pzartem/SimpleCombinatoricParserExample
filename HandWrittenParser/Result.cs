namespace HandWrittenParser
{
	public struct Result<T>
	{
		public T Value { get; }
		public bool WasSuccessful { get; }
		public string Message { get; }
		public Input Remainder { get; set; }

		public Result(string errorMessage)
		{
			Value = default(T);
			Remainder = default(Input);

			Message = errorMessage;
			WasSuccessful = false;
		}

		public Result(T value, Input remainder, bool wasSuccessful, string errorMessage)
		{
			Value = value;
			WasSuccessful = wasSuccessful;
			Message = errorMessage;
			Remainder = remainder;
		}

		public static Result<T> Success(T inputCurrent, Input getNext)
		{
			return new Result<T>(inputCurrent, getNext, true, string.Empty);
		}

		public static Result<T> Failure(string errrorMessage)
		{
			return new Result<T>(errrorMessage);
		}
	}
}
