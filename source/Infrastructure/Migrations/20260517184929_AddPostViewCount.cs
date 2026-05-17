using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostViewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QTD_VISUALIZACOES",
                table: "T_POST",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QTD_VISUALIZACOES",
                table: "T_POST");
        }
    }
}
