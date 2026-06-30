using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPalpites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_PALPITE",
                columns: table => new
                {
                    ID_PALPITE = table.Column<Guid>(type: "uuid", nullable: false),
                    ID_EVENTO = table.Column<int>(type: "integer", nullable: false),
                    TX_TIME_CASA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TX_TIME_FORA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    QTD_GOLS_CASA = table.Column<int>(type: "integer", nullable: false),
                    QTD_GOLS_FORA = table.Column<int>(type: "integer", nullable: false),
                    TX_APELIDO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DT_CRIADO_EM = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_CRIADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_MODIFICACAO = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_MODIFICADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FL_EXCLUIDO = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PALPITE", x => x.ID_PALPITE);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PALPITE_EVENTO_APELIDO",
                table: "T_PALPITE",
                columns: new[] { "ID_EVENTO", "TX_APELIDO" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_PALPITE");
        }
    }
}
