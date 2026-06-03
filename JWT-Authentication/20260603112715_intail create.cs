using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWT_Authentication.Migrations
{
    public partial class intailcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(MAX) NOT NULL,
        [Email] NVARCHAR(MAX) NOT NULL,
        [Password] NVARCHAR(MAX) NOT NULL
    );
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Users];
END
");
        }
    }
}
