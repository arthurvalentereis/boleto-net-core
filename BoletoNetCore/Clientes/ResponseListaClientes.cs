using System.Collections.Generic;
using System;

namespace BoletoNetCore.Clientes
{
    public class ResponseListaClientes
    {
        public string Name { get; set; }
        public string CpfCnpj { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
    }
    public class Customer
    {
        public string Object { get; set; }
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }
        public string AddressNumber { get; set; }
        public string Complement { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string CpfCnpj { get; set; }
        public string PersonType { get; set; }
        public bool Deleted { get; set; }
        public string AdditionalEmails { get; set; }
        public string ExternalReference { get; set; }
        public bool NotificationDisabled { get; set; }
        public string Observations { get; set; }
        public string MunicipalInscription { get; set; }
        public string StateInscription { get; set; }
        public bool CanDelete { get; set; }
        public string CannotBeDeletedReason { get; set; }
        public bool CanEdit { get; set; }
        public string CannotEditReason { get; set; }
        /// <summary>Identificador único da cidade no Asaas. Pode ser null quando não informado.</summary>
        public int? City { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    public class CustomerList
    {
        public string Object { get; set; }
        public bool HasMore { get; set; }
        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<Customer> Data { get; set; }
    }
}
