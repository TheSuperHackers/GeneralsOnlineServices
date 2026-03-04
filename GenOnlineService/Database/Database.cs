using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AppDbContext : DbContext
{
	public DbSet<User> Users => Set<User>();
	public DbSet<DailyStat> DailyStats => Set<DailyStat>();

	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Fluent configuration here
		base.OnModelCreating(modelBuilder);

		// Fluent config for user
		modelBuilder.Entity<User>(entity =>
		{
			entity.ToTable("users");

			// prim key
			entity.HasKey(e => e.ID);

			entity.Property(e => e.ID).HasColumnName("user_id");

			entity.Property(e => e.AccountType).HasColumnName("account_type");
			entity.Property(e => e.SteamID).HasColumnName("steam_id");
			entity.Property(e => e.DiscordID).HasColumnName("discord_id");
			entity.Property(e => e.DiscordUsername).HasColumnName("discord_username").HasColumnType("varchar(32)"); ;
			entity.Property(e => e.GameReplaysID).HasColumnName("gamereplays_id");
			entity.Property(e => e.GameReplaysUsername).HasColumnName("gamereplays_username").HasColumnType("varchar(32)"); ;
			entity.Property(e => e.DisplayName).HasColumnName("displayname").HasColumnType("varchar(32)"); ;
			entity.Property(e => e.LastLogin).HasColumnName("lastlogin").HasColumnType("datetime(6)");
			entity.Property(e => e.LastIPAddress).HasColumnName("last_ip").HasColumnType("varchar(45)"); ;
			entity.Property(e => e.ClientID).HasColumnName("client_id");
			entity.Property(e => e.FavoriteColor).HasColumnName("favorite_color");
			entity.Property(e => e.FavoriteSide).HasColumnName("favorite_side");
			entity.Property(e => e.FavoriteMap).HasColumnName("favorite_map").HasColumnType("varchar(128)"); ;
			entity.Property(e => e.FavoriteStartingMoney).HasColumnName("favorite_starting_money");
			entity.Property(e => e.LimitSuperweapons).HasColumnName("favorite_limit_superweapons");
			entity.Property(e => e.IsAdmin).HasColumnName("admin");
			entity.Property(e => e.IsBanned).HasColumnName("banned");
			entity.Property(e => e.EloRating).HasColumnName("elo_rating");
			entity.Property(e => e.EloNumberOfMatches).HasColumnName("elo_num_matches");
			entity.Property(e => e.BanReason).HasColumnName("ban_reason").HasColumnType("varchar(128)"); ;
			entity.Property(e => e.BannedBy).HasColumnName("banned_by").HasColumnType("varchar(50)"); ;
			entity.Property(e => e.BanVerifiedBy).HasColumnName("ban_verified_by").HasColumnType("varchar(50)"); ;
			entity.Property(e => e.BanAliases).HasColumnName("ban_alises").HasColumnType("varchar(50)"); ;
		});

		// Fluent config for daily stats
		modelBuilder.Entity<DailyStat>(entity =>
		{
			entity.ToTable("daily_stats");

			// prim key
			entity.HasKey(e => e.DayOfYear);

			entity.Property(e => e.DayOfYear).HasColumnName("day_of_year");

			// TODO_EFCORE: use column type json later (needs db update)

			entity.Property(e => e.Stats)
				.HasColumnName("stats_structure")
				.HasColumnType("longtext")
				.HasConversion(
					v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
					v => JsonSerializer.Deserialize<DailyStatsStructure>(v, (JsonSerializerOptions)null)
				);
		});
	}
}