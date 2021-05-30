BEGIN TRANSACTION;
GO

ALTER TABLE [Courses] DROP CONSTRAINT [FK_Courses_Departments_DepartmentId];
GO

DROP INDEX [IX_Courses_DepartmentId] ON [Courses];
DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'DepartmentId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Courses] ALTER COLUMN [DepartmentId] int NOT NULL;
ALTER TABLE [Courses] ADD DEFAULT 0 FOR [DepartmentId];
CREATE INDEX [IX_Courses_DepartmentId] ON [Courses] ([DepartmentId]);
GO

ALTER TABLE [Courses] ADD CONSTRAINT [FK_Courses_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId]) ON DELETE CASCADE;
GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20201205040655_ver 1.1';
GO

COMMIT;
GO

