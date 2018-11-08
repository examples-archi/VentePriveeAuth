using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentePriveeAuth.Entities;

namespace VentePriveeAuth.Services
{
    public class ClientService : IClientService
    {
        // Liste des clients qui peux etre stockee dans une base de donnee
        private List<Client> _clients = new List<Client>
        {
             new Client { Id = 1, AccessKeyId = "VENTEPRIVEE2018", SecretAccessKey = "fhut79YKJUY!REZAOUTZAVCXWCLE" }
        };

        public async Task<Client> Authorisation(string accessKeyId, string secretAccessKey)
        {
            var client = await Task.Run(() => _clients.SingleOrDefault(x => x.AccessKeyId == accessKeyId && x.SecretAccessKey == secretAccessKey));

            // Client introuvable
            if (client == null)
                return null;
            // authorisation avec succée, retourn les details du client sans SecretAccessKey
            client.SecretAccessKey = null;
            return client;
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            return await Task.Run(() => _clients.Select(x =>
            {
                x.SecretAccessKey = null;
                return x;
            }));
        }
    }
    public interface IClientService
    {
        /// <summary>
        /// authentification de client 
        /// </summary>
        /// <param name="accessKeyId">accessKeyId </param>
        /// <param name="secretAccessKey">secretAccessKey</param>
        /// <returns>retourne le client authentifié</returns>
        Task<Client> Authorisation(string accessKeyId, string secretAccessKey);
        /// <summary>
        /// Retourne la liste des clients
        /// </summary>
        /// <returns>liste des clients</returns>
        Task<IEnumerable<Client>> GetAll();
    }
}
