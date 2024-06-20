using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace WCFServiceWebRole1
{
    public class Uzytkownik : TableEntity
    {
        public Uzytkownik(string rk, string pk)
        {
            this.PartitionKey = pk;
            this.RowKey = rk;
        }
        public Uzytkownik() { }
        public string Login { get; set; }
        public string Haslo { get; set; }
        public Guid Id_sesji { get; set; }
    }
}