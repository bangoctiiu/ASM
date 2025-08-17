using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM.Migrations
{
    /// <inheritdoc />
    public partial class hehe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportSlips_Customers_CustomerId",
                table: "ExportSlips");

            migrationBuilder.RenameColumn(
                name: "ExportPrice",
                table: "ExportSlipDetails",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ExportSlips",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ExportSlips",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExportSlipCode",
                table: "ExportSlips",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportSlips_Customers_CustomerId",
                table: "ExportSlips",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportSlips_Customers_CustomerId",
                table: "ExportSlips");

            migrationBuilder.DropColumn(
                name: "ExportSlipCode",
                table: "ExportSlips");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ExportSlipDetails",
                newName: "ExportPrice");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ExportSlips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ExportSlips",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportSlips_Customers_CustomerId",
                table: "ExportSlips",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
