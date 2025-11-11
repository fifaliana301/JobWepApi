using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobWebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjouterTache_travail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Travaux_Taches_IdTache",
                table: "Travaux");

            migrationBuilder.AddColumn<string>(
                name: "MetierCode",
                table: "Activites",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activites_MetierCode",
                table: "Activites",
                column: "MetierCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Activites_Metiers_MetierCode",
                table: "Activites",
                column: "MetierCode",
                principalTable: "Metiers",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Travaux_Taches_IdTache",
                table: "Travaux",
                column: "IdTache",
                principalTable: "Taches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activites_Metiers_MetierCode",
                table: "Activites");

            migrationBuilder.DropForeignKey(
                name: "FK_Travaux_Taches_IdTache",
                table: "Travaux");

            migrationBuilder.DropIndex(
                name: "IX_Activites_MetierCode",
                table: "Activites");

            migrationBuilder.DropColumn(
                name: "MetierCode",
                table: "Activites");

            migrationBuilder.AddForeignKey(
                name: "FK_Travaux_Taches_IdTache",
                table: "Travaux",
                column: "IdTache",
                principalTable: "Taches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
