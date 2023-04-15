using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    internal class Custom_Contract_Resolver: DefaultContractResolver
    {
        public new static readonly Custom_Contract_Resolver Instance = new Custom_Contract_Resolver();

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);
            return contract;
        }
    }
}
