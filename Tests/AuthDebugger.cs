using System;
using System.Threading.Tasks;
using EduKin.DataSets;

namespace EduKin.Tests
{
    public class AuthDebugger
    {
        public static async Task TestAdminFetch()
        {
            var connexion = Connexion.Instance;
            Console.WriteLine($"Connexion Status: {(connexion.IsOnline ? "Online (MySQL)" : "Offline (SQLite)")}");

            using (var conn = connexion.GetConnection())
            {
                conn.Open();
                
                // On simule exactement la requête de FormAuthDialog
                const string sql = @"
                    SELECT 
                        u.id_user,
                        u.username,
                        u.pwd_hash,
                        u.user_index,
                        r.nom_role as fk_role,
                        u.compte_verrouille,
                        u.account_locked_until,
                        r.niveau_acces,
                        u.type_user
                    FROM t_users_infos u
                    LEFT JOIN t_roles r ON u.fk_role = r.id_role
                    WHERE r.niveau_acces >= 8 OR u.type_user = 'SYSTEM'";

                var users = await Dapper.SqlMapper.QueryAsync(conn, sql);

                Console.WriteLine("\n--- Utilisateurs éligibles pour l'AuthDialog ---");
                foreach (var user in users)
                {
                    Console.WriteLine($"Username: {user.username}");
                    Console.WriteLine($"Role: {user.fk_role}");
                    Console.WriteLine($"Level: {user.niveau_acces}");
                    Console.WriteLine($"Type: {user.type_user}");
                    Console.WriteLine($"Locked: {user.compte_verrouille}");
                    Console.WriteLine("--------------------------------------------");
                }

                if (!users.Any())
                {
                    Console.WriteLine("AUCUN Super Admin ou Admin trouvé !");
                }
            }
        }
    }
}
