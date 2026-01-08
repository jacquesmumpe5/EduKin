using System;
using System.Data.SQLite;
using System.IO;

namespace EduKin.DataSets
{
    /// <summary>
    /// Classe pour initialiser la base de données SQLite avec les vues nécessaires
    /// </summary>
    public class SQLiteInitializer
    {
        private readonly Connexion _connexion;

        public SQLiteInitializer()
        {
            _connexion = Connexion.Instance;
        }

        /// <summary>
        /// Initialise la base de données SQLite avec les vues nécessaires
        /// </summary>
        public bool InitializeViews()
        {
            try
            {
                using (var conn = _connexion.GetSQLiteConnection())
                {
                    conn.Open();

                    // Créer la vue vue_avenue_hierarchie
                    CreateAvenueHierarchieView(conn);

                    // Créer la vue vue_ecole
                    CreateEcoleView(conn);

                    // Créer les tables nécessaires à l'authentification en mode offline
                    CreateAuthTables(conn);

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'initialisation des vues SQLite : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie si les vues existent dans la base de données SQLite
        /// </summary>
        public bool ViewsExist()
        {
            try
            {
                using (var conn = _connexion.GetSQLiteConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT COUNT(*) 
                            FROM sqlite_master 
                            WHERE type='view' 
                            AND name IN ('vue_avenue_hierarchie', 'vue_ecole')";
                        
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 2;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void CreateAvenueHierarchieView(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                // Supprimer la vue si elle existe
                cmd.CommandText = "DROP VIEW IF EXISTS vue_avenue_hierarchie";
                cmd.ExecuteNonQuery();

                // Créer la vue
                cmd.CommandText = @"
                    CREATE VIEW vue_avenue_hierarchie AS
                    SELECT 
                        av.IdEntite AS Id_Avenue,
                        av.IntituleEntite AS Avenue,
                        q.IdEntite AS Id_Quartier,
                        q.IntituleEntite AS Quartier,
                        c.IdEntite AS Id_Commune,
                        c.IntituleEntite AS Commune,
                        v.IdEntite AS Id_Ville,
                        v.IntituleEntite AS Ville,
                        p.IdEntite AS Id_Province,
                        p.IntituleEntite AS Province
                    FROM t_entite_administrative av
                    INNER JOIN t_entite_administrative q ON av.Fk_EntiteMere = q.IdEntite AND q.Fk_TypeEntite = 'TEA00000000092019'
                    INNER JOIN t_entite_administrative c ON q.Fk_EntiteMere = c.IdEntite AND c.Fk_TypeEntite = 'TEA00000000072019'
                    INNER JOIN t_entite_administrative v ON c.Fk_EntiteMere = v.IdEntite AND v.Fk_TypeEntite = 'TEA00000000032019'
                    INNER JOIN t_entite_administrative p ON v.Fk_EntiteMere = p.IdEntite AND p.Fk_TypeEntite = 'TEA00000000022019'
                    WHERE av.Fk_TypeEntite = 'TEA00000000132019'";
                
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateEcoleView(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                // Supprimer la vue si elle existe
                cmd.CommandText = "DROP VIEW IF EXISTS vue_ecole";
                cmd.ExecuteNonQuery();

                // Créer la vue
                cmd.CommandText = @"
                    CREATE VIEW vue_ecole AS
                    SELECT 
                        e.id_ecole,
                        e.denomination AS Ecole,
                        e.logo,
                        e.numero AS NumParcelle,
                        av.IntituleEntite AS Avenue,
                        q.IntituleEntite AS Quartier,
                        c.IntituleEntite AS Commune,
                        v.IntituleEntite AS Ville,
                        p.IntituleEntite AS Province
                    FROM t_ecoles e
                    INNER JOIN t_entite_administrative av ON e.FkAvenue = av.IdEntite AND av.Fk_TypeEntite = 'TEA00000000132019'
                    INNER JOIN t_entite_administrative q ON av.Fk_EntiteMere = q.IdEntite AND q.Fk_TypeEntite = 'TEA00000000092019'
                    INNER JOIN t_entite_administrative c ON q.Fk_EntiteMere = c.IdEntite AND c.Fk_TypeEntite = 'TEA00000000072019'
                    INNER JOIN t_entite_administrative v ON c.Fk_EntiteMere = v.IdEntite AND v.Fk_TypeEntite = 'TEA00000000032019'
                    INNER JOIN t_entite_administrative p ON v.Fk_EntiteMere = p.IdEntite AND p.Fk_TypeEntite = 'TEA00000000022019'";
                
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateAuthTables(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                // Table des rôles
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS t_roles (
                        id_role TEXT PRIMARY KEY,
                        nom_role TEXT NOT NULL,
                        niveau_acces INTEGER DEFAULT 1,
                        etat INTEGER DEFAULT 1
                    )";
                cmd.ExecuteNonQuery();

                // Table des utilisateurs
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS t_users_infos (
                        id_user TEXT PRIMARY KEY,
                        username TEXT UNIQUE,
                        pwd_hash TEXT NOT NULL,
                        user_index INTEGER DEFAULT 0,
                        fk_role TEXT,
                        type_user TEXT DEFAULT 'ECOLE',
                        compte_verrouille INTEGER DEFAULT 0,
                        account_locked_until DATETIME,
                        id_ecole TEXT
                    )";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
