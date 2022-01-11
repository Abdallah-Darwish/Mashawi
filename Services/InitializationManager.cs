using System;
using System.Collections.Generic;
using System.IO;
using Npgsql;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mashawi.Db;
using Mashawi.Resources;
using System.Reflection;
using System.Linq;
using Mashawi.Db.Entities;

namespace Mashawi.Services
{
    //works only with postgres
    public class InitializationManager
    {
        private readonly AppDbContext _dbContext;
        private readonly AppOptions _appOptions;
        private readonly IServiceProvider _serviceProvider;

        public InitializationManager(AppDbContext dbContext, IOptions<AppOptions> appOptions, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _appOptions = appOptions.Value;
            _serviceProvider = serviceProvider;
        }

       
        public async Task RecreateDb()
        {
            var dirs = new[]
            {
                BookFileManager.SaveDirectory,
            };
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }

                Directory.CreateDirectory(dir);
            }

            using (var postgreCon = new NpgsqlConnection(_appOptions.BuildPostgresConnectionString()))
            {
                await postgreCon.OpenAsync().ConfigureAwait(false);
                using (var dropCommand = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_appOptions.DbName}\" WITH (FORCE);", postgreCon))
                {
                    await dropCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                NpgsqlConnection.ClearAllPools();
                using (var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{_appOptions.DbName}\";", postgreCon))
                {
                    await createCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }

            NpgsqlConnection.ClearAllPools();
            string dbScript = await AppResourcesManager.GetText("DbInitScript.sql").ConfigureAwait(false);

            using (var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString()))
            using (var initCommand = new NpgsqlCommand(dbScript, dbCon))
            {
                await dbCon.OpenAsync().ConfigureAwait(false);
                await initCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task EnsureDb()
        {
            try
            {
                await using var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString());
                await dbCon.OpenAsync().ConfigureAwait(false);
            }
            catch
            {
                await RecreateDb().ConfigureAwait(false);
            }
        }

        private async Task Seed()
        {
            _dbContext.ChangeTracker.Clear();
            await _dbContext.Database.CloseConnectionAsync().ConfigureAwait(false);
            NpgsqlConnection.ClearAllPools();

            SeedingContext seedingContext = new();
          /*  var _book = new Book{
                    Id=1,
                    Title="Testing title",
                    Isbn="111111",
                    AuthorId=1,
                    Price=55,
                    RatersCount=2,
                    RatingSum=7,
                    IsUsed=false,
                    Description="THIS IS THE FIRST BOOK TO BE ADDED MANUALLY",
                    Language=BookLanguage.English,
                   PublishDate=DateTime.Today,
                   AddedDate=DateTime.Today,
                   Genre=BookGenre.Fantasy,
                   Stock = 20,
                   Sold=5
        };
        _dbContext.Books.Add(_book);*/

            Author.CreateSeed(seedingContext);
            OrderAddress.CreateSeed(seedingContext);
            Book.CreateSeed(seedingContext);
            User.CreateSeed(seedingContext);
            BookReview.CreateSeed(seedingContext);
            Order.CreateSeed(seedingContext);
            OrderItem.CreateSeed(seedingContext);
            CartItem.CreateSeed(seedingContext);
            WishListItem.CreateSeed(seedingContext);

            await _dbContext.Authors.AddRangeAsync(seedingContext.Authors).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.OrdersAddresses.AddRangeAsync(seedingContext.OrdersAddresses).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Books.AddRangeAsync(seedingContext.Books).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Users.AddRangeAsync(seedingContext.Users).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.Orders.AddRangeAsync(seedingContext.Orders).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.OrdersItems.AddRangeAsync(seedingContext.OrdersItems).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.CartsItems.AddRangeAsync(seedingContext.CartsItems).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.WhishListItems.AddRangeAsync(seedingContext.WhishListItems).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await _dbContext.BooksReviews.AddRangeAsync(seedingContext.BooksReviews).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            await Book.CreateSeedFiles(_serviceProvider, seedingContext).ConfigureAwait(false);

            List<string> tablesNames = new();

            await using (var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString()))
            await using (var listCommand =
                new NpgsqlCommand(
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';",
                    dbCon))
            {
                await dbCon.OpenAsync().ConfigureAwait(false);
                await using var reader = await listCommand.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    tablesNames.Add(reader.GetString(0));
                }
            }

            foreach (var table in tablesNames)
            {
                var conString = _appOptions.BuildAppConnectionString();
                try
                {
                    await using var dbCon = new NpgsqlConnection(_appOptions.BuildAppConnectionString());
                    await using var resetCommand =
                        new NpgsqlCommand(
                            $"SELECT setval(pg_get_serial_sequence('\"{table}\"', 'Id'), coalesce(max(\"Id\"),0) + 1, false) FROM \"{table}\";",
                            dbCon);
                    await dbCon.OpenAsync().ConfigureAwait(false);
                    await resetCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch (NpgsqlException)
                {
                }
            }
        }
        public void InitializeClasses()
        {
            var initializationMethods = Assembly
                            .GetExecutingAssembly()
                            .DefinedTypes
                            .Where(t => !t.IsGenericType)
                            .Select(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                          .FirstOrDefault(m => m.Name == nameof(BookFileManager.Init)))
                            .Where(m => m != null);
            var initializationMethodParams = new object[] { _serviceProvider };
            foreach (var initMethod in initializationMethods)
            {
                initMethod?.Invoke(null, initializationMethodParams);
            }
        }
        public async Task InitializeSystem()
        {
            InitializeClasses();
            await EnsureDb().ConfigureAwait(false);
        }
        public async Task RecreateAndSeedDb()
        {
            await RecreateDb().ConfigureAwait(false);
            await Seed().ConfigureAwait(false);
        }
    }
}