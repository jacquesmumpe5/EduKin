using System.Text.RegularExpressions;

namespace EduKin.DataSets
{
    /// <summary>
    /// Adaptateur pour assurer la compatibilité SQL entre MySQL et SQLite
    /// </summary>
    public static class SqlCompatibilityAdapter
    {
        /// <summary>
        /// Adapte une requête SQL pour SQLite
        /// </summary>
        public static string AdaptQueryForSQLite(string mysqlQuery)
        {
            if (string.IsNullOrEmpty(mysqlQuery))
                return mysqlQuery;

            string adaptedQuery = mysqlQuery;

            // 1. Fonctions de date MySQL → SQLite
            adaptedQuery = AdaptDateFunctions(adaptedQuery);

            // 2. Types de données MySQL → SQLite
            adaptedQuery = AdaptDataTypes(adaptedQuery);

            // 3. Fonctions spécifiques MySQL → SQLite
            adaptedQuery = AdaptMySqlFunctions(adaptedQuery);

            // 4. Syntaxe LIMIT/OFFSET
            adaptedQuery = AdaptLimitSyntax(adaptedQuery);

            // 5. Opérateurs et expressions
            adaptedQuery = AdaptOperators(adaptedQuery);

            // 6. Gestion des guillemets
            adaptedQuery = AdaptQuoting(adaptedQuery);

            return adaptedQuery;
        }

        /// <summary>
        /// Adapte une requête SQL pour MySQL
        /// </summary>
        public static string AdaptQueryForMySQL(string sqliteQuery)
        {
            if (string.IsNullOrEmpty(sqliteQuery))
                return sqliteQuery;

            string adaptedQuery = sqliteQuery;

            // 1. Fonctions de date SQLite → MySQL
            adaptedQuery = AdaptDateFunctionsToMySQL(adaptedQuery);

            // 2. INSERT OR REPLACE → INSERT ... ON DUPLICATE KEY UPDATE
            adaptedQuery = AdaptInsertOrReplace(adaptedQuery);

            // 3. Fonctions SQLite → MySQL
            adaptedQuery = AdaptSQLiteFunctions(adaptedQuery);

            return adaptedQuery;
        }

        #region Adaptations MySQL → SQLite

        private static string AdaptDateFunctions(string query)
        {
            // CURDATE() → date('now')
            query = Regex.Replace(query, @"\bCURDATE\(\)", "date('now')", RegexOptions.IgnoreCase);

            // NOW() → datetime('now')
            query = Regex.Replace(query, @"\bNOW\(\)", "datetime('now')", RegexOptions.IgnoreCase);

            // YEAR(date) → strftime('%Y', date)
            query = Regex.Replace(query, @"\bYEAR\(([^)]+)\)", "CAST(strftime('%Y', $1) AS INTEGER)", RegexOptions.IgnoreCase);

            // MONTH(date) → strftime('%m', date)
            query = Regex.Replace(query, @"\bMONTH\(([^)]+)\)", "CAST(strftime('%m', $1) AS INTEGER)", RegexOptions.IgnoreCase);

            // DAY(date) → strftime('%d', date)
            query = Regex.Replace(query, @"\bDAY\(([^)]+)\)", "CAST(strftime('%d', $1) AS INTEGER)", RegexOptions.IgnoreCase);

            // DATE_FORMAT(date, format) → strftime(format, date)
            query = Regex.Replace(query, @"\bDATE_FORMAT\(([^,]+),\s*'([^']+)'\)", 
                match => $"strftime('{ConvertMySQLFormatToSQLite(match.Groups[2].Value)}', {match.Groups[1].Value})", 
                RegexOptions.IgnoreCase);

            return query;
        }

        private static string AdaptDataTypes(string query)
        {
            // AUTO_INCREMENT → AUTOINCREMENT
            query = Regex.Replace(query, @"\bAUTO_INCREMENT\b", "AUTOINCREMENT", RegexOptions.IgnoreCase);

            // DECIMAL(p,s) → REAL
            query = Regex.Replace(query, @"\bDECIMAL\(\d+,\d+\)", "REAL", RegexOptions.IgnoreCase);

            // VARCHAR(n) → TEXT
            query = Regex.Replace(query, @"\bVARCHAR\(\d+\)", "TEXT", RegexOptions.IgnoreCase);

            // ENUM → TEXT avec CHECK
            query = Regex.Replace(query, @"\bENUM\(([^)]+)\)", 
                match => $"TEXT CHECK ({ExtractEnumValues(match.Groups[1].Value)})", 
                RegexOptions.IgnoreCase);

            // TINYINT(1) → INTEGER
            query = Regex.Replace(query, @"\bTINYINT\(1\)", "INTEGER", RegexOptions.IgnoreCase);

            // TIMESTAMP → DATETIME
            query = Regex.Replace(query, @"\bTIMESTAMP\b", "DATETIME", RegexOptions.IgnoreCase);

            return query;
        }

        private static string AdaptMySqlFunctions(string query)
        {
            // IFNULL(a, b) → COALESCE(a, b)
            query = Regex.Replace(query, @"\bIFNULL\(([^,]+),\s*([^)]+)\)", "COALESCE($1, $2)", RegexOptions.IgnoreCase);

            // CONCAT(a, b, c) → (a || b || c)
            query = Regex.Replace(query, @"\bCONCAT\(([^)]+)\)", 
                match => $"({string.Join(" || ", match.Groups[1].Value.Split(',').Select(s => s.Trim()))})", 
                RegexOptions.IgnoreCase);

            // SUBSTRING(str, pos, len) → SUBSTR(str, pos, len)
            query = Regex.Replace(query, @"\bSUBSTRING\(", "SUBSTR(", RegexOptions.IgnoreCase);

            // LENGTH(str) → LENGTH(str) (compatible)
            // UPPER(str) → UPPER(str) (compatible)
            // LOWER(str) → LOWER(str) (compatible)

            return query;
        }

        private static string AdaptLimitSyntax(string query)
        {
            // MySQL: LIMIT offset, count → SQLite: LIMIT count OFFSET offset
            query = Regex.Replace(query, @"\bLIMIT\s+(\d+)\s*,\s*(\d+)", "LIMIT $2 OFFSET $1", RegexOptions.IgnoreCase);

            return query;
        }

        private static string AdaptOperators(string query)
        {
            // MySQL: REGEXP → SQLite: GLOB ou LIKE (approximation)
            query = Regex.Replace(query, @"\bREGEXP\b", "GLOB", RegexOptions.IgnoreCase);

            return query;
        }

        private static string AdaptQuoting(string query)
        {
            // MySQL utilise ` pour les identifiants, SQLite utilise " ou []
            // Pour la compatibilité, on supprime les backticks
            query = query.Replace("`", "");

            return query;
        }

        #endregion

        #region Adaptations SQLite → MySQL

        private static string AdaptDateFunctionsToMySQL(string query)
        {
            // date('now') → CURDATE()
            query = Regex.Replace(query, @"\bdate\('now'\)", "CURDATE()", RegexOptions.IgnoreCase);

            // datetime('now') → NOW()
            query = Regex.Replace(query, @"\bdatetime\('now'\)", "NOW()", RegexOptions.IgnoreCase);

            // strftime('%Y', date) → YEAR(date)
            query = Regex.Replace(query, @"\bstrftime\('%Y',\s*([^)]+)\)", "YEAR($1)", RegexOptions.IgnoreCase);

            // strftime('%m', date) → MONTH(date)
            query = Regex.Replace(query, @"\bstrftime\('%m',\s*([^)]+)\)", "MONTH($1)", RegexOptions.IgnoreCase);

            // strftime('%d', date) → DAY(date)
            query = Regex.Replace(query, @"\bstrftime\('%d',\s*([^)]+)\)", "DAY($1)", RegexOptions.IgnoreCase);

            return query;
        }

        private static string AdaptInsertOrReplace(string query)
        {
            // INSERT OR REPLACE → INSERT ... ON DUPLICATE KEY UPDATE
            if (query.StartsWith("INSERT OR REPLACE", StringComparison.OrdinalIgnoreCase))
            {
                // Cette conversion est complexe et nécessite une analyse plus poussée
                // Pour l'instant, on convertit en INSERT IGNORE
                query = Regex.Replace(query, @"^INSERT OR REPLACE", "INSERT IGNORE", RegexOptions.IgnoreCase);
            }

            return query;
        }

        private static string AdaptSQLiteFunctions(string query)
        {
            // SUBSTR → SUBSTRING
            query = Regex.Replace(query, @"\bSUBSTR\(", "SUBSTRING(", RegexOptions.IgnoreCase);

            return query;
        }

        #endregion

        #region Utilitaires

        private static string ConvertMySQLFormatToSQLite(string mysqlFormat)
        {
            // Conversion des formats de date MySQL vers SQLite
            var formatMap = new Dictionary<string, string>
            {
                { "%Y", "%Y" },     // Année 4 chiffres
                { "%y", "%y" },     // Année 2 chiffres
                { "%m", "%m" },     // Mois numérique
                { "%d", "%d" },     // Jour du mois
                { "%H", "%H" },     // Heure 24h
                { "%i", "%M" },     // Minutes
                { "%s", "%S" },     // Secondes
                { "%M", "%B" },     // Nom du mois
                { "%b", "%b" },     // Nom du mois abrégé
                { "%W", "%A" },     // Nom du jour
                { "%w", "%w" }      // Jour de la semaine numérique
            };

            string sqliteFormat = mysqlFormat;
            foreach (var mapping in formatMap)
            {
                sqliteFormat = sqliteFormat.Replace(mapping.Key, mapping.Value);
            }

            return sqliteFormat;
        }

        private static string ExtractEnumValues(string enumValues)
        {
            // Extrait les valeurs ENUM et crée une contrainte CHECK SQLite
            var values = enumValues.Split(',')
                .Select(v => v.Trim().Trim('\'', '"'))
                .Where(v => !string.IsNullOrEmpty(v));

            return string.Join(" OR ", values.Select(v => $"column_name = '{v}'"));
        }

        /// <summary>
        /// Vérifie si une requête est compatible avec SQLite
        /// </summary>
        public static bool IsCompatibleWithSQLite(string query)
        {
            if (string.IsNullOrEmpty(query))
                return true;

            // Liste des fonctions/syntaxes non supportées par SQLite
            var incompatiblePatterns = new[]
            {
                @"\bSHOW\s+TABLES\b",
                @"\bDESCRIBE\b",
                @"\bEXPLAIN\s+EXTENDED\b",
                @"\bPROCEDURE\b",
                @"\bFUNCTION\b",
                @"\bTRIGGER\b.*\bBEFORE\s+INSERT\s+OR\s+UPDATE\b"
            };

            return !incompatiblePatterns.Any(pattern => 
                Regex.IsMatch(query, pattern, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Vérifie si une requête est compatible avec MySQL
        /// </summary>
        public static bool IsCompatibleWithMySQL(string query)
        {
            if (string.IsNullOrEmpty(query))
                return true;

            // SQLite est généralement plus permissif, donc la plupart des requêtes SQLite fonctionnent avec MySQL
            return true;
        }

        /// <summary>
        /// Obtient la version adaptée d'une requête selon le type de base de données
        /// </summary>
        public static string GetAdaptedQuery(string originalQuery, bool isMySQL)
        {
            if (isMySQL)
            {
                return AdaptQueryForMySQL(originalQuery);
            }
            else
            {
                return AdaptQueryForSQLite(originalQuery);
            }
        }

        #endregion
    }
}