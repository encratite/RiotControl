namespace RiotControl
{
	class ItemInformation
	{
		public int Id;
		public string Name;
		public string Description;
		public bool UnknownItem;

		public ItemInformation(int id, string name, string description, bool unknownItem = false)
		{
			Id = id;
			Name = name;
			Description = description;
			UnknownItem = unknownItem;
		}
	}
}
