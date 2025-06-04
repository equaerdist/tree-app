using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tree_api.Database.Migrations
{
    /// <inheritdoc />
    public partial class searchNodesFunc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
            CREATE OR REPLACE FUNCTION search_trees_and_nodes(searchTerm text, limitCount integer)
            RETURNS TABLE (
                "EntityType" text,
                "Id" bigint,
                "Name" text
            ) AS $$
            BEGIN
                RETURN QUERY
                SELECT 'Tree' AS "EntityType", t."Id", t."Name"::text
                FROM "Trees" t
                WHERE t."Name" ILIKE '%' || searchTerm || '%'
                LIMIT limitCount;

                RETURN QUERY
                SELECT 'Node' AS "EntityType", n."Id", n."Name"::text
                FROM "Nodes" n
                WHERE n."Name" ILIKE '%' || searchTerm || '%'
                LIMIT limitCount;
            END;
            $$ LANGUAGE plpgsql;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP FUNCTION IF EXISTS search_trees_and_nodes(text, integer);"
            );
        }
    }
}