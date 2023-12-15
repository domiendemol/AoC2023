namespace AoC2023;

public class Day15
{
	public void Run(List<string> input)
	{
		string line = input[0];

		Console.WriteLine($"PART 1: {Part1(line)}");
		Console.WriteLine($"PART 2: {Part2(line)}");
	}
	
	int Part1(string line)
	{
		int part1 = 0;
		foreach (string part in line.Split(',')) part1 += Hash(part);
		return part1;
	}
	
	int Part2(string line)
	{
		Box[] boxes = new Box[256];
		for (int i = 0; i < boxes.Length; i++) boxes[i] = new Box();
		
		foreach (string part in line.Split(','))
		{
			bool dash = part.Last() == '-';
			string label = (dash) ? part.Substring(0, part.Length - 1) : part.Substring(0, part.IndexOf('='));
			int hash = Hash(label);

			if (dash) {
				boxes[hash].RemoveLens(label);
			}
			else {
				boxes[hash].AddLens(label, Int32.Parse(part.Substring(part.IndexOf('=')+1)));
			}
		}

		return boxes.Select((box, index) => box.GetFocusingPower() * (index + 1)).Sum();
	}
	

	int Hash(string str)
	{
		int hash = 0;
		Array.ForEach(str.ToCharArray(),c => hash = Hash(c, hash));
		return hash;
	}
	
	int Hash(char c, int currentHash) => ((currentHash + (int) c) * 17) % 256;

	class Box
	{
		private LinkedList<Lens> _lenses = new LinkedList<Lens>();

		public void RemoveLens(string label)
		{
			Lens existing = _lenses.FirstOrDefault(lens => lens.label.Equals(label), null);
			if (existing != null) _lenses.Remove(existing);
		}

		public void AddLens(string label, int focalLength)
		{
			Lens existing = _lenses.FirstOrDefault(lens => lens.label.Equals(label), null);
			if (existing != null)
				existing.focalLength = focalLength;
			else
			{
				Lens newLens = new Lens(focalLength, label);
				_lenses.AddLast(newLens);
			}
		}

		public int GetFocusingPower()
		{
			return _lenses.Select((lens, index) => lens.focalLength * (index + 1)).Sum();
		}
	}

	class Lens
	{
		public int focalLength;
		public string label;

		public Lens(int focalLength, string label)
		{
			this.focalLength = focalLength;
			this.label = label;
		}
	}
}