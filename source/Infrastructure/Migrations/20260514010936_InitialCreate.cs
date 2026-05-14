using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_LOG",
                columns: table => new
                {
                    ID_LOG = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TX_ENDERECO_IP = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TX_AGENTE_USUARIO = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TX_URL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TX_DESCRICAO = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TX_EMAIL_USUARIO = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NM_USUARIO = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DT_DATA_HORA = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TX_DETALHES = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LOG", x => x.ID_LOG);
                });

            migrationBuilder.CreateTable(
                name: "T_PERFIL",
                columns: table => new
                {
                    ID_PERFIL = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_CRIADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_CRIADO_EM = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_MODIFICADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_MODIFICACAO = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FL_EXCLUIDO = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NM_PERFIL = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NM_PERFIL_NORMALIZADO = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TX_CONCURRENCY_STAMP = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PERFIL", x => x.ID_PERFIL);
                });

            migrationBuilder.CreateTable(
                name: "T_PERMISSAO",
                columns: table => new
                {
                    ID_PERMISSAO = table.Column<Guid>(type: "uuid", nullable: false),
                    NM_PERMISSAO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NM_EXIBICAO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DS_PERMISSAO = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CTG_PERMISSAO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    DataModificacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModificadoPor = table.Column<string>(type: "text", nullable: true),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PERMISSAO", x => x.ID_PERMISSAO);
                });

            migrationBuilder.CreateTable(
                name: "T_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<Guid>(type: "uuid", nullable: false),
                    NM_COMPLETO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TX_CPF = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    TX_FOTO = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FL_NOTIFICACAO_EMAIL = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FL_NOTIFICACAO_SMS = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FL_CAN_PUBLISH = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TX_REFRESH_TOKEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DT_EXPIRACAO_REFRESH_TOKEN = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TX_PRIMEIRO_TOKEN_ACESSO = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DT_EXPIRACAO_PRIMEIRO_ACESSO = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TX_CRIADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_CRIADO_EM = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_MODIFICADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_MODIFICACAO = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FL_EXCLUIDO = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NM_USUARIO = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NM_USUARIO_NORMALIZADO = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TX_EMAIL = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TX_EMAIL_NORMALIZADO = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FL_EMAIL_CONFIRMADO = table.Column<bool>(type: "boolean", nullable: false),
                    TX_SENHA_HASH = table.Column<string>(type: "text", nullable: true),
                    TX_SECURITY_STAMP = table.Column<string>(type: "text", nullable: true),
                    TX_CONCURRENCY_STAMP = table.Column<string>(type: "text", nullable: true),
                    NR_TELEFONE_IDENTITY = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FL_TELEFONE_CONFIRMADO = table.Column<bool>(type: "boolean", nullable: false),
                    FL_TWO_FACTOR_HABILITADO = table.Column<bool>(type: "boolean", nullable: false),
                    DT_FIM_BLOQUEIO = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FL_BLOQUEIO_HABILITADO = table.Column<bool>(type: "boolean", nullable: false),
                    QTD_TENTATIVAS_FALHAS = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "T_PERFIL_CLAIM",
                columns: table => new
                {
                    ID_PERFIL_CLAIM = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_PERFIL = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_TIPO_CLAIM = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TX_VALOR_CLAIM = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PERFIL_CLAIM", x => x.ID_PERFIL_CLAIM);
                    table.ForeignKey(
                        name: "FK_T_PERFIL_CLAIM_T_PERFIL_ID_PERFIL",
                        column: x => x.ID_PERFIL,
                        principalTable: "T_PERFIL",
                        principalColumn: "ID_PERFIL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_POST",
                columns: table => new
                {
                    ID_POST = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_TITULO = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TX_SLUG = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    TX_RESUMO = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    TX_CONTEUDO = table.Column<string>(type: "text", nullable: false),
                    TX_CAPA_URL = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TX_CATEGORIA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AUTOR_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DT_PUBLICADO_EM = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    QTD_MINUTOS_LEITURA = table.Column<int>(type: "integer", nullable: false),
                    DT_CRIADO_EM = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_CRIADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DT_MODIFICACAO = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TX_MODIFICADO_POR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FL_EXCLUIDO = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_POST", x => x.ID_POST);
                    table.ForeignKey(
                        name: "FK_T_POST_T_USUARIO_AUTOR_ID",
                        column: x => x.AUTOR_ID,
                        principalTable: "T_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_USUARIO_CLAIM",
                columns: table => new
                {
                    ID_USUARIO_CLAIM = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_USUARIO = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_TIPO_CLAIM = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TX_VALOR_CLAIM = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USUARIO_CLAIM", x => x.ID_USUARIO_CLAIM);
                    table.ForeignKey(
                        name: "FK_T_USUARIO_CLAIM_T_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "T_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_USUARIO_LOGIN",
                columns: table => new
                {
                    TX_PROVEDOR_LOGIN = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TX_CHAVE_PROVEDOR = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NM_EXIBICAO_PROVEDOR = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ID_USUARIO = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USUARIO_LOGIN", x => new { x.TX_PROVEDOR_LOGIN, x.TX_CHAVE_PROVEDOR });
                    table.ForeignKey(
                        name: "FK_T_USUARIO_LOGIN_T_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "T_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_USUARIO_PERFIL",
                columns: table => new
                {
                    ID_USUARIO = table.Column<Guid>(type: "uuid", nullable: false),
                    ID_PERFIL = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USUARIO_PERFIL", x => new { x.ID_USUARIO, x.ID_PERFIL });
                    table.ForeignKey(
                        name: "FK_T_USUARIO_PERFIL_T_PERFIL_ID_PERFIL",
                        column: x => x.ID_PERFIL,
                        principalTable: "T_PERFIL",
                        principalColumn: "ID_PERFIL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_USUARIO_PERFIL_T_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "T_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_USUARIO_TOKEN",
                columns: table => new
                {
                    ID_USUARIO = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_PROVEDOR_LOGIN = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NM_TOKEN = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TX_VALOR_TOKEN = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USUARIO_TOKEN", x => new { x.ID_USUARIO, x.TX_PROVEDOR_LOGIN, x.NM_TOKEN });
                    table.ForeignKey(
                        name: "FK_T_USUARIO_TOKEN_T_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "T_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LOG_DATA_HORA",
                table: "T_LOG",
                column: "DT_DATA_HORA");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_EMAIL_USUARIO",
                table: "T_LOG",
                column: "TX_EMAIL_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_NOME_USUARIO",
                table: "T_LOG",
                column: "NM_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_PERFIL_EXCLUIDO",
                table: "T_PERFIL",
                column: "FL_EXCLUIDO");

            migrationBuilder.CreateIndex(
                name: "IX_PERFIL_NOME_NORMALIZADO",
                table: "T_PERFIL",
                column: "NM_PERFIL_NORMALIZADO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PERFIL_CLAIM_ID_PERFIL",
                table: "T_PERFIL_CLAIM",
                column: "ID_PERFIL");

            migrationBuilder.CreateIndex(
                name: "IX_T_PERMISSAO_CTG_PERMISSAO",
                table: "T_PERMISSAO",
                column: "CTG_PERMISSAO");

            migrationBuilder.CreateIndex(
                name: "IX_T_PERMISSAO_Excluido",
                table: "T_PERMISSAO",
                column: "Excluido");

            migrationBuilder.CreateIndex(
                name: "IX_T_PERMISSAO_NM_PERMISSAO",
                table: "T_PERMISSAO",
                column: "NM_PERMISSAO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POST_CATEGORIA",
                table: "T_POST",
                column: "TX_CATEGORIA");

            migrationBuilder.CreateIndex(
                name: "IX_POST_EXCLUIDO",
                table: "T_POST",
                column: "FL_EXCLUIDO");

            migrationBuilder.CreateIndex(
                name: "IX_POST_PUBLICADO_EM",
                table: "T_POST",
                column: "DT_PUBLICADO_EM");

            migrationBuilder.CreateIndex(
                name: "IX_POST_SLUG",
                table: "T_POST",
                column: "TX_SLUG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_POST_AUTOR_ID",
                table: "T_POST",
                column: "AUTOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_EMAIL",
                table: "T_USUARIO",
                column: "TX_EMAIL");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_EMAIL_NORMALIZADO",
                table: "T_USUARIO",
                column: "TX_EMAIL_NORMALIZADO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_EXCLUIDO",
                table: "T_USUARIO",
                column: "FL_EXCLUIDO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_NOME_NORMALIZADO",
                table: "T_USUARIO",
                column: "NM_USUARIO_NORMALIZADO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_CLAIM_ID_USUARIO",
                table: "T_USUARIO_CLAIM",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_LOGIN_ID_USUARIO",
                table: "T_USUARIO_LOGIN",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_PERFIL_ID_PERFIL",
                table: "T_USUARIO_PERFIL",
                column: "ID_PERFIL");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_PERFIL_ID_USUARIO",
                table: "T_USUARIO_PERFIL",
                column: "ID_USUARIO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_LOG");

            migrationBuilder.DropTable(
                name: "T_PERFIL_CLAIM");

            migrationBuilder.DropTable(
                name: "T_PERMISSAO");

            migrationBuilder.DropTable(
                name: "T_POST");

            migrationBuilder.DropTable(
                name: "T_USUARIO_CLAIM");

            migrationBuilder.DropTable(
                name: "T_USUARIO_LOGIN");

            migrationBuilder.DropTable(
                name: "T_USUARIO_PERFIL");

            migrationBuilder.DropTable(
                name: "T_USUARIO_TOKEN");

            migrationBuilder.DropTable(
                name: "T_PERFIL");

            migrationBuilder.DropTable(
                name: "T_USUARIO");
        }
    }
}
