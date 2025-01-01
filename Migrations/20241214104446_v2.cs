using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taphoa.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ma_literal",
                table: "ma_literal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ma_literal",
                table: "ma_literal",
                columns: new[] { "cd_type", "kbn1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ma_literal",
                table: "ma_literal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ma_literal",
                table: "ma_literal",
                column: "cd_type");
        }
    }
}
