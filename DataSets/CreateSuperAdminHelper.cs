using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.DataSets;

namespace EduKin.DataSets
{
    /// <summary>
    /// Helper pour créer le Super Admin depuis l'interface
    /// </summary>
    public static class CreateSuperAdminHelper
    {
        /// <summary>
        /// Crée le Super Admin avec confirmation utilisateur
        /// </summary>
        public static async Task CreateWithConfirmation()
        {
            var result = MessageBox.Show(
                "Voulez-vous créer le Super Administrateur système ?\n\n" +
                "Nom: MUMPE BALANDA Jacques\n" +
                "Username: jacques7\n" +
                "Password: sandwiche1991\n" +
                "Rôle: Super Administrateur",
                "Création Super Admin",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var creator = new SuperAdminCreator();
                    bool success = await creator.CreateSuperAdmin();

                    if (success)
                    {
                        MessageBox.Show(
                            "✅ Super Administrateur créé avec succès!\n\n" +
                            "Vous pouvez maintenant vous connecter avec:\n" +
                            "Username: jacques7\n" +
                            "Password: sandwiche1991",
                            "Succès",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "❌ Erreur lors de la création du Super Administrateur.\n" +
                            "Vérifiez les logs pour plus de détails.",
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"❌ Erreur: {ex.Message}",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}