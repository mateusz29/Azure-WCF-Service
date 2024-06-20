using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        bool Create(string login, string haslo);
        [OperationContract]
        Guid Login(string login, string haslo);
        [OperationContract]
        bool Logout(string login);
        [OperationContract]
        bool Put(string nazwa, string tresc, Guid id_sesji);
        [OperationContract]
        string Get(string nazwa, Guid id_sesji);
    }
}
