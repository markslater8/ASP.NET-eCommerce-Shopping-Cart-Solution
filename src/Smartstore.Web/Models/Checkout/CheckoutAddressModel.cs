﻿using Smartstore.Web.Modelling;
using Smartstore.Web.Models.Common;
using System.Collections.Generic;

namespace Smartstore.Web.Models.Checkout
{
    public partial class CheckoutAddressModel : ModelBase
    {
        public List<AddressModel> ExistingAddresses { get; set; } = new();

        public AddressModel NewAddress { get; set; } = new();

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool NewAddressPreselected { get; set; }
    }
}
