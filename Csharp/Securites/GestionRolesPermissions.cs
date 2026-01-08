using Dapper;
using EduKin.DataSets;

namespace EduKin.Csharp.Securites
{
    public class GestionRolesPermissions
    {
        private readonly Connexion _connexion;

        public GestionRolesPermissions()
        {
            _connexion = Connexion.Instance;
        }

        #region CRUD Rôles

        public bool CreateRole(string nomRole, string? description = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_roles (nom_role, description, date_creation) 
                                  VALUES (@NomRole, @Description, @DateCreation)";
                    
                    conn.Execute(query, new 
                    { 
                        NomRole = nomRole,
                        Description = description,
                        DateCreation = DateTime.Now
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création du rôle: {ex.Message}", ex);
            }
        }

        public dynamic? GetRole(int idRole)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_roles WHERE id_role = @IdRole";
                return conn.QueryFirstOrDefault(query, new { IdRole = idRole });
            }
        }

        public dynamic? GetRoleByName(string nomRole)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_roles WHERE nom_role = @NomRole";
                return conn.QueryFirstOrDefault(query, new { NomRole = nomRole });
            }
        }

        public IEnumerable<dynamic> GetAllRoles()
        {
            using (var conn = _connexion.GetConnection())
            {
                return conn.Query("SELECT * FROM t_roles ORDER BY nom_role");
            }
        }

        public bool UpdateRole(int idRole, string nomRole, string? description = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_roles 
                                  SET nom_role = @NomRole, description = @Description 
                                  WHERE id_role = @IdRole";
                    
                    conn.Execute(query, new { IdRole = idRole, NomRole = nomRole, Description = description });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteRole(int idRole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Vérifier si le rôle est utilisé
                    var countQuery = "SELECT COUNT(*) FROM t_users_infos WHERE fk_role = @IdRole";
                    var count = conn.ExecuteScalar<int>(countQuery, new { IdRole = idRole });

                    if (count > 0)
                    {
                        throw new Exception("Ce rôle est attribué à des utilisateurs et ne peut pas être supprimé");
                    }

                    conn.Execute("DELETE FROM t_roles WHERE id_role = @IdRole", new { IdRole = idRole });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Permissions

        public bool CreatePermission(string nomPermission, string? description = null, string? module = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_permissions (nom_permission, description, module, date_creation) 
                                  VALUES (@NomPermission, @Description, @Module, @DateCreation)";
                    
                    conn.Execute(query, new 
                    { 
                        NomPermission = nomPermission,
                        Description = description,
                        Module = module,
                        DateCreation = DateTime.Now
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public dynamic? GetPermission(int idPermission)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_permissions WHERE id_permission = @IdPermission";
                return conn.QueryFirstOrDefault(query, new { IdPermission = idPermission });
            }
        }

        public IEnumerable<dynamic> GetAllPermissions()
        {
            using (var conn = _connexion.GetConnection())
            {
                return conn.Query("SELECT * FROM t_permissions ORDER BY module_concerne, nom_permission");
            }
        }

        public IEnumerable<dynamic> GetPermissionsByModule(string module)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_permissions WHERE module = @Module ORDER BY nom_permission";
                return conn.Query(query, new { Module = module });
            }
        }

        public bool UpdatePermission(int idPermission, string nomPermission, string? description = null, string? module = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_permissions 
                                  SET nom_permission = @NomPermission, description = @Description, module = @Module 
                                  WHERE id_permission = @IdPermission";
                    
                    conn.Execute(query, new 
                    { 
                        IdPermission = idPermission,
                        NomPermission = nomPermission,
                        Description = description,
                        Module = module
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeletePermission(int idPermission)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_permissions WHERE id_permission = @IdPermission", new { IdPermission = idPermission });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region Attribution Rôles aux Utilisateurs

        public bool AssignRoleToUser(string userId, string idRole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Mettre à jour le rôle de l'utilisateur dans t_users_infos
                    var query = @"UPDATE t_users_infos 
                                  SET fk_role = @IdRole, 
                                      updated_at = @UpdatedAt 
                                  WHERE id_user = @UserId";
                    
                    conn.Execute(query, new 
                    { 
                        UserId = userId,
                        IdRole = idRole,
                        UpdatedAt = DateTime.Now
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool RemoveRoleFromUser(string userId)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_users_infos 
                                  SET fk_role = NULL, 
                                      updated_at = @UpdatedAt 
                                  WHERE id_user = @UserId";
                    conn.Execute(query, new { UserId = userId, UpdatedAt = DateTime.Now });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public dynamic? GetUserRole(string userId)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT r.* 
                              FROM t_users_infos u 
                              INNER JOIN t_roles r ON u.fk_role = r.id_role 
                              WHERE u.id_user = @UserId";
                return conn.QueryFirstOrDefault(query, new { UserId = userId });
            }
        }

        public IEnumerable<dynamic> GetUsersByRole(string idRole)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT u.id_user, u.nom, u.postnom, u.prenom, u.role, u.id_ecole, u.created_at 
                              FROM t_users_infos u 
                              WHERE u.fk_role = @IdRole 
                              ORDER BY u.nom, u.postnom";
                return conn.Query(query, new { IdRole = idRole });
            }
        }

        #endregion

        #region Attribution Permissions aux Rôles

        public bool AssignPermissionToRole(int idRole, int idPermission)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Vérifier si l'attribution existe déjà
                    var checkQuery = @"SELECT COUNT(*) FROM t_role_permissions 
                                       WHERE id_role = @IdRole AND id_permission = @IdPermission";
                    var exists = conn.ExecuteScalar<int>(checkQuery, new { IdRole = idRole, IdPermission = idPermission }) > 0;

                    if (exists)
                    {
                        throw new Exception("Cette permission est déjà attribuée à ce rôle");
                    }

                    var query = @"INSERT INTO t_role_permissions (id_role, id_permission, date_attribution) 
                                  VALUES (@IdRole, @IdPermission, @DateAttribution)";
                    
                    conn.Execute(query, new 
                    { 
                        IdRole = idRole,
                        IdPermission = idPermission,
                        DateAttribution = DateTime.Now
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool RemovePermissionFromRole(int idRole, int idPermission)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "DELETE FROM t_role_permissions WHERE id_role = @IdRole AND id_permission = @IdPermission";
                    conn.Execute(query, new { IdRole = idRole, IdPermission = idPermission });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetRolePermissions(int idRole)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT p.*, rp.date_attribution 
                              FROM t_role_permissions rp 
                              INNER JOIN t_permissions p ON rp.id_permission = p.id_permission 
                              WHERE rp.id_role = @IdRole 
                              ORDER BY p.module, p.nom_permission";
                return conn.Query(query, new { IdRole = idRole });
            }
        }

        public IEnumerable<dynamic> GetRolesByPermission(int idPermission)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT r.*, rp.date_attribution 
                              FROM t_role_permissions rp 
                              INNER JOIN t_roles r ON rp.id_role = r.id_role 
                              WHERE rp.id_permission = @IdPermission 
                              ORDER BY r.nom_role";
                return conn.Query(query, new { IdPermission = idPermission });
            }
        }

        #endregion

        #region Vérification des Permissions

        public bool UserHasPermission(string userId, string nomPermission)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COUNT(*) 
                              FROM t_users_infos u 
                              INNER JOIN t_roles_permissions rp ON u.fk_role = rp.fk_role 
                              INNER JOIN t_permissions p ON rp.fk_permission = p.id_permission 
                              WHERE u.id_user = @UserId 
                              AND p.nom_permission = @NomPermission 
                              AND rp.accordee = 1 
                              AND p.etat = 1";
                
                return conn.ExecuteScalar<int>(query, new { UserId = userId, NomPermission = nomPermission }) > 0;
            }
        }

        public bool UserHasRole(string userId, string nomRole)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COUNT(*) 
                              FROM t_users_infos u 
                              INNER JOIN t_roles r ON u.fk_role = r.id_role 
                              WHERE u.id_user = @UserId 
                              AND r.nom_role = @NomRole 
                              AND r.etat = 1";
                
                return conn.ExecuteScalar<int>(query, new { UserId = userId, NomRole = nomRole }) > 0;
            }
        }

        public IEnumerable<dynamic> GetUserPermissions(string userId)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT DISTINCT p.* 
                              FROM t_users_infos u 
                              INNER JOIN t_roles_permissions rp ON u.fk_role = rp.fk_role 
                              INNER JOIN t_permissions p ON rp.fk_permission = p.id_permission 
                              WHERE u.id_user = @UserId 
                              AND rp.accordee = 1 
                              AND p.etat = 1 
                              ORDER BY p.module_concerne, p.nom_permission";
                
                return conn.Query(query, new { UserId = userId });
            }
        }

        public IEnumerable<dynamic> GetUserPermissionsByModule(string userId, string module)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT DISTINCT p.* 
                              FROM t_users_infos u 
                              INNER JOIN t_roles_permissions rp ON u.fk_role = rp.fk_role 
                              INNER JOIN t_permissions p ON rp.fk_permission = p.id_permission 
                              WHERE u.id_user = @UserId 
                              AND p.module_concerne = @Module 
                              AND rp.accordee = 1 
                              AND p.etat = 1 
                              ORDER BY p.nom_permission";
                
                return conn.Query(query, new { UserId = userId, Module = module });
            }
        }

        #endregion

        #region Initialisation des Rôles et Permissions par Défaut

        public void InitializeDefaultRolesAndPermissions()
        {
            try
            {
                // Créer les rôles par défaut
                var roles = new[]
                {
                    new { Nom = "Administrateur", Description = "Accès complet au système" },
                    new { Nom = "Directeur", Description = "Gestion de l'école" },
                    new { Nom = "Enseignant", Description = "Gestion pédagogique" },
                    new { Nom = "Comptable", Description = "Gestion financière" },
                    new { Nom = "Secrétaire", Description = "Gestion administrative" }
                };

                foreach (var role in roles)
                {
                    try
                    {
                        CreateRole(role.Nom, role.Description);
                    }
                    catch { /* Ignorer si existe déjà */ }
                }

                // Créer les permissions par défaut
                var permissions = new[]
                {
                    // Module Administration
                    new { Nom = "ADMIN_GERER_ECOLES", Description = "Gérer les écoles", Module = "Administration" },
                    new { Nom = "ADMIN_GERER_PROMOTIONS", Description = "Gérer les promotions", Module = "Administration" },
                    new { Nom = "ADMIN_GERER_SECTIONS", Description = "Gérer les sections", Module = "Administration" },
                    
                    // Module Élèves
                    new { Nom = "ELEVES_CREER", Description = "Créer des élèves", Module = "Élèves" },
                    new { Nom = "ELEVES_MODIFIER", Description = "Modifier des élèves", Module = "Élèves" },
                    new { Nom = "ELEVES_SUPPRIMER", Description = "Supprimer des élèves", Module = "Élèves" },
                    new { Nom = "ELEVES_CONSULTER", Description = "Consulter les élèves", Module = "Élèves" },
                    
                    // Module Agents
                    new { Nom = "AGENTS_CREER", Description = "Créer des agents", Module = "Agents" },
                    new { Nom = "AGENTS_MODIFIER", Description = "Modifier des agents", Module = "Agents" },
                    new { Nom = "AGENTS_SUPPRIMER", Description = "Supprimer des agents", Module = "Agents" },
                    new { Nom = "AGENTS_CONSULTER", Description = "Consulter les agents", Module = "Agents" },
                    
                    // Module Finances
                    new { Nom = "FINANCES_GERER_PAIEMENTS", Description = "Gérer les paiements", Module = "Finances" },
                    new { Nom = "FINANCES_GERER_FRAIS", Description = "Gérer les frais", Module = "Finances" },
                    new { Nom = "FINANCES_GERER_CAISSE", Description = "Gérer la caisse", Module = "Finances" },
                    new { Nom = "FINANCES_VOIR_RAPPORTS", Description = "Voir les rapports financiers", Module = "Finances" },
                    
                    // Module Pédagogie
                    new { Nom = "PEDAGOGIE_GERER_COURS", Description = "Gérer les cours", Module = "Pédagogie" },
                    new { Nom = "PEDAGOGIE_GERER_GRILLES", Description = "Gérer les grilles", Module = "Pédagogie" },
                    new { Nom = "PEDAGOGIE_GERER_PALMARES", Description = "Gérer les palmarès", Module = "Pédagogie" },
                    
                    // Module Sécurité
                    new { Nom = "SECURITE_GERER_USERS", Description = "Gérer les utilisateurs", Module = "Sécurité" },
                    new { Nom = "SECURITE_GERER_ROLES", Description = "Gérer les rôles", Module = "Sécurité" },
                    new { Nom = "SECURITE_GERER_PERMISSIONS", Description = "Gérer les permissions", Module = "Sécurité" }
                };

                foreach (var permission in permissions)
                {
                    try
                    {
                        CreatePermission(permission.Nom, permission.Description, permission.Module);
                    }
                    catch { /* Ignorer si existe déjà */ }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'initialisation: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
