using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OperationInvoker.Contracts.ServiceContracts;
using OperationInvoker.Contracts.DataContracts;

namespace OperationInvoker.Services
{
    public class OrganizitionService : IOrganizitionService
    {
        public Organizitions RetrieveOrganizitions(string alt)
        {
            return this.MockOrganizitionData();
        }

        public Organizitions RetrieveOrganizitionsWithOutput(string alt, out Organizitions output)
        {
            output = this.MockOrganizitionData();
            return this.MockOrganizitionData();
        }

        private Organizitions MockOrganizitionData()
        {
            Organizitions organizitions = new Organizitions()
            {
                ResponseLink = "http://test",
                Organizition = new List<string>()
                {
                    "Organizition1",
                    "Organizition2"
                }
            };

            return organizitions;
        }
    }
}
