namespace Code.World.GUI.Homes;

public sealed class MilMo_HomeVote
{
	public sealed class RatingData
	{
		internal readonly float MCurrent;

		internal readonly int MVotes;

		internal readonly int MMinVotes;

		public RatingData(float current, int votes, int minVotes)
		{
			MCurrent = current;
			MVotes = votes;
			MMinVotes = minVotes;
		}
	}

	private RatingData _mRatingData = new RatingData(0f, 0, 15);
}
