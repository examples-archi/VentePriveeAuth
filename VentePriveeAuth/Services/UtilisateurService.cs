using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentePriveeAuth.Entities;

namespace VentePriveeAuth.Services
{
    public class UtilisateurService : IUtilisateurService
    {
        /// <summary>
        /// Liste des utilisateurs peut etre stockée dans une base de données 
        /// </summary>
        private List<Utilisateur> _utilisateurs = new List<Utilisateur>
        {
            new  Utilisateur { Id = 1, Email = "benradhouene.habib@gmail.com", Password = "xxxxx@xxxx" }
        };

        public async Task<Utilisateur> Authenticate(string username, string password)
        {
            var utilisateur = await Task.Run(() => _utilisateurs.SingleOrDefault(x => x.Email == username && x.Password == password));

            // retourne null si l'utilisateur est introuvable
            if (utilisateur == null)
                return null;

            // authentification réussi retourne les details d'utilisateur sans le mot de passe
            utilisateur.Password = null;
            return utilisateur;
        }
        /// <summary>
        /// retourne liste des utilisateurs sans les mots de passe
        /// </summary>
        /// <returns>retourne liste des utilisateurs</returns>
        public async Task<IEnumerable<Utilisateur>> GetAll()
        {
            return await Task.Run(() => _utilisateurs.Select(x => {
                x.Password = null;
                return x;
            }));
        }
    }
    public interface IUtilisateurService
    {
        /// <summary>
        /// Fonction d'authentification
        /// </summary>
        /// <param name="username">nom d'utilisateur</param>
        /// <param name="password">mot de passe</param>
        /// <returns>retourne l'entité utilisateur</returns>
        Task<Utilisateur> Authenticate(string username, string password);
        /// <summary>
        /// Fonction de récupération de tous les utilisateurs
        /// </summary>
        /// <returns>retourne la liste des utilisateurs</returns>
        Task<IEnumerable<Utilisateur>> GetAll();
    }
}
