using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPalpiteStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TX_FASE",
                table: "T_PALPITE",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TX_FASE",
                table: "T_PALPITE");
        }
    }
}
