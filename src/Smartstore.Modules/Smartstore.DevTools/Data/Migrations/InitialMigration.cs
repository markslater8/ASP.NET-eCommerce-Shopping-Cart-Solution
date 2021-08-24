﻿using FluentMigrator;
using Smartstore.Core.Data.Migrations;
using Smartstore.Data;

namespace Smartstore.DevTools.Data.Migrations
{
    [MigrationVersion("2021-08-18 15:51:30", "Add DevTools test entity.")]
    public class InitialMigration : Migration
    {
        private const string TEST_TABLE = "DevToolsTestEntity";

        public override void Up()
        {
            var dbSystemName = DataSettings.Instance.DbFactory.DbSystem.ToString();

            if (!this.TableExists(dbSystemName, TEST_TABLE))
            {
                Create.Table(TEST_TABLE)
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("Name").AsString(400).NotNullable()
                    .WithColumn("Description").AsString(int.MaxValue).Nullable()
                    .WithColumn("PageSize").AsInt32().Nullable()
                    .WithColumn("LimitedToStores").AsBoolean().NotNullable()
                    .WithColumn("SubjectToAcl").AsBoolean().NotNullable()
                    .WithColumn("Published").AsBoolean().NotNullable()
                    .WithColumn("Deleted").AsBoolean().NotNullable()
                    .WithColumn("DisplayOrder").AsInt32().NotNullable()
                    .WithColumn("CreatedOnUtc").AsDateTime2().NotNullable()
                    .WithColumn("UpdatedOnUtc").AsDateTime2().NotNullable();
            }

            this.CreateIndex(dbSystemName, TEST_TABLE, "Deleted", "IX_Deleted")?.Ascending()?.WithOptions()?.NonClustered();
            this.CreateIndex(dbSystemName, TEST_TABLE, "DisplayOrder", "IX_DisplayOrder")?.Ascending()?.WithOptions()?.NonClustered();
            this.CreateIndex(dbSystemName, TEST_TABLE, "LimitedToStores", "IX_LimitedToStores")?.Ascending()?.WithOptions()?.NonClustered();
            this.CreateIndex(dbSystemName, TEST_TABLE, "SubjectToAcl", "IX_SubjectToAcl")?.Ascending()?.WithOptions()?.NonClustered();
        }

        public override void Down()
        {
            this.DeleteTables(TEST_TABLE);
        }
    }
}