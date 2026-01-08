using System;
using System.Threading.Tasks;
using Dapper;
using EduKin.DataSets;

namespace EduKin.DataSets
{
    /// <summary>
    /// Classe utilitaire pour créer le Super Administrateur système
    /// </summary>
    public class SuperAdminCreator
    {
        private readonly Connexion _connexion;

        public SuperAdminCreator()
        {
            _connexion = Connexion.Instance;
        }

        /// <summary>
        /// Crée le Super Administrateur système jacques7
        /// </summary>
        public async Task<bool> CreateSuperAdmin()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    // Vérifier si l'utilisateur existe déjà
                    var existingUser = await conn.QueryFirstOrDefaultAsync(
                        "SELECT username FROM t_users_infos WHERE username = @username",
                        new { username = "jacques7" });

                    if (existingUser != null)
                    {
                        Console.WriteLine("L'utilisateur jacques7 existe déjà.");
                        return true;
                    }

                    // Vérifier que le rôle Super Administrateur existe
                    var superAdminRole = await conn.QueryFirstOrDefaultAsync(
                        "SELECT id_role FROM t_roles WHERE nom_role = @role",
                        new { role = "Super Administrateur" });

                    if (superAdminRole == null)
                    {
                        Console.WriteLine("Erreur: Le rôle 'Super Administrateur' n'existe pas dans la base.");
                        return false;
                    }

                    // Générer le hash bcrypt pour le mot de passe
                    string password = "sandwiche1991";
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 10);

                    // Insérer le Super Administrateur
                    var insertQuery = @"
                        INSERT INTO t_users_infos (
                            id_user, nom, postnom, prenom, sexe, username, pwd_hash,
                            telephone, id_ecole, fk_role, type_user, user_index,
                            compte_verrouille, tentatives_connexion, created_at, last_password_change
                        ) VALUES (
                            @IdUser, @Nom, @PostNom, @Prenom, @Sexe, @Username, @PwdHash,
                            @Telephone, @IdEcole, @FkRole, @TypeUser, @UserIndex,
                            @CompteVerrouille, @TentativesConnexion, @CreatedAt, @LastPasswordChange
                        )";

                    var result = await conn.ExecuteAsync(insertQuery, new
                    {
                        IdUser = "USR00100000000012025",
                        Nom = "MUMPE",
                        PostNom = "BALANDA", 
                        Prenom = "Jacques",
                        Sexe = "M",
                        Username = "jacques7",
                        PwdHash = hashedPassword,
                        Telephone = (string)null,
                        IdEcole = (string)null,  // NULL pour utilisateur système
                        FkRole = superAdminRole.id_role,
                        TypeUser = "SYSTEM",
                        UserIndex = 1,
                        CompteVerrouille = 0,
                        TentativesConnexion = 0,
                        CreatedAt = DateTime.Now,
                        LastPasswordChange = DateTime.Now
                    });

                    if (result > 0)
                    {
                        Console.WriteLine("✅ Super Administrateur jacques7 créé avec succès!");
                        Console.WriteLine("   Username: jacques7");
                        Console.WriteLine("   Password: sandwiche1991");
                        Console.WriteLine("   Rôle: Super Administrateur");
                        Console.WriteLine("   Type: SYSTEM");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("❌ Erreur lors de la création de l'utilisateur.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la création du Super Admin: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie si le Super Admin existe et peut se connecter
        /// </summary>
        public async Task<bool> VerifySuperAdmin()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    var query = @"
                        SELECT u.username, u.nom, u.prenom, r.nom_role, u.type_user, u.compte_verrouille
                        FROM t_users_infos u
                        LEFT JOIN t_roles r ON u.fk_role = r.id_role
                        WHERE u.username = @username";

                    var user = await conn.QueryFirstOrDefaultAsync(query, new { username = "jacques7" });

                    if (user != null)
                    {
                        Console.WriteLine("✅ Super Administrateur trouvé:");
                        Console.WriteLine($"   Nom: {user.nom} {user.prenom}");
                        Console.WriteLine($"   Username: {user.username}");
                        Console.WriteLine($"   Rôle: {user.nom_role}");
                        Console.WriteLine($"   Type: {user.type_user}");
                        Console.WriteLine($"   Verrouillé: {(user.compte_verrouille == 1 ? "Oui" : "Non")}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("❌ Super Administrateur jacques7 non trouvé.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la vérification: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Méthode statique pour créer rapidement le Super Admin
        /// </summary>
        public static async Task<bool> CreateDefaultSuperAdmin()
        {
            var creator = new SuperAdminCreator();
            return await creator.CreateSuperAdmin();
        }
    }
}