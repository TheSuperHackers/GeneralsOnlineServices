using Microsoft.EntityFrameworkCore;

// TODO_EFCORE: When updating this, make sure we preserve old ON DUPLICATE behavior, to overwrite the old data since day_of_year key will be re-used
public class DailyStat
{
	public DailyStat()
	{
		DayOfYear = DateTime.Now.DayOfYear;
		Stats = new();
	}

	public int DayOfYear { get; set; } = -1;
	public DailyStatsStructure Stats { get; set; } = null;
}

public class DailyStatsStructure
{
	public const int numSides = 12;
	public int[] matches { get; set; } = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	public int[] wins { get; set; } = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
}

public static class DailyStatsManager
{
	public static DailyStat g_StatsContainer = new();

	public static async Task LoadFromDB(AppDbContext db)
	{
		int day_of_year = DateTime.Now.DayOfYear;
		g_StatsContainer = await db.DailyStats.FirstOrDefaultAsync(x => x.DayOfYear == day_of_year);

		// if null, instantiate, but dont save immediately, let the normal save timer handle it
		if (g_StatsContainer == null)
		{
			g_StatsContainer = new DailyStat();
		}
	}

	public static async Task SaveToDB(AppDbContext db)
	{
		//await Database.Functions.Auth.StoreDailyStats(GlobalDatabaseInstance.g_Database, g_Stats);

		int day_of_year = DateTime.Now.DayOfYear;

		var entity = await db.DailyStats
			.FirstOrDefaultAsync(x => x.DayOfYear == day_of_year);

		// Insert if new, otherwise update
		if (entity == null)
		{
			entity = g_StatsContainer;
			db.DailyStats.Add(entity);
		}
		else
		{
			entity.Stats = g_StatsContainer.Stats;
			db.DailyStats.Update(entity);
		}

		await db.SaveChangesAsync();
	}

	public static void RegisterOutcome(int army, bool bWon)
	{
		try
		{
			int armyIndex = army - 2; // teams start at 2, so substract for array indices

			if (armyIndex >= 0 && armyIndex <= 11)
			{
				++g_StatsContainer.Stats.matches[armyIndex];

				if (bWon)
				{
					++g_StatsContainer.Stats.wins[armyIndex];
				}

				// clamp to a sane value, just incase (wins can never be more than matches)
				if (g_StatsContainer.Stats.wins[armyIndex] > g_StatsContainer.Stats.matches[armyIndex])
				{
					g_StatsContainer.Stats.wins[armyIndex] = g_StatsContainer.Stats.matches[armyIndex];

				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] RegisterOutcome failed: {ex.Message}");
		}
	}
}