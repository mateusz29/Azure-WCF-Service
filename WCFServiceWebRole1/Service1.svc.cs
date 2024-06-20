using System;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private CloudTableClient tableClient;
        private CloudBlobClient blobClient;

        public Service1()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            this.tableClient = account.CreateCloudTableClient();
            this.blobClient = account.CreateCloudBlobClient();
        }

        private CloudTable GetTableReference(string tableName)
        {
            var table = this.tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        private CloudBlobContainer GetContainerReference(string containerName)
        {
            var container = this.blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            return container;
        }

        public bool Create(string login, string haslo)
        {
            var table = GetTableReference("uzytkownicy");

            var user = new Uzytkownik(login, login) { Login = login, Haslo = haslo, Id_sesji = Guid.Empty };
            var insertOperation = TableOperation.Insert(user);

            try
            {
                table.Execute(insertOperation);
                return true;
            }
            catch (StorageException)
            {
                return false;
            }
        }

        public Guid Login(string login, string haslo)
        {
            var table = GetTableReference("uzytkownicy");

            TableOperation retrieveOperation = TableOperation.Retrieve<Uzytkownik>(login, login);
            var result = table.Execute(retrieveOperation);
            var user = result.Result as Uzytkownik;

            if (user == null || user.Haslo != haslo)
                return Guid.Empty;

            user.Id_sesji = Guid.NewGuid();
            TableOperation updateOperation = TableOperation.Replace(user);
            table.Execute(updateOperation);

            return user.Id_sesji;
        }

        public bool Logout(string login)
        {
            var table = GetTableReference("uzytkownicy");

            TableOperation retrieveOperation = TableOperation.Retrieve<Uzytkownik>(login, login);
            var result = table.Execute(retrieveOperation);
            var user = result.Result as Uzytkownik;

            if (user == null)
                return false;

            user.Id_sesji = Guid.Empty;
            TableOperation updateOperation = TableOperation.Replace(user);
            table.Execute(updateOperation);

            return true;
        }

        public bool Put(string name, string tresc, Guid id_sesji)
        {
            var table = GetTableReference("uzytkownicy");
            var container = GetContainerReference("pliki");

            TableQuery<Uzytkownik> query = new TableQuery<Uzytkownik>().Where(TableQuery.GenerateFilterConditionForGuid("Id_sesji", QueryComparisons.Equal, id_sesji));
            var user = table.ExecuteQuery(query).SingleOrDefault();

            if (user == null)
                return false;

            string blobName = user.Login + name;
            var blob = container.GetBlockBlobReference(blobName);

            var bytes = Encoding.UTF8.GetBytes(tresc);
            using (var stream = new MemoryStream(bytes))
            {
                blob.UploadFromStream(stream);
            }

            return true;
        }

        public string Get(string name, Guid id_sesji)
        {
            var table = GetTableReference("uzytkownicy");
            var container = GetContainerReference("pliki");

            TableQuery<Uzytkownik> query = new TableQuery<Uzytkownik>().Where(TableQuery.GenerateFilterConditionForGuid("Id_sesji", QueryComparisons.Equal, id_sesji));
            var user = table.ExecuteQuery(query).SingleOrDefault();

            if (user == null)
                return "";

            string blobName = user.Login + name;
            var blob = container.GetBlockBlobReference(blobName);

            if (blob == null)
                return "";

            try
            {
                using (var stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (StorageException)
            {
                return "";
            }
        }
    }
}

