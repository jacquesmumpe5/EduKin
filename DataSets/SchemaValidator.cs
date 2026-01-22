using System.Data;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using Dapper;
using System.Text.Json;

namespace EduKin.DataSets
{
    /// <summary>
    /// Validateur et migrateur de schéma pour assurer la compatibilité MySQL/SQLite
    /// </summary>
    public class SchemaValidator
    {
        private readonly Connexion _connexion;
        private readonly Dictionary<string, TableSchema> _expectedSchemas;

        public SchemaValidator()
        {
            _connexion = Connexion.Instance;
            _expectedSchemas = LoadExpectedSchemas();
        }

        /// <summary>
        /// Valide que les schémas MySQL et SQLite sont compatibles
        /// </summary>
        public async Task<SchemaValidationResult> ValidateSchemaCompatibility()
        {
            var result = new SchemaValidationResult();
            
            try
            {
                // Valider le schéma MySQL si disponible
                if (_connexion.IsOnline)
                {
                    var mysqlSchema = await GetMySQLSchema();
                    result.MySQLSchema = mysqlSchema;
                    result.MySQLValidation = ValidateAgainstExpected(mysqlSchema, "MySQL");
                }

                // Valider le schéma SQLite
                var sqliteSchema = await GetSQLiteSchema();
                result.SQLiteSchema = sqliteSchema;
                result.SQLiteValidation = ValidateAgainstExpected(sqliteSchema, "SQLite");

                // Comparer les schémas si les deux sont disponibles
                if (result.MySQLSchema != null && result.SQLiteSchema != null)
                {
                    result.CompatibilityIssues = CompareSchemas(result.MySQLSchema, result.SQLiteSchema);
                }

                result.IsValid = result.MySQLValidation.IsValid && result.SQLiteValidation.IsValid && 
                               result.CompatibilityIssues.Count == 0;
                result.ValidationTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ValidationError = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Migre automatiquement le schéma SQLite pour le rendre compatible avec MySQL
        /// </summary>
        public async Task<SchemaMigrationResult> MigrateSQLiteSchema()
        {
            var result = new SchemaMigrationResult();
            var migrations = new List<string>();

            try
            {
                using var conn = _connexion.GetSQLiteConnection();
                conn.Open();

                using var transaction = conn.BeginTransaction();

                try
                {
                    // Création d'objets DB désactivée - les objets doivent être créés manuellement
                    System.Diagnostics.Debug.WriteLine("Création d'objets DB désactivée - les objets doivent être créés manuellement");

                    transaction.Commit();
                    result.Success = true;
                    result.AppliedMigrations = migrations;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Erreur lors de la migration: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Obtient le schéma MySQL
        /// </summary>
        private async Task<Dictionary<string, TableSchema>> GetMySQLSchema()
        {
            var schema = new Dictionary<string, TableSchema>();

            using var conn = _connexion.GetMySqlConnection();
            conn.Open();

            // Obtenir la liste des tables
            var tables = await conn.QueryAsync<string>("SHOW TABLES");

            foreach (var tableName in tables)
            {
                var tableSchema = new TableSchema { Name = tableName };

                // Obtenir les colonnes
                var columns = await conn.QueryAsync(@"
                    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT, EXTRA
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'ecole_db' AND TABLE_NAME = @TableName
                    ORDER BY ORDINAL_POSITION", new { TableName = tableName });

                foreach (var column in columns)
                {
                    tableSchema.Columns.Add(new ColumnSchema
                    {
                        Name = column.COLUMN_NAME,
                        DataType = column.DATA_TYPE,
                        IsNullable = column.IS_NULLABLE == "YES",
                        DefaultValue = column.COLUMN_DEFAULT,
                        Extra = column.EXTRA
                    });
                }

                // Obtenir les index
                var indexes = await conn.QueryAsync(@"
                    SHOW INDEX FROM " + tableName);

                foreach (var index in indexes)
                {
                    var indexName = index.Key_name;
                    if (!tableSchema.Indexes.ContainsKey(indexName))
                    {
                        tableSchema.Indexes[indexName] = new IndexSchema
                        {
                            Name = indexName,
                            IsUnique = index.Non_unique == 0,
                            Columns = new List<string>()
                        };
                    }
                    tableSchema.Indexes[indexName].Columns.Add(index.Column_name);
                }

                schema[tableName] = tableSchema;
            }

            return schema;
        }

        /// <summary>
        /// Obtient le schéma SQLite
        /// </summary>
        private async Task<Dictionary<string, TableSchema>> GetSQLiteSchema()
        {
            var schema = new Dictionary<string, TableSchema>();

            using var conn = _connexion.GetSQLiteConnection();
            conn.Open();

            // Obtenir la liste des tables
            var tables = await conn.QueryAsync<string>(@"
                SELECT name FROM sqlite_master 
                WHERE type='table' AND name NOT LIKE 'sqlite_%'");

            foreach (var tableName in tables)
            {
                var tableSchema = new TableSchema { Name = tableName };

                // Obtenir les colonnes
                var columns = await conn.QueryAsync($"PRAGMA table_info({tableName})");

                foreach (var column in columns)
                {
                    tableSchema.Columns.Add(new ColumnSchema
                    {
                        Name = column.name,
                        DataType = column.type,
                        IsNullable = column.notnull == 0,
                        DefaultValue = column.dflt_value,
                        IsPrimaryKey = column.pk == 1
                    });
                }

                // Obtenir les index
                var indexes = await conn.QueryAsync($"PRAGMA index_list({tableName})");

                foreach (var index in indexes)
                {
                    var indexInfo = await conn.QueryAsync($"PRAGMA index_info({index.name})");
                    
                    tableSchema.Indexes[index.name] = new IndexSchema
                    {
                        Name = index.name,
                        IsUnique = index.unique == 1,
                        Columns = indexInfo.Select(i => (string)i.name).ToList()
                    };
                }

                schema[tableName] = tableSchema;
            }

            return schema;
        }

        /// <summary>
        /// Valide un schéma contre le schéma attendu
        /// </summary>
        private ValidationResult ValidateAgainstExpected(Dictionary<string, TableSchema> actualSchema, string databaseType)
        {
            var result = new ValidationResult { DatabaseType = databaseType };

            foreach (var expectedTable in _expectedSchemas)
            {
                if (!actualSchema.ContainsKey(expectedTable.Key))
                {
                    result.Issues.Add($"Table manquante: {expectedTable.Key}");
                    continue;
                }

                var actualTable = actualSchema[expectedTable.Key];
                var expectedTableSchema = expectedTable.Value;

                // Vérifier les colonnes
                foreach (var expectedColumn in expectedTableSchema.Columns)
                {
                    var actualColumn = actualTable.Columns.FirstOrDefault(c => c.Name == expectedColumn.Name);
                    if (actualColumn == null)
                    {
                        result.Issues.Add($"Colonne manquante: {expectedTable.Key}.{expectedColumn.Name}");
                    }
                    else
                    {
                        // Vérifier la compatibilité des types
                        if (!AreTypesCompatible(expectedColumn.DataType, actualColumn.DataType, databaseType))
                        {
                            result.Issues.Add($"Type incompatible: {expectedTable.Key}.{expectedColumn.Name} " +
                                            $"(attendu: {expectedColumn.DataType}, actuel: {actualColumn.DataType})");
                        }
                    }
                }
            }

            result.IsValid = result.Issues.Count == 0;
            return result;
        }

        /// <summary>
        /// Compare deux schémas pour détecter les incompatibilités
        /// </summary>
        private List<string> CompareSchemas(Dictionary<string, TableSchema> mysqlSchema, Dictionary<string, TableSchema> sqliteSchema)
        {
            var issues = new List<string>();

            foreach (var mysqlTable in mysqlSchema)
            {
                if (!sqliteSchema.ContainsKey(mysqlTable.Key))
                {
                    issues.Add($"Table {mysqlTable.Key} présente dans MySQL mais absente de SQLite");
                    continue;
                }

                var sqliteTable = sqliteSchema[mysqlTable.Key];

                // Comparer les colonnes
                foreach (var mysqlColumn in mysqlTable.Value.Columns)
                {
                    var sqliteColumn = sqliteTable.Columns.FirstOrDefault(c => c.Name == mysqlColumn.Name);
                    if (sqliteColumn == null)
                    {
                        issues.Add($"Colonne {mysqlTable.Key}.{mysqlColumn.Name} présente dans MySQL mais absente de SQLite");
                    }
                    else if (!AreTypesCompatible(mysqlColumn.DataType, sqliteColumn.DataType, "Cross"))
                    {
                        issues.Add($"Types incompatibles pour {mysqlTable.Key}.{mysqlColumn.Name}: " +
                                 $"MySQL({mysqlColumn.DataType}) vs SQLite({sqliteColumn.DataType})");
                    }
                }
            }

            return issues;
        }

        /// <summary>
        /// Vérifie si deux types de données sont compatibles
        /// </summary>
        private bool AreTypesCompatible(string type1, string type2, string context)
        {
            // Normaliser les types
            type1 = NormalizeDataType(type1, context);
            type2 = NormalizeDataType(type2, context);

            // Mappings de compatibilité
            var compatibilityMap = new Dictionary<string, HashSet<string>>
            {
                { "TEXT", new HashSet<string> { "VARCHAR", "CHAR", "TEXT", "STRING" } },
                { "INTEGER", new HashSet<string> { "INT", "INTEGER", "BIGINT", "SMALLINT", "TINYINT" } },
                { "REAL", new HashSet<string> { "DECIMAL", "FLOAT", "DOUBLE", "REAL", "NUMERIC" } },
                { "DATETIME", new HashSet<string> { "TIMESTAMP", "DATETIME", "DATE", "TIME" } },
                { "BLOB", new HashSet<string> { "BLOB", "BINARY", "VARBINARY" } }
            };

            foreach (var mapping in compatibilityMap)
            {
                if (mapping.Value.Contains(type1.ToUpper()) && mapping.Value.Contains(type2.ToUpper()))
                {
                    return true;
                }
            }

            return type1.Equals(type2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Normalise un type de données
        /// </summary>
        private string NormalizeDataType(string dataType, string context)
        {
            if (string.IsNullOrEmpty(dataType)) return "TEXT";

            dataType = dataType.ToUpper();

            // Supprimer les spécifications de taille
            dataType = System.Text.RegularExpressions.Regex.Replace(dataType, @"\(\d+(?:,\d+)?\)", "");

            // Mappings spécifiques
            var mappings = new Dictionary<string, string>
            {
                { "VARCHAR", "TEXT" },
                { "CHAR", "TEXT" },
                { "LONGTEXT", "TEXT" },
                { "MEDIUMTEXT", "TEXT" },
                { "TINYTEXT", "TEXT" },
                { "INT", "INTEGER" },
                { "BIGINT", "INTEGER" },
                { "SMALLINT", "INTEGER" },
                { "TINYINT", "INTEGER" },
                { "DECIMAL", "REAL" },
                { "FLOAT", "REAL" },
                { "DOUBLE", "REAL" },
                { "TIMESTAMP", "DATETIME" },
                { "DATE", "DATETIME" },
                { "TIME", "DATETIME" }
            };

            return mappings.GetValueOrDefault(dataType, dataType);
        }

        /// <summary>
        /// Charge les schémas attendus depuis la configuration
        /// </summary>
        private Dictionary<string, TableSchema> LoadExpectedSchemas()
        {
            // Pour l'instant, on définit les schémas en dur
            // Dans une version future, ceci pourrait être chargé depuis un fichier JSON
            return new Dictionary<string, TableSchema>
            {
                ["t_eleves"] = new TableSchema
                {
                    Name = "t_eleves",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "matricule", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "nom", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "postnom", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "prenom", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "sexe", DataType = "ENUM", IsNullable = false },
                        new() { Name = "date_naiss", DataType = "DATE", IsNullable = true },
                        new() { Name = "created_at", DataType = "TIMESTAMP", IsNullable = true },
                        new() { Name = "updated_at", DataType = "TIMESTAMP", IsNullable = true }
                    }
                },
                ["t_agents"] = new TableSchema
                {
                    Name = "t_agents",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "matricule", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "nom", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "postnom", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "prenom", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "sexe", DataType = "ENUM", IsNullable = false },
                        new() { Name = "fk_ecole", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "created_at", DataType = "TIMESTAMP", IsNullable = true },
                        new() { Name = "updated_at", DataType = "TIMESTAMP", IsNullable = true }
                    }
                },
                ["t_promotions"] = new TableSchema
                {
                    Name = "t_promotions",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "id_promotion", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "description", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "fk_option", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "created_at", DataType = "TIMESTAMP", IsNullable = true },
                        new() { Name = "updated_at", DataType = "TIMESTAMP", IsNullable = true }
                    }
                },
                ["t_options"] = new TableSchema
                {
                    Name = "t_options",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "id_option", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "description", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "fk_section", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "code_epst", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "created_at", DataType = "TIMESTAMP", IsNullable = true },
                        new() { Name = "updated_at", DataType = "TIMESTAMP", IsNullable = true }
                    }
                },
                ["t_sections"] = new TableSchema
                {
                    Name = "t_sections",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "id_section", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "description", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "etat", DataType = "INT", IsNullable = true },
                        new() { Name = "date_create", DataType = "DATETIME", IsNullable = true }
                    }
                },
                ["t_affectation"] = new TableSchema
                {
                    Name = "t_affectation",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "id_affect", DataType = "INT", IsNullable = false },
                        new() { Name = "fk_matricule_eleve", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "fk_promotion", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "annee_scol", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "indice_promo", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "created_at", DataType = "TIMESTAMP", IsNullable = true },
                        new() { Name = "updated_at", DataType = "TIMESTAMP", IsNullable = true }
                    }
                },
                ["t_affect_sect"] = new TableSchema
                {
                    Name = "t_affect_sect",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "num_affect", DataType = "INT", IsNullable = false },
                        new() { Name = "fk_ecole", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "fk_section", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "date_affect", DataType = "DATETIME", IsNullable = true }
                    }
                },
                ["t_grilles"] = new TableSchema
                {
                    Name = "t_grilles",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "num", DataType = "INT", IsNullable = false },
                        new() { Name = "fk_matricule_eleve", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "periode", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "annee_scol", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "fk_cours", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "intitule", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "cotes", DataType = "DECIMAL", IsNullable = true },
                        new() { Name = "maxima", DataType = "DECIMAL", IsNullable = true },
                        new() { Name = "statut", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "fk_promo", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "indice", DataType = "VARCHAR", IsNullable = true },
                        new() { Name = "created_at", DataType = "TIMESTAMP", IsNullable = true },
                        new() { Name = "updated_at", DataType = "TIMESTAMP", IsNullable = true }
                    }
                },
                ["t_cours"] = new TableSchema
                {
                    Name = "t_cours",
                    Columns = new List<ColumnSchema>
                    {
                        new() { Name = "id_cours", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "intitule", DataType = "VARCHAR", IsNullable = false },
                        new() { Name = "etat", DataType = "INT", IsNullable = true },
                        new() { Name = "date_create", DataType = "DATETIME", IsNullable = true }
                    }
                }
                // Ajouter d'autres tables selon les besoins
            };
        }

        /// <summary>
        /// Obtient les tables manquantes dans SQLite
        /// </summary>
        private async Task<List<string>> GetMissingTables(SQLiteConnection conn)
        {
            var existingTables = await conn.QueryAsync<string>(@"
                SELECT name FROM sqlite_master 
                WHERE type='table' AND name NOT LIKE 'sqlite_%'");

            return _expectedSchemas.Keys.Except(existingTables).ToList();
        }

        /// <summary>
        /// Obtient les colonnes manquantes dans SQLite
        /// </summary>
        private async Task<List<MissingColumn>> GetMissingColumns(SQLiteConnection conn)
        {
            var missingColumns = new List<MissingColumn>();

            foreach (var expectedTable in _expectedSchemas)
            {
                try
                {
                    var existingColumns = await conn.QueryAsync($"PRAGMA table_info({expectedTable.Key})");
                    var existingColumnNames = existingColumns.Select(c => (string)c.name).ToHashSet();

                    foreach (var expectedColumn in expectedTable.Value.Columns)
                    {
                        if (!existingColumnNames.Contains(expectedColumn.Name))
                        {
                            missingColumns.Add(new MissingColumn
                            {
                                TableName = expectedTable.Key,
                                ColumnName = expectedColumn.Name,
                                DataType = ConvertToSQLiteType(expectedColumn.DataType)
                            });
                        }
                    }
                }
                catch
                {
                    // Table n'existe pas, sera créée par GetMissingTables
                }
            }

            return missingColumns;
        }

        /// <summary>
        /// Obtient les index manquants dans SQLite
        /// </summary>
        private async Task<List<MissingIndex>> GetMissingIndexes(SQLiteConnection conn)
        {
            var missingIndexes = new List<MissingIndex>();

            // Index de base recommandés
            var recommendedIndexes = new[]
            {
                new MissingIndex { TableName = "t_eleves", IndexName = "idx_eleves_nom", Columns = new[] { "nom", "prenom" } },
                new MissingIndex { TableName = "t_agents", IndexName = "idx_agents_ecole", Columns = new[] { "fk_ecole" } },
                new MissingIndex { TableName = "t_affectation", IndexName = "idx_affectation_eleve", Columns = new[] { "matricule" } }
            };

            foreach (var recommendedIndex in recommendedIndexes)
            {
                try
                {
                    var existingIndexes = await conn.QueryAsync($"PRAGMA index_list({recommendedIndex.TableName})");
                    var existingIndexNames = existingIndexes.Select(i => (string)i.name).ToHashSet();

                    if (!existingIndexNames.Contains(recommendedIndex.IndexName))
                    {
                        missingIndexes.Add(recommendedIndex);
                    }
                }
                catch
                {
                    // Table n'existe pas
                }
            }

            return missingIndexes;
        }

        /// <summary>
        /// Convertit un type MySQL vers SQLite
        /// </summary>
        private string ConvertToSQLiteType(string mysqlType)
        {
            return mysqlType.ToUpper() switch
            {
                var t when t.StartsWith("VARCHAR") => "TEXT",
                var t when t.StartsWith("CHAR") => "TEXT",
                "TEXT" => "TEXT",
                "LONGTEXT" => "TEXT",
                "MEDIUMTEXT" => "TEXT",
                "TINYTEXT" => "TEXT",
                var t when t.StartsWith("INT") => "INTEGER",
                "BIGINT" => "INTEGER",
                "SMALLINT" => "INTEGER",
                "TINYINT" => "INTEGER",
                var t when t.StartsWith("DECIMAL") => "REAL",
                "FLOAT" => "REAL",
                "DOUBLE" => "REAL",
                "TIMESTAMP" => "DATETIME",
                "DATETIME" => "DATETIME",
                "DATE" => "DATE",
                "TIME" => "TIME",
                var t when t.StartsWith("ENUM") => "TEXT",
                "BLOB" => "BLOB",
                _ => "TEXT"
            };
        }
    }

    #region Classes de support pour la validation de schéma

    public class SchemaValidationResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, TableSchema>? MySQLSchema { get; set; }
        public Dictionary<string, TableSchema>? SQLiteSchema { get; set; }
        public ValidationResult MySQLValidation { get; set; } = new();
        public ValidationResult SQLiteValidation { get; set; } = new();
        public List<string> CompatibilityIssues { get; set; } = new();
        public string? ValidationError { get; set; }
        public DateTime ValidationTime { get; set; }
    }

    public class SchemaMigrationResult
    {
        public bool Success { get; set; }
        public List<string> AppliedMigrations { get; set; } = new();
        public string? Error { get; set; }
        public DateTime MigrationTime { get; set; } = DateTime.Now;
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string DatabaseType { get; set; } = string.Empty;
        public List<string> Issues { get; set; } = new();
    }

    public class TableSchema
    {
        public string Name { get; set; } = string.Empty;
        public List<ColumnSchema> Columns { get; set; } = new();
        public Dictionary<string, IndexSchema> Indexes { get; set; } = new();
    }

    public class ColumnSchema
    {
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsNullable { get; set; }
        public string? DefaultValue { get; set; }
        public string? Extra { get; set; }
        public bool IsPrimaryKey { get; set; }
    }

    public class IndexSchema
    {
        public string Name { get; set; } = string.Empty;
        public bool IsUnique { get; set; }
        public List<string> Columns { get; set; } = new();
    }

    public class MissingColumn
    {
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
    }

    public class MissingIndex
    {
        public string TableName { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string[] Columns { get; set; } = Array.Empty<string>();
        public bool IsUnique { get; set; }
    }

    #endregion
}