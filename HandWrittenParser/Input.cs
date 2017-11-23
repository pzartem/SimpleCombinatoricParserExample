namespace HandWrittenParser
{
	public struct Input
	{
		public string Source { get; }
		public int Position { get; }

		public char Current => Source[Position];
		public bool AtEnd => Position == Source.Length;

		public Input(string source)
		{
			Source = source;
			Position = 0;
		}

		public Input(string source, int position)
		{
			Source = source;
			Position = position;
		}

		public Input GetRemainder() => new Input(Source, Position + 1);
	}
}