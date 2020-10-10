CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "ValueItems" (
    "Id" uuid NOT NULL,
    "Key" text NULL,
    "Value" integer NOT NULL,
    CONSTRAINT "PK_ValueItems" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200610091104_InitialMigration', '3.1.5');

