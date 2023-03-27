using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HallHaven.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditHour",
                columns: table => new
                {
                    CreditHourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditHourName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Classification = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditHour", x => x.CreditHourId);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    GenderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.GenderId);
                });

            migrationBuilder.CreateTable(
                name: "Major",
                columns: table => new
                {
                    MajorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MajorName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Major", x => x.MajorId);
                });

            migrationBuilder.CreateTable(
                name: "Dorm",
                columns: table => new
                {
                    DormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DormName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    CreditHourId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dorm", x => x.DormId);
                    table.ForeignKey(
                        name: "FK_Dorm_CreditHour",
                        column: x => x.CreditHourId,
                        principalTable: "CreditHour",
                        principalColumn: "CreditHourId");
                    table.ForeignKey(
                        name: "FK_Dorm_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "GenderId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenderId = table.Column<int>(type: "int", nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ProfileBio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true, computedColumnSql: "(([FirstName]+' ')+[LastName])", stored: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "GenderId");
                });

            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DormId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MajorId = table.Column<int>(type: "int", nullable: false),
                    CreditHourId = table.Column<int>(type: "int", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    IsCandiateStudent = table.Column<bool>(type: "bit", nullable: false),
                    IsStudentAthlete = table.Column<bool>(type: "bit", nullable: false),
                    NeatnessLevel = table.Column<int>(type: "int", nullable: false),
                    VisitorLevel = table.Column<int>(type: "int", nullable: false),
                    FitnessLevel = table.Column<int>(type: "int", nullable: false),
                    AcademicLevel = table.Column<int>(type: "int", nullable: false),
                    SocialLevel = table.Column<int>(type: "int", nullable: false),
                    SharingLevel = table.Column<int>(type: "int", nullable: false),
                    BackgroundNoiseLevel = table.Column<int>(type: "int", nullable: false),
                    BedTimeRanking = table.Column<int>(type: "int", nullable: false),
                    ModestyLevel = table.Column<int>(type: "int", nullable: false),
                    NumberOfBelongings = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_Form_CreditHour_CreditHourId",
                        column: x => x.CreditHourId,
                        principalTable: "CreditHour",
                        principalColumn: "CreditHourId");
                    table.ForeignKey(
                        name: "FK_Form_Dorm_DormId",
                        column: x => x.DormId,
                        principalTable: "Dorm",
                        principalColumn: "DormId");
                    table.ForeignKey(
                        name: "FK_Form_Major_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Major",
                        principalColumn: "MajorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Form_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApplicationUser1Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    User2Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApplicationUser2Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsMatched = table.Column<bool>(type: "bit", nullable: false),
                    SimilarityPercentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Matches_Users_User1Id",
                        column: x => x.User1Id,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Matches_Users_User2Id",
                        column: x => x.User2Id,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dorm_CreditHourId",
                table: "Dorm",
                column: "CreditHourId");

            migrationBuilder.CreateIndex(
                name: "IX_Dorm_GenderId",
                table: "Dorm",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_CreditHourId",
                table: "Form",
                column: "CreditHourId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_DormId",
                table: "Form",
                column: "DormId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_MajorId",
                table: "Form",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_UserId",
                table: "Form",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User1Id",
                table: "Matches",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User2Id",
                table: "Matches",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GenderId",
                table: "Users",
                column: "GenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Form");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Dorm");

            migrationBuilder.DropTable(
                name: "Major");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CreditHour");

            migrationBuilder.DropTable(
                name: "Gender");
        }
    }
}
