
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class walletandpayment
    {
        public Residents residents { get; set; }
        public IEnumerable<PaymentDetails> payments { get; set; }

    }
}
