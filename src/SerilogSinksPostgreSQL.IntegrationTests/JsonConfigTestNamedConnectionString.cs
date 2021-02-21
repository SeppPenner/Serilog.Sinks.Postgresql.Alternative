﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigTestNamedConnectionString.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests for creating PostgreSql logger from a JSON configuration with named connection strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SerilogSinksPostgreSQL.IntegrationTests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.Extensions.Configuration;

    using Serilog;
    using Serilog.Events;

    using SerilogSinksPostgreSQL.IntegrationTests.Objects;

    using Xunit;

    /// <summary>
    /// Tests for creating PostgreSql logger from a JSON configuration with named connection strings.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class JsonConfigTestNamedConnectionString : BaseTests
    {
        /// <summary>
        /// The test logs.
        /// </summary>
        private const string TableName = "TestLogs";

        /// <summary>
        /// The database helper.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private readonly DbHelper dbHelper = new DbHelper(ConnectionString);

        /// <summary>
        ///     This method is used to test the logger creation from the configuration.
        /// </summary>
        [Fact]
        public void ShouldCreateLoggerFromConfig()
        {
            this.dbHelper.RemoveTable(string.Empty, TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(".\\PostgreSinkConfigurationConnectionString.json", false, true)
                .Build();

            var logger = new LoggerConfiguration().WriteTo.PostgreSQL(
                ConnectionString,
                TableName,
                null,
                LogEventLevel.Verbose,
                null,
                null,
                30,
                null,
                false,
                string.Empty,
                true,
                false,
                null,
                configuration).CreateLogger();

            const int RowsCount = 2;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "{@LogEvent} {TestProperty}",
                    testObject,
                    "TestValue");
            }

            Log.CloseAndFlush();

            var actualRowsCount = this.dbHelper.GetTableRowsCount(string.Empty, TableName);

            Assert.Equal(RowsCount, actualRowsCount);
        }
    }
}
