using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJogadorFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_JOGADOR_FOTO",
                columns: table => new
                {
                    ID_JOGADOR_FOTO = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_PLAYER_ID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TX_PLAYER_NAME = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TX_NOME_ARQUIVO = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    ID_USUARIO = table.Column<Guid>(type: "uuid", nullable: true),
                    DT_CRIADO_EM = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_CRIADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_MODIFICACAO = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_MODIFICADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FL_EXCLUIDO = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_JOGADOR_FOTO", x => x.ID_JOGADOR_FOTO);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JOGADOR_FOTO_PLAYER_ID",
                table: "T_JOGADOR_FOTO",
                column: "TX_PLAYER_ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_JOGADOR_FOTO");
        }
    }
}
