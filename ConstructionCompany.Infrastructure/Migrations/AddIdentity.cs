using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace ConstructionCompany.Infrastructure.Migrations;
[DbContext(typeof(ConstructionCompanyContext))]
[Migration("20261126000200_AddIdentity")]
public sealed class AddIdentity : Migration
{
    protected override void Up(MigrationBuilder m) {
        m.Sql("CREATE TABLE AspNetRoles (Id TEXT PRIMARY KEY NOT NULL, Name TEXT, NormalizedName TEXT, ConcurrencyStamp TEXT);");
        m.Sql("CREATE TABLE AspNetUsers (Id TEXT PRIMARY KEY NOT NULL, DisplayName TEXT NOT NULL DEFAULT '', UserName TEXT, NormalizedUserName TEXT, Email TEXT, NormalizedEmail TEXT, EmailConfirmed INTEGER NOT NULL, PasswordHash TEXT, SecurityStamp TEXT, ConcurrencyStamp TEXT, PhoneNumber TEXT, PhoneNumberConfirmed INTEGER NOT NULL, TwoFactorEnabled INTEGER NOT NULL, LockoutEnd TEXT, LockoutEnabled INTEGER NOT NULL, AccessFailedCount INTEGER NOT NULL);");
        m.Sql("CREATE UNIQUE INDEX RoleNameIndex ON AspNetRoles(NormalizedName);");
        m.Sql("CREATE UNIQUE INDEX UserNameIndex ON AspNetUsers(NormalizedUserName);");
        m.Sql("CREATE INDEX EmailIndex ON AspNetUsers(NormalizedEmail);");
        m.Sql("CREATE TABLE AspNetRoleClaims (Id INTEGER PRIMARY KEY AUTOINCREMENT, RoleId TEXT NOT NULL REFERENCES AspNetRoles(Id) ON DELETE CASCADE, ClaimType TEXT, ClaimValue TEXT);");
        m.Sql("CREATE TABLE AspNetUserClaims (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE, ClaimType TEXT, ClaimValue TEXT);");
        m.Sql("CREATE TABLE AspNetUserLogins (LoginProvider TEXT NOT NULL, ProviderKey TEXT NOT NULL, ProviderDisplayName TEXT, UserId TEXT NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE, PRIMARY KEY(LoginProvider,ProviderKey));");
        m.Sql("CREATE TABLE AspNetUserRoles (UserId TEXT NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE, RoleId TEXT NOT NULL REFERENCES AspNetRoles(Id) ON DELETE CASCADE, PRIMARY KEY(UserId,RoleId));");
        m.Sql("CREATE TABLE AspNetUserTokens (UserId TEXT NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE, LoginProvider TEXT NOT NULL, Name TEXT NOT NULL, Value TEXT, PRIMARY KEY(UserId,LoginProvider,Name));");
        m.Sql("CREATE INDEX IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims(RoleId);");
        m.Sql("CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims(UserId);");
        m.Sql("CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins(UserId);");
        m.Sql("CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId);");
    }
    protected override void Down(MigrationBuilder m) {
        foreach(var table in new[]{"AspNetUserTokens","AspNetUserRoles","AspNetUserLogins","AspNetUserClaims","AspNetRoleClaims","AspNetUsers","AspNetRoles"}) m.DropTable(table);
    }
}
