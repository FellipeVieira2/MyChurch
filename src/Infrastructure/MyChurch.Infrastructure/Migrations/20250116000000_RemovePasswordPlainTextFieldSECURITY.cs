using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordPlainTextFieldSECURITY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ?? CRÍTICO DE SEGURANÇA: Remover campo 'password' que armazena senha em texto plano
            // ATENÇÃO: Esta migration irá APAGAR todas as senhas em texto plano
            // Certifique-se de fazer backup antes de executar em produção!
            
            migrationBuilder.DropColumn(
                name: "password",
                schema: "postgres",
                table: "member");
            
            // Nota: O campo 'password_hash' continua existindo e agora armazenará o hash BCrypt
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recriar coluna password (apenas para rollback - NÃO RECOMENDADO!)
            migrationBuilder.AddColumn<string>(
                name: "password",
                schema: "postgres",
                table: "member",
                type: "varchar(500)",
                nullable: true);
        }
    }
}
